// 
// ParseTypeName.cs
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

namespace SorentoLib.Tools
{
	public class ParseTypeName
	{
		private string _fullname;
		private string _namespace;
		private string _name;
		private string _method;

		public string Fullname
		{
			get
			{
				return this._fullname;
			}
		}

		public string Namspace
		{
			get
			{
				return this._namespace;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public string Method
		{
			get
			{
				return this._method;
			}
		}

		public ParseTypeName (string Fullname)
		{
			this._fullname = Fullname;
			string[] split = this._fullname.Split (".".ToCharArray ());
			
			if (split.Length > 2)
			{
				for (int i = 0; i < split.Length - 2; i++)
				{
					this._namespace += split[i] + ".";
				}
				this._namespace = this._namespace.TrimEnd (".".ToCharArray ());
				this._name = split[split.Length - 2];
				this._method = split[split.Length - 1];
			}

			else
			{
				this._namespace = "SorentoLib";
				this._name = split[0];
				this._method = split[1];
			}
			
			this._fullname = this._namespace + "." + this._name;
		}
	}
}
