//
// Usergroup.cs
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

namespace SorentoLib
{
	[Serializable]
	public class Usergroup
	{
		#region Static Fields
		public static string DatabaseTableName = SorentoLib.Services.Database.Prefix + "usergroups";
		private static List<Usergroup> BuiltInUsergroups = new List<Usergroup> ();
		#endregion

		#region Private Fields
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		private SorentoLib.Enums.UsergroupType _type;
		private string _name;
//		private Enums.Accesslevel _accesslevel;
		private Enums.UsergroupStatus _status;
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

		public SorentoLib.Enums.UsergroupType Type
		{
			get
			{
				return this._type;
			}

			set
			{
				this._type = value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}

			set
			{
				this._name = value;
			}
		}

//		public Enums.Accesslevel Accesslevel
//		{
//			get
//			{
//				return this._accesslevel;
//			}
//
//			set
//			{
//				this._accesslevel = value;
//			}
//		}

		public Enums.UsergroupStatus Status
		{
			get
			{
				return this._status;
			}

			set
			{
				this._status = value;
			}
		}
		#endregion

		#region Constructors
		public Usergroup()
		{
			this._id = Guid.NewGuid();
			this._createtimestamp = Date.CurrentDateTimeToTimestamp();
			this._updatetimestamp = Date.CurrentDateTimeToTimestamp();
			this._type = SorentoLib.Enums.UsergroupType.Custom;
			this._name = string.Empty;
//			this._accesslevel = Enums.Accesslevel.Guest;
			this._status = Enums.UsergroupStatus.Enabled;
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
				qb.AddWhere("id", "=", this._id);
			}

			qb.Table (DatabaseTableName);
			qb.Columns
				(
					"id",
			            "createtimestamp",
			            "updatetimestamp",
			            "type",
			            "name",
//			            "accesslevel",
			            "status"
				);

			qb.Values
				(
					this._id,
					this._createtimestamp,
					this._updatetimestamp,
					this._type,
					this._name,
//					this._accesslevel,
					this._status
				);
				
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
				throw new Exception (string.Format (Strings.Exception.UsergroupSave, this._id));
			}
		}

		public XmlDocument ToXmlDocument ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimestamp", this._updatetimestamp);
			result.Add ("type", this._type);
			result.Add ("name", this._name);
//			result.Add ("accesslevel", this._accesslevel);
			result.Add ("status", this._status);

			return SNDK.Convert.ToXmlDocument (result, this.GetType ().FullName.ToLower ());
		}
		#endregion

		#region Public Static Methods
		public static Usergroup Load (Guid id)
		{
			bool success = false;

			Usergroup result = Usergroup.BuiltInUsergroups.Find (delegate (Usergroup u) { return u.Id == id;});

			if (result == null)
			{
				result = new Usergroup ();

				QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
	
				qb.Table (DatabaseTableName);
				qb.Columns
					(
						"id",
				            "createtimestamp",
				            "updatetimestamp",
				            "type",
				            "name",
//				            "accesslevel",
				            "status"
					);
	
				qb.AddWhere ("id", "=", id);
	
				Query query = Services.Database.Connection.Query (qb.QueryString);
				if (query.Success)
				{
					if (query.NextRow ())
					{
						result._id = query.GetGuid (qb.ColumnPos ("id"));
						result._createtimestamp = query.GetInt(qb.ColumnPos("createtimestamp"));
						result._updatetimestamp = query.GetInt(qb.ColumnPos("updatetimestamp"));
						result._type = query.GetEnum<Enums.UsergroupType> (qb.ColumnPos ("type"));
						result._name = query.GetString(qb.ColumnPos("name"));
//						result._accesslevel = query.GetEnum<Enums.Accesslevel> (qb.ColumnPos ("accesslevel"));
						result._status = query.GetEnum<Enums.UsergroupStatus> (qb.ColumnPos ("status"));
	
						success = true;
					}
				}
	
				query.Dispose ();
				query = null;
				qb = null;
			}
			else
			{
				success = true;
			}

			if (!success)
			{
				throw new Exception (string.Format (Strings.Exception.UsergroupLoad, id));
			}

			return result;
		}

		public static void Delete (Guid id)
		{
			bool success = false;

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);

			qb.Table (DatabaseTableName);
			qb.AddWhere ("id", "=", id);

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
				throw new Exception (string.Format (Strings.Exception.UsergroupDelete, id));
			}
		}
		
		public static List<Usergroup> List ()
		{
			return List (Enums.UsergroupListFilter.None, null);
		}
							
		public static List<Usergroup> List(Enums.UsergroupListFilter filter , object filterData)
		{
			List<Usergroup> result = new List<Usergroup>();

			QueryBuilder qb = new SNDK.DBI.QueryBuilder (QueryBuilderType.Select);

			qb.Table (DatabaseTableName);
			qb.Columns ("id");

			switch (filter)
			{
				case SorentoLib.Enums.UsergroupListFilter.ExcludeUsergroupsThatUserIdIsMemberOf:
				{
					User user = User.Load ((Guid)filterData);

					foreach (Usergroup usergroup in user.Usergroups)
					{
						qb.AddWhere ("id", "!=", usergroup.Id);
					}

					user = null;
					break;
				}

				case SorentoLib.Enums.UsergroupListFilter.ExcludeUsergroupsThatUsernameIsMemberOf:
				{
					User user = User.Load ((string)filterData);

					foreach (Usergroup usergroup in user.Usergroups)
					{
						qb.AddWhere ("id", "!=", usergroup.Id);
					}

					user = null;
					break;
				}
			}

			Query query = Services.Database.Connection.Query (qb.QueryString);
			if (query.Success) 
			{
				while (query.NextRow ())
				{
					try
					{
						Usergroup usergroup = Usergroup.Load (query.GetGuid (qb.ColumnPos ("id")));
						result.Add (usergroup);
						usergroup = null;
					}
					catch
					{
						Services.Logging.LogError (string.Format (Strings.LogError.UsergroupListUsergroup, query.GetGuid (qb.ColumnPos ("id"))));
					}
				}
			}

			query.Dispose ();
			query = null;
			qb = null;

			foreach (Usergroup usergroup in BuiltInUsergroups)
			{
				result.Add (usergroup);
			}

			return result;
		}

		public static Usergroup AddBuildInUsergroup (Guid id, string name)
		{
			SorentoLib.Usergroup result = new SorentoLib.Usergroup ();
			result._id = id;
			result._type = SorentoLib.Enums.UsergroupType.BuildIn;
			result._name = name;
			BuiltInUsergroups.Add (result);

			return result;
		}

		public static Usergroup FromXmlDocument (XmlDocument xmlDocument)
		{
			Hashtable item = (Hashtable)SNDK.Convert.FromXmlDocument (xmlDocument);

			Usergroup result;

			if (item.ContainsKey ("id"))
			{
				try
				{
					result = Usergroup.Load (new Guid ((string)item["id"]));
				}
				catch
				{
					result = new Usergroup ();
					result._id = new Guid ((string)item["id"]);
				}
			}
			else
			{
				throw new Exception ("USERGROUP1");
			}

			if (item.ContainsKey ("type"))
			{
				result._type = SNDK.Convert.StringToEnum<SorentoLib.Enums.UsergroupType> ((string)item["type"]);
			}

			if (item.ContainsKey ("name"))
			{
				result._name = (string)item["name"];
			}

//			if (item.ContainsKey ("accesslevel"))
//			{
//				result._accesslevel = SNDK.Convert.StringToEnum<SorentoLib.Enums.Accesslevel> ((string)item["accesslevel"]);
//			}

			if (item.ContainsKey ("status"))
			{
				result._status = SNDK.Convert.StringToEnum<SorentoLib.Enums.UsergroupStatus> ((string)item["status"]);
			}

			return result;
		}
		#endregion

		#region Internal Static Methods
		internal static void Purge ()
		{
			foreach (Usergroup usergroup in Usergroup.List ())
			{
				QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
				qb.Table (DatabaseTableName);
				qb.AddWhere ("id", "=", usergroup.Id);

				Query query = Services.Database.Connection.Query (qb.QueryString);
				query.Dispose ();
				query = null;
				qb = null;
			}
		}

		internal static void ServiceConfigChanged ()
		{
			DatabaseTableName = SorentoLib.Services.Database.Prefix + "usergroups";
		}
		#endregion
	}
}

#region OLD
			// TODO: this should probally be done by the render.
			//result.Sort(delegate(Usergroup o1, Usergroup o2) { return o1._name.CompareTo(o2._name); });

#endregion
