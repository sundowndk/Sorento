//
// ConfigKey.cs
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

using System;

namespace SorentoLib.Enums
{
	public enum ConfigKey
	{
		core_hostname,
		core_sessiontimeout,
		core_enablecache,
		core_encoding,
		core_showexceptions,
		core_loglevel,
		core_authenticationtype,
		core_defaultusergroupid,
		core_filenameincrementformat,
		core_mediamaxtempage,

		path_content,
		path_html,
		path_media,
		path_publicmedia,
		path_snapshot,
		path_script,
		path_temp,

		database_driver,
		database_hostname,
		database_username,
		database_password,
		database_database,
		database_prefix,

		smtp_server,
		smtp_encoding,

		fastcgi_maxconnections,
		fastcgi_maxrequests,
		fastcgi_multiplexconnections,





		media_tempmaxage,

		core_bootstrap,

		core_httperror500url,
		core_httperror404url
	}
}



//    <hostname>sorentotest.sundown.dk</hostname>
//    <sessiontimeout>1800</sessiontimeout>
//    <enablecache>false</enablecache>
//    <defaultencoding>UTF-8</defaultencoding>
//    <showexceptions>true</showexceptions>


//	<pathscript>data/scripts/</pathscript>
//    <enabled>true</enabled>
//    <contentencoding>ISO-8859-15</contentencoding>
//    <verificationtimeout>43200</verificationtimeout>
//    <authenticationtype>plaintext</authenticationtype>
//    <enablersalogin>false</enablersalogin>
//    <pathcontent>../Content</pathcontent>
//    <pathhtmlroot>../../html/</pathhtmlroot>
//    <pathtmp>tmp/</pathtmp>
//    <pathsnapshot>data/snapshots/</pathsnapshot>
//    <pathmedia>../Media/</pathmedia>
//    <pathpublicmedia>../../html/media/</pathpublicmedia>
//    <defaultusergroup>09b2b48f-9686-4232-b699-b5cc013998d0</defaultusergroup>
//    <cgiexec>Sorento.exe</cgiexec>
//    <cgiurl>/cgi-bin/</cgiurl>
//    <loglevel>info, warning, error, fatalerror, debug</loglevel>
//    <offlineurl>info, warning, error, fatalerror</offlineurl>
//    <encoding>UTF-8</encoding>
