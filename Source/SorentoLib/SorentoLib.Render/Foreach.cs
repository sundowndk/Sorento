//
// Foreach.cs
//  
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2007 Rasmus Pedsersen
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

namespace SorentoLib.Render
{
	public class Foreach : SorentoLib.Render.Loop
	{
		#region Private Static Fields
		private static System.Text.RegularExpressions.Regex ExpForeach = new System.Text.RegularExpressions.Regex(@"(?<destination>[^ ]*) in (?<source>.*)", RegexOptions.Compiled);
		#endregion

		#region Private Fields
		private System.String _source = System.String.Empty;
		private System.String _destination = System.String.Empty;
		#endregion

		#region Public Fields
		/// <summary>
		/// Source
		/// </summary>
		public System.String Source
		{
			get
			{
				return this._source;
			}
		}

		/// <summary>
		/// Destination
		/// </summary>
		public System.String Destination
		{
			get
			{
				return this._destination;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Create new Foreach
		/// </summary>
		public Foreach(System.String body)
		{
			base.Type = SorentoLib.Enums.LoopType.Foreach;

			System.Text.RegularExpressions.Match match  = Foreach.ExpForeach.Match(body);
			if (match.Success)
			{
				this._source = match.Groups["source"].Value;
				this._destination = match.Groups["destination"].Value;
			}
		}
		#endregion
	}
}
