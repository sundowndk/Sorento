// 
// Template.cs
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
	public class Template
	{
		#region Public Static Fields
		public static string DatastoreAisle = "scms_templates";
		#endregion

		#region Private Fields
		private Guid _id;

		private int _createtimestamp;
		private int _updatetimestamp;

		private Guid _parentid;

		private List<string> _stylesheetfilenames;
		private List<string> _stylesheetids;

		private string _title;
		private string _content;

		private List<Field> _fields;

		private Template _tempparent;
		private List<Stylesheet> _tempstylesheets;
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

		public Template Parent
		{
			get
			{
				if (this._tempparent != null)
				{
					return this._tempparent;
				}
				else if (this._parentid != Guid.Empty)
				{
					return Template.Load (this._parentid);
				}

				return null;
			}

			set
			{
				if (value != null)
				{
					this._parentid = value.Id;
				}
				else
				{
					this._parentid = Guid.Empty;
				}

				this._tempparent = value;
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

		public List<Stylesheet> Stylesheets
		{
			get
			{
				if (this._tempstylesheets != null)
				{
					return this._tempstylesheets;
				}
				else
				{
					this._tempstylesheets = new List<Stylesheet> ();

					foreach (string filename in this._stylesheetids)
					{
						this._tempstylesheets.Add (Stylesheet.Load (filename));
					}

					return this._tempstylesheets;
				}
			}
		}

		public List<Field> Fields
		{
			get
			{
				return this._compilefields ();
			}
		}

		public List<Field> LocalFields
		{
			get
			{
				this._fields.Sort (delegate(Field o1, Field o2) { return o1.Sort.CompareTo(o2.Sort); });
				return this._fields;
			}
		}

		// TODO: fix this
		public int DependantPages
		{
			get
			{
				int result = 0;

//				foreach (Page page in sCMS.Page.List ())
//				{
//					if (page.Template.Id == this._id)
//					{
//						result++;
//					}
//				}

				return result;
			}
		}

		// TODO: fix this.
		public int DependantTemplates
		{
			get
			{
				int result = 0;

				foreach (Template template in sCMS.Template.List ())
				{
					if (template.Parent != null)
					{
						if (template.Parent.Id == this._id)
						{
							result++;
						}
					}
				}

				return result;
			}
		}
		#endregion

		#region Public Constructors
		public Template ()
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._parentid = Guid.Empty;
			this._title = string.Empty;
			this._stylesheetids = new List<string> ();
			this._content = string.Empty;
			this._fields = new List<Field> ();

			this._tempparent = null;
			this._tempstylesheets = null;
		}
		#endregion

		#region Private Methods
		private List<Field> _compilefields ()
		{
			List<Field> result = new List<Field> ();
			this._fields.Sort (delegate(Field o1, Field o2) { return o1.Sort.CompareTo(o2.Sort); });
			result.AddRange (this._fields);
			if (this.Parent != null)
			{
				foreach (Field field in this.Parent._compilefields ())
				{
					field._inherit = true;
					result.Add (field);
				}
			}

			// TODO: is this needed?
//			result.Sort (delegate(Field o1, Field o2) { return o1.Sort.CompareTo(o2.Sort); });
//			result.Sort (delegate(Field o1, Field o2) { return o1.Name.CompareTo(o2.Name); });


			return result;
		}

		private string _compiletemplates ()
		{
			string result = string.Empty;

			if (this._parentid != Guid.Empty)
			{
				result = Template.Load (this._parentid)._compiletemplates ().Replace (SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.scms_templateplaceholdertag), this._content);
			}
			else
			{
				result = this._content;
			}

			return result;
		}

		private List<string> _compilestylesheets ()
		{
			List<string> result = new List<string> ();

			if (this._parentid != Guid.Empty)
			{
				result.AddRange (Template.Load (this._parentid)._compilestylesheets ());
			}

			result.AddRange (this._stylesheetids);

			return result;
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			// TODO: remove in final;
			foreach (Field field in this._fields)
			{
				if (field.Options == null)
				{
					field._options = new Hashtable ();
				}
			}

			this._stylesheetfilenames = null; // TODO: remove in final;

			this._stylesheetids.Clear ();
			foreach (Stylesheet stylesheet in this.Stylesheets)
			{
				this._stylesheetids.Add (stylesheet.Filename);
			}

			this._tempparent = null;
			this._tempstylesheets = null;

			try
			{
				SorentoLib.Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Serializer.SerializeObjectToString (this));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.TemplateSave, this._id.ToString ()));
			}
		}

		public void AddField (Field Field)
		{
			int count = 2;
			string dummy = Field.Name;
			while (this._compilefields ().Exists (delegate (Field o) { return o.Name == Field.Name; }))
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
			return this._compilefields ().Find (delegate (Field o) { return o.Name == Name.Replace (" ", "_").ToUpper (); });
		}

		public Field GetField (Guid Id)
		{
			return this._compilefields ().Find (delegate (Field o) { return o.Id == Id; });
		}

		public List<string> Build ()
		{
			List<string> result = new List<string> ();

			foreach (string line in this._compiletemplates ().Split ("\n".ToCharArray ()))
			{
				if (line.Contains (SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.scms_stylesheetplaceholdertag)))
				{
					foreach (string filename in this._compilestylesheets ())
					{
						result.Add ("<link rel=\"stylesheet\" href=\""+ SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.scms_stylesheeturl) +"/"+ filename +"\" type=\"text/css\"/>");
					}

					continue;
				}

				result.Add (line);
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
			result.Add ("title", this._title);
			result.Add ("content", this._content);

			if (this._parentid != Guid.Empty)
			{
				result.Add ("parentid", this._parentid);
			}
			else
			{
				result.Add ("parentid", string.Empty);
			}

			List<Hashtable> stylesheets = new List<Hashtable> ();
			foreach (Stylesheet stylesheet in this.Stylesheets)
			{
				stylesheets.Add (stylesheet.ToAjaxItem ());
			}
			result.Add ("stylesheets", stylesheets);

			List<Hashtable> fields = new List<Hashtable> ();
			foreach (Field field in this._fields)
			{
				fields.Add (field.ToAjaxItem ());
			}
			result.Add ("fields", fields);

			// TODO: fix this
//			result.Add ("dependantpages", this.DependantPages);
//			result.Add ("dependanttemplates", this.DependantTemplates);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Template Load (Guid Id)
		{
			try
			{
				Template result = SNDK.Serializer.DeSerializeObjectFromString<Template> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));

				if (result._stylesheetids == null) // TODO: Can be removed in FINAL!
				{
					result._stylesheetids = new List<string> ();
				}

				if (result._stylesheetfilenames != null) // TODO: Can be removed in FINAL!
				{
					result._stylesheetids = result._stylesheetfilenames;
				}

				// TODO: remove in final;
				foreach (Field field in result._fields)
				{
					if (field.Options == null)
					{
						field._options = new Hashtable ();
					}
				}

				return result;
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.TemplateLoad, Id.ToString ()));
			}
		}

		public static void Delete (Guid Id)
		{
			try
			{
				SorentoLib.Services.Datastore.Delete (DatastoreAisle, Id.ToString ());
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.TemplateDelete, Id.ToString ()));
			}
		}

		public static List<Template> List ()
		{
			List<Template> result = new List<Template> ();

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				try
				{
					result.Add (Template.Load (new Guid (id)));
				}
				catch
				{
					SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.TemplateList, id));
				}
			}

			return result;
		}

		public static Template FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromAjaxItem (Request.Data);
		}

		public static Template FromAjaxItem (Hashtable Item)
		{
			Template result;
			bool restore = false;

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
					result = Template.Load (id);
				}
				catch
				{
					restore = true;
					result = new Template ();
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
				result = new Template ();
			}

			if (Item.ContainsKey ("parentid"))
			{
				result._tempparent = null;
				if ((string)Item["parentid"] != string.Empty)
				{
					result._parentid = new Guid ((string)Item["parentid"]);
				}
				else
				{
					result._parentid = Guid.Empty;
				}
			}

			if (Item.ContainsKey ("title"))
			{
				result._title = (string)Item["title"];
			}

			if (Item.ContainsKey ("content"))
			{
				result._content = (string)Item["content"];
			}

			if (Item.ContainsKey ("stylesheets"))
			{
				result._stylesheetids.Clear ();
				result._tempstylesheets = null;

				foreach (Hashtable item in (List<Hashtable>)Item["stylesheets"])
				{
					result.Stylesheets.Add (Stylesheet.FromAjaxItem (item));
				}
			}

			if (Item.ContainsKey ("fields"))
			{
				result._fields.Clear ();
				foreach (Hashtable item in (List<Hashtable>)Item["fields"])
				{
					if (!restore)
					{
						result.AddField (Field.FromAjaxItem (item));
					}
					else
					{
						result._fields.Add (Field.FromAjaxItem (item));
					}
				}
			}

			return result;
		}
		#endregion
	}
}
