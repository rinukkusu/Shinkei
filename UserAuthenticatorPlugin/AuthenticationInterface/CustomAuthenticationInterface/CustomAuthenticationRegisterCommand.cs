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
using System.Collections.Generic;
using System.Security.Cryptography;

using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace UserAuthenticatorPlugin.AuthenticationInterface.CustomAuthenticationInterface
{
	class CustomAuthenticationRegisterCommand : ICommandExecutor
	{
		#region Properties
		internal CustomAuthenticationInterface CustomAuthenticationInterface
		{
			get;
			set;
		}
		#endregion

		internal CustomAuthenticationRegisterCommand(CustomAuthenticationInterface CustomAuthenticationInterface)
		{
			this.CustomAuthenticationInterface = CustomAuthenticationInterface;
		}

		public bool Execute(Command Command, EntUser Executor, CommandMessage Data)
		{
			if (Data.Arguments.Count < 2)
				return false;
			
			string Nick = Data.Arguments[0];
			string Pass = Data.Arguments[1];
			string Salt;
			string Hash;

			this.CustomAuthenticationInterface.CreatePasswordHash(Pass, this.CustomAuthenticationInterface.CustomAuthenticationInterfaceSettings.DefaultHashAlgorithm, out Salt, out Hash);

			var RegisteredUser = new CustomAuthenticationInterface.RegisteredUser();
			RegisteredUser.ID = this.CustomAuthenticationInterface.GetNewID();
			RegisteredUser.Name = Nick;
			RegisteredUser.HashAlgorithm = this.CustomAuthenticationInterface.CustomAuthenticationInterfaceSettings.DefaultHashAlgorithm;
			RegisteredUser.HashedPassword = Hash;
			RegisteredUser.Salt = Salt;

			if (this.CustomAuthenticationInterface.GetRegisteredUser(RegisteredUser.Name) != null)
			{
				Data.SendResponse($"The nick {RegisteredUser.Name} is already registered.");
				return true;
			}

			this.CustomAuthenticationInterface.AddRegisteredUser(RegisteredUser);
			this.CustomAuthenticationInterface.LoggedInUsers.Add(Executor.Username, RegisteredUser.ID);

			Data.SendResponse("You have been successfully registered.");
			Data.SendResponse("You have been logged in.");
			return true;
		}
	}
}
