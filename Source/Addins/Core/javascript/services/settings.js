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

	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Settings.Set", "data", "POST", false);	

	var content = new Array ();
	content["settings"] = keys;

	request.send (content);

	return true;
},

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

	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Settings.Get", "data", "POST", false);	

	var content = new Array();	
	content["settings"] = keys;
			
	request.send (content);
			
			
	var settings = request.respons ()["settings"];
	var result = {};
	for (index in settings)
	{
		for (key in settings[index])
		{
			result[key] = settings[index][key];
		}
	}
						
	return result;
}