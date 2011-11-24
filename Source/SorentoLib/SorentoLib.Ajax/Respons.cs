//
// AjaxRespons.cs
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
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using SNDK;

namespace SorentoLib.Ajax
{
	public class Respons
	{
		#region REMOVE
		public Hashtable Data
		{
			get { return this._data2; }

			set { this._data2 = value; }
		}
		private Hashtable _data2 = new Hashtable ();

//		public string OuterXml
//		{
//			get
//			{
//				// Create new XmlDocument.
//				XmlDocument xmldocument = new XmlDocument ();
//
//				// Create new element.
//				XmlNode root = xmldocument.CreateElement ("", "ajax", "");
//
//				// Append element to XmlDocument.
//				xmldocument.AppendChild (root);
//
//				// Parse all data added the respons.
//				foreach (object data in this._data)
//				{
//					Parse (data, xmldocument);
//				}
//
//				return xmldocument.OuterXml;
//			}
//		}
		#endregion

		#region Private Properties
		private List<object> _data;
		#endregion

		#region Public Properties
		public XmlDocument XmlDocument
		{
			get
			{
				// Create new XmlDocument.
				XmlDocument result = new XmlDocument ();

				// Create new element.
				XmlNode root = result.CreateElement ("", "ajax", "");

				// Append element to XmlDocument.
				result.AppendChild (root);

				// Parse all data added the respons.
				foreach (object data in this._data)
				{
					foreach (XmlNode node in SNDK.Convert.ToXmlDocument (data).DocumentElement)
					{
						root.AppendChild (result.ImportNode (node, true));
					}
				}

//				foreach (object data in this._data)
//				{
//					Parse (data, result);
//				}

				return result;
			}
		}
		#endregion

		#region Constructors
		public Respons()
		{
			this._data = new List<object> ();
		}
		#endregion

		#region Public Methods
		public void Add (string Key, object Value)
		{
			Hashtable hashtable = new Hashtable ();
			hashtable.Add (Key, Value);
			this._data.Add (hashtable);
		}

		public void Add (object Value)
		{
			this._data.Add (Value);
		}
		#endregion

		#region Private Methods
//		private void Parse (object Object, XmlDocument XmlDocument)
//		{
//			switch (Object.GetType ().Name.ToLower ())
//			{
//				#region Hashtable
//				case "hashtable":
//				{
//					// Convert Hashtable to XmlDocument, and import nodes.
////					foreach (XmlNode node in SNDK.Convert.ToXmlDocument ((Hashtable)System.Convert.ChangeType (Object, typeof (Hashtable))).DocumentElement)
//					foreach (XmlNode node in SNDK.Convert.ToXmlDocument (Object).DocumentElement)
//					{
//						XmlDocument.DocumentElement.AppendChild (XmlDocument.ImportNode (node, true));
//					}
//					break;
//				}
//				#endregion
//
//				#region List<T>
//				case "list`1":
//				{
//					// Create new xml element.
//					XmlElement elements = XmlDocument.CreateElement ("", Object.GetType ().GetGenericArguments ()[0].ToString ().ToLower () +"s", "");
//
//					// Set xml element type to 'list'.
//					XmlAttribute type = XmlDocument.CreateAttribute ("type");
//					type.Value = "list";
//					elements.Attributes.Append (type);
//
//					// Get IEnumerator of Object.
//					System.Collections.IEnumerator enumerator = (System.Collections.IEnumerator)Object.GetType ().GetMethod("GetEnumerator").Invoke (Object, null);
//
//					// Enumerate objects inside Object.
//					while (enumerator.MoveNext ())
//					{
//						// Create new xml element.
//						XmlElement element = XmlDocument.CreateElement ("", Object.GetType ().GetGenericArguments ()[0].ToString ().ToLower (), "");
//
//						// DEBUG: TryCatch, Console.WriteLine
//						try
//						{
//							if (enumerator.Current.GetType ().GetMethod ("ToXmlDocument") != null)
//							{
//								// Call ToXMLDocument on all objects inside Object, and import nodes into XmlDocument.
//								foreach (XmlNode node in ((XmlDocument)enumerator.Current.GetType ().GetMethod ("ToXmlDocument").Invoke (enumerator.Current, null)).DocumentElement.ChildNodes)
//								{
//									element.AppendChild (XmlDocument.ImportNode (node, true));
//								}
//							}
//							else
//							{
//								Console.WriteLine (enumerator.Current.GetType ().ToString ());
////								SNDK.Convert.ToXmlDocument (enumerator.Current);
//								foreach (XmlNode node in SNDK.Convert.ToXmlDocument (enumerator.Current).DocumentElement)
//								{
////									Console.WriteLine (enumerator.Current.GetType ().ToString ());
//									element.AppendChild (XmlDocument.ImportNode (node, true));
//								}
//							}
//						}
//						catch (Exception e)
//						{
//							Console.WriteLine (e);
//						}
//
//						// Append element to elements
//						elements.AppendChild (element);
//					}
//
//					// Append elements to XmlDocument.
//					XmlDocument.DocumentElement.AppendChild (elements);
//					break;
//				}
//				#endregion
//
//				#region Default
//				default:
//				{
//					// Create new xml element.
//					XmlElement element = XmlDocument.CreateElement ("", Object.GetType ().FullName.ToLower (), "");
//
//					// Set xml element type to 'object'.
//					XmlAttribute type = XmlDocument.CreateAttribute ("type");
//					type.Value = "object";
//					element.Attributes.Append (type);
//
//					if (Object.GetType ().GetMethod ("ToXmlDocument") != null)
//					{
//						// Call ToXMLDocument on all objects inside Object, and import nodes into XmlDocument.
//						foreach (XmlNode node in ((XmlDocument)Object.GetType ().GetMethod ("ToXmlDocument").Invoke (Object, null)).DocumentElement.ChildNodes)
//						{
//							element.AppendChild (XmlDocument.ImportNode (node, true));
//						}
//					}
//					else
//					{
//						// TODO: Objects without the ToXmlDocument method will be skipped.
//						break;
//					}
//
//					// Append element to XmlDocument.
//					XmlDocument.DocumentElement.AppendChild (element);
//					break;
//				}
//				#endregion
//			}
//		}
		#endregion
	}
}

#region OLD
//		public string WriteResponse ()
//		{
//			string result = string.Empty;

//			XmlDocument xmldocument = new XmlDocument ();
//
//			XmlElement root = xmldocument.CreateElement ("", "variables", "");
//			xmldocument.AppendChild (root);
//
//			Test (xmldocument, root, this._data);

//			result = xmldocument.OuterXml;




//			result = this._xml.OuterXml;
//			result = OuterXml;
//			return result;
//		}

//		private void Test (XmlDocument XmlDocument, XmlElement ParentElement, Hashtable Data)
//		{
//			foreach (string key in Data.Keys)
//			{
//				switch (Data[key].GetType ().Name.ToLower ())
//				{
//					case "string":
//					{
//						XmlElement element = XmlDocument.CreateElement ("", key, "");
//						XmlAttribute type = XmlDocument.CreateAttribute ("type");
//						type.Value = "string";
//						element.Attributes.Append (type);
//
//						element.AppendChild (XmlDocument.CreateCDataSection ((string)Data[key]));
//
//						ParentElement.AppendChild (element);
//
//						break;
//					}
//
////					case "boolean":
////					{
////						XmlElement element = XmlDocument.CreateElement ("", key, "");
////						XmlAttribute type = XmlDocument.CreateAttribute ("type");
////						type.Value = "boolean";
////						element.Attributes.Append (type);
////
////						element.AppendChild (XmlDocument.CreateCDataSection (Data[key].ToString ().ToLower ()));
////
////						ParentElement.AppendChild (element);
////
////						break;
////					}
//
//					case "hashtable":
//					{
//						XmlElement element = XmlDocument.CreateElement ("", key, "");
//						XmlAttribute type = XmlDocument.CreateAttribute ("type");
//						type.Value = "hashtable";
//						element.Attributes.Append (type);
//
//						Test (XmlDocument, element, (Hashtable)Data[key]);
//
//						ParentElement.AppendChild (element);
//
//						break;
//					}
//
//					case "list`1":
//					{
//						XmlElement element = XmlDocument.CreateElement ("", key, "");
//						XmlAttribute type = XmlDocument.CreateAttribute ("type");
//						type.Value = "list";
//						element.Attributes.Append (type);
//
//						foreach (Hashtable data in ((List<Hashtable>)Data[key]))
//						{
//							XmlElement element2 = XmlDocument.CreateElement("", "item", "");
//
//							Test (XmlDocument, element2, data);
//
//							element.AppendChild (element2);
//						}
//
//						ParentElement.AppendChild (element);
//
//						break;
//					}
//
//					default:
//					{
//						XmlElement element = XmlDocument.CreateElement ("", key, "");
//						XmlAttribute type = XmlDocument.CreateAttribute ("type");
//						type.Value = "string";
//						element.Attributes.Append (type);
//
//						try
//						{
//							element.AppendChild (XmlDocument.CreateCDataSection (Data[key].ToString().ToLower ()));
//						}
//						catch
//						{
//							element.AppendChild (XmlDocument.CreateCDataSection (string.Empty));
//						}
//
//						ParentElement.AppendChild (element);
//
//						break;
//					}
//				}
//			}
//		}
//
//		public string WriteResponseold ()
//		{
//			// Definitions.
//			string result = string.Empty;
//			
//			// Create new XML document.
//			XmlDocument xml = new XmlDocument ();
//			
////			XmlNode xmldeclaration = xml.CreateNode(XmlNodeType.XmlDeclaration, "", "");
//			// TODO: Dont know if declaration is really needed. It seams to mess up if httpheader is send on respons.
//			//xml.AppendChild(xmldeclaration);
//			
//			XmlElement root = xml.CreateElement ("", "variables", "");
//			xml.AppendChild (root);
//						
//			foreach (string variablename in this._data.Keys) 
//			{								
//				if (!_data[variablename].GetType().Name.Contains("List"))
//				{
//					XmlElement variable = xml.CreateElement("", variablename, "");				
//					
//					XmlAttribute type = xml.CreateAttribute("type");
//					type.Value = "string";
//					variable.Attributes.Append(type);
//
//					try
//					{
//						// All variables that are NOT strings will be made cast to string and than lowercased.
//						if (this._data[variablename].GetType () == typeof (string))
//						{
//							variable.AppendChild(xml.CreateCDataSection(this._data[variablename].ToString()));
//						}
//						else
//						{
//							variable.AppendChild(xml.CreateCDataSection(this._data[variablename].ToString().ToLower ()));
//						}
//
//
//
//					}
//					catch
//					{
//						variable.AppendChild(xml.CreateCDataSection(string.Empty));
//					}
//					root.AppendChild(variable);
//				}				
//				
//				if (_data[variablename].GetType().Name.Contains("List"))
//				{
//					XmlElement variable = xml.CreateElement("", variablename, "");				
//					
//					XmlAttribute type = xml.CreateAttribute("type");
//					type.Value = "list";
//					variable.Attributes.Append(type);						
//					
//					foreach (Hashtable itemhash in ((List<Hashtable>)_data[variablename])) 
//					{
//						XmlElement item = xml.CreateElement("", "item", "");
//						
//						foreach (string itemvariablename in itemhash.Keys) 
//						{
//							XmlElement itemvariable = xml.CreateElement("", itemvariablename, "");					
//							//XmlText value = xml.CreateTextNode(itemhash[itemvariablename].ToString());
//							try
//							{
//								// All variables that are NOT strings will be made cast to string and than lowercased.
//								if (itemhash[itemvariablename].GetType () == typeof (string))
//								{
//									itemvariable.AppendChild(xml.CreateCDataSection(itemhash[itemvariablename].ToString ()));
//								}
//								else
//								{
//									itemvariable.AppendChild(xml.CreateCDataSection(itemhash[itemvariablename].ToString ().ToLower ()));
//								}
//							}
//							catch
//							{
//								itemvariable.AppendChild(xml.CreateCDataSection(string.Empty));
//							}
//							item.AppendChild(itemvariable);
//						}
//						
//						variable.AppendChild(item);
//					}
//										
//					root.AppendChild(variable);
//				}			
//			}
//
//			result = xml.OuterXml;
//			
//			// Finish.
//			return result;				
//		}
//
//		public void Add2<T> (T Object)
//		{
//			Console.WriteLine (Object.GetType ().Name.ToLower ());
//			switch (Object.GetType ().Name.ToLower ())
//			{
//				#region Hashtable
//				case "hashtable":
//				{
//					// Convert Hashtable to XmlDocument, and import nodes.
//					foreach (XmlNode node in SNDK.Convert.HashtabelToXmlDocument ((Hashtable)System.Convert.ChangeType (Object, typeof (Hashtable))).DocumentElement)
//					{
//						this._test.AppendChild (this._xml.ImportNode (node, true));
//					}
//					break;
//				}
//				#endregion
//
//				#region List<T>
//				case "list`1":
//				{
//					// Create new xml element.
//					XmlElement elements = this._xml.CreateElement ("", Object.GetType ().GetGenericArguments ()[0].ToString ().ToLower () +"s", "");
//
//					// Set xml element type to 'list'.
//					XmlAttribute type = this._xml.CreateAttribute ("type");
//					type.Value = "list";
//					elements.Attributes.Append (type);
//
//					// Get IEnumerator of Object.
//					System.Collections.IEnumerator enumerator = (System.Collections.IEnumerator)typeof (T).GetMethod("GetEnumerator").Invoke (Object, null);
//
//					// Enumerate objects inside Object.
//					while (enumerator.MoveNext ())
//					{
//						// Create new xml element.
//						XmlElement element = this._xml.CreateElement ("", Object.GetType ().GetGenericArguments ()[0].ToString ().ToLower (), "");
//
//						// DEBUG: TryCatch, Console.WriteLine
//						try
//						{
//							if (enumerator.Current.GetType ().GetMethod ("ToXmlDocument") != null)
//							{
//								// Call ToXMLDocument on all objects inside Object, and import nodes into XmlDocument.
//								foreach (XmlNode node in ((XmlDocument)enumerator.Current.GetType ().GetMethod ("ToXmlDocument").Invoke (enumerator.Current, null)).DocumentElement.ChildNodes)
//								{
//									element.AppendChild (this._xml.ImportNode (node, true));
//								}
//							}
//							else
//							{
//								// TODO: Objects without the ToXmlDocument method will be skipped.
//								continue;
//							}
//						}
//						catch (Exception e)
//						{
//							Console.WriteLine (e);
//						}
//
//						// Append element to elements
//						elements.AppendChild (element);
//					}
//
//					// Append elements to XmlDocument.
//					this._test.AppendChild (elements);
//					break;
//				}
//				#endregion
//
//				#region Default
//				default:
//				{
//					// Create new xml element.
//					XmlElement element = this._xml.CreateElement ("", Object.GetType ().FullName.ToLower (), "");
//
//					// Set xml element type to 'object'.
//					XmlAttribute type = this._xml.CreateAttribute ("type");
//					type.Value = "object";
//					element.Attributes.Append (type);
//
//					if (typeof (T).GetMethod ("ToXmlDocument") != null)
//					{
//						// Call ToXMLDocument on all objects inside Object, and import nodes into XmlDocument.
//						foreach (XmlNode node in ((XmlDocument)typeof (T).GetMethod ("ToXmlDocument").Invoke (Object, null)).DocumentElement.ChildNodes)
//						{
//							element.AppendChild (this._xml.ImportNode (node, true));
//						}
//					}
//					else
//					{
//						// TODO: Objects without the ToXmlDocument method will be skipped.
//						break;
//					}
//
//					// Append element to XmlDocument.
//					this._xml.DocumentElement.AppendChild (element);
//					break;
//				}
//				#endregion
//			}
//		}
//
//
//		public void Add4<T> (T Object, XmlDocument XmlDocument)
//		{
//			Console.WriteLine (Object.GetType ().Name.ToLower ());
//			switch (Object.GetType ().Name.ToLower ())
//			{
//				#region Hashtable
//				case "hashtable":
//				{
//					// Convert Hashtable to XmlDocument, and import nodes.
//					foreach (XmlNode node in SNDK.Convert.HashtabelToXmlDocument ((Hashtable)System.Convert.ChangeType (Object, typeof (Hashtable))).DocumentElement)
//					{
//						XmlDocument.AppendChild (XmlDocument.ImportNode (node, true));
//					}
//					break;
//				}
//				#endregion
//
//				#region List<T>
//				case "list`1":
//				{
//					// Create new xml element.
//					XmlElement elements = XmlDocument.CreateElement ("", Object.GetType ().GetGenericArguments ()[0].ToString ().ToLower () +"s", "");
//
//					// Set xml element type to 'list'.
//					XmlAttribute type = XmlDocument.CreateAttribute ("type");
//					type.Value = "list";
//					elements.Attributes.Append (type);
//
//					// Get IEnumerator of Object.
//					System.Collections.IEnumerator enumerator = (System.Collections.IEnumerator)typeof (T).GetMethod("GetEnumerator").Invoke (Object, null);
//
//					// Enumerate objects inside Object.
//					while (enumerator.MoveNext ())
//					{
//						// Create new xml element.
//						XmlElement element = XmlDocument.CreateElement ("", Object.GetType ().GetGenericArguments ()[0].ToString ().ToLower (), "");
//
//						// DEBUG: TryCatch, Console.WriteLine
//						try
//						{
//							if (enumerator.Current.GetType ().GetMethod ("ToXmlDocument") != null)
//							{
//								// Call ToXMLDocument on all objects inside Object, and import nodes into XmlDocument.
//								foreach (XmlNode node in ((XmlDocument)enumerator.Current.GetType ().GetMethod ("ToXmlDocument").Invoke (enumerator.Current, null)).DocumentElement.ChildNodes)
//								{
//									element.AppendChild (XmlDocument.ImportNode (node, true));
//								}
//							}
//							else
//							{
//								// TODO: Objects without the ToXmlDocument method will be skipped.
//								continue;
//							}
//						}
//						catch (Exception e)
//						{
//							Console.WriteLine (e);
//						}
//
//						// Append element to elements
//						elements.AppendChild (element);
//					}
//
//					// Append elements to XmlDocument.
//					XmlDocument.DocumentElement.AppendChild (elements);
//					break;
//				}
//				#endregion
//
//				#region Default
//				default:
//				{
//					// Create new xml element.
//					XmlElement element = XmlDocument.CreateElement ("", Object.GetType ().FullName.ToLower (), "");
//
//					// Set xml element type to 'object'.
//					XmlAttribute type = XmlDocument.CreateAttribute ("type");
//					type.Value = "object";
//					element.Attributes.Append (type);
//
//					if (typeof (T).GetMethod ("ToXmlDocument") != null)
//					{
//						// Call ToXMLDocument on all objects inside Object, and import nodes into XmlDocument.
//						foreach (XmlNode node in ((XmlDocument)typeof (T).GetMethod ("ToXmlDocument").Invoke (Object, null)).DocumentElement.ChildNodes)
//						{
//							element.AppendChild (XmlDocument.ImportNode (node, true));
//						}
//					}
//					else
//					{
//						// TODO: Objects without the ToXmlDocument method will be skipped.
//						break;
//					}
//
//					// Append element to XmlDocument.
//					XmlDocument.DocumentElement.AppendChild (element);
//					break;
//				}
//				#endregion
//			}
//		}
#endregion
