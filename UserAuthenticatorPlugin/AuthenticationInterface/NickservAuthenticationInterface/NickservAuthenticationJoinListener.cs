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

using Shinkei.IRC.Events;
using Shinkei.IRC.Entities;

namespace UserAuthenticatorPlugin.AuthenticationInterface.NickservAuthenticationInterface
{
	class NickservAuthenticationJoinListener : IListener
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

		internal NickservAuthenticationJoinListener(NickservAuthenticationInterface NickservAuthenticationInterface)
		{
			this.NickservAuthenticationInterface = NickservAuthenticationInterface;
			this.NickservName = this.NickservAuthenticationInterface.NickservAuthenticationSettings.NickservName;
		}

		[EventHandler]
		public void OnIRCUserJoinEvent(IrcJoinEvent IRCUserJoinEvent)
		{
			EntUser User = (EntUser) IRCUserJoinEvent.Sender;

			if (this.NickservAuthenticationInterface.RunningQueryNicks.ContainsKey(User.Nickname))
				this.NickservAuthenticationInterface.RunningQueryNicks.Remove(User.Nickname);

			this.NickservAuthenticationInterface.RunningQueryNicks.Add(User.Nickname, User);
			var Server = IRCUserJoinEvent.ServerInstance;
			if (!this.NickservAuthenticationInterface.SendNickservInfoRequest(User))
				UserAuthenticatorPlugin.UserAuthenticatorLog.Warn("Could not send a nickserv info request. (Too many requests in the last minute).");
		}
	}
}
