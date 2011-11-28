//
// Statement.cs
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2007 - 2011 Rasmus Pedsersen
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
using Mono.CSharp;

namespace SorentoLib.Render
{
	public class Condition
	{
		#region Public Static Methods
		public static bool Evaluate (Session session, string condition)
		{
			bool result = false;

			try
			{
//				result = (bool)Evaluator.Evaluate (Test (session, condition) +";");
				result = (bool)Resolver.ParseString (session, condition);
			}
			catch (Exception e)
			{
				SorentoLib.Services.Logging.LogDebug ("Condition evaluation failed: "+ e);
			}

			return result;
		}
		#endregion

//		#region Private Static Methods
//		private static string Test (Session session, string condition)
//		{
//			int pos = 0;
//
//			bool instring = false;
//			bool inparam = false;
//			bool incondition = false;
//			bool isleft = true;
//			bool isright = false;
//
//			string left = "";
//			string right = "";
//			string toggle = "";
//
//
//			bool failed = false;
//
//			while (pos < condition.Length)
//			{
//				string c = condition.Substring(pos, 1);
//
//				switch (condition.Substring(pos, 1))
//				{
//					case "(":
//						if (!instring && !inparam)
//						{
//							inparam = true;
//						}
//						break;
//
//					case ")":
//						if (!instring && inparam)
//						{
//							inparam = false;
//						}
//						break;
//
//					case @"""":
//						if (instring)
//						{
//							instring = false;
//						}
//						else
//						{
//							instring = true;
//						}
//						break;
//				}
//
//
//
//				if (((c == "=") || (c == "!" || (c == "<") || (c == ">")) ) && (inparam == false) && (instring == false))
//				{
//					incondition = true;
//					if (condition.Substring(pos+1,1) == "=")
//					{
//						toggle = condition.Substring(pos, 2);
//						pos+=2;
//					}
//					else
//					{
//						toggle = condition.Substring(pos, 1);
//						pos++;
//					}
//
//
//					isleft = false;
//					isright = true;
//
//					continue;
//				}
//
//				if (isleft)
//				{
//					left += c;
//				}
//
//				if (isright)
//				{
//					right += c;
//				}
//
//				pos++;
//			}
//
//			SorentoLib.Render.Resolver resolver1 = new SorentoLib.Render.Resolver(session);
//
//			resolver1.Parse(left.TrimEnd(" ".ToCharArray()));
//
//			SorentoLib.Render.Resolver resolver2 = new SorentoLib.Render.Resolver(session);
//			if (right != string.Empty)
//			{
//
//				resolver2.Parse(right.TrimStart(" ".ToCharArray()));
//				right = resolver2.Result.ToString();
//			}
//			else
//			{
//				right = "True";
//			}
//
//			if (toggle == string.Empty)
//			{
//				toggle = "==";
//			}
//
//			left = resolver1.Result.ToString();
//
//
//			return @"("""+ left +@""" "+ toggle +@" """+ right +@""")";
//		}
//		#endregion
	}
}
