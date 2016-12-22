/*
 *  UserAuthenticatorPlugin - Authenticate and identify users in IRC
 *  Copyright (C) 2016  Shadow1Raven
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *  
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
**/

using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shinkei.IRC.Events;
using Shinkei.API.Events;
using Shinkei.IRC.Entities;
using Shinkei.API.Commands;
using Shinkei.IRC.Commands;

namespace UserAuthenticatorPlugin.AuthenticationInterface.NickservAuthenticationInterface
{
	class NickservEntry
	{
		public int ID
		{
			get;
			set;
		}

		public string NickservGroup
		{
			get;
			set;
		}
	}

	class NickservAuthenticationInterface : IAuthenticationInterface
	{
		#region Fields
		private object _LOCK = new object();
		#endregion

		#region Properties
		public List<string> Description
		{
			get;
		} = new List<string>() { "An identification system based on NickServ.", "Solely identifies you based on communication with NickServ.", "This should not require further action by the user." };

		public string Name
		{
			get;
		} = "NickservAuth";

		internal Dictionary<string, NickservEntry> LoggedInUsers
		{
			get;
			set;
		} = new Dictionary<string, NickservEntry>();

		internal List<NickservEntry> NickservEntries
		{
			get;
			set;
		}

		internal List<string> ExceptedNickservGroups
		{
			get;
			set;
		}

		internal NickservAuthenticationSettings NickservAuthenticationSettings
		{
			get;
			private set;
		}

		internal Dictionary<string, EntUser> RunningQueryNicks
		{
			get;
		} = new Dictionary<string, EntUser>();

		internal UserAuthenticatorPlugin UserAuthenticatorPlugin
		{
			get;
			private set;
		}

		private Thread NickservRequestsResetThread
		{
			get;
			set;
		}

		private int NickservRequests
		{
			get;
			set;
		}

		private FileInfo NickservAuthenticationConfigurationFile
		{
			get;
			set;
		}

		private FileInfo NickservAuthenticationDatabaseFile
		{
			get;
			set;
		}

		private FileInfo ExceptedNickservGroupsDatabaseFile
		{
			get;
			set;
		}
		#endregion

		public void Initialise(Settings Settings, UserAuthenticatorPlugin Plugin)
		{
			this.UserAuthenticatorPlugin = Plugin;
			this.NickservAuthenticationConfigurationFile = new FileInfo(Path.Combine(Plugin.DataDirectory, "NickservAuthenticationSettings.jsn"));
			this.NickservAuthenticationDatabaseFile = new FileInfo(Path.Combine(Plugin.DataDirectory, "NickservAuthenticationDatabase.jsn"));
			this.ExceptedNickservGroupsDatabaseFile = new FileInfo(Path.Combine(Plugin.DataDirectory, "ExceptedNickservGroups.jsn"));

			Timer Timer = new Timer(this.ResetNickservRequestCount, null, 1000, 1000);

			if (!this.NickservAuthenticationConfigurationFile.Exists)
			{
				using (FileStream FileStream = this.NickservAuthenticationConfigurationFile.Create())
				{
					using (StreamWriter StreamWriter = new StreamWriter(FileStream, UserAuthenticatorPlugin.Encoding))
					{
						this.NickservAuthenticationSettings = new NickservAuthenticationSettings();
						string Json = JsonConvert.SerializeObject(this.NickservAuthenticationSettings, UserAuthenticatorPlugin.JsonSerializerSettings);
						StreamWriter.Write(Json);
					}
				}
			}
			else
			{
				using (FileStream FileStream = this.NickservAuthenticationConfigurationFile.OpenRead())
				{
					using (StreamReader StreamReader = new StreamReader(FileStream, UserAuthenticatorPlugin.Encoding, false))
					{
						string Json = StreamReader.ReadToEnd();
						this.NickservAuthenticationSettings = JsonConvert.DeserializeObject<NickservAuthenticationSettings>(Json);
					}
				}
			}

			if (!this.NickservAuthenticationDatabaseFile.Exists)
			{
				this.NickservEntries = new List<NickservEntry>();
				this.SaveUserDatabase();
			}
			else
			{
				this.LoadUserDatabase();
			}

			if (!this.ExceptedNickservGroupsDatabaseFile.Exists)
			{
				this.ExceptedNickservGroups = new List<string>();
				this.SaveExceptedUsersDatabase();
			}
			else
			{
				this.LoadExceptedUsersDatabase();
			}

			{
				ICommandExecutor NickservAuthenticationUnregisterCommandExecutor = new NickservAuthenticationUnregisterCommand(this);
				Command NickservAuthenticationUnregisterCommand = new Command("nickservauth:unregister", "nickservauth:unregister <nickserv_name>", "Remove your nickserv entry from the plugin's database. (Does not delete you from Nicksev!)", NickservAuthenticationUnregisterCommandExecutor);
				CommandHandler.GetInstance().RegisterCommand(NickservAuthenticationUnregisterCommand, Plugin);
			}

			{
				ICommandExecutor NickservAuthenticationAuthCommandExecutor = new NickservAuthenticationAuthCommand(this);
				Command NickservAuthenticationAuthCommand = new Command("nickservauth:auth", "nickservauth:auth", "Dispatches a reply about your nickserv status.", NickservAuthenticationAuthCommandExecutor);
				CommandHandler.GetInstance().RegisterCommand(NickservAuthenticationAuthCommand, Plugin);
			}

			{
				IListener NickservAuthenticationJoinListener = new NickservAuthenticationJoinListener(this);
				EventManager.GetInstance().RegisterEvents(NickservAuthenticationJoinListener, Plugin);
			}

			{
				IListener NickservAuthenticationMessageListener = new NickservAuthenticationMessageListener(this);
				EventManager.GetInstance().RegisterEvents(NickservAuthenticationMessageListener, Plugin);
			}
		}

		public int GetUserID(string IRCUser)
		{
			var IDs = (from E in this.LoggedInUsers
					   where E.Key.Equals(IRCUser)
					   select E.Value.ID);
			if (IDs.Count() < 1)
				return -1;
			else
				return IDs.First();
		}

		public bool IsAuthenticated(string IRCUser)
		{
			return this.LoggedInUsers.ContainsKey(IRCUser);
		}

		private void ResetNickservRequestCount(object State)
		{
			lock (this._LOCK)
			{
				this.NickservRequests = 0;
			}
		}

		internal bool SendNickservInfoRequest(EntUser User)
		{
			lock (this._LOCK)
			{
				this.NickservRequests++;

				if (this.NickservRequests > this.NickservAuthenticationSettings.MaximumNickservRequestsPerMinute)
				{
					return false;
				}
			}

			User.Server.WriteLine($"PRIVMSG {this.NickservAuthenticationSettings.NickservName} :info {User.Nickname}");
			return true;
		}

		internal void SaveUserDatabase()
		{
			using (FileStream FileStream = this.NickservAuthenticationDatabaseFile.Create())
			{
				using (StreamWriter StreamWriter = new StreamWriter(FileStream, UserAuthenticatorPlugin.Encoding))
				{
					string Json = JsonConvert.SerializeObject(this.NickservEntries, UserAuthenticatorPlugin.JsonSerializerSettings);
					StreamWriter.Write(Json);
				}
			}
		}

		internal void SaveExceptedUsersDatabase()
		{
			using (FileStream FileStream = this.ExceptedNickservGroupsDatabaseFile.Create())
			{
				using (StreamWriter StreamWriter = new StreamWriter(FileStream, UserAuthenticatorPlugin.Encoding))
				{
					string Json = JsonConvert.SerializeObject(this.ExceptedNickservGroups, UserAuthenticatorPlugin.JsonSerializerSettings);
					StreamWriter.Write(Json);
				}
			}
		}

		internal void LoadUserDatabase()
		{
			using (FileStream FileStream = this.NickservAuthenticationDatabaseFile.OpenRead())
			{
				using (StreamReader StreamReader = new StreamReader(FileStream, UserAuthenticatorPlugin.Encoding, false))
				{
					string Json = StreamReader.ReadToEnd();
					this.NickservEntries = JsonConvert.DeserializeObject<List<NickservEntry>>(Json);
				}
			}
		}

		internal void LoadExceptedUsersDatabase()
		{
			using (FileStream FileStream = this.ExceptedNickservGroupsDatabaseFile.OpenRead())
			{
				using (StreamReader StreamReader = new StreamReader(FileStream, UserAuthenticatorPlugin.Encoding, false))
				{
					string Json = StreamReader.ReadToEnd();
					this.ExceptedNickservGroups = JsonConvert.DeserializeObject<List<string>>(Json);
				}
			}
		}
	}
}
