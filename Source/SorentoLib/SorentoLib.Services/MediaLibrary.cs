// 
// MediaLibrary.cs
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

using SNDK;

namespace SorentoLib.Services
{
	public static class MediaLibrary
	{
//		public static Media AddFromByteArray ()
//
//		public static Media AddFromByteArray (byte[] Data, string Filename)
//		{
//			Media media = new Media ();
//			media.Path = System.IO.Path.GetDirectoryName (Filename) +"/";
//			media.Filename = System.IO.Path.GetFileName (Filename);
//			media.Size = Data.LongLength;
//
//			if (!Directory.Exists (SorentoLib.Services.Config.Get("core", "mediapath") + media.Path))
//			{
//				Directory.CreateDirectory (SorentoLib.Services.Config.Get("core", "mediapath") + media.Path);
//			}
//
//			if (File.Exists (SorentoLib.Services.Config.Get("core", "mediapath") + media.Path + media.Filename))
//			{
//				media.Filename = IO.IncrementFilename (SorentoLib.Services.Config.Get("core", "mediapath") + media.Path + media.Filename);
//			}
//
//			FileStream filestream = File.Create(SorentoLib.Services.Config.Get("core", "mediapath") + media.Path + media.Filename);
//			BinaryWriter binarywriter = new BinaryWriter(filestream);
//			binarywriter.Write(Data);
//			binarywriter.Close();
//			filestream.Close();
//			Data = null;
//
//			media.Mimetype = Toolbox.IO.GetMimeType (SorentoLib.Services.Config.Get("core", "mediapath") + media.Path + media.Filename);
//
//			media.Save ();
//
//			return media;
//		}

		public static void ChangeStatus (SorentoLib.Media Media, SorentoLib.Enums.MediaStatus Status)
		{
			
		}

		public static void AddFromFile (string Filename)
		{

		}

		public static void AddVariant (Guid Id)
		{

		}

		public static void Rename (Guid Id, string Filename)
		{

		}

		public static void Move (Guid Id, string Path)
		{

		}

		public static void Remove (Guid Id)
		{

		}

		public static void Update (Guid Id)
		{

		}

	}
}
