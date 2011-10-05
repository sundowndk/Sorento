//
// AjaxRespons.cs: Parses Ajax respons data.
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

#region Includes
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace SorentoLib.Ajax
{
	/// <summary>
	///    This class helps build Ajax response data quickly.	
	/// </summary>
	public class Respons
	{
		#region Private Fields
		/// <summary>
		///    Contains a list represention of the responsdata.
		/// </summary>		
		private Hashtable _data = new Hashtable();
		#endregion

		#region Constructors
		public Respons()
		{
		}
		#endregion
	
		#region Public Properties		
		/// <summary>
		///    Gets an hashtable representation of the data to be send as a
		///    response.
		/// </summary>
		/// <value>
		///    A <see cref="Hashtable" /> contains a list of variables.
		/// </value>
		public Hashtable Data
		{			
			get { return this._data; }

			set { this._data = value; }
		}			
		#endregion		

		#region Public Methods

		public string WriteResponse ()
		{
			string result = string.Empty;

			XmlDocument xmldocument = new XmlDocument ();

			XmlElement root = xmldocument.CreateElement ("", "variables", "");
			xmldocument.AppendChild (root);

			Test (xmldocument, root, this._data);

			result = xmldocument.OuterXml;

			return result;
		}

		private void Test (XmlDocument XmlDocument, XmlElement ParentElement, Hashtable Data)
		{
			foreach (string key in Data.Keys)
			{
				switch (Data[key].GetType ().Name.ToLower ())
				{
					case "string":
					{
						XmlElement element = XmlDocument.CreateElement ("", key, "");
						XmlAttribute type = XmlDocument.CreateAttribute ("type");
						type.Value = "string";
						element.Attributes.Append (type);

						element.AppendChild (XmlDocument.CreateCDataSection ((string)Data[key]));

						ParentElement.AppendChild (element);

						break;
					}

//					case "boolean":
//					{
//						XmlElement element = XmlDocument.CreateElement ("", key, "");
//						XmlAttribute type = XmlDocument.CreateAttribute ("type");
//						type.Value = "boolean";
//						element.Attributes.Append (type);
//
//						element.AppendChild (XmlDocument.CreateCDataSection (Data[key].ToString ().ToLower ()));
//
//						ParentElement.AppendChild (element);
//
//						break;
//					}

					case "hashtable":
					{
						XmlElement element = XmlDocument.CreateElement ("", key, "");
						XmlAttribute type = XmlDocument.CreateAttribute ("type");
						type.Value = "hashtable";
						element.Attributes.Append (type);

						Test (XmlDocument, element, (Hashtable)Data[key]);

						ParentElement.AppendChild (element);

						break;
					}

					case "list`1":
					{
						XmlElement element = XmlDocument.CreateElement ("", key, "");
						XmlAttribute type = XmlDocument.CreateAttribute ("type");
						type.Value = "list";
						element.Attributes.Append (type);

						foreach (Hashtable data in ((List<Hashtable>)Data[key]))
						{
							XmlElement element2 = XmlDocument.CreateElement("", "item", "");

							Test (XmlDocument, element2, data);

							element.AppendChild (element2);
						}

						ParentElement.AppendChild (element);

						break;
					}

					default:
					{
						XmlElement element = XmlDocument.CreateElement ("", key, "");
						XmlAttribute type = XmlDocument.CreateAttribute ("type");
						type.Value = "string";
						element.Attributes.Append (type);

						try
						{
							element.AppendChild (XmlDocument.CreateCDataSection (Data[key].ToString().ToLower ()));
						}
						catch
						{
							element.AppendChild (XmlDocument.CreateCDataSection (string.Empty));
						}

						ParentElement.AppendChild (element);

						break;
					}
				}
			}
		}

		public string WriteResponseold ()
		{
			// Definitions.
			string result = string.Empty;
			
			// Create new XML document.
			XmlDocument xml = new XmlDocument ();
			
//			XmlNode xmldeclaration = xml.CreateNode(XmlNodeType.XmlDeclaration, "", "");
			// TODO: Dont know if declaration is really needed. It seams to mess up if httpheader is send on respons.
			//xml.AppendChild(xmldeclaration);
			
			XmlElement root = xml.CreateElement ("", "variables", "");
			xml.AppendChild (root);
						
			foreach (string variablename in this._data.Keys) 
			{								
				if (!_data[variablename].GetType().Name.Contains("List"))
				{
					XmlElement variable = xml.CreateElement("", variablename, "");				
					
					XmlAttribute type = xml.CreateAttribute("type");
					type.Value = "string";
					variable.Attributes.Append(type);

					try
					{
						// All variables that are NOT strings will be made cast to string and than lowercased.
						if (this._data[variablename].GetType () == typeof (string))
						{
							variable.AppendChild(xml.CreateCDataSection(this._data[variablename].ToString()));
						}
						else
						{
							variable.AppendChild(xml.CreateCDataSection(this._data[variablename].ToString().ToLower ()));
						}



					}
					catch
					{
						variable.AppendChild(xml.CreateCDataSection(string.Empty));
					}
					root.AppendChild(variable);
				}				
				
				if (_data[variablename].GetType().Name.Contains("List"))
				{
					XmlElement variable = xml.CreateElement("", variablename, "");				
					
					XmlAttribute type = xml.CreateAttribute("type");
					type.Value = "list";
					variable.Attributes.Append(type);						
					
					foreach (Hashtable itemhash in ((List<Hashtable>)_data[variablename])) 
					{
						XmlElement item = xml.CreateElement("", "item", "");
						
						foreach (string itemvariablename in itemhash.Keys) 
						{
							XmlElement itemvariable = xml.CreateElement("", itemvariablename, "");					
							//XmlText value = xml.CreateTextNode(itemhash[itemvariablename].ToString());
							try
							{
								// All variables that are NOT strings will be made cast to string and than lowercased.
								if (itemhash[itemvariablename].GetType () == typeof (string))
								{
									itemvariable.AppendChild(xml.CreateCDataSection(itemhash[itemvariablename].ToString ()));
								}
								else
								{
									itemvariable.AppendChild(xml.CreateCDataSection(itemhash[itemvariablename].ToString ().ToLower ()));
								}
							}
							catch
							{
								itemvariable.AppendChild(xml.CreateCDataSection(string.Empty));
							}
							item.AppendChild(itemvariable);
						}
						
						variable.AppendChild(item);
					}
										
					root.AppendChild(variable);
				}			
			}

			result = xml.OuterXml;
			
			// Finish.
			return result;				
		}		
		#endregion		
	}
}
