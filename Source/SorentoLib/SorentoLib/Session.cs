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
		public static string DatabaseTableName = Services.Database.Prefix + "sessions";
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

		public String RemoteAdress
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

//		public Enums.Accesslevel AccessLevel
//		{
//			get
//			{
//				if (this._user != null)
//				{
//					return this._user.Accesslevel;
//				}
//				else
//				{
//					return Enums.Accesslevel.Guest;
//				}
//			}
//		}

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

		#region Private Constructor
		private Session ()
		{
		}
		#endregion

		#region Public Constructor
		public Session (IDictionary<string, string> parameters, byte[] postData)
		{
			bool expired = false;

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
				try
				{
					Session session = Session.Load (new Guid (this._request.CookieJar.Get ("sorentosessionid").Value));

					this._id = session.Id;
					this._createtimestamp = session._createtimestamp;
					this._user = session._user;

					if ((Date.DateTimeToTimestamp (DateTime.Now) - session._updatetimestamp) > Services.Config.Get<int> ("core", "sessiontimeout") || session._remoteaddress != this._request.Environment.RemoteAddress)
					{
						expired = true;
					}
				}
				catch
				{
					expired = true;
				}
			}

			if (expired)
			{
				Console.WriteLine ("SESSION EXPIRED");
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.SessionTimeout, this._request.CookieJar.Get ("sorentosessionid").Value));

				this._id = Guid.NewGuid ();
				this._createtimestamp = Date.CurrentDateTimeToTimestamp ();
				this._user = null;
				this._request.CookieJar.Get ("sorentosessionid").Value = this._id.ToString ();
			}
			else
			{
				if (this._user != null)
				{
					if (this._user.Status == Enums.UserStatus.Disabled)
					{
						this._user = null;
					}
				}
			}

			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._remoteaddress = this._request.Environment.RemoteAddress;

			// Fill accepted languages.
			this._languages = new List<string> ();
			foreach (string language in this._request.Environment.HttpAcceptLanguage.Split (";".ToCharArray ())[0].ToString ().Split (",".ToCharArray ()))
			{
				this._languages.Add (language);
			}

			this._timeout = Services.Config.Get<int> ("core", "sessiontimeout");
			this._page = new Page (this._request.Environment.HttpHost);
			this._error = new Error ();

			this.Save ();
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			bool success = false;
			this._updatetimestamp = Date.CurrentDateTimeToTimestamp ();

			QueryBuilder qb = null;
			if (!SNDK.DBI.Helpers.GuidExists (Services.Database.Connection, DatabaseTableName, this._id))
			{
				qb = new QueryBuilder (QueryBuilderType.Insert);
			}
			else
			{
				qb = new QueryBuilder (QueryBuilderType.Update);
				qb.AddWhere ("id", "=", this._id);
			}

			qb.Table (DatabaseTableName);
			qb.Columns ("id",
			            "createtimestamp",
			            "updatetimestamp",
			            "userid",
			            "remoteaddress");

			qb.Values (this._id,
			           this._createtimestamp,
			           this._updatetimestamp,
			           this._userasstring,
			           this._remoteaddress);

			Query query = Services.Database.Connection.Query (qb.QueryString);

//			Console.WriteLine (query.AffectedRows);
			if (query.AffectedRows > 0)
			{
				success = true;
			}
			
			query.Dispose ();
			query = null;
			qb = null;

			if (!success)
			{
//				Console.WriteLine ("BLA");
				throw new Exception (string.Format (Strings.Exception.SessionSave, this._id));
			}
		}

		public bool Login (string Username, string Password)
		{
			bool result = false;

			try
			{
				this._user = User.Load (Username);

				string password = string.Empty;
				if (Services.Config.Get<bool> ("core", "enablersalogin"))
				{
					Console.WriteLine ("RSA DECODE");
					password = SorentoLib.Tools.StringHelper.ASCIIBytesToString (SorentoLib.Services.Crypto.Decrypt (SorentoLib.Tools.StringHelper.HexStringToBytes (Password)));
				}
				else
				{
					password = SNDK.Crypto.SHAHash (SHAHashAlgorithm.SHA1, Password + Password);
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
			catch (Exception E)
			{
				Console.WriteLine (E.ToString ());
				Services.Logging.LogError (string.Format (Strings.LogError.SessionLoginUser, Username));
			}

			return result;
		}

		public bool Logout ()
		{
			bool result = false;

			if (this._user != null)
			{
				this._user = null;
				this.Save ();
				result = true;
				ServiceStatsUpdate ();
				Services.Events.Invoke.SessionLogout (this);
			}

			return result;
		}

//		public bool AuthenticateByAccesslevel (Enums.Accesslevel Accesslevel)
//		{
//			bool result = false;
//
//			if (this._user != null)
//			{
//				if (this._user.Accesslevel >= Accesslevel)
//				{
//					result = true;
//				}
//			}
//			else
//			{
//				if (Enums.Accesslevel.Guest >= Accesslevel)
//				{
//					result = true;
//				}
//			}
//
//			return result;
//		}

		public bool AuthenticateByUsergroup (Guid Id)
		{
			bool result = false;

			if (this._user != null)
			{
				foreach (Usergroup usergroup in this._user.Usergroups)
				{
					if (usergroup.Id == Id)
					{
						result = true;
					}
				}
			}

			return result;
		}

		public Hashtable ToItem ()
		{

			Hashtable result = new Hashtable ();
			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimstamp", this._updatetimestamp);

			if (this.LoggedIn != null)
			{
				result.Add ("userid", this._user.Id);
			}

			result.Add ("loggedin", this.LoggedIn);
//			result.Add ("accesslevel", this.AccessLevel);

			result.Add ("remoteaddress", this._remoteaddress);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Session Load (Guid Id)
		{
			bool success = false;
			Session result = new Session ();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("id",
			            "createtimestamp",
			            "updatetimestamp",
			            "userid",
			            "remoteaddress");

			qb.AddWhere ("id", "=", Id);

			Query query = Services.Database.Connection.Query (qb.QueryString);

			if (query.Success)
			{
				if (query.NextRow ())
				{
					result._id = query.GetGuid (qb.ColumnPos ("id"));
					result._createtimestamp = query.GetInt (qb.ColumnPos ("createtimestamp"));
					result._updatetimestamp = query.GetInt (qb.ColumnPos ("updatetimestamp"));
					result._remoteaddress = query.GetString (qb.ColumnPos ("remoteaddress"));
					result._userasstring = query.GetString (qb.ColumnPos ("userid"));
					success = true;
				}
			}

			query.Dispose ();
			query = null;
			qb = null;

			if (!success)
			{
				throw new Exception (string.Format (Strings.Exception.SessionLoad, Id));
			}

			return result;
		}

		public static void Delete (Guid Id)
		{
			bool success = false;

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
			qb.Table (DatabaseTableName);
			qb.AddWhere ("id", "=", Id);

			Query query = Services.Database.Connection.Query (qb.QueryString);

			if (query.AffectedRows > 0)
			{
				success = true;
			}

			query.Dispose ();
			query = null;
			qb = null;

			if (!success)
			{
				throw new Exception (string.Format (Strings.Exception.SessionDelete, Id));
			}
		}
		#endregion

		#region Internal Static Methods
		internal static void ServiceConfigChanged ()
		{
			DatabaseTableName = SorentoLib.Services.Database.Prefix + "sessions";
		}

		internal static void ServiceGarbageCollector ()
		{
			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table(DatabaseTableName);
			qb.Columns("id",
			           "updatetimestamp");

			Query query = Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				while (query.NextRow ())
				{
					int updatetimestamp = query.GetInt (qb.ColumnPos ("updatetimestamp"));

					if ((Date.DateTimeToTimestamp (DateTime.Now) - updatetimestamp) >  Services.Config.Get<int> ("core", "sessiontimeout"))
					{
						try
						{
							Session.Delete (query.GetGuid (qb.ColumnPos ("id")));
						}
						catch (Exception E)
						{
							Services.Logging.LogError (string.Format (Strings.LogError.Exception, "session.delete", E.Message.ToString (), "session.garbagecollector"));
						}
					}
				}
			}

			query.Dispose ();
			query = null;
			qb = null;

			SorentoLib.Services.Logging.LogDebug (Strings.LogDebug.SessionGarbageCollector);
		}

		internal static void ServiceStatsUpdate ()
		{
			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table(DatabaseTableName);
			qb.Columns("updatetimestamp",
			           "userid");

			Query query = Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				int activesessions = 0;
				int usersessions = 0;

				while (query.NextRow ())
				{
					int updatetimestamp = query.GetInt (qb.ColumnPos ("updatetimestamp"));
					string userid = query.GetString (qb.ColumnPos ("userid"));

					if (userid != string.Empty)
					{
						usersessions++;
					}

					activesessions++;
				}

				Services.Stats.Set ("sorentolib.session.activesessions", activesessions);
				Services.Stats.Set ("sorentolib.session.usersessions", usersessions);
				Services.Stats.Set ("sorentolib.session.guestsessions", activesessions - usersessions);
			}

			query.Dispose ();
			query = null;
			qb = null;

			Services.Logging.LogDebug (Strings.LogDebug.SessionStats);
		}
		#endregion
	}
}
