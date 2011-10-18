// 
// Field.cs
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

namespace sCMS
{
	[Serializable]
	public class Field
	{
		#region Private Fields
		private Guid _id;
		private sCMS.Enums.FieldType _type;
		private int _sort;
		private bool _hidden;

		// TODO: remove in final.
//		private string _defaultvalue;

		private object _defaultvalue;
		#endregion

		#region Internal Fields
		internal string _name;
		internal string _alias;
		internal bool _inherit;
		internal Hashtable _options; // TODO: revert to private on final.
		#endregion

		#region Public Fields
		public Guid Id
		{
			get
			{
				return this._id;
			}
		}

		public sCMS.Enums.FieldType Type
		{
			get
			{
				return this._type;
			}

			set
			{
				this._type = value;
			}
		}

		public int Sort
		{
			get
			{
				return this._sort;
			}

			set
			{
				this._sort = value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}

		}

		public string Alias
		{
			get
			{
				return this._alias;
			}
		}

		public bool Hidden
		{
			get
			{
				return this._hidden;
			}

			set
			{
				this._hidden = value;
			}
		}

		public bool Inherit
		{
			get
			{
				return this._inherit;
			}
		}

		public Hashtable Options
		{
			get
			{
				return this._options;
			}
		}

		public object DefaultValue
		{
			get
			{
				if ((string)this._defaultvalue == string.Empty)
				{
					switch (this._type)
					{
						case sCMS.Enums.FieldType.String:
							return string.Empty;

						case sCMS.Enums.FieldType.Text:
							return string.Empty;

						case sCMS.Enums.FieldType.Image:
							return null;

						case sCMS.Enums.FieldType.ListImage:
							return new List<Media> ();

						case sCMS.Enums.FieldType.ListPage:
							return new List<Page> ();
					}
				}
				else
				{
					return this._defaultvalue;
				}

				return this._defaultvalue;
			}

			set
			{
				this._defaultvalue = value;
			}

		}
		#endregion

		#region Constructors
		public Field (sCMS.Enums.FieldType Type, string Name)
		{
			this._id = Guid.NewGuid ();
			this._type = Type;
			this._sort = 0;
			this._name = Name.Replace (" ", "_").ToUpper ();
			this._alias = string.Empty;
			this._hidden = false;
			this._inherit = false;
			this._defaultvalue = string.Empty;
			this._options = new Hashtable ();
		}

		private Field ()
		{
			this._id = Guid.NewGuid ();
			this._sort = 0;
			this._name = string.Empty;
			this._alias = string.Empty;
			this._hidden = false;
			this._inherit = false;
			this._defaultvalue = string.Empty;
			this._options = new Hashtable ();
		}
		#endregion

		#region Public Methods
		public void ToAjaxRespos (SorentoLib.Ajax.Respons Respons)
		{
//			Respons.Data = ToAjaxItem ();
		}

		public Hashtable ToAjaxItem ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("type", this._type.ToString ().ToLower ());
			result.Add ("sort", this._sort);
			result.Add ("name", this._name);
			result.Add ("alias", this._alias);
			result.Add ("hidden", this._hidden);
			result.Add ("inherit", this._inherit);
			result.Add ("defaultvalue", this._defaultvalue);

			// TODO: Remove in final
			if (this._options != null)
			{
				result.Add ("options", this._options);
			}

//			List<Hashtable> options = new List<Hashtable> ();
//			foreach (string option in this._options)
//			foreach (string key in this._options.Keys)
//			{
//				Hashtable item = new Hashtable ();
//				item.Add ("name", key);
//				item.Add ("value")
//				mimetypes.Add (item);
//			}

			return result;
		}

		public static Field FromAjaxRequest (SorentoLib.Ajax.Respons Request)
		{
			return FromAjaxItem (Request.Data);
		}

		public static Field FromAjaxItem (Hashtable Item)
		{
			Field result = new Field ();

			if ((string)Item["id"] != string.Empty)
			{
				result._id = new Guid ((string)Item["id"]);
			}

			if (Item.ContainsKey ("type"))
			{
				result._type = SNDK.Convert.StringToEnum<Enums.FieldType> ((string)Item["type"]);
			}

			if (Item.Contains ("sort"))
			{
				result._sort = int.Parse ((string)Item["sort"]);
			}

			if (Item.Contains ("name"))
			{
				result._name = ((string)Item["name"]).Replace (" ", "_").ToUpper ();
			}

			if (Item.Contains ("alias"))
			{
				result._alias = (string)Item["alias"];
			}

			if (Item.Contains ("hidden"))
			{
				result._hidden =  SNDK.Convert.StringToBool ((string)Item["hidden"]);
			}

			if (Item.Contains ("inherit"))
			{
				result._inherit = SNDK.Convert.StringToBool ((string)Item["inherit"]);
			}

			if (Item.Contains ("defaultvalue"))
			{
				result._defaultvalue = (string)Item["defaultvalue"];
			}

			if (Item.Contains ("options"))
			{
				result._options = (Hashtable)Item["options"];
			}

			return result;
		}
		#endregion
	}
}

