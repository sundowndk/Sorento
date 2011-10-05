// 
// Encryption.cs
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
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace SorentoLib.Services
{
	public static class Crypto
	{
		#region Public Static Fields
		private static RSACryptoServiceProvider ServiceProvider;
		private static RSAParameters Parameters;

		private static string PrivateKeyPath = "data/rsa/sorento.private.xml";
		private static string PublicKeyPath = "data/rsa/sorento.public.xml";
		#endregion

		#region Public Static Methods
		public static void Initalize ()
		{
			if ((!File.Exists (SorentoLib.Services.Crypto.PrivateKeyPath)) || (!File.Exists (SorentoLib.Services.Crypto.PublicKeyPath)))
			{
//				SorentoLib.Services.Logging.LogInfo ("No RSA encryption keys found. New keys will be generated.");

				try
				{
					File.Delete (SorentoLib.Services.Crypto.PrivateKeyPath);
					File.Delete (SorentoLib.Services.Crypto.PublicKeyPath);
				}
				catch
				{
				}

				SorentoLib.Services.Crypto.ServiceProvider = new RSACryptoServiceProvider (1024);

				string privatekey = SorentoLib.Services.Crypto.ServiceProvider.ToXmlString (true);
				TextWriter writer = new StreamWriter (SorentoLib.Services.Crypto.PrivateKeyPath);
				writer.Write (privatekey);
				writer.Close ();

				string publickey = SorentoLib.Services.Crypto.ServiceProvider.ToXmlString (false);
				writer = new StreamWriter (SorentoLib.Services.Crypto.PublicKeyPath);
				writer.Write (publickey);
				writer.Close ();
			}
			else
			{
//				CspParameters cspparams = new CspParameters ();
//				cspparams.Flags = CspProviderFlags.UseMachineKeyStore;

				SorentoLib.Services.Crypto.ServiceProvider = new RSACryptoServiceProvider ();
				System.IO.StreamReader reader = new StreamReader (SorentoLib.Services.Crypto.PrivateKeyPath);
				string privatekey = reader.ReadToEnd ();

				SorentoLib.Services.Crypto.ServiceProvider.FromXmlString (privatekey);
			}

			SorentoLib.Services.Crypto.Parameters = SorentoLib.Services.Crypto.ServiceProvider.ExportParameters (false);

			SorentoLib.Services.Logging.LogInfo("Service enabled: Crypto");

//			SorentoLib.Services.Logging.LogInfo ("Successfully initalized RSA service provider.");
		}

		public static string EncryptExponent
		{
			get
			{
				return SorentoLib.Tools.StringHelper.BytesToHexString(SorentoLib.Services.Crypto.Parameters.Exponent);
			}
		}

		public static string Modulus
		{
			get
			{
				return SorentoLib.Tools.StringHelper.BytesToHexString(SorentoLib.Services.Crypto.Parameters.Modulus);
			}
		}

		private static string DecryptExponent
		{
			get
			{
				return SorentoLib.Tools.StringHelper.BytesToHexString(SorentoLib.Services.Crypto.Parameters.D);
			}
		}

		public static byte[] Encrypt (string Data)
		{
			byte[] result;

			ASCIIEncoding enc = new ASCIIEncoding ();
			int numOfChars = enc.GetByteCount (Data);
			byte[] tempArray = enc.GetBytes (Data);
			result = SorentoLib.Services.Crypto.ServiceProvider.Encrypt (tempArray, false);

			return result;
		}

		public static byte[] Decrypt (byte[] Data)
		{
			byte[] result;

			result = SorentoLib.Services.Crypto.ServiceProvider.Decrypt (Data, false);

			return result;
		}
		#endregion
	}
}
