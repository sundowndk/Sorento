//
// Function.cs
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

using SorentoLib;

namespace Autoform.Addin
{
	public class Function : SorentoLib.Addins.IFunction
	{
		#region Private Fields
		private List<string> _namespaces = new List<string> ();
		#endregion

		#region Constructor
		public Function ()
		{
			this._namespaces.Add ("autoform");
		}
		#endregion

		#region Public Methods
		public bool IsProvided (string Namespace)
		{
			return this._namespaces.Exists (delegate (string s) {return (s == Namespace.ToLower ());});
		}

		public bool Process (SorentoLib.Session Session, string Fullname, string Method)
		{
			switch (Fullname.ToLower ())
			{
				#region Form
				case "autoform.form":

					switch (Method.ToLower ())
					{
						case "send":
							Form form = Form.Load (new Guid (Session.Request.QueryJar.Get ("cmd.formid").Value));
							form.Send (Session);

							break;
					}

					break;
				#endregion

				default:
					throw new Exception (string.Format (Autoform.Strings.Exception.FunctionMethodNotFound, Fullname));
			}

			return true;
		}
		#endregion
	}
}
