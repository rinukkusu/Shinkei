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

using Shinkei.IRC.Events;

namespace UserAuthenticatorPlugin
{
	interface IAuthenticationInterface : IListener
	{
		string Name
		{
			get;
		}

		List<string> Description
		{
			get;
		}
		
		void Initialise(Settings Settings, UserAuthenticatorPlugin Plugin);
		
		bool IsAuthenticated(string IRCUser);
		
		int GetUserID(string IRCUser);
	}
}
