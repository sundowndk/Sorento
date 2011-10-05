//
// Snapshot.cs
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
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Mono.Addins;

namespace SorentoLib.Services
{
	public class Snapshot
	{
		#region Private Fields
		private string _id;
		private Hashtable _manifest;
		#endregion

		#region Public Fields
		public string Id
		{
			get
			{
				return this._id;
			}
		}

		public DateTime Date
		{
			get
			{
				return DateTime.Parse ((string)this._manifest["date"]);
			}
		}

		public string Description
		{
			get
			{
				return (string)this._manifest["description"];
			}
		}

		public string FileName

		{
			get
			{
				return this._id +".zip";
			}
		}
		#endregion

		#region Constructor
		private Snapshot ()
		{
			Initialize ();
		}

		private Snapshot (string Description)
		{
			Initialize ();
			this._manifest.Add ("description", Description);
		}

		private void Initialize ()
		{
			DateTime date = DateTime.Now;
			this._id = "snapshot_"+ date.ToString ().Replace (":","-").Replace (" ","_").Replace ("/","_");
			this._manifest = new Hashtable ();
			this._manifest.Add ("date", date.ToString ());
		}
		#endregion

		#region Public Methods
		public void ToAjaxRespons (SorentoLib.Ajax.Respons Respons)
		{
			Respons.Data = this.ToAjaxItem ();
		}

		public Hashtable ToAjaxItem ()
		{
			Hashtable result = new Hashtable ();

			result.Add ("id", this._id);
			result.Add ("date", this.Date);
			result.Add ("filename", this.FileName);

			return result;
		}
		#endregion

		#region Public Static Methods
		public static Snapshot Load (string Id)
		{
			Snapshot result = null;

			if (File.Exists (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.snapshot_path) + Id +".zip"))
			{
				result = new Snapshot ();
				result._id = Id;

				#region READ MANIFEST
				{
					try
					{
						System.Diagnostics.Process proc = new System.Diagnostics.Process();
						proc.EnableRaisingEvents=false;
						proc.StartInfo.FileName="unzip";
						proc.StartInfo.WorkingDirectory = Environment.CurrentDirectory +"/"+ SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_pathtmp);
						proc.StartInfo.Arguments = "-qq " + Environment.CurrentDirectory +"/"+ SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.snapshot_path) + Id +".zip " + Id + "/manifest.xml -d "+ Environment.CurrentDirectory +"/"+ SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_pathtmp);
						proc.Start ();
						proc.WaitForExit ();

						result._manifest = SorentoLib.Tools.Helpers.FileToItem (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_pathtmp) + Id + "/manifest.xml");

						Directory.Delete (Path.GetDirectoryName (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_pathtmp) + Id +"/"), true);
					}
					catch (Exception exception)
					{
						Console.WriteLine (exception.Message);
						throw new Exception (string.Format (Strings.Exception.ServicesSnapshotLoadManifest, Id));
					}
				}
				#endregion
			}
			else
			{
				throw new Exception (string.Format (Strings.Exception.ServicesSnapshotLoad, Id));
			}

			return result;
		}

		public static void Delete (string Id)
		{
			if (File.Exists (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.snapshot_path) + Id +".zip"))
			{
				File.Delete (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.snapshot_path) + Id +".zip");
			}
			else
			{
				throw new Exception (string.Format (Strings.Exception.SerivcesSnapshotDelete, Id));
			}
		}

		public static List<Snapshot> List ()
		{
			List<Snapshot> result = new List<Snapshot> ();

			foreach (string file in System.IO.Directory.GetFiles (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.snapshot_path)))
			{
				try
				{
					result.Add (Snapshot.Load (Path.GetFileNameWithoutExtension (file)));
				}
				catch
				{
					Services.Logging.LogError (string.Format (Strings.LogError.ServicesSnapshotList, Path.GetFileNameWithoutExtension (file)));
				}
			}

			return result;
		}

		public static void Develop (Snapshot Snapshot)
		{
			List<string> errors = new List<string> ();

			string workingdirectory = SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.snapshot_path);
			string snapshotroot = workingdirectory + Snapshot.Id +"/";

			#region UNZIP ARCHIVE
			{
				System.Diagnostics.Process process = new System.Diagnostics.Process ();
				process.StartInfo.FileName = "unzip";
				process.StartInfo.WorkingDirectory = workingdirectory;
				process.StartInfo.Arguments = "-q "+ Snapshot.Id +".zip";
				process.Start ();
				process.WaitForExit ();
			}
			#endregion

			#region ADDINS
			foreach (SorentoLib.Addins.ISnapshot snapshot in AddinManager.GetExtensionObjects (typeof (SorentoLib.Addins.ISnapshot)))
			{
				errors.AddRange (snapshot.Develop (snapshotroot));
			}
			#endregion

			#region CLEANUP
			{
				Directory.Delete (snapshotroot, true);
			}
			#endregion

			foreach (string error in errors)
			{
				Console.WriteLine (error);
			}
		}

		public static Snapshot Take ()
		{
			return Take (string.Empty);
		}

		public static Snapshot Take (string Description)
		{
			Snapshot result = new Snapshot (Description);

			List<string> errors = new List<string> ();

			string workingdirectory = Environment.CurrentDirectory +"/"+ SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_pathtmp);
			string snapshotroot = workingdirectory + result._id +"/";

			#region CREATE SNAPSHOTROOT
			Directory.CreateDirectory (snapshotroot);
			#endregion

			#region ADDINS
			foreach (SorentoLib.Addins.ISnapshot snapshot in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.ISnapshot)))
			{
				errors.AddRange (snapshot.Take (snapshotroot));
			}
			#endregion

			#region CREATE MANIFEST
			{
				SorentoLib.Tools.Helpers.ItemToFile (result._manifest, snapshotroot + "manifest.xml");
			}
			#endregion

			#region CREATE ARCHIVE
			{
				try
				{
					System.Diagnostics.Process process = new System.Diagnostics.Process();
					process.StartInfo.FileName = "zip";
					process.StartInfo.WorkingDirectory = workingdirectory;
					process.StartInfo.Arguments = "-rq "+ workingdirectory + result.FileName +" "+ result.Id;
					process.Start ();
					process.WaitForExit ();

					string source = workingdirectory + result.FileName;
					string destination = SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.snapshot_path) + result.FileName;
					SNDK.IO.CopyFile (source, destination);
				}
				catch (Exception exception)
				{
					errors.Add (exception.Message);
				}
			}
			#endregion

			#region CLEANUP
			{
				try
				{
					Directory.Delete (snapshotroot, true);
					File.Delete (workingdirectory + result.FileName);
				}
				catch (Exception exception)
				{
					errors.Add (exception.Message);
				}
			}
			#endregion

			return result;
		}
		#endregion
	}
}

