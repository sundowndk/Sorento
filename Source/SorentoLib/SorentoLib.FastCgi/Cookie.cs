//
// Cookie.cs
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
using System.Web;

namespace SorentoLib.FastCgi
{	
	public class Cookie
	{				
		#region Private Fields
		private string _name;
		private string _value;
		private string _domain;
		private string _expires;
		private string _path;
		private string _secure;
		#endregion
				
		#region Public Fields
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

		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public string Domain
		{
			get
			{
				return this._domain;
			}
			set
			{
				this._domain = value;
			}
		}
        
		public string Expires
		{
			get
			{
				return this._expires;
			}
			set
			{
				this._expires = value;
			}
		}
			
		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				this._path = value;
			}
		}

		public string Secure
		{
			get
			{
				return this._secure;
			}
			set
			{
				this._secure = value;
			}
		}
		#endregion
		
		#region Constructor
		public Cookie ()
		{
			this._domain = string.Empty;
			this._expires = string.Empty;
			this._name = string.Empty;
			this._path = string.Empty;
			this._secure = string.Empty;
			this._value = string.Empty;
		}
		#endregion

 		#region Public Methods
		public string Bake ()
		{
			string result = "set-cookie:"+ HttpUtility.UrlEncode (this._name) +"="+ HttpUtility.UrlEncode (this._value);
			
			if (this._domain != string.Empty || this._path != string.Empty || this._expires != string.Empty || this._secure != string.Empty)
			{
				result += "; ";				
			}
			
			if (this._domain != string.Empty)
			{
				result += "domain="+ this._domain +"; ";
			}
			if (this._path != string.Empty)
			{
				result += "path="+ this._path +"; ";
			}
			if (this._expires != string.Empty)
			{
				result += "expires="+ this._expires +"; ";
			}
			if (this._secure != string.Empty)
			{
				result  += "secure="+this._secure +"; ";
			}	

			return result.TrimEnd (';');
		}
		#endregion		
	}
}
