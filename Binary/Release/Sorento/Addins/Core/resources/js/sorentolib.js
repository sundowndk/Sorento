// ---------------------------------------------------------------------------------------------------------------
// PROJECT: sorentolib
// ---------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------
// CLASS: sorentoLib
// ---------------------------------------------------------------------------------------------------------------
var sorentoLib =
{
	// ---------------------------------------------------------------------------------------------------------------
	// CLASS: user
	// ---------------------------------------------------------------------------------------------------------------
	user :
	{
		create : function (username, email)
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.User.New", "data", "POST", false);			
			
			var content = new Array ();
			content["username"] = username;
			content["email"] = email;
			request.send (content);
			
			return request.respons ()["sorentolib.user"];		
		},
		
		load : function (id)
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.User.Load", "data", "POST", false);	
			
			var content = new Array ();
			content["id"] = id;
						
			request.send (content);
			
			return request.respons ()["sorentolib.user"];
		},
		
		save : function (user)
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.User.Save", "data", "POST", false);				
			
			var content = new Array ();
			content["sorentolib.user"] = user;
				
			request.send (content);		
			
			return true;
		},
		
		delete : function (id)
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.User.Delete", "data", "POST", false);	
			
			var content = new Array ();
			content["id"] = id;
			
			request.send (content);				
			
			return true;
		},
		
		list : function ()
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.User.List", "data", "POST", false);	
				
			request.send ();
													
			return request.respons ()["sorentolib.users"];
		},
		
		changePassword : function (userid, newPassword, oldPassword)
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.User.ChangePassword", "data", "POST", false);
			
			var content = new Array ();
			content["userid"] = userid;
			content["newpassword"] = newPassword;
			if (oldPassword != null)
			{
				content["oldpassword"] = oldPassword;
			}
				
			request.send (content);
			
			return request.respons ()["result"];	
		},
		
		isUsernameInUse : function (username, id)
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.User.IsUsernameInUse", "data", "POST", false);	
		
			var content = new Array ();	
			content['username'] = username;		
			if (id != null)
			{
				content['id'] = id;
			}
			
			request.send (content);
		
		 	return request.respons ()["result"];
		},
		
		isEmailInUse : function (email, id)
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.User.IsEmailInUse", "data", "POST", false);	
		
			var content = new Array ();
		
			content["email"] = email;
			if (id != null)
			{
				content["id"] = id;
			}
		
			request.send (content);
		
			return request.respons ()["result"];
		}		
	},

	// ---------------------------------------------------------------------------------------------------------------
	// CLASS: usergroup
	// ---------------------------------------------------------------------------------------------------------------
	usergroup :
	{
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
	},

	// ---------------------------------------------------------------------------------------------------------------
	// CLASS: session
	// ---------------------------------------------------------------------------------------------------------------
	session :
	{
		getCurrent : function ()
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Session.GetCurrent", "data", "POST", false);
			request.send ();
		
			return request.respons ()["sorentolib.session"];
		},
		
		loggedIn : function ()
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Session.LoggedIn", "data", "POST", false);
			request.send ();
		
			return request.respons ()["value"];
		},
		
		login : function (username, password)
		{
			var content = new Array ();
			content["username"] = username;
			content["password"] = password;
		
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Session.Login", "data", "POST", false);				
			request.send (content);
		
			return request.respons ();
		},
		
		logout : function ()
		{
			var request = new SNDK.ajax.request ("/", "cmd=Ajax;cmd.function=SorentoLib.Session.Logout", "data", "POST", false);				
			request.send ();
		
			return request.respons ();
		}		
	},

	// ---------------------------------------------------------------------------------------------------------------
	// CLASS: media
	// ---------------------------------------------------------------------------------------------------------------
	media :
	{
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
	},

	// ---------------------------------------------------------------------------------------------------------------
	// CLASS: mediaTransformation
	// ---------------------------------------------------------------------------------------------------------------
	mediaTransformation :
	{
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
	},

	// ---------------------------------------------------------------------------------------------------------------
	// CLASS: services
	// ---------------------------------------------------------------------------------------------------------------
	services :
	{
		// ---------------------------------------------------------------------------------------------------------------
		// CLASS: config
		// ---------------------------------------------------------------------------------------------------------------
		config :
		{
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
		},
	
		// ---------------------------------------------------------------------------------------------------------------
		// CLASS: snapshot
		// ---------------------------------------------------------------------------------------------------------------
		snapshot :
		{
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
		},
	
		// ---------------------------------------------------------------------------------------------------------------
		// CLASS: addins
		// ---------------------------------------------------------------------------------------------------------------
		addins :
		{
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
		}
	}
}

