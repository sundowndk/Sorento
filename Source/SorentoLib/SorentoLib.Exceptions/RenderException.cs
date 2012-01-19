// 
// RenderException.cs
//  
// Author:
//       sundown <${AuthorEmail}>
// 
// Copyright (c) 2012 rvp
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

namespace SorentoLib.Exceptions
{
	public class RenderException: System.Exception
	{
		public RenderException (string message): base (message)
		{
			this._line = 0;
			this._filename = string.Empty;
		}

		public RenderException (string message, int line, string filename): base (message)
		{
			this._line = line;
			this._filename = filename;
		}

		public int Line
		{
			get
			{
				return this._line;
			}
		}

		public string Filename
		{
			get
			{
				return this._filename;
			}

		}

		private int _line;
		private string _filename;
	}
}

