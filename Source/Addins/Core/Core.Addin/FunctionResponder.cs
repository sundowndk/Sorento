//
// FunctionResponder.cs
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
using System.IO;

using Mono.Addins;

using SorentoLib;

namespace Core.Addin
{
	public class FunctionResponder : SorentoLib.Addins.IFunctionResponder
	{
		#region Constructor
		public FunctionResponder ()
		{
		}
		#endregion

		#region Public Methods
		public void Process (SorentoLib.Session Session)
		{
			// True if function returns successfully.
			bool success = false;

			// TODO: fix all this.
			string resulttemplate = string.Empty;


			// Find Addin to handle resolve.
			SorentoLib.Tools.ParseTypeName typename = new SorentoLib.Tools.ParseTypeName (Session.Request.QueryJar.Get ("cmd.function").Value);
			foreach (SorentoLib.Addins.IFunction function in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IFunction)))
			{
				if (function.IsProvided (typename.Namspace))
				{
					success = function.Process (Session, typename.Fullname, typename.Method);					
					break;
				}
			}

			// If function returned successfully we need to show the success page, if it has been specified.
			if (success)
			{
				if (Session.Request.QueryJar.Exist ("cmd.onsuccess"))
				{
					resulttemplate = Session.Request.QueryJar.Get ("cmd.onsuccess").Value;
				}
			}
			// If function returned unsuccessfull we should show the error page, if it has been specified.
			else
			{
				if (Session.Request.QueryJar.Exist ("cmd.onerror"))
				{
					resulttemplate = Session.Request.QueryJar.Get ("cmd.onerror").Value;
				}
			}

			// If redirect has been specfied lets do that. This is good for hiding POST urls.
			if (Session.Request.QueryJar.Exist ("cmd.redirect"))
			{
				if (Session.Request.QueryJar.Get ("cmd.redirect").Value.ToLower () == "true")
				{
					Session.Page.Clear ();
					Session.Page.Redirect = resulttemplate;
//					Session.Page.Lines.Add(@"<meta HTTP-EQUIV=""REFRESH"" content=""0; url="+ resulttemplate +@""">");
					Session.Responder.Request.SendOutputText (Session.Page.Write (Session));
					return;
				}
			}

			if (resulttemplate != string.Empty)
			{
				SorentoLib.Render.Template template = new SorentoLib.Render.Template (Session, resulttemplate);
				template.Render ();
				template = null;
			}

			Session.Responder.Request.SendOutputText (Session.Page.Write (Session));
		}
		#endregion
	}
}
