getCurrent : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Session.GetCurrent", "data", "POST", false);
	request.send ();

	return request.respons ()["sorentolib.session"];
},

loggedIn : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Session.LoggedIn", "data", "POST", false);
	request.send ();

	return request.respons ()["value"];
},

login : function (username, password)
{
	var content = new Array ();
	content["username"] = username;
	content["password"] = password;

	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Session.Login", "data", "POST", false);				
	request.send (content);

	return request.respons ();
},

logout : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Session.Logout", "data", "POST", false);				
	request.send ();

	return request.respons ();
}		