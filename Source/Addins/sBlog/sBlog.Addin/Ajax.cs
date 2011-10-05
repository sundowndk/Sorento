//
// Ajax.cs
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
using System.Collections;
using System.Collections.Generic;

using SorentoLib;

namespace sBlog.Addin
{
	public class Ajax : SorentoLib.Addins.IAjaxBaseClass, SorentoLib.Addins.IAjax
	{
		#region Constructor
		public Ajax ()
		{
			base.NameSpaces.Add ("sblog");
		}
		#endregion

		#region Public Methods
		new public SorentoLib.Ajax.Respons Process (SorentoLib.Session Session, string Fullname, string Method)
		{
			SorentoLib.Ajax.Respons result = new SorentoLib.Ajax.Respons ();
			SorentoLib.Ajax.Request request = new SorentoLib.Ajax.Request (Session.Request.QueryJar.Get ("data").Value);

			switch (Fullname.ToLower ())
			{
				#region sBlog.Blog
				case "sblog.blog":
					switch (Method.ToLower ())
					{
						#region New
						case "new":
						{
							Blog blog = Blog.FromAjaxRequest (request);
							blog.Save ();
							blog.ToAjaxResponse (result);

							break;
						}
						#endregion

						#region Load
						case "load":
						{
							Blog blog = Blog.Load (new Guid (request.Key<string> ("id")));
							blog.ToAjaxResponse (result);

							break;
						}
						#endregion

						#region Save
						case "save":
						{
							Blog blog = Blog.FromAjaxRequest (request);
							blog.Save ();

							break;
						}
						#endregion

						#region Delete
						case "delete":
						{
							Blog.Delete (new Guid (request.Key<string> ("id")));

							break;
						}
						#endregion

						#region List
						case "list":
						{
							List<Hashtable> blogs = new List<Hashtable> ();
							foreach (Blog blog in Blog.List ())
							{
								blogs.Add (blog.ToItem ());
							}
							result.Data.Add ("blogs", blogs);

							break;
						}
						#endregion
					}
					break;
				#endregion

				#region sBlog.Entry
				case "sblog.entry":
					switch (Method.ToLower ())
					{
						#region New
						case "new":
						{
							Entry entry = Entry.FromAjaxRequest (request);
							entry.Save ();
							entry.ToAjaxResponse (result);

							break;
						}
						#endregion

						#region Load
						case "load":
						{
							Entry entry = Entry.Load (new Guid (request.Key<string> ("id")));
							entry.ToAjaxResponse (result);

							break;
						}
						#endregion

						#region Save
						case "save":
						{
							Entry entry = Entry.FromAjaxRequest (request);
							entry.Save ();

							break;
						}
						#endregion

						#region Delete
						case "delete":
						{
							Entry.Delete (new Guid (request.Key<string> ("id")));

							break;
						}
						#endregion

						#region List
						case "list":
						{
							List<Hashtable> entries = new List<Hashtable> ();
							foreach (Entry entry in Entry.List ())
							{
								Console.WriteLine (entry.Title);
								entries.Add (entry.ToItem ());
							}
							result.Data.Add ("entries", entries);

							break;
						}
						#endregion
					}
					break;
				#endregion

				#region sBlog.Comment
				case "sblog.comment":
					switch (Method.ToLower ())
					{
						#region New
						case "new":
						{
							Comment comment = Comment.FromAjaxRequest (request);
							comment.Save ();
							comment.ToAjaxResponse (result);

							break;
						}
						#endregion

						#region Load
						case "load":
						{
							Comment comment = Comment.Load (new Guid (request.Key<string> ("id")));
							comment.ToAjaxResponse (result);

							break;
						}
						#endregion

						#region Save
						case "save":
						{
							Comment comment = Comment.FromAjaxRequest (request);
							comment.Save ();

							break;
						}
						#endregion

						#region Delete
						case "delete":
						{
							Comment.Delete (new Guid (request.Key<string> ("id")));

							break;
						}
						#endregion

						#region List
//						case "list":
//						{
//							List<Hashtable> entries = new List<Hashtable> ();
//							foreach (Entry entry in Entryu.List ())
//							{
//								blogs.Add (blog.ToItem ());
//							}
//							result.Data.Add ("blogs", blogs);
//
//							break;
//						}
						#endregion
					}
					break;
				#endregion
			}

			return result;
		}
		#endregion
	}
}
