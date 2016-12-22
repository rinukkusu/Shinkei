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

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace UserAuthenticatorPlugin.AuthenticationInterface
{
	class NoAuthenticationInterface : IAuthenticationInterface
	{
		#region Properties
		public List<string> Description
		{
			get;
		} = new List<string>() { "Does not provide authentication at all.", "Basically just a nick storage." };

		public string Name
		{
			get;
		} = "NoAuthentication";

		private FileInfo NoAuthenticationDatabaseFile
		{
			get;
			set;
		}

		private IDictionary<int, string> NoAuthenticationDatabase
		{
			get;
			set;
		} = new Dictionary<int, string>();
		#endregion

		public int GetUserID(string IRCUser)
		{
			var IDs = (from E in this.NoAuthenticationDatabase
					   where E.Value.Equals(IRCUser)
					   select E.Key);

			int ID;
			if (IDs.Count() < 1)
			{
				Random Random = new Random();
				ID = Random.Next();
				while (this.NoAuthenticationDatabase.ContainsKey(ID))
					ID = Random.Next();

				this.NoAuthenticationDatabase.Add(ID, IRCUser);
				this.SaveDatabase();
			}
			else
			{
				ID = IDs.First();
			}

			return ID;
		}

		public void Initialise(Settings Settings, UserAuthenticatorPlugin Plugin)
		{
			this.NoAuthenticationDatabaseFile = new FileInfo(Path.Combine(Plugin.DataDirectory, "NoAuthenticationDatabase.jsn"));
			if (!this.NoAuthenticationDatabaseFile.Exists)
			{
				this.SaveDatabase();
			}
			else
			{
				this.LoadDatabase();
			}
		}

		public bool IsAuthenticated(string IRCUser)
		{
			return true;
		}

		internal void SaveDatabase()
		{
			using (FileStream FileStream = this.NoAuthenticationDatabaseFile.Create())
			{
				using (StreamWriter StreamWriter = new StreamWriter(FileStream, UserAuthenticatorPlugin.Encoding))
				{
					string Json = JsonConvert.SerializeObject(this.NoAuthenticationDatabase, UserAuthenticatorPlugin.JsonSerializerSettings);
					StreamWriter.Write(Json);
				}
			}
		}

		internal void LoadDatabase()
		{
			using (FileStream FileStream = this.NoAuthenticationDatabaseFile.OpenRead())
			{
				using (StreamReader StreamReader = new StreamReader(FileStream, UserAuthenticatorPlugin.Encoding, false))
				{
					string Json = StreamReader.ReadToEnd();
					this.NoAuthenticationDatabase = JsonConvert.DeserializeObject<Dictionary<int, string>>(Json);
				}
			}
		}
	}
}
