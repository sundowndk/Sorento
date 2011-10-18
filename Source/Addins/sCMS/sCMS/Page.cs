// 
// Page.cs
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
	public class Page
	{
		#region Public Static Fields
		public static string DatastoreAisle = "scms_pages";
		#endregion

		#region Private Fields
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		private Guid _templateid;
		internal Guid _parentid;
		private string _name;
		private List<string> _aliases;
		private List<Content> _contents;

		private Template _temptemplate;
		private Page _tempparent;
		#endregion

		#region Internal Fields
		internal Guid ParentId
		{
			get
			{
				return this._parentid;
			}

			set
			{
				this._parentid = value;
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

		public Template Template
		{
			get
			{
				if (this._temptemplate != null)
				{
					return this._temptemplate;
				}
				else
				{
					return Template.Load (this._templateid);
				}
			}
		}

		public Page Parent
		{
			get
			{
				if (this._parentid != Guid.Empty)
				{
					if (this._tempparent != null)
					{
						return this._tempparent;
					}
					else
					{
						return Page.Load (this._parentid);
					}
				}
				else
				{
					return null;
				}
			}

			set
			{
				this._parentid = value.Id;
				this._tempparent = value;
			}
		}

		public string Path
		{
			get
			{
				string result = string.Empty;

				try
				{
					if (this._parentid != Guid.Empty)
					{
						result += this.Parent.Path;
					}
				}
				catch
				{
				}

				result += "/";

				result += this._name;

				return result;
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
				string newname = Helpers.MakeStringURLSafe (value);

				if (this._name != newname)
				{
					int count = 2;
					string dummy = newname;
					while (List ().Exists (delegate (Page o) { return o.Name == newname; }))
					{
						newname = dummy +"_"+ count++;
					}

					this._name = newname;
				}
			}
		}

		public List<string> Aliases
		{
			get
			{
				return this._aliases;
			}
		}

		public List<Content> Contents
		{
			get
			{
				return this._contents;
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

		#region Constructors
		public Page (Template Template)
		{
			Initialize ();

			this._templateid = Template.Id;
			this._temptemplate = Template;
		}

		private Page ()
		{
			Initialize ();
		}

		private void Initialize ()
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._templateid = Guid.Empty;
			this._parentid = Guid.Empty;
			this.Name = "Untitled";
			this._aliases = new List<string> ();
			this._contents = new List<Content> ();
		}

		#endregion

		#region Public Methods
		public void Save ()
		{
			this._temptemplate = null;

			try
			{
				SorentoLib.Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Serializer.SerializeObjectToString (this));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.PageSave, this._id.ToString ()));
			}
		}

		public object GetContent (string Name)
		{
			Field field = this.Template.Fields.Find (delegate (Field f) { return f.Name.ToUpper () == Name.ToUpper (); });

			if (field != null)
			{
				return GetContent (field.Id);
			}

			return null;
		}

		public object GetContent (Guid Id)
		{
			return this._contents.Find (delegate (Content c) { return c.FieldId == Id; }).Data;
		}

		public void ToAjaxResponse (SorentoLib.Ajax.Respons Respons)
		{
//			Respons.Data = this.ToAjaxItem ();
		}

		public Hashtable ToAjaxItem ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimestamp", this._updatetimestamp);
			result.Add ("templateid", this._templateid);

			if (this._parentid != Guid.Empty)
			{
				result.Add ("parentid", this._parentid);
			}
			else
			{
				result.Add ("parentid", string.Empty);
			}

			result.Add ("path", this.Path);
			result.Add ("name", this._name);

			List<Hashtable> aliases = new List<Hashtable> ();
			foreach (string alias in this._aliases)
			{
				Hashtable item = new Hashtable ();
				item.Add ("name", alias);

				aliases.Add (item);
			}
			result.Add ("aliases", aliases);

			List<Hashtable> contents = new List<Hashtable> ();
			foreach (Field field in this.Template.Fields)
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
			result.Add ("dependantpages", this.DependantPages);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Page Load (string Name)
		{
			Page result = null;

//			Console.WriteLine (Name);

			foreach (Page page in List ())
			{
//				Console.WriteLine ("Page: "+ page.Path);

				if (Name == page.Path)
				{
//					Console.WriteLine ("PAGE FOUND");
					result = page;
					break;
				}
				else
				{
//					foreach (string a in page.Aliases)
//					{
//						if (System.IO.Path.GetDirectoryName (page.Path) == "/")
//						{
//							Console.WriteLine ("Alias: "+ System.IO.Path.GetDirectoryName (page.Path) + a);
//						}
//						else
//						{
//							Console.WriteLine ("Alias: "+ System.IO.Path.GetDirectoryName (page.Path) +"/"+ a);
//						}
//
//					}

					if (page.Aliases.Exists (delegate (string o) {
						string directoryname = System.IO.Path.GetDirectoryName (page.Path);
						Console.WriteLine (directoryname);
						if (directoryname == "/")
						{return directoryname + o == Name;}
						else
						{return directoryname +"/"+ o == Name;}
					}))
//					if (page.Aliases.Exists (delegate (string o) { return System.IO.Path.GetDirectoryName (page.Path) +"/"+ o == Name;}))
					{
//						Console.WriteLine ("ALIAS FOUND");

						result = page;
						break;
					}
				}
			}


//			foreach (Page page in List ())
//			{
//				if (page.Name == Name)
//				{
//					result = page;
//					break;
//				}
//				else
//				{
//					if (page.Aliases.Exists (delegate (string o) { return o == Name; }))
//					{
//						result = page;
//						break;
//					}
//				}
//			}

			if (result == null)
			{
				throw new Exception (string.Format (Strings.Exception.PageLoadName, Name));
			}

			return result;
		}

		public static Page Load (Guid Id)
		{
			try
			{
				return SNDK.Serializer.DeSerializeObjectFromString<Page> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.PageLoadGuid, Id.ToString ()));
			}
		}

		public static void Delete (Guid Id)
		{
			try
			{
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

				SorentoLib.Services.Datastore.Delete (DatastoreAisle, Id.ToString ());
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.PageDelete, Id.ToString ()));
			}
		}

		public static List<Page> List ()
		{
			List<Page> result = new List<Page> ();

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				try
				{
					result.Add (Page.Load (new Guid (id)));
				}
				catch
				{
					SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.PageList, id));
				}
			}

			return result;
		}

		public static Page FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromAjaxItem (Request.Data);
		}

		public static Page FromAjaxItem (Hashtable Item)
		{
			Page result = null;

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
					result = Page.Load (id);
				}
				catch
				{
					result = new Page ();
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
					Template template = Template.Load (new Guid ((string)Item["templateid"]));
					result = new Page (template);
				}
				catch
				{
					throw new Exception (string.Format (Strings.Exception.PageFromAjaxItem, "TemplateId"));
				}
			}

			if (Item.ContainsKey ("templateid"))
			{
				if ((string)Item["templateid"] != string.Empty)
				{
					result._templateid = new Guid ((string)Item["templateid"]);
				}
			}

			if (Item.ContainsKey ("parentid"))
			{
				if ((string)Item["parentid"] != string.Empty)
				{
					result._parentid = new Guid ((string)Item["parentid"]);
				}
			}

			if (Item.ContainsKey ("name"))
			{
				result.Name = (string)Item["name"];
			}

			if (Item.ContainsKey ("aliases"))
			{
				result._aliases.Clear ();
				foreach (Hashtable item in (List<Hashtable>)Item["aliases"])
				{
					result._aliases.Add ((string)item["name"]);
				}
			}

			if (Item.ContainsKey ("fields"))
			{
				result._contents.Clear ();
				foreach (Hashtable item in (List<Hashtable>)Item["fields"])
				{
					Field field = result.Template.GetField (new Guid ((string)item["id"]));
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

