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

using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace UserAuthenticatorPlugin.AuthenticationInterface.NickservAuthenticationInterface
{
	class NickservAuthenticationAuthCommand : ICommandExecutor
	{
		#region Properties
		private string NickservName
		{
			get;
			set;
		}
		
		internal NickservAuthenticationInterface NickservAuthenticationInterface
		{
			get;
			set;
		}
		#endregion

		public NickservAuthenticationAuthCommand(NickservAuthenticationInterface NickservAuthenticationInterface)
		{
			this.NickservAuthenticationInterface = NickservAuthenticationInterface;
			this.NickservName = this.NickservAuthenticationInterface.NickservAuthenticationSettings.NickservName;
		}

		public bool Execute(Command Command, EntUser Executor, CommandMessage Data)
		{
			if (this.NickservAuthenticationInterface.RunningQueryNicks.ContainsKey(Executor.Nickname))
				this.NickservAuthenticationInterface.RunningQueryNicks.Remove(Executor.Nickname);

			this.NickservAuthenticationInterface.RunningQueryNicks.Add(Executor.Nickname, Executor);
			if (!this.NickservAuthenticationInterface.SendNickservInfoRequest(Executor))
			{
				Data.SendResponseNotice("Could not query nickserv due to high load.");
				UserAuthenticatorPlugin.UserAuthenticatorLog.Warn("Could not send a nickserv info request. (Too many requests in the last minute).");
			}
			else
			{
				Data.SendResponseNotice("You will soon receive information about your authentication status.");
			}

			return true;
		}
	}
}
