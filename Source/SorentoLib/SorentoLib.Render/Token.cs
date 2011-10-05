//
// Token.cs
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SorentoLib.Render
{
	public class Token
	{
		#region Private Static Fields
		private static Regex ExpGender = new Regex(@"^\$", RegexOptions.Compiled);
		private static Regex ExpVariable = new Regex(@"^\$(?<set>[^ =]*) *= *""(?<value>(\\.|[^""])*)""$|^\$((?<add>.*)\+\+)|^\$((?<sub>.*)\-\-)|^\$(?<get>[^ =\+\!\-\;]*) ?(?<options>.*)", RegexOptions.Compiled);
		private static Regex ExpStatement = new Regex(@"^\$?(?<head>[^-+ ]*) ?\(?((?<body>.*)\))?(?<add>\+\+)?(?<sub>\-\-)?(?<set>=)? *(?<value>.*)?", RegexOptions.Compiled);

//		private static Regex ExpOptions					= new Regex("(?<key>[^ ]+) {0,10}= {0,10}[\"|'](?<value>.*)[\"|']", RegexOptions.Compiled);
		private static Regex ExpCleanToken 				= new Regex(@"<\%(?<clean>.+?)\%>", RegexOptions.Compiled);
		#endregion

		#region Private Fields
		private string _raw = string.Empty;							
		private string _clean = string.Empty;
		private string _result 	= string.Empty;
		private string _content	= string.Empty;

		private bool _execute = false;
		private bool _succces = false;

		private string _name = string.Empty;
		private string _variablename = string.Empty;

		private Dictionary<string, string> _options	= new Dictionary<string, string>();	

		private object _data;
		#endregion

		#region Public Fields
		/// <value>
		/// Raw
		/// </value>
		public string Raw
		{
			get { return this._raw; }
		}	
		
		/// <value>
		/// Clean
		/// </value>
		public string Clean
		{
			get { return this._clean; }
		}	
		
		/// <value>
		/// Result
		/// </value>
		public string Result
		{
			get
			{
				if (this._result == null)
				{
					this._result = string.Empty;
				}

				return this._result;

			}
			set { this._result = value; }
		}	
		
		/// <value>
		/// Content
		/// </value>
		public string Content
		{
			get { return this._content; }
		}				
		
		/// <value>
		/// Success
		/// </value>
		public bool Success
		{
			get { return this._succces; }
		}	
		
		/// <value>
		/// Execute
		/// </value>
		public bool Execute
		{
			get { return this._execute; }
		}	
		
		/// <value>
		/// Name
		/// </value>
		public string Name
		{
			set { this._name = value; }
			get { return this._name; }
		}

		/// <value>
		/// VariableName
		/// </value>
		public string VariableName
		{
			set { this._variablename = value; }
			get { return this._variablename; }
		}	
		
		/// <value>
		/// Options
		/// </value>
		public Dictionary<string, string> Options
		{
			get { return this._options; }
		}		
		
		/// <value>
		/// Data
		/// </value>
		public object Data
		{
			get { return this._data; }
		}			
		#endregion

		#region Constructor
		/// <summary>
		/// Create new Token
		/// </summary>
		public Token(string rawtoken)
		{								
			this._raw = rawtoken;										
			Match match  = Token.ExpCleanToken.Match(rawtoken);
			this._clean = match.Groups["clean"].Value;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Parse
		/// </summary>
		public bool Parse(Template template)
		{
			// Defintions.
			bool success = false;

			Match statement = Token.ExpStatement.Match(this._clean);
			if (statement.Success)
			{
				switch (statement.Groups["head"].Value.ToLower())
				{
					#region If
					case "if":
						if (template.CurrentBlock.Loop == null)
						{
							// Since If was triggered, we need a new Block.
							Block block = new Block();

							// If parent Block is not being rendered then this Block wont be rendered either.
							if (!template.Blocks[template.Blocks.Count-1].Render)
							{
								block.Render = false;
							}

							// Render Block if needed.
							if (block.Render)
							{

								SorentoLib.Render.Condition condition = new SorentoLib.Render.Condition(template.Session, statement.Groups["body"].Value);
								if (condition.Result)
								{
									block.LastIfStatementWasTrue = true;
								}
								else
								{
									block.Render = false;
								}

								// CleanUp
								condition = null;
							}

							// Add Block to list.
							template.Blocks.Add(block);

							// Finish.
							success = true;
						}
					break;
					#endregion

					#region ElseIf
					case "elseif":
						if (template.CurrentBlock.Loop == null)
						{
							// If inital If or previous ElseIf was true, than we dont need dont need to render.
							if (template.Blocks[template.Blocks.Count-1].LastIfStatementWasTrue)
							{
								template.Blocks[template.Blocks.Count-1].Render = false;
							}
							else
							{
								SorentoLib.Render.Condition condition = new SorentoLib.Render.Condition(template.Session, statement.Groups["body"].Value);
								if (condition.Result)
								{
									template.Blocks[template.Blocks.Count-1].Render = true;
									template.Blocks[template.Blocks.Count-1].LastIfStatementWasTrue = true;
								}

								// CleanUp
								condition = null;
							}

							// Finish
							success = true;
						}
					break;
					#endregion

					#region Else
					case "else":
						if (template.CurrentBlock.Loop == null)
						{
							// If inital If or ElseIf was true, do not render.
							if (template.Blocks[template.Blocks.Count-2].Render)
							{
								if (template.Blocks[template.Blocks.Count-1].LastIfStatementWasTrue)
								{
									template.Blocks[template.Blocks.Count-1].Render = false;
								}
								else
								{
									template.Blocks[template.Blocks.Count-1].Render = true;
								}
							}

							// Finish.
							success = true;
						}
					break;
					#endregion

					#region Endif
					case "endif":
						if (template.CurrentBlock.Loop == null)
						{
							// Block depth has to be more than 0.
							if (template.Blocks.Count > 0)
							{
								// Remove current Block.
								template.Blocks.RemoveAt(template.Blocks.Count-1);
							}

							// Finish.
							success = true;
						}
					break;
					#endregion

					#region Foreach
					case "foreach":
						if (template.CurrentBlock.BeginLoop())
						{
							// Create BLOCK to hold FOREACH.
							template.AddBlock();
							template.CurrentBlock.Loop = new Foreach(statement.Groups["body"].Value);

							// Finish.
							success = true;
						}
					break;
					#endregion

					#region EndForeach
					case "endforeach":
						if (template.CurrentBlock.EndLoop(Enums.LoopType.Foreach))
						{
							// Create resolver
							SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver(template.Session, ((Foreach)template.CurrentBlock.Loop).Source);

							// Render FOREACH block.
							foreach (object item in (List<object>)resolver.Result)
							{
//								template.Session.Page.Variables.Set(, , item.GetType().Name.ToLower());
								template.Session.Page.Variables.Add(((Foreach)template.CurrentBlock.Loop).Destination, item);
								Template loop = new Template(template.Session, template.CurrentBlock.Loop.Content);
								loop.Render();
							}

							// Remove FOREACH block.
							template.RemoveBlock();

							// Cleanup
							resolver = null;

							// Finish
							success = true;
						}
					break;
					#endregion

					#region Default
					default:
						// TODO: this if should be removed.

						if (!success)
						{
							if (template.CurrentBlock.Loop == null)
							{
								if (statement.Groups["set"].Success)
								{
//Console.WriteLine(statement.Groups["head"].Value+"]");
//Console.WriteLine(statement.Groups["value"].Value+"]");
//									Variables.SetValue(template.Session, statement.Groups["head"].Value, Variables.ParseVariables(template.Session, statement.Groups["value"].Value));



									// Create resolver
									SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver(template.Session, statement.Groups["value"].Value);

									template.Session.Page.Variables.Add(statement.Groups["head"].Value, resolver.Result);

//									Console.WriteLine("type:");
//									Console.WriteLine(resolver.Result);

//									Variables.SetValue(template.Session, statement.Groups["head"].Value, resolver.Result);

									// CleanUp
									resolver = null;

									// Finish
									success = true;
								}
								else
								{
									// Create resolver
									SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver(template.Session, this._clean);

									// Return resolver results
									this._result = (string)resolver.Result;

									// Cleanup
									resolver = null;

									// Finish
									success = true;
								}
							}
						}
					break;
					#endregion
				}
			}

			// Escaping problem characters.
			this._raw = Regex.Replace(this._raw, @"\\", @"\\", RegexOptions.Compiled);	// \
			this._raw = Regex.Replace(this._raw, @"\(", @"\(", RegexOptions.Compiled);	// (
			this._raw = Regex.Replace(this._raw, @"\$", @"\$", RegexOptions.Compiled);	// $
			this._raw = Regex.Replace(this._raw, @"\)", @"\)", RegexOptions.Compiled);	// )
			this._raw = Regex.Replace(this._raw, @"\|", @"\|", RegexOptions.Compiled);	// |
			this._raw = Regex.Replace(this._raw, @"\+", @"\+", RegexOptions.Compiled);	// +

			// CleanUp
			template = null;
			statement = null;

			// Finish.
			this._succces = success;
			return success;
		}
		#endregion
	}
}
