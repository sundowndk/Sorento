//
// DatastoreItem.cs
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
using System.Xml;
using System.IO;

namespace SorentoLib.Services
{
	public class DatastoreItem
	{
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		private string _aisle;
		private string _shelf;
		private string _data;
		private string _meta;

		public Guid Id
		{
			get
			{
				return this._id;
			}
		}

		public int CreateTimestamp
		{
			get
			{
				return this._createtimestamp;
			}
		}

		public int UpdateTimestamp
		{
			get
			{
				return this._updatetimestamp;
			}
		}

		public string Aisle
		{
			get
			{
				return this._aisle;
			}
		}

		public string Shelf
		{
			get
			{
				return this._shelf;
			}
		}

		public string Data
		{
			get
			{
				return this._data;
			}
		}

		public T Get<T> ()
		{
			try
			{
				switch (typeof (T).Name.ToLower ())
				{
//					case "guid":
//						return (T)System.Convert.ChangeType (new Guid (Get (Aisle, Shelf)), typeof(T));
//
//					case "list`1":
//						return (T)System.Convert.ChangeType (Serializer.DeSerializeObjectFromString<T> (Get (Aisle, Shelf)), typeof(T));

					case "string":
					{
						return (T)System.Convert.ChangeType (this._data, typeof (T));
					}
//
					case "xmldocument":
					{
						return (T)System.Convert.ChangeType (SNDK.Convert.StringToXmlDocument (this._data), typeof(T));
					}

					default:
						XmlDocument xml = new XmlDocument ();
						xml.Load (new StringReader (this._data));
//
						return (T)typeof (T).GetMethod ("FromXmlDocument").Invoke (null, new Object[] { xml });

//						return (T)System.Console
//						return (T)System.Convert.ChangeType (Get (Aisle, Shelf), typeof(T));
				}
			}
			catch (Exception exception)
			{
				throw new Exception (string.Format (Strings.Exception.ServicesDatastoreLocationNotValidType, Aisle +"."+ Shelf, typeof (T).Name));
			}
		}

		public string Meta
		{
			get
			{
				return this._meta;
			}
		}

		internal DatastoreItem (Guid Id, int CreateTimestamp, int UpdateTimestamp, string Aisle, string Shelf, string Data, string Meta)
		{
			this._id = Id;
			this._createtimestamp = CreateTimestamp;
			this._updatetimestamp = UpdateTimestamp;
			this._aisle = Aisle;
			this._shelf = Shelf;
			this._data = Data;
			this._meta = Meta;
		}


	}
}

