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
	public class User
	{
		#region Public Static Fields
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
		private Enums.UserStatus _status;

		private string _usergroupsasstring
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

				return result;
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

			// Add default usergroup.
			try
			{
				this._usergroups.Add (Usergroup.Load (Services.Config.Get<Guid> (Enums.ConfigKey.core_defaultusergroupid)));
			}
			catch
			{
				// LOG: LogErrorUserCreateDefaultUsergroup
//				Services.Logging.LogError (string.Format (Strings.LogError.UserCreateDefaultUsergroup, Services.Config.Get<Guid> (Enums.ConfigKey.core_defaultusergroupid)));
				Services.Logging.LogError (Strings.LogError.UserCreateDefaultUsergroup);
			}
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
			this._status = Enums.UserStatus.Disabled;
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			try
			{
				Services.Datastore.Meta meta = new Services.Datastore.Meta ();
				meta.Add ("id", this._id);
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
			result.Add ("usergroups", this._usergroups);
			result.Add ("username", this._username);
			result.Add ("password", this._password);
			result.Add ("realname", this._realname);
			result.Add ("email", this._email);
			result.Add ("status", this._status);

			return SNDK.Convert.ToXmlDocument (result, this.GetType ().FullName.ToLower ());
		}
		#endregion

		#region Private Static Methods
		private static User Load (Guid id, string username)
		{
			User result = default (User);
			bool success = false;

			try
			{
				Hashtable item;

				if (id != Guid.Empty)
				{
					item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (Services.Datastore.Get<XmlDocument> (DatastoreAisle, id.ToString ()).SelectSingleNode ("(//sorentolib.user)[1]")));
					success = true;
				}
				else
				{
					item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (Services.Datastore.Get<XmlDocument> (DatastoreAisle, new Services.Datastore.MetaSearch ("username", Enums.DatastoreMetaSearchCondition.Equal, username))));
					success = true;
				}

				if (item.ContainsKey ("id"))
				{
					result._id = new Guid ((string)item["id"]);
				}
				else
				{
					throw new Exception (string.Empty);
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
					result._usergroupsasstring = item["usergroups"];
				}

				if (item.ContainsKey ("username"))
				{
					result._username = (string)item["username"];
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
					result._email = (string)item["email"];
				}

				if (item.ContainsKey ("realname"))
				{
					result._realname = (string)item["realname"];
				}

				if (item.ContainsKey ("status"))
				{
					result._status = SNDK.Convert.StringToEnum<SorentoLib.Enums.UserStatus> ((string)item["status"]);
				}
			}
			catch {}

			if (!success)
			{
				if (id != Guid.Empty)
				{
					throw new Exception (string.Format (Strings.Exception.UserLoadGuid, id));
				}
				else
				{
					throw new Exception (string.Format (Strings.Exception.UserLoadUsername, username));
				}
			}

			return result;
		}

		private static void Delete (Guid id, string username)
		{
			bool success = false;

			try
			{
				if (id != Guid.Empty)
				{
					Services.Datastore.Delete (DatastoreAisle, id.ToString ());
					success = true;
				}
				else
				{
					Services.Datastore.Delete (DatastoreAisle, new Services.Datastore.MetaSearch ("username", Enums.DatastoreMetaSearchCondition.Equal, username));
					success = true;
				}
			}
			catch {}

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

		public static void Delete (string username)
		{
			Delete (Guid.Empty, username);
		}

		public static void Delete (Guid id)
		{
			Delete (id, string.Empty);
		}

		public static List<User> List ()
		{
			List<User> result = new List<User> ();

			foreach (string shelf in Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				result.Add (User.Load (new Guid (shelf)));
			}

			return result;
		}

		static public bool IsUsernameInUse (string username)
		{
			return IsUsernameInUse (username, Guid.Empty);
		}

		static public bool IsUsernameInUse (string username, Guid filterOutUserId)
		{
			bool result = false;

			if (Services.Datastore.FindShelf (DatastoreAisle, new Services.Datastore.MetaSearch ("username", Enums.DatastoreMetaSearchCondition.Equal, username), new Services.Datastore.MetaSearch ("id", Enums.DatastoreMetaSearchCondition.NotEqual, filterOutUserId)) != string.Empty)
			{
				result = true;
			}

			return result;
		}

		static public bool IsEmailInUse (string Email)
		{
			return IsEmailInUse(Email, Guid.Empty);
		}

		static public bool IsEmailInUse (string Email, Guid filterOutUserId)
		{
			bool result = false;

			if (Services.Datastore.FindShelf (DatastoreAisle, new Services.Datastore.MetaSearch ("email", Enums.DatastoreMetaSearchCondition.Equal, Email), new Services.Datastore.MetaSearch ("id", Enums.DatastoreMetaSearchCondition.NotEqual, filterOutUserId)) != string.Empty)
			{
				result = true;
			}

			return result;
		}

		public static User FromXmlDocument ()
		{

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
				throw new Exception (Strings.Exception.UserFromXMLDocument);
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
				User.Delete (user.Id);
			}
		}

		internal static void ServiceConfigChanged ()
		{
//			DatabaseTableName = SorentoLib.Services.Database.Prefix + "users";
		}

		internal static void ServiceStatsUpdate ()
		{
//			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
//			qb.Table (DatabaseTableName);
//			qb.Columns("id");
//
//			Query query = Services.Database.Connection.Query (qb.QueryString);
//			if (query.Success)
//			{
//				int totalusers = 0;
//				while (query.NextRow())
//				{
//					totalusers++;
//				}
//
//				Services.Stats.Set ("sorentolib.user.totalusers", totalusers);
//			}
//
//			query.Dispose ();
//			query = null;
//			qb = null;

			Services.Logging.LogDebug (Strings.LogDebug.UserStats);
		}
		#endregion

		#region OLD
		public static List<User> List (Enums.UserListFilter filter, object filterData)
		{
			List<User> result = new List<User>();

//			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
//			qb.Table (DatabaseTableName);
//			qb.Columns ("id");
//
//			switch (filter) {
//				case SorentoLib.Enums.UserListFilter.OnlyUsersThatIsMemberOfUsergroupId:
//				{
//					qb.AddWhere ("usergroups", "like", "%"+ ((Guid)filterData).ToString () +"%");
//					break;
//				}
//			}
//
//			Query query = Services.Database.Connection.Query (qb.QueryString);
//			if (query.Success)
//			{
//				while (query.NextRow ())
//				{
//					User user = Load (query.GetGuid (qb.ColumnPos ("id")));
//					result.Add (user);
//				}
//			}
//
//			query.Dispose ();
//			query = null;
//			qb = null;

			return result;
		}

		public bool Authenticate (Usergroup usergroup)
		{
			bool result = false;

			if (this._usergroups.Find (delegate (Usergroup u) { return u.Id == usergroup.Id;}) != null)
			{
				result = true;
			}

			return result;
		}

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


//		private string __usergroups_as_string
//		{
//			get
//			{
//				string result = string.Empty;
//				foreach (Usergroup usergroup in this._usergroups)
//				{
//					// Remove duplicates
//					if (result.Contains (usergroup.Id.ToString ()))
//					{
//						continue;
//					}
//
//					result += usergroup.Id.ToString () + ";";
//				}
//
//				return  result.TrimEnd (";".ToCharArray ());
//			}
//
//			set
//			{
//				this._usergroups.Clear ();
//
//				foreach (string id in value.Split (";".ToCharArray (), StringSplitOptions.RemoveEmptyEntries))
//				{
//					try
//					{
//						this._usergroups.Add (Usergroup.Load (new Guid (id)));
//					}
//					catch
//					{
//						// LOG: LogErrorUserLoadUsergroup
//						Services.Logging.LogError (string.Format (Strings.LogError.UserLoadUsergroup, id));
//					}
//				}
//			}
//		}

		#endregion
	}
}


