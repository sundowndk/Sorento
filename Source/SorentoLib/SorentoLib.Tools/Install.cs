// 
// Install.cs
//  
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2010 Rasmus Pedersen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using Toolbox.DBI;
using Toolbox.Enums;

namespace SorentoLib.Tools
{
	internal static class Install
	{
		internal static void Application ()
		{

		}
//CREATE TABLE `tester`.`table1` (
//`test` INT NOT NULL
//) ENGINE = MYISAM ;
		internal static void Database ()
		{
			#region SESSION TABLE
			{
				string querystring = string.Empty;
				querystring += "CREATE TABLE '"+ SorentoLib.Services.Database.Name +"'.'"+ SorentoLib.Services.Database.Prefix +"sessions' ";
				querystring += "(";
				querystring += "'id' VARCHAR (50) NOT NULL,";
				querystring += "'createtimestamp' INT NOT NULL,";
				querystring += "'updatetimestamp' INT NOT NULL,";
				querystring += "'userid' VARCHAR (50) NOT NULL,";
				querystring += "'remotehost' VARCHAR (250) NOT NULL,";
				querystring += "'salt' INT NOT NULL";
				querystring += ")";

				Query query = SorentoLib.Services.Database.Connection.Query (querystring);

				if (!query.Success)
				{
					SorentoLib.Services.Logging.LogFatalError ("Could not create SESSIONS table in database.");
				}

				query.Dispose ();
				query = null;
			}
			#endregion
		}
	}
}
