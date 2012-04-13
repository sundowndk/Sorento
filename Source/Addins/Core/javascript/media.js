load : function (id)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Media.Load", "data", "POST", false);	
	
	var content = new Array ();
	content["id"] = id;
				
	request.send (content);
	
	return request.respons ()["sorentolib.media"];
},

save : function (media)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Media.Save", "data", "POST", false);				
	
	var content = new Array ();
	content["sorentolib.media"] = media;
		
	request.send (content);		
	
	return true;
},
