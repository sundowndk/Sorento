//
// Request.cs: Handles CGI/FastCGI sessions.
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2009 Rasmus Pedersen
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
using System.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SorentoLib.FastCgi
{		
	public class Request
	{				
		#region Private Fields
		private SorentoLib.FastCgi.CookieJar _cookiejar;
		private SorentoLib.FastCgi.QueryJar _queryjar;
		private SorentoLib.FastCgi.Environment _environment;
		private byte[] _postdata;
		#endregion
		
		#region Public Fields
		public SorentoLib.FastCgi.CookieJar CookieJar
		{
			get
			{
				return this._cookiejar;
			}
		}
		
		public SorentoLib.FastCgi.QueryJar QueryJar
		{
			get
			{
				return this._queryjar;
			}
		}

		public SorentoLib.FastCgi.Environment Environment
		{
			get
			{
				return this._environment;
			}
		}


		
		
		public string HttpHeader(string charset)
		{			
			// Definitions
			string result = string.Empty;
								
			//result += "Content-Type: text/plain\n\n";
			result = this._cookiejar.Bake();
			result += "Content-type: text/html; charset="+ charset +"\n\n";		
			
			// Finish
			return result;
		}	
		
		public string HttpRedirect (string charset, string url, SorentoLib.Enums.RedirectType Type)
		{
			string result = string.Empty;

			result = this._cookiejar.Bake ();

			switch (Type)
			{
				case SorentoLib.Enums.RedirectType.HTTP301:
					result += "Status: HTTP/1.1 301 Moved Permanently\n\n";
					result += "Location: http://www.example.org/\n\n";
					break;

				case SorentoLib.Enums.RedirectType.Location:
					result += "Location: "+ url +"\n\n";
					break;
			}

			result += "Content-type: text/html; charset="+ charset +"\n\n";

			return result;
		}
		
		public string HttpHeader (string charset, string mimetype)
		{
			// Definitions
			string result = string.Empty;

			result = this._cookiejar.Bake ();
			result += "Content-type: " + mimetype + "; charset=" + charset + "\n\n";

			// Finish
			return result;
		}

		public string HttpHeader(string mimetype, string charset, bool includecookies)
		{			
			// Definitions
			string result = string.Empty;
						
			if (includecookies)
			{
				result = this._cookiejar.Bake();
			}
			result += "Content-type: "+ mimetype +"; charset="+ charset +"\n\n";
				
			// Finish
			return result;
		}		
		
		#endregion		
										
		#region Constructors	
		public Request ()
		{
			this._cookiejar = new SorentoLib.FastCgi.CookieJar();
			this._queryjar = new SorentoLib.FastCgi.QueryJar();
			this._environment = new SorentoLib.FastCgi.Environment();

			if (this._environment.RequestMethod == "POST")
			{
				Stream stream = Console.OpenStandardInput();								
				this._postdata = ReadBinary(stream, Int32.Parse(this._environment.ContentLength));
				stream.Close();
			}

			this.ParseCookies();					

			this.ParseHttpQueryString();
		}
				
		public Request(IDictionary<string, string> parameters, byte [] PostData)
		{
			this._cookiejar = new SorentoLib.FastCgi.CookieJar();
			this._queryjar = new SorentoLib.FastCgi.QueryJar();

			this._environment = new SorentoLib.FastCgi.Environment (parameters);
			this._postdata = PostData;

			this.ParseCookies();

			this.ParseHttpQueryString();	
		}		
		#endregion
		
		#region Private Methods
		/// <summary>
		/// ParseCookies
		/// </summary>
		private void ParseCookies()
		{									
			string httpcookie = this._environment.HttpCookie;

			if (httpcookie != null) 
			{ 				
				if (httpcookie.Length > 0) 
				{		
					if (httpcookie.Contains(";")) 
					{
						string[] cookies = Regex.Split(httpcookie, "; ");			
						for(int x = 0; x < cookies.Length; x++)
						{
							string[] cookiesplit = Regex.Split(cookies[x], "=");
							if (cookiesplit.Length > 1) 
							{
								Cookie cookie = new Cookie();
								cookie.Name = HttpUtility.UrlDecode(cookiesplit[0]);
								cookie.Value = HttpUtility.UrlDecode(cookiesplit[1]);										
								cookie.Path = "/";
								if (!this._cookiejar.Exist(cookie.Name))
								{
									this._cookiejar.Add (cookie);
								}
							}
						}
					} 
					else 
					{
						string[] cookiesplit = Regex.Split(httpcookie, "=");
						if (cookiesplit.Length > 1) 
						{
							Cookie cookie = new Cookie();
							cookie.Name = HttpUtility.UrlDecode(cookiesplit[0]);
							cookie.Value = HttpUtility.UrlDecode(cookiesplit[1]);		
							cookie.Path = "/";
							if (!this._cookiejar.Exist(cookie.Name))
							{
								this._cookiejar.Add (cookie);
							}
						}
					}
				}
			}
		}	
			
		/// <summary>
		/// ParseHttpQueryString
		/// </summary>
		private void ParseHttpQueryString()
		{						
			// Process POST requests.
			if (this._environment.RequestMethod == "POST")
			{
				long offset = 0;
				
				// Decode binary data. 								
				//string rawhttpquerystring = System.Text.Encoding.GetEncoding("UTF-8").GetString(this.postData, 0, Int32.Parse(this.HttpContentLength));	
				string rawhttpquerystring = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(this._postdata, 0, Int32.Parse(this._environment.ContentLength));
									
				
				// Before we begin, remove last boundry.
				rawhttpquerystring = rawhttpquerystring.Replace("--"+ this._environment.ContentTypeMultipartBoundary +"--\r\n","");
				// Split content by boundry.
				//foreach (string part in Regex.Split(rawhttpquerystring, @"[\s]*--" + this.HttpContentTypeMultipartBoundary + @"[\s.]*\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline)) 
				foreach (string part in Regex.Split(rawhttpquerystring, "--" + this._environment.ContentTypeMultipartBoundary+"\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline))
				{
					// Create new query.
					Query query = new Query();
					
					// We only process content that are larger than 0 in length.
					if (part.Length > 0)
					{											
						// Find content header.
						Match contentheader = Regex.Match(part, @"(.*)\r\n");
						
						// Calculate headersize.
						int headersize = contentheader.Groups[1].Value.Length+2;											
												
						// Split header.
						foreach (Match NextMatch in Regex.Matches(contentheader.Groups[1].Value, @"([^"";= ]*)=""([^""]*)"))
						{	
							// Get field name.
							if (NextMatch.Groups[1].Value == "name")
							{				
								query.Name = NextMatch.Groups[2].Value;								
							}
							
							// If we find FILENAME inside the header, this content might be binary data.
							if (NextMatch.Groups[1].Value == "filename")
							{																		
								// Mark query as binary.
								query.HasBinaryData = true;												
								
								// Find mime type.
								Match content = Regex.Match(part, @"\n\n?Content-Type: ([^\n]*)(\n.*)");								
								query.BinaryContentType = content.Groups[1].Value.Trim();
								
								// Calculate size of content-type header.
								headersize += content.Groups[1].Value.Length + 17;
								
								// Since this is binary, then we need a filename.
								query.Value = NextMatch.Groups[2].Value;								
																
								// Calculate length of binary data, and create a byte array to hold it.
								query.BinaryLength = part.Length-headersize-2;																					
								query.BinaryData = new byte[query.BinaryLength];
																
								// Copy binary data from source to byte array.
								long destinationoffset = 0;
								for (long sourceoffset = (offset + headersize); sourceoffset < ((offset + headersize) + query.BinaryLength); sourceoffset++)
								{																			
									query.BinaryData[destinationoffset] = this._postdata[sourceoffset];
									destinationoffset++;
								}													
							}																				
						}	
						
						// If content was not marked as binary, then process this content as a normal field. 
						if (!query.HasBinaryData)
						{	
							// Copy binary data from source to byte array.
							byte[] test = new byte[part.Length];
							long destinationoffset = 0;
							for (long sourceoffset = (offset); sourceoffset < ((offset) + part.Length); sourceoffset++)
							{																			
								test[destinationoffset] = this._postdata[sourceoffset];
								destinationoffset++;
							}	
							
							Match formdata = Regex.Match(System.Text.Encoding.GetEncoding("UTF-8").GetString(test), @"\n\r\n(.*)\r\n", RegexOptions.Singleline);
							
							query.Value = formdata.Groups[1].Value;							
						}					
					}
					
					// Store Query in Jar.				
					if (this._queryjar.Exist(query.Name))
					{
//						Query oldquery = this.QueryJar.Find(delegate(Query x) { return x.Name == query.Name; });
						Query oldquery = this._queryjar.Find (query.Name);
						if (oldquery.Values.Count > 0)
						{
							oldquery.Values.Add(query.Value);																
						}
						else 
						{
							oldquery.Values.Add(oldquery.Value);								
							oldquery.Values.Add(query.Value);																							
						}							
					}
					else
					{
						this._queryjar.Add(query);
					}					
					
					// Since we are done with this content, move the offset to the next boundry.
					offset += part.Length+this._environment.ContentTypeMultipartBoundary.Length+4;
				}
				
			}	
			else 
			{
				if (this._environment.QueryString != null)
				{
					if (this._environment.QueryString.Length > 0)
					{			
						if (this._environment.QueryString.Contains("&"))
						{
							string[] queries = Regex.Split(this._environment.QueryString, "&");
							for(int x = 0; x < queries.Length; x++)
							{
								string[] querysplit = Regex.Split(queries[x], "=");
								if (querysplit.Length > 1) 
								{					
									Query query = new Query();
									query.Name = HttpUtility.UrlDecode(querysplit[0]);
									query.Value = HttpUtility.UrlDecode(querysplit[1]);

									if (this._queryjar.Exist(query.Name))
									{
										Query oldquery = this._queryjar.Find (query.Name);
										if (oldquery.Values.Count > 0)
										{
											oldquery.Values.Add(query.Value);																
										}
										else 
										{
											oldquery.Values.Add(query.Value);																
											oldquery.Values.Add(oldquery.Value);								
										}							
									}
									else
									{
										this._queryjar.Add(query);
									}
								}		
							}
						} 
						else
						{
							string[] querysplit = Regex.Split(this._environment.QueryString, "=");
							if (querysplit.Length > 1) 
							{
								Query query = new Query();
								query.Name = HttpUtility.UrlDecode(querysplit[0]);
								query.Value = HttpUtility.UrlDecode(querysplit[1]);	
								if (!this._queryjar.Exist(query.Name))
								{
									this._queryjar.Add(query);
								}
							}
						}
					}
				}								
			}
		}	

		/// <summary>
		/// ReadBinary
		/// </summary>
		private static byte[] ReadBinary (Stream stream, int initialLength)
		{
    		// If we've been passed an unhelpful initial length, just
    		// use 32K.
    		if (initialLength < 1)
    		{
        		initialLength = 32768;
    		}
    
    		byte[] buffer = new byte[initialLength];
    		int read=0;
    
		    int chunk;
    		while ( (chunk = stream.Read(buffer, read, buffer.Length-read)) > 0)
    		{
        		read += chunk;
        
        		// If we've reached the end of our buffer, check to see if there's
        		// any more information
        		if (read == buffer.Length)
        		{
            		int nextByte = stream.ReadByte();
            
		            // End of stream? If so, we're done
            		if (nextByte==-1)
            		{
                		return buffer;
            		}
            
		            // Nope. Resize the buffer, put in the byte we've just
            		// read, and continue
            		byte[] newBuffer = new byte[buffer.Length*2];
            		Array.Copy(buffer, newBuffer, buffer.Length);
            		newBuffer[read]=(byte)nextByte;
            		buffer = newBuffer;
            		read++;							
        		}		
    		}
    		// Buffer is now too big. Shrink it.
    		byte[] ret = new byte[read];
    		Array.Copy(buffer, ret, read);
    		return ret;
		}				
		#endregion	
	}
}
