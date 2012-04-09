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
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using Mono.Unix;

using SNDK;

namespace SorentoLib
{
	public class Media
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

					case Enums.MediaType.TemporaryPublic:
					{
						return Services.Config.Get<string> (Enums.ConfigKey.path_temp) + this._id;
					}

					case Enums.MediaType.Public:
					{
						return Services.Config.Get<string> (Enums.ConfigKey.path_media) + this._id;
					}

					case Enums.MediaType.Restricted:
					{
						return Services.Config.Get<string> (Enums.ConfigKey.path_media) + this._id;
					}
				}

				return string.Empty;
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
		public Media (string Path, byte [] Data)
		{
			Initialize (Path);

			FileStream filestream = File.Create (Services.Config.Get<string> (Enums.ConfigKey.path_temp) + this._id);
			BinaryWriter binarywriter = new BinaryWriter (filestream);
			binarywriter.Write (Data);
			binarywriter.Close ();
			filestream.Close ();

			Update ();
		}

		public Media (string Path, string SourcePath, bool MoveFile)
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

		public Media (string Path, string Url)
		{
			Initialize (Path);

			SNDK.IO.DownloadToFile (Url, Services.Config.Get<string> (Enums.ConfigKey.path_temp) + this._id);

			Update ();
		}

		private Media ()
		{
			this._createtimestamp = 0;
			this._updatetimestamp = 0;
			this._path = string.Empty;
			this._mimetype = string.Empty;
			this._size = 0;
			this._description = string.Empty;
			this._copyright = string.Empty;
			this._type = SorentoLib.Enums.MediaType.Temporary;

			this._temppath = string.Empty;
			this._temptype = SorentoLib.Enums.MediaType.Temporary;
		}

		private void Initialize (string Path)
		{
			this._id = Guid.NewGuid ();
			this._createtimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._updatetimestamp = SNDK.Date.CurrentDateTimeToTimestamp ();
			this._usergroups = new List<Usergroup> ();
			this._path = FixPath (Path);
			this._mimetype = string.Empty;
			this._size = 0;
			this._description = string.Empty;
			this._copyright = string.Empty;
			this._type = SorentoLib.Enums.MediaType.Temporary;

			this._temppath = FixPath (Path);
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

					case Enums.MediaType.TemporaryPublic:
					{
						source = Services.Config.Get<string> (Enums.ConfigKey.path_temp);
						break;
					}

					case Enums.MediaType.Public:
					{
						source = Services.Config.Get<string> (Enums.ConfigKey.path_media);
						break;
					}

					case Enums.MediaType.Restricted:
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

					case Enums.MediaType.TemporaryPublic:
					{
						destination = Services.Config.Get<string> (Enums.ConfigKey.path_temp);
						break;
					}

					case Enums.MediaType.Public:
					{
						destination = Services.Config.Get<string> (Enums.ConfigKey.path_media);
						break;
					}

					case Enums.MediaType.Restricted:
					{
						destination = Services.Config.Get<string> (Enums.ConfigKey.path_media);
						break;
					}
				}

				if ((this._path != this._temppath) || (this._type != this._temptype))
				{
					// Remove old symlink if needed.
					if ((this._type == Enums.MediaType.Public) || (this._type == Enums.MediaType.TemporaryPublic))
					{
						File.Delete (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + this._path);
					}
				}

				if (source != destination)
				{
					FileInfo file = new FileInfo (source + this._id);
					file.MoveTo (destination + this._id);
				}
			}

			this._path = this._temppath;
			this._type = this._temptype;

			// Create new symlink if needed.
			if ((this._temptype == Enums.MediaType.Public) || (this._temptype == Enums.MediaType.TemporaryPublic))
			{
				// Check if path exists, if not create it.
				if (!Directory.Exists (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + System.IO.Path.GetDirectoryName (this._temppath)))
				{
					Directory.CreateDirectory (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + System.IO.Path.GetDirectoryName (this._temppath));
				}

				// Create a new symlink.
				UnixFileInfo unixfileinfo = new UnixFileInfo (this.DataPath);
				unixfileinfo.CreateSymbolicLink (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + this._temppath);
			}

			string path = string.Empty;

			switch (this._type)
			{
				case Enums.MediaType.Temporary:
				{
					path = Services.Config.Get<string> (Enums.ConfigKey.path_temp);
					break;
				}

				case Enums.MediaType.TemporaryPublic:
				{
					path = Services.Config.Get<string> (Enums.ConfigKey.path_temp);
					break;
				}

				case Enums.MediaType.Public:
				{
					path = Services.Config.Get<string> (Enums.ConfigKey.path_media);
					break;
				}

				case Enums.MediaType.Restricted:
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
				result = path + string.Format (Services.Config.Get<string> (Enums.ConfigKey.core_filenameincrementformat), filename, increment, extension);
				increment++;
			}

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
				item.Add ("path", this._path);
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
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.MEDIA", exception.Message));

				// EXCEPTION: Exception.MediaSave
				throw new Exception (string.Format (Strings.Exception.MediaSave, this._id));
			}
		}
		#endregion

		#region Public Static Methods
		public static Media Load (Guid id)
		{
			Media result;

			try
			{
				Hashtable item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (Services.Datastore.Get<XmlDocument> (DatastoreAisle, id.ToString ()).SelectSingleNode ("(//sorentolib.media2)[1]")));
				result = new Media ();

				result._id = new Guid ((string)item["id"]);

				if (item.ContainsKey ("createtimestamp"))
				{
					result._createtimestamp = int.Parse ((string)item["createtimestamp"]);
				}

				if (item.ContainsKey ("updatetimestamp"))
				{
					result._updatetimestamp = int.Parse ((string)item["updatetimestamp"]);
				}

				if (item.ContainsKey ("type"))
				{
					result._type = SNDK.Convert.StringToEnum<SorentoLib.Enums.MediaType> ((string)item["type"]);
					result._temptype = SNDK.Convert.StringToEnum<SorentoLib.Enums.MediaType> ((string)item["type"]);
				}

				if (item.ContainsKey ("path"))
				{
					result._path = (string)item["path"];
					result._temppath = (string)item["path"];
				}

				if (item.ContainsKey ("mimetype"))
				{
					result._mimetype = (string)item["mimetype"];
				}

				if (item.ContainsKey ("size"))
				{
					result._size = long.Parse ((string)item["size"]);
				}

				if (item.ContainsKey ("description"))
				{
					result._description = (string)item["description"];
				}

				if (item.ContainsKey ("copyright"))
				{
					result._copyright = (string)item["copyright"];
				}
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.MEDIA", exception.Message));

				// EXCEPTION: Excpetion.MediaLoad
				throw new Exception (string.Format (Strings.Exception.MediaLoad, id));
			}

			return result;
		}

		public static List<Media> List ()
		{
			List<Media> result = new List<Media> ();

			foreach (string shelf in Services.Datastore.ListOfShelfs (DatastoreAisle))
			{
				result.Add (Load (new Guid (shelf)));
			}

			return result;
		}

		public static void Delete (Guid id)
		{
			try
			{
				Media media = Media.Load (id);

				switch (media.Type)
				{
					case Enums.MediaType.Temporary:
					{
						File.Delete (Services.Config.Get<string> (Enums.ConfigKey.path_temp) + media.Id);
						break;
					}

					case Enums.MediaType.TemporaryPublic:
					{
						File.Delete (Services.Config.Get<string> (Enums.ConfigKey.path_temp) + media.Id);
						File.Delete (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + media.Path);
						break;
					}

					case Enums.MediaType.Public:
					{
						File.Delete (Services.Config.Get<string> (Enums.ConfigKey.path_media) + media.Id);
						File.Delete (Services.Config.Get<string> (Enums.ConfigKey.path_publicmedia) + media.Path);
						break;
					}


					case Enums.MediaType.Restricted:
					{
						File.Delete (Services.Config.Get<string> (Enums.ConfigKey.path_media) + media.Id);
						break;
					}
				}

				Services.Datastore.Delete (DatastoreAisle, id.ToString ());
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.MEDIA", exception.Message));

				// EXCEPTION: Exception.MediaDelete
				throw new Exception (string.Format (Strings.Exception.MediaDelete, id));
			}
		}
		#endregion

		#region Internal Static Methods
		internal static void ServicesSnapshotPurge ()
		{
		}

		internal static void ServiceGarbageCollector ()
		{
			foreach (Media media in List ())
			{
				if ((Date.DateTimeToTimestamp (DateTime.Now) - media.UpdateTimestamp) >  Services.Config.Get<int> (Enums.ConfigKey.core_mediamaxtempage))
				{
					if ((media.Type == SorentoLib.Enums.MediaType.Temporary) || (media.Type == SorentoLib.Enums.MediaType.TemporaryPublic))
					{
						try
						{
							Media.Delete (media.Id);
						}
						catch (Exception exception)
						{
							// LOG: LogDebug.ExceptionUnknown
							Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.MEDIA", exception.Message));
						}
					}
				}
			}

			// LOG: LogDebug.MediaGarbageCollector
			SorentoLib.Services.Logging.LogDebug (Strings.LogDebug.MediaGarbageCollector);
		}

		internal static void ServiceConfigChanged ()
		{
		}

		internal static void ServiceStatsUpdate ()
		{
			int count = 0;
			long totalsize = 0;

			foreach (Media media in Media.List ())
			{
				count++;
				totalsize += media.Size;
			}

			Services.Stats.Set (Enums.StatKey.sorentolib_media_count, count);
			Services.Stats.Set (Enums.StatKey.sorentolib_media_totalsize, totalsize);

			// LOG: LogDebug.MediaStats
			Services.Logging.LogDebug (Strings.LogDebug.MediaStats);
		}
		#endregion
	}
}

