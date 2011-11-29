new : function (usergroup)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Usergroup.New", "data", "POST", false);			
	request.send (usergroup);
	
	return request.respons ()["sorentolib.usergroup"];		
},

load : function (id)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Usergroup.Load", "data", "POST", false);	
	
	var content = new Array ();
	content["id"] = id;
				
	request.send (content);
	
	return request.respons ()["sorentolib.usergroup"];
},

save : function (usergroup)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Usergroup.Save", "data", "POST", false);				
	
	var content = new Array ();
	content["sorentolib.usergroup"] = usergroup;
	request.send (content);		
	
	return true;
},

delete : function (id)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Usergroup.Delete", "data", "POST", false);	
	
	var content = new Array ();
	content["id"] = id;
	
	request.send (content);				
	
	return true;
},		

list : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Usergroup.List", "data", "POST", false);					
	request.send ();
											
	return request.respons ()["sorentolib.usergroups"];
},

accesslevels : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Usergroup.Accesslevels", "data", "POST", false);					
	request.send ();
																													
	return request.respons ()["sorentolib.enums.accesslevels"];
}
