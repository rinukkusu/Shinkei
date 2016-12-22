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
using System.Linq;
using System.Collections.Generic;

using Shinkei.IRC.Events;
using Shinkei.IRC.Entities;

namespace UserAuthenticatorPlugin.AuthenticationInterface.NickservAuthenticationInterface
{
	class NickservAuthenticationMessageListener : IListener
	{
		#region Properties
		internal NickservAuthenticationInterface NickservAuthenticationInterface
		{
			get;
			set;
		}
		#endregion

		internal NickservAuthenticationMessageListener(NickservAuthenticationInterface NickservAuthenticationInterface)
		{
			this.NickservAuthenticationInterface = NickservAuthenticationInterface;
		}

		[Shinkei.IRC.Events.EventHandler]
		public void OnIRCMessageEvent(IrcNoticeEvent IRCNoticeEvent)
		{
			string StartsWithRegisteredString = this.NickservAuthenticationInterface.NickservAuthenticationSettings.StartsWithRegisteredString;
			string StartsWithNotRegisteredString = this.NickservAuthenticationInterface.NickservAuthenticationSettings.StartsWithNotRegisteredString;

			var RunningQueriesNick = (from S in this.NickservAuthenticationInterface.RunningQueryNicks
									  where IRCNoticeEvent.Text.Contains(S.Key)
									  select S).FirstOrDefault();

			if (RunningQueriesNick.Equals(default(KeyValuePair<string, EntUser>)))
				return;

			string CompareNotRegisteredString = string.Format(StartsWithNotRegisteredString, RunningQueriesNick.Key);
			string CompareRegisteredString = string.Format(StartsWithRegisteredString, RunningQueriesNick.Key);

			if (IRCNoticeEvent.Text.StartsWith(CompareRegisteredString))
			{
				string NickservName = IRCNoticeEvent.Text.Substring(CompareRegisteredString.Length);
				if (this.NickservAuthenticationInterface.ExceptedNickservGroups.Contains(NickservName))
					return;

				NickservEntry NickservEntry = (from E in this.NickservAuthenticationInterface.NickservEntries
											   where E.NickservGroup.Equals(NickservName)
											   select E).FirstOrDefault();

				if (NickservEntry == null)
				{
					if (this.NickservAuthenticationInterface.LoggedInUsers.ContainsKey(RunningQueriesNick.Value.Username))
						return;

					Random Random = new Random();
					int ID = Random.Next();
					while (this.NickservAuthenticationInterface.NickservEntries.Count(E => E.ID.Equals(ID)) > 0)
						ID = Random.Next();

					NickservEntry = new NickservEntry();
					NickservEntry.ID = ID;
					NickservEntry.NickservGroup = NickservName;
					this.NickservAuthenticationInterface.NickservEntries.Add(NickservEntry);
					this.NickservAuthenticationInterface.SaveUserDatabase();

					var User = this.NickservAuthenticationInterface.RunningQueryNicks[RunningQueriesNick.Key];
					User.SendMessage("You have been registered into the UserAuth nickserv database.");
					User.SendMessage("Only your Nickserv name (Nickserv Group) has been saved into the database.");
					User.SendMessage("If you want to be removed, consider nickservauth:unregister.");
				}

				var AuthenticatedUser = this.NickservAuthenticationInterface.RunningQueryNicks[RunningQueriesNick.Key];
				this.NickservAuthenticationInterface.LoggedInUsers.Add(AuthenticatedUser.Username, NickservEntry);
				AuthenticatedUser.SendMessage("You have been authenticated using Nickserv.");
			}
			else if (IRCNoticeEvent.Text.Equals(CompareNotRegisteredString))
			{
				this.NickservAuthenticationInterface.RunningQueryNicks.Remove(RunningQueriesNick.Key);
			}
		}
	}
}
