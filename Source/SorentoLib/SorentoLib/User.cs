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
using System.Collections;
using System.Collections.Generic;

using SNDK;
using SNDK.DBI;

namespace SorentoLib
{
	[Serializable]
	public class User
	{
		#region Public Static Fields
		public static string DatabaseTableName = SorentoLib.Services.Database.Prefix + "users";
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

				foreach (string id in value.Split (";".ToCharArray ()))
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

		public Enums.Accesslevel Accesslevel
		{
			get
			{
				Enums.Accesslevel result = Enums.Accesslevel.Guest;
				foreach (Usergroup usergroup in this.Usergroups)
				{
					if (usergroup.Type == Enums.UsergroupType.Core)
					{
						if (usergroup.Accesslevel > result)
						{
							result = usergroup.Accesslevel;
						}
					}
				}
				return result;
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

		#region Constructors
		public User (string Username, string Email)
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = Date.CurrentDateTimeToTimestamp ();
			this._usergroups = new List<Usergroup> ();
			this._username = string.Empty;
			this._password = string.Empty;
			this._realname = string.Empty;
			this._email = string.Empty;
			this._avatarid = Guid.Empty;
			this._status = Enums.UserStatus.None;

			// Check if specified username and email is available.
			if (User.IsUsernameInUse (Username))
			{
				// EXCEPTION: UserCreateUsername
				throw new Exception (string.Format (SorentoLib.Strings.Exception.UserCreateUsername, Username));
			}

			if (User.IsEmailInUse (Email))
			{
				// EXCEPTION: UserCreateEmail
				throw new Exception (string.Format (SorentoLib.Strings.Exception.UserCreateEmail, Email));
			}

			// Add default usergroup.
			try
			{
				this._usergroups.Add (Usergroup.Load (Services.Config.Get<Guid> (Enums.ConfigKey.core_defaultusergroup)));
			}
			catch
			{
				// LOG: LogErrorUserCreateDefaultUsergroup
				Services.Logging.LogError (string.Format (Strings.LogError.UserCreateDefaultUsergroup, Services.Config.Get<Guid> (Enums.ConfigKey.core_defaultusergroup)));
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
			this._avatarid = Guid.Empty;
			this._status = Enums.UserStatus.None;
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			bool success = false;
			QueryBuilder qb = null;

			if (!SNDK.DBI.Helpers.GuidExists (Services.Database.Connection, DatabaseTableName, this._id))
			{
				qb = new QueryBuilder (QueryBuilderType.Insert);
				ServiceStatsUpdate ();
			}
			else
			{
				qb = new QueryBuilder (QueryBuilderType.Update);
				qb.AddWhere ("id", "=", this._id);
			}

			this._updatetimestamp = Date.CurrentDateTimeToTimestamp ();

			qb.Table (DatabaseTableName);
			qb.Columns ("id",
			            "createtimestamp",
			            "updatetimestamp",
			            "usergroups",
			            "username",
			            "password",
			            "realname",
			            "email",
			            "avatar",
			            "status");

			qb.Values (this._id,
			           this._createtimestamp,
			           this._updatetimestamp,
			           this.__usergroups_as_string,
			           this._username,
			           this._password,
			           this._realname,
			           this._email,
			           this._avatarid,
			           this._status);

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
				throw new Exception (string.Format (Strings.Exception.UserSave, this._id));
			}
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
			}

			return result;
		}

		public void ToAjaxRespons (SorentoLib.Ajax.Respons Respons)
		{
//			Respons.Data = this.ToAjaxItem ();
		}

		public Hashtable ToAjaxItem ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimestamp", this._updatetimestamp);
			result.Add ("usergroupids", this.__usergroups_as_string);

//			List<Hashtable> usergroups = new List<Hashtable> ();
//			foreach (Usergroup usergroup in this.Usergroups)
//			{
//				usergroups.Add (usergroup.ToAjaxItem ());
//			}
//			result.Add ("usergroups", usergroups);

			result.Add ("username", this._username);
			result.Add ("password", this._password);
			result.Add ("realname", this._realname);
			result.Add ("email", this._email);
			result.Add ("avatarid", this._avatarid);
			result.Add ("accesslevel", this.Accesslevel);
			result.Add ("status", this._status);


			return result;
		}
		#endregion

		#region Public Static Methods
		public static User Load (string Username)
		{
			return Load (Guid.Empty, Username);
		}

		public static User Load (Guid Id)
		{
			return Load (Id, string.Empty);
		}

		private static User Load (Guid Id, string Username)
		{
			bool success = false;
			User result = new User ();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (SorentoLib.User.DatabaseTableName);
			qb.Columns ("id",
			            "createtimestamp",
			            "updatetimestamp",
			            "usergroups",
			            "username",
			            "password",
			            "realname",
			            "email",
			            "avatar",
			            "status");

			if (Id != Guid.Empty)
			{
				qb.AddWhere ("id", "=", Id);
			}
			else
			{
				qb.AddWhere ("username", "=", Username);
			}

			Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);

			if (query.Success)
			{
				if (query.NextRow ())
				{
					result._id = query.GetGuid (qb.ColumnPos ("id"));
					result._createtimestamp = query.GetInt (qb.ColumnPos ("createtimestamp"));
					result._updatetimestamp = query.GetInt (qb.ColumnPos ("updatetimestamp"));
					result.__usergroups_as_string = query.GetString (qb.ColumnPos ("usergroups"));
					result._username = query.GetString (qb.ColumnPos ("username"));
					result._password = query.GetString (qb.ColumnPos ("password"));
					result._realname = query.GetString (qb.ColumnPos ("realname"));
					result._email = query.GetString (qb.ColumnPos ("email"));
					result._avatarid = query.GetGuid (qb.ColumnPos ("avatar"));
					result._status = query.GetEnum<SorentoLib.Enums.UserStatus> (qb.ColumnPos ("status"));

					success = true;
				}
			}

			query.Dispose ();
			query = null;
			qb = null;

			if (!success)
			{
				if (Id != Guid.Empty)
				{
					throw new Exception (string.Format (Strings.Exception.UserLoadGuid, Id));
				}
				else
				{
					throw new Exception (string.Format (Strings.Exception.UserLoadUsername, Username));
				}
			}

			return result;
		}

		public static void Delete (string Username)
		{
			Delete (Guid.Empty, Username);
		}

		public static void Delete (Guid Id)
		{
			Delete (Id, string.Empty);
		}

		private static void Delete (Guid Id, string Username)
		{
			bool success = false;

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
			qb.Table (DatabaseTableName);

			if (Id != Guid.Empty)
			{
				qb.AddWhere ("id", "=", Id);
			}
			else
			{
				qb.AddWhere ("username", "=", Username);
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
				if (Id != Guid.Empty)
				{
					throw new Exception (string.Format (Strings.Exception.UserDeleteGuid, Id));
				}
				else
				{
					throw new Exception (string.Format (Strings.Exception.UserDeleteUsername, Username));
				}
			}
		}

		public static List<User> List()
		{
			return List (Enums.UserListFilter.None, null);
		}

		public static List<User> List(Enums.UserListFilter Filter, Object FilterData)
		{
			List<User> result = new List<User>();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("id");

			switch (Filter) {
				case SorentoLib.Enums.UserListFilter.OnlyUsersThatIsMemberOfUsergroupId:
				{
					qb.AddWhere ("usergroups", "like", "%"+ ((Guid)FilterData).ToString () +"%");
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

		static public bool IsUsernameInUse (string Username)
		{
			return IsUsernameInUse (Username, Guid.Empty);
		}

		static public bool IsUsernameInUse (string Username, Guid FilterOutUserId)
		{
			bool result = false;

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("id");
			qb.AddWhere ("username", "=", Username);

			if (FilterOutUserId != Guid.Empty)
			{
				qb.AddWhereAND ();
				qb.AddWhere ("id", "!=", FilterOutUserId);
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

		static public bool IsEmailInUse (string Email, Guid FilterOutUserId)
		{
			bool result = false;

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("id");
			qb.AddWhere ("email", "=", Email);

			if (FilterOutUserId != Guid.Empty)
			{
				qb.AddWhereAND ();
				qb.AddWhere ("id", "!=", FilterOutUserId);
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

		public static User FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return User.FromAjaxItem (Request.Data);
		}

		public static User FromAjaxItem (Hashtable Item)
		{
			User result = null;

			Guid id = Guid.Empty;

			try
			{
				id = new Guid ((string)Item["id"]);
			}
			catch {}

			if (id != Guid.Empty)
			{
				try
				{
					result = User.Load (id);
				}
				catch
				{
					result = new User ();
					result._id = id;
					if (Item.ContainsKey ("createtimestamp"))
					{
						result._createtimestamp = int.Parse ((string)Item["createtimestamp"]);
					}

					if (Item.ContainsKey ("updatetimestamp"))
					{
						result._createtimestamp = int.Parse ((string)Item["updatetimestamp"]);
					}
				}
			}
			else
			{
				try
				{
					result = new User ((string)Item["username"], (string)Item["email"]);
				}
				catch (Exception E)
				{
					throw new Exception (string.Format (Strings.Exception.UserFromAjaxItem, E.ToString ()));
				}
			}

			if (Item.ContainsKey ("usergroupids"))
			{
				result.__usergroups_as_string = (string)Item["usergroupids"];
			}

			if (Item.ContainsKey ("username"))
			{
				result.Username = (string)Item["username"];
			}

			if (Item.ContainsKey ("password"))
			{
				if ((string)Item["password"] != string.Empty)
				{
					result._password = (string)Item["password"];
				}
			}

			if (Item.ContainsKey ("email"))
			{
				result.Email = (string)Item["email"];
			}

			if (Item.ContainsKey ("realname"))
			{
				result._realname = (string)Item["realname"];
			}

			if (Item.ContainsKey ("avatarid"))
			{
				try
				{
					result._avatarid = new Guid ((string)Item["avatarid"]);
				}
				catch {}
			}

			if (Item.ContainsKey ("status"))
			{
				result._status = SNDK.Convert.StringToEnum<SorentoLib.Enums.UserStatus> ((string)Item["status"]);
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
	}
}