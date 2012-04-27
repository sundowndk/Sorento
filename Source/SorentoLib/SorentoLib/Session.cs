//
// Session.cs
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
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using SNDK;
using SNDK.DBI;
using SNDK.Enums;

namespace SorentoLib
{
	public class Session
	{
		#region Public Static Fields
		public static string DatastoreAisle = "sessions";
		#endregion

		#region Private Fields
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		private FastCgi.Request _request;
		private FastCgi.Responder _responder;
		private User _user;
		private string _remoteaddress;
		private List<string> _languages;
		private int _timeout;
		private Page _page;
		private Error _error;

		private string _userasstring
		{
			get
			{
				string result = string.Empty;

				if (this._user != null)
				{
					result = this._user.Id.ToString ();
				}

				return result;
			}

			set
			{
				if (value != string.Empty)
				{
					      try
					{
						this._user = User.Load (new Guid (value));
					}
					catch
					{
						this._user = null;
						Services.Logging.LogError (string.Format (Strings.LogError.SessionLoadUser, value));
					}
				}
			}
		}
		#endregion

		#region Public Fields
		public Guid Id
		{
			get
			{
				return this._id;
			}
		}

		public int CreateTimestamp
		{
			get
			{
				return this._createtimestamp;
			}
		}

		public int UpdateTimestamp
		{
			get
			{
				return this._updatetimestamp;
			}
		}

		public IList<string> Languages
		{
			get
			{
				return this._languages.AsReadOnly ();
			}
		}

		public FastCgi.Request Request
		{
			get
			{
				return this._request;
			}
		}

		public FastCgi.Responder Responder
		{
			get
			{
				return this._responder;
			}

			set
			{
				this._responder = value;
			}
		}

		public User User
		{
			get
			{
				return this._user;
			}

			set
			{
				this._user = value;
			}
		}

		public String RemoteAddress
		{
			get
			{
				return this._remoteaddress;
			}
		}

		public Page Page
		{
			get
			{
				return this._page;
			}
		}

		public Error Error
		{
			get
			{
				return this._error;
			}

			set
			{
				this._error = value;
			}
		}

		public bool LoggedIn
		{
			get
			{
				if (this._user != null)
				{
					return true;
				}

				return false;
			}
		}
		#endregion

		#region Public Constructor
		public Session (IDictionary<string, string> parameters, byte[] postData)
		{


			this._request = new FastCgi.Request (parameters, postData);

			if (!this._request.CookieJar.Exist ("sorentosessionid"))
			{
				this._id = Guid.NewGuid ();
				FastCgi.Cookie cookie = new FastCgi.Cookie();
				cookie.Name = "sorentosessionid";
				cookie.Value = this._id.ToString ();
				cookie.Path = "/";
				this._request.CookieJar.Add (cookie);
			}
			else
			{
				bool expired = false;

				try
				{
					Session session = Session.Load (new Guid (this._request.CookieJar.Get ("sorentosessionid").Value));

					this._id = session.Id;
					this._createtimestamp = session._createtimestamp;
					this._user = session._user;

					if ((Date.DateTimeToTimestamp (DateTime.Now) - session._updatetimestamp) > Services.Config.Get<int> (Enums.ConfigKey.core_sessiontimeout) || session._remoteaddress != this._request.Environment.RemoteAddress)
					{
						expired = true;
					}
				}
				catch (Exception exception)
				{
					// LOG: LogDebug.ExceptionUnknown
					Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SESSION", exception.Message));

					expired = true;
				}

				if (expired)
				{
					Services.Logging.LogDebug (string.Format (Strings.LogDebug.SessionTimeout, this._request.CookieJar.Get ("sorentosessionid").Value));

					this._id = Guid.NewGuid ();
					this._createtimestamp = Date.CurrentDateTimeToTimestamp ();
					this._user = null;
					this._request.CookieJar.Get ("sorentosessionid").Value = this._id.ToString ();
				}
			}

			if (this._user != null)
			{
				if (this._user.Status == Enums.UserStatus.Disabled)
				{
					this._user = null;
				}
			}

			this._remoteaddress = this._request.Environment.RemoteAddress;

			// Fill accepted languages.
			this._languages = new List<string> ();
			foreach (string language in this._request.Environment.HttpAcceptLanguage.Split (";".ToCharArray ())[0].ToString ().Split (",".ToCharArray ()))
			{
				this._languages.Add (language);
			}

			this._timeout = Services.Config.Get<int> (Enums.ConfigKey.core_sessiontimeout);
			this._page = new Page (this._request.Environment.HttpHost);
			this._error = new Error ();

			this.Save ();
		}

		private Session ()
		{
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			try
			{
				this._updatetimestamp = Date.CurrentDateTimeToTimestamp ();

				Hashtable item = new Hashtable ();

				item.Add ("id", this._id);
				item.Add ("createtimestamp", this._createtimestamp);
				item.Add ("updatetimestamp", this._updatetimestamp);
				item.Add ("userid", this._userasstring);
				item.Add ("remoteaddress", this._remoteaddress);

				Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Convert.ToXmlDocument (item, this.GetType ().FullName.ToLower ()));
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SESSION", exception.Message));

				// EXCEPTION: Exception.SessionSave
				throw new Exception (string.Format (Strings.Exception.SessionSave, this._id));
			}
		}

		public bool SignIn (string Username, string Password)
		{
			bool result = false;

			try
			{
				this._user = User.Load (Username);

				string password = string.Empty;
				switch (Services.Config.Get<string> (Enums.ConfigKey.core_authenticationtype))
				{
					case "rsa":
					{
//						Console.WriteLine ("RSA DECODE");
						password = SorentoLib.Tools.StringHelper.ASCIIBytesToString (SorentoLib.Services.Crypto.Decrypt (SorentoLib.Tools.StringHelper.HexStringToBytes (Password)));
						break;
					}

					default:
					{
						password = SNDK.Crypto.SHAHash (SHAHashAlgorithm.SHA1, Password + Password);
						break;
					}
				}

				if (this._user.Authenticate (password))
				{
					this.Save ();

					result = true;
					ServiceStatsUpdate ();
					SorentoLib.Services.Events.Invoke.SessionLoginSuccess (this);
				}
				else
				{
					this._user = null;
					SorentoLib.Services.Events.Invoke.SessionLoginFailed (this);
				}
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SESSION", exception.Message));

				// LOG: LogError.SessionSignIn
				Services.Logging.LogError (string.Format (Strings.LogError.SessionSignIn, Username));
			}

			return result;
		}

		public bool SignOut ()
		{
			if (this._user != null)
			{
				string username = this._user.Username;

				try
				{
					this._user = null;
					this.Save ();
					ServiceStatsUpdate ();
					Services.Events.Invoke.SessionLogout (this);

					// LOG: LogDebug.SessionUserSignOut
					Services.Logging.LogDebug (string.Format (Strings.LogDebug.SessionUserSignOut, username));
				}
				catch (Exception exception)
				{
					// LOG: LogDebug.ExceptionUnknown
					Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SESSION", exception.Message));

					// LOG: LogError.SessionSignOut
					Services.Logging.LogError (string.Format (Strings.LogError.SessionSignOut, username));
				}
			}

			return true;
		}

		public bool Authenticate (string password)
		{
			if (this._user != null)
			{
				return this._user.Authenticate (password);
			}

			return false;
		}

		public bool Authenticate (Guid usergroupid)
		{
			if (this._user != null)
			{
				return this._user.Authenticate (usergroupid);
			}

			return false;
		}

		public XmlDocument ToXmlDocument ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimestamp", this._updatetimestamp);
			result.Add ("user", this._user);
			
			return SNDK.Convert.ToXmlDocument (result, this.GetType ().FullName.ToLower ());
		}
		#endregion

		#region Public Static Methods
		public static Session Load (Guid id)
		{
			Session result = new Session ();

			try
			{
				Hashtable item;
				item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (Services.Datastore.Get<XmlDocument> (DatastoreAisle, id.ToString ()).SelectSingleNode ("(//sorentolib.session)[1]")));


				result._id = new Guid ((string)item["id"]);

				if (item.ContainsKey ("createtimestamp"))
				{
					result._createtimestamp = int.Parse ((string)item["createtimestamp"]);
				}

				if (item.ContainsKey ("updatetimestamp"))
				{
					result._updatetimestamp = int.Parse ((string)item["updatetimestamp"]);
				}

				if (item.ContainsKey ("userid"))
				{
					result._userasstring = (string)item["userid"];
				}

				if (item.ContainsKey ("remoteaddress"))
				{
					result._remoteaddress = (string)item["remoteaddress"];
				}
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SESSION", exception.Message));

				// EXCEPTION: Exception.SessionLoad
				throw new Exception (string.Format (Strings.Exception.SessionLoad, id));
			}

			return result;
		}

		public static void Delete (Guid id)
		{
			try
			{
				Services.Datastore.Delete (DatastoreAisle, id.ToString ());

//				ServiceStatsUpdate ();
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SESSION", exception.Message));

				// EXCEPTION: Exception.SessionDelete
				throw new Exception (string.Format (Strings.Exception.SessionDelete, id));
			}
		}

		public static List<Session> List ()
		{
			List<Session> result = new List<Session> ();

			foreach (string shelf in Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				try
				{
					result.Add (Load (new Guid (shelf)));
				}
				catch (Exception exception)
				{
					// LOG: LogDebug.ExceptionUnknown
					Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SESSION", exception.Message));
				}
			}

			return result;
		}
		#endregion

		#region Internal Static Methods
		internal static void ServiceGarbageCollector ()
		{
			foreach (Session session in List ())
			{
				if ((Date.DateTimeToTimestamp (DateTime.Now) - session.UpdateTimestamp) >  Services.Config.Get<int> (Enums.ConfigKey.core_sessiontimeout))
				{
					try
					{
						Session.Delete (session.Id);
					}
					catch (Exception exception)
					{
						// LOG: LogDebug.ExceptionUnknown
						Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SESSION", exception.Message));
					}
				}
			}


			// LOG: LogDebug.SessionGarbageCollector
			SorentoLib.Services.Logging.LogDebug (Strings.LogDebug.SessionGarbageCollector);
		}

		internal static void ServiceStatsUpdate ()
		{
			int activesessions = 0;
			int usersessions = 0;

			foreach (Session session in List ())
			{
				activesessions++;
				if (session.LoggedIn)
				{
					usersessions++;
				}
			}

			Services.Stats.Set (Enums.StatKey.sorentolib_session_activesessions, activesessions);
			Services.Stats.Set (Enums.StatKey.sorentolib_session_usersessions, usersessions);
			Services.Stats.Set (Enums.StatKey.sorentolib_session_guestsessions, activesessions - usersessions);

			// LOG: LogDebug.SessionStats
			Services.Logging.LogDebug (Strings.LogDebug.SessionStats);
		}
		#endregion
	}
}
