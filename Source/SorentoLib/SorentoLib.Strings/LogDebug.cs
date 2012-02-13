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
namespace SorentoLib.Strings
{
	public class LogDebug
	{
		#region EXCEPTIONS
		public static string ExceptionUnknown = "[{0}] Exception: {1}";
		#endregion

		#region SESSION
		public static string SessionStats = "[SORENTOLIB.SESSION]: Stats collected for sessions.";
		public static string SessionGarbageCollector = "[SORENTOLIB.SESSION]: Garbage collected for sessions.";
		public static string SessionTimeout = "[SORENTOLIB.SESSION]: Session with id: {0}, has expired.";
		public static string SessionUserSignIn = "[SORENTOLIB.SESSION]: User '{0}' has sucessfully signed in.";
		public static string SessionUserSignOut = "[SORENTOLIB.SESSION]: User '{0}' has succesfully singed out.";
		#endregion

		#region USER
		public static string UserStats = "[USER]: Stats collected for users.";
		#endregion

		#region USERGROUP
		public static string UsergroupStats = "[SORENTOLIB.USERGROUP]: Stats collectd for usergroups.";
		#endregion

		#region MEDIA
		public static string MediaStats = "[SORENTOLIB.MEDIA]: Stats collected for media.";
		public static string MediaGarbageCollector =  "[SORENTOLIB.MEDIA]: Garbage collected for media.";
		#endregion
	}
}

