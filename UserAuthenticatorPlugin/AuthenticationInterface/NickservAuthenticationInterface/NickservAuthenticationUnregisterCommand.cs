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

using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace UserAuthenticatorPlugin.AuthenticationInterface.NickservAuthenticationInterface
{
	class NickservAuthenticationUnregisterCommand : ICommandExecutor
	{
		#region Properties
		internal NickservAuthenticationInterface NickservAuthenticationInterface
		{
			get;
			set;
		}
		#endregion

		public NickservAuthenticationUnregisterCommand(NickservAuthenticationInterface NickservAuthenticationInterface)
		{
			this.NickservAuthenticationInterface = NickservAuthenticationInterface;
		}

		public bool Execute(Command Command, EntUser Executor, CommandMessage Data)
		{
			if (Data.Arguments.Count < 1)
				return false;

			var IDEntries = (from E in this.NickservAuthenticationInterface.NickservEntries
					  where E.NickservGroup.Equals(Data.Arguments[0])
					  select E);

			if (IDEntries.Count() < 1)
			{
				Data.SendResponseNotice("Could not find that nickserv name.");
				Data.SendResponseNotice("Are you registered?");
			}
			else
			{
				NickservEntry NickservEntry = IDEntries.First();
				this.NickservAuthenticationInterface.NickservEntries.Remove(NickservEntry);
				this.NickservAuthenticationInterface.ExceptedNickservGroups.Add(NickservEntry.NickservGroup);

				Data.SendResponseNotice("You have been removed from the database. Your nickserv group was added to the exceptions list.");
				Data.SendResponseNotice("You will not be added to the database anymore. Should you happen to change your mind, please ask an administrator.");
			}

			return true;
		}
	}
}
