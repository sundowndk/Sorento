//
// FastCGI.cs
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
using System.Net;

using Mono.FastCgi;

namespace SorentoLib.FastCgi
{
	public static class Runtime
	{
		#region Private Static Fields
		private static Mono.FastCgi.Server Server;
		private static Socket Socket;
		#endregion
		
		#region Public Static Methods
		public static void Initialize ()
		{
			try
			{
				// Create socket, and start listing for FastCGI connections.
				Socket socket;
				socket = SocketFactory.CreatePipeSocket (IntPtr.Zero);

				// Create server
				SorentoLib.FastCgi.Runtime.Server = new Mono.FastCgi.Server (socket);
				SorentoLib.FastCgi.Runtime.Server.SetResponder (typeof(SorentoLib.FastCgi.Responder));

				// Configure server
				SorentoLib.FastCgi.Runtime.Server.MaxConnections = SorentoLib.Services.Config.Get<int> ("fastcgi", "maxconnections");
				SorentoLib.FastCgi.Runtime.Server.MaxRequests = SorentoLib.Services.Config.Get<int> ("fastcgi", "maxrequests");
				SorentoLib.FastCgi.Runtime.Server.MultiplexConnections = true;
				
				// Start listing.
				SorentoLib.FastCgi.Runtime.Server.Start (false);
			}
			catch
			{
				SorentoLib.Services.Logging.LogFatalError("Failed to start FastCgi server.");
				SorentoLib.Runtime.Shutdown();
			}
							
			SorentoLib.Services.Logging.LogInfo("FastCgi server started.");			
		}					
				
		public static void Shutdown ()
		{
			if (SorentoLib.FastCgi.Runtime.Server != null)
			{
				try
				{
					Socket.Close ();
					SorentoLib.FastCgi.Runtime.Server.Stop ();

				} catch (Exception e)
				{
				}

				SorentoLib.Services.Logging.LogInfo("FastCgi server stopped.");
			}
		}
		#endregion
	}		
}
