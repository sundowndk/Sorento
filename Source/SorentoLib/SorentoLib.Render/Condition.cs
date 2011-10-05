//
// Statement.cs
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

using Jyc.Expr;

namespace SorentoLib.Render
{
	public class Condition
	{
		#region Private Static Fields
		private static System.Text.RegularExpressions.Regex ExpString = new System.Text.RegularExpressions.Regex(@"(?<=^(([^\""]*(?<!\\\\)\""[^\""]*(?<!\\\\)\""[^\""]*)*|[^\""]*))(?<variablename>\$[A-z0-9\.]*)");
		#endregion

		#region Private Fields
		private System.Boolean _result = false;
		#endregion

		#region Public Fields
		/// <value>
		/// Result of the condition.
		/// </value>
		public System.Boolean Result
		{
			get
			{
				return _result;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Create new Condition
		/// </summary>
		public Condition(SorentoLib.Session session, System.String condition)
		{
//			condition = "("+ condition +")";

			Jyc.Expr.Parser ep = new Jyc.Expr.Parser();
			Jyc.Expr.Evaluator evaluater = new Jyc.Expr.Evaluator();
			ParameterVariableHolder pvh = new ParameterVariableHolder();

			pvh.Parameters["char"] = new Parameter(typeof(System.Char));
			pvh.Parameters["sbyte"] = new Parameter(typeof(System.SByte));
			pvh.Parameters["byte"] = new Parameter(typeof(System.Byte));
			pvh.Parameters["short"] = new Parameter(typeof(System.Int16));
			pvh.Parameters["ushort"] = new Parameter(typeof(System.UInt16));
			pvh.Parameters["int"] = new Parameter(typeof(System.Int32));
			pvh.Parameters["uint"] = new Parameter(typeof(System.UInt32));
			pvh.Parameters["long"] = new Parameter(typeof(System.String));
			pvh.Parameters["ulong"] = new Parameter(typeof(System.UInt64));
			pvh.Parameters["float"] = new Parameter(typeof(System.Single));
			pvh.Parameters["double"] = new Parameter(typeof(System.Double));
			pvh.Parameters["decimal"] = new Parameter(typeof(System.Decimal));
			pvh.Parameters["DateTime"] = new Parameter(typeof(System.DateTime));
			pvh.Parameters["string"] = new Parameter(typeof(System.String));
			pvh.Parameters["Guid"] = new Parameter(typeof(System.Guid));
			pvh.Parameters["Convert"] = new Parameter(typeof(System.Convert));
			pvh.Parameters["Math"] = new Parameter(typeof(System.Math));
			pvh.Parameters["Array "] = new Parameter(typeof(System.Array));
			pvh.Parameters["Random"] = new Parameter(typeof(System.Random));
			pvh.Parameters["TimeZone"] = new Parameter(typeof(System.TimeZone));
			pvh.Parameters["AppDomain "] = new Parameter(typeof(System.AppDomain));
			pvh.Parameters["Console"] = new Parameter(typeof(System.Console));
			pvh.Parameters["evaluater"] = new Parameter(evaluater);

//			System.Text.RegularExpressions.MatchCollection matches = SorentoLib.Render.Condition.ExpString.Matches(condition);
//			for (System.Int32 i = 0; i < matches.Count; i++)
//			{
//				if (matches[i].Groups["variablename"].Value != string.Empty)
//				{
//					Console.WriteLine(matches[i].Groups["variablename"].Value);
//
//					SorentoLib.Render.Resolver resolver = new SorentoLib.Render.Resolver(session, matches[i].Groups["variablename"].Value);
//					condition = System.Text.RegularExpressions.Regex.Replace(condition, matches[i].Groups["variablename"].Value.Replace("$", @"\$"), "\""+resolver.Result.ToString()+"\"");
//				}
//			}

			condition = Parse(session, condition);
//			Console.WriteLine(condition);


			evaluater.VariableHolder = pvh;
			Jyc.Expr.Tree tree = ep.Parse(condition);
			System.Object result = evaluater.Eval(tree);

			if (result != null)
			{
				this._result = (System.Boolean)result;
			}				
			else
			{
				this._result = false;
			}										
		}
		#endregion

		private string Parse(Session session, string Condition)
		{
			int pos = 0;

			bool instring = false;
			bool inparam = false;
			bool incondition = false;
			bool isleft = true;
			bool isright = false;

			string left = "";
			string right = "";
			string toggle = "";


			bool failed = false;

			while (pos < Condition.Length)
			{
				string c = Condition.Substring(pos, 1);

				switch (Condition.Substring(pos, 1))
				{
					case "(":
						if (!instring && !inparam)
						{
							inparam = true;
						}
						break;

					case ")":
						if (!instring && inparam)
						{
							inparam = false;
						}
						break;

					case @"""":
						if (instring)
						{
							instring = false;
						}
						else
						{
							instring = true;
						}
						break;
				}



				if (((c == "=") || (c == "!" || (c == "<") || (c == ">")) ) && (inparam == false) && (instring == false))
				{
					incondition = true;
					if (Condition.Substring(pos+1,1) == "=")
					{
						toggle = Condition.Substring(pos, 2);
						pos+=2;
					}
					else
					{
						toggle = Condition.Substring(pos, 1);
						pos++;
					}


					isleft = false;
					isright = true;

					continue;
				}

				if (isleft)
				{
					left += c;
				}

				if (isright)
				{
					right += c;
				}

				pos++;
			}

			SorentoLib.Render.Resolver resolver1 = new SorentoLib.Render.Resolver(session);

			resolver1.Parse(left.TrimEnd(" ".ToCharArray()));

			SorentoLib.Render.Resolver resolver2 = new SorentoLib.Render.Resolver(session);
			if (right != string.Empty)
			{

				resolver2.Parse(right.TrimStart(" ".ToCharArray()));
				right = resolver2.Result.ToString();
			}
			else
			{
				right = "True";
			}

			if (toggle == string.Empty)
			{
				toggle = "==";
			}

//			Console.WriteLine(right);
//			Console.WriteLine(toggle);
			left = resolver1.Result.ToString();


			return @"("""+ left +@""" "+ toggle +@" """+ right +@""")";
		}
	}
}
