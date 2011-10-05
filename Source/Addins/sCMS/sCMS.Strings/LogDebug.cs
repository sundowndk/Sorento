// 
// LogDebug.cs
//  
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2011 Rasmus Pedersen
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
namespace sCMS.Strings
{
	public class LogDebug
	{
		#region TEMPLATE
		public static string TemplateList = "[sCMS.TEMPLATE]: Cannot load TEMPLATE with id: {0}, will be excluded from list.";

		#endregion

		#region PAGE
		public static string PageSetGuid = "[sCMS.PAGE]: PAGE does not contain a FIELD with id: {0}";
		public static string PageSetName = "[sCMS.PAGE]: PAGE does not contain a FIELD with name: {0}";
		public static string PageList = "[sCMS.PAGE]: Cannot load PAGE with id: {0}, will be excluded from list.";
		#endregion

		#region Root
		public static string RootList = "[sCMS.ROOT]: Cannot load ROOT with id: {0}, will be exluded from list.";
		#endregion

		#region GLOBAL
		public static string GlobalList = "[sCMS.GLOBAL]: Cannot load GLOBAL with id: {0}, will be excluded from list.";
		#endregion

		#region Collection
		public static string CollectionList = "[sCMS.COLLECTION]: Cannot load COLLECTION with id: {0}, will be excluded from list.";
		#endregion

		#region CollectionSchema
		public static string CollectionSchemaList = "[sCMS.COLLECTIONSCHEMA]: Cannot load COLLECTIONSCHEMA with id: {0}, will be excluded from list.";
		#endregion

		#region STYLESHEET
		public static string StylesheetLoadContent = "[sCMS.STYLESHEET]: Cannot load content for stylesheet: {0}";
		public static string StylesheetList = "[sCMS.STYLEHSEET]: Cannot load STYLESHEET with path: '{0}', will be excluded from list.";
		#endregion

	}
}

