create : function (options)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Snapshot.New", "data", "POST", true);

	var content = new Array ();				

	request.onLoaded (options.onDone);										
	request.send (content);
},

develop : function (options)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Snapshot.Develop", "data", "POST", true);

	var content = new Array ();			
	content["id"] = options.id;	

	request.onLoaded (options.onDone);										
	request.send (content);
},						
		
load : function (id)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Snapshot.Load", "data", "POST", false);	

	var content = new Array ();
	content["id"] = id;
				
	request.send (content);

	return request.respons ();
},
	
remove : function (id)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Snapshot.Delete", "data", "POST", false);	

	var content = new Array ();
	content["id"] = id;

	request.send (content);				

	return true;
},		

list : function ()
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Snapshot.List", "data", "POST", false);					
	request.send ();
										
	return request.respons ()["snapshots"];
}