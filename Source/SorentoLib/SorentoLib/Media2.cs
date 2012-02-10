// 
// Media2.cs
//  
// Author:
//       rvp <${AuthorEmail}>
// 
// Copyright (c) 2012 rvp
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
using System.Collections;
using System.Collections.Generic;

using Mono.Unix;

using SNDK;

namespace SorentoLib
{
	public class Media2
	{
		#region Public Static Fields
		public static string DatastoreAisle = "media";
		#endregion

		#region Private Fields
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;
		private List<Usergroup> _usergroups;
		private Enums.MediaType _type;
		private string _path;
		private string _mimetype;
		private long _size;
		private string _description;
		private string _copyright;

		#endregion


		#region Temp Fields
		private string _temppath;
		private Enums.MediaType _temptype;
		#endregion

		#region Public Fields
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

		public Enums.MediaType Type
		{
			get
			{
				return this._type;
			}

			set
			{
				this._temptype = value;
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}

			set
			{
				this._temppath = FixPath (value);
			}
		}


		public string DataPath
		{
			get
			{
				switch (this._type)
				{
					case Enums.MediaType.Temporary:
					{
						return Services.Config.Get<string> (Enums.ConfigKey.path_temp) + this._id;
					}

					case Enums.MediaType.Public:
					{
						return Services.Config.Get<string> (Enums.ConfigKey.path_media) + this._id;
					}

					default:
					{
						return string.Empty;
					}
				}
			}
		}


		public string Mimetype
		{
			get
			{
				return this._mimetype;
			}

			set
			{
				this._mimetype = value;
			}
		}

		public long Size
		{
			get
			{
				return this._size;
			}
		}

		public string Description
		{
			get
			{
				return this._description;
			}

			set
			{
				this._description = value;
			}
		}

		public string Copyright
		{
			get
			{
				return this._copyright;
			}

			set
			{
				this._copyright = value;
			}
		}
		#endregion

		#region Constructor
		public Media2 (string Path, byte [] Data)
		{
			Initialize (Path);

			FileStream filestream = File.Create (Services.Config.Get<string> (Enums.ConfigKey.path_temp) + this._id);
			BinaryWriter binarywriter = new BinaryWriter (filestream);
			binarywriter.Write (Data);
			binarywriter.Close ();
			filestream.Close ();

			Update ();
		}

		public Media2 (string Path, string SourcePath, bool MoveFile)
		{
			Initialize (Path);

			FileInfo fileinfo = new FileInfo (SourcePath);
			if (MoveFile)
			{
				fileinfo.MoveTo (Services.Config.Get<string> (Enums.ConfigKey.path_temp) + this._id);
			}
			else
			{
				fileinfo.CopyTo (Services.Config.Get<string> (Enums.ConfigKey.path_temp) + this._id);
			}

			Update ();
		}

		public Media2 (string Path, string Url)
		{
			Initialize (Path);

			SNDK.IO.DownloadToFile (Url, Services.Config.Get<string> (Enums.ConfigKey.path_temp) + this._id);

			Update ();
		}

		private void Initialize (string Path)
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._usergroups = new List<Usergroup> ();
			this._path = Path;
			this._mimetype = string.Empty;
			this._size = 0;
			this._description = string.Empty;
			this._copyright = string.Empty;
			this._type = SorentoLib.Enums.MediaType.Temporary;

			this._temppath = Path;
			this._temptype = SorentoLib.Enums.MediaType.Temporary;
		}
		#endregion

		#region Private Methods
		public void Update ()
		{
			// Check if data still exists.
			if (!File.Exists (this.DataPath))
			{
				// EXCEPTION: Exception.MediaSaveData
				throw new Exception (string.Format (Strings.Exception.MediaSaveData, this._id));
			}

			if (this._type != this._temptype)
			{
				string source = string.Empty;
				string destination = string.Empty;

				switch (this._type)
				{
					case Enums.MediaType.Temporary:
					{
						source = Services.Config.Get<string> (Enums.ConfigKey.path_temp);
						break;
					}

					case Enums.MediaType.Public:
					{
						source = Services.Config.Get<string> (Enums.ConfigKey.path_media);
						break;
					}
				}

				switch (this._temptype)
				{
					case Enums.MediaType.Temporary:
					{
						destination = Services.Config.Get<string> (Enums.ConfigKey.path_temp);
						break;
					}

					case Enums.MediaType.Public:
					{
						destination = Services.Config.Get<string> (Enums.ConfigKey.path_media);
						break;
					}
				}


				if (source != destination)
				{
					FileInfo file = new FileInfo (source + this._id);
					file.MoveTo (destination + this._id);
				}
			}

			if ((this._path != this._temppath) || (this._type != this._temptype))
			{
				// Remove old symlink if needed.
				if (this._type == Enums.MediaType.Public)
				{
					File.Delete (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + this._path);
				}

				// Create new symlink if needed.
				if (this._temptype == Enums.MediaType.Public)
				{
					// Check if path exists, if not create it.
					if (!Directory.Exists (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + System.IO.Path.GetDirectoryName (this._temppath)))
					{
						Directory.CreateDirectory (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + System.IO.Path.GetDirectoryName (this._temppath));
					}

					// Create a new symlink.
					UnixFileInfo unixfileinfo = new UnixFileInfo (this.DataPath);
					Console.WriteLine (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + this._temppath);
					unixfileinfo.CreateSymbolicLink (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + this._temppath);
				}
			}

			this._path = this._temppath;
			this._type = this._temptype;

			string path = string.Empty;

			switch (this._type)
			{
				case Enums.MediaType.Temporary:
				{
					path = Services.Config.Get<string> (Enums.ConfigKey.path_temp);
					break;
				}

				case Enums.MediaType.Public:
				{
					path = Services.Config.Get<string> (Enums.ConfigKey.path_media);
					break;
				}
			}

			// Get filesize
			FileInfo fileinfo = new FileInfo (path + this._id);
			this._size = fileinfo.Length;

			// Get mimetype
			this._mimetype = SNDK.IO.GetMimeType (path + this._id);
		}

		private static string FixPath (string Path)
		{
			string result = Path;

			string path = System.IO.Path.GetDirectoryName (Path) +"/";
			string filename = System.IO.Path.GetFileNameWithoutExtension (Path);
			string extension = System.IO.Path.GetExtension (Path);

			int increment = 1;
			while (Services.Datastore.ShelfExists (DatastoreAisle, new Services.Datastore.MetaSearch ("path", Enums.DatastoreMetaSearchCondition.Equal, result)))
			{
				result = path + filename +"("+ increment +")" + extension;
				increment++;
			}


//			foreach (string shelf in Services.Datastore.ListOfShelfs (DatastoreAisle, new Services.Datastore.MetaSearch ("username", Enums.DatastoreMetaSearchCondition.Equal, Path)))
//			{
//
//			}

//			List<string> files = Services.Datastore.ListOfShelfs (DatastoreAisle, new Services.Datastore.MetaSearch ("username", Enums.DatastoreMetaSearchCondition.Equal, Path));

//			int increment = 1;
//			while (files.Contains (result))
//			{
//				result = path + filename +"("+ increment +")" + extension;
//				increment++;
//			}

//			if (Services.Datastore.FindShelf (DatastoreAisle, new Services.Datastore.MetaSearch ("username", Enums.DatastoreMetaSearchCondition.Equal, username), new Services.Datastore.MetaSearch ("id", Enums.DatastoreMetaSearchCondition.NotEqual, filterOutUserId)) != string.Empty)
//			{
//				result = true;
//			}

//			string path = System.IO.Path.GetDirectoryName (Path) +"/";
//			string filename = System.IO.Path.GetFileNameWithoutExtension (Path);
//			string extension = System.IO.Path.GetExtension (Path);
//			List<string> files = new List<string> ();
//
//			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
//			qb.Table (DatabaseTableName);
//			qb.Columns ("path");
//			qb.AddWhere ("path", "like", "%"+ path +"%");
//
//			Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
//			if (query.Success)
//			{
//				while (query.NextRow ())
//				{
//					files.Add (query.GetString (qb.ColumnPos ("path")));
//				}
//			}
//
//			int increment = 1;
//			while (files.Contains (result))
//			{
//				result = path + filename +"("+ increment +")" + extension;
//				increment++;
//			}

			return result;
		}

		#endregion

		#region Public Methods
		public void Save ()
		{
			try
			{
				Update ();

				this._updatetimestamp = Date.CurrentDateTimeToTimestamp ();

				Hashtable item = new Hashtable ();

				item.Add ("id", this._id);
				item.Add ("createtimestamp", this._createtimestamp);
				item.Add ("updatetimestamp", this._updatetimestamp);
				item.Add ("type", this._type);
				item.Add ("mimetype", this._mimetype);
				item.Add ("size", this._size);
				item.Add ("description", this._description);
				item.Add ("copyright", this._copyright);

				Services.Datastore.Meta meta = new Services.Datastore.Meta ();
				meta.Add ("path", this._path);

				Services.Datastore.Set (DatastoreAisle, this._id.ToString (), SNDK.Convert.ToXmlDocument (item, this.GetType ().FullName.ToLower ()), meta);
			}
			catch (Exception exception)
			{
				Console.WriteLine (exception);
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.MEDIA", exception.Message));

				// EXCEPTION: Exception.MediaSave
				throw new Exception (string.Format (Strings.Exception.MediaSave, this._id));
			}
		}
		#endregion

		#region Public Static Methods
		#endregion

		#region Internal Static Methods
		#endregion
	}
}

