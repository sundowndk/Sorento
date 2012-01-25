//
// User.cs
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
using SNDK.Enums;
using SNDK.DBI;

namespace SorentoLib
{
	[Serializable]
	public class User
	{
		#region Public Static Fields
		public static string DatabaseTableName = SorentoLib.Services.Database.Prefix + "users";
		public static string DatastoreAisle = "users";

		#endregion

		#region Private Fields
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		private List<Usergroup> _usergroups;
		private string _username;
		private string _password;
		private string _realname;
		private string _email;
		private Guid _avatarid;
		private Enums.UserStatus _status;

		private string __usergroups_as_string
		{
			get
			{
				string result = string.Empty;
				foreach (Usergroup usergroup in this._usergroups)
				{
					// Remove duplicates
					if (result.Contains (usergroup.Id.ToString ()))
					{
						continue;
					}

					result += usergroup.Id.ToString () + ";";
				}

				return  result.TrimEnd (";".ToCharArray ());
			}

			set
			{
				this._usergroups.Clear ();

				foreach (string id in value.Split (";".ToCharArray (), StringSplitOptions.RemoveEmptyEntries))
				{
					try
					{
						this._usergroups.Add (Usergroup.Load (new Guid (id)));
					}
					catch
					{
						// LOG: LogErrorUserLoadUsergroup
						Services.Logging.LogError (string.Format (Strings.LogError.UserLoadUsergroup, id));
					}
				}

			}
		}

//		private string _usergroupidsasstring
//		{
//			get
//			{
//				string result = string.Empty;
//				foreach (Guid id in this._usergroupids)
//				{
//					// Remove duplicates
//					if (result.Contains (id.ToString ()))
//					{
//						continue;
//					}
//
//					result += id.ToString () + ";";
//				}
//
//				return result.TrimEnd (";".ToCharArray ());
//			}
//
//			set
//			{
//				this._usergroupids.Clear ();
//
//				if (value != string.Empty)
//				{
//					foreach (string id in value.Split (";".ToCharArray ()))
//					{
//						try
//						{
//							this._usergroupids.Add (new Guid (id));
//						}
//						catch
//						{
//							// LOG: LogErrorUserLoadUsergroup
//							Services.Logging.LogError (string.Format (Strings.LogError.UserLoadUsergroup, id));
//						}
//					}
//				}
//			}
//		}

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

		public List<Usergroup> Usergroups
		{
			get
			{
				return this._usergroups;
			}
		}

		public string Username
		{
			get
			{
				return this._username;
			}

			set
			{
				if (User.IsUsernameInUse (value, this._id))
				{
					throw new Exception (string.Format (Strings.Exception.UserSetUsername, value));
				}

				this._username = value;
			}
		}

		public string Password
		{
			get
			{
				return this._password;
			}

			set
			{
				this._password = Crypto.SHAHash (SNDK.Enums.SHAHashAlgorithm.SHA1, value, value);
			}
		}

		public string Realname
		{
			get
			{
				return this._realname;
			}

			set
			{
				this._realname = value;
			}
		}

		public string FirstName
		{
			get
			{
				return this._realname.Split (" ".ToCharArray ())[0];
			}
		}

		public string LastName
		{
			get
			{
				return this._realname.Split (" ".ToCharArray ())[1];
			}
		}

		public string Email
		{
			get
			{
				return this._email;
			}

			set
			{
				if (User.IsEmailInUse (value, this._id))
				{
					throw new Exception (string.Format (Strings.Exception.UserSetEmail, value));
				}
				this._email = value;
			}
		}

		public Media Avatar
		{
			get
			{
				return null;
			}

			set
			{

			}
		}

//		public Enums.Accesslevel Accesslevel
//		{
//			get
//			{
//				Enums.Accesslevel result = Enums.Accesslevel.Guest;
//				foreach (Usergroup usergroup in this.Usergroups)
//				{
//					if (usergroup.Type == Enums.UsergroupType.BuildIn)
//					{
//						if (usergroup.Accesslevel > result)
//						{
//							result = usergroup.Accesslevel;
//						}
//					}
//				}
//				return result;
//			}
//		}

		public Enums.UserStatus Status
		{
			get
			{
				return this._status;
			}

			set
			{
				this._status = value;

				// EVENT: UserStatusChanged
				Services.Events.Invoke.UserStatusChanged (this, this._status, value);
			}
		}
		#endregion

		#region Constructor
		public User (string username, string email)
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = Date.CurrentDateTimeToTimestamp ();
			this._usergroups = new List<Usergroup> ();
			this._username = username;
			this._password = string.Empty;
			this._realname = string.Empty;
			this._email = email;
			this._avatarid = Guid.Empty;
			this._status = Enums.UserStatus.Disabled;

			// Check if specified username is available.
			if (User.IsUsernameInUse (username))
			{
				// EXCEPTION: UserCreateUsername
				throw new Exception (string.Format (SorentoLib.Strings.Exception.UserCreateUsername, username));
			}

			// Check if specified email is available.
			if (User.IsEmailInUse (email))
			{
				// EXCEPTION: UserCreateEmail
				throw new Exception (string.Format (SorentoLib.Strings.Exception.UserCreateEmail, email));
			}

//			// Add default usergroup.
//			this._usergroups.Add (Runtime.UsergroupUser);
//			try
//			{
//				this._usergroups.Add (Usergroup.Load (Services.Config.Get<Guid> (Enums.ConfigKey.core_defaultusergroup)));
//			}
//			catch
//			{
//				// LOG: LogErrorUserCreateDefaultUsergroup
//				Services.Logging.LogError (string.Format (Strings.LogError.UserCreateDefaultUsergroup, Services.Config.Get<Guid> (Enums.ConfigKey.core_defaultusergroup)));
//			}
		}

		private User ()
		{
			this._id = Guid.Empty;
			this._createtimestamp = 0;
			this._updatetimestamp = 0;
			this._usergroups = new List<Usergroup> ();
			this._username = string.Empty;
			this._password = string.Empty;
			this._realname = string.Empty;
			this._email = string.Empty;
			this._avatarid = Guid.Empty;
			this._status = Enums.UserStatus.Disabled;
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			try
			{
				Services.Datastore.Meta meta = new Services.Datastore.Meta ();
				meta.Add ("username", this._username);
				meta.Add ("email", this._email);

				Services.Datastore.Set (DatastoreAisle, this._id.ToString (), this.ToXmlDocument (), meta);
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.UserSave, this._id));
			}
		}


		public XmlDocument ToXmlDocument ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimestamp", this._updatetimestamp);
//			result.Add ("usergroupids", this._usergroups);
			result.Add ("usergroups", this._usergroups);
			result.Add ("username", this._username);
			result.Add ("password", this._password);
			result.Add ("realname", this._realname);
			result.Add ("email", this._email);
			result.Add ("avatarid", this._avatarid);
//			result.Add ("accesslevel", this.Accesslevel);
			result.Add ("status", this._status);

			return SNDK.Convert.ToXmlDocument (result, this.GetType ().FullName.ToLower ());
		}
		#endregion

		#region Public Static Methods
		public static User Load (string username)
		{
			return Load (Guid.Empty, username);
		}

		public static User Load (Guid id)
		{
			return Load (id, string.Empty);
		}

		private static User Load (Guid id, string username)
		{
			if (id != Guid.Empty)
			{
				return FromXmlDocument (Services.Datastore.Get<XmlDocument> (DatastoreAisle, id.ToString ()));
			}
			else
			{
				return Services.Datastore.Get<SorentoLib.User> (DatastoreAisle, id.ToString ());
//				return Services.Datastore.Get<SorentoLib.User> (DatastoreAisle, )
			}


//			bool success = false;
//			User result = new User ();

//			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
//			qb.Table (SorentoLib.User.DatabaseTableName);
//			qb.Columns
//				(
//					"id",
//					"createtimestamp",
//					"updatetimestamp",
//			            "usergroupids",
//			            "username",
//			            "password",
//			            "realname",
//			            "email",
//			            "avatar",
//			            "status"
//				);
//
//			if (id != Guid.Empty)
//			{
//				qb.AddWhere ("id", "=", id);
//			}
//			else if (username != string.Empty)
//			{
//				qb.AddWhere ("username", "=", username);
//			}
//			else
//			{
//				throw new Exception (Strings.Exception.UserLoad);
//			}
//
//			Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
//
//			if (query.Success)
//			{
//				if (query.NextRow ())
//				{
//					result._id = query.GetGuid (qb.ColumnPos ("id"));
//					result._createtimestamp = query.GetInt (qb.ColumnPos ("createtimestamp"));
//					result._updatetimestamp = query.GetInt (qb.ColumnPos ("updatetimestamp"));
//					result.__usergroups_as_string = query.GetString (qb.ColumnPos ("usergroupids"));
//					result._username = query.GetString (qb.ColumnPos ("username"));
//					result._password = query.GetString (qb.ColumnPos ("password"));
//					result._realname = query.GetString (qb.ColumnPos ("realname"));
//					result._email = query.GetString (qb.ColumnPos ("email"));
//					result._avatarid = query.GetGuid (qb.ColumnPos ("avatar"));
//					result._status = query.GetEnum<SorentoLib.Enums.UserStatus> (qb.ColumnPos ("status"));
//
//					success = true;
//				}
//			}
//
//			query.Dispose ();
//			query = null;
//			qb = null;
//
//			if (!success)
//			{
//				if (id != Guid.Empty)
//				{
//					throw new Exception (string.Format (Strings.Exception.UserLoadGuid, id));
//				}
//				else
//				{
//					throw new Exception (string.Format (Strings.Exception.UserLoadUsername, username));
//				}
//			}

//			return result;
		}

		public static void Delete (string username)
		{
			Delete (Guid.Empty, username);
		}

		public static void Delete (Guid id)
		{
			Delete (id, string.Empty);
		}

		private static void Delete (Guid id, string username)
		{
			bool success = false;

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
			qb.Table (DatabaseTableName);

			if (id != Guid.Empty)
			{
				qb.AddWhere ("id", "=", id);
			}
			else
			{
				qb.AddWhere ("username", "=", username);
			}

			Query query = Services.Database.Connection.Query (qb.QueryString);

			if (query.AffectedRows > 0)
			{
				success = true;
			}

			query.Dispose ();
			query = null;
			qb = null;

			if (success)
			{
				ServiceStatsUpdate ();
			}
			else
			{
				if (id != Guid.Empty)
				{
					throw new Exception (string.Format (Strings.Exception.UserDeleteGuid, id));
				}
				else
				{
					throw new Exception (string.Format (Strings.Exception.UserDeleteUsername, username));
				}
			}
		}

		public static List<User> List ()
		{
			return List (Enums.UserListFilter.None, null);
		}

		public static List<User> List (Enums.UserListFilter filter, object filterData)
		{
			List<User> result = new List<User>();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("id");

			switch (filter) {
				case SorentoLib.Enums.UserListFilter.OnlyUsersThatIsMemberOfUsergroupId:
				{
					qb.AddWhere ("usergroups", "like", "%"+ ((Guid)filterData).ToString () +"%");
					break;
				}
			}

			Query query = Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				while (query.NextRow ())
				{
					User user = Load (query.GetGuid (qb.ColumnPos ("id")));
					result.Add (user);
				}
			}

			query.Dispose ();
			query = null;
			qb = null;

			return result;
		}

		static public bool IsUsernameInUse (string username)
		{


			return IsUsernameInUse (username, Guid.Empty);
		}

		static public bool IsUsernameInUse (string username, Guid filterOutUserId)
		{
			bool result = false;

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("id");
			qb.AddWhere ("username", "=", username);

			if (filterOutUserId != Guid.Empty)
			{
				qb.AddWhereAND ();
				qb.AddWhere ("id", "!=", filterOutUserId);
			}

			Query query = Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				if (query.NextRow ())
				{
					result = true;
				}
			}

			query.Dispose ();
			query = null;
			qb = null;

			return result;
		}

		static public bool IsEmailInUse (string Email)
		{
			return IsEmailInUse(Email, Guid.Empty);
		}

		static public bool IsEmailInUse (string Email, Guid filterOutUserId)
		{
			bool result = false;

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("id");
			qb.AddWhere ("email", "=", Email);

			if (filterOutUserId != Guid.Empty)
			{
				qb.AddWhereAND ();
				qb.AddWhere ("id", "!=", filterOutUserId);
			}

			Query query = Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				if (query.NextRow ())
				{
					result = true;
				}
			}

			query.Dispose ();
			query = null;
			qb = null;

			return result;
		}

		public static User FromXmlDocument (XmlDocument xmlDocument)
		{
			Hashtable item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (xmlDocument.SelectSingleNode ("(//sorentolib.user)[1]")));

			User result;

			if (item.ContainsKey ("id"))
			{
				result = new User ();
				result._id = new Guid ((string)item["id"]);
				result._username = (string)item["name"];
				result._email = (string)item["email"];
			}
			else
			{
				throw new Exception ("USER1");
			}

			if (item.ContainsKey ("createtimestamp"))
			{
				result._createtimestamp = int.Parse ((string)item["createtimestamp"]);
			}

			if (item.ContainsKey ("updatetimestamp"))
			{
				result._updatetimestamp = int.Parse ((string)item["updatetimestamp"]);
			}

			if (item.ContainsKey ("usergroups"))
			{
				result._usergroups.Clear ();

				foreach (XmlDocument usergroup in (List<XmlDocument>)item["usergroups"])
				{
					result._usergroups.Add (Usergroup.FromXmlDocument (usergroup));
				}
			}

			if (item.ContainsKey ("username"))
			{
				result.Username = (string)item["username"];
			}

			if (item.ContainsKey ("password"))
			{
				if ((string)item["password"] != string.Empty)
				{
					result._password = (string)item["password"];
				}
			}

			if (item.ContainsKey ("email"))
			{
				result.Email = (string)item["email"];
			}

			if (item.ContainsKey ("realname"))
			{
				result._realname = (string)item["realname"];
			}

			if (item.ContainsKey ("avatarid"))
			{
				try
				{
					result._avatarid = new Guid ((string)item["avatarid"]);
				}
				catch {}
			}

			if (item.ContainsKey ("status"))
			{
				result._status = SNDK.Convert.StringToEnum<SorentoLib.Enums.UserStatus> ((string)item["status"]);
			}

			return result;
		}
		#endregion

		#region Internal Static Methods
		public static void Purge ()
		{
			foreach (User user in User.List ())
			{
				QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
				qb.Table (DatabaseTableName);
				qb.AddWhere ("id", "=", user.Id);

				Query query = Services.Database.Connection.Query (qb.QueryString);
				query.Dispose ();
				query = null;
				qb = null;
			}
		}

		internal static void ServiceConfigChanged ()
		{
			DatabaseTableName = SorentoLib.Services.Database.Prefix + "users";
		}

		internal static void ServiceStatsUpdate ()
		{
			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns("id");

			Query query = Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				int totalusers = 0;
				while (query.NextRow())
				{
					totalusers++;
				}

				Services.Stats.Set ("sorentolib.user.totalusers", totalusers);
			}

			query.Dispose ();
			query = null;
			qb = null;

			Services.Logging.LogDebug (Strings.LogDebug.UserStats);
		}
		#endregion


		#region OLD
//		private Media _avatar;
//		private List<Usergroup> _usergroups;

//		private string _avatarid
//		{
//			get
//			{
//				if (this._avatar != null)
//				{
//					return this._avatar.Id.ToString ();
//				}
//				else
//				{
//					return string.Empty;
//				}
//			}
//
//			set
//			{
//				try
//				{
//					this._avatar = Media.Load (new Guid (value));
//				}
//				catch
//				{
//					// LOG: ErrorUserLoadAvatar
//					Services.Logging.LogError (string.Format (Strings.LogError.UserLoadAvatar, value));
//				}
//			}
//		}
					//result.Add ("usergroupids", this.__usergroups_as_string);
//			List<Hashtable> usergroups = new List<Hashtable> ();
//			foreach (Usergroup usergroup in this.Usergroups)
//			{
//				usergroups.Add (usergroup ());
//			}

		public bool Authenticate (Usergroup usergroup)
		{
			bool result = false;

			if (this._usergroups.Find (delegate (Usergroup u) { return u.Id == usergroup.Id;}) != null)
			{
				result = true;
			}

			return result;
		}

//		public bool Authenticate (SorentoLib.Enums.Accesslevel accesslevel)
//		{
//			bool result = false;
//
//			if (this.Accesslevel >= accesslevel)
//			{
//				result = true;
//			}
//
//			return result;
//		}

		public bool Authenticate (string Password)
		{
			bool result = false;

			if (this._status != Enums.UserStatus.Disabled)
			{
				if (this._password == Password)
				{
					result = true;
				}
				else
				{
					if (this._password == SNDK.Crypto.SHAHash (SHAHashAlgorithm.SHA1, Password + Password))
					{
						result = true;
					}
				}
			}

			return result;
		}

		#endregion

	}
}


