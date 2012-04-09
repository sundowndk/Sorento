//
// Snapshot.cs
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
using System.Collections;
using System.Collections.Generic;

using SorentoLib;

using SNDK.DBI;


namespace Core.Addin
{
	public class Snapshot : SorentoLib.Addins.ISnapshot
	{
		#region Private Fields
		#endregion

		#region Constructor
		public Snapshot ()
		{
		}
		#endregion

		#region Public Methods
		public List<string> Take (string SnapshotRoot)
		{
			List<string> errors = new List<string> ();

			#region USER
			{
//				try
//				{
//					string root = SnapshotRoot + "sorentolib.user/";
//					Directory.CreateDirectory (root);
//					foreach (User user in User.List ())
//					{
//						try
//						{
//							SorentoLib.Tools.Helpers.ItemToFile (user.ToAjaxItem (), root + user.Id.ToString () +".xml");
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.user : " + exception.Message);
//						}
//					}
//				}
//				catch (Exception exception)
//				{
//					errors.Add ("sorentolib.user : " + exception.Message);
//				}
			}
			#endregion

			#region USERGROUP
			{
//				try
//				{
//					string root = SnapshotRoot + "sorentolib.usergroup/";
//					Directory.CreateDirectory (root);
//					foreach (Usergroup usergroup in Usergroup.List ())
//					{
//						try
//						{
//							SorentoLib.Tools.Helpers.ItemToFile (usergroup.ToAjaxItem (), root + usergroup.Id.ToString () +".xml");
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.usergroup : " + exception.Message);
//						}
//					}
//				}
//				catch (Exception exception)
//				{
//					errors.Add ("sorentolib.usergroup : " + exception.Message);
//				}
			}
			#endregion

//			#region MEDIA
//			{
//				try
//				{
//					string root = SnapshotRoot + "sorentolib.media/";
//					Directory.CreateDirectory (root);
//
//					foreach (Media media in Media.List ())
//					{
//						if (media.Status != SorentoLib.Enums.MediaStatus.Temporary || media.Status != SorentoLib.Enums.MediaStatus.PublicTemporary)
//						{
//							try
//							{
//								SorentoLib.Tools.Helpers.ItemToFile (media.ToAjaxItem (), root + media.Id.ToString () +".xml");
//							}
//							catch (Exception exception)
//							{
//								errors.Add ("sorentolib.media : " + exception.Message);
//							}
//						}
//					}
//				}
//				catch (Exception exception)
//				{
//					errors.Add ("sorentolib.media : " + exception.Message);
//				}
//			}
//			#endregion

			#region MEDIADATA
			{
				try
				{
					string root = SnapshotRoot + "sorentolib.media.data/";
					Directory.CreateDirectory (root);

					foreach (string path in SNDK.IO.GetFilesRecursive (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_media)))
					{
						try
						{
							string source = Path.GetDirectoryName (path) +"/" + Path.GetFileName (path);
							string destination = root + Path.GetFileName (path);

							SNDK.IO.CopyFile (source, destination);
						}
						catch (Exception exception)
						{
							errors.Add ("sorentolib.media.data : " + exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("sorentolib.media.data : " + exception.Message);
				}
			}
			#endregion

//			#region MEDIATRANSFORMATIONS
//			{
//				try
//				{
//					string root = SnapshotRoot + "sorentolib.mediatransformation/";
//					Directory.CreateDirectory (root);
//					foreach (MediaTransformation mediatransformation in MediaTransformation.List ())
//					{
//						try
//						{
//							SorentoLib.Tools.Helpers.ItemToFile (mediatransformation.ToAjaxItem (), root + mediatransformation.Id.ToString () +".xml");
//						}
//						catch (Exception exception)
//						{
//							errors.Add (exception.Message);
//						}
//					}
//				}
//				catch (Exception exception)
//				{
//					errors.Add (exception.Message);
//				}
//			}
//			#endregion

			#region DATA
			#endregion

			#region HTMLROOT
			{
				try
				{
					string root = SnapshotRoot +"sorentolib.htmlroot/";
					Directory.CreateDirectory (root);

					foreach (string directory in SNDK.IO.GetDirectoriesRecursive (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html)))
					{
						if (!directory.Contains (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html) + "administration"))
						{
							try
							{
								Directory.CreateDirectory (root + directory.Replace (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html), string.Empty));
							}
							catch (Exception exception)
							{
								errors.Add ("sorentolib.htmlroot : " + exception.Message);
							}
						}
					}

					foreach (string file in SNDK.IO.GetFilesRecursive (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html)))
					{
						if (!file.Contains (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html) + "administration/"))
						{
							try
							{
								string source = Path.GetDirectoryName (file) +"/" + Path.GetFileName (file);
								string destination = (root + Path.GetDirectoryName (file) +"/"+ Path.GetFileName (file)).Replace (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html), string.Empty);

								SNDK.IO.CopyFile (source, destination);
							}
							catch (Exception exception)
							{
								errors.Add ("sorentolib.htmlroot : " + exception.Message);
							}
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("sorentolib.htmroot : " + exception.Message);
				}
			}
			#endregion

			return errors;
		}

		public List<string> Develop (string SnapshotRoot)
		{
			List<string> errors = new List<string> ();

			#region HTMLROOT
			{
				string root = SnapshotRoot + "sorentolib.htmlroot/";

				try
				{
					foreach (string directory in System.IO.Directory.GetDirectories (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html)))
					{
						if (!directory.Contains (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html) + "administration"))
						{
							try
							{
								Directory.Delete (directory, true);
							}
							catch (Exception exception)
							{
								errors.Add ("sorentolib.htmlroot: "+ exception.Message);
							}
						}
					}

					foreach (string file in System.IO.Directory.GetFiles (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html)))
					{
						if (!file.Contains (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html) + "administration"))
						{
							try
							{
								File.Delete (file);
							}
							catch (Exception exception)
							{
								errors.Add ("sorentolib.htmlroot: "+ exception.Message);
							}
						}
					}

					foreach (string directory in SNDK.IO.GetDirectoriesRecursive (root))
					{
						try
						{
							Directory.CreateDirectory (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html) + directory.Replace (root, string.Empty));
						}
						catch (Exception exception)
						{
							errors.Add ("sorentolib.htmlroot: "+ exception.Message);
						}
					}

					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
					{
						try
						{
							string source = Path.GetDirectoryName (filepath) +"/"+ Path.GetFileName (filepath);
							string destination = SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_html) + source.Replace (root, string.Empty);

							SNDK.IO.CopyFile (source, destination);
						}
						catch (Exception exception)
						{
							errors.Add ("sorentolib.htmlroot: "+ exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("sorentolib.htmlroot: "+ exception.Message);
				}
			}
			#endregion

			#region USERGROUP
			{
//				string root = SnapshotRoot + "sorentolib.usergroup/";
//
//				try
//				{
//					foreach (Usergroup usergroup in Usergroup.List ())
//					{
//						try
//						{
//							QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
//							qb.Table (Usergroup.DatabaseTableName);
//							qb.AddWhere ("id", "=", usergroup.Id);
//
//							Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
//							query.Dispose ();
//							query = null;
//							qb = null;
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.usergroup :"+ exception.Message);
//						}
//					}
//
//					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
//					{
//						try
//						{
//							Usergroup usergroup = SorentoLib.Usergroup.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
//							usergroup.Save ();
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.usergroup :"+ exception.Message);
//						}
//					}
//				}
//				catch (Exception exception)
//				{
//					errors.Add ("sorentolib.usergroup: "+ exception.Message);
//				}
			}
			#endregion

			#region USER
			{
//				string root = SnapshotRoot + "sorentolib.user/";
//
//				try
//				{
//					foreach (User user in User.List ())
//					{
//						try
//						{
//							QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
//							qb.Table (User.DatabaseTableName);
//							qb.AddWhere ("id", "=", user.Id);
//
//							Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
//							query.Dispose ();
//							query = null;
//							qb = null;
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.user: "+ exception.Message);
//						}
//					}
//
//					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
//					{
//						try
//						{
//							User user = SorentoLib.User.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
//							user.Save ();
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.user: "+ exception.Message);
//						}
//					}
//				}
//				catch (Exception exception)
//				{
//					errors.Add ("sorentolib.user: "+ exception.Message);
//				}
			}
			#endregion

//			#region MEDIATRANSFORMATION
//			{
//				try
//				{
//					string root = SnapshotRoot + "sorentolib.mediatransformation/";
//
//					foreach (MediaTransformation mediatransformation in MediaTransformation.List ())
//					{
//						try
//						{
//							SorentoLib.Services.Datastore.Delete (MediaTransformation.DatastoreAisle, mediatransformation.Id.ToString ());
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.mediatransformation: "+ exception.Message);
//						}
//					}
//
//					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
//					{
//						try
//						{
//							MediaTransformation mediatransformation = SorentoLib.MediaTransformation.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
//							mediatransformation.Save ();
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.mediatransformation: "+ exception.Message);
//						}
//					}
//				}
//				catch (Exception exception)
//				{
//					errors.Add ("sorentolib.mediatransformation: "+ exception.Message);
//				}
//			}
//			#endregion

			#region MEDIA.DATA
			{
				string root = SnapshotRoot + "sorentolib.media.data/";

				try
				{
					foreach (string filepath in SNDK.IO.GetFilesRecursive (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_media)))
					{
						try
						{
							File.Delete (filepath);
						}
						catch (Exception exception)
						{
							errors.Add ("sorentolib.media.data: "+ exception.Message);
						}
					}

					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
					{
						try
						{
							string source = Path.GetDirectoryName (filepath) +"/"+ Path.GetFileName (filepath);
							string destination = SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_media) +"/"+ Path.GetFileName (filepath);

							SNDK.IO.CopyFile (source, destination);
						}
						catch (Exception exception)
						{
							errors.Add ("sorentolib.media.data: "+ exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("sorentolib.media.data: "+ exception.Message);
				}
			}
			#endregion

//			#region MEDIA
//			{
//				string root = SnapshotRoot + "sorentolib.media/";
//
//				try
//				{
//					foreach (Media media in Media.List ())
//					{
//						try
//						{
//							QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
//							qb.Table (Media.DatabaseTableName);
//							qb.AddWhere ("id", "=", media.Id);
//
//							Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
//							query.Dispose ();
//							query = null;
//							qb = null;
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.media: "+ exception.Message);
//						}
//					}
//
//					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
//					{
//						try
//						{
//							Media media = SorentoLib.Media.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
//							media.Save ();
//						}
//						catch (Exception exception)
//						{
//							errors.Add ("sorentolib.media: "+ exception.Message);
//						}
//					}
//				}
//				catch (Exception exception)
//				{
//					errors.Add ("sorentolib.media: "+ exception.Message);
//				}
//			}
//			#endregion

			return errors;
		}
		#endregion
	}
}
