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

using System.Collections.Generic;

using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace UserAuthenticatorPlugin.AuthenticationInterface.CustomAuthenticationInterface
{
	class CustomAuthenticationHelpCommand : ICommandExecutor
	{
		internal enum HelpEntries
		{
			HELP_BEGIN = 1,
			HELP_HELP,
			HELP_LOGIN,
			HELP_LOGOUT,
			HELP_REGISTER,
			HELP_UNREGISTER
		}

		internal static IDictionary<HelpEntries, string> Help
		{
			get;
		} = new Dictionary<HelpEntries, string>();

		static CustomAuthenticationHelpCommand()
		{
			Help.Add(HelpEntries.HELP_BEGIN, "~~~ Custom Authentication - Help ~~~");
			Help.Add(HelpEntries.HELP_HELP, "customauth <command> - Send help for a command.");
			Help.Add(HelpEntries.HELP_LOGIN, "customauth:login <user> <password> - Identify as \"user\".");
			Help.Add(HelpEntries.HELP_LOGOUT, "customauth:logout - De-identify.");
			Help.Add(HelpEntries.HELP_REGISTER, "customauth:register <user> <password> - Register as \"user\".");
			Help.Add(HelpEntries.HELP_UNREGISTER, "customauth:unregister <user> <password> - Unregister \"user\".");
		}

		public bool Execute(Command Command, EntUser Executor, CommandMessage Data)
		{
			if (Data.Arguments.Count < 1)
			{
				foreach (string S in Help.Values)
					Data.SendResponseNotice(S);
			}
			else
			{
				switch (Data.Arguments[0].ToLower())
				{
					case "help":
						Data.SendResponseNotice(Help[HelpEntries.HELP_BEGIN]);
						Data.SendResponseNotice(Help[HelpEntries.HELP_HELP]);
						break;
					case "login":
						Data.SendResponseNotice(Help[HelpEntries.HELP_BEGIN]);
						Data.SendResponseNotice(Help[HelpEntries.HELP_LOGIN]);
						break;
					case "logout":
						Data.SendResponseNotice(Help[HelpEntries.HELP_BEGIN]);
						Data.SendResponseNotice(Help[HelpEntries.HELP_LOGOUT]);
						break;
					case "register":
						Data.SendResponseNotice(Help[HelpEntries.HELP_BEGIN]);
						Data.SendResponseNotice(Help[HelpEntries.HELP_REGISTER]);
						break;
					case "unregister":
						Data.SendResponseNotice(Help[HelpEntries.HELP_BEGIN]);
						Data.SendResponseNotice(Help[HelpEntries.HELP_UNREGISTER]);
						break;
					default:
						foreach (string S in Help.Values)
							Data.SendResponseNotice(S);
						break;
				}
			}

			return true;
		}
	}
}
