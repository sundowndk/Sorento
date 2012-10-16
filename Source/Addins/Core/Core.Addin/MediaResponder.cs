//
// MediaResponder.cs
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

using Mono.Addins;

using SorentoLib;

namespace Core.Addin
{
	public class MediaResponder : SorentoLib.Addins.IMediaResponder
	{
		#region Constructor
		public MediaResponder ()
		{
		}
		#endregion

		#region Public Methods
		public void Process (SorentoLib.Session Session)
		{
			try
			{
				Media media = Media.Load (new Guid (Session.Request.QueryJar.Get ("cmd.mediaid").Value));

				Session.Responder.Request.SendOutputText ("Content-Type: "+ media.Mimetype +"\n");

				if (Session.Request.CookieJar.Exist ("download"))
				{
					if (Session.Request.QueryJar.Get ("download").Value == "1")
					{
						Session.Responder.Request.SendOutputText ("Content-Disposition: attachment; filename = "+ media.Filename +"\n");
					}
					else
					{
						Session.Responder.Request.SendOutputText ("Content-Disposition: filename = "+ media.Filename +"\n");
					}
				}
				else
				{
					Session.Responder.Request.SendOutputText ("Content-Disposition: filename = "+ media.Filename +"\n");
				}

//				if (Session.Request.QueryJar.Get ("download").Value == "1")
//				{
//					Session.Responder.Request.SendOutputText ("Content-Disposition: attachment; filename = "+ media.Filename +"\n");
//				}
//				else
//				{
//					Session.Responder.Request.SendOutputText ("Content-Disposition: filename = "+ media.Filename +"\n");
//				}



				// Open file from disk and server it.
				string filename = media.DataPath;

				FileStream  fs = File.OpenRead (filename);

				System.IO.FileInfo fi = new System.IO.FileInfo (filename);
				Session.Responder.Request.SendOutputText ("Content-Length: "+ fi.Length.ToString() +"\n\n");
					
				int offset=0;
				int buffersize = 163840;
				int bufferread = buffersize ;
				long remaining = fi.Length;
					
				while (remaining > 0)
				{
					if (remaining < buffersize)
					{
						bufferread = (int)remaining;
					}

					byte[] test = new byte[bufferread];
					int read = fs.Read(test, 0, bufferread);

					Session.Responder.Request.SendOutput (test);
					//Thread.Sleep(2);
					remaining = remaining - (long)read;
					offset += read;
				}

				fs.Close();
			}
			catch (Exception exception)
			{
				Console.WriteLine (exception);
			}


				

//				                                                                       this._request.SendOutputText("Content-Type: "+ media.Mimetype +"\n");
		}



//			Media media = new Media();
			//                                                                              if (media.Load(new Guid(session.Request.QueryJar.Get("mediaid").Value)))
			//                                                                              {
			//                                                                                      this._request.SendOutputText("Content-Type: "+ media.Mimetype +"\n");
			//
			//                                                                                      if (session.Request.QueryJar.Get("download").Value == "1")
			//                                                                                      {
			//                                                                                              this._request.SendOutputText("Content-Disposition: attachment; filename = "+ media.Filename +"\n");
			//                                                                                      }
			//                                                                                      else
			//                                                                                      {
			//                                                                                              this._request.SendOutputText("Content-Disposition: filename = "+ media.Filename +"\n");
			//                                                                                      }
			//
			//                                                                                      // TODO: This needs to be cleaned up.
			//
			//                                                                                      // Open file from disk and server it.
			//                                                                                      string filename = Toolbox.Global.Variables.Get<Toolbox.Config>("config").Get("core", "mediapath") + media.Id;
			//                                                                                      FileStream  fs = File.OpenRead(filename);
			//
			//                                                                                      System.IO.FileInfo fi = new System.IO.FileInfo("./media/"+media.Id);
			//                                                                                      this._request.SendOutputText("Content-Length: "+ fi.Length.ToString() +"\n\n");
			//
			//                                                                                      int offset=0;
			//                                                                                      int buffersize = 163840;
			//                                                                                      int bufferread = buffersize ;
			//                                                                                      long remaining = fi.Length;
			//
			//                                                                                      while (remaining > 0)
			//                                                                                      {
			//                                                                                              if (remaining < buffersize)
			//                                                                                              {
			//                                                                                                      bufferread = (int)remaining;
			//                                                                                              }
			//
			//                                                                                              byte[] test = new byte[bufferread];
			//                                                                                              int read = fs.Read(test, 0, bufferread);
			//
			//                                                                                              this._request.SendOutput(test);
			//                                                                                              //Thread.Sleep(2);
			//                                                                                              remaining = remaining - (long)read;
			//                                                                                              offset += read;
			//                                                                                      }
			//
			//                                                                                      fs.Close();


//			Session.Responder.SendOutput (SNDK.IO.FileToByteArray ("/home/rvp/Skrivebord/upload.pdf"), SNDK.IO.FileToByteArray ("/home/rvp/Skrivebord/upload.pdf").Length);
//			Session.Responder.SendOutput ()
//		}
		#endregion
	}
}
