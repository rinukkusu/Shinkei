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
using System.Reflection;
using System.ComponentModel.Composition;

using log4net;

using Newtonsoft.Json;

using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.IRC.Commands;

using UserAuthenticatorPlugin.Commands;

namespace UserAuthenticatorPlugin
{
	[Export(typeof(Plugin))]
	[ExportMetadata("Name", "UserAuthenticatorPlugin")]
	[ExportMetadata("Version", "0.0.0.1")]
	[ExportMetadata("Author", "Shadow1Raven")]
	[ExportMetadata("Description", "Limit access to data depending on identities.")]
	class UserAuthenticatorPlugin : Plugin
	{
		#region Fields
		private FileInfo fConfigurationFileInfo;
		#endregion

		#region Properties
		internal static ILog UserAuthenticatorLog
		{
			get;
		} = LogManager.GetLogger(typeof(UserAuthenticatorPlugin));

		internal static JsonSerializerSettings JsonSerializerSettings
		{
			get;
		} = new JsonSerializerSettings()
										{
											DateFormatHandling = DateFormatHandling.IsoDateFormat,
											Formatting = Formatting.Indented,
											StringEscapeHandling = StringEscapeHandling.Default
										};

		internal static System.Text.Encoding Encoding
		{
			get;
		} =	new System.Text.UTF8Encoding(false);

		internal FileInfo ConfigurationFileInfo
		{
			get
			{
				return this.fConfigurationFileInfo;
			}
		}
		
		internal Settings Settings
		{
			get;
			private set;
		}
		
		internal IAuthenticationInterface AuthenticationInterface
		{
			get;
			private set;
		}
		#endregion

		public override void OnEnable()
		{
			this.fConfigurationFileInfo = new FileInfo(Path.Combine(this.DataDirectory, "UserAuthenticatorPlugin.jsn"));
			this.Settings = LoadSettings();

			this.AuthenticationInterface = this.GetAuthenticationInterface(Settings);
			this.AuthenticationInterface.Initialise(Settings, this);

			{
				ICommandExecutor AuthTypeCommandExecutor = new AuthTypeCommandExecutor(this);
				Command AuthTypeCommand = new Command("authtype", "authtype", "Display information about current authentication interface.", AuthTypeCommandExecutor);
				CommandHandler.GetInstance().RegisterCommand(AuthTypeCommand, this);
			}

			{
				ICommandExecutor AuthenticatedCommandExecutor = new AuthenticatedCommand(this);
				Command AuthenticatedCommand = new Command("authenticated", "authenticated", "Display information about your current authentication status.", AuthenticatedCommandExecutor);
				CommandHandler.GetInstance().RegisterCommand(AuthenticatedCommand, this);
			}
		}

		private Settings LoadSettings()
		{
			Settings Settings;

			if (!this.ConfigurationFileInfo.Exists)
			{
				Settings = new Settings();
				using (FileStream FileStream = this.ConfigurationFileInfo.Create())
				{
					using (StreamWriter StreamWriter = new StreamWriter(FileStream, UserAuthenticatorPlugin.Encoding))
					{
						string SerialisedObject = JsonConvert.SerializeObject(Settings, Formatting.Indented);
						StreamWriter.Write(SerialisedObject);
					}
				}
			}
			else
			{
				using (FileStream FileStream = this.ConfigurationFileInfo.OpenRead())
				{
					using (StreamReader StreamReader = new StreamReader(FileStream, UserAuthenticatorPlugin.Encoding, false))
					{
						string SerialisedObject = StreamReader.ReadToEnd();
						Settings = JsonConvert.DeserializeObject<Settings>(SerialisedObject);
					}
				}
			}

			return Settings;
		}
		
		private IAuthenticationInterface GetAuthenticationInterface(Settings Settings)
		{
			IAuthenticationInterface AuthenticationInterface;
			try
			{
				Type Type = Type.GetType(Settings.AuthenticationType, true, true);
				ConstructorInfo ConstructorInfo = Type.GetConstructor(System.Type.EmptyTypes);
				AuthenticationInterface = ((IAuthenticationInterface) ConstructorInfo.Invoke(null));
			}
			catch (Exception E)
			{
				UserAuthenticatorPlugin.UserAuthenticatorLog.Fatal("Could not create authentication interface type.");
				UserAuthenticatorPlugin.UserAuthenticatorLog.Fatal("Plugin initialisation has FAILED.", E);
				return null;
			}

			return AuthenticationInterface;
		}
	}
}
