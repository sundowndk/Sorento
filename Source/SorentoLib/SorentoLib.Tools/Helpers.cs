// 
// Helpers.cs
//  
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2010 Rasmus Pedersen
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
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace SorentoLib.Tools
{
	public class Helpers
	{
		public static T[] EnumToArray<T> ()
		{
			Array values = Enum.GetValues (typeof(T));
			T[] result = (T[])values;
			return result;
		}

		public static List<T> EnumToList<T> ()
		{
			T[] array = EnumToArray<T> ();
			List<T> result = new List<T> (array);
			return result;
		}


		public static void SendMail (string From, string To, string Subject, string Body)
		{
			SorentoLib.Tools.Helpers.SendMail (From, To, Subject, Body, false);
		}

		public static void SendMail (string From, string To, string Subject, string Body, bool BodyIsHtml)
		{
			System.Web.Mail.MailMessage message = new System.Web.Mail.MailMessage ();
			message.From = From;
			message.To = To;
			message.Subject = Subject;
			message.Body = Body;
			message.BodyEncoding = Encoding.GetEncoding (SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.smtp_encoding));
			
			if (BodyIsHtml)
			{
				message.BodyFormat = System.Web.Mail.MailFormat.Html;
			}
			else
			{
				message.BodyFormat = System.Web.Mail.MailFormat.Text;
			}
			
			System.Web.Mail.SmtpMail.SmtpServer = SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.smtp_server);
			System.Web.Mail.SmtpMail.Send (message);
		}

		public static Encoding GetFileEncoding (string srcFile)
		{
			// *** Use Default of Encoding.Default (Ansi CodePage)
			Encoding enc = Encoding.Default;
			
			// *** Detect byte order mark if any - otherwise assume default
			byte[] buffer = new byte[5];
			FileStream file = new FileStream (srcFile, FileMode.Open);
			file.Read (buffer, 0, 5);
			file.Close ();
			
			if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
				enc = Encoding.UTF8;
			else if (buffer[0] == 0xfe && buffer[1] == 0xff)
				enc = Encoding.Unicode;
			else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
				enc = Encoding.UTF32;
			else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
				enc = Encoding.UTF7;
			return enc;
		}

		public static Hashtable FileToItem (string Path)
		{
			return FileToItem (Path, Encoding.GetEncoding (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_encoding)));
		}

		public static Hashtable FileToItem (string Path, Encoding Encoding2)
		{
			string xml = string.Empty;
//			foreach (string line in Toolbox.IO.ReadTextFile (Path, Encoding.UTF8))
			foreach (string line in SNDK.IO.ReadTextFile (Path, Encoding2))
			{
				xml += line +"\n";
			}

			return SorentoLib.Tools.Helpers.XMLToItem (xml);
		}

		public static Hashtable XMLToItem (string XML)
		{
			Hashtable Item = new Hashtable ();

			XmlDocument xml = new XmlDocument();
			xml.Load (new StringReader (XML));
			XmlElement root = xml.DocumentElement;

			XMLToItemParser (root.ChildNodes, Item);

			return Item;
		}


		public static void XMLToItemParser (XmlNodeList Nodes, Hashtable Item)
		{
			foreach (XmlNode node in Nodes)
			{
				switch (node.Attributes["type"].Value.ToString().ToLower())
				{
					case "string":
					{
						Item.Add(node.Name, node.InnerText);
						break;
					}

//					case "boolean":
//					{
//						break;
//					}

					case "hashtable":
					{
						Hashtable hashtable = new Hashtable ();
						XMLToItemParser (node.ChildNodes, hashtable);
						Item.Add (node.Name, hashtable);

						break;
					}

					case "list":
					{
						List<Hashtable> list = new List<Hashtable>();
						foreach (XmlNode node2 in node.ChildNodes)
						{
							Hashtable hashtable = new Hashtable ();
							XMLToItemParser (node2.ChildNodes, hashtable);

							list.Add (hashtable);
						}
						Item.Add (node.Name, list);

						break;
					}
				}
			}
		}

		public static void ItemToFile (Hashtable Item, string Path)
		{
			ItemToFile (Item, Path, Encoding.GetEncoding (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_encoding)));
		}

		public static void ItemToFile (Hashtable Item, string Path, Encoding Encoding2)
		{
//			Toolbox.IO.WriteTextFile (Path, SorentoLib.Tools.Helpers.ItemToXML (Item), Encoding.UTF8);
			SNDK.IO.WriteTextFile (Path, SorentoLib.Tools.Helpers.ItemToXML (Item), Encoding2);
		}

		public static string ItemToXML (Hashtable Item)
		{
			string result = string.Empty;

			XmlDocument xmldocument = new XmlDocument ();

			XmlElement root = xmldocument.CreateElement ("", "variables", "");
			xmldocument.AppendChild (root);

			ItemToXMLParser (xmldocument, root, Item);

			result = xmldocument.OuterXml;

			return result;
		}

		private static void ItemToXMLParser (XmlDocument XmlDocument, XmlElement ParentElement, Hashtable Data)
		{
			foreach (string key in Data.Keys)
			{
//				Console.WriteLine (key +" "+ Data[key]);

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

						ItemToXMLParser (XmlDocument, element, (Hashtable)Data[key]);

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

							ItemToXMLParser (XmlDocument, element2, data);

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
	}
}
