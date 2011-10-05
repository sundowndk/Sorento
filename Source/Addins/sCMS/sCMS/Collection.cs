// 
// Collection.cs
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
using System.Collections;
using System.Collections.Generic;

namespace sCMS
{
	[Serializable]
	public class Collection
	{
		#region Public Static Fields
		public static string DatastoreAisle = "scms_collections";
		#endregion

		#region Private Fields
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		internal Guid _schemaid;
		private string _name;
		private List<Content> _contents;

		private CollectionSchema _tempschema;
		#endregion

		#region Public Fields
		public Guid Id
		{
			get
			{
				return this._id;
			}

			set
			{
				this._id = value;
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

		public CollectionSchema Schema
		{
			get
			{
				if (this._tempschema == null)
				{
					this._tempschema = CollectionSchema.Load (this._schemaid);
				}

				return this._tempschema;
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

		public List<Content> Contents
		{
			get
			{
				return this._contents;
			}
		}
		#endregion

		#region Constructor
		public Collection (string Name, CollectionSchema Schema)
		{
			Initalize ();

			this._schemaid = Schema.Id;
			this._name = Name;
		}

		private Collection ()
		{
			Initalize ();
		}

		private void Initalize ()
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._schemaid = Guid.Empty;
			this._contents = new List<Content> ();
			this._name = string.Empty;
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			this._tempschema = null;

			try
			{
				SorentoLib.Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Serializer.SerializeObjectToString (this));

				CollectionSchema schema = CollectionSchema.Load (this._schemaid);
				if (!schema._collectionids.Contains (this._id))
				{
					schema._collectionids.Add (this._id);
					schema.Save ();
				}
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CollectionSave, this._id.ToString ()));
			}
		}

		public object GetContent (string Name)
		{
			Field field = this.Schema.Fields.Find (delegate (Field f) { return f.Name.ToUpper () == Name.ToUpper (); });

			if (field != null)
			{
				return GetContent (field.Id);
			}

			return null;
		}

		public object GetContent (Guid Id)
		{
			return this._contents.Find (delegate (Content content) { return content.FieldId == Id; }).Data;
		}

		public void SetContent (Guid Id, object Data)
		{
			try
			{
				this._contents.Find (delegate (Content content) { return content.FieldId == Id; }).Data = Data;
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CollectionSetContent, Id.ToString ()));
			}

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
			result.Add ("schemaid", this._schemaid);
			result.Add ("name", this._name);

			List<Hashtable> contents = new List<Hashtable> ();
			foreach (Field field in this.Schema.Fields)
			{
				Hashtable item = new Hashtable ();
				item = field.ToAjaxItem ();

				Content content = this._contents.Find (delegate (Content c) { return c.FieldId == field.Id; });
				if (content != null)
				{
					item.Add ("data", content.DataAsString);
				}
				else
				{
					item.Add ("data", string.Empty);
				}
				contents.Add (item);
			}

			result.Add ("fields", contents);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Collection Load (Guid Id)
		{
			try
			{
				return SNDK.Serializer.DeSerializeObjectFromString<Collection> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CollectionLoadGuid, Id.ToString ()));
			}
		}

		public static void Delete (Guid Id)
		{
			try
			{
				Guid schemaid = Collection.Load (Id).Schema.Id;
				SorentoLib.Services.Datastore.Delete (DatastoreAisle, Id.ToString ());

				CollectionSchema schema = CollectionSchema.Load (schemaid);
				if (!schema._collectionids.Contains (Id))
				{
					schema._collectionids.Remove (Id);
					schema.Save ();
				}
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CollectionDelete, Id.ToString ()));
			}
		}

//		public static List<Collection> List (CollectionSchema Schema)
//		{
//
//		}

		public static List<Collection> List ()
		{
			List<Collection> result = new List<Collection> ();

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				try
				{
					result.Add (Collection.Load (new Guid (id)));
				}
				catch
				{
					SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.CollectionList, id));
				}
			}

			return result;
		}

		public static Collection FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromAjaxItem (Request.Data);
		}

		public static Collection FromAjaxItem (Hashtable Item)
		{
			Collection result = null;

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
					result = Collection.Load (id);
				}
				catch
				{
					result = new Collection ();
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
					CollectionSchema schema = CollectionSchema.Load (new Guid ((string)Item["schemaid"]));
					result = new Collection ((string)Item["name"], schema);
				}
				catch
				{
					throw new Exception (string.Format (Strings.Exception.PageFromAjaxItem, "SchemaId"));
				}
			}

			if (Item.ContainsKey ("schemaid"))
			{
				if ((string)Item["schemaid"] != string.Empty)
				{
					result._schemaid = new Guid ((string)Item["schemaid"]);
				}
			}

			if (Item.ContainsKey ("name"))
			{
				result.Name = (string)Item["name"];
			}

			if (Item.ContainsKey ("fields"))
			{
				result._contents.Clear ();
				foreach (Hashtable item in (List<Hashtable>)Item["fields"])
				{
					Field field = result.Schema.GetField (new Guid ((string)item["id"]));
					if (field != null)
					{
						result._contents.Add (new Content (field, (string)item["data"]));
					}
				}
			}

			return result;
		}
		#endregion
	}
}

