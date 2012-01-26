//
// Datastore.cs
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
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using SNDK;
using SNDK.DBI;

namespace SorentoLib.Services
{
	public class Datastore
	{
		#region Static Fields
		public static string DatabaseTableName = Services.Database.Prefix + "datastore";
		#endregion

		#region Private Fields
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		private string _aisle;
		private string _shelf;
		private string _data;
		private Meta _meta;
		#endregion

		#region Constructor
		private Datastore ()
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = Date.CurrentDateTimeToTimestamp ();
			this._aisle = string.Empty;
			this._shelf = string.Empty;
			this._data = string.Empty;
		}
		#endregion

		#region Private Methods
		private void Save ()
		{
			bool success = false;
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();

			QueryBuilder qb = null;
			if (!SNDK.DBI.Helpers.GuidExists (Services.Database.Connection, DatabaseTableName, this._id))
			{
				qb = new SNDK.DBI.QueryBuilder (QueryBuilderType.Insert);
			}
			else
			{
				qb = new SNDK.DBI.QueryBuilder (QueryBuilderType.Update);
				qb.AddWhere ("id", "=", this._id);
			}

			qb.Table (Services.Datastore.DatabaseTableName);
			qb.Columns ("id",
					"createtimestamp",
					"updatetimestamp",
					"aisle",
					"shelf",
					"data",
					"meta"
				);

			qb.Values
				(
					this._id,
					this._createtimestamp,
					this._updatetimestamp,
					this._aisle,
					this._shelf,
					this._data,
					this._meta.ToString ()
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
				throw new Exception (string.Format (Strings.Exception.ServicesDatastoreSave, this._id));
			}
		}
		#endregion

		#region Private Static Methods
		private static Datastore Load (string Aisle, string Shelf)
		{
			return Load (Guid.Empty, Aisle, Shelf, new Hashtable ());
		}

		private static Datastore Load (Guid Id)
		{
			return Load (Id, string.Empty, string.Empty, new Hashtable ());
		}

		private static Datastore Load (Guid Id, string Aisle, string Shelf, Hashtable Meta)
		{
			bool success = false;
			Datastore result = new Datastore ();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);

			qb.Table (Services.Datastore.DatabaseTableName);
			qb.Columns ("id",
			            "createtimestamp",
			            "updatetimestamp",
			            "aisle",
			            "shelf",
			            "data");

			if (Id != Guid.Empty)
			{
				qb.AddWhere ("id", "=", Id);
			}
			else
			{
				qb.AddWhere ("aisle = '"+ Aisle +"' AND Shelf = '" + Shelf + "'");
			}

			Query query = Services.Database.Connection.Query (qb.QueryString);

			if (query.Success)
			{
				if (query.NextRow ())
				{
					result._id = query.GetGuid (qb.ColumnPos ("id"));
					result._createtimestamp = query.GetInt (qb.ColumnPos ("createtimestamp"));
					result._updatetimestamp = query.GetInt (qb.ColumnPos ("updatetimestamp"));
					result._aisle = query.GetString (qb.ColumnPos ("aisle"));
					result._shelf = query.GetString (qb.ColumnPos ("shelf"));
					result._data = query.GetString (qb.ColumnPos ("data"));

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
					throw new Exception (string.Format (Strings.Exception.ServicesDatastoreLoadGuid, Id));
				}
				else
				{
					throw new Exception (string.Format (Strings.Exception.ServicesDatastoreLoadLocation, Aisle +"."+ Shelf));
				}
			}

			return result;
		}

		private static string Get (string Aisle, string Shelf)
		{
			string result = string.Empty;

			try
			{
				Datastore datastore = Datastore.Load (Aisle, Shelf);
				result = datastore._data;
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.ServicesDatastoreLocationNotFound, Aisle +"."+ Shelf));
			}

			return result;
		}

		private static void Delete (Guid Id, string Aisle, string Shelf)
		{
			bool success = false;

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
			qb.Table (SorentoLib.Services.Datastore.DatabaseTableName);

			if (Id != Guid.Empty)
			{
				qb.AddWhere ("id", "=", Id);
			}
			else
			{
				qb.AddWhere ("aisle = '"+ Aisle +"' AND Shelf = '" + Shelf + "'");
			}

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
				if (Id != Guid.Empty)
				{
					throw new Exception (string.Format (Strings.Exception.ServicesDatastoreDeleteGuid, Id));
				}
				else
				{
					throw new Exception (string.Format (Strings.Exception.ServicesDatastoreDeleteLocation, Aisle +"."+ Shelf));
				}
			}
		}
		#endregion

		#region Public Static Methods
//		public static T Get<T> (string Aisle, Hashtable Meta)
//		{
//			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
//			qb.Table (DatabaseTableName);
//			qb.Columns ("shelf");
//			qb.AddWhere ("aisle", "=", Aisle);
//
//			foreach (string key in Meta.Keys)
//			{
//				string metakey = ((string)key.Split ("|".ToCharArray ())[0]).ToUpper ().Replace (":", "_");
//				string metacondition = key.Split ("|".ToCharArray ())[1];
//				string metadata = Meta[key].ToString ().Replace (":","_").Replace ("|","_");
//
//				qb.AddWhereAND ();
//
//				switch (metacondition)
//				{
//					case "=":
//						qb.AddWhere ("meta", "like", "%"+metakey +":"+ metadata +"%");
//						break;
//
//					case "!=":
//						qb.AddWhere ("meta", "not like", "%"+metakey +":"+ metadata +"%");
//						break;
//				}
//			}
//
//			Query query = Services.Database.Connection.Query (qb.QueryString);
//			if (query.Success)
//			{
//				while (query.NextRow ())
//				{
////					result.Add (query.GetString (qb.ColumnPos ("shelf")));
//				}
//			}
//
//			query.Dispose ();
//			query = null;
//			qb = null;
//		}

		public static string FindShelf (string Aisle, params MetaSearch[] MetaSearch)
		{
			string result = string.Empty;

			List<string> shelfs = ListOfShelfs (Aisle, MetaSearch);
			string shelf = string.Empty;

			if (shelfs.Count > 0)
			{
				result = shelfs[0];
			}

			return result;
		}

		public static T Get<T> (string Aisle, params MetaSearch[] MetaSearch)
		{
			List<string> shelfs = ListOfShelfs (Aisle, MetaSearch);
			string shelf = string.Empty;

			if (shelfs.Count > 0)
			{
				shelf = shelfs[0];
				return Get<T> (Aisle, shelf);
			}
			else
			{
				return default(T);
			}
		}

		public static T Get<T> (string Aisle, string Shelf)
		{
			try
			{
				switch (typeof (T).Name.ToLower ())
				{
//					case "guid":
//						return (T)System.Convert.ChangeType (new Guid (Get (Aisle, Shelf)), typeof(T));
//
//					case "list`1":
//						return (T)System.Convert.ChangeType (Serializer.DeSerializeObjectFromString<T> (Get (Aisle, Shelf)), typeof(T));
//
					case "xmldocument":
					{
						return (T)System.Convert.ChangeType (SNDK.Convert.StringToXmlDocument (Get (Aisle, Shelf)), typeof(T));
					}

					default:
						XmlDocument xml = new XmlDocument ();
						xml.Load (new StringReader (Get (Aisle, Shelf)));
//
						return (T)typeof (T).GetMethod ("FromXmlDocument").Invoke (null, new Object[] { xml });

//						return (T)System.Console
//						return (T)System.Convert.ChangeType (Get (Aisle, Shelf), typeof(T));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine (e);
				throw new Exception (string.Format (Strings.Exception.ServicesDatastoreLocationNotValidType, Aisle +"."+ Shelf, typeof (T).Name));
			}
		}

		public static void Set (string Aisle, string Shelf, Object Data)
		{
			Set (Aisle, Shelf, Data, new Meta ());
		}

		public static void Set (string Aisle, string Shelf, Object Data, Meta Meta)
		{
			Datastore datastore = null;

			try
			{
				datastore = Datastore.Load (Aisle, Shelf);
			}
			catch
			{
				datastore = new Datastore ();
			}

			datastore._aisle = Aisle;
			datastore._shelf = Shelf;
			datastore._meta = Meta;

			if (Data != null)
			{
				switch (Data.GetType ().Name.ToLower ())
				{
//					case "boolean":
//						datastore._data = SNDK.Convert.BoolToString (Data);
//						break;
//
//					case "enum":
//						datastore._data = SNDK.Convert.EnumToString (Data);
//						break;
//
//					case "int":
//						datastore._data = Data.ToString ();
//
//					case "decimal":
//						datastore._data = Data.ToString ().Replace (",", ".");
//						break;
//
//					case "list`1":
//						datastore._data = Serializer.SerializeObjectToString (Data);
//						break;

					case "xmldocument":
					{
						datastore._data = ((XmlDocument)Data).InnerXml;
						break;
					}

					default:
						datastore._data = SNDK.Convert.ToXmlDocument (Data).InnerXml;
						break;
				}
			}

			datastore.Save ();
		}

		public static void Delete (string Aisle, params MetaSearch[] MetaSearch)
		{
			foreach (string shelf in ListOfShelfs (Aisle, MetaSearch))
			{
				Delete (Aisle, shelf);
			}
		}

		public static void Delete (string Aisle, string Shelf)
		{
			Delete (Guid.Empty, Aisle, Shelf);
		}

		public static void Delete (Guid Id)
		{
			Delete (Id, string.Empty, string.Empty);
		}

		public static List<string> ListOfShelfs (string Aisle)
		{
			return ListOfShelfs (Aisle, null);
		}

		public static List<string> ListOfShelfs (string Aisle, params MetaSearch[] Search)
		{
			List<string> result = new List<string>();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("shelf");
			qb.AddWhere ("aisle", "=", Aisle);

			if (Search != null)
			{
			foreach (MetaSearch search in Search)
			{
				qb.AddWhereAND ();

				switch (search.Condition)
				{
					case Enums.DatastoreMetaSearchCondition.Equal:
					{
						qb.AddWhere ("meta", "like binary", "%|"+ search.Key +":"+ search.Value +"|%");
						break;
					}

					case Enums.DatastoreMetaSearchCondition.NotEqual:
					{
						qb.AddWhere ("meta", "not like binary", "%|"+ search.Key +":"+ search.Value +"|%");
						break;
					}

				}
			}
			}

			Query query = Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				while (query.NextRow ())
				{
					result.Add (query.GetString (qb.ColumnPos ("shelf")));
				}
			}

			query.Dispose ();
			query = null;
			qb = null;

			return result;
		}

//		public static List<string> ListOfShelfs (string Aisle, Hashtable Meta)
//		{
//			List<string> result = new List<string>();
//
//			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
//			qb.Table (DatabaseTableName);
//			qb.Columns ("shelf");
//			qb.AddWhere ("aisle", "=", Aisle);
//
//			foreach (string key in Meta.Keys)
//			{
//				string metakey = ((string)key.Split ("|".ToCharArray ())[0]).ToUpper ().Replace (":", "_");
//				string metacondition = key.Split ("|".ToCharArray ())[1];
//				string metadata = Meta[key].ToString ().Replace (":","_").Replace ("|","_");
//
//				qb.AddWhereAND ();
//
//				switch (metacondition)
//				{
//					case "=":
//						qb.AddWhere ("meta", "like", "%"+metakey +":"+ metadata +"%");
//						break;
//
//					case "!=":
//						qb.AddWhere ("meta", "not like", "%"+metakey +":"+ metadata +"%");
//						break;
//				}
//			}
//
//			Query query = Services.Database.Connection.Query (qb.QueryString);
//			if (query.Success)
//			{
//				while (query.NextRow ())
//				{
//					result.Add (query.GetString (qb.ColumnPos ("shelf")));
//				}
//			}
//
//			query.Dispose ();
//			query = null;
//			qb = null;
//
//			return result;
//		}
		#endregion

		#region Internal Static Methods
		internal static void ServiceConfigChanged ()
		{
			DatabaseTableName = SorentoLib.Services.Database.Prefix + "datastore";
		}
		#endregion

		#region Nested Classes
		public class MetaSearch
		{
			#region Private Fields
			private string _key;
			private Enums.DatastoreMetaSearchCondition _condition;
			private string _value;
			#endregion

			#region Public Fields
			public string Key
			{
				get
				{
					return this._key;
				}
			}

			public Enums.DatastoreMetaSearchCondition Condition
			{
				get
				{
					return this._condition;
				}
			}

			public string Value
			{
				get
				{
					return this._value;
				}
			}
			#endregion

			#region Constructor
			public MetaSearch (string Key, Enums.DatastoreMetaSearchCondition Condition, object Value)
			{
				this._key = Key;
				this._condition = Condition;
				this._value = Value.ToString ();
			}
			#endregion
		}

		public class Meta
		{
			#region Private Fields
			private Hashtable _meta;
			#endregion

			#region Constructor
			public Meta ()
			{
				this._meta = new Hashtable ();
			}
			#endregion

			#region Public Methods
			public void Remove (string Key)
			{
				if (this._meta.ContainsKey (Key))
				{
					this._meta.Remove (Key);
				}
			}

			public void Add (string Key, object Value)
			{
				if (this._meta.ContainsKey (Key))
				{
					this._meta[Key] = Value.ToString ();
				}
				else
				{
					this._meta.Add (Key, Value.ToString ());
				}
			}

			override public string ToString ()
			{
				string result = string.Empty;

				foreach (string key in this._meta.Keys)
				{
					result += "|"+ key.ToLower () +":"+ this._meta[key] +"|";
				}

				return result;
			}
			#endregion
		}
		#endregion
	}
}
