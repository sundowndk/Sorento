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

/// <summary>
/// Settings.
/// </summary>
namespace SorentoLib.Services
{
	public static class Settings
	{

		public static void Initialize ()
		{
			// Set default settings.
			foreach (Enums.SettingsKey key in Enum.GetValues (typeof (Enums.SettingsKey)))
			{
				if (!SorentoLib.Services.Settings.Exist (key))
				{
					SorentoLib.Services.Settings.Set (key, defaults[key]);
					
					// LOG: LogDebug.ExceptionUnknown
					SorentoLib.Services.Logging.LogDebug (string.Format (SorentoLib.Strings.LogDebug.ServiceSettingsDefaultSet, key.ToString ().ToLower ()));
				}
			}
		}

		public static Hashtable defaults = new Hashtable ()
		{
			{ Enums.SettingsKey.path_addins, "../Addins/" },
			{ Enums.SettingsKey.parser_timeout, 25 }
		};

		#region Public Static Methods
		/// <summary>
		/// Set the specified Key and Value.
		/// </summary>
		public static void Set (object Key, object Value)
		{
			Setting setting = new Setting (Key.ToString ().ToLower (), Value.ToString ());
			setting.Save ();
		}

		/// <summary>
		/// Get the specified Key.
		/// </summary>
		public static T Get<T> (object Key)
		{
			Setting setting;

			try
			{
				setting = Setting.Load (Key.ToString ().ToLower ());
			}
			catch (Exception exception)
			{
				// LOG: LogDebug.ExceptionUnknown
				Services.Logging.LogDebug (string.Format (Strings.LogDebug.ExceptionUnknown, "SORENTOLIB.SERVICES.SETTINGS", exception.Message));
				
				// EXCEPTION: Exception.ServicesSettingsKeyError
				throw new Exception (string.Format (Strings.Exception.ServicesSettingsKeyError, Key.ToString ().ToLower ()));
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
				throw new Exception (string.Format (Strings.Exception.ServicesConfigKeyNotValidType, Key.ToString ().ToLower (), typeof (T).Name));
			}
		}

		/// <summary>
		/// Delete the specified Key.
		/// </summary>
		public static void Delete (object Key)
		{
			Setting.Delete (Key.ToString ().ToLower ());
		}

		/// <summary>
		/// Check if the specifed key sxist.
		/// </summary>
		public static bool Exist (object Key)
		{
			return Setting.Exist (Key.ToString ().ToLower ());
		}


		#endregion

		#region Internal Classes
		/// <summary>
		/// Setting.
		/// </summary>
		internal class Setting
		{
			#region Private Fields
			private string _ḱey;
			private string _value;
			#endregion

			#region Internal Fields
			internal static string DatastoreAisle = "sorentolib.services.settings";

			internal string Key
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

			internal string Value
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
			/// <summary>
			/// Initializes a new instance of the <see cref="SorentoLib.Services.Settings+Setting"/> class.
			/// </summary>
			internal Setting (string Key, string Value)
			{
				this._ḱey = Key;
				this._value = Value;
			}

			internal Setting ()
			{
				this._ḱey = string.Empty;
				this._value = string.Empty;
			}
			#endregion

			#region Internal Methods
			/// <summary>
			/// Save this instance.
			/// </summary>
			internal void Save ()
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
			#endregion

			#region Internal Static Methods
			/// <summary>
			/// Load the specified Key.
			/// </summary>
			internal static Setting Load (string Key)
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

			/// <summary>
			/// Delete the specified Key.
			/// </summary>
			internal static void Delete (String Key)
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

			/// <summary>
			/// See if specified Key exists.
			/// </summary>
			internal static bool Exist (string Key)
			{
				// TODO: THIS SHOULD BE DONE WITH FINDAISLE.
				try
				{
					Load (Key);
					return true;
				}
				catch 
				{
					return false;
				}
			}
			#endregion
		}
		#endregion
	}
}

