//
// Query.cs
//
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
//
// Copyright (c) 2010 Rasmus Pedersen
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

namespace SorentoLib.FastCgi
{	
	public class Query
	{
		#region Private Fields
		private string _name;
		private string _value;
		private List<string> _values;
		private bool _hasbinarydata;
		private byte[] _binarydata;
		private long _binarylength;
		private string _binarycontenttype;
		#endregion

		#region Public Fields
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public string Value
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

		public List<string> Values
		{
			get
			{
				return this._values;
			}
			set
			{
				this._values = value;
			}
		}

		public bool HasBinaryData
		{
			get
			{
				return this._hasbinarydata;
			}
			set
			{
				this._hasbinarydata = value;
			}
		}

		public byte[] BinaryData
		{
			get
			{
				return this._binarydata;
			}
			set
			{
				this._binarydata = value;
			}
		}

		public long BinaryLength
		{
			get
			{
				return this._binarylength;
			}
			set
			{
				this._binarylength = value;
			}
		}

		public string BinaryContentType
		{
			get
			{
				return this._binarycontenttype;
			}
			set
			{
				this._binarycontenttype = value;
			}
		}
		#endregion

		#region Constructor
		public Query()
		{
			this._name = string.Empty;
			this._value = string.Empty;
			this._values = new List<string>();
			this._hasbinarydata = false;
			this._binarydata = null;
			this._binarylength = 0;
			this._binarycontenttype = string.Empty;
		}
		#endregion
	}
}
