get : function (module, key)
{
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Config.Get", "data", "POST", false);	

	var content = new Array();

	if (key == null)
	{
		var keys = {};
		for (index = 0; index < module.length; index++)
		{
			keys["key"+ index] = module[index];
		}
		
		content["keys"] = keys;
	}
	else
	{
		content["module"] = module;
		content["key"] = key;
	}

	request.send (content);
			
	if (key == null)
	{													
		return request.respons ()["data"];
	}
	else
	{
		return request.respons ()["value"];
	}
},

set : function (module, key, value)
{					
	var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Services.Config.Set", "data", "POST", false);	

	var content = new Array ();

	if (key == null && value == null)
	{
		content["keys"] = module;
	}
	else
	{
		content['module'] = module;
		content['key'] = key;
		content['value'] = value;
	}

	request.send (content);

	return true;
}				