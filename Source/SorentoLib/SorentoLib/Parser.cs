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

using SorentoLib;

namespace SorentoLib
{
	public class Parser
	{
		#region Private Fields
		private List<string> _input;
		private List<string> _output;
		#endregion

		#region Public Fields
		public List<string> Input
		{
			get
			{
				return this._input;
			}
		}

		public List<string> Output
		{
			get
			{
				return this._output;
			}
		}
		#endregion

		#region Constructor
		public Parser (List<string> Content)
		{
			this._input = new List<string> ();
			this._output = new List<string> ();

			#region PARSE CONTENT
			List<string> blockdepth = new List<string> ();
			foreach (string line in Content)
			{
				if (line.Contains ("#begin:csharp#"))
				{
					blockdepth.Add ("csharp");
					continue;
				}

				if (line.Contains ("#end:csharp#"))
				{
					if (blockdepth[(blockdepth.Count - 1)] == "csharp")
					{
						blockdepth.RemoveAt ((blockdepth.Count - 1));
					}
					continue;
				}

				if (line.Contains ("#begin:html#"))
				{
					blockdepth.Add ("html");
					continue;
				}

				if (line.Contains ("#end:html#"))
				{
					if (blockdepth[(blockdepth.Count - 1)] == "html")
					{
						blockdepth.RemoveAt ((blockdepth.Count - 1));
					}
					continue;
				}


				if (blockdepth.Count > 0)
				{
					if (blockdepth[(blockdepth.Count - 1)] == "csharp")
					{
						this._input.Add (line);
					}

					if (blockdepth[(blockdepth.Count - 1)] == "html")
					{
						this._input.Add ("Parser.Hooks.Output.Add (\""+ line +"\");");
					}
				}
			}
			#endregion

			#region RUN CONTENT
			string input = string.Empty;
			foreach (string line in this._input)
			{
				input += line;
			}


//			BackgroundWorker bw = new BackgroundWorker ();

//			bw.DoWork += BackgroundWorker;
//			bw.RunWorkerCompleted += bw_RunWorkerCompleted;
//			bw.RunWorkerAsync (input);

//			bw.CancelAsync ();
//			while (bw.IsBusy)
//			{
//				Console.WriteLine ("Waiting...");
//				Thread.Sleep (1);
//			}




			// Has thread pool for a new thread to do the work.
			Thread thread = null;
			Task<List<string>> task = Task.Factory.StartNew<List<string>> (() => 
			{
				//Capture the thread, so we can abort worker thread if it hangs.
				thread = Thread.CurrentThread;

				// Setup error reporting.
				StringWriter reportwriter = new StringWriter ();
				try
				{
					string code = string.Empty;
					Report report = new Report (new Mono.CSharp.StreamReportPrinter (reportwriter));

					// Create new evaluator instance.
					var evaluator = new Evaluator (new CompilerSettings (), report); 

					// Reference current assembly.
					evaluator.ReferenceAssembly (Assembly.GetExecutingAssembly ());

					// Reference SorentoLib.
					evaluator.ReferenceAssembly (typeof(SorentoLib.Runtime).Assembly);

					evaluator.Run ("using Test;");

					// Using.
					evaluator.Run ("using System; using SorentoLib;");

					// Initalize Addins who needs it.
					foreach (SorentoLib.Addins.IRuntime runtime in AddinManager.GetExtensionObjects (typeof(SorentoLib.Addins.IRuntime)))
					{
						evaluator.ReferenceAssembly (runtime.Assembly);
						evaluator.Run ("using "+ runtime.Assembly.GetName ().Name +";");
					}

					// Anonymous methods.
					code += "Test.Parser.Hooks.Commands.Print.Delegate Print = delegate (object Value) { Test.Parser.Hooks.Commands.Print.Method (Value);};";


					code += (string)input;

					// Output.
					Parser.Hooks.Output = new List<string> ();

					// Run evaluation.
					evaluator.Run (code);


//					foreach (string line in Parser._threadstaticoutput)
//					{
//						Console.WriteLine (line);
//					}


//					string result = reportwriter.ToString();
//					Console.WriteLine (result);
					evaluator = null;
					report = null;

				}
				catch
				{
//					Console.WriteLine ("Error");
				}

				reportwriter.Close ();
				reportwriter.Dispose ();

//				Console.WriteLine ("Evaluator done...");

				return Parser.Hooks.Output;
			});



//
//    //This is needed in the example to avoid thread being still NULL
//    Thread.Sleep(10);
//
//    //Cancel the task by aborting the thread
//    thread.Abort();


			if (!task.Wait (25000))
			{
				thread.Abort ();
				Console.WriteLine ("Evaluator took to long.. thread killed...");
			}

//			foreach (string line in task.Result)
//			{
//				Console.WriteLine (line);
//			}

			task.Dispose ();

//			evaluatorthread.Start ();       
//			if (!evaluatorthread.Join (10000))
//			{
//				evaluatorthread.Abort ();
//				Console.WriteLine ("Evaluator took to long.. thread killed...");
//			}

//			System.Threading.Tasks

//			Console.WriteLine ("Done...");

			#endregion
		}
		#endregion

		#region Nested classes
		public static class Hooks
		{
			[ThreadStatic]
			public static List<string> Output;

			[ThreadStatic]
			public static SorentoLib.Session Session;

			public static class Commands
			{
				public static class Print
				{
					public delegate void Delegate (object Value);

					public static void Method (object Value)
					{
						Parser.Hooks.Output.Add (Value.ToString ());
					}
				}
			}
		}
		#endregion
	}
}

