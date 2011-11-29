logout : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Session.Logout", "data", "POST", false);				
	request.send ();

	return request.respons ();
}		