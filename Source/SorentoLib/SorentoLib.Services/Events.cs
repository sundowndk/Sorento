// 
// Events.cs
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
using System.Collections.Generic;

namespace SorentoLib.Services
{
	public class Events
	{


		#region Events
		public static event EventHandler SessionLoginSuccess;
		public static event EventHandler SessionLoginFailed;
		public static event EventHandler SessionLogout;

		public static event EventHandler UserCreated;
		public static event EventHandler UserUpdated;
		public static event EventHandler UserDeleted;
		public static event EventHandler UserStatusChanged;

		public static event EventHandler UsergroupCreated;
		public static event EventHandler UsergroupUpdated;
		public static event EventHandler UsergroupDeleted;

		public static event EventHandler ServiceConfigChanged;
		public static event EventHandler ServiceStatsUpdate;
		public static event EventHandler ServiceGarbageCollector;

//		public static event EventHandler TemplateRenderSuccess;
//		public static event EventHandler TemplateRenderFailed;

//		public static event EventHandler RequestSuccess;
//		public static event EventHandler RequestFailed;


		#endregion

		#region Internal Class
		internal static class Invoke
		{
//			internal static void TemplateRenderSuccess (SorentoLib.Session Session, SorentoLib.Render.Template Template)
//			{
//				List<object> sender = new List<object>();
//				sender.Add(Session);
//				sender.Add(Template);
//
//				if (SorentoLib.Services.Events.TemplateRenderSuccess != null)
//				{
//					SorentoLib.Services.Events.TemplateRenderSuccess(sender, new EventArgs ());
//				}
//
//				sender = null;
//			}
//
//			internal static void TemplateRenderFailed (SorentoLib.Session Session, SorentoLib.Render.Template Template)
//			{
//				List<object> sender = new List<object>();
//				sender.Add(Session);
//				sender.Add(Template);
//
//				if (SorentoLib.Services.Events.TemplateRenderFailed != null)
//				{
//					SorentoLib.Services.Events.TemplateRenderFailed(sender, new EventArgs ());
//				}
//
//				sender = null;
//			}

			#region SESSION
			internal static void SessionLoginSuccess (SorentoLib.Session Session)
			{
				if (SorentoLib.Services.Events.SessionLoginSuccess != null)
				{
					List<object> sender = new List<object>();
					sender.Add (Session);

					SorentoLib.Services.Events.SessionLoginSuccess (sender, new EventArgs ());
				}
			}

			internal static void SessionLoginFailed (SorentoLib.Session Session)
			{
				if (SorentoLib.Services.Events.SessionLoginFailed != null)
				{
					List<object> sender = new List<object>();
					sender.Add (Session);

					SorentoLib.Services.Events.SessionLoginFailed (sender, new EventArgs ());
				}
			}

			internal static void SessionLogout (SorentoLib.Session Session)
			{
				if (SorentoLib.Services.Events.SessionLogout != null)
				{
					List<object> sender = new List<object>();
					sender.Add (Session);

					SorentoLib.Services.Events.SessionLogout (sender, new EventArgs ());
				}
			}
			#endregion

			#region USER
			internal static void UserCreated (SorentoLib.User User)
			{
				if (SorentoLib.Services.Events.UserCreated != null)
				{
					List<object> sender = new List<object>();
					sender.Add (User);

					SorentoLib.Services.Events.UserCreated (sender, new EventArgs ());
				}
			}

			internal static void UserUpdated (SorentoLib.User User)
			{
				if (SorentoLib.Services.Events.UserUpdated != null)
				{
					List<object> sender = new List<object>();
					sender.Add (User);

					SorentoLib.Services.Events.UserUpdated (sender, new EventArgs ());
				}
			}

			internal static void UserDeleted (SorentoLib.User User)
			{
				if (SorentoLib.Services.Events.UserDeleted != null)
				{
					List<object> sender = new List<object>();
					sender.Add (User);

					SorentoLib.Services.Events.UserDeleted (sender, new EventArgs ());
				}
			}

			internal static void UserStatusChanged (SorentoLib.User User, SorentoLib.Enums.UserStatus From, SorentoLib.Enums.UserStatus To)
			{
				if (SorentoLib.Services.Events.UserStatusChanged != null)
				{
					List<object> sender = new List<object>();
					sender.Add (User);
					sender.Add (From);
					sender.Add (To);

					SorentoLib.Services.Events.UserStatusChanged (sender, new EventArgs ());
				}
			}
			#endregion

			#region USERGROUP
			internal static void UsergroupCreated (Usergroup Usergroup)
			{
				if (Services.Events.UsergroupCreated != null)
				{
					List<object> sender = new List<object>();
					sender.Add (Usergroup);

					Services.Events.UsergroupCreated (sender, new EventArgs ());
				}
			}

			internal static void UsergroupUpdated (Usergroup Usergroup)
			{
				if (Services.Events.UsergroupUpdated != null)
				{
					List<object> sender = new List<object>();
					sender.Add (Usergroup);

					Services.Events.UsergroupUpdated (sender, new EventArgs ());
				}
			}

			internal static void UsergroupDeleted (Usergroup Usergroup)
			{
				if (Services.Events.UsergroupDeleted != null)
				{
					List<object> sender = new List<object>();
					sender.Add (Usergroup);

					Services.Events.UsergroupDeleted (sender, new EventArgs ());
				}
			}

			#endregion

			#region SERVICES
			internal static void ServiceConfigChanged ()
			{
				if (SorentoLib.Services.Events.ServiceConfigChanged != null)
				{
					SorentoLib.Services.Events.ServiceConfigChanged (null, new EventArgs ());
				}
			}

			internal static void ServiceStatsUpdate ()
			{
				if (SorentoLib.Services.Events.ServiceStatsUpdate != null)
				{
					SorentoLib.Services.Events.ServiceStatsUpdate (null, new EventArgs ());
				}
			}

			internal static void ServiceGarbageCollector ()
			{
				if (SorentoLib.Services.Events.ServiceGarbageCollector != null)
				{
					SorentoLib.Services.Events.ServiceGarbageCollector (null, new EventArgs ());
				}
			}
			#endregion
		}
		#endregion
	}
}
