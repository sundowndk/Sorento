//
// Ajax.cs
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
using System.Collections;
using System.Collections.Generic;

using SorentoLib;

namespace Core.Resolvers.Addin
{
	public class Ajax : SorentoLib.Addins.IAjaxBaseClass, SorentoLib.Addins.IAjax
	{
		#region Constructor
		public Ajax ()
		{
			base.NameSpaces.Add ("sorentolib");
			base.NameSpaces.Add ("sorentolib.services");
			base.NameSpaces.Add ("sorentolib.services.config");
			base.NameSpaces.Add ("sorentolib.services.snapshot");
		}
		#endregion

		#region Public Methods
		new public SorentoLib.Ajax.Respons Process (SorentoLib.Session Session, string Fullname, string Method)
		{
			SorentoLib.Ajax.Respons result = new SorentoLib.Ajax.Respons ();
			SorentoLib.Ajax.Request request = new SorentoLib.Ajax.Request (Session.Request.QueryJar.Get ("data").Value);

			switch (Fullname.ToLower ())
			{
				#region SorentoLib.User
				case "sorentolib.user":
				{
					switch (Method.ToLower ())
					{
//						if (Session.AccessLevel < SorentoLib.Enums.Accesslevel.Administrator) throw new Exception (string.Format (sCMS.Strings.Exception.AjaxSessionPriviliges, "template.new"));

						case "new":
						{
							result.Add (new SorentoLib.User (request.getValue<string> ("username"), request.getValue<string> ("email")));
							break;
						}

						case "load":
						{
							result.Add (SorentoLib.User.Load (request.getValue<Guid> ("id")));
							break;
						}

						case "save":
						{
							request.getValue<SorentoLib.User> ("sorentolib.user").Save ();
							break;
						}

						case "delete":
						{
							SorentoLib.User.Delete (request.getValue<Guid> ("id"));
							break;
						}

						case "list":
						{
							result.Add (SorentoLib.User.List ());
							break;
						}

						case "changepassword":
						{
//							string oldpassword = SorentoLib.Tools.StringHelper.ASCIIBytesToString (SorentoLib.Services.Crypto.Decrypt (SorentoLib.Tools.StringHelper.HexStringToBytes (request.Key<string> ("oldpassword"))));
//							string newpassword = SorentoLib.Tools.StringHelper.ASCIIBytesToString (SorentoLib.Services.Crypto.Decrypt (SorentoLib.Tools.StringHelper.HexStringToBytes (request.Key<string> ("newpassword"))));

//							SorentoLib.User user = new SorentoLib.User ();
//							if (user.Load (new Guid (request.Data<string> ("id"))))
//							{
//								if (user.Authenticate (oldpassword))
//								{
//									user.Password = newpassword;
//
//									if (user.Save ())
//									{
//										result.Data.Add ("success", "true");
//									}
//								}
//							}
							break;
						}

						case "isusernameinuse":
						{
							if (request.ContainsVariable ("id"))
							{
//								result.Data.Add ("result", SorentoLib.User.IsUsernameInUse (request.Key<string>("username"), new Guid (request.Key<string>("id"))));
							}
							else
							{
//								result.Data.Add ("result", SorentoLib.User.IsUsernameInUse (request.Key<string>("username")));
							}

							break;
						}

						case "isemailinuse":
						{
							if (request.ContainsVariable ("id"))
							{
//								result.Data.Add ("result", SorentoLib.User.IsUsernameInUse (request.Key<string>("email"), new Guid (request.Key<string>("id"))));
							}
							else
							{
//								result.Data.Add ("result", SorentoLib.User.IsUsernameInUse (request.Key<string>("email")));
							}

							break;
						}

						default:
							break;
					}
					break;
				}
				#endregion

				#region SorentoLib.Usergroup
				case "sorentolib.usergroup":
				{
					switch (Method.ToLower ())
					{
						case "new":
						{
							result.Add (new SorentoLib.Usergroup ());
							break;
						}

						case "load":
						{
							result.Add (SorentoLib.Usergroup.Load (request.getValue<Guid> ("id")));
							break;
						}

						case "save":
						{
							request.getValue<SorentoLib.Usergroup> ("sorentolib.usergroup").Save ();
							break;
						}

						case "delete":
						{
							SorentoLib.Usergroup.Delete (request.getValue<Guid> ("id"));
							break;
						}

						case "list":
						{
							result.Add (SorentoLib.Usergroup.List ());
							break;
						}
					}
					break;
				}
				#endregion

				#region SorentoLib.Session
				case "sorentolib.session":
				{
					switch (Method.ToLower ())
					{
						case "getcurrent":
						{
//							result.Data = Session.ToItem ();
							break;
						}

						case "logout":
						{
							result.Add (Session.Logout ());
							break;
						}
					}
					break;
				}
				#endregion

				#region SorentoLib.Media
				case "sorentolib.media":
				{
					switch (Method.ToLower ())
					{
						case "load":
						{
							SorentoLib.Media media = SorentoLib.Media.Load (new Guid (request.Key<string> ("id")));

//							result.Data.Add ("id", media.Id);
//							result.Data.Add ("createtimestamp", media.CreateTimestamp);
//							result.Data.Add ("updatetimestamp", media.UpdateTimestamp);
//							result.Data.Add ("path", media.Path);
//							result.Data.Add ("directoryname", media.DirectoryName);
//							result.Data.Add ("filename", media.FileName);
//							result.Data.Add ("mimetype", media.Mimetype);
//							result.Data.Add ("size", media.Size);
//							result.Data.Add ("accesslevel", media.Accesslevel);
//							result.Data.Add ("status", media.Status);

							break;
						}
					}
					break;
				}
				#endregion

				#region SorentoLib.Transformation
				case "sorentolib.mediatransformation":
				{
					switch (Method.ToLower ())
					{
						case "new":
						{
							MediaTransformation mediatransformation = MediaTransformation.FromAjaxRequest (request);
							mediatransformation.Save ();
							mediatransformation.ToAjaxRespons (result);

							break;
						}

						case "load":
						{
							MediaTransformation mediatransformation = MediaTransformation.Load (new Guid (request.Key<string> ("id")));
							mediatransformation.ToAjaxRespons (result);

							break;
						}

						case "save":
						{
							MediaTransformation mediatransformation = MediaTransformation.FromAjaxRequest (request);
							mediatransformation.Save ();

							break;
						}

						case "delete":
						{
							MediaTransformation.Delete (new Guid (request.Key<string> ("id")));

							break;
						}

						case "list":
						{
							List<Hashtable> mediatransformations = new List<Hashtable> ();
							foreach (SorentoLib.MediaTransformation mediatransformation in SorentoLib.MediaTransformation.List ())
							{
								mediatransformations.Add (mediatransformation.ToAjaxItem ());
							}
//							result.Data.Add ("mediatransformations", mediatransformations);

							break;
						}

						default:
							break;
					}
					break;
				}
				#endregion

				#region SorentoLib.Services.Config
				case "sorentolib.services.config":
				{
					switch (Method.ToLower ())
					{
						case "get":
						{
							if (request.Data.ContainsKey ("keys"))
							{
								Hashtable data = new Hashtable ();
								foreach (string key in ((Hashtable)request.Data["keys"]).Keys)
								{
									data.Add (((Hashtable)request.Data["keys"])[key], SorentoLib.Services.Config.Get<string> (((Hashtable)request.Data["keys"])[key]));
								}
//								result.Data.Add ("data", data);
							}
							else
							{
//								result.Data.Add ("value", SorentoLib.Services.Config.Get<string> (request.Key<string> ("module"), request.Key<string> ("key")));
							}

							break;
						}

						case "set":
						{
							if (request.Data.ContainsKey ("keys"))
							{
								foreach (string key in ((Hashtable)request.Data["keys"]).Keys)
								{
									SorentoLib.Services.Config.Set (key, ((Hashtable)request.Data["keys"])[key]);
								}
							}
							else
							{
								SorentoLib.Services.Config.Set (request.Key<string> ("module"), request.Key<string> ("key"), request.Key<string> ("value"));
							}

							break;
						}
					}
					break;
				}
				#endregion

				#region SorentoLib.Serivces.Snapshot
				case "sorentolib.services.snapshot":
				{
					switch (Method.ToLower ())
					{
						case "new":
						{
							SorentoLib.Services.Snapshot.Take ();

							break;
						}

						case "load":
						{
							SorentoLib.Services.Snapshot snapshot = SorentoLib.Services.Snapshot.Load (request.Key<string> ("id"));
							snapshot.ToAjaxRespons (result);

							break;
						}

						case "develop":
						{
							SorentoLib.Services.Snapshot.Develop (SorentoLib.Services.Snapshot.Load (request.Key<string> ("id")));

							break;
						}

						case "delete":
						{
							SorentoLib.Services.Snapshot.Delete (request.Key<string> ("id"));

							break;
						}

						case "list":
						{
							List<Hashtable> snapshots = new List<Hashtable> ();
							foreach (SorentoLib.Services.Snapshot snapshot in SorentoLib.Services.Snapshot.List ())
							{
								snapshots.Add (snapshot.ToAjaxItem ());
							}
//							result.Data.Add ("snapshots", snapshots);

							break;
						}

						default:
							break;
					}
					break;
				}
				#endregion
			}

			return result;
		}
		#endregion
	}
}
