new : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.New", "data", "POST", false);			
	request.send ();
	
	return request.respons ()["sorentolib.mediatransformation"];		
},

load : function (id)
{
	var content = new Array ();
	content["id"] = id;

	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.Load", "data", "POST", false);		
	request.send (content);
	
	return request.respons ()["sorentolib.mediatransformation"];
},

save : function (mediatransformation)
{
	var content = new Array ();
	content["sorentolib.mediatransformation"] = mediatransformation;

	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.Save", "data", "POST", false);				
	request.send (content);		
	
	return true;
},

delete : function (id)
{
	var content = new Array ();
	content["id"] = id;

	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.Delete", "data", "POST", false);	
	request.send (content);				
	
	return true;
},		

list : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.MediaTransformation.List", "data", "POST", false);					
	request.send ();
											
	return request.respons ()["sorentolib.mediatransformations"];
}
