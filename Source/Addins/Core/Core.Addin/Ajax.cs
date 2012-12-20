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
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using Mono.Addins;

using SorentoLib;

namespace Core.Addin
{
	public class Ajax : SorentoLib.Addins.IAjaxBaseClass, SorentoLib.Addins.IAjax
	{
		#region Constructor
		public Ajax ()
		{
			base.NameSpaces.Add ("sorentolib");
			base.NameSpaces.Add ("sorentolib.services");
			base.NameSpaces.Add ("sorentolib.services.config");
			base.NameSpaces.Add ("sorentolib.services.settings");
			base.NameSpaces.Add ("sorentolib.services.snapshot");
			base.NameSpaces.Add ("sorentolib.services.addins");
		}
		#endregion

		#region Public Methods
		override public SorentoLib.Ajax.Respons Process (SorentoLib.Session Session, string Fullname, string Method)
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
							result.Add (new SorentoLib.User (request.getValue<string> ("username")));
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
							if (request.getValue<Guid> ("userid") == Session.User.Id)
							{
								if (request.xPathExists ("oldpassword"))
								{
									string oldpassword = request.getValue<string> ("oldpassword");
									string newpassword = request.getValue<string> ("newpassword");

									if (Session.User.Authenticate (oldpassword))
									{
										Session.User.Password = newpassword;
										Session.User.Save ();
										result.Add ("result", true);
									}
									else
									{
										result.Add ("result", false);
									}
								}
								else
								{
									result.Add ("result", false);
								}
							}
							else
							{
								string newpassword = request.getValue<string> ("newpassword");

								SorentoLib.User user = SorentoLib.User.Load (request.getValue<Guid> ("userid"));
								user.Password = newpassword;
								user.Save ();
								result.Add ("result", true);
							}

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
							if (request.xPathExists ("id"))
							{
								result.Add ("result", SorentoLib.User.IsUsernameInUse (request.getValue<string> ("username"), new Guid (request.Key<string>("id"))));
//								result.Add ("result", SorentoLib.User.IsUsernameInUse (request.Key<string>("username"), new Guid (request.Key<string>("id"))));
							}
							else
							{
								result.Add ("result", SorentoLib.User.IsUsernameInUse (request.getValue<string> ("username")));
//								result.Add ("result", SorentoLib.User.IsUsernameInUse (request.Key<string>("username")));
							}

							break;
						}

						case "isemailinuse":
						{
							if (request.xPathExists ("id"))
							{
								result.Add ("result", SorentoLib.User.IsEmailInUse (request.getValue<string>("email"), new Guid (request.getValue<string>("id"))));
							}
							else
							{
								result.Add ("result", SorentoLib.User.IsEmailInUse (request.getValue<string>("email")));
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

						case "accesslevels":
						{
							List<SorentoLib.Enums.Accesslevel> test1 = new List<SorentoLib.Enums.Accesslevel> ();
							foreach (SorentoLib.Enums.Accesslevel accesslevel in Enum.GetValues(typeof(SorentoLib.Enums.Accesslevel)))
							{

//								Hashtable test2 = new Hashtable ();
//								test2.Add ("name", accesslevel.ToString ());
//								test2.Add ("value", (int)accesslevel);

								test1.Add (accesslevel);
							}

							result.Add (test1);
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

							result.Add (Session);
							break;
						}

						case "loggedin":
						{
							result.Add (Session.LoggedIn);
							break;
						}

						case "login":
						{
							result.Add (Session.SignIn (request.getValue<string> ("username"), request.getValue<string> ("password")));
							break;
						}

						case "logout":
						{
							result.Add (Session.SignOut ());
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
							result.Add (SorentoLib.Media.Load (request.getValue<Guid> ("id")));
							break;
						}

						case "save":
						{
							request.getValue<SorentoLib.Media> ("sorentolib.media").Save ();
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
							result.Add (new SorentoLib.MediaTransformation ());
							break;
						}

						case "load":
						{
							result.Add (SorentoLib.MediaTransformation.Load (request.getValue<Guid> ("id")));
							break;
						}

						case "save":
						{
							request.getValue<SorentoLib.MediaTransformation> ("sorentolib.mediatransformation").Save ();
							break;
						}

						case "delete":
						{
							SorentoLib.MediaTransformation.Delete (request.getValue<Guid> ("id"));
							break;
						}

						case "list":
						{
							result.Add (SorentoLib.MediaTransformation.List ());
							break;
						}
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
							Hashtable item = (Hashtable)SNDK.Convert.FromXmlDocument (request.XmlDocument);

							List<Hashtable> test = new List<Hashtable> ();

							foreach (XmlDocument key in (List<XmlDocument>)item["config"])
							{
								string keyname = (string)((Hashtable)SNDK.Convert.FromXmlDocument (key))["value"];
								Hashtable val = new Hashtable ();
								val.Add (keyname, SorentoLib.Services.Config.Get<string> (keyname));
								test.Add (val);
							}

							result.Add ("config", test);

							break;
						}

						case "set":
						{
							Hashtable item = (Hashtable)SNDK.Convert.FromXmlDocument (request.XmlDocument);

							foreach (XmlDocument xml in (List<XmlDocument>)item["config"])
							{
								Hashtable conf = (Hashtable)SNDK.Convert.FromXmlDocument (xml);
//								SorentoLib.Services.Config.Set (conf["module"], conf["key"], (object)conf["value"]);
								SorentoLib.Services.Config.Set (conf["key"], conf["value"]);
//								Console.WriteLine (conf["module"] +" "+ conf["key"] +" "+ conf["value"]);
							}

//							foreach (string key in item.Keys)
//							{
//								Console.WriteLine (item[key].GetType ());
//							}

							//Console.WriteLine (request.GetXml ("config").OuterXml);

//							foreach (XmlDocument usergroup in (List<XmlDocument>)item["usergroups"])
//							{
//								result._usergroups.Add (Usergroup.FromXmlDocument (usergroup));
//							}

//							foreach (XmlDocument config in request.getValue<List<XmlDocument>> ("config"))
//							{
//								//result._usergroups.Add (Usergroup.FromXmlDocument (usergroup));
//							}

//							if (request.Data.ContainsKey ("keys"))
//							{
//								foreach (string key in ((Hashtable)request.Data["keys"]).Keys)
//								{
//									SorentoLib.Services.Config.Set (key, ((Hashtable)request.Data["keys"])[key]);
//								}
//							}
//							else
//							{
//								SorentoLib.Services.Config.Set (request.Key<string> ("module"), request.Key<string> ("key"), request.Key<string> ("value"));
//							}

							break;
						}
					}
					break;
				}
				#endregion

				#region SorentoLib.Services.Settings
				case "sorentolib.services.settings":
				{
					switch (Method.ToLower ())
					{
						case "get":
						{
							Hashtable item = (Hashtable)SNDK.Convert.FromXmlDocument (request.XmlDocument);
							
							List<Hashtable> test = new List<Hashtable> ();
							
							foreach (XmlDocument key in (List<XmlDocument>)item["config"])
							{
								string keyname = (string)((Hashtable)SNDK.Convert.FromXmlDocument (key))["value"];
								Hashtable val = new Hashtable ();
								val.Add (keyname, SorentoLib.Services.Settings.Get<string> (keyname));
								test.Add (val);
							}
							
							result.Add ("settings", test);
							
							break;
						}
							
						case "set":
						{
							Hashtable item = (Hashtable)SNDK.Convert.FromXmlDocument (request.XmlDocument);
							
							foreach (XmlDocument xml in (List<XmlDocument>)item["config"])
							{
								Hashtable conf = (Hashtable)SNDK.Convert.FromXmlDocument (xml);
								//								SorentoLib.Services.Config.Set (conf["module"], conf["key"], (object)conf["value"]);
								SorentoLib.Services.Settings.Set (conf["key"], conf["value"]);
								//								Console.WriteLine (conf["module"] +" "+ conf["key"] +" "+ conf["value"]);
							}
							
							//							foreach (string key in item.Keys)
							//							{
							//								Console.WriteLine (item[key].GetType ());
							//							}
							
							//Console.WriteLine (request.GetXml ("config").OuterXml);
							
							//							foreach (XmlDocument usergroup in (List<XmlDocument>)item["usergroups"])
							//							{
							//								result._usergroups.Add (Usergroup.FromXmlDocument (usergroup));
							//							}
							
							//							foreach (XmlDocument config in request.getValue<List<XmlDocument>> ("config"))
							//							{
							//								//result._usergroups.Add (Usergroup.FromXmlDocument (usergroup));
							//							}
							
							//							if (request.Data.ContainsKey ("keys"))
							//							{
							//								foreach (string key in ((Hashtable)request.Data["keys"]).Keys)
							//								{
							//									SorentoLib.Services.Config.Set (key, ((Hashtable)request.Data["keys"])[key]);
							//								}
							//							}
							//							else
							//							{
							//								SorentoLib.Services.Config.Set (request.Key<string> ("module"), request.Key<string> ("key"), request.Key<string> ("value"));
							//							}
							
							break;
						}
					}
					break;
				}
				#endregion

				#region SorentoLib.Services.Addins
				case "sorentolib.services.addins":
				{
					switch (Method.ToLower ())
					{
						case "enableaddin":
						{
							SorentoLib.Services.Addins.EnableAddin (request.getValue<string> ("id"));
							break;
						}

						case "disableaddin":
						{
							SorentoLib.Services.Addins.DisableAddin (request.getValue<string> ("id"));
							break;
						}

						case "list":
						{
							List<Hashtable> addins = new List<Hashtable> ();
							foreach (Mono.Addins.Addin addin in SorentoLib.Services.Addins.List ())
							{
								Hashtable item = new Hashtable ();
								item.Add ("id", addin.Id);
								item.Add ("enabled", addin.Enabled);
								item.Add ("name", addin.LocalId);
								item.Add ("version", addin.Version);
								item.Add ("author", addin.Description.Author);
								item.Add ("description", addin.Description.Description);
								item.Add ("url", addin.Description.Url);
								item.Add ("candisable", addin.Description.CanDisable);

								addins.Add (item);
							}

							result.Add ("sorentolib.services.addins", addins);
							break;
						}

						default:
							break;
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
