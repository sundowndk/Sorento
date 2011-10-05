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


namespace sCMS.Addin
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

			#region ROOT
			{
				try
				{
					string root = SnapshotRoot + "scms.root/";
					Directory.CreateDirectory (root);

					foreach (Root _root in Root.List ())
					{
						try
						{
							SorentoLib.Tools.Helpers.ItemToFile (_root.ToAjaxItem (), root + _root.Id.ToString () +".xml");
						}
						catch (Exception exception)
						{
							errors.Add ("scms.root : " + exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.root : " + exception.Message);
				}
			}
			#endregion

			#region TEMPLATE
			{
				try
				{
					string root = SnapshotRoot + "scms.template/";
					Directory.CreateDirectory (root);

					foreach (Template template in Template.List ())
					{
						try
						{
							SorentoLib.Tools.Helpers.ItemToFile (template.ToAjaxItem (), root + template.Id.ToString () +".xml");
						}
						catch (Exception exception)
						{
							errors.Add ("scms.template : " + exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.template : " + exception.Message);
				}
			}
			#endregion

			#region PAGE
			try
			{
				string root = SnapshotRoot + "scms.page/";
				Directory.CreateDirectory (root);

				foreach (Page page in Page.List ())
				{
					try
					{
						SorentoLib.Tools.Helpers.ItemToFile (page.ToAjaxItem (), root + page.Id.ToString () +".xml");
					}
					catch (Exception exception)
					{
						errors.Add ("scms.page : " + exception.Message);
					}
				}
			}
			catch (Exception exception)
			{
				errors.Add ("scms.page : " + exception.Message);
			}
			#endregion

			#region GLOBAL
			{
				try
				{
					string root = SnapshotRoot + "scms.global/";
					Directory.CreateDirectory (root);

					foreach (Global _global in Global.List ())
					{
						try
						{
							SorentoLib.Tools.Helpers.ItemToFile (_global.ToAjaxItem (), root + _global.Id.ToString () +".xml");
						}
						catch (Exception exception)
						{
							errors.Add ("scms.global : " + exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.global : " + exception.Message);
				}
			}
			#endregion

			#region COLLECTIONSCHEMA
			{
				try
				{
					string root = SnapshotRoot + "scms.collectionschema/";
					Directory.CreateDirectory (root);

					foreach (CollectionSchema collectionschema in CollectionSchema.List ())
					{
						try
						{
							SorentoLib.Tools.Helpers.ItemToFile (collectionschema.ToAjaxItem (), root + collectionschema.Id.ToString () +".xml");
						}
						catch (Exception exception)
						{
							errors.Add ("scms.collectionschema : " + exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.collectionschema : " + exception.Message);
				}
			}
			#endregion

			#region COLLECTION
			{
				try
				{
					string root = SnapshotRoot + "scms.collection/";
					Directory.CreateDirectory (root);

					foreach (Collection collection in Collection.List ())
					{
						try
						{
							SorentoLib.Tools.Helpers.ItemToFile (collection.ToAjaxItem (), root + collection.Id.ToString () +".xml");
						}
						catch (Exception exception)
						{
							errors.Add ("scms.collection : " + exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.collection : " + exception.Message);
				}
			}
			#endregion

			return errors;
		}

		public List<string> Develop (string SnapshotRoot)
		{
			List<string> errors = new List<string> ();

			#region ROOT
			{
				string root = SnapshotRoot + "scms.root/";

				try
				{
					foreach (Root _root in Root.List ())
					{
						try
						{
							Root.Delete (_root.Id);
						}
						catch (Exception exception)
						{
							errors.Add ("scms.root: "+ exception.Message);
						}
					}

					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
					{
						try
						{
							Root _root = Root.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
							_root.Save ();
						}
						catch (Exception exception)
						{
							errors.Add ("scms.root: "+ exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.root: "+ exception.Message);
				}
			}
			#endregion

			#region TEMPLATE
			{
				string root = SnapshotRoot + "scms.template/";

				try
				{
					foreach (Template template in Template.List ())
					{
						try
						{
							Template.Delete (template.Id);
						}
						catch (Exception exception)
						{
							errors.Add ("scms.template: "+ exception.Message);
						}
					}

					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
					{
						try
						{
							Template template = Template.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
							template.Save ();
						}
						catch (Exception exception)
						{
							errors.Add ("scms.template: : "+ exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.template: "+ exception.Message);
				}
			}
			#endregion

			#region PAGE
			{
				string root = SnapshotRoot + "scms.page/";

				try
				{
					foreach (Page page in Page.List ())
					{
						try
						{
							Page.Delete (page.Id);
						}
						catch (Exception exception)
						{
							errors.Add ("scms.page: "+ exception.Message);
						}
					}

					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
					{
						try
						{
							Page page = Page.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
							page.Save ();
						}
						catch (Exception exception)
						{
							errors.Add ("scms.page: "+ exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.page: "+ exception.Message);
				}
			}
			#endregion

			#region GLOBAL
			{
				string root = SnapshotRoot + "scms.global/";

				try
				{
					foreach (Global _global in Global.List ())
					{
						try
						{
							Global.Delete (_global.Id);
						}
						catch (Exception exception)
						{
							errors.Add ("scms.global: "+ exception.Message);
						}
					}

					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
					{
						try
						{
							Global _global = Global.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
							_global.Save ();
						}
						catch (Exception exception)
						{
							errors.Add ("scms.global: "+ exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.global: "+ exception.Message);
				}
			}
			#endregion

			#region COLLECTIONSCHEMA
			{
				string root = SnapshotRoot + "scms.collectionschema/";

				try
				{
					foreach (Collection collection in Collection.List ())
					{
						try
						{
							Collection.Delete (collection.Id);
						}
						catch (Exception exception)
						{
							errors.Add ("scms.collection: "+ exception.Message);
						}
					}

					foreach (CollectionSchema collectionschema in CollectionSchema.List ())
					{
						try
						{
							CollectionSchema.Delete (collectionschema.Id);
						}
						catch (Exception exception)
						{
							errors.Add ("scms.collectionschema: "+ exception.Message);
						}
					}

					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
					{
						try
						{
							CollectionSchema collectionschema = CollectionSchema.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
							collectionschema.Save ();
						}
						catch (Exception exception)
						{
							errors.Add ("scms.collectionschema: "+ exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.collectionschema: "+ exception.Message);
				}
			}
			#endregion

			#region COLLECTION
			{
				string root = SnapshotRoot + "scms.collection/";

				try
				{
					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
					{
						try
						{
							Collection collection = Collection.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
							collection.Save ();
						}
						catch (Exception exception)
						{
							errors.Add ("scms.collection: "+ exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("scms.collection: "+ exception.Message);
				}
			}
			#endregion

			foreach (string error in errors)
			{
				Console.WriteLine (error);
			}

			return errors;
		}
		#endregion
	}
}
