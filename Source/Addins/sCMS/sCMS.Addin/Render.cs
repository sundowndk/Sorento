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

namespace sCMS.Addin
{
	public class Render : SorentoLib.Addins.IRender
	{
		#region Private Fields
		private List<string> _namespaces = new List<string> ();
		#endregion

		#region Constructor
		public Render ()
		{
			this._namespaces.Add ("scms");
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
				#region sCMS.Page
				case "scms.page":

					switch (Method)
					{
						#region Variable
						case "":
							return ((sCMS.Page)Variable);
						#endregion

						#region Fields
						case "id":
							return ((sCMS.Page)Variable).Id;

						case "createtimestamp":
							return ((sCMS.Page)Variable).CreateTimestamp;

						case "updatetimestamp":
							return ((sCMS.Page)Variable).UpdateTimestamp;

						case "name":
							return ((sCMS.Page)Variable).Name;

						case "path":
							return ((sCMS.Page)Variable).Path;

						case "template":
							return ((sCMS.Page)Variable).Template;
						#endregion

						#region Methods
						case "content":
							switch (Parameters.Type (0).Name.ToLower())
							{
								case "guid":
									return ((sCMS.Page)Variable).GetContent (Parameters.Get<Guid>(0));
									break;

								case "string":
									return ((sCMS.Page)Variable).GetContent (Parameters.Get<string>(0));
									break;
							}
							break;
						#endregion

						#region Static Methods
						case "load":
							return sCMS.Page.Load (Parameters.Get<Guid>(0));

						case "list":
							return sCMS.Page.List ();
						#endregion
					}
					break;
				#endregion

				#region sCMS.Template
				case "scms.template":

					switch (Method)
					{
						#region Variable
						case "":
							return ((sCMS.Template)Variable);
						#endregion

						#region Fields
						case "id":
							return ((sCMS.Template)Variable).Id;

						case "createtimestamp":
							return ((sCMS.Template)Variable).CreateTimestamp;

						case "updatetimestamp":
							return ((sCMS.Template)Variable).UpdateTimestamp;

						case "name":
							return ((sCMS.Template)Variable).Title;

						case "fields":
							return SorentoLib.Render.Variables.ConvertToListObject<sCMS.Field> (((sCMS.Template)Variable).Fields);
						#endregion

						#region Methods
						#endregion

						#region Static Methods
						case "list":
							if (Session.AccessLevel < SorentoLib.Enums.Accesslevel.Editor) throw new Exception (string.Format (sCMS.Strings.Exception.ResolverSessionPriviliges, "template.list"));

							return SorentoLib.Render.Variables.ConvertToListObject<sCMS.Template> (sCMS.Template.List ());
						#endregion
					}
					break;
				#endregion

				#region sCMS.Field
				case "scms.field":
					switch (Method)
					{
						#region Variable
						case "":
							return ((sCMS.Field)Variable);
						#endregion

						#region Fields
						case "id":
							return ((sCMS.Field)Variable).Id;

						case "name":
							return ((sCMS.Field)Variable).Name;

						case "type":
							return ((sCMS.Field)Variable).Type;
						#endregion

						#region Methods
						#endregion

						#region Static Methods
						#endregion
					}
					break;
				#endregion

				#region sCMS.Content
				case "scms.contentdata":

					switch (Method)
					{
						#region Variable
						case "":
							return ((sCMS.Content)Variable);
						#endregion

						#region Fields
						case "data":
							return ((sCMS.Content)Variable).Data;
						#endregion

						#region Methods
						#endregion

						#region Static Methods
						#endregion
					}
					break;
				#endregion

				#region sCMS.CollectionSchema
				case "scms.collectionschema":

					switch (Method)
					{
						#region Variable
						case "":
							return ((sCMS.CollectionSchema)Variable);
						#endregion

						#region Name
						case "name":
							return ((sCMS.CollectionSchema)Variable).Name;
						#endregion

						#region Collections
						case "collections":
							return ((sCMS.CollectionSchema)Variable).Collections;
						#endregion

						#region Methods
						#endregion

						#region Static Methods
						case "load":
							switch (Parameters.Type (0).Name.ToLower())
							{
								case "guid":
									return sCMS.CollectionSchema.Load (Parameters.Get<Guid>(0));

								case "string":
									return sCMS.CollectionSchema.Load (new Guid (Parameters.Get<string>(0)));
							}
							break;
						#endregion
					}
					break;
				#endregion

				#region sCMS.Collection
				case "scms.collection":

					switch (Method)
					{
						#region Variable
						case "":
							return ((sCMS.Collection)Variable);
						#endregion

						#region Id
						case "id":
							return ((sCMS.Collection)Variable).Id;
						#endregion

						#region Name
						case "name":
							return ((sCMS.Collection)Variable).Name;
						#endregion

//						#region Name
//						case "content":
//							return ((sCMS.Collection)Variable).Contents;
//						#endregion

						#region Methods
						case "content":
							switch (Parameters.Type (0).Name.ToLower())
							{
								case "guid":
									return ((sCMS.Collection)Variable).GetContent (Parameters.Get<Guid>(0));

								case "string":
									return ((sCMS.Collection)Variable).GetContent (Parameters.Get<string>(0));
							}
							break;
						#endregion

						#region Static Methods
						case "load":
							switch (Parameters.Type (0).Name.ToLower())
							{
								case "guid":
									return sCMS.Collection.Load (Parameters.Get<Guid>(0));

								case "string":
									return sCMS.Collection.Load (new Guid (Parameters.Get<string>(0)));
							}
							break;
						#endregion
					}
					break;
				#endregion

				default:
					throw new Exception (string.Format (sCMS.Strings.Exception.ResolverMethodNotFound, Fullname));
			}

			return null;
		}
		#endregion
	}
}
