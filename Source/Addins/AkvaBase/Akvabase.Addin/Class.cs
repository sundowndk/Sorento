//
// Class.cs
//
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2009 Rasmus Pedersen
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
using System.Collections.Generic;

using SorentoLib;

namespace Akvabase.Addin
{
	public class Class : SorentoLib.Addins.IClass
	{
		#region Private Fields
		private List<string> _namespaces = new List<string> ();
		#endregion

		#region Public Fields
		public List<string> Namespaces
		{
			get
			{
				return this._namespaces;
			}
		}
		#endregion

		#region Constructor
		public Class ()
		{
			this._namespaces.Add (typeof(Akvabase.Profile).Namespace.ToLower ());
		}
		#endregion

		#region Public Methods
		public bool IsProvided (string Namespace)
		{
			foreach (string _namespace in this._namespaces)
			{
				if (_namespace == Namespace.ToLower ())
				{
					return true;
				}
			}
			return false;
		}

		public object Dynamic (SorentoLib.Session Session, string Method, List<object> Parameters, object Variable)
		{
			return this.Resolve (Session, SorentoLib.Enums.ResolveType.Dynamic, Variable.GetType ().FullName.ToLower (), Method.ToLower (), Parameters, Variable);
		}

		public object Static (SorentoLib.Session Session, string Fullname, string Method, List<object> Parameters)
		{
			return this.Resolve (Session, SorentoLib.Enums.ResolveType.Static, Fullname.ToLower (), Method.ToLower (), Parameters, null);
		}
		#endregion

		#region Private Methods
		private object Resolve (SorentoLib.Session Session, SorentoLib.Enums.ResolveType ResolveType, string Fullname, string Method, List<object> Parameters, object Variable)
		{
			switch (Fullname)
			{
				#region Profile
				case "akvabase.profile":
					switch (ResolveType)
					{
						#region Dynamic
						case SorentoLib.Enums.ResolveType.Dynamic:
							switch (Method)
							{
								case "id":
									return ((Akvabase.Profile)Variable).Id;
								
								case "createtimestamp":
									return ((Akvabase.Profile)Variable).CreateTimestamp;
								
								case "updatetimestamp":
									return ((Akvabase.Profile)Variable).UpdateTimestamp;
								
								case "username":
									return ((Akvabase.Profile)Variable).Username;

								case "realname":
									return ((Akvabase.Profile)Variable).Realname;

								case "name":
									return ((Akvabase.Profile)Variable).Name;

								case "attention":
									return ((Akvabase.Profile)Variable).Attention;

								case "firstname":
									return ((Akvabase.Profile)Variable).Firstname;
								
								case "lastname":
									return ((Akvabase.Profile)Variable).Lastname;

								case "address":
									return ((Akvabase.Profile)Variable).Address;

								case "postcode":
									return ((Akvabase.Profile)Variable).Postcode;

								case "city":
									return ((Akvabase.Profile)Variable).City;

								case "vatno":
									return ((Akvabase.Profile)Variable).Vatno;

								case "email":
									return ((Akvabase.Profile)Variable).Email;

								case "avatar":
									return ((Akvabase.Profile)Variable).Avatar;

								case "newsletter":
									return ((Akvabase.Profile)Variable).Newsletter.ToString ().ToLower ();

								case "type":
									return ((Akvabase.Profile)Variable).Type.ToString ().ToLower ();

								case "load":
									Console.WriteLine (Parameters[0].GetType ().Name.ToLower ());
									switch (Parameters[0].GetType ().Name.ToLower ())
									{
										case "guid":
											return ((Akvabase.Profile)Variable).Load ((Guid)Parameters[0]);

										default:

											return null;
									}

								default:
								return null;
							}
						#endregion

						#region Static
						case SorentoLib.Enums.ResolveType.Static:
							switch (Method)
							{
								case "new":
									Console.WriteLine ("NEW");
									return new Akvabase.Profile();

								case "verifyactivation":
									try
									{
										return SorentoLib.User.Verify (new Guid (Parameters[0].ToString()), Session);
									}
									catch
									{
										return false;
									}

								default:
									return null;
							}

						#endregion
					}

					break;
				#endregion

				#region Profile
				case "akvabase.family":
					switch (ResolveType)
					{
						#region Dynamic
						case SorentoLib.Enums.ResolveType.Dynamic:
							switch (Method)
							{
								case "id":
									return ((Akvabase.Profile)Variable).Id;
								
								case "createtimestamp":
									return ((Akvabase.Profile)Variable).CreateTimestamp;
								
								case "updatetimestamp":
									return ((Akvabase.Profile)Variable).UpdateTimestamp;
								
								case "username":
									return ((Akvabase.Profile)Variable).Username;

								case "realname":
									return ((Akvabase.Profile)Variable).Realname;

								case "name":
									return ((Akvabase.Profile)Variable).Name;

								case "attention":
									return ((Akvabase.Profile)Variable).Attention;

								case "firstname":
									return ((Akvabase.Profile)Variable).Firstname;
								
								case "lastname":
									return ((Akvabase.Profile)Variable).Lastname;

								case "address":
									return ((Akvabase.Profile)Variable).Address;

								case "postcode":
									return ((Akvabase.Profile)Variable).Postcode;

								case "city":
									return ((Akvabase.Profile)Variable).City;

								case "vatno":
									return ((Akvabase.Profile)Variable).Vatno;

								case "email":
									return ((Akvabase.Profile)Variable).Email;

								case "avatar":
									return ((Akvabase.Profile)Variable).Avatar;

								case "newsletter":
									return ((Akvabase.Profile)Variable).Newsletter.ToString ().ToLower ();

								case "type":
									return ((Akvabase.Profile)Variable).Type.ToString ().ToLower ();

								case "load":
									Console.WriteLine (Parameters[0].GetType ().Name.ToLower ());
									switch (Parameters[0].GetType ().Name.ToLower ())
									{
										case "guid":
											return ((Akvabase.Profile)Variable).Load ((Guid)Parameters[0]);

										default:

											return null;
									}

								default:
								return null;
							}
						#endregion

						#region Static
						case SorentoLib.Enums.ResolveType.Static:
							switch (Method)
							{
								case "new":
									Console.WriteLine ("NEW");
									return new Akvabase.Profile();

								case "verifyactivation":
									try
									{
										return SorentoLib.User.Verify (new Guid (Parameters[0].ToString()), Session);
									}
									catch
									{
										return false;
									}

								default:
									return null;
							}

						#endregion
					}

					break;
				#endregion
			}

			return null;
		}
		#endregion
	}
}
