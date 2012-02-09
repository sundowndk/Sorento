//
// SorentoMain.cs
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

using SorentoLib;

namespace Sorento.Startup
{
	public class SorentoMain
	{
		public static void Main ()
		{
			// Set process name.
			SorentoLib.Runtime.SetProcessName ("Sorento");

			// Initialize config.
			SorentoLib.Services.Config.Initialize ();

			// Initalize addins.
			SorentoLib.Services.Addins.Initialize ();

			// Initalize logging.
			SorentoLib.Services.Logging.Initialize ();

			// Initialize database.
			SorentoLib.Services.Database.Initalize ();

			// Initialize crypto.
			SorentoLib.Services.Crypto.Initalize ();

			// Initialize datastore
			SorentoLib.Services.Datastore.Initalize ();

			// Initialize Stats
			SorentoLib.Services.Stats.Initialize ();

			// Initialize GarbageCollector
			SorentoLib.Services.GarbageCollector.Initialize ();

			// Initialize cache.
			SorentoLib.Services.Cache.Initialize ();

			// Initialize FastCGI server.
			SorentoLib.FastCgi.Runtime.Initialize ();

			// Initialize SoretoLib.
			SorentoLib.Runtime.Initalize ();

			// Version
			SorentoLib.Services.Logging.LogInfo ("Application version " + SorentoLib.Runtime.GetVersionString () + " (" + SorentoLib.Runtime.GetCompileDate ().ToString ("dd.MM.yyyy") + ")");
		}
	}
}
