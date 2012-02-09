//
// GarbageCollector.cs
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
using System.Timers;
using System.Collections;

namespace SorentoLib.Services
{
	public class GarbageCollector
	{
		#region Private Static Fields
		#endregion

		#region Public Static Methods
		public static void Initialize ()
		{
			// Setup automated stat pulling, every 5 minutes.
			Timer timer = new Timer();
			timer.Elapsed += new ElapsedEventHandler( EventHandler );
			timer.Interval = 2000;
			timer.Start ();

			// LOG: LogInfo.RuntimeServiceInitialized
			Services.Logging.LogInfo (string.Format (Strings.LogInfo.RuntimeServiceInitialized, "GarbageCollector"));
		}
		#endregion

		#region Private Static Methods
		private static void EventHandler (object Sender, ElapsedEventArgs E)
		{
			((Timer)Sender).Interval = 300000;
//			((Timer)Sender).Interval = 10000;
			SorentoLib.Services.Events.Invoke.ServiceGarbageCollector ();
		}
		#endregion
	}
}
