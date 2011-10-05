//
// Tools.cs
//  
// Author:
//       sundown <>
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
using System.IO;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace SorentoLib
{
	public class Tools
	{

//		public static string ListToString<T> (List<T> List, string Seperator)
//		{
//			string result = string.Empty;
//
//			foreach (object value in List)
//			{
//				result += value.ToString () + Seperator;
//			}
//
//			return result;
//		}
//
////		public static List<T> StringToList<T> (string Value, string Seperator)
////		{
////
////
////		}

		public static string HexStringToAscii (string HexString)
		{
			StringBuilder sb = new StringBuilder ();
			for (int i = 0; i <= HexString.Length - 2; i += 2)
			{
				sb.Append (Convert.ToString (Convert.ToChar (Int32.Parse (HexString.Substring (i, 2), System.Globalization.NumberStyles.HexNumber))));
			}
			return sb.ToString ();
		}

		public static string DecodeFrom64 (string encodedData)
		{
			byte[] encodedDataAsBytes = System.Convert.FromBase64String (encodedData);

			string returnValue = System.Text.Encoding.Unicode.GetString (encodedDataAsBytes);

			return returnValue;
		}


		public static string Decrypt (string cryptedString)
		{

			byte[] bytes = ASCIIEncoding.ASCII.GetBytes ("12345678");
			if (String.IsNullOrEmpty (cryptedString))
			{
				throw new ArgumentNullException
					("The string which needs to be decrypted can not be null.");
			}
			DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider ();
			MemoryStream memoryStream = new MemoryStream (Convert.FromBase64String (cryptedString));
			CryptoStream cryptoStream = new CryptoStream (memoryStream, cryptoProvider.CreateDecryptor (bytes, bytes), CryptoStreamMode.Read);
			StreamReader reader = new StreamReader (cryptoStream);
			return reader.ReadToEnd ();
		}

		public static string Encrypt (string originalString)
		{
			byte[] bytes = ASCIIEncoding.ASCII.GetBytes ("12345678");
			if (String.IsNullOrEmpty (originalString))
			{
				throw new ArgumentNullException
					("The string which needs to be encrypted can not be null.");
			}
			DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider ();
			MemoryStream memoryStream = new MemoryStream ();
			CryptoStream cryptoStream = new CryptoStream (memoryStream, cryptoProvider.CreateEncryptor (bytes, bytes), CryptoStreamMode.Write);
			StreamWriter writer = new StreamWriter (cryptoStream);
			writer.Write (originalString);
			writer.Flush ();
			cryptoStream.FlushFinalBlock ();
			writer.Flush ();

			//string str;
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding ();
			return enc.GetString(memoryStream.GetBuffer ());
			//str =


			//return Convert.ToString (memoryStream.GetBuffer (), 0, (int)memoryStream.Length);

		}

		public static string EncodeTo64(string toEncode)
		{

			byte[] toEncodeAsBytes = System.Text.Encoding.Unicode.GetBytes(toEncode);

			string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

			return returnValue;
		}

		public static Encoding GetFileEncoding(string srcFile)
		{
			// *** Use Default of Encoding.Default (Ansi CodePage)
			Encoding enc = Encoding.Default;

			// *** Detect byte order mark if any - otherwise assume default
			byte[] buffer = new byte[5];
			FileStream file = new FileStream(srcFile, FileMode.Open);
			file.Read(buffer, 0, 5);
			file.Close();

			if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
				enc = Encoding.UTF8;
			else if (buffer[0] == 0xfe && buffer[1] == 0xff)
				enc = Encoding.Unicode;
			else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
				enc = Encoding.UTF32;
			else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
				enc = Encoding.UTF7;
			return enc;
		}

		

	}
}
