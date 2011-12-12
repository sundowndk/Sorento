enableAddin : function (id)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Addins.EnableAddin", "data", "POST", false);
	
	var content = new Array ();
	content["id"] = id;
				
	request.send (content);
	
	return true;
},

disableAddin : function (id)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Addins.DisableAddin", "data", "POST", false);

	var content = new Array ();
	content["id"] = id;
				
	request.send (content);
	
	return true;
},

list : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Addins.List", "data", "POST", false);					
	request.send ();
										
	return request.respons ()["sorentolib.services.addins"];
}