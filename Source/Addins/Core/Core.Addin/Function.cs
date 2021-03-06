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

using SNDK;

using SorentoLib;

namespace Core.Addin
{
	public class Function : SorentoLib.Addins.IFunction
	{
		#region Private Fields
		private List<string> _namespaces = new List<string> ();
		#endregion

		#region Constructor
		public Function ()
		{
			this._namespaces.Add (typeof (SorentoLib.User).Namespace.ToLower ());
			this._namespaces.Add (typeof (SorentoLib.Media).Namespace.ToLower ());
			this._namespaces.Add (typeof (SorentoLib.Services.Snapshot).Namespace.ToLower ());
		}
		#endregion

		#region Public Methods
		public bool IsProvided (string Namespace)
		{
			return this._namespaces.Exists (delegate (string o) {return (o == Namespace.ToLower ());});
		}

		public bool Process (SorentoLib.Session Session, string Fullname, string Method)
		{
			bool result = false;

			switch (Fullname.ToLower ())
			{
				#region Session
				case "sorentolib.session":
				{
					switch (Method.ToLower ())
					{
						case "login":
						{
//							return Session.Login (Session.Request.QueryJar.Get ("username").Value, SorentoLib.Tools.StringHelper.ASCIIBytesToString (SorentoLib.Services.Crypto.Decrypt (SorentoLib.Tools.StringHelper.HexStringToBytes (Session.Request.QueryJar.Get("password").Value))));
							result = Session.SignIn (Session.Request.QueryJar.Get ("username").Value, Session.Request.QueryJar.Get("password").Value);
							break;
						}

						case "logout":
						{
							result = Session.SignOut ();
							break;
						}
					}
					break;
				}
				#endregion

				#region Media
				case "sorentolib.media":
				{
					switch (Method.ToLower ())
					{
						case "upload":
						{
							try
							{
								string filename = System.IO.Path.GetFileNameWithoutExtension (Session.Request.QueryJar.Get ("mediaupload").Value).Replace ("%", "_");
								string extension = System.IO.Path.GetExtension (Session.Request.QueryJar.Get ("mediaupload").Value).ToLower ();

								string path = Session.Request.QueryJar.Get ("path").Value;
								path = path.Replace ("%%GUID%%", Guid.NewGuid ().ToString ()).Replace ("%%FILENAME%%", filename).Replace ("%%EXTENSION%%", extension);

								SorentoLib.Enums.MediaType type = SNDK.Convert.StringToEnum<SorentoLib.Enums.MediaType> (Session.Request.QueryJar.Get ("mediatype").Value);
								string mimetypes = Session.Request.QueryJar.Get ("mimetypes").Value;

								if ((mimetypes.Contains (Session.Request.QueryJar.Get ("mediaupload").BinaryContentType)) || (mimetypes == string.Empty))
								{
									SorentoLib.Media media = new SorentoLib.Media (path, Session.Request.QueryJar.Get ("mediaupload").BinaryData);
									media.Type = type;
									media.Save ();

									foreach (string script in Session.Request.QueryJar.Get ("postuploadscripts").Value.Split (";".ToCharArray (), StringSplitOptions.RemoveEmptyEntries))
									{
										SorentoLib.MediaTransformation.Transform (media, SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_script) + script);
									}

									foreach (string mediatransformationid in Session.Request.QueryJar.Get ("mediatransformations").Value.Split (";".ToCharArray (), StringSplitOptions.RemoveEmptyEntries))
									{
										MediaTransformation mediatransformation = MediaTransformation.Load (new Guid (mediatransformationid));
										mediatransformation.Transform (media);
									}

									Session.Page.Variables.Add ("media", media);
									Session.Page.Variables.Add ("mediaid", media.Id);
									Session.Page.Variables.Add ("mediapath", media.Path);
									Session.Page.Variables.Add ("cmdsuccess", true);
									Session.Page.Variables.Add ("cmderrormessage", string.Empty);
								}
								else
								{
									SorentoLib.Services.Logging.LogError (string.Format (Strings.LogError.MediaUploadMimeType, Session.Request.QueryJar.Get ("mediaupload").BinaryContentType));

									throw new Exception (string.Format (Strings.LogError.MediaUploadMimeType, Session.Request.QueryJar.Get ("mediaupload").BinaryContentType));
								}

								result = true;
							}
							catch (Exception exception)
							{
								Session.Page.Variables.Add ("cmdsuccess", false);
								Session.Page.Variables.Add ("cmderrormessage", exception.Message);

								SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.MediaUploadException, exception.ToString ()));
							}
						}
						break;
					}
					break;
				}
				#endregion

				#region User
				case "sorentolib.user":
					break;
				#endregion

				// TODO: Implement Function[SorentoLib.UserGroup]
				#region UserGroup
				case "sorentolib.usergroup":
					break;
				#endregion

				#region SorentoLib.Services.Snapshot
				case "sorentolib.services.snapshot":
				{
					switch (Method.ToLower ())
					{
						case "upload":
						{
							if (Session.Request.QueryJar.Get ("upload").BinaryContentType == "application/zip")
							{
								FileStream filestream = File.Create (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.path_snapshot) + Session.Request.QueryJar.Get ("upload").Value);
								BinaryWriter binarywriter = new BinaryWriter(filestream);
								binarywriter.Write(Session.Request.QueryJar.Get ("upload").BinaryData);
								binarywriter.Close();
								filestream.Close();

								Session.Page.Variables.Add ("uploadsuccess", "true");
							}
							break;
						}
					}
					break;
				}
				#endregion
			}

			return result;
		}
		#endregion
	}
}
