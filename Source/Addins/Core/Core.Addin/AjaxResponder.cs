//
// AjaxResponder.cs
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
using System.Xml;
using System.Collections;
using Mono.Addins;

using SorentoLib;

namespace Core.Addin
{
	public class AjaxResponder : SorentoLib.Addins.IAjaxResponder
	{
		#region Constructor
		public AjaxResponder ()
		{
		}
		#endregion

		#region Public Methods
		public void Process (SorentoLib.Session session)
		{
			SorentoLib.Ajax.Respons respons = null;
			SorentoLib.Tools.ParseTypeName typename = new SorentoLib.Tools.ParseTypeName (session.Request.QueryJar.Get ("cmd.function").Value);

			try
			{
				// Find ajax addin that can respond to this request.
				foreach (SorentoLib.Addins.IAjax ajax in AddinManager.GetExtensionObjects (typeof (SorentoLib.Addins.IAjax)))
				{


					if (ajax.IsProvided (typename.Namspace))
					{
						respons = ajax.Process (session, typename.Fullname, typename.Method);

						Hashtable status = new Hashtable ();
						status.Add ("success", true);
						respons.Add (status);

						break;
					}
				}

				// If no addin was found, throw exception.
				if (respons == null)
				{
					throw new Exception (string.Format ("[AJAX]: Namespace '{0}' was not found.", typename.Namspace));
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine (exception);
				// Handel exceptions, and parse the information onto the client.
				respons = new SorentoLib.Ajax.Respons ();

				Hashtable status = new Hashtable ();
				status.Add ("success", false);
				status.Add ("exception", exception.Message);

				respons.Add (status);
			}

			// Output ajax respons.
			session.Responder.Request.SendOutputText (session.Request.HttpHeader ("UTF-8", "text/xml"));
			session.Responder.Request.SendOutputText ("\n" + respons.XmlDocument.OuterXml);

			// Cleanup
			typename = null;
			respons = null;
		}
		#endregion
	}
}
