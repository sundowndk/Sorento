//
// Function.cs
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
	public class Function : SorentoLib.Addins.IFunction
	{
		#region Private Fields
		private List<string> _namespaces = new List<string> ();
		#endregion

		#region Public Fields
		public List<string> Namespaces
		{
			get
			{
				return this._namespaces;
			}
		}
		#endregion

		#region Constructor
		public Function ()
		{
			this._namespaces.Add (typeof(Akvabase.Profile).Namespace.ToLower ());
		}
		#endregion

		#region Public Methods
		public bool IsProvided (string Namespace)
		{
			foreach (string _namespace in this._namespaces)
			{
				if (_namespace == Namespace.ToLower ())
				{
					return true;
				}
			}
			return false;
		}

		public bool Process (SorentoLib.Session Session, string Fullname, string Method)
		{
			switch (Fullname.ToLower ())
			{
				#region Profile
				case "akvabase.profile":
					switch (Method.ToLower ())
					{
						case "create":
						{
							Akvabase.Profile profile = new Akvabase.Profile();
							profile.Firstname = Session.Request.QueryJar.Get ("firstname").Value;
							profile.Lastname = Session.Request.QueryJar.Get ("lastname").Value;
							profile.Email = Session.Request.QueryJar.Get ("email").Value;
							profile.Password = SorentoLib.Tools.StringHelper.ASCIIBytesToString (SorentoLib.Services.Crypto.Decrypt (SorentoLib.Tools.StringHelper.HexStringToBytes (Session.Request.QueryJar.Get("password").Value)));
							profile.Status = SorentoLib.Enums.UserStatus.NotVerified;
							if (profile.Save ())
							{
								string verificationid = profile.GetVerificationId ();
								string subject = SorentoLib.Services.Datastore.Get ("sorento.email.signup.subject", "da");
								string body = SorentoLib.Services.Datastore.Get ("sorento.email.signup.body", Session.Language).Replace ("%%REALNAME%%", profile.Realname).Replace ("%%VERIFICATIONID%%", verificationid);
								SorentoLib.Tools.Helpers.SendMail (SorentoLib.Services.Config.Get("core", "mailfrom"), profile.Email, subject, body, true);
							}

							return true;
						}

						case "lostpassword":
						{
							Akvabase.Profile profile = new Akvabase.Profile ();
							if (profile.Load (Session.Request.QueryJar.Get ("email").Value))
							{
								string verificationid = profile.GetVerificationId ();
								string subject = SorentoLib.Services.Datastore.Get ("sorento.email.lostpassword1.subject", "da");
								string body = SorentoLib.Services.Datastore.Get ("sorento.email.lostpassword1.body", Session.Language).Replace ("%%REALNAME%%", profile.Realname).Replace ("%%VERIFICATIONID%%", verificationid);
								SorentoLib.Tools.Helpers.SendMail (SorentoLib.Services.Config.Get("core", "mailfrom"), profile.Email, subject, body, true);
								return true;
							}

							return true;
						}

						case "uploadavatar":
						{
							// RESTRICTIONS: ACCESSLEVEL >= USER
							if (Session.AccessLevel >= SorentoLib.Enums.Accesslevel.User)
							{
								List<string> allowedfiletypes = new List<string> ();
								allowedfiletypes.Add ("image/jpeg");
								allowedfiletypes.Add ("image/png");
								allowedfiletypes.Add ("image/gif");

								if (allowedfiletypes.Contains (Session.Request.QueryJar.Get ("upload").BinaryContentType))
								{
									SorentoLib.Media image = new SorentoLib.Media ("avatars/"+ Session.User.Username +".jpg", Session.Request.QueryJar.Get ("upload").BinaryData);
									image.Status = SorentoLib.Enums.MediaStatus.PublicTemporary;
									image.Save ();

									string script = SorentoLib.Services.Datastore.Get ("akvabase.graphics.profile.avatar", "resize");
									bool test = Toolbox.Graphics.ParseJob (script, image.HardPath, image.HardPath);
									Console.WriteLine (test);

									if (test)
									{
										Session.Page.Variables.Add ("mediaid", image.Id);
										Session.Page.Variables.Add ("mediasoftpath", image.SoftPath);
										Session.Page.Variables.Add ("uploadsuccess", "true");
										return true;
									}
									else
									{
										Session.Page.Variables.Add ("uploadsuccess", "false");
										return false;
									}
								}
							}
							return false;
						}

						default:

							return false;
					}
				#endregion


			}

			return false;
		}
		#endregion
	}
}
