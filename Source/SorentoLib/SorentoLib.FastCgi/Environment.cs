// 
// Environment.cs
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SorentoLib.FastCgi
{
	public class Environment
	{
		#region Static Fields
		private static Regex ExpBoundary = new Regex(@"boundary=(?<boundary>.*)", RegexOptions.Compiled);
		#endregion
		
		#region Private Fields
		private IDictionary<string, string> _parameters;
		#endregion

		#region Public Fields
		public string GatewayInterface
		{
			get
			{
				if (this._parameters.ContainsKey("GATEWAY_INTERFACE"))
				{
					return this._parameters["GATEWAY_INTERFACE"];
				}
				return string.Empty;
			}
		}

		public string RequestMethod
		{
			get
			{
				if (this._parameters.ContainsKey("REQUEST_METHOD"))
				{
					return this._parameters["REQUEST_METHOD"];
				}
				return string.Empty;
			}
		}

		public string AuthType
		{
			get
			{
				if (this._parameters.ContainsKey("AUTH_TYPE"))
				{
					return this._parameters["AUTH_TYPE"];
				}
				return string.Empty;
			}
		}

		public string QueryString
		{
			get
			{
				if (this._parameters.ContainsKey("QUERY_STRING"))
				{
					return this._parameters["QUERY_STRING"];
				}
				return string.Empty;
			}
		}

		public string RedirectQueryString
		{
			get
			{
				if (this._parameters.ContainsKey("REDIRECT_QUERY_STRING"))
				{
					return this._parameters["REDIRECT_QUERY_STRING"];
				}
				return string.Empty;
			}
		}


		public string PathInfo
		{
			get
			{
				if (this._parameters.ContainsKey("PATH_INFO"))
				{
					return this._parameters["PATH_INFO"];
				}
				return string.Empty;
			}
		}

		public string PathTranslated
		{
			get
			{
				if (this._parameters.ContainsKey("PATH_TRANSLATED"))
				{
					return this._parameters["PATH_TRANSLATED"];
				}
				return string.Empty;
			}
		}

		public string ScriptName
		{
			get
			{
				if (this._parameters.ContainsKey("SCRIPT_NAME"))
				{
					return this._parameters["SCRIPT_NAME"];
				}
				return string.Empty;
			}
		}

		public string ServerAddress
		{
			get
			{
				if (this._parameters.ContainsKey("SERVER_ADDR"))
				{
					return this._parameters["SERVER_ADDR"];
				}
				return string.Empty;
			}
		}

		public string ServerName
		{
			get
			{
				if (this._parameters.ContainsKey("SERVER_NAME"))
				{
					return this._parameters["SERVER_NAME"];
				}
				return string.Empty;
			}
		}

		public string ServerPort
		{
			get
			{
				if (this._parameters.ContainsKey("SERVER_PORT"))
				{
					return this._parameters["SERVER_PORT"];
				}
				return string.Empty;
			}
		}

		public string ServerProtocol
		{
			get
			{
				if (this._parameters.ContainsKey("SERVER_PROTOCOL"))
				{
					return this._parameters["SERVER_PROTOCOL"];
				}
				return string.Empty;
			}
		}

		public string ServerSoftware
		{
			get
			{
				if (this._parameters.ContainsKey("SERVER_SOFTWARE"))
				{
					return this._parameters["SERVER_SOFTWARE"];
				}
				return string.Empty;
			}
		}

		public string ServerSignature
		{
			get
			{
				if (this._parameters.ContainsKey("SERVER_SIGNATURE"))
				{
					return this._parameters["SERVER_SIGNATURE"];
				}
				return string.Empty;
			}
		}

		public string ServerAdmin
		{
			get
			{
				if (this._parameters.ContainsKey("SERVER_ADMIN"))
				{
					return this._parameters["SERVER_ADMIN"];
				}
				return string.Empty;
			}
		}

		public string RemoteHost
		{
			get
			{
				if (this._parameters.ContainsKey("REMOTE_HOST"))
				{
					return this._parameters["REMOTE_HOST"];
				}
				return string.Empty;
			}
		}

		public string RemoteAddress
		{
			get
			{
				if (this._parameters.ContainsKey("REMOTE_ADDR"))
				{
					return this._parameters["REMOTE_ADDR"];
				}
				return string.Empty;
			}
		}

		public string RemoteIdent
		{
			get
			{
				if (this._parameters.ContainsKey("REMOTE_IDENT"))
				{
					return this._parameters["REMOTE_IDENT"];
				}
				return string.Empty;
			}
		}

		public string RemoteUser

		{
			get
			{
				if (this._parameters.ContainsKey("REMOTE_USER"))
				{
					return this._parameters["REMOTE_USER"];
				}
				return string.Empty;
			}
		}

		public string ContentType
		{
			get
			{
				if (this._parameters.ContainsKey("CONTENT_TYPE"))
				{
					string[] split = this._parameters["CONTENT_TYPE"].Split(";".ToCharArray());
					return split[0];
				}
				return string.Empty;
			}
		}

		public string ContentTypeMultipartBoundary
		{
			get
			{
				if (this._parameters.ContainsKey("CONTENT_TYPE"))
				{
					Match match = SorentoLib.FastCgi.Environment.ExpBoundary.Match(this._parameters["CONTENT_TYPE"]);
					if (match.Success)
					{
						return match.Groups["boundary"].Value;
					}
				}
				return string.Empty;
			}
		}

		public string ContentLength
		{
			get
			{
				if (this._parameters.ContainsKey("CONTENT_LENGTH"))
				{
					return this._parameters["CONTENT_LENGTH"];
				}
				return string.Empty;
			}
		}

		public string HttpCookie
		{
			get
			{
				if (this._parameters.ContainsKey("HTTP_COOKIE"))
				{
					return this._parameters["HTTP_COOKIE"];
				}
				return string.Empty;
			}
		}

		public string HttpAccept
		{
			get
			{ 				
				if (this._parameters.ContainsKey("HTTP_ACCEPT"))
				{
					return this._parameters["HTTP_ACCEPT"];
				}
				return string.Empty;
			}
		}

		public string HttpUserAgent
		{
			get
			{
				if (this._parameters.ContainsKey("HTTP_USER_AGENT"))
				{
					return this._parameters["HTTP_USER_AGENT"];
				}
				return string.Empty;
			}
		}

		public string HttpKeepAlive
		{
			get
			{
				if (this._parameters.ContainsKey("HTTP_KEEP_ALIVE"))
				{
					return this._parameters["HTTP_KEEP_ALIVE"];
				}
				return string.Empty;
			}
		}

		public string HttpAcceptLanguage
		{
			get
			{
				if (this._parameters.ContainsKey("HTTP_ACCEPT_LANGUAGE"))
				{
					return this._parameters["HTTP_ACCEPT_LANGUAGE"];
				}
				return string.Empty;
			}
		}

		public string HttpAcceptCharset
		{
			get
			{
				if (this._parameters.ContainsKey("HTTP_ACCEPT_CHARSET"))
				{
					return this._parameters["HTTP_ACCEPT_CHARSET"];
				}
				return string.Empty;
			}
		}

		public string HttpAcceptEncoding
		{
			get
			{
				if (this._parameters.ContainsKey("HTTP_ACCEPT_ENCODING"))
				{
					return this._parameters["HTTP_ACCEPT_ENCODING"];
				}
				return string.Empty;
			}
		}

		public string HttpHost
		{
			get
			{
				if (this._parameters.ContainsKey("HTTP_HOST"))
				{
					return this._parameters["HTTP_HOST"];
				}
				return string.Empty;
			}
		}

		public string HttpConnection
		{
			get
			{
				if (this._parameters.ContainsKey("HTTP_CONNECTION"))
				{
					return this._parameters["HTTP_CONNECTION"];
				}
				return string.Empty;
			}
		}

		public string HttpReferer
		{
			get
			{
				if (this._parameters.ContainsKey("HTTP_REFERER"))
				{
					return this._parameters["HTTP_REFERER"];
				}
				return string.Empty;
			}
		}

		public string DocumentRoot
		{
			get
			{
				if (this._parameters.ContainsKey("DOCUMENT_ROOT"))
				{
					return this._parameters["DOCUMENT_ROOT"];
				}
				return string.Empty;
			}
		}

		public string ScriptFilename
		{
			get
			{
				if (this._parameters.ContainsKey("SCRIPT_FILENAME"))
				{
					return this._parameters["SCRIPT_FILENAME"];
				}
				return string.Empty;
			}
		}

		public string RedirectStatus
		{
			get
			{
				if (this._parameters.ContainsKey("REDIRECT_STATUS"))
				{
					return this._parameters["REDIRECT_STATUS"];
				}
				return string.Empty;
			}
		}

		public string RedirectUrl
		{
			get
			{
				if (this._parameters.ContainsKey("REDIRECT_URL"))
				{
					return this._parameters["REDIRECT_URL"];
				}
				return string.Empty;
			}
		}

		public string RequestUri
		{
			get
			{
				if (this._parameters.ContainsKey("REQUEST_URI"))
				{
					return this._parameters["REQUEST_URI"];
				}
				return string.Empty;
			}
		}
		#endregion

		#region Constructor
		public Environment ()
		{
			this._parameters = new Dictionary<string, string>();

			this._parameters.Add("GATEWAY_INTERFACE", System.Environment.GetEnvironmentVariable("GATEWAY_INTERFACE"));
			this._parameters.Add("REQUEST_METHOD", System.Environment.GetEnvironmentVariable("REQUEST_METHOD"));
			this._parameters.Add("AUTH_TYPE", System.Environment.GetEnvironmentVariable("AUTH_TYPE"));
			this._parameters.Add("QUERY_STRING", System.Environment.GetEnvironmentVariable("QUERY_STRING"));
			this._parameters.Add("PATH_INFO", System.Environment.GetEnvironmentVariable("PATH_INFO"));
			this._parameters.Add("PATH_TRANSLATED", System.Environment.GetEnvironmentVariable("PATH_TRANSLATED"));
			this._parameters.Add("SCRIPT_NAME", System.Environment.GetEnvironmentVariable("SCRIPT_NAME"));
			this._parameters.Add("SERVER_NAME", System.Environment.GetEnvironmentVariable("SERVER_NAME"));
			this._parameters.Add("SERVER_PORT", System.Environment.GetEnvironmentVariable("SERVER_PORT"));
			this._parameters.Add("SERVER_PROTOCOL", System.Environment.GetEnvironmentVariable("SERVER_PROTOCOL"));
			this._parameters.Add("SERVER_SOFTWARE", System.Environment.GetEnvironmentVariable("SERVER_SOFTWARE"));
			this._parameters.Add("REMOTE_HOST", System.Environment.GetEnvironmentVariable("REMOTE_HOST"));
			this._parameters.Add("REMOTE_ADDR", System.Environment.GetEnvironmentVariable("REMOTE_ADDR"));
			this._parameters.Add("REMOTE_IDENT", System.Environment.GetEnvironmentVariable("REMOTE_IDENT"));
			this._parameters.Add("REMOTE_USER",  System.Environment.GetEnvironmentVariable("REMOTE_USER"));
			this._parameters.Add("CONTENT_TYPE", System.Environment.GetEnvironmentVariable("CONTENT_TYPE"));
			this._parameters.Add("CONTENT_LENGTH", System.Environment.GetEnvironmentVariable("CONTENT_LENGTH"));
			this._parameters.Add("HTTP_COOKIE", System.Environment.GetEnvironmentVariable("HTTP_COOKIE"));
			this._parameters.Add("HTTP_ACCEPT", System.Environment.GetEnvironmentVariable("HTTP_ACCEPT"));
			this._parameters.Add("HTTP_USER_AGENT", System.Environment.GetEnvironmentVariable("HTTP_USER_AGENT"));
		}

		public Environment (IDictionary<string, string> Parameters)
		{
			this._parameters = Parameters;
		}
		#endregion
	}
}
