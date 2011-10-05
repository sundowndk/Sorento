//
// Method.cs
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
using System.Collections.Generic;

using Mono.Addins;

using SorentoLib;

namespace sBlog.Addin
{
	class Method : SorentoLib.Addins.IMethod
	{
		#region Private Fields
		private List<Type> _types = new List<Type> ();
		#endregion

		#region Public Fields
		public List<Type> Types
		{
			get
			{
				return this._types;
			}
		}
		#endregion

		#region Constructor
		public Method ()
		{
			this._types.Add (typeof(sBlog.Blog));
			this._types.Add (typeof(sBlog.Entry));
			this._types.Add (typeof(sBlog.Content));
			this._types.Add (typeof(sBlog.Comment));
		}
		#endregion

		#region Public Methods
		public bool IsProvided (string Namespace)
		{
			foreach (Type type in this._types)
			{
				if (type.Namespace.ToLower() == Namespace.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public object NonStatic (object Variable, string Fullname, List<object> Parameters)
		{
			string n = string.Empty;
			string method = string.Empty;

			switch (n.ToLower ())
			{
				#region Blog
				case "blog":
					switch (method.ToLower ())
					{
						case "load":
							return ((sBlog.Blog)Variable).Load((Guid)Parameters[0]);
						case "save":
							return ((sBlog.Blog)Variable).Save();
						case "delete":
							return ((sBlog.Blog)Variable).Delete();
					}
				#endregion

				#region Entry
				case "entry":
					switch (method.ToLower ())
					{
						case "load":
							return ((sBlog.Entry)Variable).Load((Guid)Parameters[0]);
						case "save":
							return ((sBlog.Entry)Variable).Save();
						case "delete":
							return ((sBlog.Entry)Variable).Delete();
					}
				#endregion

				#region Content
				case "content":
					switch (method.ToLower ())
					{
						case "load":
							return ((sBlog.Content)Variable).Load((Guid)Parameters[0]);
						case "save":
							return ((sBlog.Content)Variable).Save();
						case "delete":
							return ((sBlog.Content)Variable).Delete();
					}
				#endregion

				#region Comment
				case "comment":
					switch (method.ToLower ())
					{
						case "load":
							return ((sBlog.Comment)Variable).Load((Guid)Parameters[0]);
						case "save":
							return ((sBlog.Comment)Variable).Save();
						case "delete":
							return ((sBlog.Comment)Variable).Delete();
					}
				#endregion
			}
		}

		public object Static (SorentoLib.Session Session, string Fullname, List<object> Parameters)
		{
			switch (Fullname.ToLower ())
			{
				#region Blog
				case "sblog.blog.delete":
					switch (Parameters.Count)
					{
						default:
							return sBlog.Blog.Delete((Guid)Parameters[0]);
					}

				case "sblog.blog.list":
					switch (Parameters.Count)
					{
						default:
							return SorentoLib.Render.Variables.ConvertToListObject<sBlog.Blog> (sBlog.Blog.List ());
					}
				#endregion

				#region Entry
				case "sblog.entry.list":
					switch (Parameters.Count)
					{
						case 2:
							return SorentoLib.Render.Variables.ConvertToListObject<sBlog.Entry> (sBlog.Entry.List (Toolbox.Tools.StringToEnum<sBlog.SortBy> ((string)Parameters[0]), Toolbox.Tools.StringToEnum<sBlog.SortOrder> ((string)Parameters[1])));
						default:
							return SorentoLib.Render.Variables.ConvertToListObject<sBlog.Entry> (sBlog.Entry.List ());
					}
				#endregion

				#region Content
				#endregion

				#region Comment
				case "sblog.comment.list":
					switch (Parameters.Count)
					{
						case 1:
							return SorentoLib.Render.Variables.ConvertToListObject<sBlog.Comment> (sBlog.Comment.List((Guid)Parameters[0]));
						default:
						break;
					}
				#endregion
			}

			// Finish
			return null;
		}
		#endregion
	}
}
