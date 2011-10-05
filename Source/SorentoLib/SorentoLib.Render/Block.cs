//
// Block.cs
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

namespace SorentoLib.Render
{	
	public class Block
	{
		#region Private Fields
		private System.Boolean _render = true;
		private System.Boolean _lastifstatementwastrue = false;
		private SorentoLib.Render.Loop _loop = null;
		#endregion				
		
		#region Public Fields
		/// <summary>
		/// True if Block should be rendered.
		/// </summary>
		public System.Boolean Render
		{
			set
			{
				this._render = value;
			}
			get
			{
				return this._render;
			}
		}

		// TODO: Is this really being used?!
		/// <summary>
		/// ???ListIfStatementWasTrue???
		/// </summary>
		public System.Boolean LastIfStatementWasTrue
		{
			get
			{
				return this._lastifstatementwastrue;
			}
			set
			{
				this._lastifstatementwastrue = value;
			}
		}

		/// <summary>
		/// Holds the current Loop if any.
		/// </summary>
		public SorentoLib.Render.Loop Loop
		{
			set
			{
				this._loop = value;
			}
			get
			{
				return this._loop;
			}
		}
		
		/// <summary>
		///  Tells if Block is constructing a loop.
		/// </summary>
		public System.Boolean InLoop
		{
			get
			{
				if (this._loop != null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Begin Loop construction.
		/// </summary>
		public System.Boolean BeginLoop ()
		{
			System.Boolean result = false;

			if (this._loop == null)
			{
				result = true;
			}
			else
			{
				this._loop.Children++;
			}

			return result;
		}

		/// <summary>
		/// End Loop construction.
		/// </summary>
		public System.Boolean EndLoop (SorentoLib.Enums.LoopType looptype)
		{
			System.Boolean result = false;

			if (this._loop != null)
			{
				if ((this._loop.Type == looptype) && (this._loop.Children == 0))
				{
					result = true;
				}
				else
				{
					this._loop.Children--;
				}
			}

			return result;
		}

		#endregion

		#region Constructor
		/// <summary>
		/// Create new Block. 
		/// </summary>
		public Block()
		{					
		}
		#endregion
	}
}
