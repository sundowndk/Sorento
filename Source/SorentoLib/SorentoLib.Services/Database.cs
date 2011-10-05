//
// Config.cs
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2009 Rasmus Pedersen
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;

using SNDK.DBI;

namespace SorentoLib.Services
{
	public static class Database
	{
		#region Public Static Fields
		public static SNDK.DBI.Connection Connection;
		public static string Prefix = string.Empty;
		public static string Name = string.Empty;
		#endregion
		
		#region Public Static Methods
		public static void Initalize ()
		{
			try
			{
				SorentoLib.Services.Database.Prefix = SorentoLib.Services.Config.Get<string> ("database", "prefix");
				SorentoLib.Services.Database.Name = SorentoLib.Services.Config.Get<string> ("database", "database");

				SorentoLib.Services.Database.Connection = new SNDK.DBI.Connection (SNDK.Enums.DatabaseConnector.Mysql,
				                                                                      SorentoLib.Services.Config.Get<string> ("database", "hostname"),
				                                                                      SorentoLib.Services.Config.Get<string> ("database", "database"),
				                                                                      SorentoLib.Services.Config.Get<string> ("database", "username"),
				                                                                      SorentoLib.Services.Config.Get<string> ("database", "password"),
				                                                                      true);
				SorentoLib.Services.Database.Connection.Connect();
			}
			catch  (Exception e)
			{
				//SorentoLib.Services.Logging.LogWarning(e.ToString().Replace("\n", ""));
				SorentoLib.Services.Logging.LogFatalError("Faild to establish connetion to database server.");
				SorentoLib.Runtime.Shutdown();				
			}		
										
			SorentoLib.Services.Logging.LogInfo("Service enabled: Database");
		}
		#endregion

		#region Internal Static Methods
		internal static void ServiceConfigChanged ()
		{
			Initalize ();
		}
		#endregion
	}
}
