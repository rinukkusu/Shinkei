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
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json;

using Shinkei.IRC.Entities;
using Shinkei.API.Commands;
using Shinkei.IRC.Commands;

namespace UserAuthenticatorPlugin.AuthenticationInterface.CustomAuthenticationInterface
{
	internal class CustomAuthenticationInterface : IAuthenticationInterface
	{
		#region Containers
		internal class RegisteredUser
		{
			public int ID
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public CustomAuthenticationInterfaceSettings.HASH_ALGORITHM HashAlgorithm
			{
				get;
				set;
			}

			public string HashedPassword
			{
				get;
				set;
			}

			public string Salt
			{
				get;
				set;
			}
		}
		#endregion

		public const int NOT_LOGGED_IN = -1;
		public const int NOT_REGISTERED_ID = -2;

		#region Properties
		public List<string> Description
		{
			get;
		} = new List<string>() { "A custom authentication algorithm.", "Uses a nickname and a password.", "For further information, use the command \"customauth\"." };

		public string Name
		{
			get;
		} = "CustomAuth";

		private FileInfo ConfigurationFileInfo
		{
			get;
			set;
		}

		private FileInfo DatabaseFileInfo
		{
			get;
			set;
		}

		internal CustomAuthenticationInterfaceSettings CustomAuthenticationInterfaceSettings
		{
			get;
			set;
		}

		private IList<RegisteredUser> RegisteredUsers
		{
			get;
			set;
		}

		internal IDictionary<string, int> LoggedInUsers
		{
			get;
		} = new Dictionary<string, int>();

		private RNGCryptoServiceProvider RNGCryptoServiceProvider
		{
			get;
		} = new RNGCryptoServiceProvider();
		#endregion

		public void Initialise(Settings Settings, UserAuthenticatorPlugin Plugin)
		{
			this.ConfigurationFileInfo = new FileInfo(Path.Combine(Plugin.DataDirectory, "CustomAuthenticationInterface.jsn"));

			if (!this.ConfigurationFileInfo.Exists)
			{
				this.CustomAuthenticationInterfaceSettings = new CustomAuthenticationInterfaceSettings();
				string JsonString = JsonConvert.SerializeObject(CustomAuthenticationInterfaceSettings, UserAuthenticatorPlugin.JsonSerializerSettings);
				using (FileStream FileStream = this.ConfigurationFileInfo.Create())
				{
					using (StreamWriter StreamWriter = new StreamWriter(FileStream, UserAuthenticatorPlugin.Encoding))
					{
						StreamWriter.Write(JsonString);
					}
				}
			}
			else
			{
				using (FileStream FileStream = this.ConfigurationFileInfo.OpenRead())
				{
					using (StreamReader StreamReader = new StreamReader(FileStream, UserAuthenticatorPlugin.Encoding, false))
					{
						string Json = StreamReader.ReadToEnd();
						this.CustomAuthenticationInterfaceSettings = JsonConvert.DeserializeObject<CustomAuthenticationInterfaceSettings>(Json, UserAuthenticatorPlugin.JsonSerializerSettings);
					}
				}
			}

			this.DatabaseFileInfo = new FileInfo(Path.Combine(Plugin.DataDirectory, "CustomAuthenticationInterfaceDatabase.jsn"));
			if (!this.DatabaseFileInfo.Exists)
			{
				this.SaveList();
			}
			else
			{
				try
				{
					LoadList();
				}
				catch
				{
					this.RegisteredUsers = new List<RegisteredUser>();
				}
			}

			{
				ICommandExecutor CustomAuthenticationHelpCommandExecutor = new CustomAuthenticationHelpCommand();
				Command CustomAuthenticationHelpCommand = new Command("customauth", "customauth [help <command>]", "Display information about the custom authentication system", CustomAuthenticationHelpCommandExecutor);
				CommandHandler.GetInstance().RegisterCommand(CustomAuthenticationHelpCommand, Plugin);
			}

			{
				ICommandExecutor CustomAuthenticationRegisterCommandExecutor = new CustomAuthenticationRegisterCommand(this);
				Command CustomAuthenticationRegisterCommand = new Command("customauth:register", "customauth:register <user> <password>", "Register into the custom authentication system.", CustomAuthenticationRegisterCommandExecutor);
				CommandHandler.GetInstance().RegisterCommand(CustomAuthenticationRegisterCommand, Plugin);
			}

			{
				ICommandExecutor CustomAuthenticationLoginCommandExecutor = new CustomAuthenticationLoginCommand(this);
				Command CustomAuthenticationLoginCommand = new Command("customauth:login", "customauth:login <user> <password>", "Identify as \"user\" using \"password\".", CustomAuthenticationLoginCommandExecutor);
				CommandHandler.GetInstance().RegisterCommand(CustomAuthenticationLoginCommand, Plugin);
			}

			{
				ICommandExecutor CustomAuthenticationLogoutCommandExecutor = new CustomAuthenticationLogoutCommand(this);
				Command CustomAuthenticationLogoutCommand = new Command("customauth:logout", "customauth:logout", "Log out.", CustomAuthenticationLogoutCommandExecutor);
				CommandHandler.GetInstance().RegisterCommand(CustomAuthenticationLogoutCommand, Plugin);
			}

			{
				ICommandExecutor CustomAuthenticationUnregisterCommandExecutor = new CustomAuthenticationUnregisterCommand(this);
				Command CustomAuthenticationUnregisterCommand = new Command("customauth:unregister", "customauth:unregister <user> <password>", "Delete \"user\" from the database.", CustomAuthenticationUnregisterCommandExecutor);
				CommandHandler.GetInstance().RegisterCommand(CustomAuthenticationUnregisterCommand, Plugin);
			}
		}

		public int GetUserID(string IRCUser)
		{
			var IDs = (from ID in this.LoggedInUsers
					  where ID.Key.Equals(IRCUser)
					  select ID.Value);
			if (IDs.Count() < 1)
				return CustomAuthenticationInterface.NOT_LOGGED_IN;
			else
				return IDs.First();
		}

		public bool IsAuthenticated(string IRCUser)
		{
			return this.LoggedInUsers.ContainsKey(IRCUser);
		}
		
		private void LoadList()
		{
			using (FileStream FileStream = this.DatabaseFileInfo.OpenRead())
			{
				using (StreamReader StreamReader = new StreamReader(FileStream, UserAuthenticatorPlugin.Encoding, false))
				{
					string SerialisedList = StreamReader.ReadToEnd();
					List<RegisteredUser> List = JsonConvert.DeserializeObject<List<RegisteredUser>>(SerialisedList, UserAuthenticatorPlugin.JsonSerializerSettings);
					this.RegisteredUsers = List;
				}
			}
		}

		private void SaveList()
		{
			using (FileStream FileStream = this.DatabaseFileInfo.Open(FileMode.Create))
			{
				using (StreamWriter StreamWriter = new StreamWriter(FileStream, UserAuthenticatorPlugin.Encoding))
				{
					string SerialisedList = JsonConvert.SerializeObject(this.RegisteredUsers, UserAuthenticatorPlugin.JsonSerializerSettings);
					StreamWriter.Write(SerialisedList);
				}
			}
		}

		internal int GetNewID()
		{
			if (this.RegisteredUsers.Count() < 1)
				return 1;
			else
				return this.RegisteredUsers.Max(U => U.ID) + 1;
		}

		internal void AddRegisteredUser(RegisteredUser RegisteredUser)
		{
			this.RegisteredUsers.Add(RegisteredUser);
			this.SaveList();
		}

		internal void RemoveRegisteredUser(RegisteredUser RegisteredUser)
		{
			this.RegisteredUsers.Remove(RegisteredUser);
			this.SaveList();
		}

		internal HashAlgorithm GetHashAlgorithm(CustomAuthenticationInterfaceSettings.HASH_ALGORITHM HashAlgorithmSetting)
		{
			HashAlgorithm HashAlgorithm;
			switch (HashAlgorithmSetting)
			{
				case CustomAuthenticationInterfaceSettings.HASH_ALGORITHM.SHA2_512:
				default:
					HashAlgorithm = new SHA512Managed();
					break;
			}

			return HashAlgorithm;
		}

		internal void CreatePasswordHash(string Pass, CustomAuthenticationInterfaceSettings.HASH_ALGORITHM HashAlgorithmSetting, out string Salt, out string HashedPassword)
		{
			int SaltByteCount = this.CustomAuthenticationInterfaceSettings.SaltByteCount;
			byte[] SaltData = new byte[SaltByteCount];
			this.RNGCryptoServiceProvider.GetBytes(SaltData);
			Salt = System.Convert.ToBase64String(SaltData);
			
			HashedPassword = HashPassword(Pass, Salt, HashAlgorithmSetting);
		}

		internal string HashPassword(string Pass, string Salt, CustomAuthenticationInterfaceSettings.HASH_ALGORITHM HashAlgorithmSetting)
		{
			byte[] PassData = UserAuthenticatorPlugin.Encoding.GetBytes(Pass);
			byte[] SaltData = System.Convert.FromBase64String(Salt);

			List<byte> SaltedPassDataList = new List<byte>();
			SaltedPassDataList.AddRange(PassData);
			SaltedPassDataList.AddRange(SaltData);

			HashAlgorithm HashAlgorithm = GetHashAlgorithm(HashAlgorithmSetting);

			byte[] SaltedPassData = SaltedPassDataList.ToArray();
			byte[] HashedPassword = HashAlgorithm.ComputeHash(SaltedPassData);

			string Hash = System.Convert.ToBase64String(HashedPassword);
			return Hash;
		}

		internal RegisteredUser GetRegisteredUser(string Nick)
		{
			var RegisteredUser = (from U in this.RegisteredUsers
								  where U.Name.Equals(Nick)
								  select U).FirstOrDefault();

			return RegisteredUser;
		}
	}
}
