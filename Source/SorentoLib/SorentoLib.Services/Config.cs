//
// Config.cs
//
// Author:
//   Rasmus Pedersen (rasmus@akvaservice.dk)
//
// Copyright (C) 2009 Rasmus Pedersen
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Xml;
using System.Timers;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SorentoLib.Services
{
	public static class Config
	{
		#region Private Static Fields
		private static Regex ExpMatchXmlError = new Regex (".* '(.*)'.*xml (.*)", RegexOptions.Compiled);

		//		.* '(.*)'.*xml (.*)
		private static string _path = "../config.xml";
		private static List<Key> _keys;
		private static List<Key> _defaults;
		private static System.Timers.Timer _writedelay;
		private static object _lock;
		private static FileSystemWatcher _watcher;
		#endregion

		#region Public Static Methods
		public static void Initialize ()
		{
			_lock = new object ();
			_keys = new List<Key> ();
			_defaults = new List<Key> ();

			// SetDefaults
			SetDefaults ();

			// Parse conf
			SorentoLib.Services.Config.Read ();
			
			// Monitor configuration file for changes.
			_watcher = new FileSystemWatcher ();
			_watcher.Path = "../";
			_watcher.Changed += EventHandlerChanged;
			_watcher.NotifyFilter = NotifyFilters.LastWrite;
			_watcher.EnableRaisingEvents = true;

			// Setup delayed write of conf.
			_writedelay = new System.Timers.Timer ();
			_writedelay.Elapsed += EventHandlerDelay;
			_writedelay.Interval = 10000;
			_writedelay.AutoReset = false;

			// LOG: LogInfo.RuntimeServiceInitialized
//			Services.Logging.LogInfo (string.Format (Strings.LogInfo.RuntimeServiceInitialized, "Config"));
		}

		private static void SetDefaults ()
		{
			#region CORE
			SetDefault (Enums.ConfigKey.core_hostname, "localhost");
			SetDefault (Enums.ConfigKey.core_sessiontimeout, 1800);
			SetDefault (Enums.ConfigKey.core_enablecache, false);
			SetDefault (Enums.ConfigKey.core_encoding, "UTF-8");
			SetDefault (Enums.ConfigKey.core_showexceptions, true);
			SetDefault (Enums.ConfigKey.core_loglevel, "info, warning, error, fatalerror, debug");
			SetDefault (Enums.ConfigKey.core_authenticationtype, "plaintext");
			SetDefault (Enums.ConfigKey.core_defaultusergroupid, Guid.Empty);
			#endregion

			#region DATABASE
			SetDefault (Enums.ConfigKey.database_driver, string.Empty);
			SetDefault (Enums.ConfigKey.database_hostname, "localhost");
			SetDefault (Enums.ConfigKey.database_username, string.Empty);
			SetDefault (Enums.ConfigKey.database_password, string.Empty);
			SetDefault (Enums.ConfigKey.database_database, string.Empty);
			SetDefault (Enums.ConfigKey.database_prefix, string.Empty);
			#endregion

			#region SMTP
			SetDefault (Enums.ConfigKey.smtp_server, "localhost");
			SetDefault (Enums.ConfigKey.smtp_encoding, "UTF-8");
			#endregion

			#region FASTCGI
			SetDefault (Enums.ConfigKey.fastcgi_maxconnections, 500);
			SetDefault (Enums.ConfigKey.fastcgi_maxrequests, 500);
			SetDefault (Enums.ConfigKey.fastcgi_multiplexconnections, true);
			#endregion

			#region PATH
			SetDefault (Enums.ConfigKey.path_content, "../Content");
			SetDefault (Enums.ConfigKey.path_html, "../../html/");
			SetDefault (Enums.ConfigKey.path_media, "../Media/");
			SetDefault (Enums.ConfigKey.path_publicmedia, "../../html/");
			SetDefault (Enums.ConfigKey.path_snapshot, "data/snapshots/");
			SetDefault (Enums.ConfigKey.path_script, "data/scripts/");
			SetDefault (Enums.ConfigKey.path_temp, "tmp/");
			#endregion
		}

		public static T Get<T> (object Key)
		{
			string module = Key.ToString ().Split ("_".ToCharArray ())[0];
			string name = Key.ToString ().Split ("_".ToCharArray ())[1];

			return (T)Get<T> (module, name);
		}

//		public static T Get<T> (Enums.ConfigKey Key)
//		{
//			string module = Key.ToString ().Split ("_".ToCharArray ())[0];
//			string name = Key.ToString ().Split ("_".ToCharArray ())[1];
//
//			return (T)Get<T> (module, name);
//		}

		private static T Get<T> (string Key)
		{
			string module = Key.Split ("_".ToCharArray ())[0];
			string name = Key.Split ("_".ToCharArray ())[1];

			return (T)Get<T> (module, name);
		}

		private static T Get<T> (string Module, string Name)
		{
			try
			{
				switch (typeof (T).Name.ToLower ())
				{
					case "guid":
						return (T)Convert.ChangeType (new Guid (Get (Module, Name)), typeof(T));

					default:
						return (T)Convert.ChangeType (Get (Module, Name), typeof(T));
				}
			}
			catch
			{
				throw new Exception (string.Format (Strings.Exception.ServicesConfigKeyNotValidType, Module +"."+ Name, typeof (T).Name));
			}
		}

		public static bool Exists (object Key)
		{
			string module = Key.ToString ().Split ("_".ToCharArray ())[0];
			string name = Key.ToString ().Split ("_".ToCharArray ())[1];

			return Exist (module, name);
		}

		private static bool Exist (string Module, String Name)
		{
			Key key = _keys.Find (delegate (Key k) { return k.Path == Module + "_" + Name; });
			if (key != null)
			{
				return true;
			}

			return false;
		}

		public static void SetDefault (object Key, object Value)
		{
			string module = Key.ToString ().Split ("_".ToCharArray ())[0];
			string name = Key.ToString ().Split ("_".ToCharArray ())[1];

			Key key = _defaults.Find (delegate (Key k) { return k.Path == module + "_" + name; });
			if (key != null)
			{
				key.Value = Value.ToString ();
			}
			else
			{
				key = new Key ();
				key.Module = module;
				key.Name = name;
				key.Value = Value.ToString ();
				_defaults.Add (key);
			}
		}

		public static void Set (object Key, Object Value)
		{
			string module = Key.ToString ().Split ("_".ToCharArray ())[0];
			string name = Key.ToString ().Split ("_".ToCharArray ())[1];

			Set (module, name, Value);
		}

		private static void Set (string Key, Object Value)
		{
			string module = Key.Split ("_".ToCharArray ())[0];
			string name = Key.Split ("_".ToCharArray ())[1];

			Set (module, name, Value);
		}

		private static void Set (string Module, string Name, Object Value)
		{
			Key key = _keys.Find (delegate (Key k) { return k.Path == Module + "_" + Name; });
			if (key != null)
			{
				key.Value = Value.ToString ();
			}
			else
			{
				key = new Key ();
				key.Module = Module;
				key.Name = Name;
				key.Value = Value.ToString ();
				_keys.Add (key);
			}

			_writedelay.Stop ();
			_writedelay.Start ();
		}
		#endregion

		#region Private Static Methods
		private static void Read ()
		{
			lock (_lock)
			{
				_keys.Clear ();

				try
				{

					XmlDocument xml = new XmlDocument ();
					xml.Load (_path);

					XmlElement root = xml.DocumentElement;

					foreach (XmlNode node in root.ChildNodes)
					{
						switch (node.Name.ToLower ())
						{
							case "module":
							{
								foreach (XmlNode node2 in node.ChildNodes)
								{
									Key key = new Key ();
									key.Module = node.Attributes["name"].Value.ToString ().ToLower ();
									key.Name = node2.Name.ToLower ();
									key.Value = node2.InnerText;
									_keys.Add (key);
								}
								break;
							}

						}
					}
				}
				catch (Exception exception)
				{
					Match xmlerror = ExpMatchXmlError.Match (exception.ToString ());

					SorentoLib.Services.Logging.LogFatalError (Strings.LogErrorFatal.ServicesConfigLoad);
					SorentoLib.Services.Logging.LogFatalError (string.Format (Strings.LogErrorFatal.ServicesConfigLoadXmlError, xmlerror.Groups[1].Value.ToLower (), xmlerror.Groups[2].Value.ToLower ()));
//					SorentoLib.Services.Logging.LogFatalError (string.Format (Strings.LogErrorFatal.ServicesConfigLoadException, exception.ToString ()));
					SorentoLib.Runtime.Shutdown ();
				}
			}
		}

		private static void Write ()
		{
			lock (_lock)
			{
				try
				{
					List<string> modules = new List<string> ();
					foreach (Key key in _keys)
					{
						if (!modules.Contains (key.Module))
						{
							modules.Add (key.Module);
						}
					}

					XmlTextWriter writer = new XmlTextWriter(_path, null);
					writer.Formatting = Formatting.Indented;

					writer.WriteStartDocument();
					writer.WriteStartElement("config");

					foreach (string module in modules)
					{
						writer.WriteStartElement("module");
						writer.WriteAttributeString ("name", module);

						foreach (Key key in _keys.FindAll (delegate (Key k) { return k.Module == module; }))
						{
							writer.WriteElementString (key.Name, key.Value);
						}

						writer.WriteEndElement();
					}

					writer.WriteEndElement();
					writer.WriteEndDocument();
					writer.Close ();
				}
				catch (Exception exception)
				{
					SorentoLib.Services.Logging.LogError (Strings.LogError.ServicesConfigSave);
					SorentoLib.Services.Logging.LogError (string.Format (Strings.LogError.ServicesConfigSaveException, exception.ToString ()));
				}
			}
		}

		private static void Refresh ()
		{
			Read ();
			Services.Events.Invoke.ServiceConfigChanged ();
		}

		private static string Get (string Module, string Name)
		{
			Key key = _keys.Find (delegate (Key k) { return k.Path == Module + "_" + Name; });

			if (key == null)
			{
				key = _defaults.Find (delegate (Key k) { return k.Path == Module + "_" + Name; });
			}

			if (key == null)
			{
				throw new Exception (string.Format (Strings.Exception.ServicesConfigKeyNotFound, Module +"."+ Name));
			}

			return key.Value;
		}
		#endregion

		#region EventHandlers
		static void EventHandlerChanged (object Sender, FileSystemEventArgs EventArgs)
		{
			if (EventArgs.Name == "config.xml")
			{
				SorentoLib.Services.Logging.LogInfo (Strings.LogInfo.ServicesConfigChanged);
				Refresh ();
			}
		}

		static void EventHandlerDelay (object Sender, ElapsedEventArgs EventArgs)
		{
			_watcher.EnableRaisingEvents = false;

			Write ();

			Thread.Sleep (1000);
			_watcher.EnableRaisingEvents = true;
		}
		#endregion

		#region Internal Classes
		internal class Key
		{
			#region Privat Fields
			private string _module;
			private string _name;
			private string _value;
			#endregion

			#region Public Fields
			public string Path
			{
				get
				{
					return this._module + "_" + this._name;
				}
			}

			public string Module
			{
				get
				{
					return this._module;
				}

				set
				{
					this._module = value;
				}
			}

			public string Name
			{
				get
				{
					return this._name;
				}

				set
				{
					this._name = value;
				}
			}

			public string Value
			{
				get
				{
					return this._value;
				}

				set
				{
					this._value = value;
				}
			}
			#endregion

			#region Constructor
			public Key ()
			{
				this._module = string.Empty;
				this._name = string.Empty;
				this._value = string.Empty;
			}
			#endregion
		}
		#endregion
	}
}
