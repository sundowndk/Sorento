// 
// CollectionSchema.cs
//  
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2011 Rasmus Pedersen
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

namespace sCMS
{
	[Serializable]
	public class CollectionSchema
	{
		#region Public Static Fields
		public static string DatastoreAisle = "scms_collectionschemas";
		#endregion

		#region Private Fields
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		private string _name;
		private List<Field> _fields;
		#endregion

		#region Internal Fields
		internal List<Guid> _collectionids;
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

		public List<Field> Fields
		{
			get
			{
				this._fields.Sort (delegate (Field field1, Field field2) {return field1.Sort.CompareTo (field2.Sort);});
				return this._fields;
			}
		}

		public List<Collection> Collections
		{
			get
			{
				List<Collection> result = new List<Collection> ();

				foreach (Guid id in this._collectionids)
				{
					try
					{
						result.Add (Collection.Load (id));
					}
					catch
					{
					}
				}

				return result;
			}
		}

		public int DependantCollections
		{
			get
			{
				int result = 0;

				foreach (Collection collection in Collection.List ())
				{
					if (collection._schemaid == this._id)
					{
						result++;
					}
				}

				return result;
			}
		}
		#endregion

		#region Constructor
		public CollectionSchema (string Name)
		{
			Initialize ();
			this._name = Name;
		}

		private CollectionSchema ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._name = string.Empty;
			this._fields = new List<Field> ();
			this._collectionids = new List<Guid> ();
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			try
			{
				SorentoLib.Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Serializer.SerializeObjectToString (this));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CollectionSchemaSave, this._id.ToString ()));
			}
		}

		public void AddField (Field Field)
		{
			int count = 2;
			string dummy = Field.Name;
			while (this._fields.Exists (delegate (Field field) {return field.Name == Field.Name; }))
			{
				Field._name = dummy +"_"+ count++;
			}

			this._fields.Add (Field);
		}

		public void RemoveField (string Name)
		{
			int index = 0;
			foreach (Field field in this._fields)
			{
				if (field.Name == Name.Replace (" ", "_").ToUpper ())
				{
					break;
				}
				index++;
			}

			this._fields.RemoveAt (index);
		}

		public void RemoveField (Guid Id)
		{
			int index = 0;
			foreach (Field field in this._fields)
			{
				if (field.Id == Id)
				{
					break;
				}
				index++;
			}

			this._fields.RemoveAt (index);
		}

		public Field GetField (string Name)
		{
			return this._fields.Find (delegate (Field field) { return field.Name == Name.Replace (" ", "_").ToUpper (); });
		}

		public Field GetField (Guid Id)
		{
			return this._fields.Find (delegate (Field field) { return field.Id == Id; });
		}

		public void ToAjaxResponse (SorentoLib.Ajax.Respons Respons)
		{
			Respons.Data = this.ToAjaxItem ();
		}

		public Hashtable ToAjaxItem ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimestamp", this._updatetimestamp);
			result.Add ("name", this._name);

			List<Hashtable> fields = new List<Hashtable> ();
			foreach (Field field in this._fields)
			{
				fields.Add (field.ToAjaxItem ());
			}
			result.Add ("fields", fields);

			result.Add ("dependantcollections", this.DependantCollections);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static CollectionSchema Load (Guid Id)
		{
			try
			{
				return SNDK.Serializer.DeSerializeObjectFromString<CollectionSchema> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CollectionSchemaLoadGuid, Id.ToString ()));
			}
		}

		public static void Delete (Guid Id)
		{
			try
			{
				SorentoLib.Services.Datastore.Delete (DatastoreAisle, Id.ToString ());

				foreach (Collection collection in Collection.List ())
				{
					try
					{
						if (collection._schemaid == Id)
						{
							Collection.Delete (collection.Id);
						}
					}
					catch
					{}
				}
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CollectionSchemaDelete, Id.ToString ()));
			}
		}

		public static List<CollectionSchema> List ()
		{
			List<CollectionSchema> result = new List<CollectionSchema> ();

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				try
				{
					result.Add (CollectionSchema.Load (new Guid (id)));
				}
				catch
				{
					SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.CollectionSchemaList, id));
				}
			}

			return result;
		}

		public static CollectionSchema FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromAjaxItem (Request.Data);
		}

		public static CollectionSchema FromAjaxItem (Hashtable Item)
		{
			CollectionSchema result = null;

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
					result = CollectionSchema.Load (id);
				}
				catch
				{
					result = new CollectionSchema ();
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
					result = new CollectionSchema ((string)Item["name"]);				}
				catch
				{
					throw new Exception (string.Format (Strings.Exception.PageFromAjaxItem, "Name"));
				}
			}

			if (Item.ContainsKey ("name"))
			{
				result.Name = (string)Item["name"];
			}

			if (Item.ContainsKey ("fields"))
			{
				result._fields.Clear ();
				foreach (Hashtable item in (List<Hashtable>)Item["fields"])
				{
					result.AddField (Field.FromAjaxItem (item));
				}
			}

			result._collectionids.Clear ();
			foreach (Collection collection in Collection.List ())
			{
				if (collection._schemaid == result._id)
				{
					result._collectionids.Add (collection.Id);
				}
			}

			return result;
		}
		#endregion
	}
}

