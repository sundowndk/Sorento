//
// Variables.cs
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

#region Usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SNDK;
#endregion

namespace SorentoLib.Render
{
	public class Variables
	{
		#region Private Fields
		private Hashtable _variables = new Hashtable();
		#endregion

		#region Public Fields
		public Variable this [string index]
		{
			get
			{
				return (Variable)this._variables[index];
			}
		}
		#endregion

		#region Constructor
		public Variables()
		{
		}
		#endregion

		#region Methods
		public void Clear()
		{
			this._variables.Clear();
		}

		public bool Contains (string variablename)
		{
			return this._variables.Contains (variablename);
		}


		public void Add (string variablename, object value)
		{
			Variable variable = new Variable();
			variable.Name = variablename;
			variable.Value = value;
			this._variables[variablename] = variable;
		}




//		public static List<object> ConvertToListObject<T>(object data)
//		{
//			List<object> result = new List<object>();
//			foreach (T item in (List<T>)data)
//			{
//				result.Add(item);
//			}
//			return result;
//		}

		public static void ConvertToListTest (object data)
		{

			

//			foreach (object test in data)
//			{
//
//			}

//			Type t = data.GetType ();
//
//			Activator.

//			object t1 = Activator.CreateInstance(t).GetType ();



			//Console.WriteLine (Activator.CreateInstance(t).GetType ());

			//Console.WriteLine (data.GetType ().GetGenericTypeDefinition ());


		}



		public static List<object> ConvertToListObjectNew<T>(List<T> data)
		{
			List<object> result = new List<object>();
			foreach (T item in data)
			{
				result.Add(item);
			}
			return result;
		}

		public static List<object> ConvertToListObject<T>(List<T> data)
		{
			List<object> result = new List<object>();
			foreach (T item in data)
			{
				result.Add(item);
			}
			return result;
		}

		public static List<object> ConvertToListObject<T>(IList<T> data)
		{
			List<object> result = new List<object>();
			foreach (T item in data)
			{
				result.Add(item);
			}
			return result;
		}
		#endregion


		/// <summary>
		/// Expression: Variable
		/// </summary>
//		public static Regex ExpVariable = new Regex(@"([^\\]\$(?<variablename>[a-zA-Z0-9\.]*))", RegexOptions.Compiled);

		/// <summary>
		/// Expression: Env
		/// </summary>
//		public static Regex ExpEnv = new Regex(@"^env\.(?<scope>[^.:]*)\.(?<variable>[^.:]*)\.?(?<options>.[^:]*)?", RegexOptions.Compiled);

		/// <summary>
		/// Expression: NameValue
		/// </summary>
//		public static Regex ExpNameValue = new Regex(@"(?<name>.*)\.(?<value>.*)");

		/// <summary>
		/// Variable1
		/// </summary>
//		public static Regex ExpVariable1 = new Regex(@"\$(?<namespace>.*)\.(?<name>[^(]*) *\(?((?<parameters>.*)\))?");

		/// <summary>
		/// Method
		/// </summary>
//		public static Regex ExpMethod = new Regex(@"^(?<fullname>[^ (]*) *\((?<parameters>.*)\)");

		/// <summary>
		/// Params
		/// </summary>
//		public static Regex ExpParams = new Regex("(\"(?<word>[^\"]+|\"\")*\"|(?<word>[^ ,]*))");

		/// <summary>
		/// Match
		/// </summary>
//		public static Match Match = null;

//		public Hashtable nvariables
//		{
//			get
//			{
//				return this._nvariables;
//			}
//
//			set
//			{
//				this._nvariables = value;
//			}
//		}

		//		/// <summary>
//		/// Get variable
//		/// </summary>
//		public T Get<T>(string Name)
//		{
//			T result = default(T);
//
//			try
//			{
//				Variable variable = (Variable)this._variables[Name];
//				result = (T)variable.Value;
//			}
//			catch {}
//
//			return (T)result;
//		}

		/// <summary>
		/// Environment
		/// </summary>
//		public static string Environment(SorentoLib.Session Session, string Name, string Subname, string Options) 
//		{
//			// Definitions
//			string result = string.Empty;
//			
//			switch (Name)
//			{
//				case "date":
//					switch (Subname)
//					{
//						case "now":
//						result = DateTime.Now.ToString();
//						break;
//					}				
//					break;
//				
//				case "core":
//					switch (Subname)
//					{
//						case "version":
//						result = Toolbox.Global.Variables.Get<string>("version");
//						break;
//
//						case "cgiurl":
//						result = Session.CgiUrl;
//						break;
//					
//						case "cgiexec":
//						result = Session.CgiExec;
//						break;
//					}								
//					break;
//
//				case "query":
//					result = Session.Cgi.QueryJar.Query(Subname).Value;
//					break;
//
//				case "session":						
//					switch (Subname)
//					{									
//						case "validuser":
//						result = Session.VaildUser.ToString().ToLower();						
//						break;
//
//						case "userid":
//						result = Session.User.Id.ToString();
//						break;
//
//						case "accesslevel":
//						string[] split = Options.Split(".".ToCharArray());						
//
//						if (Session.VaildUser)
//						{
//							result = Session.User.AuthenticateByAccessLevel(split[0], Toolbox.Tools.StringToEnum<Enums.Accesslevels>(split[1])).ToString().ToLower();
//						}
//						else
//						{
//							result = "false";
//						}				
//					
//						break;					
//					}				
//					break;
//			}
//		
//			// Finish
//			return result;
//		}

//		public static string ParseVariables(SorentoLib.Session Session, string contentstring)
//		{
//			//FIXME: Clean this shit up!
//			string result = string.Empty;
//				bool fix = false;
//				if (contentstring.Substring(0,1) == "$")
//				{
//					result = " "+contentstring;
//					fix = true;
//				}
//				else
//				{
//					result = contentstring;
//				}
//
//
//				bool search = true;
//				while (search)
//				{
//					Match match = Variables.ExpVariable.Match(result);
//
//					if (match.Success)
//					{
//						string variablecontent = Variables.GetValue(Session, match.Groups["variablename"].Value, null);
//						
//						Capture c = match.Groups[2];
//
//						if (fix)
//						{
//							result = result.Substring(0,c.Index-1) + variablecontent + result.Substring(c.Index+c.Length, result.Length-c.Index-c.Length);
//							result = result.Substring(1, variablecontent.Length);
//						}
//						else
//						{
//							result = result.Substring(0,c.Index-1) + variablecontent + result.Substring(c.Index+c.Length, result.Length-c.Index-c.Length);	
//						}					
//						continue;
//					}
//					search = false;
//				}
//			return result;
//		}

//		public object Parse (Session session, string data)
//		{
//			object result = null;
//
//
//			if (data.Substring(0,1) == "$")
//			{
//				Console.WriteLine("VARIABLE:");
//
//				Match match = Variables.ExpVariable1.Match(data);
//				if (match.Success)
//				{
//
//					string variablename = match.Groups["variablename"].Value;
//					string method = match.Groups["method"].Value;
//
//					List<string> param = new List<string>();
//
//					Variable test = (Variable)this._variables[variablename];
//					object test2 = test.Value;
//
//					string name = test2.GetType().Name;
//					string fullname = test2.GetType().FullName;
//
//					foreach (SorentoPlugin plugin in Toolbox.Global.Variables.Get<List<SorentoPlugin>>("plugins"))
//					{
//						foreach (Type type in plugin.Types)
//						{
//							if (type.FullName == fullname)
//							{
////								result = plugin.Variables(test2, name, match.Groups["method"].Value, this.);
//							}
//						}
//					}
//
////					result = string.Empty;
//
//				}
//			}
//			else
//			{
//				SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver(session, data);
//
//				Console.WriteLine("METHOD: ["+ resolver.Fullname+"]");
//
//				foreach (SorentoPlugin plugin in Toolbox.Global.Variables.Get<List<SorentoPlugin>>("plugins"))
//				{
//					foreach (string namespace_ in plugin.Namespaces)
//					{
//						if (namespace_.ToLower() == resolver.Namespace.ToLower())
//						{
////							result = plugin.Methods(session, resolver.Fullname, resolver.Parameters);
//						}
//					}
//				}
//
//
//
////				Match match = Variables.ExpMethod.Match(data);
////				if (match.Success)
////				{
//
////					string fullname = string.Empty;
////					List<string> param = new List<string>();
//
////					fullname = match.Groups["namespace"].Value +"."+ match.Groups["name"].Value;
//
//
//
//
////					if (match.Groups["param"].Success)
////					{
////
////						MatchCollection matches = Variables.ExpParams.Matches(match.Groups["param"].ToString());
////
////
////						for (int i = 0; i < matches.Count; i++)
////						{
////							if (matches[i].Groups["word"].ToString() != string.Empty)
////							{
////								param.Add(matches[i].Groups["word"].ToString());
////							}
////
////						}
////					}
//
////					foreach (SorentoPlugin plugin in Toolbox.Global.Variables.Get<List<SorentoPlugin>>("plugins"))
////					{
////						foreach (Type type in plugin.Types)
////						{
////							if (type.FullName == fullname)
////							{
////								result = plugin.Methods(match.Groups["name"].Value +"."+ match.Groups["method"].Value, param);
////							}
////						}
////					}
////				}
//			}
//			return result;
//		}

		// TODO: What are these three?!
//		public static void SetValue (SorentoLib.Session Session, string Name, string Value)
//		{			
//			Value = Variables.ParseVariables(Session, Value);
//			Session.Page.Variables.Set(Name, Value.Replace(@"\""", @""""));
//		}
		
//		public static string GetValue (SorentoLib.Session Session, string Name, Dictionary<string, string> Options)
//		{
//			// Definitions
//			string result = string.Empty;
//
//			Variables.Match = Variables.ExpEnv.Match(Name);
//			if (Variables.Match.Success) 									
//			{							 	
//				result = Variables.Environment(Session, Variables.Match.Groups["scope"].Value, Variables.Match.Groups["variable"].Value, Variables.Match.Groups["options"].Value);
//			} 
//			else
//			{								
//				Variables.Match = Variables.ExpNameValue.Match(Name);
//				if (Variables.Match.Success)
//				{		
//					if (Variables.Match.Groups["name"].Value.ToLower() == "env")
//					{					
//						result = Session.Page.Variables.Get<string>(Name);						
//					}
//					else 
//					{								
////						result = InternalPlugin.Variable(Session, Session.Page.Variables.GetObjectType(Name), Variables.Match.Groups["name"].Value, Variables.Match.Groups["value"].Value, Options);
//						
////						if (result == string.Empty)
////						{
//							foreach (SorentoPlugin plugin in ((List<SorentoPlugin>)Toolbox.Global.Variables.Get<List<SorentoPlugin>>("plugins"))) 
//							{
//								foreach (string objecttype in plugin.ObjectTypes) 
//								{														
//									if (objecttype == Session.Page.Variables.GetObjectType(Name))
//									{
//										result = plugin.Variable(Session, objecttype, Variables.Match.Groups["name"].Value, Variables.Match.Groups["value"].Value, Options);
//									}
//								}
//							}																		
////						}
//					}
//				}
//				else 
//				{
// 					result = Session.Page.Variables.Get<string>(Name);
//					if (result == null)
//					{
//						result = string.Empty;
//					}
//				}			
//			}
//						
//			// Finish
//			return result;
//		}

		/// <summary>
		/// Get object type. 
		/// </summary>
//		public string GetObjectType(string Name)
//		{			 	
//			// Definitions
//			string result = string.Empty;			
//			string[] split = Name.Split(".".ToCharArray());
//						
//			if (this._variables.ContainsKey(split[0]))
//			{	
//				Variable variable = (Variable)this._variables[split[0]];
//				result = variable.ObjectType;
//			}
//			
//			// Finish
//			return result;
//		}		
					
		/// <summary>
		/// Set
		/// </summary>
//		public void Set(string Name, object Value)
//		{
//			this.Set(Name, Value, string.Empty);
//		}
		
		/// <summary>
		/// Set
		/// </summary>
//		public void Set(string Name, object Value, string ObjectType)
//		{
//			// Check if we are not trying to set an ENV Variable.
//			Match match = Variables.ExpEnv.Match(Name);
//
//			if (!match.Success)
//			{
//				if (this._variables.ContainsKey(Name))
//				{
//					Variable variable = new Variable();
//					variable.Name = Name;
//					variable.Value = Value;
//					variable.ObjectType = ObjectType;
//					this._variables[Name] = variable;
//				}
//				else
//				{
//					Variable variable = new Variable();
//					variable.Name = Name;
//					variable.Value = Value;
//					variable.ObjectType = ObjectType;
//
//					this._variables.Add(Name, variable);
//				}
//			}
//		}

	}
}
