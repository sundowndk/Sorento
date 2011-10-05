//
// Global.cs
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
	public class Global
	{
		#region Public Static Fields
		public static string DatastoreAisle = "scms_globals";
		#endregion

		#region Private Fields
		private Guid _id;

		private int _createtimestamp;
		private int _updatetimestamp;
		private Field _field;
		private Content _content;
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
				return this._field.Name;
			}

//			set
//			{
//				string newname = value.ToUpper ();
//
//				if (this._name != newname)
//				{
//					int count = 2;
//					string dummy = newname;
//					while (List ().Exists (delegate (Global o) { return o.Name == newname; }))
//					{
//						newname = dummy +"_"+ count++;
//					}
//
//					this._name = newname.Replace (" ", "_");
//				}
//			}
		}

		public Field Field
		{
			get
			{
				return this._field;
			}

			set
			{
				this._field = value;
			}
		}

		public Content Content
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
		#endregion

		#region Public Constructors
		public Global (sCMS.Enums.FieldType Type, string Name)
		{
			Initalize ();

			string name = Name.ToUpper ();

			int count = 2;
			string dummy = name;
			while (List ().Exists (delegate (Global o) { return o.Name == name; }))
			{
				name = dummy +"_"+ count++;
			}

			name = name.Replace (" ", "_");

			this._field = new Field (Type, name);
			this._content = new Content (Type, string.Empty);
		}

		private Global ()
		{
			Initalize ();
		}

		private void Initalize ()
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
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
				throw new Exception (string.Format (Strings.Exception.GlobalSave, this._id.ToString ()));
			}
		}

		public void ToAjaxRespons (SorentoLib.Ajax.Respons Respons)
		{
			Respons.Data = ToAjaxItem ();
		}

		public Hashtable ToAjaxItem ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimestamp", this._updatetimestamp);
			result.Add ("name", this._field.Name);
			result.Add ("type", this._field.Type);

			List<Hashtable> fields = new List<Hashtable> ();

			Hashtable field = this._field.ToAjaxItem ();
			field.Add ("data", this._content.DataAsString);

			fields.Add (field);

			result.Add ("fields", fields);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Global Load (Guid Id)
		{
			try
			{
				return SNDK.Serializer.DeSerializeObjectFromString<Global> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.GlobalLoadGuid, Id.ToString ()));
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
				throw new Exception (string.Format (Strings.Exception.GlobalDelete, Id.ToString ()));
			}
		}

		public static List<Global> List ()
		{
			List<Global> result = new List<Global> ();

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				try
				{
					result.Add (Global.Load (new Guid (id)));
				}
				catch
				{
					SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.GlobalList, id));
				}
			}

			return result;
		}

		public static Global FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromAjaxItem (Request.Data);
		}

		public static Global FromAjaxItem (Hashtable Item)
		{
			Global result;

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
					result = Global.Load (id);
				}
				catch
				{
					result = new Global ();
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
					result = Global.Load (new Guid ((string)Item["id"]));
				}
				catch
				{
					result = new Global (SNDK.Convert.StringToEnum<Enums.FieldType> ((string)Item["type"]), (string)Item["name"]);
				}
			}

			if (Item.ContainsKey ("fields"))
			{
				result._field = Field.FromAjaxItem ((Hashtable)((List<Hashtable>)Item["fields"])[0]);
				result._content = new Content (result._field.Type, string.Empty);
				result._content.Data = (string)((Hashtable)((List<Hashtable>)Item["fields"])[0])["data"];
			}

//			if (Item.ContainsKey ("field"))
//			{
//				result.Field = Field.FromAjaxItem ((Hashtable)Item["field"]);
//				result.Content.Data = (string)Item["data"];
//			}

//			if (Item.ContainsKey ("content"))
//				result._content.Data = (string)Item["content"];
//						{
//			}

			return result;
		}
		#endregion
	}
}
