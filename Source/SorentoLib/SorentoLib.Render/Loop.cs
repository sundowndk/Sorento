//
// Loop.cs
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
using System.Collections.Generic;
#endregion

namespace SorentoLib.Render
{	
	public class Loop
	{
		#region Private Fields
		/// <summary>
		/// Type
		/// </summary>
		private SorentoLib.Enums.LoopType _type;

		/// <summary>
		/// Content
		/// </summary>
		private System.Collections.Generic.List<System.String> _content = new System.Collections.Generic.List<System.String>();

		/// <summary>
		/// Children
		/// </summary>
		private System.Int32 _children = 0;
		#endregion

		#region Public Fields
		/// <summary>
		/// Type
		/// </summary>
		public SorentoLib.Enums.LoopType Type
		{
			set
			{
				this._type = value;
			}
			get
			{
				return this._type;
			}
		}

		/// <summary>
		/// Content
		/// </summary>
		public System.Collections.Generic.List<System.String> Content
		{
			set
			{
				this._content = value;
			}
			get
			{
				return this._content;
			}
		}

		/// <summary>
		/// Children
		/// </summary>
		public System.Int32 Children
		{
			set
			{
				this._children = value;
			}
			get
			{
				return this._children;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Create new Loop
		/// </summary>
		public Loop()
		{			
		}
		#endregion
	}
}
