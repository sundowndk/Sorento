//
// IAjaxBaseClass.cs
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

namespace SorentoLib.Addins
{
	public class IAjaxBaseClass
	{
		#region Private Fields
		private List<string> _namespaces = new List<string> ();
		#endregion

		#region Public Fields
		public List<string> NameSpaces
		{
			get
			{
				return this._namespaces;
			}
		}
		#endregion

		#region Constructor
		public IAjaxBaseClass ()
		{
		}
		#endregion

		#region Public Methods
		public bool IsProvided (string Namespace)
		{
			return this._namespaces.Exists (delegate (string s) {return (s == Namespace.ToLower ());});
		}

		public SorentoLib.Ajax.Respons Process (SorentoLib.Session Session, string Fullname, string Method)
		{
			return null;
		}
		#endregion
	}
}
