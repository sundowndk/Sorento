//
// Page.cs
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2007 Rasmus Pedsersen
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#region Usings
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#endregion

namespace SorentoLib
{
	public class Page
	{
		#region Privat Fields
		private System.Collections.Generic.List<System.String> _lines = new System.Collections.Generic.List<System.String>();

		private static Regex ExpIsFullUrl = new Regex (@"^http", RegexOptions.Compiled);


		private SorentoLib.Render.Variables _variables = new SorentoLib.Render.Variables();

		private string _redirect = string.Empty;
		private string _hostname = string.Empty;
		#endregion

		#region Public Fields
		/// <value>
		/// Lines
		/// </value>
		public System.Collections.Generic.List<System.String> Lines
		{
			set
			{
				this._lines = value;
			}
			get {
				return this._lines;
			}
		}

		/// <value>
		/// Variables
		/// </value>
		public SorentoLib.Render.Variables Variables
		{
			get
			{
				return this._variables;
			}
		}

		public string Redirect
		{
			set
			{
				if (SorentoLib.Page.ExpIsFullUrl.Match (value).Success)
				{
					this._redirect = value;
				}
				else
				{
					this._redirect = "http://"+ this._hostname + value;
				}
			}

			get
			{
				return this._redirect;
			}
		}

		#endregion

		#region Constructors
		/// <summary>
		/// Create new Page
		/// </summary>
		public Page (string Hostname)
		{
			this._hostname = Hostname;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Reset
		/// </summary>
		public void Clear()
		{
			this._lines.Clear();
			this._variables.Clear();
		}

		/// <summary>
		/// Write Page
		/// </summary>
		public System.String Write (SorentoLib.Session session)
		{
			// Definitions
			System.String result = System.String.Empty;
			
			// TODO: What is this?
			//Console.WriteLine(Session.Cgi.HttpHeader(""));
			
			if (this._redirect != string.Empty)
			{
				result += session.Request.HttpRedirect ("UTF-8", this._redirect, SorentoLib.Enums.RedirectType.Location);
			}
			else
			{
				result += session.Request.HttpHeader("UTF-8");
				result += "\n";

				// Write every line in Page.
				foreach (System.String line in this._lines)
				{
					result += line + "\n";
				}

			}

			// Finish.
			return result;
		}
		#endregion
	}
}
