//
// Render.cs
//
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2009 Rasmus Pedersen
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
using System.Reflection;

using SorentoLib;

namespace Core.Addin
{
	public class Render : SorentoLib.Addins.IRenderBaseClass, SorentoLib.Addins.IRender
	{
		#region Constructor
		public Render ()
		{
			base.NameSpaces.Add ("system");
			base.NameSpaces.Add ("system.collections");
			base.NameSpaces.Add ("system.collections.generic");
			base.NameSpaces.Add ("sorentolib");
			base.NameSpaces.Add ("sorentolib.fastcgi");
			base.NameSpaces.Add ("sorentolib.services");
		}
		#endregion

		#region Public Methods
		override public object Process (SorentoLib.Session Session, string Fullname, string Method, object Variable, SorentoLib.Render.Resolver.Parameters Parameters)
		{
			switch (Fullname)
			{
				#region SorentoLib.Session
				case "sorentolib.session":
					switch (Method)
					{
						case "":
							return ((SorentoLib.Session)Variable);

						case "id":
							return ((SorentoLib.Session)Variable).Id;

						case "createtimestamp":
							return ((SorentoLib.Session)Variable).CreateTimestamp;

						case "updatetimestamp":
							return ((SorentoLib.Session)Variable).UpdateTimestamp;

						case "languages":
							return SorentoLib.Render.Variables.ConvertToListObject<string> (((SorentoLib.Session)Variable).Languages);

						case "user":
							return ((SorentoLib.Session)Variable).User;

						case "cookiejar":
							return ((SorentoLib.Session)Variable).Request.CookieJar;

						case "queryjar":
							return ((SorentoLib.Session)Variable).Request.QueryJar;

						case "remoteaddress":
							return ((SorentoLib.Session)Variable).RemoteAddress;

//						case "accesslevel":
//							return ((SorentoLib.Session)Variable).AccessLevel;

						case "loggedin":
							return ((SorentoLib.Session)Variable).LoggedIn;

//						case "authenticatebyaccesslevel":
//							return ((SorentoLib.Session)Variable).AuthenticateByAccesslevel (SNDK.Convert.StringToEnum<SorentoLib.Enums.Accesslevel>(Parameters.Get<string>(0)));

						case "authenticate":
							switch (Parameters.Type (0).Name.ToLower())
							{
								case "guid":
									return ((SorentoLib.Session)Variable).Authenticate (Parameters.Get<Guid>(0));

								case "string":
									return ((SorentoLib.Session)Variable).Authenticate (Parameters.Get<string>(0));

								default:
									return null;
							}

						case "current":
							return Session;

						case "error":
							return Session.Error;
					}
					break;
				#endregion

				#region SorentoLib.Error
				case "sorentolib.error":
					switch (Method)
					{
						case "":
							return ((SorentoLib.Error)Variable);

						case "id":
							return ((SorentoLib.Error)Variable).Id;

						case "title":
							return ((SorentoLib.Error)Variable).Title;

						case "text":
							return ((SorentoLib.Error)Variable).Text;
					}
					break;
				#endregion

				#region SorentoLib.FastCgi.CookieJar
				case "sorentolib.fastcgi.cookiejar":
					switch (Method)
					{
						case "":
							return ((SorentoLib.FastCgi.CookieJar)Variable);

						case "get":
							return ((SorentoLib.FastCgi.CookieJar)Variable).Get (Parameters.Get<string> (0));

						case "exist":
							return ((SorentoLib.FastCgi.CookieJar)Variable).Exist (Parameters.Get<string> (0));
					}
					break;
				#endregion

				#region SorentoLib.FastCgi.Cookie
				case "sorentolib.fastcgi.cookie":
					switch (Method)
					{
						case "":
							return ((SorentoLib.FastCgi.Cookie)Variable);

						case "name":
							return ((SorentoLib.FastCgi.Cookie)Variable).Name;

						case "domain":
							return ((SorentoLib.FastCgi.Cookie)Variable).Domain;

						case "path":
							return ((SorentoLib.FastCgi.Cookie)Variable).Path;

						case "expires":
							return ((SorentoLib.FastCgi.Cookie)Variable).Expires;

						case "value":
							return ((SorentoLib.FastCgi.Cookie)Variable).Value;

						case "secure":
							return ((SorentoLib.FastCgi.Cookie)Variable).Secure;
					}
					break;
				#endregion

				#region SorentoLib.FastCgi.QueryJar
				case "sorentolib.fastcgi.queryjar":
					switch (Method)
					{
						case "":
							return ((SorentoLib.FastCgi.QueryJar)Variable);

						case "get":
							return ((SorentoLib.FastCgi.QueryJar)Variable).Get (Parameters.Get<string>(0));

						case "exist":
							return ((SorentoLib.FastCgi.QueryJar)Variable).Exist (Parameters.Get<string>(0));
					}
					break;
				#endregion

				#region SorentoLib.FastCgi.Query
				case "sorentolib.fastcgi.query":
					switch (Method)
					{
						case "":
							return ((SorentoLib.FastCgi.Query)Variable);

						case "name":
							return ((SorentoLib.FastCgi.Query)Variable).Name;

						case "value":
							return ((SorentoLib.FastCgi.Query)Variable).Value;
					}
					break;
				#endregion

				#region SorentoLib.User
				case "sorentolib.user":
					switch (Method)
					{
						case "":
						      return ((SorentoLib.User)Variable);

						case "id":
							return ((SorentoLib.User)Variable).Id;

						case "createtimestamp":
							return ((SorentoLib.User)Variable).CreateTimestamp;

						case "updatetimestamp":
							return ((SorentoLib.User)Variable).UpdateTimestamp;

						case "username":
							return ((SorentoLib.User)Variable).Username;

						case "email":
							return ((SorentoLib.User)Variable).Email;

						case "realname":
							return ((SorentoLib.User)Variable).Realname;

						case "firstname":
							return ((SorentoLib.User)Variable).FirstName;

						case "lastname":
							return ((SorentoLib.User)Variable).LastName;

//						case "accesslevel":
//							return ((SorentoLib.User)Variable).Accesslevel;

						case "usergroups":
							return SorentoLib.Render.Variables.ConvertToListObject<SorentoLib.Usergroup> (((SorentoLib.User)Variable).Usergroups);

						case "status":
							return ((SorentoLib.User)Variable).Status;

						case "load":
							switch (Parameters.Type (0).Name.ToLower())
							{
								case "guid":
									return SorentoLib.User.Load (Parameters.Get<Guid>(0));

								case "string":
									return SorentoLib.User.Load (Parameters.Get<string>(0));

								default:
									return null;
							}
							break;

						case "list":
							switch (Parameters.Count)
							{
								case 1:
									return SorentoLib.Render.Variables.ConvertToListObject<SorentoLib.User> (SorentoLib.User.List(SorentoLib.Enums.UserListFilter.OnlyUsersThatIsMemberOfUsergroupId, Parameters.Get<Guid>(0)));

								default:
									return SorentoLib.Render.Variables.ConvertToListObject<SorentoLib.User> (SorentoLib.User.List());
							}
				      }
					break;
				#endregion

				#region SorentoLib.Usergroup
				case "sorentolib.usergroup":
					switch (Method)
					{
						case "":
							return ((SorentoLib.Usergroup)Variable);

						case "id":
							return ((SorentoLib.Usergroup)Variable).Id;

						case "createtimestamp":
							return ((SorentoLib.Usergroup)Variable).CreateTimestamp;

						case "updatetimestamp":
							return ((SorentoLib.Usergroup)Variable).UpdateTimestamp;

						case "name":
							return ((SorentoLib.Usergroup)Variable).Name;

//						case "accesslevel":
//							return ((SorentoLib.Usergroup)Variable).Accesslevel;

						case "status":
							return ((SorentoLib.Usergroup)Variable).Status;

						case "load":
							return Usergroup.Load (Parameters.Get<Guid>(0));

						case "list":
							switch (Parameters.Count)
							{
//								case 2:
//									return SorentoLib.Render.Variables.ConvertToListObject<SorentoLib.Usergroup> (SorentoLib.Usergroup.List(SorentoLib.Enums.UsergroupListFilter.ExcludeUsergroupsThatUsernameIsMemberOf, (string)Parameters[1]));
//									return SorentoLib.Render.Variables.ConvertToListObject<SorentoLib.Usergroup> (SorentoLib.Usergroup.List(SorentoLib.Enums.UsergroupListFilter.ExcludeUsergroupsThatUsernameIsMemberOf, (string)Parameters[1]));

								default:
									return SorentoLib.Render.Variables.ConvertToListObject<SorentoLib.Usergroup> (SorentoLib.Usergroup.List());
							}
					}
					break;
				#endregion

				#region SorentoLib.Media
				case "sorentolib.media":
					switch (Method)
					{
						case "":
							return ((SorentoLib.Media)Variable);

						case "id":
							return ((SorentoLib.Media)Variable).Id;

						case "createtimestamp":
							return ((SorentoLib.Media)Variable).CreateTimestamp;

						case "updatetimestamp":
							return ((SorentoLib.Media)Variable).UpdateTimestamp;

						case "path":
							return ((SorentoLib.Media)Variable).Path;

						case "filename":
//							return ((SorentoLib.Media)Variable).FileName;

						case "directoryname":
//							return ((SorentoLib.Media)Variable).DirectoryName;

						case "mimetype":
							return ((SorentoLib.Media)Variable).Mimetype;

						case "size":
							return ((SorentoLib.Media)Variable).Size;

						case "status":
//							return ((SorentoLib.Media)Variable).Status;

						case "accesslevel":
//							return ((SorentoLib.Media)Variable).Accesslevel;

						case "usergroups":
//							return SorentoLib.Render.Variables.ConvertToListObject<SorentoLib.Usergroup> (((SorentoLib.Media)Variable).Usergroups);

						case "load":
							return SorentoLib.Media.Load (Parameters.Get<Guid>(0));
					}
					break;
				#endregion

				#region SorentoLib.Services.Config
				case "sorentolib.services.config":
					switch (Method)
					{
						case "getstring":
							return SorentoLib.Services.Config.Get<string> (Parameters.Get<string> (0) +"_"+ Parameters.Get<string> (1));
//
						case "getbool":
							return SorentoLib.Services.Config.Get<bool> (Parameters.Get<string> (0) +"_"+ Parameters.Get<string> (1));
//
						case "getint":
							return SorentoLib.Services.Config.Get<int> (Parameters.Get<string> (0) +"_"+ Parameters.Get<string> (1));
//
						case "getdecimal":
							return SorentoLib.Services.Config.Get<decimal> (Parameters.Get<string> (0) +"_"+ Parameters.Get<string> (1));
//
						case "getguid":
							return SorentoLib.Services.Config.Get<Guid> (Parameters.Get<string> (0) +"_"+ Parameters.Get<string> (1));
//
//						case "exist":
//							return SorentoLib.Services.Config.Exist (Parameters.Get<string> (0) +"_"+ Parameters.Get<string> (1));
					}
					break;
				#endregion

				#region SorentoLib.Services.Stats
				case "sorentolib.services.stats":
					switch (Method)
					{
						case "getstring":
							return SorentoLib.Services.Stats.Get<string> (Parameters.Get<string> (0));

						case "getint":
							return SorentoLib.Services.Stats.Get<int> (Parameters.Get<string> (0));

						case "getdecimal":
							return SorentoLib.Services.Stats.Get<decimal> (Parameters.Get<string> (0));

						case "exist":
							return SorentoLib.Services.Stats.Exist (Parameters.Get<string> (0));
					}
					break;
				#endregion

				#region SorentoLib.Services.Crypto
				case "sorentolib.services.crypto":
					switch (Method)
					{
						case "encryptexponent":
							return SorentoLib.Services.Crypto.EncryptExponent;

						case "modulus":
							return SorentoLib.Services.Crypto.Modulus;
					}
					break;
				#endregion

				#region SorentoLib.Env
				case "sorentolib.env":
					switch (Method)
					{
						case "authtype":
							return Session.Request.Environment.AuthType;

						case "contentlength":
							return Session.Request.Environment.ContentLength;

						case "contenttype":
							return Session.Request.Environment.ContentType;

						case "contenttypemultipartboundary":
							return Session.Request.Environment.ContentTypeMultipartBoundary;

						case "documentroot":
							return Session.Request.Environment.DocumentRoot;

						case "gatewayinterface":
							return Session.Request.Environment.GatewayInterface;

						case "httpaccept":
							return Session.Request.Environment.HttpAccept;

						case "httpacceptcharset":
							return Session.Request.Environment.HttpAcceptCharset;

						case "httpacceptencoding":
							return Session.Request.Environment.HttpAcceptEncoding;

						case "httpacceptlanguage":
							return Session.Request.Environment.HttpAcceptLanguage;

						case "httpconnection":
							return Session.Request.Environment.HttpConnection;

						case "httpcookie":
							return Session.Request.Environment.HttpCookie;

						case "httphost":
							return Session.Request.Environment.HttpHost;

						case "httpkeepalive":
							return Session.Request.Environment.HttpKeepAlive;

						case "httpreferer":
							return Session.Request.Environment.HttpReferer;

						case "httpuseragent":
							return Session.Request.Environment.HttpUserAgent;

						case "pathinfo":
							return Session.Request.Environment.PathInfo;

						case "pathtranslated":
							return Session.Request.Environment.PathTranslated;

						case "querystring":
							return Session.Request.Environment.QueryString;

						case "redirectquerystring":
							return Session.Request.Environment.RedirectQueryString;

						case "redirectstatus":
							return Session.Request.Environment.RedirectStatus;

						case "redirecturl":
							return Session.Request.Environment.RedirectUrl;

						case "remoteaddress":
							return Session.Request.Environment.RemoteAddress;

						case "remotehost":
							return Session.Request.Environment.RemoteHost;

						case "remoteident":
							return Session.Request.Environment.RemoteIdent;

						case "remoteuser":
							return Session.Request.Environment.RemoteUser;

						case "requestmethod":
							return Session.Request.Environment.RequestMethod;

						case "requesturi":
							return Session.Request.Environment.RequestUri;

						case "serveraddress":
							return Session.Request.Environment.ServerAddress;

						case "serveradmin":
							return Session.Request.Environment.ServerAdmin;

						case "servername":
							return Session.Request.Environment.ServerName;

						case "serverport":
							return Session.Request.Environment.ServerPort;

						case "serverprotocol":
							return Session.Request.Environment.ServerProtocol;

						case "serversignature":
							return Session.Request.Environment.ServerSignature;

						case "serversoftware":
							return Session.Request.Environment.ServerSoftware;

						case "version":
							return SorentoLib.Runtime.GetVersionString ();

						case "compiledate":
							return SorentoLib.Runtime.GetCompileDate ();
					}
					break;
				#endregion

				#region System.Guid
				case "system.guid":
					switch (Method)
					{
						case "":
							return ((Guid)Variable);

						case "tostring":
							return ((Guid)Variable).ToString();

						case "new":
							switch (Parameters.Count)
							{
								case 1:
									return new Guid (Parameters.Get<string> (0));

								default:
									return Guid.NewGuid();
							}
					}
					break;
				#endregion

				#region System.String
				case "system.string":
					switch (Method)
					{
						case "":
							return ((string)Variable);

						case "length":
							return ((string)Variable);

						case "padleft":
							return ((string)Variable).PadLeft (Parameters.Get<int> (0), Parameters.Get<string> (1)[0]);

						case "padright":
							return ((string)Variable).PadRight (Parameters.Get<int> (0), Parameters.Get<string> (1)[0]);

						case "substring":
							return ((string)Variable).Substring (Parameters.Get<int> (0), Parameters.Get<int> (1));

						case "removenewline":
							return ((string)Variable).Replace (System.Environment.NewLine, string.Empty);

						case "replace":
							//SorentoLib.Services.Logging.LogDebug ("Char:"+Parameters.Get<string> (0));
							return ((string)Variable).Replace (Parameters.Get<string> (0), Parameters.Get<string> (1));
							//string test = "\n";
							//return ((string)Variable).Replace (test, Parameters.Get<string> (1));
							//return ((string)Variable).Replace (System.Environment.NewLine, Parameters.Get<string> (1));

						case "split":
							return ((string)Variable).Split (Parameters.Get<string> (0).ToCharArray (), StringSplitOptions.RemoveEmptyEntries);

						case "tolower":
							return ((string)Variable).ToLower ();

						case "toupper":
							return ((string)Variable).ToUpper ();

						case "nolinebreak":
							return ((string)Variable).Replace ("\n","").Replace ("\r","");

						case "truncate":
							switch (Parameters.Count)
							{
								case 2:
									if (Parameters.Get<int> (0) < ((string)Variable).Length)
									{
										return ((string)Variable).Substring (0, Parameters.Get<int> (0)) + Parameters.Get<string> (1);
									}
									else
									{
										return (string)Variable;
									}

								default:
									if (Parameters.Get<int> (0) < ((string)Variable).Length)
									{
										return ((string)Variable).Substring (0, Parameters.Get<int> (0));
									}
									else
									{
										return (string)Variable;
									}
							}
					}
					break;
				#endregion

				#region System.String[]
				case "system.string[]":
					switch (Method)
					{
						case "":
							if (Parameters.Count > 0)
							{
								return ((string[])Variable)[Parameters.Get<int> (0)];
							}
							else
							{
								return ((string[])Variable);
							}

						case "length":
							return ((string[])Variable).Length;
					}
					break;
				#endregion

				#region System.Int
				case "system.int32":
					switch (Method)
					{
						case "":
							return ((int)Variable);
						
						case "tostring":
							return ((int)Variable).ToString ();
					}
					break;
				#endregion

				#region System.Boolean
				case "system.boolean":
					switch (Method)
					{
						case "":
						{
							return ((bool)Variable);
						}
					}
					break;
				#endregion

				#region System.Datetime
				case "system.datetime":
					switch (Method)
					{
						case "":
							return ((DateTime)Variable);

						case "day":
							return ((DateTime)Variable).Day;

						case "month":
							return ((DateTime)Variable).Month;

						case "year":
							return ((DateTime)Variable).Year;

						case "hour":
							return ((DateTime)Variable).Hour;

						case "minute":
							return ((DateTime)Variable).Minute;

						case "second":
							return ((DateTime)Variable).Second;

						case "millisecond":
							return ((DateTime)Variable).Millisecond;

						case "dayofweek":
							return ((DateTime)Variable).DayOfWeek;

						case "dayofyear":
							return ((DateTime)Variable).DayOfYear;

						case "now":
							return DateTime.Now;
					}
					break;
				#endregion

				#region System.Collections.Generic
				case "system.collections.generic.list`1":
					switch (Method)
					{
						case "":
							return (object)Variable;

						case "count":
							MethodInfo methodInfo = typeof(SorentoLib.Render.Variables).GetMethod("ConvertToListObjectNew", System.Reflection.BindingFlags.Static | BindingFlags.Public);
						MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(new Type[] { ((object)Variable).GetType ().GetGenericArguments()[0] });
						List<object> returnvalue = (List<object>)genericMethodInfo.Invoke(null, new object[] { (object)Variable });


							return returnvalue.Count;
					}
					break;
				#endregion

				#region System.Collections.Hashtable
				case "system.collections.hashtable":
//					Console.WriteLine ("11");
					switch (Method)
					{
						case "":
							return ((Hashtable)Variable);

						case "get":
							return ((Hashtable)Variable)[Parameters.Get<string> (0)];

						case "keys":
							List<string> result = new List<string> ();
							foreach (string key in ((Hashtable)Variable).Keys)
							{
								result.Add (key);
							}
							return SorentoLib.Render.Variables.ConvertToListObject<string> (result);
					}
					break;
				#endregion

				#region System.Version
				case "system.version":
				{
					switch (Method)
					{
						case "":
							return ((Version)Variable);

						case "major":
							return ((Version)Variable).Major;

						case "minor":
							return ((Version)Variable).Minor;

						case "Build":
							return ((Version)Variable).Build;

						case "Revision":
							return ((Version)Variable).Revision;

						case "MajorRevision":
							return ((Version)Variable).MajorRevision;

						case "MinorRevision":
							return ((Version)Variable).MinorRevision;
					}
					break;
				}
				#endregion
			}

			throw new SorentoLib.Exceptions.RenderExceptionMemberNotFound ();
		}
		#endregion
	}
}
