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
		private string _scope;

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
						// LOG: LogError.UserLoadUsergroup
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
				if (value != string.Empty)
				{
					if (User.IsUsernameInUse (value, this._id))
					{
						// EXCEPTION: Exception.UserSetUsername
						throw new Exception (string.Format (Strings.Exception.UserSetUsername, value));
					}

					this._username = value;
				}
				else
				{
					// EXCEPTION: Exception.UserSetUsernameStringEmpty
					throw new Exception (string.Format (Strings.Exception.UserSetUsernameStringEmpty));
				}
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
				if (value != string.Empty)
				{
					if (User.IsEmailInUse (value, this._id))
					{
						// EXCEPTION: Exception.UserSetEmail
						throw new Exception (string.Format (Strings.Exception.UserSetEmail, value));
					}
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

		public string Scope 
		{
			get
			{
				return this._scope;
			}

			set
			{
				this._scope = value;
			}
		}
		#endregion

		#region Constructor
		public User (string username)
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = Date.CurrentDateTimeToTimestamp ();
			this._usergroups = new List<Usergroup> ();
			this._username = username;
			this._password = string.Empty;
			this._realname = string.Empty;
			this._email = string.Empty;
			this._status = Enums.UserStatus.Disabled;
			this._scope = string.Empty;
			
			// Check if specified username is available.
			if (User.IsUsernameInUse (username))
			{
				// EXCEPTION: Exception.UserCreateUsername
				throw new Exception (string.Format (SorentoLib.Strings.Exception.UserCreateUsername, username));
			}
								
			// Add default usergroup.
			try
			{
				this._usergroups.Add (Usergroup.Load (Services.Config.Get<Guid> (Enums.ConfigKey.core_defaultusergroupid)));
			}
			catch
			{
				// LOG: LogError.UserCreateDefaultUsergroup
				//				Services.Logging.LogError (string.Format (Strings.LogError.UserCreateDefaultUsergroup, Services.Config.Get<Guid> (Enums.ConfigKey.core_defaultusergroupid)));
				Services.Logging.LogError (Strings.LogError.UserCreateDefaultUsergroup);
			}
		}

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
			this._scope = string.Empty;

			// Check if specified username is available.
			if (User.IsUsernameInUse (username))
			{
				// EXCEPTION: Exception.UserCreateUsername
				throw new Exception (string.Format (SorentoLib.Strings.Exception.UserCreateUsername, username));
			}

			// Check if specified email is available.
			if (User.IsEmailInUse (email))
			{
				// EXCEPTION: Exception.UserCreateEmail
				throw new Exception (string.Format (SorentoLib.Strings.Exception.UserCreateEmail, email));
			}

			// Add default usergroup.
			try
			{
				this._usergroups.Add (Usergroup.Load (Services.Config.Get<Guid> (Enums.ConfigKey.core_defaultusergroupid)));
			}
			catch
			{
				// LOG: LogError.UserCreateDefaultUsergroup
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
			this._scope = string.Empty;
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
				item.Add ("usergroupids", this._usergroupsasstring);
				item.Add ("username", this._username);
				item.Add ("password", this._password);
				item.Add ("realname", this._realname);
				item.Add ("email", this._email);
				item.Add ("status", this._status);
				item.Add ("scope", this._scope);

				Services.Datastore.Meta meta = new Services.Datastore.Meta ();
				meta.Add ("id", this._id);
				meta.Add ("username", this._username);
				meta.Add ("email", this._email);
				meta.Add ("scope", this._scope);

				//Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Convert.ToXmlDocument (item, this.GetType ().FullName.ToLower ()), meta);
				Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Convert.ToXmlDocument (item, "sorentolib.user"), meta);
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.USER", exception.Message));

				// EXCEPTION: Exception.UserSave
				throw new Exception (string.Format (Strings.Exception.UserSave, this._id));
			}
	}

		public bool Authenticate (Usergroup usergroup)
		{
			return Authenticate (usergroup.Id);
		}

		public bool Authenticate (Guid usergroupid)
		{
			bool result = false;

			if (this._usergroups.Find (delegate (Usergroup u) { return u.Id == usergroupid;}) != null)
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

		public XmlDocument ToXmlDocument ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimestamp", this._updatetimestamp);
			result.Add ("usergroups", this._usergroups);
			result.Add ("username", this._username);
			result.Add ("realname", this._realname);
			result.Add ("email", this._email);
			result.Add ("status", this._status);
			result.Add ("scope", this._scope);

			return SNDK.Convert.ToXmlDocument (result, "sorentolib.user");
		}
		#endregion

		#region Private Static Methods
		private static User Load (Guid id, string username)
		{
			User result = new User ();

			try
			{
				Hashtable item;

				if (id != Guid.Empty)
				{
					item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (Services.Datastore.Get<XmlDocument> (DatastoreAisle, id.ToString ()).SelectSingleNode ("(//sorentolib.user)[1]")));
				}
				else
				{
					item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (Services.Datastore.Get<XmlDocument> (DatastoreAisle, new Services.Datastore.MetaSearch ("username", Enums.DatastoreMetaSearchComparisonOperator.Equal, username)).SelectSingleNode ("(//sorentolib.user)[1]")));
				}

				result._id = new Guid ((string)item["id"]);

				if (item.ContainsKey ("createtimestamp"))
				{
					result._createtimestamp = int.Parse ((string)item["createtimestamp"]);
				}

				if (item.ContainsKey ("updatetimestamp"))
				{
					result._updatetimestamp = int.Parse ((string)item["updatetimestamp"]);
				}

				if (item.ContainsKey ("usergroupids"))
				{
					result._usergroupsasstring = (string)item["usergroupids"];
				}

				if (item.ContainsKey ("username"))
				{
					result._username = (string)item["username"];
				}

				if (item.ContainsKey ("password"))
				{
					result._password = (string)item["password"];
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

				if (item.ContainsKey ("scope"))
				{
					result._scope = (string)item["scope"];
				}
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.USER", exception.Message));

				Console.WriteLine (exception);

				if (id != Guid.Empty)
				{
					// EXCEPTION: Exception.UserLoadGuid
					throw new Exception (string.Format (Strings.Exception.UserLoadGuid, id));
				}
				else
				{
					// EXCEPTION: Exception.UserLoadUsername
					throw new Exception (string.Format (Strings.Exception.UserLoadUsername, username));
				}
			}

			return result;
		}

		private static void Delete (Guid id, string username)
		{
			try
			{
				if (id != Guid.Empty)
				{
					Services.Datastore.Delete (DatastoreAisle, id.ToString ());
				}
				else
				{
					Services.Datastore.Delete (DatastoreAisle, new Services.Datastore.MetaSearch ("username", Enums.DatastoreMetaSearchComparisonOperator.Equal, username));
				}

				ServiceStatsUpdate ();
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.USER", exception.Message));

				if (id != Guid.Empty)
				{
					// EXCEPTION: Exception.UserDeleteGuid
					throw new Exception (string.Format (Strings.Exception.UserDeleteGuid, id));
				}
				else
				{
					// EXCEPTION: Exception.UserDeleteUsername
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
				result.Add (Load (new Guid (shelf)));
			}
			
			return result;
		}

		public static List<User> List (string Scope)
		{
			List<User> result = new List<User> ();

			Services.Datastore.MetaSearch metasearch = new SorentoLib.Services.Datastore.MetaSearch ("scope", Enums.DatastoreMetaSearchComparisonOperator.Equal, Scope);

			foreach (string shelf in Services.Datastore.ListOfShelfs (DatastoreAisle, metasearch))
			{
				result.Add (Load (new Guid (shelf)));
			}

			return result;
		}

		public static bool IsUsernameInUse (string username)
		{
			return IsUsernameInUse (username, Guid.Empty);
		}

		public static bool IsUsernameInUse (string username, Guid filterOutUserId)
		{
			bool result = false;

			if (Services.Datastore.FindShelf (DatastoreAisle, new Services.Datastore.MetaSearch ("username", Enums.DatastoreMetaSearchComparisonOperator.Equal, username), new Services.Datastore.MetaSearch (Enums.DatastoreMetaSearchLogicOperator.And), new Services.Datastore.MetaSearch ("id", Enums.DatastoreMetaSearchComparisonOperator.NotEqual, filterOutUserId)) != string.Empty)
			{
				result = true;
			}

			return result;
		}

		public static bool IsEmailInUse (string Email)
		{
			return IsEmailInUse(Email, Guid.Empty);
		}

		public static bool IsEmailInUse (string Email, Guid filterOutUserId)
		{
			bool result = false;

			if (Services.Datastore.FindShelf (DatastoreAisle, new Services.Datastore.MetaSearch ("email", Enums.DatastoreMetaSearchComparisonOperator.Equal, Email), new Services.Datastore.MetaSearch (Enums.DatastoreMetaSearchLogicOperator.And), new Services.Datastore.MetaSearch ("id", Enums.DatastoreMetaSearchComparisonOperator.NotEqual, filterOutUserId)) != string.Empty)
//			if (Services.Datastore.FindShelf (DatastoreAisle, new Services.Datastore.MetaSearch ("email", Enums.DatastoreMetaSearchComparisonOperator.Equal, Email), new Services.Datastore.MetaSearch (Enums.DatastoreMetaSearchLogicOperator.And)) != string.Empty)
			{
//				Console.WriteLine ("IN USE");
				result = true;
			}
//			Console.WriteLine ("NOT IN USE");

			return result;
		}

		public static User FromXmlDocument (XmlDocument xmlDocument)
		{
			Hashtable item;
			User result;

			try
			{
				item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (xmlDocument.SelectSingleNode ("(//sorentolib.user)[1]")));
			}
			catch
			{
				item = (Hashtable)SNDK.Convert.FromXmlDocument (xmlDocument);
			}

			if (item.ContainsKey ("id"))
			{
				try
				{
					result = Load (new Guid ((string)item["id"]));
				}
				catch
				{
					result = new User ();
					result._id = new Guid ((string)item["id"]);
					result._username = (string)item["name"];
					result._email = (string)item["email"];
				}
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

			if (item.ContainsKey ("scope"))
			{
				result._scope = (string)item["scope"];
			}

			return result;
		}
		#endregion

		#region Internal Static Methods
		internal static void ServiceStatsUpdate ()
		{
			Services.Stats.Set (Enums.StatKey.sorentolib_user_count, Services.Datastore.NumberOfShelfsInAisle (DatastoreAisle));

			// LOG: LogDebug.UserStats
			Services.Logging.LogDebug (Strings.LogDebug.UserStats);
		}
		#endregion
	}
}


