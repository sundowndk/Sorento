//
// Function.cs
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
using System.Collections.Generic;

using SorentoLib;

namespace sCMS.Addin
{
	public class Function : SorentoLib.Addins.IFunction
	{
		#region Private Fields
		private List<string> _namespaces = new List<string> ();
		#endregion

		#region Constructor
		public Function ()
		{
			this._namespaces.Add ("scms");
		}
		#endregion

		#region Public Methods
		public bool IsProvided (string Namespace)
		{
			return this._namespaces.Exists (delegate (string o) {return (o == Namespace.ToLower ());});
		}

		public bool Process (SorentoLib.Session Session, string Fullname, string Method)
		{

			switch (Fullname.ToLower ())
			{
				#region Page
				case "scms.page":

					switch (Method.ToLower ())
					{
						case "imageupload":
							List<string> allowedfiletypes = new List<string> ();
							allowedfiletypes.Add ("image/jpeg");
							allowedfiletypes.Add ("image/png");
							allowedfiletypes.Add ("image/gif");


							if (allowedfiletypes.Contains (Session.Request.QueryJar.Get ("UploadImage").BinaryContentType))
							{
								SorentoLib.Media image = new SorentoLib.Media ("/media/content/"+ Guid.NewGuid ().ToString () +".jpg", Session.Request.QueryJar.Get ("UploadImage").BinaryData);
								image.Status = SorentoLib.Enums.MediaStatus.Public;
								image.Save ();

								

								SNDK.IO.CopyFile (image.DataPath, SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_pathpublicmedia) +"/administration/cache/thumbnails/"+ image.FileName);

								SorentoLib.MediaTransformation.Transform (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_pathpublicmedia) +"/administration/cache/thumbnails/"+ image.FileName, "data/scripts/media_image_thumbnail.xml");


								SorentoLib.MediaTransformation.Transform (image.DataPath, "data/scripts/test.xml");
//								SorentoLib.MediaTransformation.Transform (image, "data/scripts/test.xml");


//								SorentoLib.Media thumbnail = image.Clone ("administration/cache/thumbnails/"+ image.FileName);

//								SorentoLib.MediaTransformation.TransformWithScript ("data/scripts/media_image_thumbnail.xml", thumbnail);

//								MediaTransformation thumbnailer = new MediaTransformation (SorentoLib.Enums.MediaTransformationType.Image);

//								string xml = string.Empty;
//								foreach (string line in Toolbox.IO.ReadTextFile ("data/scripts/media_image_thumbnail.xml", Encoding.UTF8))
//								{
//									xml += line;
//								}

//								thumbnailer.Script = xml;
//								thumbnailer.Transform (thumbnail);

//								Media media2 = media1.Clone ("thumbs/"+ media1.FileName);
								//			Media media2 = media1.Clone (media1.Path);
								//			t1.Transform (media2);


//								Console.WriteLine (thumbnail.Path);

//								string script = SorentoLib.Services.Datastore.Get ("akvabase.graphics.profile.avatar", "resize");
//								bool test = Toolbox.Graphics.ParseJob (script, image.HardPath, image.HardPath);
//								Console.WriteLine (test);

//								if (test)
//								{
									Session.Page.Variables.Add ("mediaid", image.Id);
									Session.Page.Variables.Add ("mediasoftpath", image.Path);
									Session.Page.Variables.Add ("uploadsuccess", "true");
									return true;
//								}
//								else
//								{
//									Session.Page.Variables.Add ("uploadsuccess", "false");
//									return false;
//								}
							}

							Session.Page.Variables.Add ("uploadsuccess", "false");
							return true;

						default:
							return false;
					}
				#endregion
			}

			return false;
		}
		#endregion
	}
}
