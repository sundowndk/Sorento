// 
// Root.cs
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
	public class Root
	{
		#region Public Static Fields
		public static string DatastoreAisle = "scms_roots";
		#endregion

		#region Private Fields
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		private string _name;
		private List<RootFilter> _filters;
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

		public List<RootFilter> Filters
		{
			get
			{
				return this._filters;
			}
		}

		public int DependantPages
		{
			get
			{
				int result = 0;

				foreach (Page page in Page.List ())
				{
//					try
//					{
						if (page.ParentId == this._id)
						{
							result++;
						}
//					}
//					catch
//					{}
				}

				return result;
			}
		}
		#endregion

		#region Constructor
		public Root (string Name)
		{
			Initialize ();
			this._name = Name;
		}

		private Root ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._name = string.Empty;
			this._filters = new List<RootFilter> ();
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
				throw new Exception (string.Format (Strings.Exception.RootSave, this._id.ToString ()));
			}
		}

		public void ToAjaxRespons (SorentoLib.Ajax.Respons Respons)
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

			List<Hashtable> filters = new List<Hashtable> ();
			foreach (RootFilter filter in this._filters)
			{
				filters.Add (filter.ToAjaxItem ());
			}

			result.Add ("filters", filters);
			result.Add ("dependantpages", this.DependantPages);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Root Load (Guid Id)
		{
			try
			{
				Root result = SNDK.Serializer.DeSerializeObjectFromString<Root> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));

				return result;
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.RootLoad, Id.ToString ()));
			}

		}

		public static void Delete (Guid Id)
		{
			try
			{
				SorentoLib.Services.Datastore.Delete (DatastoreAisle, Id.ToString ());

				foreach (Page page in Page.List ())
				{
//					try
//					{
						if (page.ParentId == Id)
						{
							Page.Delete (page.Id);
						}
//					}
//					catch
//					{}
				}
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.RootDelete, Id.ToString ()));
			}
		}

		public static List<Root> List ()
		{
			List<Root> result = new List<Root> ();

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				try
				{
					result.Add (Root.Load (new Guid (id)));
				}
				catch
				{
					SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.RootList, id));
				}
			}

			return result;
		}

		public static Root FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromAjaxItem (Request.Data);
		}

		public static Root FromAjaxItem (Hashtable Item)
		{
			Root result;

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
					result = Root.Load (id);
				}
				catch
				{
					result = new Root ();
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
					result = new Root ((string)Item["name"]);
				}
				catch
				{
					throw new Exception (string.Format (Strings.Exception.RootFromAjaxItem, "Name"));
				}
			}

			if (Item.ContainsKey ("name"))
			{
				result._name = (string)Item["name"];
			}

			if (Item.ContainsKey ("filters"))
			{
				result._filters.Clear ();

				foreach (Hashtable item in (List<Hashtable>)Item["filters"])
				{
					result._filters.Add (RootFilter.FromAjaxItem (item));
				}
			}

			return result;
		}
		#endregion
	}
}

