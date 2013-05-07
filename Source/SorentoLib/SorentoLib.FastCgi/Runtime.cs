//
// FastCGI.cs
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2009 - 2012 Rasmus Pedersen
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
using System.Net;

using Mono.FastCgi;

namespace SorentoLib.FastCgi
{
	public static class Runtime
	{


		#region Public Static Fields
		public static Mono.FastCgi.Server Server;
		public static Socket Socket;
		#endregion
		static readonly object _object = new object();

		public static int requests = 0;


		public static void Test ()
		{
			lock (_object)
			{
				requests++;
			}
		}

		#region Public Static Methods
		public static void Initialize ()
		{
			try
			{
				// Create socket, and start listing for FastCGI connections.
				Socket socket;
				socket = SocketFactory.CreatePipeSocket (IntPtr.Zero);
//				Socket = SocketFactory.CreatePipeSocket (IntPtr.Zero);

				// Create server
				Server = new Mono.FastCgi.Server (socket);
//				Server = new Mono.FastCgi.Server (Socket);
				Server.SetResponder (typeof (SorentoLib.FastCgi.Responder));

				// Configure server
//				Server.MaxConnections = Services.Config.Get<int> (Enums.ConfigKey.fastcgi_maxconnections);
//				Server.MaxRequests = Services.Config.Get<int> (Enums.ConfigKey.fastcgi_maxrequests);
				Server.MaxConnections = 50;
				Server.MaxRequests = 50;
				Server.MultiplexConnections = false;
//				Server.MultiplexConnections = Services.Config.Get<bool> (Enums.ConfigKey.fastcgi_multiplexconnections);
				
				// Start listing.
				Server.Start (false);
			}
			catch
			{
				Services.Logging.LogFatalError (Strings.LogErrorFatal.FastCGIFailedToStartServer);
				Shutdown ();
			}
							
			// LOG: LogInfo.RuntimeServiceInitialized
			Services.Logging.LogInfo (string.Format (Strings.LogInfo.RuntimeServiceInitialized, "FastCGI"));
		}					
				
		public static void Shutdown ()
		{
			if (Server != null)
			{
				try
				{
					Socket.Close ();
					Server.Stop ();
				}
				catch (Exception exception)
				{
//					Services.Logging.LogDebug (exception.ToString ());
				}

				Services.Logging.LogInfo (Strings.LogInfo.FastCGIServerStopped);
			}
		}
		#endregion
	}		
}
