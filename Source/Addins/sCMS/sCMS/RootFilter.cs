// 
// RootFilter.cs
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
	public class RootFilter
	{
		#region Private Fields
		public Enums.RootFilterType _type;
		public string _data;
		#endregion

		#region Public Fields
		public Enums.RootFilterType Type
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

		public string Data
		{
			get
			{
				return this._data;
			}

			set
			{
				this._data = value;
			}
		}
		#endregion

		#region Constructor
		public RootFilter (Enums.RootFilterType Type)
		{
			this._type = Type;
			this._data = string.Empty;
		}
		#endregion

		#region Public Methods
		public void ToAjaxRespons (SorentoLib.Ajax.Respons Respons)
		{
			Respons.Data = this.ToAjaxItem ();
		}

		public Hashtable ToAjaxItem ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("type", this._type);
			result.Add ("data", this._data);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static RootFilter FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromAjaxItem (Request.Data);
		}

		public static RootFilter FromAjaxItem (Hashtable Item)
		{
			RootFilter result;

			try
			{
				result = new RootFilter ( SNDK.Convert.StringToEnum<Enums.RootFilterType> ((string)Item["type"]));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.RootFilterFromAjaxItem, "Type"));
			}

			if (Item.ContainsKey ("data"))
			{
				result._data = (string)Item["data"];
			}

			return result;
		}
		#endregion
	}
}

