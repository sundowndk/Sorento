//
// Ajax.cs
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

namespace Google.Addin
{
	public class Ajax : SorentoLib.Addins.IAjax
	{
		#region Private Fields
		private List<string> _namespaces = new List<string> ();
		#endregion

		#region Constructor
		public Ajax ()
		{
			this._namespaces.Add ("google");
		}
		#endregion

		#region Public Methods
		public bool IsProvided (string Namespace)
		{
			return this._namespaces.Exists (delegate (string s) {return (s == Namespace.ToLower ());});
		}

		public SorentoLib.Ajax.Respons Process (SorentoLib.Session Session, string Fullname, string Method)
		{
			SorentoLib.Ajax.Respons result = new SorentoLib.Ajax.Respons ();
			SorentoLib.Ajax.Request request = new SorentoLib.Ajax.Request (Session.Request.QueryJar.Get ("data").Value);

			switch (Fullname.ToLower ())
			{
				#region Google.
				case "google.":
					break;
				#endregion
			}

			return result;
		}
		#endregion
	}
}
