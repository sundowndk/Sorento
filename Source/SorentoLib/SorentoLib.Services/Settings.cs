//
// Settings.cs
//
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
//
// Copyright (c) 2012 Rasmus Pedersen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace SorentoLib.Services
{
	public static class Settings
	{
		public static void Set (object Key, object Value)
		{
			Set (Key.ToString ().ToLower (), Value);
		}

		private static void Set (string Key, object Value)
		{
			Setting setting = new Setting ();
			setting.Key = Key;
			setting.Value = Value.ToString ();
			setting.Save ();
		}

		public static T Get<T> (object Key)
		{
			return (T)Get<T> (Key.ToString ().ToLower ());
		}

		private static T Get<T> (string Key)
		{
			Setting setting;

			try
			{
				setting = Setting.Load (Key);
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SERVICES.SETTINGS", exception.Message));
				
				// EXCEPTION: Exception.ServicesSettingsssKeyError
				throw new Exception (string.Format (Strings.Exception.ServicesSettingsKeyError, Key));
			}

			try
			{
				switch (typeof (T).Name.ToLower ())
				{
					case "guid":
					{
						return (T)Convert.ChangeType (new Guid (setting.Value), typeof(T));
					}

					default:
					{
						return (T)Convert.ChangeType (setting.Value, typeof(T));
					}
				}
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SERVICES.SETTINGS", exception.Message));

				// EXCEPTION: Exception.ServicesConfigKeyNotValidType
				throw new Exception (string.Format (Strings.Exception.ServicesConfigKeyNotValidType, Key, typeof (T).Name));
			}
		}

		public static void Delete (object Key)
		{
			Delete (Key.ToString ().ToLower ());
		}

		private static void Delete (string Key)
		{
			Setting.Delete (Key);
		}

		public static bool Exist (object Key)
		{
			return Exist (Key.ToString ().ToLower ());
		}

		private static bool Exist (string Key)
		{
			try
			{
				Setting.Load (Key);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private class Setting
		{
			public static string DatastoreAisle = "sorentolib.services.settings";

			private string _ḱey;
			private string _value;

			public string Key
			{
				get
				{
					return this._ḱey;
				}

				set
				{
					this._ḱey = value.ToLower ();
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

			public Setting ()
			{
				this._ḱey = string.Empty;
				this._value = string.Empty;
			}

			public void Save ()
			{
				try
				{
					Hashtable item = new Hashtable ();
					
					item.Add ("value", this._value);
					
					Services.Datastore.Set (DatastoreAisle, this._ḱey, SNDK.Convert.ToXmlDocument (item, "sorentolib.services.settings.setting"));
				}
				catch (Exception exception)
				{
					// LOG: LogDebug.ExceptionUnknown
					Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SERVICES.SETTINGS.SETTING", exception.Message));
					
					// EXCEPTION: Exception.ServicesSettingsSettingSave
					throw new Exception (string.Format (Strings.Exception.ServicesSettingsSettingSave, this._ḱey));
				}
			}

			public static Setting Load (string Key)
			{
				Setting result = new Setting ();
				
				try
				{
					Hashtable item = (Hashtable)SNDK.Convert.FromXmlDocument (SNDK.Convert.XmlNodeToXmlDocument (Services.Datastore.Get<XmlDocument> (DatastoreAisle, Key.ToLower ()).SelectSingleNode ("(//sorentolib.services.settings.setting)[1]")));

					result._ḱey = Key;

					if (item.ContainsKey ("value"))
					{
						result._value = (string)item["value"];
					}
				}
				catch (Exception exception)
				{
					// LOG: LogDebug.ExceptionUnknown
					Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SERVICES.SETTINGS.SETTING", exception.Message));
					
					// EXCEPTION: Exception.ServicesSettingsSettingLoadKey
					throw new Exception (string.Format (Strings.Exception.ServicesSettingsSettingLoadKey, Key));
				}
				
				return result;
			}

			public static void Delete (String Key)
			{
				try
				{
					Services.Datastore.Delete (DatastoreAisle, Key.ToLower ());
				}
				catch (Exception exception)
				{
					// LOG: LogDebug.ExceptionUnknown
					Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SERVICES.SETTINGS", exception.Message));
					
					// EXCEPTION: Exception.ServicesSettingsSettingDeleteKey
					throw new Exception (string.Format (Strings.Exception.ServicesSettingsSettingDeleteKey, Key.ToLower ()));
				}
			}
		}
	}
}

