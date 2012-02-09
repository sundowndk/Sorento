//
// LogError.cs
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
	public class LogError
	{
		#region GENERIC
		public static string Exception = "Method: {0}, Error: {1}, Caller: {2}";
		#endregion

		#region SESSION
		public static string SessionLoadUser = "[SORENTOLIB.SESSION]: Cannot load user with id: {0}, session will continue without user attached.";
		public static string SessionSignIn = "[SORENTOLIB.SESSION]: Failed to singin user '{0}'";
		public static string SessionSignOut = "[SORENTOLIB.SESSION]: Failed to signout user '{0}'";
		#endregion

		#region USER
		public static string UserCreateDefaultUsergroup = "[SORENTOLIB.USER]: Default usergroup with id: {0}, does not exist. User has been created without default usergroup, this is a security issue.";
		public static string UserLoadUsergroup = "[USER]: Cannot load usergroup with id: {0}.";
		public static string UserLoadAvatar = "[SORENTOLIB.USER]: Cannot load media with id: {0}.";
		#endregion

		#region USERGROUP
		public static string UsergroupListUsergroup = "[SORENTOLIB.USERGROUP]: Cannot load usergroup with id: {0}. Usergroup has been excluded from list.";
		#endregion

		#region MEDIA
		public static string MediaLoadUsergroup = "[SORENOTLIB.MEDIA]: Cannot load usergroup with id: {0}. Usergroup has been removed from media.";
		public static string MediaLoadVariant = "[SORENOTLIB.MEDIA]: Cannot load variant with id: {0}. Variant has been removed from media.";		
		public static string MediaGarbageCollector =  "[SORENTOLIB.MEDIA]: GarbageCollector.";
		public static string MediaList = "[SORENTOLIB.MEDIA]: Cannot load MEDIA with id: {0}, will be excluded from list.";
		#endregion

		#region SERVICES.CONFIG
		public static string ServicesConfigSave = "[SORENOTLIB.SERVICES.CONFIG]: Failed to write configuration file:";
		public static string ServicesConfigSaveException = "[SORENOTLIB.SERVICES.CONFIG]: {0}";
		#endregion

		#region SERVICES.SNAPSHOT
		public static string ServicesSnapshotList = "[SORENTOLIB.SERVICES.SNAPSHOT]: Cannot load snapshot with id: {0}. Snapshot has been excluded from list.";
		#endregion

	}
}

