//
// Blog.cs
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

using sCMS;
using SorentoLib;

namespace sBlog
{
	[Serializable]
	public class Blog
	{
		#region Public Static Fields
		public static string DatastoreAisle = "sblog_blogs";
		#endregion

		#region Private Fields
		private Guid _id;

		private int _createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
		private int _updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();

		private string _name = string.Empty;
		private Guid _collectionschemaid;
		#endregion

		#region Temp Fields
		private sCMS.CollectionSchema __collectionschema;
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

		public sCMS.CollectionSchema CollectionSchema
		{
			get
			{
				if (this.__collectionschema == null)
				{
					this.__collectionschema = sCMS.CollectionSchema.Load (this._collectionschemaid);
				}

				return this.__collectionschema;
			}
		}

		public List<sCMS.Field> Fields
		{
			get
			{
				return this.CollectionSchema.Fields;
			}
		}

		public List<sBlog.Entry> Entries
		{
			get
			{
				return Entry.List (this);
			}
		}
		#endregion

		#region Constructors
		public Blog ()
		{
			this._id = System.Guid.NewGuid();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();

			// Create new sCMS.CollectionSchema.
			this.__collectionschema = new sCMS.CollectionSchema (this._id.ToString ());
			this._collectionschemaid = this.__collectionschema.Id;
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			try
			{
				// Clear temp fields.
				if (this.__collectionschema != null)
				{
					this.__collectionschema.Save ();
					this.__collectionschema = null;
				}

				// Update timestamp.
				this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();

				// Store instance in Datestore.
				SorentoLib.Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Serializer.SerializeObjectToString (this));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.BlogSave, this._id.ToString ()));
			}
		}

		public void ToAjaxResponse (SorentoLib.Ajax.Respons Respons)
		{
			Respons.Data = this.ToItem ();
		}

		public Hashtable ToItem ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimestamp", this._updatetimestamp);
			result.Add ("name", this._name);
			result.Add ("collectionschemaid", this._collectionschemaid);

			List<Hashtable> fields = new List<Hashtable> ();
			foreach (Field field in this.Fields)
			{
				fields.Add (field.ToAjaxItem ());
			}
			result.Add ("fields", fields);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Blog Load (Guid Id)
		{
			try
			{
				return SNDK.Serializer.DeSerializeObjectFromString<Blog> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.BlogLoad, Id.ToString ()));
			}
		}

		public static void Delete (Guid Id)
		{
			try
			{
				Delete (Blog.Load (Id));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.BlogDelete, Id.ToString ()));
			}
		}

		public static void Delete (Blog Blog)
		{
			try
			{
				// Delete sCMS.CollectionSchema.
				sCMS.CollectionSchema.Delete (Blog.CollectionSchema.Id);

				// Delete entries.
				foreach (Entry entry in Entry.List (Blog))
				{
					Entry.Delete (entry.Id);
				}

				SorentoLib.Services.Datastore.Delete (DatastoreAisle, Blog.Id.ToString ());
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.BlogDelete, Blog.Id.ToString ()));
			}
		}

		public static List<Blog> List ()
		{
			List<Blog> result = new List<Blog> ();

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				try
				{
					result.Add (Blog.Load (new Guid (id)));
				}
				catch
				{
					SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.BlogList, id));
				}
			}

			return result;
		}

		public static Blog FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromItem (Request.Data);
		}

		public static Blog FromItem (Hashtable Item)
		{
			Blog result = null;

			Guid id = Guid.Empty;

			try
			{
				id = new Guid ((string)Item["id"]);
			}
			catch {}

			if (id != Guid.Empty)
			{
				#region RESTORE
				try
				{
					result = Blog.Load (id);
				}
				catch
				{
					result = new Blog ();
					result.__collectionschema = null;

					result._id = id;

					if (Item.ContainsKey ("createtimestamp"))
					{
						result._createtimestamp = int.Parse ((string)Item["createtimestamp"]);
					}

					if (Item.ContainsKey ("updatetimestamp"))
					{
						result._createtimestamp = int.Parse ((string)Item["updatetimestamp"]);
					}

					if (Item.ContainsKey ("collectionschemaid"))
					{
						result._collectionschemaid = new Guid (((string)Item["collectionschemaid"]));
					}
					else
					{
						throw new Exception (string.Format (Strings.Exception.BlogFromItem, "CollectionSchemaId"));
					}
				}
				#endregion
			}
			else
			{
				#region NEW
				result = new Blog ();
				#endregion
			}

			if (Item.ContainsKey ("name"))
			{
				result.Name = (string)Item["name"];
			}

			if (Item.ContainsKey ("fields"))
			{
				result.CollectionSchema.Fields.Clear ();
				foreach (Hashtable item in (List<Hashtable>)Item["fields"])
				{
					result.CollectionSchema.Fields.Add (Field.FromAjaxItem (item));
				}
			}
			return result;
		}
		#endregion
	}
}
