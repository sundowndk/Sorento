//
// Exception.cs
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

namespace SorentoLib.Strings
{
	public class Exception
	{
		#region RENDER
		public static string RenderVariableTypeNotFound = "${0} is an unknown type";
		public static string RenderVariableVariableNotFound = "${0} is not defined";
		public static string RenderVariableVariableIsNull = "${0} is null";
		public static string RenderVariableMemberNotFound = "${0} does not have '{1}'";
		public static string RenderMethodMemberNotFound = "{0} does not have '{1}'";
		public static string RenderMethodTypeNotFound = "{0} is an unknown type";
		public static string RenderParseTypeName = "Malformed method call '{0}'";
		public static string RenderTemplateNotFound = "";

		#endregion

		#region RESPONDER
		public static string ResponderPageNotFound = "Page '{0}' was not found.";
		#endregion

		#region SESSION
		public static string SessionLoad = "Session with id: {0} was not found.";
		public static string SessionSave = "Could not save session with id: {0}";
		public static string SessionDelete = "Could not delete session with id: {0}";
		public static string SessionExpired = "Session '{0}' has expired.";
		#endregion



		#region SERVICES.CONFIG
		public static string ServicesConfigKeyNotFound = "Config key: {0} was not found.";
		public static string ServicesConfigKeyNotValidType = "Config key: {0} is not of type: {1}";
		#endregion

		#region SERVICES.STATS
		public static string ServicesStatsKeyNotFound = "Stats key: {0} was not found.";
		public static string ServicesStatsKeyNotValidType = "Stats key: {0} is not of type: {1}";
		#endregion

		#region SERVICES.DATASTORE
		public static string ServicesDatastoreLoadLocation = "Location: {0} was not found.";
		public static string ServicesDatastoreLoadGuid = "Location with id: {0} was not found.";
		public static string ServicesDatastoreSave = "Location with id: {0} could not be saved.";
		public static string ServicesDatastoreDeleteGuid = "Location with id: {0} could not be deleted.";
		public static string ServicesDatastoreDeleteLocation = "Location: {0} could not be deleted.";
		public static string ServicesDatastoreLocationNotFound = "Location: {0} was not found.";
		public static string ServicesDatastoreLocationNotValidType = "Location: {0} is not of type: {1}";
		#endregion

		#region SERVICES.SNAPSHOT
		public static string ServicesSnapshotLoad = "Could not load snapshot with id: {0}";
		public static string ServicesSnapshotLoadManifest = "Could not load snapshot with id: {0}. Manifest seams to be damaged or missing.";
		public static string SerivcesSnapshotDelete = "Could not delete snapshot with id: {0}";
		#endregion

		#region USER
		public static string UserCreateUsername = "Could not create user. Username '{0}' is allready in use.";
		public static string UserCreateEmail = "Could not create user. Email '{0}̈́' is allready in use.";
		public static string UserSetUsername = "Username '{0}' is allready in use.";
		public static string UserSetEmail = "Email '{0}' is allready in use.";
		public static string UserLoad = "Could not load user. Please make sure you supplied a valid Id or Username.";
		public static string UserLoadGuid = "Could not load user with id: {0}";
		public static string UserLoadUsername = "Could not load user with username: {0}";
		public static string UserSave = "Could not save user, with id: {0}";
		public static string UserDeleteGuid = "Could not delete user with id: {0}";
		public static string UserDeleteUsername = "Could not delete user with username: {0}";
		public static string UserFromXMLDocument = "Failed to create USER from XMLDocument.";
		#endregion

		#region USERGROUP
		public static string UsergroupLoad = "Could not load usergroup with id: {0}";
		public static string UsergroupSave =  "Could not save usergroup.";
		public static string UsergroupDelete = "Could not delete usergroup with id: {0}";
		public static string UsergroupFromXMLDocument = "Failed to create USERGROUP from XMLDocument.";
		#endregion

		#region MEDIA
		public static string MediaLoad = "Could not load media with id: {0}";
		public static string MediaSave = "Could not save media with id: {0}";
		public static string MediaSaveData = "Could not save media with id: {0}";
		public static string MediaDelete = "Could not delete media with id: {0}";
		#endregion

		#region RENDER.TEMPLATE
		public static string RenderTemplateLoad = "Could not load template with filenname: {0}";
		#endregion

		#region AJAX.REQUEST
		public static string AjaxRequestXPathNotFound = "Could not find xpath: {0}";
		public static string AjaxRequestCouldNotCastType = "Could not cast xpath: {0} to type: {1}. Xml node type is given as: {2} - \n {3}";
		#endregion
	}
}

