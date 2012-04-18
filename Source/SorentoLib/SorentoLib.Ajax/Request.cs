//
// AjaxReqeust.cs
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2009 Rasmus Pedsersen
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

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace SorentoLib.Ajax
{	
	public class Request
	{
		#region REMOVE
		public T Key<T>(string variablename)
		{
			// Definitions
			T result = default(T);

			try
			{
				if (this._data.ContainsKey(variablename))
				{
					result = (T)this._data[variablename];
				}
			}
			catch {}

			// Finish
			return (T)result;
		}

		public bool ContainsVariable(string variablename)
		{
			// Defintions
			bool result = false;

			result =  this._data.ContainsKey(variablename);

			// Finish
			return result;
		}

		private Hashtable _data = new Hashtable ();
		private Hashtable _data2 = new Hashtable ();
		public Hashtable Data
		{
			get
			{
				return this._data;
			}
		}

		public Hashtable Data2
		{
			get
			{
				return this._data2;
			}
		}

//		public T GetValue<T> (string xPath)
//		{
//			T result = default(T);
//
//			try
//			{
//				result = (T)this._data[variablename];
//			}
//			catch {}
//
//			// Finish
//			return (T)result;
//		}
		#endregion

		#region Private Properties
		private XmlDocument _xmldocument;
		#endregion

		#region Public Properties
		public XmlDocument XmlDocument
		{
			get
			{
				return this._xmldocument;
			}
		}
		#endregion

		#region Constructor
		public Request (string data)
		{
			// Create new xmldocument from data.
			this._xmldocument = new XmlDocument ();
			this._xmldocument.Load (new StringReader (data));
		}
		#endregion

		#region Public Methods
		public bool xPathExists (string xPath)
		{
			try
			{
				GetXml (xPath);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool ContainsXPath (string xPath)
		{
			bool result = false;

			XmlDocument search = new XmlDocument ();

			try
			{
				// Search for the given xPath.
				search.AppendChild (search.ImportNode (this._xmldocument.SelectSingleNode ("/ajax/"+ xPath), true));
				result = true;
			}
			catch
			{
			}

			search = null;

			return result;
		}

		public T getValue<T> (string xPath)
		{
			T result = default (T);

			XmlDocument xml = GetXml (xPath);

			try
			{
				if (typeof (T) == typeof (string))
				{
					result = (T)(object)xml.DocumentElement.InnerText;
				}
				else if (typeof (T) == typeof (Guid))
				{
					result = (T)(object)new Guid (xml.DocumentElement.InnerText);
				}
				else if (typeof (T).BaseType == typeof (Enum))
				{
					result = SNDK.Convert.StringToEnum<T> (xml.DocumentElement.InnerText);
				}
				else
				{
					result = (T)typeof (T).GetMethod ("FromXmlDocument").Invoke (null, new Object[] { xml });
				}
			}
			catch (Exception e)
			{
				throw new Exception (string.Format (Strings.Exception.AjaxRequestCouldNotCastType, xPath.ToUpper (), typeof(T).Name.ToUpper (), xml.DocumentElement.Attributes["type"].Value.ToString().ToUpper (), e));
			}

			return result;
		}

		public XmlDocument GetXml (string xPath)
		{
			XmlDocument result = new XmlDocument ();

			try
			{
				// Search for the given xPath.
				result.AppendChild (result.ImportNode (this._xmldocument.SelectSingleNode ("/ajax/"+ xPath), true));
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.AjaxRequestXPathNotFound, xPath));
			}

			return result;
		}
		#endregion
	}
}

#region OLD
//public void Test (XmlNodeList Nodes, Hashtable Data)
//		{
//			foreach (XmlNode node in Nodes)
//			{
//				switch (node.Attributes["type"].Value.ToString().ToLower())
//				{
//					case "string":
//					{
//						Data.Add(node.Name, node.InnerText);
//						break;
//					}
//
//					case "boolean":
//					{
//						Data.Add (node.Name, SNDK.Convert.IntToBool (int.Parse (node.InnerText)));
//						break;
//					}
//
//					case "object":
//					{
////						Console.WriteLine (node.Name);
//						Hashtable hashtable = new Hashtable ();
//						Test (node.ChildNodes, hashtable);
//						Data.Add (node.Name, hashtable);
//
//						XmlDocument bla = new XmlDocument ();
////						XmlElement element = bla.CreateElement ("", node.Name, "");
//
//						bla.AppendChild (bla.ImportNode (node, true));
////						bla.AppendChild (element);
//
//
//						this._data2.Add (node.Name, bla);
//						break;
//					}
//
//					case "hashtable":
//					{
//						Hashtable hashtable = new Hashtable ();
//						Test (node.ChildNodes, hashtable);
//						Data.Add (node.Name, hashtable);
//
//						break;
//					}
//
//					case "list":
//					{
//						List<Hashtable> list = new List<Hashtable>();
//						foreach (XmlNode node2 in node.ChildNodes)
//						{
//							Hashtable hashtable = new Hashtable ();
//							Test (node2.ChildNodes, hashtable);
//
//							list.Add (hashtable);
//						}
//						Data.Add (node.Name, list);
//
//						break;
//					}
//				}
//			}
//		}


//				if (node.Attributes["type"].Value.ToString().ToLower() == "string")
//				{
//					this._data.Add(node.Name, node.InnerText);
//				}
//				else if (node.Attributes["type"].Value.ToString().ToLower() == "list")
//				{
//					List<Hashtable> itemlist = new List<Hashtable>();
//					foreach (XmlNode a in node.ChildNodes)
//					{
//						Hashtable item = new Hashtable();
//
//						foreach (XmlNode b in a.ChildNodes)
//						{
//							item.Add(b.Name, b.InnerText);
//						}
//
//						itemlist.Add(item);
//					}
//					this._data.Add(node.Name, itemlist);
//				}
//			}


//		public Request(string data)
//		{
//			// Parse xml.
//
//			XmlDocument xml = new XmlDocument();
//			xml.Load(new StringReader(data));
//			XmlElement root = xml.DocumentElement;
//			foreach (XmlNode node in root.ChildNodes)
//			{	
//				if (node.Attributes["type"].Value.ToString().ToLower() == "string")
//				{
//					this._data.Add(node.Name, node.InnerText);									
//				}
//				else if (node.Attributes["type"].Value.ToString().ToLower() == "list")
//				{
//					List<Hashtable> itemlist = new List<Hashtable>();
//					foreach (XmlNode a in node.ChildNodes)
//					{
//						Hashtable item = new Hashtable();
//						
//						foreach (XmlNode b in a.ChildNodes)
//						{
//							item.Add(b.Name, b.InnerText);
//						}						
//						
//						itemlist.Add(item);
//					}
//					this._data.Add(node.Name, itemlist);
//				}				
//			}
//
//		}

		/// <summary>
		///    Gets value of variable recieved by he request.
		///    by the request.
		/// </summary>
		/// <value>
		///    A <see cref="object" /> contains value of variable.
		/// </value>		
//		public object Data(string variablename)
//		{
//			// Definitions
//			object result = null;
//			
//			try
//			{
//				result = this._data[variablename].ToString();
//			}
//			catch {}
//			
//			// Finish
//			return result;			
//		}
#endregion
