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

namespace Core.Resolvers.Addin
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
							result = Session.Login (Session.Request.QueryJar.Get ("username").Value, Session.Request.QueryJar.Get("password").Value);
							break;
						}

						case "logout":
						{
							result = Session.Logout ();
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
								string path = Session.Request.QueryJar.Get ("path").Value;

								string filename = System.IO.Path.GetFileNameWithoutExtension (Session.Request.QueryJar.Get ("upload").Value).Replace ("%", "_");
								string extension = System.IO.Path.GetExtension (Session.Request.QueryJar.Get ("upload").Value).ToLower ();

								path = path.Replace ("%%GUID%%", Guid.NewGuid ().ToString ()).Replace ("%%FILENAME%%", filename).Replace ("%%EXTENSION%%", extension);

								SorentoLib.Enums.MediaStatus status = SNDK.Convert.StringToEnum<SorentoLib.Enums.MediaStatus> (Session.Request.QueryJar.Get ("status").Value);
								string mediatransformations = Session.Request.QueryJar.Get ("mediatransformations").Value;
								string mimetypes = Session.Request.QueryJar.Get ("mimetypes").Value;
								string postuploadscript = Session.Request.QueryJar.Get ("postuploadscript").Value;

								if (mimetypes.Contains (Session.Request.QueryJar.Get ("upload").BinaryContentType))
								{
									SorentoLib.Media media = new SorentoLib.Media (path, Session.Request.QueryJar.Get ("upload").BinaryData);
									media.Status = status;
									media.Save ();

									if (postuploadscript != string.Empty)
									{
										SorentoLib.MediaTransformation.Transform (media.DataPath, SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.core_pathscript) + postuploadscript);
									}

									if (mediatransformations != string.Empty)
									{
										foreach (string mediatransformationid in mediatransformations.Split (";".ToCharArray (), StringSplitOptions.RemoveEmptyEntries))
										{
											MediaTransformation mediatransformation = MediaTransformation.Load (new Guid (mediatransformationid));
											mediatransformation.Transform (media);
										}
									}

									Session.Page.Variables.Add ("mediaid", media.Id);
									Session.Page.Variables.Add ("mediasoftpath", media.Path);

									Console.WriteLine (media.Path);
									Session.Page.Variables.Add ("uploadsuccess", "true");
								}
								else
								{
									Session.Page.Variables.Add ("uploadsuccess", "false");
									Session.Page.Variables.Add ("cmderrormessage", Strings.ErrorMessage.MediaUploadMimeType);
								}

								result = true;
							}
							catch (Exception exception)
							{
								
								Session.Page.Variables.Add ("uploadsuccess", "false");
								Session.Page.Variables.Add ("cmderrormessage", Strings.ErrorMessage.MediaUploadUnknown);
								SorentoLib.Services.Logging.LogDebug (string.Format (Strings.LogDebug.MediaUploadException, exception.ToString ()));
							}
						}
						break;
					}
					break;
				}
				#endregion

				// TODO: Implement Function[SorentoLib.User]
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
								FileStream filestream = File.Create (SorentoLib.Services.Config.Get<string> (SorentoLib.Enums.ConfigKey.snapshot_path) + Session.Request.QueryJar.Get ("upload").Value);
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
