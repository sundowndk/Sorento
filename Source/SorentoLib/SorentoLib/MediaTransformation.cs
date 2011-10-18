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
			SorentoLib.Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Serializer.SerializeObjectToString (this));
		}

		public void ToAjaxRespons (SorentoLib.Ajax.Respons Respons)
		{
//			Respons.Data = this.ToAjaxItem ();
		}

		public Hashtable ToAjaxItem ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("title", this._title);
			result.Add ("script", this._script);

			List<Hashtable> mimetypes = new List<Hashtable> ();
			foreach (string mimetype in this._mimetypes)
			{
				Hashtable item = new Hashtable ();
				item.Add ("mimetype", mimetype);
				mimetypes.Add (item);
			}

			result.Add ("mimetypes", mimetypes);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static MediaTransformation Load (Guid Id)
		{
			return SNDK.Serializer.DeSerializeObjectFromString<MediaTransformation> (SorentoLib.Services.Datastore.Get<string> (DatastoreAisle, Id.ToString ()));
		}

		public static void Delete (MediaTransformation MediaTransformation)
		{
			Delete (MediaTransformation.Id);
		}

		public static void Delete (Guid Id)
		{
			SorentoLib.Services.Datastore.Delete (DatastoreAisle, Id.ToString ());
		}

		public static List<MediaTransformation> List ()
		{
			List<MediaTransformation> result = new List<MediaTransformation> ();

			foreach (string id in SorentoLib.Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				result.Add (MediaTransformation.Load (new Guid (id)));
			}

			return result;
		}

		public static MediaTransformation FromAjaxRequest (SorentoLib.Ajax.Request Request)
		{
			return FromAjaxItem (Request.Data);
		}

		public static MediaTransformation FromAjaxItem (Hashtable Item)
		{

			MediaTransformation result = null;

			Guid id = Guid.Empty;

			try
			{
				id = new Guid ((string)Item["id"]);
			}
			catch {}

			if (id != Guid.Empty)
			{
				try
				{
					result = MediaTransformation.Load (id);
				}
				catch
				{
					result = new MediaTransformation ();
					result._id = id;
				}
			}
			else
			{
				result = new MediaTransformation ();
			}

			if (Item.ContainsKey ("title"))
			{
				result._title = (string)Item["title"];
			}

			if (Item.ContainsKey ("script"))
			{
				result._script = (string)Item["script"];
			}

			if (Item.ContainsKey ("mimetypes"))
			{
				result._mimetypes.Clear ();
				foreach (Hashtable item in (List<Hashtable>)Item["mimetypes"])
				{
					result._mimetypes.Add ((string)item["mimetype"]);
				}
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

