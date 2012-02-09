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
				SorentoLib.Services.Database.Prefix = SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.database_prefix);
				SorentoLib.Services.Database.Name = SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.database_database);

				SorentoLib.Services.Database.Connection = new SNDK.DBI.Connection (SNDK.Enums.DatabaseConnector.Mysql,
				                                                                      SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.database_hostname),
				                                                                      SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.database_database),
				                                                                      SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.database_username),
				                                                                      SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.database_password),
				                                                                      true);
				SorentoLib.Services.Database.Connection.Connect();
			}
			catch  (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SERVICES.DATABASE", exception.Message));

				// LOG: LogFatalError.ServicesDatabaseConnectionFailed
				SorentoLib.Services.Logging.LogFatalError (Strings.LogErrorFatal.ServicesDatabaseConnectionFailed);

				SorentoLib.Runtime.Shutdown ();
			}		

			// LOG: LogInfo.
			SorentoLib.Services.Logging.LogInfo (string.Format (Strings.LogInfo.RuntimeServiceInitialized, "Database"));
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
