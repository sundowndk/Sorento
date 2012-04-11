// 
// MediaTransformation.cs
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
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using SNDK;

namespace SorentoLib
{
	[Serializable]
	public class MediaTransformation
	{
		#region Public Static Fields
		public static string DatastoreAisle = "sorento_transformations";
		#endregion

		#region Private Fields
		private Guid _id;
		private List<string> _mimetypes;
		private string _title;
		private string _script;
		#endregion

		#region Public Fields
		public Guid Id
		{
			get
			{
				return this._id;
			}
		}

		public List<string> Mimetypes
		{
			get
			{
				return this._mimetypes;
			}
		}

		public string Title
		{
			get
			{
				return this._title;
			}

			set
			{
				this._title = value;
			}
		}

		public string Script
		{
			get
			{
				return this._script;
			}

			set
			{
				this._script = value;
			}
		}
		#endregion

		#region Constructors
		public MediaTransformation ()
		{
			this._id = Guid.NewGuid ();
			this._mimetypes = new List<string> ();
			this._script = string.Empty;
		}
		#endregion

		#region Public Methods
		public void Transform (Guid MediaId)
		{
			Transform (Media.Load (MediaId));
		}

		public void Transform (Media Media)
		{
			SorentoLib.Services.Scripting.Parse (this._script, Media.DataPath, Media.DataPath);
		}

		public void Save ()
		{
			try
			{
				Hashtable item = new Hashtable ();

				item.Add ("id", this._id);
				item.Add ("mimetypes", this._mimetypes);
				item.Add ("title", this._title);
				item.Add ("script", this._script);

				Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Convert.ToXmlDocument (item, this.GetType ().FullName.ToLower ()));
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.MEDIATRANSFORMATION", exception.Message));

				// EXCEPTION: Exception.MediaTransformationSave
				throw new Exception (string.Format (Strings.Exception.MediaTransformationSave, this._id));
			}
		}

		public XmlDocument ToXmlDocument ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("mimetypes", this._mimetypes);
			result.Add ("title", this._title);
			result.Add ("script", this._script);

			return SNDK.Convert.ToXmlDocument (result, this.GetType ().FullName.ToLower ());
		}
		#endregion

		#region Public Static Methods
		public static MediaTransformation Load (Guid Id)
		{
			MediaTransformation result;

			try
			{
				Hashtable item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (Services.Datastore.Get<XmlDocument> (DatastoreAisle, Id.ToString ()).SelectSingleNode ("(//sorentolib.mediatransformation)[1]")));
				result = new MediaTransformation ();

				result._id = new Guid ((string)item["id"]);

				if (item.ContainsKey ("title"))
				{
					result._title = (string)item["title"];
				}

				if (item.ContainsKey ("mimetypes"))
				{
					foreach (XmlDocument mimetype in (List<XmlDocument>)item["mimetypes"])
					{
						result._mimetypes.Add ((string)((Hashtable)SNDK.Convert.FromXmlDocument (mimetype))["value"]);
					}
				}

				if (item.ContainsKey ("script"))
				{
					result._script = (string)item["script"];
				}
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.MEDIATRANSFORMATION", exception.Message));

				// EXCEPTION: Excpetion.MediaTransformationLoad
				throw new Exception (string.Format (Strings.Exception.MediaTransformationLoad, Id));
			}

			return result;
		}

		public static void Delete (MediaTransformation MediaTransformation)
		{
			Delete (MediaTransformation.Id);
		}

		public static void Delete (Guid Id)
		{
			try
			{
				Services.Datastore.Delete (DatastoreAisle, Id.ToString ());
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.MEDIATRANSFORMATION", exception.Message));

				// EXCEPTION: Exception.MediaTransformationDelete
				throw new Exception (string.Format (Strings.Exception.MediaTransformationDelete, Id));
			}
		}

		public static List<MediaTransformation> List ()
		{
			List<MediaTransformation> result = new List<MediaTransformation> ();

			foreach (string shelf in Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				result.Add (Load (new Guid (shelf)));
			}

			return result;
		}

		public static void Transform (Media Media, string ScriptPath)
		{
			string xml = string.Empty;
			foreach (string line in SNDK.IO.ReadTextFile (ScriptPath, Encoding.UTF8))
			{
				xml += line;
			}

			MediaTransformation transformation = new MediaTransformation ();
			transformation.Script = xml;
			transformation.Transform (Media);
		}

		public static void Transform (string Path, string ScriptPath)
		{
			string xml = string.Empty;
			foreach (string line in SNDK.IO.ReadTextFile (ScriptPath, Encoding.UTF8))
			{
				xml += line;
			}

			SorentoLib.Services.Scripting.Parse (xml, Path, Path);
		}

		public static MediaTransformation FromXmlDocument (XmlDocument xmlDocument)
		{
			Hashtable item;
			MediaTransformation result;

			try
			{
				item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (xmlDocument.SelectSingleNode ("(//sorentolib.mediatransformation)[1]")));
			}
			catch
			{
				item = (Hashtable)SNDK.Convert.FromXmlDocument (xmlDocument);
			}

			if (item.ContainsKey ("id"))
			{
				try
				{
					result = Load (new Guid ((string)item["id"]));
				}
				catch
				{
					result = new MediaTransformation ();
					result._id = new Guid ((string)item["id"]);
				}
			}
			else
			{
				throw new Exception (Strings.Exception.MediaTransformationFromXMLDocument);
			}

			if (item.ContainsKey ("title"))
			{
				result._title = (string)item["title"];
			}

			if (item.ContainsKey ("mimetypes"))
			{
				result._mimetypes.Clear ();
				foreach (XmlDocument mimetype in (List<XmlDocument>)item["mimetypes"])
				{
					result._mimetypes.Add ((string)((Hashtable)SNDK.Convert.FromXmlDocument (mimetype))["value"]);
				}
			}

			if (item.ContainsKey ("script"))
			{
				result._script = (string)item["script"];
			}

			return result;
		}
		#endregion

		#region Internal Static Methods
		internal static void ServicesSnapshotPurge ()
		{
			foreach (MediaTransformation mediatransformation in MediaTransformation.List ())
			{
				Services.Datastore.Delete (DatastoreAisle, mediatransformation.Id.ToString ());
			}
		}
		#endregion
	}
}

