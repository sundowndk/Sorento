//
// Media.cs
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
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Mono.Unix;

using SNDK;
using SNDK.DBI;
using SNDK.Enums;

namespace SorentoLib
{	
	public class Media
	{
		#region Public Static Fields
		public static string DatabaseTableName = Services.Database.Prefix + "media";
		#endregion

		#region Private Fields		
		private Guid _id;
		private int _createtimestamp;
		private int _updatetimestamp;

		private string _path;
		private string _currentpath;
		private string _mimetype;
		private long _size;

		private Enums.MediaStatus _status;
		private Enums.MediaStatus _currentstatus;

		private Enums.Accesslevel _accesslevel;
		private string _usergroupids;

		private List<Usergroup> _usergroups;

		private string _description;
		private string _copyright;

		private string _variantids;

		private List<Usergroup> _tempusergroups;
		private List<Media> _tempvariants;
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

		public string DataPath
		{
			get
			{
				switch (this._currentstatus)
				{
					case SorentoLib.Enums.MediaStatus.Temporary:
						return Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id;

					case SorentoLib.Enums.MediaStatus.PublicTemporary:
						return Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id;

					default:
						return Services.Config.Get<string> (Enums.ConfigKey.core_pathmedia) + this._id;
				}
			}
		}

		public string Path
		{
			get
			{
				return this._currentpath;
			}

			set
			{
				this._path = Media.FixPath (value);
			}
		}

		public string FileName
		{
			get
			{
				return System.IO.Path.GetFileName (this._currentpath);
			}
		}

		public string DirectoryName
		{
			get
			{
				return System.IO.Path.GetDirectoryName (this._currentpath);
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

		public Enums.MediaStatus Status
		{
			get
			{
				return this._currentstatus;
			}

			set
			{
				this._status = value;
			}
		}

		public Enums.Accesslevel Accesslevel
		{
			get
			{
				return this._accesslevel;
			}

			set
			{
				this._accesslevel = value;
			}
		}

		public List<Usergroup> Usergroups
		{
			get
			{
				if (this._tempusergroups == null)
				{
					this._tempusergroups = new List<Usergroup> ();
					foreach (string usergroupid in this._usergroupids.Split (";".ToCharArray ()))
					{
						try
						{
							this._usergroups.Add (Usergroup.Load (new Guid (usergroupid)));
						}
						catch
						{
							Services.Logging.LogError (string.Format (Strings.LogError.MediaLoadUsergroup, usergroupid));
						}
					}
				}

				return this._tempusergroups;
			}
		}

		public List<Media> Variants
		{
			get
			{
				if (this._tempvariants == null)
				{
					this._tempvariants = new List<Media> ();
					foreach (string variantid in this._variantids.Split (";".ToCharArray ()))
					{
						try
						{
							this._tempvariants.Add (Media.Load (new Guid (variantid)));
						}
						catch
						{
							Services.Logging.LogError (string.Format (Strings.LogError.MediaLoadVariant, variantid));
						}
					}
				}

				return this._tempvariants;
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

		#region Private Constructor
		private Media ()
		{
			this._usergroups = new List<Usergroup> ();
		}
		#endregion

		#region Public Constructor
		public Media (string Path, byte [] Data)
		{
			this.Initalize ();

			FileStream filestream = File.Create (Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id);
			BinaryWriter binarywriter = new BinaryWriter(filestream);
			binarywriter.Write(Data);
			binarywriter.Close();
			filestream.Close();

			this._path = Media.FixPath (Path);
			this._size = Data.LongLength;
			this._mimetype = IO.GetMimeType (SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id);
		}

		public Media (string Path, string SourcePath, bool MoveFile)
		{
			this.Initalize ();

			FileInfo fileinfo = new FileInfo (SourcePath);
			if (MoveFile)
			{
				fileinfo.MoveTo (Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id);
			}
			else
			{
				fileinfo.CopyTo (Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id);
			}

			this._path = Media.FixPath (Path);
			this._size = fileinfo.Length;
			this._mimetype = IO.GetMimeType (Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id);
		}

		public Media (string Path, string Url)
		{
			this.Initalize ();

			IO.DownloadToFile (Url, Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id);

			FileInfo fileinfo = new FileInfo (Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id);

			this._path = Media.FixPath (Path);
			this._size = fileinfo.Length;
			this._mimetype = IO.GetMimeType (Services.Config.Get<string> (Enums.ConfigKey.core_pathtmp) + this._id);
		}
		#endregion

		#region Private Methods
		private void Initalize ()
		{
			this._id = Guid.NewGuid();
			this._createtimestamp = Date.CurrentDateTimeToTimestamp();
			this._updatetimestamp = Date.CurrentDateTimeToTimestamp();

			this._accesslevel = Enums.Accesslevel.Guest;
			this._usergroupids = string.Empty;

			this._status = Enums.MediaStatus.Temporary;
			this._currentstatus = Enums.MediaStatus.Temporary;

			this._path = string.Empty;
			this._currentpath = string.Empty;

			this._description = string.Empty;
			this._copyright = string.Empty;

			this._variantids = string.Empty;
		}

		private void UpdateData ()
		{
			// Check if data still exists.
			if (!File.Exists (this.DataPath))
			{
				throw new Exception (string.Format (Strings.Exception.MediaSaveData, this._id));
			}

			// Make sure data is in the right place.
			string currentdatapath = this.DataPath;
			this._currentstatus = this._status;

			if (currentdatapath != this.DataPath)
			{
				FileInfo file = new FileInfo (currentdatapath);
				file.MoveTo (this.DataPath);
			}

			// Remove old symlink if needed.
			if (this._currentstatus == SorentoLib.Enums.MediaStatus.Public || this._currentstatus == SorentoLib.Enums.MediaStatus.PublicTemporary)
			{
				if (this._currentpath != string.Empty)
				{
					File.Delete (Services.Config.Get<string> (Enums.ConfigKey.core_pathpublicmedia) + this._currentpath);
				}
			}

			// Create new symlink if needed.
			if (this._status == SorentoLib.Enums.MediaStatus.Public || this._status == SorentoLib.Enums.MediaStatus.PublicTemporary)
			{
				// Check if path exists, if not create it.
				if (!Directory.Exists (Services.Config.Get<string> (Enums.ConfigKey.core_pathpublicmedia) + System.IO.Path.GetDirectoryName (this._path)))
				{
					Directory.CreateDirectory (Services.Config.Get<string> (Enums.ConfigKey.core_pathpublicmedia) + System.IO.Path.GetDirectoryName (this._path));
				}

				// Create a new symlink.
				UnixFileInfo unixfileinfo = new UnixFileInfo (this.DataPath);
				unixfileinfo.CreateSymbolicLink (Services.Config.Get<string> (Enums.ConfigKey.core_pathpublicmedia) + this._path);
			}

			// Check if size changed.
			FileInfo fileinfo = new FileInfo (this.DataPath);
			this._size = fileinfo.Length;

			this._currentpath = this._path;
			this._currentstatus = this._status;
		}
		#endregion

		#region Public Methods
		public void Save ()
		{
			bool success = false;
			this.UpdateData ();
			this._updatetimestamp = Date.CurrentDateTimeToTimestamp ();

			if (this._tempusergroups != null)
			{
				this._usergroupids = string.Empty;
				foreach (Usergroup usergroup in this._tempusergroups)
				{
					this._usergroupids += usergroup.Id.ToString () +";";
				}
				this._usergroupids = this._usergroupids.TrimEnd (";".ToCharArray ());
			}

			QueryBuilder qb;
			if (!SNDK.DBI.Helpers.GuidExists (Services.Database.Connection, DatabaseTableName, this._id))
			{
				qb = new QueryBuilder (QueryBuilderType.Insert);
			}
			else
			{
				qb = new  QueryBuilder (QueryBuilderType.Update);
				qb.AddWhere("id", "=", this._id);
			}

			qb.Table (DatabaseTableName);
			qb.Columns ("id",
			            "createtimestamp",
			            "updatetimestamp",
			            "path",
			            "mimetype",
			            "size",
			            "status",
			            "accesslevel",
			            "usergroupids",
			            "description",
			            "copyright",
			            "variantids");

			qb.Values (this._id,
			           this._createtimestamp,
			           this._updatetimestamp,
			           this._path,
			           this._mimetype,
			           this._size,
			           this._status,
			           this._accesslevel,
			           this._usergroupids,
			           this._description,
			           this._copyright,
			           this._variantids);

			Query query = Services.Database.Connection.Query (qb.QueryString);

			if (query.AffectedRows > 0)
			{
				success = true;
			}

			query.Dispose ();
			query = null;
			qb = null;

			if (!success)
			{
				throw new Exception (string.Format (Strings.Exception.MediaSave, this._id));
			}
		}

		public void ToAjaxRespons (SorentoLib.Ajax.Respons Respons)
		{
//			Respons.Data = this.ToAjaxItem ();
		}

		public Hashtable ToAjaxItem ()
		{
			Hashtable result = new Hashtable ();
			result.Add ("id", this._id);
			result.Add ("createtimestamp", this._createtimestamp);
			result.Add ("updatetimstamp", this._updatetimestamp);
			result.Add ("path", this._currentpath);
			result.Add ("filename", this.FileName);
			result.Add ("directoryname", this.DirectoryName);
			result.Add ("mimetype", this._mimetype);
			result.Add ("size", this._size);
			result.Add ("accesslevel", this._accesslevel);
			result.Add ("status", this._status);
			result.Add ("description", this._description);
			result.Add ("copyright", this._copyright);

			return result;
		}

		public Media Clone (string Path)
		{
			Media result = new Media (Path, this.DataPath, false);
			result.Status = this._currentstatus;
			result.Save ();

			return result;
		}

//		public Media AddVariant (string Name)
//		{
//			Media result = new Media (this.Path +"_"+ Name, this.DataPath, false);
//
//			if (this._tempvariants == null)
//			{
//				this._tempvariants = new List<Media> ();
//				this._tempvariants.Add (result);
//			}
//
//			return result;
//		}
		#endregion

		#region Public Static Methods
		public static SorentoLib.Media Load (Guid Id)
		{
			bool success = false;
			SorentoLib.Media result = new SorentoLib.Media ();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("id",
			            "createtimestamp",
			            "updatetimestamp",
			            "path",
			            "mimetype",
			            "size",
			            "status",
			            "accesslevel",
			            "usergroupids",
			            "description",
			            "copyright",
			            "variantids");

			qb.AddWhere ("id", "=", Id);

			Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				if (query.NextRow ())
				{
					result._id = query.GetGuid (qb.ColumnPos ("id"));
					result._createtimestamp = query.GetInt (qb.ColumnPos ("createtimestamp"));
					result._updatetimestamp = query.GetInt (qb.ColumnPos ("updatetimestamp"));
					result._currentpath = query.GetString (qb.ColumnPos ("path"));
					result._mimetype = query.GetString (qb.ColumnPos ("mimetype"));
					result._size = query.GetLong (qb.ColumnPos ("size"));
					result._currentstatus = query.GetEnum<SorentoLib.Enums.MediaStatus> (qb.ColumnPos ("status"));
					result._accesslevel = query.GetEnum<SorentoLib.Enums.Accesslevel> (qb.ColumnPos ("accesslevel"));
					result._usergroupids = query.GetString (qb.ColumnPos ("usergroupids"));
					result._description = query.GetString (qb.ColumnPos ("description"));
					result._copyright = query.GetString (qb.ColumnPos ("copyright"));
					result._variantids = query.GetString (qb.ColumnPos ("variantids"));
					success = true;
				}
			}

			query.Dispose ();
			query = null;
			qb = null;

			if (!success)
			{
				throw new Exception (string.Format (Strings.Exception.MediaLoad, Id));
			}

			return result;
		}

		public static void Delete (Guid Id)
		{
			try
			{
				Media media = Media.Load (Id);

				QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
				qb.Table (DatabaseTableName);
				qb.AddWhere ("id", "=", Id);

				Query query = Services.Database.Connection.Query (qb.QueryString);

				if (query.AffectedRows > 0)
				{
					File.Delete (media.DataPath);
					media = null;
				}

				query.Dispose ();
				query = null;
				qb = null;
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.MediaDelete, Id));
			}
		}

		public static List<Media> List()
		{
			List<Media> result = new List<Media>();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table(SorentoLib.Media.DatabaseTableName);
			qb.Columns("id");

			SNDK.DBI.Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				while (query.NextRow())
				{
					try
					{
						result.Add(SorentoLib.Media.Load (query.GetGuid (qb.ColumnPos ("id"))));
					}
					catch
					{
						SorentoLib.Services.Logging.LogDebug (string.Format (SorentoLib.Strings.LogError.MediaList, qb.ColumnPos ("id")));
					}
				}
			}

			query.Dispose();
			query = null;
			qb = null;

			return result;
		}

		public static Media FromAjaxItem (Hashtable Item)
		{
			Media result = null;

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
					result = Media.Load (id);
				}
				catch
				{
					result = new Media ();
					result._id = id;

					if (Item.ContainsKey ("createtimestamp"))
					{
						result._createtimestamp = int.Parse ((string)Item["createtimestamp"]);
					}

					if (Item.ContainsKey ("updatetimestamp"))
					{
						result._createtimestamp = int.Parse ((string)Item["updatetimestamp"]);
					}
				}
			}
			else
			{
				result = new Media ();
			}

			if (Item.ContainsKey ("path"))
			{
				result._path = (string)Item["path"];
			}

			if (Item.ContainsKey ("size"))
			{
				try
				{
					result._size = long.Parse ((string)Item["size"]);
				}
				catch {}
			}

			if (Item.ContainsKey ("mimetype"))
			{
				result._mimetype = (string)Item["mimetype"];
			}

			if (Item.ContainsKey ("accesslevel"))
			{
				result._accesslevel = SNDK.Convert.StringToEnum<SorentoLib.Enums.Accesslevel> ((string)Item["accesslevel"]);
			}

			if (Item.ContainsKey ("status"))
			{
				result._status = SNDK.Convert.StringToEnum<SorentoLib.Enums.MediaStatus> ((string)Item["status"]);
			}

			if (Item.ContainsKey ("description"))
			{
				result._description = (string)Item["description"];
			}

			if (Item.ContainsKey ("copyright"))
			{
				result._copyright = (string)Item["copyright"];
			}


//			if (Item.ContainsKey ("accesslevel"))
//			{
//				}
//			}

			return result;
		}
		#endregion

		#region Private Static Methods
		private static string FixPath (string Path)
		{
			string result = Path;
			string path = System.IO.Path.GetDirectoryName (Path) +"/";
			string filename = System.IO.Path.GetFileNameWithoutExtension (Path);
			string extension = System.IO.Path.GetExtension (Path);
			List<string> files = new List<string> ();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("path");
			qb.AddWhere ("path", "like", "%"+ path +"%");

			Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				while (query.NextRow ())
				{
					files.Add (query.GetString (qb.ColumnPos ("path")));
				}
			}

			int increment = 1;
			while (files.Contains (result))
			{
				result = path + filename +"("+ increment +")" + extension;
				increment++;
			}

			return result;
		}
		#endregion

		#region Internal Static Methods
		internal static void ServicesSnapshotPurge ()
		{
			foreach (Media media in Media.List ())
			{
				QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
				qb.Table (DatabaseTableName);
				qb.AddWhere ("id", "=", media.Id);

				Query query = Services.Database.Connection.Query (qb.QueryString);
				query.Dispose ();
				query = null;
				qb = null;
			}
		}

		internal static void ServiceConfigChanged ()
		{
			DatabaseTableName = SorentoLib.Services.Database.Prefix + "media";
		}

		internal static void ServiceGarbageCollector ()
		{
			SorentoLib.Services.Logging.LogDebug (Strings.LogError.MediaGarbageCollector);

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table(SorentoLib.Media.DatabaseTableName);
			qb.Columns ("id", "updatetimestamp");
			qb.AddWhere ("status", "=", (int)SorentoLib.Enums.MediaStatus.Temporary);
			qb.AddWhereOR ();
			qb.AddWhere ("status", "=", (int)SorentoLib.Enums.MediaStatus.PublicTemporary);

			Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				while (query.NextRow ())
				{
					if ((SNDK.Date.CurrentDateTimeToTimestamp () - query.GetInt (qb.ColumnPos ("updatetimestamp"))) > SorentoLib.Services.Config.Get<int> (Enums.ConfigKey.media_tempmaxage))
					{
						Delete (query.GetGuid (qb.ColumnPos ("id")));
					}
				}
			}

			query.Dispose ();
			query = null;
			qb = null;
		}
		#endregion
	}
}
