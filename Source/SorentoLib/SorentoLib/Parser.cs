//
// Parser.cs
//
// Author:
//       sundown <${AuthorEmail}>
//
// Copyright (c) 2013 sundown
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
using Mono.CSharp;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using SorentoLib;

namespace SorentoLib
{
	public class Parser
	{
		#region Private Fields
		private Template _template;
		private List<ParserVariable> _variables;
		private StringBuilder _output;
		private ParserError _errors;
		#endregion

		#region Public Fields
		public string Output
		{
			get
			{
				return this._output.ToString ();
			}
		}

		public ParserError Errors
		{
			get
			{
				return this._errors;
			}
		}
		#endregion

		#region Constructor
		public Parser (Template Template)
		{
			this._template = Template;
			this._variables = new List<ParserVariable> ();
			this._output = new StringBuilder ();

			this.Run ();
		}

		public Parser (Template Template, List<ParserVariable> Variables)
		{
			this._template = Template;
			this._variables = Variables;
			this._output = new StringBuilder ();

			this.Run ();
		}
		#endregion

		private void Run ()
		    {
				string input = this._template.Content;

				// Ask thread pool for a new thread to do the work.
				Thread thread = null;
				Task<StringBuilder> task = Task.Factory.StartNew<StringBuilder> (() => 
				{
					  //Capture the thread, so we can abort worker thread if it hangs.
					  thread = Thread.CurrentThread;

					  // Setup error reporting.
					  using (StringWriter reportwriter = new StringWriter ())
					  {
						    try
						    {
								string evalcode = string.Empty;

								Report report = new Report (new Mono.CSharp.StreamReportPrinter (reportwriter));

								// Create new evaluator instance.
								var evaluator = new Evaluator (new CompilerSettings (), report); 

								// Reference current assembly.
								evaluator.ReferenceAssembly (Assembly.GetExecutingAssembly ());

						evaluator.ReferenceAssembly (typeof (SNDK.Convert).Assembly);

								// Using.
								evaluator.Run ("using System; using System.Collections.Generic; using SorentoLib; using SNDK");


								foreach (SorentoLib.Addins.IRuntime runtime in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IRuntime)))
								{
									  evaluator.ReferenceAssembly (runtime.Assembly);
									  evaluator.Run ("using " + runtime.Assembly.GetName ().Name + ";");
								}

								// Anonymous methods.
								evalcode += "SorentoLib.Parser.Hooks.Commands.Print.Delegate Print = delegate (object Value) { SorentoLib.Parser.Hooks.Commands.Print.Method (Value);};\n";

								// Variables
								int counter = 0;
								Parser.Hooks.Variables = this._variables;

								foreach (ParserVariable variable in this._variables)
								{
									  evalcode += variable.Value.GetType ().FullName + " " + variable.Name + " = (" + variable.Value.GetType ().FullName + ")SorentoLib.Parser.Hooks.Variables[" + counter + "].Value;\n"; 
									  counter++;
								}

								evalcode += input;

								// Output.
								Parser.Hooks.Errors = null;
								Parser.Hooks.Output = new StringBuilder ();

								// Run evaluation.
								evaluator.Run (evalcode);

								// Cleanup.
								evaluator = null;
								report = null;
								evalcode = null;

								if (reportwriter.ToString () != string.Empty)
								{
									  Console.WriteLine ("!!!!" + reportwriter.ToString ());

									  Parser.Hooks.Errors = new ParserError (reportwriter.ToString ());
								}
						    } catch (Exception e)
						    {
								Console.WriteLine (e);

								string interactive = string.Empty;
								interactive += "{interactive}(0,0): error SE0000: " + e.Message;
								interactive += reportwriter.ToString ();

								Parser.Hooks.Errors = new ParserError (interactive);
						    }

						    this._errors = Hooks.Errors;
//					this._output = Parser.Hooks.Output;

						    return Parser.Hooks.Output;
					  }
				}
				);		

			if (!task.IsCompleted)
			{
				if (!task.Wait (25000))
				{
					thread.Abort ();
					throw new Exceptions.Parser ("TEMPLATE PARSER TIMEOUT");
				}
			}

			if (this._errors != null)
			{
				string message = string.Empty;

				bool thr = false;
				message += "PARSER EXCEPTION:<br>\n";
				foreach (SorentoLib.ParserError.Error error in this._errors.Errors)
				{
					if (error.Type == Enums.ParserErrorType.Error)
					{
						thr = true;
					}

					message += "Line: "+ error.Line + " - "+ error.Code +" - "+ error.Text +"<br>\n";
				}

				if (thr)
				{
					throw new Exceptions.Parser (message);
				}
			}

			this._output = task.Result;


//			task.Dispose ();		
		}

		#region Nested classes
		public static class Hooks
		{
			[ThreadStatic]
			public static StringBuilder Output;

			[ThreadStatic]
			public static List<ParserVariable> Variables;

			[ThreadStatic]
			public static ParserError Errors;

			public static class Commands
			{
				public static class Print
				{
					public delegate void Delegate (object Value);

					public static void Method (object Value)
					{
						Parser.Hooks.Output.AppendLine (Value.ToString ());
					}
				}
			}
		}
		#endregion
	}
}

