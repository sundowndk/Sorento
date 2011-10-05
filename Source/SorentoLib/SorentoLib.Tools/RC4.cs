// 
// RC4.cs
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
	public class RC4
	{
		public static string Encrypt (string Password, string Data)
		{
			int a, i, j, k, tmp, pwd_length, data_length;
			int[] key, box;
			byte[] cipher;
			
//			if (ispwdHex)
		//			{
		//				pwd = pack ("H*", pwd);
		//			}
		


			Data = pack ("H*", Data);
			
			pwd_length = Password.Length;
			data_length = Data.Length;
			key = new int[256];
			box = new int[256];
			cipher = new byte[Data.Length];
			
			for (i = 0; i < 256; i++)
			{
				key[i] = ord (Password[i % pwd_length]);
				box[i] = i;
			}
			
			for (j = i = 0; i < 256; i++)
			{
				j = (j + box[i] + key[i]) % 256;
				tmp = box[i];
				box[i] = box[j];
				box[j] = tmp;
			}
			
			for (a = j = i = 0; i < data_length; i++)
			{
				a = (a + 1) % 256;
				j = (j + box[a]) % 256;
				tmp = box[a];
				box[a] = box[j];
				box[j] = tmp;
				k = box[((box[a] + box[j]) % 256)];
				cipher[i] = (byte)(ord (Data[i]) ^ k);
			}

			return Encoding.GetEncoding (1252).GetString (cipher);
		}

		public static string Decrypt (string Password, string Data)
		{
			return Encrypt (Password, Data);
		}

		private static int ord (char ch)
		{
			return (int)(Encoding.GetEncoding (1252).GetBytes (ch + "")[0]);
		}

		private static char chr (int i)
		{
			byte[] bytes = new byte[1];
			bytes[0] = (byte)i;
			return Encoding.GetEncoding (1252).GetString (bytes)[0];
		}

		private static string pack (string packtype, string datastring)
		{
			int i, j, datalength, packsize;
			byte[] bytes;
			char[] hex;
			string tmp;
			
			datalength = datastring.Length;
			packsize = (datalength / 2) + (datalength % 2);
			bytes = new byte[packsize];
			hex = new char[2];
			
			for (i = j = 0; i < datalength; i += 2)
			{
				hex[0] = datastring[i];
				if (datalength - i == 1)
				{
					hex[1] = '0';
				}
				
				hex[1] = datastring[i + 1];
				tmp = new string (hex, 0, 2);
				try
				{
					bytes[j++] = byte.Parse (tmp, System.Globalization.NumberStyles.HexNumber);
				}
				catch
				{
				}
			}
			return Encoding.GetEncoding (1252).GetString (bytes);
		}

		public static string bin2hex (string bindata)
		{
			int i;
			byte[] bytes = Encoding.GetEncoding (1252).GetBytes (bindata);
			string hexString = "";
			for (i = 0; i < bytes.Length; i++)
			{
				hexString += bytes[i].ToString ("x2");
			}
			return hexString;
		}
	}
}
