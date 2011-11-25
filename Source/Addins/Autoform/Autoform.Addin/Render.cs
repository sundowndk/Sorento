//
// Render.cs
//
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2009 Rasmus Pedersen
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

namespace Autoform.Addin
{
	public class Render : SorentoLib.Addins.IRender
	{
		#region Private Fields
		private List<string> _namespaces = new List<string> ();
		#endregion

		#region Constructor
		public Render ()
		{
			this._namespaces.Add ("autoform");
		}
		#endregion

		#region Public Methods
		public bool IsProvided (string Namespace)
		{
			return this._namespaces.Exists (delegate (string o) {return (o == Namespace.ToLower ());});
		}

		public object Process (SorentoLib.Session Session, object Variable, string Method, SorentoLib.Render.Resolver.Parameters Parameters)
		{
			return Process (Session, Variable.GetType ().FullName.ToLower (), Method.ToLower (), Variable, Parameters);
		}

		public object Process (SorentoLib.Session Session, string Fullname, string Method, SorentoLib.Render.Resolver.Parameters Parameters)
		{
			return Process (Session, Fullname.ToLower (), Method.ToLower (), null, Parameters);
		}
		#endregion

		#region Private Methods
		private object Process (SorentoLib.Session Session, string Fullname, string Method, object Variable, SorentoLib.Render.Resolver.Parameters Parameters)
		{
			switch (Fullname)
			{
				#region Autoform.Form
				case "autoform.form":
					switch (Method)
					{
						#region Variable
						case "":
							return ((Form)Variable);
						#endregion

						#region Fields
						case "id":
							return ((Form)Variable).Id;

						case "createtimestamp":
							return ((Form)Variable).CreateTimestamp;

							case "updatetimestamp":
							return ((Form)Variable).UpdateTimestamp;

						case "title":
							return ((Form)Variable).Title;

						case "emailto":
							return ((Form)Variable).EmailTo;

						case "emailfrom":
							return ((Form)Variable).EmailFrom;

						case "emailsubject":
							return ((Form)Variable).EmailSubject;

						case "emailbody":
							return ((Form)Variable).EmailBody;

						case "emailbodytype":
							return ((Form)Variable).EmailBodyType;
						#endregion

						#region Methods
						#endregion

						#region Static Methods
						case "load":
							return Form.Load (Parameters.Get<Guid>(0));

						case "list":
//							if (Session.AccessLevel < SorentoLib.Enums.Accesslevel.Author) throw new Exception (string.Format (Autoform.Strings.Exception.ResolverSessionPriviliges, "list"));

							return Form.List ();
						#endregion
					}

					break;
				#endregion

				default:
					throw new Exception (string.Format (Autoform.Strings.Exception.ResolverMethodNotFound, Fullname));
			}

			return null;
		}
		#endregion
	}
}
