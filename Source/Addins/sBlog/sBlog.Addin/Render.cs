//
// Render.cs
//
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2011 Rasmus Pedersen
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

namespace sBlog.Addin
{
	public class Render : SorentoLib.Addins.IRenderBaseClass, SorentoLib.Addins.IRender
	{
		#region Constructor
		public Render ()
		{
			base.NameSpaces.Add ("sblog");
		}
		#endregion

		#region Private Methods
		override public object Process (SorentoLib.Session Session, string Fullname, string Method, object Variable, SorentoLib.Render.Resolver.Parameters Parameters)
		{
			switch (Fullname)
			{
				#region sBlog.Blog
				case "sblog.blog":
					switch (Method)
					{
						#region Variable
						case "":
							return ((sBlog.Blog)Variable);
						#endregion

						#region Fields
						case "id":
							return ((sBlog.Blog)Variable).Id;

						case "createtimestamp":
							return ((sBlog.Blog)Variable).CreateTimestamp;

						case "updatetimestamp":
							return ((sBlog.Blog)Variable).UpdateTimestamp;

						case "name":
							return ((sBlog.Blog)Variable).Name;

						case "entries":
							return ((sBlog.Blog)Variable).Entries;
						#endregion

						#region Static Methods
						case "load":
							switch (Parameters.Type (0).Name.ToLower())
							{
								case "guid":
									return sBlog.Blog.Load (Parameters.Get<Guid>(0));

								case "string":
									return sBlog.Blog.Load (new Guid (Parameters.Get<string>(0)));
							}
							break;

						case "list":
							return sBlog.Blog.List ();
						#endregion
					}
					break;
				#endregion

				#region sBlog.Entry
				case "sblog.entry":
					switch (Method)
					{
						#region Variable
						case "":
							return ((sBlog.Entry)Variable);
						#endregion

						#region Fields
						case "id":
							return ((sBlog.Entry)Variable).Id;

						case "createtimestamp":
							return ((sBlog.Entry)Variable).CreateTimestamp;

						case "updatetimestamp":
							return ((sBlog.Entry)Variable).UpdateTimestamp;

						case "title":
							return ((sBlog.Entry)Variable).Title;

						case "tags":
							return ((sBlog.Entry)Variable).Tags;
						#endregion

						#region Methods
						case "content":
							switch (Parameters.Type (0).Name.ToLower())
							{
								case "guid":
									return ((sBlog.Entry)Variable).GetContent (Parameters.Get<Guid>(0));

								case "string":
									return ((sBlog.Entry)Variable).GetContent (Parameters.Get<string>(0));
							}
							break;
						#endregion

						#region Static Methods
						case "load":
							return sBlog.Entry.Load (Parameters.Get<Guid>(0));

						case "list":
							return sBlog.Entry.List ();
						#endregion
					}
					break;
				#endregion

				default:
					throw new Exception (string.Format (Strings.Exception.ResolverMethodNotFound, Fullname));
			}

			return null;
		}
		#endregion
	}
}
