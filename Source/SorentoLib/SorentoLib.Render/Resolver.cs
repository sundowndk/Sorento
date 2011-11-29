//
// Resolver.cs
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (c) 2009 sundown
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
using Mono.CSharp;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

namespace SorentoLib.Render
{
	public class Resolver
	{
		#region REGEX SOURCE
		//System.Classname.Method ("test", $test)
		//System.Classname.Method ($variable.field)
		//System.Classname.Field
		//System.Classname.Field ("test")
		//$variable + "test" + $variable
		//$variable
		//$variable.field
		//$variable.method ("PARAM1")
		//$variable.method ($variable.field)
		//false
		//(10 + 10)

		// ISVARIABLE:
		// ((^\$[A-z|0-9|.]*)(( )*\(.*\))*$)

		// ISMETHOD:
//		^(([A-z|0-9])+\.)+([A-z|0-9])+ *(\((.)*\))?

		// ^(([A-z|0-9])+\.)+([A-z|0-9])+ *(\((.)*\))?$
		#endregion

		#region Private Static Fields
		private static Regex ExpIsVariable = new Regex (@"((^\$[A-z|0-9|.]*)$)");
		private static Regex ExpIsMethod = new Regex (@"^(([A-z|0-9])+\.)+([A-z|0-9])+ *(\((.)*\))?$");


//		private static Regex ExpIsString = new Regex(@"^\""|\+", RegexOptions.Compiled);
//		private static Regex ExpIsVariable = new Regex(@"^\$", RegexOptions.Compiled);


		private static Regex ExpIndexer = new Regex(@"\[(?<indexer>.*)\]", RegexOptions.Compiled);
		private static Regex ExpVariable = new Regex(@"\$(?<fullname>[^ (]*) *\(?((?<parameters>.*)\))?", RegexOptions.Compiled);
//		private static Regex ExpString = new Regex("\"(?<string>.*)\"", RegexOptions.Compiled);
//		private static Regex ExpString = new Regex("\"(?<string>.*)\"", RegexOptions.Compiled);
		private static Regex ExpMethod = new Regex(@"^(?<fullname>[^ (]*) *\(?((?<parameters>.*)\))?", RegexOptions.Compiled);
		private static Regex ExpParams = new Regex("(\"(?<word>[^\"]+|\"\")*\"|(?<word>[^ ,]*))", RegexOptions.Compiled);
		#endregion

		#region Private Fields
		private string _fullname;
		private string _namespace;
		private string _name;
		private string _variablename;
		private int _indexer;
		private string _method;
//		private List<object> _parameters;

		private Parameters _parameters;

		private object _result;
		private SorentoLib.Session _session;
		#endregion

		#region Public Fields
		public System.String Fullname
		{
			get
			{
				return this._fullname;
			}
		}

		public System.String Namespace
		{
			get
			{
				return this._namespace;
			}
		}

		public System.String Name
		{
			get
			{
				return this._name;
			}
		}

		public string Method
		{
			get
			{
				return this._method;
			}
		}

		public int Indexer
		{
			get
			{
				return this._indexer;
			}
		}

		public System.Object Result
		{
			get
			{
				return this._result;
			}
		}
		#endregion

		#region Constructor
		public Resolver (SorentoLib.Session session)
		{
			this._fullname = string.Empty;
			this._namespace = string.Empty;
			this._name = string.Empty;
			this._variablename = string.Empty;
			this._method = string.Empty;
			this._session = session;
			this._indexer = 0;
//			this._parameters = new System.Collections.Generic.List<System.Object>();
			this._parameters = new Resolver.Parameters ();
		}
		#endregion

		#region Public Methods
		public void Parse (string statement)
		{
//			Console.WriteLine (statement);

//			if (statement.Substring (0, 1) == "$")
			if (SorentoLib.Render.Resolver.ExpIsVariable.IsMatch (statement))
			{
				#region VARIABLE
				Console.WriteLine ("VARIABLE" + statement);

				Match match = SorentoLib.Render.Resolver.ExpVariable.Match (statement);
				if (match.Success)
				{
					string[] split = match.Groups["fullname"].Value.Split (".".ToCharArray ());
					this._name = split[split.Length - 1];
					
					if (split.Length > 1)
					{
						this._variablename = split[0];
						this._method = split[1];
					}
					else
					{
						this._variablename = split[0];
					}




					Match indexer = SorentoLib.Render.Resolver.ExpIndexer.Match (statement);
					if (indexer.Success)
					{
						this._indexer = int.Parse (indexer.Groups["indexer"].Value);
						string bla = "["+ this._indexer.ToString () +"]";
						this._variablename = this._variablename.TrimEnd (bla.ToCharArray ());
						Console.WriteLine ("indexer "+ this._indexer +" "+ this._variablename);
						this._parameters.Add (this._indexer);
					}


					this._namespace = this._session.Page.Variables[this._variablename].Value.GetType ().Namespace;

//					Console.WriteLine (this._variablename +" "+ this._namespace);


					if (match.Groups["parameters"].Success)
					{
						this.ParseParameters (match.Groups["parameters"].ToString ());
					}





					foreach (SorentoLib.Addins.IRender irender in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IRender)))
					{
						if (irender.IsProvided(this._namespace))
						{
							this._result = irender.Process (this._session, this._session.Page.Variables[this._variablename].Value, this._method, this._parameters);
//							this._result = iclass.Dynamic (this._session, this._method, this._parameters, this._session.Page.Variables[this._variablename].Value);
							break;
						}
					}
				}

				match = null;
				#endregion
			}
			else if (SorentoLib.Render.Resolver.ExpIsMethod.IsMatch (statement))
			{
				#region METHOD
				Console.WriteLine ("METHOD" + statement);
				Match match = Resolver.ExpMethod.Match (statement);
				if (match.Success)
				{
					// Get fullname
					this._fullname = match.Groups["fullname"].Value;

					// Split fullname
					string[] split = this._fullname.Split (".".ToCharArray ());

					if (split.Length > 2)
					{
						// Get Namespace
						for (int i = 0; i < split.Length - 2; i++)
						{
							this._namespace += split[i] + ".";
						}

						this._namespace = this._namespace.TrimEnd (".".ToCharArray ());

						// Get Name
						this._name = split[split.Length - 2];

						// Get Method
						this._method = split[split.Length - 1];
					}
					else
					{
						// Get Namespace
						this._namespace = "core";

						// Get Fullname
						//						this._fullname = "Render." + this._fullname;

						// Get Name
						this._name = "render";

						// Get Method
						this._method = split[split.Length - 1];
					}

//					Console.WriteLine(this._namespace);
//					Console.WriteLine(this._name);
//					Console.WriteLine(this._method);

					// Parse paremeters
					if (match.Groups["parameters"].Success)
					{
						this.ParseParameters (match.Groups["parameters"].ToString ());
					}

					// Find Addin to handle resolve.
					foreach (SorentoLib.Addins.IRender irender in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IRender)))
					{
						if (irender.IsProvided(this._namespace))
						{
							this._result = irender.Process (this._session, this._namespace +"."+ this._name, this._method, this._parameters);
//							this._result = iclass.Static (this._session, this._namespace +"."+ this._name, this._method, this._parameters);
							break;
						}
					}

					// Cleanup
					split = null;

					// Cleanup
					match = null;
				}
				#endregion
			}
			else
			{
				#region STRING
				Console.WriteLine ("STRING" + statement);
				this._result = ParseString (this._session, statement);
				#endregion
			}
		}
		#endregion

		#region Private Methods
		private void ParseParameters (string parameters)
		{
			// Split parameters, and if any needs to be resolved do so.
			MatchCollection matches = Resolver.ExpParams.Matches(parameters);
			for (int i = 0; i < matches.Count; i++)
			{
				if (matches[i].Groups["word"].ToString() != string.Empty)
				{
					// See if parameter contains a variable, if not its probally string
					if (matches[i].Groups["word"].ToString().Substring(0,1) == "$")
					{
						SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver(this._session);
						resolver.Parse(matches[i].Groups["word"].ToString());

						this._parameters.Add(resolver.Result);
					}
					else
					{
						this._parameters.Add(matches[i].Groups["word"].ToString());
					}
				}
			}
		}

		#endregion

		#region Internal Class
		public class Parameters
		{
			#region Privat Fields
			private List<object> _parameters;
			#endregion

			#region Public Fields
			public int Count
			{
				get
				{
					return this._parameters.Count;
				}
			}
			#endregion

			#region Constructor
			public Parameters ()
			{
				this._parameters = new List<object> ();
			}
			#endregion

			#region Public Methods
			public void Add (object Value)
			{
				this._parameters.Add (Value);
			}

			public T Get<T> (int Index)
			{
//				Console.WriteLine (typeof (T).Name.ToLower ());
//				if (typeof (T).Name.ToLower () == "int32")
//				{
					return (T)Convert.ChangeType (this._parameters[Index], typeof(T));
//					return  ((string)this._parameters[Index]);
//				}
//				Console.WriteLine (typeof (T));

//				return (T)this._parameters[Index];
			}

			public Type Type (int Index)
			{
				return this._parameters[Index].GetType ();
			}

//			public string GetString (int Position)
//			{
//
//			}
//
//			public int GetInt (int Position)
//			{
//
//			}
//
//			public decimal GetDecimal (int Position)
//			{
//
//			}
//
//			public Guid GetGuid (int Postition)
//			{
//
//			}

			#endregion
		}

		public static object ParseString (Session session, string statement)
		{
//			Console.WriteLine ("PREPARS: "+ statement);
			string result = string.Empty;
			
			bool inquot = false;			
			bool invariable = false;			
			string block = string.Empty;
			string prevcharacter = string.Empty;
			
			for (int pos = 0; pos < statement.Length; pos++) 
			{	
				string character = statement.Substring (pos, 1);
								
				if (prevcharacter != "\\")
				{
					if (character == "\"")
					{
						if (inquot)						
						{
							inquot = false;
							result += block;
							block = string.Empty;
						}
						else
						{
							inquot = true;
						}
					}
				}
								
				if (!inquot)
				{
					if (invariable)
					{
						if (!Regex.IsMatch (character, "[A-z]|[0-9.]") || (pos == statement.Length-1))
						{
							if (pos == statement.Length-1)
							{
								block += character;
							}

							Resolver r = new Resolver (session);
//							Console.WriteLine ("BLOCK : "+ block);
							r.Parse (block);

							invariable = false;

//							Console.WriteLine ("RESULT: "+ r.Result);

							switch (r.Result.GetType ().Name.ToLower ())
							{
								case "string":
								{
									result += "\""+ r.Result +"\"";
									break;
								}

								case "boolean":
								{
									result += r.Result.ToString ().ToLower ();
									break;
								}

								default:
								{
									result += "\""+ r.Result +"\"";
									break;
								}
							}

//							result += "\""+ r.Result +"\"";
							block = string.Empty;

							if (pos == statement.Length-1)
							{
								continue;
							}
						}
					}
					
					if (character == "$")
					{
						invariable = true;
					}
				}
				
				if (inquot || invariable)
				{
					block += character;
				}
				else
				{
					result += character;
				}
				
				prevcharacter = character;
			}

//			Console.WriteLine ("STRINGPARSER: "+ result);


			return Evaluator.Evaluate (result +";");
		}
		#endregion
	}
}
