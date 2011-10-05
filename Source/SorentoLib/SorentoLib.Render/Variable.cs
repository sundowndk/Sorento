//
// Variable.cs
//  
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (c) 2009 sundown
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

namespace SorentoLib.Render
{
	public class Variable
	{
		#region Private Fields
		/// <summary>
		/// Name
		/// </summary>
		private System.String _name = System.String.Empty;

		/// <summary>
		/// Value
		/// </summary>
		private System.Object _value;

		/// <summary>
		/// ObjectType
		/// </summary>
		private System.String _objecttype = System.String.Empty;

		// TODO: figure out where TYPE belongs.
		/// <summary>
		/// Type
		/// </summary>
//		private Type _type;
		#endregion

		#region Public Fields
		/// <summary>
		/// Name
		/// </summary>
		public System.String Name
		{
			set
			{
				this._name = value;
			}
			get
			{
				return this._name;
			}
		}

		/// <summary>
		/// Value
		/// </summary>
		public System.Object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		/// <summary>
		/// ObjectType
		/// </summary>
		public System.String ObjectType
		{
			get
			{
				return this._objecttype;
			}
			set
			{
				this._objecttype = value;
			}
		}

		/// <summary>
		/// Type
		/// </summary>
		public Type Type
		{
			get
			{
				return this._value.GetType();
			}
		}

		#endregion

		#region Constructor
		/// <summary>
		/// Create new Variable
		/// </summary>
		public Variable()
		{
		}
		#endregion
	}
}
