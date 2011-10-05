// 
// StringHelper.cs
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
using System.Text;

namespace SorentoLib.Tools
{
	public static class StringHelper
	{
		public static byte[] HexStringToBytes(string hex)
		{
			if (hex.Length==0)
			{
				return new byte[] {0};
			}

			if (hex.Length % 2 == 1)
			{
				hex = "0" + hex;
			}

			byte[] result = new byte[hex.Length/2];

			for (int i=0; i<hex.Length/2; i++)
			{
				result[i] = byte.Parse(hex.Substring(2*i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
			}

			return result;
		}

		public static string BytesToHexString(byte[] input)
		{
			StringBuilder hexString = new StringBuilder(64);

			for (int i = 0; i < input.Length; i++)
			{
				hexString.Append(String.Format("{0:X2}", input[i]));
			}
			return hexString.ToString();
		}

		public static string BytesToDecString(byte[] input)
		{
			StringBuilder decString = new StringBuilder(64);

			for (int i = 0; i < input.Length; i++) 
			{
				decString.Append(String.Format(i==0?"{0:D3}":"-{0:D3}", input[i]));
			}
			return decString.ToString();
		}

		// Bytes are string
		public static string ASCIIBytesToString(byte[] input)
		{
			System.Text.ASCIIEncoding enc = new ASCIIEncoding();
			return enc.GetString(input);
		}
		public static string UTF16BytesToString(byte[] input)
		{
			System.Text.UnicodeEncoding enc = new UnicodeEncoding();
			return enc.GetString(input);
		}
		public static string UTF8BytesToString(byte[] input)
		{
			System.Text.UTF8Encoding enc = new UTF8Encoding();
			return enc.GetString(input);
		}

		// Base64
		public static string ToBase64(byte[] input)
		{
			return Convert.ToBase64String(input);
		}

		public static byte[] FromBase64(string base64)
		{
			return Convert.FromBase64String(base64);
		}
	}
}
