//
// Template.cs
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2007 Rasmus Pedsersen
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
using Mono.CSharp;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

namespace SorentoLib.Render
{
	public class Template
	{
		#region Private Static Fields
		private static Regex ExpMatchToken = new Regex ("<\\%(.+?)\\%>", RegexOptions.Compiled);
		private static Regex ExpStatement = new Regex ("^\\$?(?<head>[^-+ ]*) ?((?<set>=) *(?<value>.*)?)?\\(?((?<body>.*)\\))?", RegexOptions.Compiled);
		private static Regex ExpMatchCleaner = new Regex ("<\\%((if.+?|elsif.+?|else|endif|foreach.+?|endforeach))\\%>", RegexOptions.Compiled);
		private static Regex ExpCommentLine = new Regex ("^#!", RegexOptions.Compiled);
		private static Regex ExpIsRoot = new Regex (@"^\/", RegexOptions.Compiled);
		#endregion

		#region Private Fields
		private SorentoLib.Session _session;
		private List<System.String> _content;
		private List<SorentoLib.Render.Block> _blocks;
		private string _filename;
		private string _path;
		private bool _endrender = false;

		private SorentoLib.Render.Block CurrentBlock
		{
			get
			{
				return this._blocks[this._blocks.Count - 1];
			}
		}

		private SorentoLib.Render.Block ParentBlock
		{
			get
			{
				return this._blocks[this._blocks.Count - 2];
			}
		}
		#endregion

		#region Public Fields
		public string Filename
		{
			get
			{
				return this._filename;
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}
		}
		#endregion

		#region Constructors
		public Template (SorentoLib.Session Session, string Filename)
		{
			this._session = Session;
			this._content = new List<System.String> ();
			this._blocks = new List<SorentoLib.Render.Block> ();

			this._filename = Filename;


			if (SorentoLib.Render.Template.ExpIsRoot.Match (this._filename).Success)
			{
				this._path = SorentoLib.Services.Config.Get<string> ("core", "pathcontent");
			}
			else
			{
				this._path = SorentoLib.Services.Config.Get<string> ("core", "pathcontent") + "/" + System.IO.Path.GetDirectoryName (Session.Request.Environment.RequestUri) + "/";
			}

			if (this._filename == string.Empty)
			{
				this._path = SorentoLib.Services.Config.Get<string> ("core", "pathcontent") + System.IO.Path.GetDirectoryName (Session.Request.Environment.RequestUri) +"/";
				this._filename = System.IO.Path.GetFileName (Session.Request.Environment.RequestUri);

				if (this._filename == string.Empty)
				{
					this._filename = "index";
				}
			}

			this._filename = this._path + this._filename;

			if (System.IO.Path.GetExtension (this._filename) != ".stpl")
			{
				this._filename += ".stpl";
			}

			if (File.Exists (this._filename))
			{
				if (SorentoLib.Services.Cache.Enabled)
				{
					if (SorentoLib.Services.Cache.Exists (this._filename))
					{
						this._content = SorentoLib.Services.Cache.Get (this._filename);
					}
					else
					{
						this._content = SorentoLib.Services.Cache.Add (this._filename);
					}
				}
				else
				{
					this._content = SorentoLib.Render.Template.Read (this._filename);
				}
			}
			else
			{
//				SorentoLib.Services.Events.InvokeTemplateRenderFailed(this._session, this);
//				SorentoLib.Services.Logging.LogWarning("Template was not found : "+ this._filename);

				throw new Exception (string.Format (Strings.Exception.RenderTemplateLoad, this._filename));
			}
		}

		public Template (SorentoLib.Session Session, List<string> Content)
		{
			this._session = Session;
			this._content = Content;
			this._blocks = new List<SorentoLib.Render.Block> ();
		}
		#endregion

		#region Public Methods
		public void Render ()
		{
			foreach (SorentoLib.Addins.IPlaceholder iplaceholder in AddinManager.GetExtensionObjects (typeof (SorentoLib.Addins.IPlaceholder)))
			{
				foreach (Placeholder placeholder in iplaceholder.Get (this._session))
				{
					for (int i = 0; i < this._content.Count; i++)
					{
						this._content[i] = this._content[i].Replace (placeholder.Tag, placeholder.Content);
					}
				}
			}

			this.AddBlock ();

			foreach (string line in this._content)
			{
				string currentline = line;

				MatchCollection tokens = SorentoLib.Render.Template.ExpMatchToken.Matches (currentline);
				for (int token = 0; token < tokens.Count; token++)
				{
					currentline = this.Parser (currentline, tokens[token].Groups[1].Value);
				}

				if (this._endrender)
				{
					break;
				}
				
				// If Block is building loop, then we add the line to the current loop and continue to next line.
				if (this.CurrentBlock.InLoop)
				{
					this.CurrentBlock.Loop.Content.Add (currentline);
					continue;
				}
				
				// Line has been rendered, so we add it to the Page.
				if (this.CurrentBlock.Render)
				{
					this._session.Page.Lines.Add (currentline);
				}
			}
		}
		#endregion

		#region Private Methods
		private System.String Parser (string Line, string Token)
		{
			string result = string.Empty;

//			Console.WriteLine (Token);

			Match statement = Template.ExpStatement.Match (Token);
			if (statement.Success)
			{
				switch (statement.Groups["head"].Value.ToLower ())
				{
					#region If
					case "if":
						if (!this.CurrentBlock.InLoop)
						{
							// Since If was triggered, we need a new Block.
							SorentoLib.Render.Block block = new SorentoLib.Render.Block ();
							
							// If parent Block is not being rendered then this Block wont be rendered either.
							if (!this.CurrentBlock.Render)
							{
								block.Render = false;
							}
							
							// Render Block if needed.
							if (block.Render)
							{
								if (Condition.Evaluate (this._session, statement.Groups["body"].Value))
								{
									block.LastIfStatementWasTrue = true;
								}
								else
								{
									block.Render = false;
								}
							}

							this._blocks.Add (block);
						}
						break;
					#endregion
					
					#region ElseIf
					case "elseif":
						if (!this.CurrentBlock.InLoop)
						{
							// If inital If or previous ElseIf was true, than we dont need dont need to render.
							if (this.CurrentBlock.LastIfStatementWasTrue)
							{
								this.CurrentBlock.Render = false;
							}

							else
							{
								if (Condition.Evaluate (this._session, statement.Groups["body"].Value))
								{
									this.CurrentBlock.Render = true;
									this.CurrentBlock.LastIfStatementWasTrue = true;
								}
							}
						}
						break;
					#endregion
					
					#region Else
					case "else":
						if (!this.CurrentBlock.InLoop)
						{
							// If inital If or ElseIf was true, do not render.
							if (this.ParentBlock.Render)
							{
								if (this.CurrentBlock.LastIfStatementWasTrue)
								{
									this.CurrentBlock.Render = false;
								}

								else
								{
									this.CurrentBlock.Render = true;
								}
							}
						}
						break;
					#endregion
					
					#region Endif
					case "endif":
						if (!this.CurrentBlock.InLoop)
						{
							this.RemoveBlock ();
						}

						break;
						#endregion
					
					#region Foreach
					case "foreach":
						if ((this.CurrentBlock.BeginLoop ()) && (this.CurrentBlock.Render))
						{
							this.AddBlock ();
							this.CurrentBlock.Loop = new SorentoLib.Render.Foreach (statement.Groups["body"].Value);
							// TODO: this should probally be dealt with.
							Line = string.Empty;
						}
						break;
					#endregion

					#region EndForeach
					case "endforeach":
						if ((this.CurrentBlock.EndLoop (Enums.LoopType.Foreach)) && (this.CurrentBlock.Render))
						{
							SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver (this._session);
							resolver.Parse(((Foreach)this.CurrentBlock.Loop).Source);

//							Console.WriteLine (resolver.Result.GetType ().IsArray);
							Console.WriteLine (resolver.Result.GetType ().Name);

							switch (resolver.Result.GetType ().Name.ToLower ())
							{
								#region List
								case "list`1":
									MethodInfo methodInfo = typeof(SorentoLib.Render.Variables).GetMethod("ConvertToListObjectNew", System.Reflection.BindingFlags.Static | BindingFlags.Public);
									MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(new Type[] { resolver.Result.GetType ().GetGenericArguments()[0] });
									List<object> returnvalue = (List<object>)genericMethodInfo.Invoke(null, new object[] { resolver.Result });

									foreach (System.Object item in (System.Collections.Generic.List<System.Object>)returnvalue)
									{
										this._session.Page.Variables.Add (((SorentoLib.Render.Foreach)this.CurrentBlock.Loop).Destination, item);
										Template loop = new Template (this._session, this.CurrentBlock.Loop.Content);
										loop.Render ();
									}

									break;

								case "string[]":
									foreach (string item in (string[])resolver.Result)
									{
										this._session.Page.Variables.Add (((SorentoLib.Render.Foreach)this.CurrentBlock.Loop).Destination, item);
										Template loop = new Template (this._session, this.CurrentBlock.Loop.Content);
										loop.Render ();
									}
									break;
								#endregion
							}


							this.RemoveBlock ();

							resolver = null;
						}
						break;
					#endregion

					#region Write
					case "write":
						// TODO: This probally needs to be done differently.
						try
						{
							if ((!this.CurrentBlock.InLoop) && (this.CurrentBlock.Render))
							{
								SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver (this._session);
								resolver.Parse (statement.Groups["body"].Value);
								result = resolver.Result.ToString ();
							}
						}
						catch
						{}
						break;
					#endregion

					#region Include
					case "include":
						if ((!this.CurrentBlock.InLoop) && (this.CurrentBlock.Render))
						{
							SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver (this._session);
							resolver.Parse (statement.Groups["body"].Value);

							SorentoLib.Render.Template include;
							include = new SorentoLib.Render.Template (this._session, (string)resolver.Result);
//							if (((string)resolver.Result).Substring(0, 2) == "./")
//							{
//								include = new SorentoLib.Render.Template (this._session, ((string)resolver.Result).Substring (2, ((string)resolver.Result).Length-2));
//							}
//							else
//							{
//								include = new SorentoLib.Render.Template (this._session, Path.GetDirectoryName (this._session.Request.Environment.RequestUri) + "/" + (string)resolver.Result);
//							}

							include.Render ();

							resolver = null;
							include = null;
						}
						break;
					#endregion

					#region Render
					case "render":
						if ((!this.CurrentBlock.InLoop) && (this.CurrentBlock.Render))
						{
							SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver (this._session);
							resolver.Parse (statement.Groups["body"].Value);

							this._session.Page.Clear();
							Template redirect = new Template(this._session, (string)resolver.Result);
							redirect.Render ();

							this._endrender = true;

							resolver = null;
						}
						break;
					#endregion

					#region Redirect
					case "redirect":
						if ((!this.CurrentBlock.InLoop) && (this.CurrentBlock.Render))
						{


							SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver (this._session);
							resolver.Parse (statement.Groups["body"].Value);

							// TODO: This probally needs to be fixed.
//							this._session.Page.Clear();


							this._session.Page.Redirect = (string)resolver.Result;



//							Console.WriteLine ("dczdf");

							this._session.Page.Lines.Add(@"<meta HTTP-EQUIV=""REFRESH"" content=""0; url="+ (string)resolver.Result +@""">");

//							HTTP/1.1 301 Moved Permanently" );
//Header( "Location: http://www.new-url.com

							this._endrender = true;

							resolver = null;
						}
						break;
					#endregion

					#region Default
					default:
						if ((!this.CurrentBlock.InLoop) && (this.CurrentBlock.Render == true))
						{
							if (statement.Groups["set"].Success)
							{
//								Console.WriteLine (statement.Groups["value"].Value);
								SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver (this._session);
								resolver.Parse(statement.Groups["value"].Value);

								this._session.Page.Variables.Add (statement.Groups["head"].Value, resolver.Result);
								resolver = null;
							}
							else
							{
								SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver (this._session);
								resolver.Parse(Token);

								resolver = null;
							}
						}
						break;
					#endregion
				}
			}
			
			// Escaping problem characters.
			Token = Regex.Replace (Token, "\\\\", "\\\\", RegexOptions.Compiled);
			Token = Regex.Replace (Token, "\\(", "\\(", RegexOptions.Compiled);
			Token = Regex.Replace (Token, "\\$", "\\$", RegexOptions.Compiled);
			Token = Regex.Replace (Token, "\\)", "\\)", RegexOptions.Compiled);
			Token = Regex.Replace (Token, "\\|", "\\|", RegexOptions.Compiled);
			Token = Regex.Replace (Token, "\\+", "\\+", RegexOptions.Compiled);
			Token = Regex.Replace (Token, "\\[", "\\[", RegexOptions.Compiled);
			Token = Regex.Replace (Token, "\\]", "\\]", RegexOptions.Compiled);

			// Render Token.
			if ((!this.CurrentBlock.InLoop) && (this.CurrentBlock.Render))
			{
				Line = Regex.Replace (Line, "<%"+ Token +"%>", result);
			}

			statement = null;
			
			return Line;
		}

		private void RemoveBlock ()
		{
			if (this._blocks.Count > 0)
			{
				// Remove current Block.
				this._blocks.RemoveAt (this._blocks.Count - 1);
			}
		}

		private void AddBlock ()
		{
			SorentoLib.Render.Block block = new SorentoLib.Render.Block ();
			this._blocks.Add (block);
		}
		#endregion

		#region Public Static Methods
		public static List<string> Read (string filename)
		{
			List<string> content = new List<string> ();

			StreamReader template = new StreamReader (filename, Encoding.GetEncoding (SorentoLib.Services.Config.Get<string> ("core", "contentencoding")));
			string readline = string.Empty;
			while ((readline = template.ReadLine ()) != null)
			{
				// Lines starting with '#!' will be discared as comments.
				if (SorentoLib.Render.Template.ExpCommentLine.Match (readline).Success)
				{
					continue;
				}

				// Lines with zero length will be discared.
				if (readline.Length < 1)
				{
					continue;
				}

				// Lex tokens.
				MatchCollection tokens = SorentoLib.Render.Template.ExpMatchCleaner.Matches (readline);
				if (tokens.Count > 0)
				{
					int index = 0;
					for (int token = 0; token < tokens.Count; token++)
					{
						readline = readline.Insert (tokens[token].Index + index, "\n");
						readline = readline.Insert (tokens[token].Index + index + tokens[token].Length + 1, "\n");
						index += 2;
					}

					foreach (string line in readline.Split ("\n".ToCharArray()))
					{
						if (line != string.Empty)
						{
							content.Add (line);
						}
					}
				}
				else
				{
					if (readline != string.Empty)
					{
						content.Add (readline);
					}
				}
			}
			template.Close ();
			template.Dispose ();


			template = null;
			readline = null;

			return content;
		}
		#endregion
	}
}
