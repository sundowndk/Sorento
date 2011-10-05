//
// Logging.cs
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

using Mono.Addins;

namespace SorentoLib.Services
{
	public static class Logging
	{
		#region Public Static Fields
		public static Enums.LogLevel Level = Enums.LogLevel.Info | Enums.LogLevel.Warning | Enums.LogLevel.Error | Enums.LogLevel.FatalError;
		#endregion

		#region Private Static Methods
		private static void Write (SorentoLib.Enums.LogLevel LogLevel, string Message)
		{


//		Console.WriteLine (Message);
			try
			{

				if (AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.ILogger)).Length != 0)
				{
					foreach (SorentoLib.Addins.ILogger logger in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.ILogger)))
					{
						logger.Log (LogLevel, Message);
					}
				}
				else
				{
					Console.WriteLine (Message);
				}
			}
			catch
			{
				Console.WriteLine (Message);
			}
		}
		#endregion

		#region Public Static Methods
		public static void Initialize ()
		{
			// Set loglevel
			Level = (Enums.LogLevel)Enum.Parse (typeof (Enums.LogLevel), Services.Config.Get<string> ("core", "loglevel"), true);

			// Initalize all Addins
			foreach (SorentoLib.Addins.ILogger logger in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.ILogger)))
			{
				logger.Initialize ();
			}

			SorentoLib.Services.Logging.LogInfo("Service enabled: Logging");
		}

		public static void LogFatalError (string Message)
		{
			if ((Level & Enums.LogLevel.FatalError) == Enums.LogLevel.FatalError)
			{
				Write (Enums.LogLevel.FatalError, Message);
			}
		}

		public static void LogError (string Message)
		{
			if ((Level & Enums.LogLevel.Warning) == Enums.LogLevel.Warning)
			{
				Write (Enums.LogLevel.Warning, Message);
			}
		}

		public static void LogWarning (string Message)
		{
			if ((Level & Enums.LogLevel.Error) == Enums.LogLevel.Error)
			{
				Write (Enums.LogLevel.Error, Message);
			}
		}

		public static void LogInfo (string Message)
		{
			if ((Level & Enums.LogLevel.Info) == Enums.LogLevel.Info)
			{
				Write (Enums.LogLevel.Info, Message);
			}
		}

		public static void LogDebug (string Message)
		{
			if ((Level & Enums.LogLevel.Debug) == Enums.LogLevel.Debug)
			{
				Write (Enums.LogLevel.Debug, Message);
			}
		}
		#endregion

		#region Internal Static Methods
		internal static void ServiceConfigChanged ()
		{
			Level = (Enums.LogLevel)Enum.Parse (typeof (Enums.LogLevel), Services.Config.Get<string> ("core", "loglevel"), true);
		}
		#endregion
	}
}
