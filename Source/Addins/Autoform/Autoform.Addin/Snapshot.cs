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


namespace Autoform.Addin
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

			#region FORM
			{
				try
				{
					string root = SnapshotRoot + "autoform.form/";
					Directory.CreateDirectory (root);

					foreach (Form form in Form.List ())
					{
						try
						{
							SorentoLib.Tools.Helpers.ItemToFile (form.ToAjaxItem (), root + form.Id.ToString () +".xml");
						}
						catch (Exception exception)
						{
							errors.Add ("autoform.form : " + exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("autoform.form : " + exception.Message);
				}
			}
			#endregion

			return errors;
		}

		public List<string> Develop (string SnapshotRoot)
		{
			List<string> errors = new List<string> ();

			#region FORM
			{
				string root = SnapshotRoot + "autoform.form/";

				try
				{
					foreach (Form form in Form.List ())
					{
						try
						{
							Form.Delete (form.Id);
						}
						catch (Exception exception)
						{
							errors.Add ("autoform.form: "+ exception.Message);
						}
					}

					foreach (string filepath in SNDK.IO.GetFilesRecursive (root))
					{
						try
						{
							Form form = Form.FromAjaxItem (SorentoLib.Tools.Helpers.FileToItem (filepath));
							form.Save ();
						}
						catch (Exception exception)
						{
							errors.Add ("autoform.form: "+ exception.Message);
						}
					}
				}
				catch (Exception exception)
				{
					errors.Add ("autoform.form: "+ exception.Message);
				}
			}
			#endregion

			return errors;
		}
		#endregion
	}
}
