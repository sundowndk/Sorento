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
		#region Private Fields
		private ResponderRequest _request;
		private bool _served;
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
			this._served = false;
		}
		#endregion

		#region Private Methods
		private void HTTP404 (Session session)
		{
			SorentoLib.Render.Template template = new SorentoLib.Render.Template (session, SNDK.IO.ReadTextFile ("data/404.stpl"));
			session.Page.Clear ();
			template.Render ();
			template = null;

			session.Responder.Request.SendOutputText (session.Page.Write (session));
		}

		private void HTTP500 (Session session)
		{
			SorentoLib.Render.Template template = new SorentoLib.Render.Template (session, SNDK.IO.ReadTextFile ("data/500.stpl"));
			session.Page.Clear ();
			template.Render ();
			template = null;

			session.Responder.Request.SendOutputText (session.Page.Write (session));
		}
		#endregion

		#region Public Methods
		public int Process ()
		{
			Tools.Timer timer = new Tools.Timer ();
			timer.Start ();

			Session session = new Session (this._request.GetParameters (), this._request.InputData);
			session.Responder = this;

			try
			{
				switch (session.Request.QueryJar.Get ("cmd").Value.ToLower ())
				{
					#region Page
					case "page":
					{
						foreach (SorentoLib.Addins.IPageResponder pageresponder in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IPageResponder)))
						{
							if (pageresponder.Process (session))
							{
								this._served = true;
								break;
							}
						}

						if (!this._served)
						{
							throw new SorentoLib.Exceptions.ResponderExceptionPageNotFound (string.Format (Strings.Exception.ResponderPageNotFound, session.Request.QueryJar.Get ("cmd.page").Value));
						}

						break;
					}
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
				}
			}
			catch (SorentoLib.Exceptions.ResponderExceptionPageNotFound exception)
			{
				session.Error = new Error (exception.Message, string.Empty);

				if (Services.Config.Get<bool> (Enums.ConfigKey.core_showexceptions))
				{
					SorentoLib.Render.Template template = new SorentoLib.Render.Template (session, SNDK.IO.ReadTextFile ("data/exception.stpl"));
					session.Page.Clear ();
					template.Render ();
					template = null;

					session.Responder.Request.SendOutputText (session.Page.Write (session));
				}
				else
				{
					HTTP404 (session);
				}
			}
			catch (SorentoLib.Exceptions.RenderException exception)
			{
				if (Services.Config.Get<bool> (Enums.ConfigKey.core_showexceptions))
				{
					session.Error = new Error (exception.Message, "in "+ exception.Filename +" line "+ exception.Line);

					SorentoLib.Render.Template template = new SorentoLib.Render.Template (session, SNDK.IO.ReadTextFile ("data/exception.stpl"));
					session.Page.Clear ();
					template.Render ();
					template = null;

					session.Responder.Request.SendOutputText (session.Page.Write (session));
				}
				else
				{
					HTTP500 (session);
				}
			}
			catch (Exception exception)
			{
				if (Services.Config.Get<bool> (Enums.ConfigKey.core_showexceptions))
				{
					session.Error = new Error (exception);

					SorentoLib.Render.Template template = new SorentoLib.Render.Template (session, SNDK.IO.ReadTextFile ("data/exception.stpl"));
					session.Page.Clear ();
					template.Render ();
					template = null;

					session.Responder.Request.SendOutputText (session.Page.Write (session));
				}
				else
				{
					HTTP500 (session);
				}
			}

			timer.Stop ();
			SorentoLib.Services.Logging.LogDebug("Request served in: "+ timer.Duration.TotalSeconds +" seconds.");
			timer = null;

			return 0;
		}
		#endregion
	}
}

//		private static Regex ExpMatchIsAdministrationUrl = new Regex (@"^\/administration/", RegexOptions.Compiled);

#region OLD
						// If site is offline, show offline page.
//						if (!SorentoLib.Services.Config.Get<bool> ("core", "enabled"))
//						{
//							if (!ExpMatchIsAdministrationUrl.Match (session.Request.QueryJar.Get ("cmd.page").Value).Success)
//							{
//								Query query = new Query ();
//								query.Name = "cmd.page";
//								query.Value = SorentoLib.Services.Config.Get<string> ("core", "offlineurl");
//
//								session.Request.QueryJar.Add (query);
//							}
//						}

#endregion