create : function (mediatransformation)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.New", "data", "POST", false);			
	request.send (mediatransformation);
	
	return request.respons ();		
},

load : function (id)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.Load", "data", "POST", false);	
	
	var content = new Array ();
	content["id"] = id;
				
	request.send (content);
	
	return request.respons ();
},

save : function (mediatransformation)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.Save", "data", "POST", false);				
	request.send (mediatransformation);		
	
	return true;
},

remove : function (id)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.Delete", "data", "POST", false);	
	
	var content = new Array ();
	content["id"] = id;
	
	request.send (content);				
	
	return true;
},		

list : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.List", "data", "POST", false);					
	request.send ();
											
	return request.respons ()["mediatransformations"];
}
