// 
// Stats.cs
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
	public class Stats
	{
		#region Private Static Fields
		private static Hashtable _data = new Hashtable ();
		#endregion

		#region Public Static Methods
		public static void Initialize ()
		{
			// Setup automated stat update, every 1 minutes.
			Timer timer = new Timer ();
			timer.Elapsed += new ElapsedEventHandler (EventHandlerUpdate);
			timer.Interval = 2000;
			//timer.Start ();

			// LOG: LogInfo.RuntimeServiceInitialized
			Services.Logging.LogInfo (string.Format (Strings.LogInfo.RuntimeServiceInitialized, "Stats"));
		}

		public static T Get<T> (object Key)
		{
			return Get<T> (Key.ToString ());
		}

		private static T Get<T> (string Key)
		{
			try
			{
				switch (typeof (T).Name.ToLower ())
				{
					default:
						return (T)Convert.ChangeType (Get (Key), typeof(T));
				}
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.ServicesStatsKeyNotValidType, Key, typeof (T).Name));
			}
		}

		public static void Set (object Key, object Value)
		{
			Set (Key.ToString (), Value);
		}

		private static void Set (string Key, object Value)
		{
			if (SorentoLib.Services.Stats._data.ContainsKey (Key.ToLower ()))
			{
				SorentoLib.Services.Stats._data[Key.ToLower ()] = Value;
			}
			else
			{
				SorentoLib.Services.Stats._data.Add (Key.ToLower (), Value);
			}
 		}

		public static bool Exist (string Key)
		{
			if (SorentoLib.Services.Stats._data.ContainsKey (Key.ToLower ()))
			{
				return true;
			}

			return false;
		}

		#endregion

		#region Private Static Methods
		private static object Get (string Key)
		{
			if (SorentoLib.Services.Stats._data.ContainsKey (Key.ToLower ()))
			{
				return SorentoLib.Services.Stats._data[Key.ToLower ()];
			}
			else
			{
				throw new Exception (string.Format (Strings.Exception.ServicesStatsKeyNotFound, Key));
			}
		}
		#endregion

		#region Eventhandlers
		private static void EventHandlerUpdate (object Sender, ElapsedEventArgs E )
		{
			((Timer)Sender).Interval = 60000;
			SorentoLib.Services.Events.Invoke.ServiceStatsUpdate ();
		}
		#endregion
	}
}
