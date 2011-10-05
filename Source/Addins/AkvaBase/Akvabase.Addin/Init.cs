// 
// Init.cs
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

using SorentoLib;

namespace Akvabase.Addin
{
	public class Init : SorentoLib.Addins.IInit
	{
		public Init ()
		{
			SorentoLib.Services.Events.ServiceStats += HandleSorentoLibServicesEventsServiceStats;

//			SorentoLib.Services.Events.UserCreated += HandleSorentoLibServicesEventsUserCreated;
//			SorentoLib.Services.Events.UserStatusChanged += HandleSorentoLibServicesEventsUserStatusChanged;
		}

		static void HandleSorentoLibServicesEventsServiceStats (object Sender, EventArgs E)
		{
			Akvabase.Profile.Stats ();
		}



		void HandleSorentoLibServicesEventsUserCreated (object Sender, EventArgs E)
		{
			SorentoLib.User user = (SorentoLib.User)((List<object>)Sender)[0];

			Akvabase.Profile profile = new Akvabase.Profile ();
			profile.Load (user.Id);

			string verificationid = profile.GetVerificationId ();
			string emailbody = SorentoLib.Services.Datastore.Get ("akvabase.email.signup.body", "da").Replace ("%%FIRSTNAME%%", profile.Firstname).Replace("%%VERIFICATIONID%%", verificationid);
			SorentoLib.Tools.Helpers.SendMail ("noreply@akvabase.dk", profile.Email, SorentoLib.Services.Datastore.Get ("akvabase.email.signup.subject", "da"), emailbody, true);
		}

		void HandleSorentoLibServicesEventsUserStatusChanged (object Sender, EventArgs E)
		{
			SorentoLib.User user = (SorentoLib.User)((List<object>)Sender)[0];
			SorentoLib.Enums.UserStatus to = (SorentoLib.Enums.UserStatus)((List<object>)Sender)[1];
			SorentoLib.Enums.UserStatus from = (SorentoLib.Enums.UserStatus)((List<object>)Sender)[2];

			if (to == SorentoLib.Enums.UserStatus.Disabled)
			{
			}

			if ((from == SorentoLib.Enums.UserStatus.NotVerified) && (to == SorentoLib.Enums.UserStatus.Enabled))
			{
				Akvabase.Profile profile = new Akvabase.Profile ();
				profile.Load (user.Id);

				string emailbody = SorentoLib.Services.Datastore.Get ("akvabase.email.welcome.body", "da").Replace ("%%FIRSTNAME%%", profile.Firstname);

				SorentoLib.Tools.Helpers.SendMail ("noreply@akvabase.dk", profile.Email, SorentoLib.Services.Datastore.Get ("akvabase.email.welcome.subject", "da"), emailbody, true);
			}
			else if (to == SorentoLib.Enums.UserStatus.Disabled)
			{
			}
		}
	}
}
