//
// Runtime.cs
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
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

using Mono.Addins;

namespace SorentoLib
{
	public static class Runtime
	{
		#region Private Static Fields
		private static bool ApplicationFilesChanged = false;
		#endregion

		#region Public Static Fields
		public static int CurrentlyOpenFiles = 0;
		#endregion

		#region Public Static Methods
		public static void Initalize ()
		{
			SorentoLib.Runtime.WatchApplicationFiles ();

			// Usergroups
			SorentoLib.Usergroup.Runtime ();


			// Subscribe to events.
			SorentoLib.Services.Events.ServiceConfigChanged += EventhandlerServiceConfigChanged;
			SorentoLib.Services.Events.ServiceGarbageCollector += EventhandlerServiceGarbageCollector;
			SorentoLib.Services.Events.ServiceStatsUpdate += EventhandlerServiceStatsUpdate;

			// Initalize Addins who needs it.
			foreach (SorentoLib.Addins.IInit init in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IInit)))
			{
			}
		}

		public static void WatchApplicationFiles ()
		{
			FileSystemWatcher watchlib = new FileSystemWatcher ();
			watchlib.Path = "";
			watchlib.IncludeSubdirectories = false;
			watchlib.Created += new FileSystemEventHandler (SorentoLib.Runtime.OnApplicationFileChanged);
			watchlib.Deleted += new FileSystemEventHandler (SorentoLib.Runtime.OnApplicationFileChanged);
			watchlib.EnableRaisingEvents = true;
		}

		public static void Shutdown ()
		{
			SorentoLib.Services.Logging.LogInfo ("Shutting down in 3 sec.");
			SorentoLib.FastCgi.Runtime.Shutdown ();
			Thread.Sleep (3000);

			Environment.Exit (0);
		}

		public static string GetVersionString ()
		{
			Version version = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version;
			return version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision;
		}

		public static DateTime GetCompileDate ()
		{
			System.Version version = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version;
			return new DateTime (version.Build * TimeSpan.TicksPerDay + version.Revision * TimeSpan.TicksPerSecond * 2).AddYears (1999).AddDays (-1);
		}

		public static void SetProcessName (string name)
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				try
				{
					unixSetProcessName (name);
				}
				catch
				{
				}
			}
		}

		static void unixSetProcessName (string name)
		{
			try
			{
				if (prctl (15, Encoding.ASCII.GetBytes (name + "\0"), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0)
				{
					throw new ApplicationException ("Error setting process name: " + Mono.Unix.Native.Stdlib.GetLastError ());
				}
			}
			catch (EntryPointNotFoundException)
			{
				// Not every BSD has setproctitle
				try
				{
					setproctitle (Encoding.ASCII.GetBytes ("%s\0"), Encoding.ASCII.GetBytes (name + "\0"));
				}
				catch (EntryPointNotFoundException)
				{
					
				}
			}
		}
		#endregion

		#region Private Static Methods
		private static void OnApplicationFileChanged (object sender, FileSystemEventArgs e)
		{
			if (!SorentoLib.Runtime.ApplicationFilesChanged)
			{
				SorentoLib.Runtime.ApplicationFilesChanged = true;
				SorentoLib.Services.Logging.LogInfo ("Application files has changed.");
				SorentoLib.Runtime.Shutdown ();
			}
		}

		[DllImport("libc")]
		// Linux
		private static extern int prctl (int option, byte[] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

		[DllImport("libc")]
		// BSD
		private static extern void setproctitle (byte[] fmt, byte[] str_arg);
		#endregion

		#region EventHandlers
		static void EventhandlerServiceConfigChanged (object Sender, EventArgs E)
//		static void HandleSorentoLibServicesEventsServiceConfigChanged (object Sender, EventArgs E)
		{
			Services.Database.ServiceConfigChanged ();
			Services.Datastore.ServiceConfigChanged ();
			Services.Logging.ServiceConfigChanged ();

			Session.ServiceConfigChanged ();
			User.ServiceConfigChanged ();
			Usergroup.ServiceConfigChanged ();
			Media.ServiceConfigChanged ();
		}

		static void EventhandlerServiceGarbageCollector (object Sender, EventArgs E)
//		static void HandleSorentoLibServicesEventsServiceGarbageCollector(object Sender, EventArgs E)
		{
			SorentoLib.Session.ServiceGarbageCollector ();
			SorentoLib.Media.ServiceGarbageCollector ();
		}

		static void EventhandlerServiceStatsUpdate (object Sender, EventArgs E)
//		static void HandleSorentoLibServicesEventsServiceStatsUpdate (object Sender, EventArgs E)
		{
			SorentoLib.Session.ServiceStatsUpdate ();
			SorentoLib.User.ServiceStatsUpdate ();
		}
		#endregion
	}
}
