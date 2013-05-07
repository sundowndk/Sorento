//
// Template.cs
//
// Author:
//       rvp <${AuthorEmail}>
//
// Copyright (c) 2013 rvp
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
using System.Text;
using System.Collections.Generic;

using SNDK;

namespace SorentoLib
{
	public class Template
	{
		#region Private Fields
		private StringBuilder _content;
		#endregion

		#region Public Fields
		public string Content
		{
			get
			{
				return this._content.ToString ();
			}
		}
		#endregion

		#region Constructor
		public Template (string Filename, Encoding Encoding)
		{
			this._content = new StringBuilder ();
			using (FileStream stream = new FileStream (Filename, FileMode.Open))
			{
				Parse (stream);
			}
		}

		public Template (string Content)
		{
			this._content = new StringBuilder ();
			using (Stream stream = SNDK.Convert.StringToStream (Content, Encoding.UTF8))
			{
				this.Parse (stream);
			}
		}
		#endregion

		#region Private Methods
		private void Parse (Stream Content)
		{
			string line;
			List<string> blockdepth = new List<string> ();

			using (StreamReader streamreader = new StreamReader (Content, Encoding.UTF8))
			{
				while ((line = streamreader.ReadLine ()) != null)
				{
					if (line.Contains ("#begin:csharp#"))
					{
						blockdepth.Add ("csharp");
						continue;
					}

					if (line.Contains ("#end:csharp#"))
					{
						if (blockdepth [(blockdepth.Count - 1)] == "csharp")
						{
							blockdepth.RemoveAt ((blockdepth.Count - 1));
						}
						continue;
					}

					if (line.Contains ("#begin:html#"))
					{
						blockdepth.Add ("html");
						continue;
					}

					if (line.Contains ("#end:html#"))
					{
						if (blockdepth [(blockdepth.Count - 1)] == "html")
						{
							blockdepth.RemoveAt ((blockdepth.Count - 1));
						}
						continue;
					}

					if (blockdepth.Count > 0)
					{
						if (blockdepth [(blockdepth.Count - 1)] == "csharp")
						{
							this._content.AppendLine (line);
						}

						if (blockdepth [(blockdepth.Count - 1)] == "html")
						{
							this._content.AppendLine ("SorentoLib.Parser.Hooks.Output.AppendLine (\"" + line.Replace ("\"", "\\\"") + "\");");
						}
					}
				}
			}

			// Cleanup.
			blockdepth = null;
		}
		#endregion
	}
}

