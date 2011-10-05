//
// QueryJar.cs
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
	public class QueryJar
	{
		#region Private Fields
		private List<SorentoLib.FastCgi.Query> _queries;
		#endregion

		#region Constructor
		public QueryJar()
		{
			this._queries = new List<Query> ();
		}
		#endregion

		#region Public Methods
		public void Add (SorentoLib.FastCgi.Query Query)
		{
			SorentoLib.FastCgi.Query find = this._queries.Find (delegate (SorentoLib.FastCgi.Query temp) { return temp.Name == Query.Name; });

			if (find != null)
			{
				this._queries.Remove (find);
			}

			this._queries.Add (Query);
		}

		public SorentoLib.FastCgi.Query Get (string Name)
		{
			SorentoLib.FastCgi.Query result = this._queries.Find (delegate (SorentoLib.FastCgi.Query temp) { return temp.Name == Name; });
			if (result != null)
			{
				return result;
			}

			return null;
		}

		public SorentoLib.FastCgi.Query Find (string Name)
		{
			SorentoLib.FastCgi.Query result = this._queries.Find (delegate (SorentoLib.FastCgi.Query temp) { return temp.Name == Name; });

			if (result != null)
			{
				return result;
			}

			return null;
		}
		
		public bool Exist(string Name)
		{
			if (this._queries.Find (delegate (SorentoLib.FastCgi.Query temp) { return temp.Name == Name; }) != null)
			{
				return true;
			} 

			return false;
		}	
		#endregion
	}
}
