//
// Plugin.cs
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

#region Includes
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Toolbox;
using Toolbox.DBI;
using Toolbox.Global;

using SorentoLib;
#endregion

namespace sBlog
{
	public class Plugin : SorentoPlugin
	{
		#region Private Fields
		private string _name = string.Empty;	
		private string _version	= string.Empty;	
		private string _author = string.Empty;
		private string _namespace = string.Empty;								
		private List<string> _objecttypes 	= new List<string>();

		private List<Type> _types = new List<Type>();
		private List<String> _namespaces = new List<string>();
		#endregion

		#region Public Properties
		public string Name
		{
			get { return this._name; }
		}

		public string Version
		{
			get { return this._version; }
		}

		public string Author
		{
			get { return this._author; }
		}

		public string Namespace
		{
			get { return this._namespace; }
		}

		public List<string> ObjectTypes
		{
			get { return this._objecttypes; }
		}

		public List<Type> Types
		{
			get { return this._types; }
		}

		public List<string> Namespaces
		{
			get { return this._namespaces; }
		}

		#endregion

 		#region Constructor
		public Plugin()
		{
			this._name = "sBlog";
			this._version = "1.0";
			this._author = "Rasmus Pedersen";
			this._namespace = "sblog";

			this._objecttypes.Add("blog");
			this._objecttypes.Add("entry");
			this._objecttypes.Add("comment");

			this._types.Add(typeof(sBlog.Blog));
			this._types.Add(typeof(sBlog.Entry));
			this._types.Add(typeof(sBlog.Comment));

			this._namespaces.Add(typeof(sBlog.Entry).Namespace);
		}
		#endregion

		#region CGICommand
		public bool CgiCommand(SorentoLib.Session session, string function)
		{
			// Defintions
			bool success = false;

//			switch (function)
//			{
//			}

			// Finish
			return success;
		}
		#endregion

		#region Variable
		public string Variables()
		{
			string result = string.Empty;



			return result;
		}

		/// <summary>
		/// Variable
		/// </summary>
		public string Variable(SorentoLib.Session Session, string ObjectType, string Name, string Value, Dictionary<string, string> Options)
		{
			// Definitions
			string result = string.Empty;

			switch (ObjectType)
			{
				#region Entry
				case "entry":
					switch (Value.ToLower())
					{
						case "id":
//							result = Session.Page.Variables.Get<Entry>(Name).Id.ToString();
							break;

						case "name":
//							result = Session.Page.Variables.Get<Entry>(Name).Name;
							break;

						case "content":
//							result = Session.Page.Variables.Get<Entry>(Name).Content;
							break;

						case "date":
//							result = Session.Page.Variables.Get<Entry>(Name).Date;
							break;

						case "urlname":
//							result = Session.Page.Variables.Get<Entry>(Name).UrlName;
							break;

						case "commentcount":
//							result = Session.Page.Variables.Get<Entry>(Name).CommentCount.ToString();
							break;

					}
					break;
				#endregion

				#region Comment
				case "comment":
					switch (Value.ToLower())
					{
						case "id":
//							result = Session.Page.Variables.Get<Comment>(Name).Id.ToString();
							break;

						case "entryid":
//							result = Session.Page.Variables.Get<Comment>(Name).EntryId.ToString();
							break;

						case "name":
//							result = Session.Page.Variables.Get<Comment>(Name).Name;
							break;

						case "content":
//							result = Session.Page.Variables.Get<Comment>(Name).Content;
							break;

						case "date":
//							result = Session.Page.Variables.Get<Comment>(Name).Date;
							break;

					}
					break;
				#endregion
			}

			// Finish
			return result;
		}
		#endregion

		public object Variables(object variable, string method, List<object> parameters)
		{
			// Definitions
			object result = null;

			switch (variable.GetType().Name.ToLower())
			{
				#region Entry
				case "entry":
					switch (method.ToLower())
					{
						case "id":
							result = ((sBlog.Entry)variable).Id;
							break;

						case "name":
							result = ((sBlog.Entry)variable).Name;
							break;

						case "content":
							result = ((sBlog.Entry)variable).Content;
							break;

						case "date":
							result = ((sBlog.Entry)variable).Date;
							break;

						case "urlname":
							result = ((sBlog.Entry)variable).UrlName;
							break;

						case "commentcount":
							result = ((sBlog.Entry)variable).CommentCount.ToString();
							break;

						case "media":
							result = SorentoLib.Render.Variables.ConvertToListObject<Media>(((sBlog.Entry)variable).Media);
							break;
					}
					break;
				#endregion
			}

			// Finish
			return result;
		}

		public object Methods(Session session, string fullname, List<object> parameters)
		{
			// Definitions
			object result = null;

			switch (fullname.ToLower())
			{
				#region Entry
				case "sblog.entry.list":
					if (parameters.Count < 1)
					{
						result = SorentoLib.Render.Variables.ConvertToListObject<sBlog.Entry>(sBlog.Entry.List());
					}
					if (parameters.Count == 2)
					{
						result = SorentoLib.Render.Variables.ConvertToListObject<sBlog.Entry>(sBlog.Entry.List(Toolbox.Tools.StringToEnum<sBlog.SortBy>((string)parameters[0]), Toolbox.Tools.StringToEnum<sBlog.SortOrder>((string)parameters[1])));
					}
				break;
				#endregion
			}

			// Finish
			return result;
		}

		#region Execute
//		public bool Execute(SorentoLib.Session Session, SorentoLib.Render.Token Token, SorentoLib.Render.Template currenttemplate)
//		{
//			// Definitions.
//			bool success = false;
//
//			switch (Token.Name.ToLower())
//			{
//				case "entry.loadbyurlname":
////				    Session.Page.Variables.Set(Token.Options["variable"], new Entry(), "entry");
////					Session.Page.Variables.Get<Entry>(Token.Options["variable"]).Load(Token.Options["urlname"]);
//					success = true;
//				break;
//
//				#region Entry.List
//				case "entry.list":
//					List<Entry> entries;
////					SorentoLib.Test test;
//					if (Token.Options.ContainsKey("sort"))
//					{
//						entries = Entry.List();
//					}
//					else
//					{
//						entries = Entry.List();
//					}
//
////					Session.Page.Variables.Set("TESTVAR", Entry.List(), "list");
////					List<object> testtest = Session.Page.Variables.Get<List<object>>("TESTVAR");
////					List<object> testtest = Entry.List();
//
////					foreach (object obj in testtest)
////					{
////					}
//
//
////					foreach (object obj in Session.Page.Variables.Get<List<object>>("TESTVAR"))
////					foreach (Entry obj in Session.Page.Variables.Get<List<Entry>>("TESTVAR"))
////					{
////						Session.Page.Variables.Set(Token.VariableName, obj, "entry");
////						Template loop = new Template(Session, (List<string>)Token.Data);
////						loop.Render();
////					}
//
////					foreach (Entry entry in entries)
////					{
////						Session.Page.Variables.Set(Token.VariableName, entry, "entry");
////						Template loop = new Template(Session, (List<string>)Token.Data);
////						loop.Render();
////					}
//
//					success = true;
//					break;
//				#endregion
//
//		  		 case "entry.images":
//		  		   if (Token.Options.ContainsKey("fromvariable"))
//		 		   {
////				       foreach (Media media in Session.Page.Variables.Get<Entry>(Token.Options["fromvariable"]).Media)
////					   {
////					       Session.Page.Variables.Set(Token.VariableName, media, "media");
////						   SorentoLib.Render.Template loop = new SorentoLib.Render.Template(Session, (List<string>)Token.Data);
////						   loop.Render();
////					   }
//				   }
//				   success = true;
//				   break;
//
//				#region Comment.List
//				case "comment.list":
//					foreach (Comment comment in Comment.List(new Guid(Token.Options["entryid"])))
//					{
////						Session.Page.Variables.Set(Token.VariableName, comment, "comment");
////						SorentoLib.Render.Template loop = new SorentoLib.Render.Template(Session, (List<string>)Token.Data);
////						loop.Render();
//					}
//
//					success = true;
//					break;
//				#endregion
//			}
//
//			// Finish.
//			return success;
//		}
		#endregion

		#region Ajax
		public AjaxRespons AjaxCommand(SorentoLib.Session session, string function)
		{
			// Definitions
			AjaxRespons result = new AjaxRespons();
			AjaxRequest request = new AjaxRequest(session.Cgi.QueryJar.Query("data").Value);

			switch (function)
			{
				#region Blog
				#region Create
				case "blog.create":
				{
					try
					{
						// Create new blog
						Blog blog = new Blog();

						// Add data from AJAX object
						blog.Name = request.Data<string>("name");

						result.Data.Add("success", blog.Save().ToString().ToLower());
					}
					catch
					{
					}
				}
				break;
				#endregion

				#region Get
				case "blog.get":
				{
					try
					{
						Blog blog = new Blog();

						// Load existing blog
						blog.Load(new Guid(request.Data<string>("id")));

						// Create AJAX object.
						result.Data.Add("id", blog.Id.ToString());
						result.Data.Add("createdate", blog.CreateTimestamp);
						result.Data.Add("updatedate", blog.UpdateTimestamp);
						result.Data.Add("name", blog.Name);

						result.Data.Add("success", "true");
					}
					catch
					{
					}
				}
				break;
				#endregion

				#region Update
				case "blog.update":
				{
					try
					{
						Blog blog = new Blog();

						// Load existing blog
						if (blog.Load(new Guid(request.Data<string>("id"))))
						{

							// Update from AJAX object
							if (request.ContainsVariable("name"))
							{
								blog.Name = request.Data<string>("name");
							}

							result.Data.Add("success", blog.Save().ToString().ToLower());
						}
					}
					catch
					{
					}
				}
				break;
				#endregion

				#region Delete
				case "blog.delete":
				{
					try
					{
						result.Data.Add("success", Blog.Delete(new Guid(request.Data<string>("id"))).ToString().ToLower());
					}
					catch
					{
					}
				}
				break;
				#endregion

				#region List
				case "blog.list":
				{
					try
					{
						List<Hashtable> blogs = new List<Hashtable>();
						foreach (Blog blog in Blog.List())
						{
							Hashtable item = new Hashtable();
							item.Add("id", blog.Id.ToString());
							item.Add("createtimestamp", blog.CreateTimestamp.ToString());
							item.Add("updatetimestamp", blog.UpdateTimestamp.ToString());
							item.Add("name", blog.Name);
							blogs.Add(item);
						}
						result.Data.Add("blogs", blogs);
						result.Data.Add("success", "true");
					}
					catch
					{
					}
				}
				break;
				#endregion
				#endregion

				#region Entry
				#region Create
				case "entry.create":
				{
					try
					{
						// Create new entry
						Entry entry = new Entry();

						// Add data from AJAX object
						entry.Name = request.Data<string>("name");
						entry.BlogId = new Guid(request.Data<string>("blogid"));
						entry.Content = request.Data<string>("content");

						foreach (Hashtable item in request.Data<List<Hashtable>>("tags"))
						{
							string tag = item["name"].ToString();
							entry.Tags.Add(tag);
						}

						result.Data.Add("success", entry.Save().ToString().ToLower());
					}
					catch
					{
					}
				}
				break;
				#endregion

				#region Get
				case "entry.get":
				{
					try
					{
						Entry entry = new Entry();

						// Load existing entry
						entry.Load(new Guid(request.Data<string>("id")));

						// Create AJAX object.
						result.Data.Add("id", entry.Id.ToString());
						result.Data.Add("createdate", entry.CreateTimestamp);
						result.Data.Add("updatedate", entry.UpdateTimestamp);
						result.Data.Add("name", entry.Name);
						result.Data.Add("blogid", entry.BlogId.ToString());
						result.Data.Add("blogname", entry.BlogName);
						result.Data.Add("content", entry.Content);

						List<Hashtable> images = new List<Hashtable>();
						foreach (Media media in entry.Media)
						{
							Hashtable item = new Hashtable();
							item.Add("id", media.Id);
							item.Add("filename", media.Filename);
							item.Add("filesize", media.Size);
							images.Add(item);
						}
						result.Data.Add("images", images);

						List<Hashtable> tags = new List<Hashtable>();
						foreach (string tag in entry.Tags)
						{
							Hashtable item = new Hashtable();
							item.Add("name", tag);
							tags.Add(item);
						}
						result.Data.Add("tags", tags);

						result.Data.Add("success", "true");
					}
					catch
					{
					}
				}
				break;
				#endregion

				#region Update
				case "entry.update":
				{
//					try
//					{
						Entry entry = new Entry();

						// Load existing entry
						if (entry.Load(new Guid(request.Data<string>("id"))))
						{
							// Update from AJAX object
							if (request.ContainsVariable("name"))
							{
								entry.Name = request.Data<string>("name");
							}

							if (request.ContainsVariable("blogid"))
							{
								entry.BlogId = new Guid(request.Data<string>("blogid"));
							}

							if (request.ContainsVariable("content"))
							{
								entry.Content = request.Data<string>("content");
							}

							if (request.ContainsVariable("tags"))
							{
								entry.Tags.Clear();
								foreach (Hashtable item in request.Data<List<Hashtable>>("tags"))
								{
									string tag = item["name"].ToString();
									entry.Tags.Add(tag);
								}
							}

							if (request.ContainsVariable("images"))
							{
								entry.Media.Clear();
								foreach (Hashtable item in request.Data<List<Hashtable>>("images"))
								{
									Media media = new Media();
									if (media.Load(new Guid(item["id"].ToString())))
									{
										entry.Media.Add(media);
										media.IsTemporary = false;
										media.Save();
									}
								}
							}

							result.Data.Add("success", entry.Save().ToString().ToLower());
						}
//					}
//					catch
//					{
//					}
				}
				break;
				#endregion

				#region Delete
				case "entry.delete":
				{
					try
					{
						result.Data.Add("success", Entry.Delete(new Guid(request.Data<string>("id"))).ToString().ToLower());
					}
					catch
					{
					}
				}
				break;
				#endregion

				#region List
				case "entry.list":
				{
					try
					{
						List<Hashtable> entries = new List<Hashtable>();
						foreach (Entry entry in Entry.List())
						{
							Hashtable item = new Hashtable();
							item.Add("id", entry.Id.ToString());
							item.Add("createtimestamp", entry.CreateTimestamp.ToString());
							item.Add("updatetimestamp", entry.UpdateTimestamp.ToString());
							item.Add("name", entry.Name);
							item.Add("blogid", entry.BlogId);
							item.Add("blogname", entry.BlogName);
							item.Add("tagasstring", entry.TagsAsString);
							entries.Add(item);
						}
						result.Data.Add("entries", entries);
						result.Data.Add("success", "true");
					}
					catch
					{
					}
				}
				break;
				#endregion
				#endregion


				#region Comment
				#region Create
				case "comment.create":
				{
					try
					{
						// Create new entry
						Comment comment = new Comment();

						// Add data from AJAX object
						comment.EntryId = new Guid(request.Data<string>("entryid"));
						comment.Name = request.Data<string>("name");
						comment.Content = request.Data<string>("content");

						result.Data.Add("success", comment.Save().ToString().ToLower());
					}
					catch
					{
					}
				}
				break;
				#endregion

				#region Delete
				case "comment.delete":
				{
//					try
//					{
						result.Data.Add("success", Comment.Delete(new Guid(request.Data<string>("id"))).ToString().ToLower());
//					}
//					catch
//					{
//					}
				}
				break;
				#endregion

				#region List
				case "comment.list":
				{
					try
					{
						Guid entryid = new Guid(request.Data<string>("filterdata"));

						List<Hashtable> comments = new List<Hashtable>();
						foreach (Comment comment in Comment.List(entryid))
						{
							Hashtable item = new Hashtable();
							item.Add("id", comment.Id.ToString());
							item.Add("createtimestamp", comment.CreateTimestamp.ToString());
							item.Add("updatetimestamp", comment.UpdateTimestamp.ToString());
							item.Add("name", comment.Name);
							item.Add("content", comment.Content);
							item.Add("date", comment.Date);
							comments.Add(item);
						}
						result.Data.Add("comments", comments);
						result.Data.Add("success", "true");
					}
					catch
					{
					}
				}
				break;
				#endregion
				#endregion
			}

			// Finish
			return result;
		}
		#endregion
	}
}
