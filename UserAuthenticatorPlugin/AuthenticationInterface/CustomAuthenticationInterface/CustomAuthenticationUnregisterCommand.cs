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

using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace UserAuthenticatorPlugin.AuthenticationInterface.CustomAuthenticationInterface
{
	class CustomAuthenticationUnregisterCommand : ICommandExecutor
	{
		private CustomAuthenticationInterface CustomAuthenticationInterface
		{
			get;
			set;
		}

		internal CustomAuthenticationUnregisterCommand(CustomAuthenticationInterface CustomAuthenticationInterface)
		{
			this.CustomAuthenticationInterface = CustomAuthenticationInterface;
		}

		public bool Execute(Command Command, EntUser Executor, CommandMessage Data)
		{
			if (Data.Arguments.Count < 2)
				return false;

			string Nick = Data.Arguments[0];
			string Pass = Data.Arguments[1];
			var RegisteredUser = this.CustomAuthenticationInterface.GetRegisteredUser(Nick);
			if (RegisteredUser == null)
			{
				Data.SendResponse($"{Nick} is not a registered user.");
				return true;
			}

			string HashedPassword = this.CustomAuthenticationInterface.HashPassword(Pass, RegisteredUser.Salt, RegisteredUser.HashAlgorithm);
			if (RegisteredUser.HashedPassword.Equals(HashedPassword))
			{
				if (this.CustomAuthenticationInterface.LoggedInUsers.ContainsKey(Executor.Username))
					this.CustomAuthenticationInterface.LoggedInUsers.Remove(Executor.Username);

				this.CustomAuthenticationInterface.RemoveRegisteredUser(RegisteredUser);
				Data.SendResponse($"{Nick} has been deleted from the database.");
			}
			else
			{
				Data.SendResponse($"Could not identify you as {Nick}");
			}

			return true;
		}
	}
}
