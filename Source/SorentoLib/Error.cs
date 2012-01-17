//
// Error.cs
//  
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2009 Rasmus Pedersen
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

#region Includes
using System;
#endregion

namespace SorentoLib
{		
	public class Error
	{
		#region Static Definitions
		public static string[] ErrorTexts = new string[3000];

		public string _title;
		public string _text;
		#endregion
		
		#region Defintions
		private int _id = 0;
		#endregion
		
		#region Values
		/// <value>
		/// Id
		/// </value>
		public int Id
		{
			get { return this._id; }
			set { this._id = value; }
		}			
		
//		public string Text
//		{
//			get { return Error.ErrorTexts[this._id]; }
//		}

		public string Title
		{
			get
			{
				return this._title;
			}
		}

		public string Text
		{
			get
			{
				return this._text;
			}
		}
		
		public bool Exception
		{
			get 
			{ 
				if (this._id > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}			
		
		#endregion		
				
		#region Constructor
		public Error()
		{						
		}

		public Error (Exception Exception)
		{
			this._title = Exception.Message;
			this._text = Exception.StackTrace.Replace ("\n", "<br>");
		}
		#endregion
		
		#region Methods
		public static void Init()
		{					
			// 1 - 100 CORE
			Error.ErrorTexts[10] = "CORE:CMD - Missing cmd query.";
			
			// 100 - 200 USER/GROUPS
			Error.ErrorTexts[100] = "USER:LOAD - Invalid guid";
			Error.ErrorTexts[101] = "USER:SAVE - Invalid guid";
			Error.ErrorTexts[102] = "USER:DELETE - Invalid guid";
			Error.ErrorTexts[103] = "USER:LIST - Could not fetch list";
			Error.ErrorTexts[104] = "USER:CREATE - Could not create user";
			Error.ErrorTexts[110] = "USERGROUP:LOAD - Invalid guid";
			Error.ErrorTexts[111] = "USERGROUP:SAVE - Invalid guid";
			Error.ErrorTexts[112] = "USERGROUP:DELETE - Invalid guid";
			Error.ErrorTexts[113] = "USERGROUP:LIST - Could not fetch list";
			Error.ErrorTexts[114] = "USERGROUP:CREATE - Could not create usergroup";			
			
			// 200 - 300 MEDIA
			Error.ErrorTexts[200] = "MEDIA:LOAD - Invalid Guid";
			Error.ErrorTexts[201] = "MEDIA:SAVE - Invalid Guid";
			Error.ErrorTexts[202] = "MEDIA:DELETE - Invalid Guid";
			
			// 1000 - 1500 AUTOSUITE;
			Error.ErrorTexts[1000] = "";			
		}
		#endregion
	}
}
