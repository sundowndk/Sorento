get : function (attributes)
{
	var keys = new Array ();						
	if (attributes.key != null)
	{
		keys[0] = {};
		keys[0].value = attributes.key;
	}
							
	if (attributes.keys != null)
	{
		for (key in attributes.keys)
		{
			var index = keys.length;
			keys[index] = {};
			keys[index].value = attributes.keys[key];									
		}							
	}											

	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Config.Get", "data", "POST", false);	

	var content = new Array();	
	content["config"] = keys;
			
	request.send (content);
			
			
	var config = request.respons ()["config"];
	var result = {};
	for (index in config)
	{
		for (key in config[index])
		{
			result[key] = config[index][key];
		}
	}
						
	return result;
},

set : function (attributes)
{					
	var keys = new Array ();
	
	if (attributes.keys != null)
	{
		for (key in attributes.keys)
		{
			var index = keys.length;			
			keys[index] = {};
			keys[index].key = key;
			keys[index].value = attributes.keys[key];
		}
	}							

	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Config.Set", "data", "POST", false);	

	var content = new Array ();
	content["config"] = keys;

	request.send (content);

	return true;
}				