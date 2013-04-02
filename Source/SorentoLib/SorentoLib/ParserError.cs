//
// ParserError.cs
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SorentoLib
{
	public class ParserError
	{
		#region Private Fields
		private static Regex _expinteractiveerror = new Regex (@"^\{interactive\}\((?<line>\d*),(?<character>\d*)\)\: error (?<code>.*)\:.(?<text>.*)", RegexOptions.Compiled|RegexOptions.Multiline);
		private List<Error> _errors;
		#endregion

		#region Public Fields
		public IList<Error> Errors
		{
			get
			{
				return this._errors.AsReadOnly ();
			}
		}
		#endregion

		#region Constructor
		public ParserError (string InteractiveError)
		{
			this._errors = new List<Error> ();

			foreach (Match match in _expinteractiveerror.Matches (InteractiveError))
			{
				this._errors.Add (new Error (int.Parse (match.Groups ["line"].Value), int.Parse (match.Groups ["character"].Value), match.Groups ["code"].Value, match.Groups ["text"].Value));
			}
		}
		#endregion

		public class Error
		{
			#region Private Fields
			private int _line;
			private int _character;
			private string _code;
			private string _text;
			#endregion

			#region Public Fields
			public int Line
			{
				get
				{
					return this._line;
				}
			}

			public int Character
			{
				get
				{
					return this._character;
				}
			}

			public string Code
			{
				get
				{
					return this._code;
				}
			}

			public string Text
			{
				get
				{
					return this._text;
				}
			}
			#endregion

			#region Constructor
			internal Error (int Line, int Character, string Code, string Text)
			{
				this._line = Line;
				this._character = Character;
				this._code = Code;
				this._text = Text;
			}
			#endregion
		}
	}
}

