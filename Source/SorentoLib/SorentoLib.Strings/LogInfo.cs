//
// LogInfo.cs
//  
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2011 - 2012 Rasmus Pedersen
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
namespace SorentoLib.Strings
{
	public class LogInfo
	{
		#region RUNTIME
		public static string RuntimeServiceInitialized = "Service initalized: '{0}'";
		public static string RuntimeShutdown = "Shutting down in 3 sec";
		public static string RuntimeApplicationFilesChanged = "Application files has changed";
		#endregion

		#region FASTCGI
		public static string FastCGIServerStarted = "FastCGI server started";
		public static string FastCGIServerStopped = "FastCGI server stopped";
		#endregion

		#region SERVICES.CONFIG
		public static string ServicesConfigChanged = "Config file has changed. Refreshing.";
		#endregion

		#region SERVICES.STATS
		public static string ServicesStatsInitialized = "Statistics initalized!";
		#endregion

		#region SERVICES.ADDINS
		public static string ServicesAddinsAddinDiscovered = "New addin(s) discovered";
		public static string ServicesAddinsAddinChanged = "Addin was deleted/changed, application reload needed.";
		public static string ServicesAddinsAddinCacheRefreshed = "Addin cache refreshed";
		public static string ServicesAddinsAddinInitialized = "Addin initialized: '{0}'";
		#endregion
	}
}

