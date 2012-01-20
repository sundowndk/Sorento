// 
// Cache.cs
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
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SorentoLib.Services
{
	public class Cache
	{
		#region Private Static Fields
		private static List<SorentoLib.Services.Cache> Store;
		#endregion

		#region Public Static Fields
		public static bool Enabled = false;
		#endregion

		#region Private Fields
		private string _filename;
		private List<string> _content;
		#endregion

		#region Public Fields
		public string Filename
		{
			get
			{
				return this._filename;
			}

			set
			{
				this._filename = value;
			}
		}

		public List<string> Content
		{
			get
			{
				return this._content;
			}
		}
		#endregion

		#region Constructor
		private Cache (string filename)
		{
			this._filename = filename;
			this._content = SorentoLib.Render.Template.Read (filename);
		}
		#endregion

		#region Private Methods
		private void Update ()
		{
			this._content.Clear ();
			this._content = SorentoLib.Render.Template.Read (this._filename);
		}
		#endregion

		#region Public Static Methods
		public static void Initialize ()
		{
			if (SorentoLib.Services.Config.Get<bool> (Enums.ConfigKey.core_enablecache))
			{
				// Initialize cache engine.
				SorentoLib.Services.Cache.Store = new List<SorentoLib.Services.Cache> ();

				// Monitor content folder for changes
				FileSystemWatcher watchlib = new FileSystemWatcher ();
				watchlib.Path = "../Content/";
				watchlib.IncludeSubdirectories = true;
				watchlib.Changed += new FileSystemEventHandler (SorentoLib.Services.Cache.OnContentFileChanged);
				watchlib.Deleted += new FileSystemEventHandler (SorentoLib.Services.Cache.OnContentFileChanged);
				watchlib.EnableRaisingEvents = true;

				SorentoLib.Services.Cache.Enabled = true;

				// Done
				SorentoLib.Services.Logging.LogInfo("Service enabled: Cache");
			}
			else
			{
			}
		}

		public static List<string> Add (string filename)
		{
			SorentoLib.Services.Cache cacheitem = SorentoLib.Services.Cache.Store.Find (delegate (Cache find) { return (find.Filename == filename); });

			if (cacheitem == null)
			{
				cacheitem = new SorentoLib.Services.Cache (filename);
				SorentoLib.Services.Cache.Store.Add (cacheitem);
				SorentoLib.Services.Logging.LogDebug ("Cache: ["+ filename +"] added.");
			}

			return cacheitem.Content;
		}

		public static List<string> Get (string filename)
		{
			Cache cacheitem = SorentoLib.Services.Cache.Store.Find (delegate (Cache find) { return (find.Filename == filename); });

			if (cacheitem != null)
			{
				SorentoLib.Services.Logging.LogDebug ("Cache: ["+ filename +"] fetched.");
			}

			return cacheitem.Content;
		}

		public static void Remove (string filename)
		{
			Cache cacheitem = SorentoLib.Services.Cache.Store .Find (delegate (Cache find) { return (find.Filename == filename); });

			if (cacheitem != null)
			{
				SorentoLib.Services.Cache.Store.Remove (cacheitem);
				SorentoLib.Services.Logging.LogDebug ("Cache: ["+ filename +"] removed.");
			}

			cacheitem = null;
		}

		public static bool Exists (string filename)
		{
			Cache cacheitem = SorentoLib.Services.Cache.Store.Find (delegate (Cache find) { return (find.Filename == filename); });

			if (cacheitem != null)
			{
				return true;
			}

			return false;
		}

		public static void Update (string filename)
		{
			Cache cacheitem = SorentoLib.Services.Cache.Store.Find (delegate (Cache find) { return (find.Filename == filename); });

			if (cacheitem != null)
			{
				cacheitem.Update ();
				SorentoLib.Services.Logging.LogDebug ("Cache: ["+ filename +"] updated.");
			}

			cacheitem = null;
		}
		#endregion

		#region Private Static Methods
		private static void OnContentFileChanged (object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType == WatcherChangeTypes.Deleted)
			{
				if (System.IO.Path.GetExtension (e.Name) == ".stpl")
				{
					SorentoLib.Services.Cache.Remove (e.FullPath);
				}
			}

			if (e.ChangeType == WatcherChangeTypes.Changed)
			{
				if (System.IO.Path.GetExtension (e.Name) == ".stpl")
				{
					SorentoLib.Services.Cache.Update (e.FullPath);
				}
			}
		}
		#endregion
	}
}

