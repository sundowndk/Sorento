//
// CookieJar.cs
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
using System.Collections.Generic;

namespace SorentoLib.FastCgi
{	
	public class CookieJar
	{
		#region Private Fields
		private List<SorentoLib.FastCgi.Cookie> _cookies;
		#endregion

		#region Constructor
		public CookieJar()
		{
			this._cookies = new List<SorentoLib.FastCgi.Cookie> ();
		}
		#endregion

		#region Public Methods
		public void Add (SorentoLib.FastCgi.Cookie Cookie)
		{
			SorentoLib.FastCgi.Cookie result = this._cookies.Find (delegate (SorentoLib.FastCgi.Cookie cookie) { return cookie.Name == Cookie.Name; });

			if (result != null)
			{
				this._cookies.Remove (result);
			}

			this._cookies.Add (Cookie);
		}

		public bool Remove (string Name)
		{
			for (int i = 0; i < this._cookies.Count; i++)
			{
				if (this._cookies[i].Name == Name)
				{
					this._cookies.RemoveAt (i);
					return true;
				}
			}

			return false;
		}

		public SorentoLib.FastCgi.Cookie Get (string Name)
		{
			SorentoLib.FastCgi.Cookie result = this._cookies.Find (delegate(SorentoLib.FastCgi.Cookie cookie) { return cookie.Name == Name; });

			if (result != null)
			{
				return result;
			}

			return null;
		}		

		public bool Exist (string Name)
		{
			if (this._cookies.Find (delegate (SorentoLib.FastCgi.Cookie cookie) { return cookie.Name == Name; }) != null)
			{
				return true;
			}

			return false;
		}

		public string Bake ()
		{
			string result = string.Empty;

			foreach (Cookie cookie in this._cookies)
			{
				result += cookie.Bake() + "\n";
			}

			return result;
		}
		#endregion
	}
}
