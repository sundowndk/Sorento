//
// Addins.cs
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
using System.IO;
using System.Threading;
using System.Collections.Generic;

using Mono.Addins;
using Mono.Addins.Setup;

namespace SorentoLib.Services
{
	public static class Addins
	{
		#region Private Static Fields
		private static bool Refreshing = false;

		private static object Lock = new object ();
		#endregion

		#region Public Static Methods
		public static void Initialize ()
		{
			// Initialize addin engine.
			AddinManager.Initialize ("data/", SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.path_addins));
			AddinManager.AddinLoaded += SorentoLib.Services.Addins.OnLoad;
			AddinManager.AddinLoadError += SorentoLib.Services.Addins.OnLoadError;
			AddinManager.Registry.Update (null);

			// Monitor addin folder for changes.
			FileSystemWatcher applicationwatcher1 = new FileSystemWatcher ();
			applicationwatcher1.Path = SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.path_addins);
			applicationwatcher1.IncludeSubdirectories = true;
			applicationwatcher1.Created += new FileSystemEventHandler (SorentoLib.Services.Addins.OnCreated);
			applicationwatcher1.EnableRaisingEvents = true;
			applicationwatcher1.Filter = "*.dll";
			
			FileSystemWatcher applicationwatcher2 = new FileSystemWatcher ();
			applicationwatcher2.Path = SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.path_addins);
			applicationwatcher2.IncludeSubdirectories = true;
			applicationwatcher2.Deleted += new FileSystemEventHandler (SorentoLib.Services.Addins.OnDeleted);
			applicationwatcher2.EnableRaisingEvents = true;
			applicationwatcher2.Filter = "*.dll";

			// LOG: LogInfo.RuntimeServiceInitialized
			Services.Logging.LogInfo (string.Format (Strings.LogInfo.RuntimeServiceInitialized, "Addins"));
		}

		public static void DisableAddin (string id)
		{
			AddinManager.Registry.DisableAddin (id);
		}

		public static void EnableAddin (string id)
		{
			AddinManager.Registry.EnableAddin (id);
		}

		public static List<Mono.Addins.Addin> List ()
		{
			List<Mono.Addins.Addin> result = new List<Mono.Addins.Addin> ();

			foreach (Mono.Addins.Addin addin in AddinManager.Registry.GetAddins ())
			{
				result.Add (addin);
			}

			return result;
		}
		#endregion

		#region Private Static Methods
		private static void OnCreated (object sender, FileSystemEventArgs e)
		{
			lock (Lock)
			{
//			if (!SorentoLib.Services.Addins.Refreshing)
//			{
				SorentoLib.Services.Addins.Refreshing = true;
				SorentoLib.Services.Logging.LogInfo (Strings.LogInfo.ServicesAddinsAddinDiscovered);
				Thread.Sleep (3000);
				AddinManager.Registry.Update (null);
				SorentoLib.Services.Addins.Refreshing = false;
				SorentoLib.Services.Logging.LogInfo (Strings.LogInfo.ServicesAddinsAddinCacheRefreshed);
			}
		}

		private static void OnDeleted (object sender, FileSystemEventArgs e)
		{
			lock (Lock)
			{
//			if (!SorentoLib.Services.Addins.Refreshing)
//			{
				SorentoLib.Services.Addins.Refreshing = true;
				SorentoLib.Services.Logging.LogInfo (Strings.LogInfo.ServicesAddinsAddinChanged);
				
				SorentoLib.Runtime.Shutdown ();
			}
		}


		private static void OnLoadError (object s, AddinErrorEventArgs args)
		{
			SorentoLib.Services.Logging.LogError ("Addin: " + args.AddinId);
			SorentoLib.Services.Logging.LogError ("Addin: " + args.Message);
			SorentoLib.Services.Logging.LogError ("Addin: " + args.Exception);
		}

		private static void OnLoad (object s, AddinEventArgs args)
		{
			SorentoLib.Services.Logging.LogInfo (string.Format (Strings.LogInfo.ServicesAddinsAddinInitialized, args.AddinId));
		}
		#endregion
	}
}

