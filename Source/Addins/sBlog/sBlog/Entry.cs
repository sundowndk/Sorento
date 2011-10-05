//
// Entry.cs
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

using SNDK;

namespace sBlog
{
	[Serializable]
	public class Entry
	{
		#region Static Fields
		public static string DatastoreAisle = "sblog_entries";
		#endregion

		#region Private Fields
		private Guid _id;

		private int _createtimestamp;
		private int _updatetimestamp;

		private Guid _blogid;
		private string _title;
		private List<string> _tags;

		private Guid _collectionid;

		private sCMS.Collection _collection
		{
			get
			{
				if (this.__collection == null)
				{
					this.__collection = sCMS.Collection.Load (this._collectionid);
				}

				return this.__collection;
			}
		}
		#endregion

		#region Temp Fields
		private Collection __collection;
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

		public Guid BlogId
		{
			get
			{
				return this._blogid;
			}
		}

		public string Title
		{
			get
			{
				return this._title;
			}

			set
			{
				this._title = value;
			}
		}

		public List<string> Tags
		{
			get
			{
				return this._tags;
			}
		}


//		public string Date
//		{
//			get
//			{
//				List<string> months = new List<string>();
//				months.Add("januar");
//				months.Add("februar");
//				months.Add("marts");
//				months.Add("april");
//				months.Add("maj");
//				months.Add("juni");
//				months.Add("juli");
//				months.Add("august");
//				months.Add("september");
//				months.Add("oktober");
//				months.Add("november");
//				months.Add("december");
//
//				int day = Toolbox.Date.TimestampToDateTime(this._createtimestamp).Day;
//				int month = Toolbox.Date.TimestampToDateTime(this._createtimestamp).Month;
//				string date = day +" "+ months[month-1];
//
//				return date;
//
//			}
//		}

		#endregion

		#region Constructors
		public Entry (Blog Blog)
		{
			this._id = System.Guid.NewGuid ();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._blogid = Blog.Id;
			this._title = string.Empty;
			this._tags = new List<string> ();

			// Create new sCMS.Collection
			this.__collection = new sCMS.Collection (this._id.ToString (), Blog.CollectionSchema);
			this._collectionid = this.__collection.Id;
		}

		private Entry ()
		{
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			try
			{
				// Clear temp fields, we dont need to store them.
				if (this.__collection != null)
				{
					this.__collection.Save ();
					this.__collection = null;
				}


				// Update timestamps
				this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();

				// Set meta tags.
				Hashtable meta = new Hashtable ();
				meta.Add ("blogid", this._blogid);

				// Store instance in Datestore.
				SorentoLib.Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Serializer.SerializeObjectToString (this), meta);
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.EntrySave, this._id.ToString ()));
			}
		}

		public object GetContent (string Name)
		{
			return this._collection.GetContent (Name);
		}

		public object GetContent (Guid Id)
		{
			return this._collection.GetContent (Id);
		}

		public void SetContent (Guid Id, object Data)
		{
			this._collection.SetContent (Id, Data);
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
			result.Add ("blogid", this._blogid);
			result.Add ("collectionid", this._collectionid);
			result.Add ("title", this._title);

			List<Hashtable> contents = new List<Hashtable> ();
			foreach (Field field in this._collection.Schema.Fields)
			{
				Hashtable item = new Hashtable ();
				item = field.ToAjaxItem ();

				Content content = this._collection.Contents.Find (delegate (Content c) { return c.FieldId == field.Id; });
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

			List<Hashtable> tags = new List<Hashtable> ();
			foreach (string tag in this._tags)
			{
				Hashtable item = new Hashtable ();
				item.Add ("name", tag);
				tags.Add (item);
			}
			result.Add ("tags", tags);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Entry Load (Guid Id)
		{
			try
			{
				return SNDK.Serializer.DeSerializeObjectFromString<Entry> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));
			}
			catch (Exception e)
			{
				Console.WriteLine (e);
				throw new Exception (string.Format (Strings.Exception.EntryLoad, Id.ToString ()));
			}
		}

		public static void Delete (Guid Id)
		{
			try
			{
				Delete (Entry.Load (Id));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.EntryDelete, Id.ToString ()));
			}
		}

		public static void Delete (Entry Entry)
		{
			try
			{
				// Delete sCMS.Collection.
				sCMS.Collection.Delete (Entry._collection.Id);

				// Delete Comment.
				foreach (Comment comment in Comment.List (Entry))
				{
					Comment.Delete (comment.Id);
				}

				SorentoLib.Services.Datastore.Delete (DatastoreAisle, Entry.Id.ToString ());
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.EntryDelete, Entry.Id.ToString ()));
			}
		}

		public static List<Entry> List ()
		{
			return List (null);
		}

		public static List<Entry> List (Blog Blog)
		{
			List<Entry> result = new List<Entry> ();

			Hashtable meta = new Hashtable ();
			if (Blog != null)
			{
				meta.Add ("blogid|=", Blog.Id);
			}

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle, meta))
			{
				try
				{
					result.Add (Entry.Load (new Guid (id)));
				}
				catch
				{
					SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.EntryList, id));
				}
			}

			return result;
		}

		public static Entry FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromItem (Request.Data);
		}

		public static Entry FromItem (Hashtable Item)
		{
			Entry result = null;

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
					result = Entry.Load (id);
				}
				catch
				{
					result = new Entry ();

					result._id = id;

					if (Item.ContainsKey ("createtimestamp"))
					{
						result._createtimestamp = int.Parse ((string)Item["createtimestamp"]);
					}

					if (Item.ContainsKey ("updatetimestamp"))
					{
						result._createtimestamp = int.Parse ((string)Item["updatetimestamp"]);
					}

					if (Item.ContainsKey ("blogid"))
					{
						result._blogid = new Guid (((string)Item["blogid"]));
					}
					else
					{
						throw new Exception (string.Format (Strings.Exception.EntryFromItem, "BlogId"));
					}

					if (Item.ContainsKey ("collectionid"))
					{
						result._collectionid = new Guid (((string)Item["collectionid"]));
					}
					else
					{
						throw new Exception (string.Format (Strings.Exception.EntryFromItem, "CollectionId"));
					}
				}
				#endregion
			}
			else
			{
				#region NEW
				try
				{
					Blog blog = Blog.Load (new Guid ((string)Item["blogid"]));
					result = new Entry (blog);
				}
				catch
				{
					throw new Exception (string.Format (Strings.Exception.EntryFromItem, "BlogId"));
				}
				#endregion
			}

			if (Item.ContainsKey ("title"))
			{
				result._title = (string)Item["title"];
			}

			if (Item.ContainsKey ("tags"))
			{
				result._tags.Clear ();
				foreach (Hashtable item in (List<Hashtable>)Item["tags"])
				{
					result._tags.Add ((string)item["name"]);
				}
			}

			if (Item.ContainsKey ("fields"))
			{
				result._collection.Contents.Clear ();
				foreach (Hashtable item in (List<Hashtable>)Item["fields"])
				{
					Field field = result._collection.Schema.GetField (new Guid ((string)item["id"]));
					if (field != null)
					{
						result._collection.Contents.Add (new Content (field, (string)item["data"]));
					}
				}
			}

			return result;
		}
		#endregion
	}
}
