//
// Responder.cs: FastCGI service responder.
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2009 Rasmus Pedsersen
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;
using Mono.FastCgi;

using SNDK;

namespace SorentoLib.FastCgi
{
	public class Responder : MarshalByRefObject, IResponder
	{
		static int test = 0;
		private static Regex ExpMatchIsAdministrationUrl = new Regex (@"^\/administration/", RegexOptions.Compiled);

		#region Private Fields
		private ResponderRequest _request;


		#endregion

		#region Public Fields
		public ResponderRequest Request
		{
			get
			{
				return this._request;
			}
		}

		public void SendOutput (string text, System.Text.Encoding encoding)
		{
			this._request.SendOutput (text, encoding);
		}

		public void SendOutput (byte[] data, int length)
		{
			this._request.SendOutput (data, length);
		}

		public string GetParameter (string name)
		{
			return this._request.GetParameter (name);
		}

		public IDictionary<string, string> GetParameters ()
		{
			return this._request.GetParameters ();
		}

		public int RequestID
		{
			get
			{
				return this._request.RequestID;
			}
		}

		public byte[] InputData
		{
			get
			{
				return this._request.InputData;
			}
		}

		public void CompleteRequest (int appStatus)
		{
			this._request.CompleteRequest (appStatus, ProtocolStatus.RequestComplete);
		}

		public bool IsConnected
		{
			get
			{
				return this._request.IsConnected;
			}
		}

		public string HostName
		{
			get
			{
				return this._request.HostName;
			}
		}

		public int PortNumber
		{
			get
			{
				return this._request.PortNumber;
			}
		}

		public string Path
		{
			get
			{
				return this._request.Path;
			}
		}

		public string PhysicalPath
		{
			get
			{
				return this._request.PhysicalPath;
			}
		}
		#endregion

		#region Constructors
		public Responder (ResponderRequest request)
		{
			this._request = request;
		}
		#endregion

		#region Public Methods
		public int Process ()
		{
			Tools.Timer timer = new Tools.Timer ();
			timer.Start ();

			Session session = new Session (this._request.GetParameters (), this._request.InputData);
			session.Responder = this;

//			SorentoLib.Session session = new SorentoLib.Session (this._request.GetParameters (), this._request.InputData);
//			session.Update ();
//			session.Responder = this;



			try
			{

				switch (session.Request.QueryJar.Get ("cmd").Value.ToLower ())
				{
					#region Page
					case "page":
						// If site is offline, show offline page.
						if (!SorentoLib.Services.Config.Get<bool> ("core", "enabled"))
						{
							if (!ExpMatchIsAdministrationUrl.Match (session.Request.QueryJar.Get ("cmd.page").Value).Success)
							{
								Query query = new Query ();
								query.Name = "cmd.page";
								query.Value = SorentoLib.Services.Config.Get<string> ("core", "offlineurl");

								session.Request.QueryJar.Add (query);
							}
						}

//						if (SorentoLib.Services.Config.Get<bool> ("core", "bootstrap"))
//						{
//							Query query = new Query ();
//							query.Name = "cmd.page";
//							query.Value = "/administration/bootstrap/"+ SorentoLib.Services.Config.Get<bool> ("core", "bootstraplevel");
//
//						}


						foreach (SorentoLib.Addins.IPageResponder pageresponder in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IPageResponder)))
						{
							if (pageresponder.Process (session))
							{
								break;
							}
						}
						break;
					#endregion

					#region Media
					case "media":
						foreach (SorentoLib.Addins.IMediaResponder mediaresponder in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IMediaResponder)))
						{
							mediaresponder.Process (session);
						}
						break;
					#endregion

					#region Function
					case "function":
						foreach (SorentoLib.Addins.IFunctionResponder functionresponder in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IFunctionResponder)))
						{
							functionresponder.Process (session);
						}
						break;
					#endregion

					#region Ajax
					case "ajax":
						foreach (SorentoLib.Addins.IAjaxResponder ajaxresponder in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IAjaxResponder)))
						{
							ajaxresponder.Process (session);
						}
						break;
					#endregion

					default:
						break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
//				#region Error Handling
//				if (session.Error.Exception)
//				{
//					this._request.SendOutputText ("Content-type: text/html; charset=UTF-8\n\n");
//					this._request.SendOutputText ("ErrorCode: " + session.Error.Id + "<br>");
//					this._request.SendOutputText ("ErrorText: " + session.Error.Text + "<br>");
					
//					Log.Write (Log.Type.Warn, "ErrorCode: " + session.Error.Id + " - " + session.Error.Text);
//				}
//				else
//				{
//					this._request.SendOutputText ("Content-type: text/html; charset=UTF-8\n\n");
//					this._request.SendOutputText (e.StackTrace.ToString ().Replace ("\n", "<br>"));
//
////					Log.Write (Log.Type.Warn, "Exception" + e.StackTrace.ToString ().Replace ("\n", " "));
//				}
//				#endregion
			}

			timer.Stop ();
			SorentoLib.Services.Logging.LogDebug("Request served in: "+ timer.Duration.TotalSeconds +" seconds.");
//			SorentoLib.Services.Logging.LogDebug("Files open: "+ SorentoLib.Runtime.CurrentlyOpenFiles);
			timer = null;

			return 0;
		}
		#endregion
	}
}
