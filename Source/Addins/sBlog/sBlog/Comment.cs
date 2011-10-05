//
// Entry.cs
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

using SorentoLib;

namespace sBlog
{
	[Serializable]
	public class Comment
	{
		#region Static Fields
		public static string DatastoreAisle = "sblog_comments";
		#endregion

		#region Private Fields
		private Guid _id;

		private int _createtimestamp;
		private int _updatetimestamp;

		private Guid _entryid;
		private string _name;
		private string _content;
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

		public Guid EntryId
		{
			get
			{
				return this._entryid;
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

		public string Content
		{
			get
			{
				return this._content;
			}

			set
			{
				this._content = value;
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
//				int second = Toolbox.Date.TimestampToDateTime(this._createtimestamp).Second;
//				int minute = Toolbox.Date.TimestampToDateTime(this._createtimestamp).Minute;
//				int hour = Toolbox.Date.TimestampToDateTime(this._createtimestamp).Hour;
//				int day = Toolbox.Date.TimestampToDateTime(this._createtimestamp).Day;
//				int month = Toolbox.Date.TimestampToDateTime(this._createtimestamp).Month;
//				string date = day +" "+ months[month] +" "+ hour +":"+ minute +":"+ second;
//
//				return date;
//			}
//		}
		#endregion

		#region Constructors
		public Comment (Entry Entry)
		{
			this._id = System.Guid.NewGuid();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._entryid = Entry.Id;
			this._name = string.Empty;
			this._content = string.Empty;
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			try
			{
				this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();

				Hashtable meta = new Hashtable ();
				meta.Add ("entryid", this._entryid);

				SorentoLib.Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Serializer.SerializeObjectToString (this), meta);
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CommentSave, this._id.ToString ()));
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
			result.Add ("entryid", this._entryid);
			result.Add ("name", this._name);
			result.Add ("content", this._content);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Comment Load (Guid Id)
		{
			try
			{
				return SNDK.Serializer.DeSerializeObjectFromString<Comment> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CommentLoad, Id.ToString ()));
			}
		}

		public static void Delete (Guid Id)
		{
			try
			{
				Delete (Comment.Load (Id));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CommentDelete, Id.ToString ()));
			}
		}

		public static void Delete (Comment Comment)
		{
			try
			{
				SorentoLib.Services.Datastore.Delete (DatastoreAisle, Comment.Id.ToString ());
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.CommentDelete, Comment.Id.ToString ()));
			}
		}

		public static List<Comment> List (Entry Entry)
		{
			List<Comment> result = new List<Comment> ();

			Hashtable meta = new Hashtable ();
			meta.Add ("entryid|=", Entry.Id);

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle, meta))
			{
				try
				{
					result.Add (Comment.Load (new Guid (id)));
				}
				catch
				{
					SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.CommentList, id));
				}
			}

			return result;
		}

		public static Comment FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromItem (Request.Data);
		}

		public static Comment FromItem (Hashtable Item)
		{
			Comment result = null;

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
					result = Comment.Load (id);
				}
				catch
				{
					try
					{
						Entry entry = Entry.Load (new Guid ((string)Item["entryid"]));
						result = new Comment (entry);
					}
					catch
					{
						throw new Exception (string.Format (Strings.Exception.CommentFromItem, "EntryId"));
					}

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
				#endregion
			}
			else
			{
				#region NEW
				try
				{
					Entry entry = Entry.Load (new Guid ((string)Item["entryid"]));
					result = new Comment (entry);
				}
				catch
				{
					throw new Exception (string.Format (Strings.Exception.CommentFromItem, "EntryId"));
				}
				#endregion
			}

			if (Item.ContainsKey ("name"))
			{
				result.Name = (string)Item["name"];
			}

			if (Item.ContainsKey ("content"))
			{
				result.Content = (string)Item["content"];
			}

			return result;
		}
		#endregion
	}
}
