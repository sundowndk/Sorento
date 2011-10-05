//
// Ajax.cs
//
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
//
// Copyright (c) 2010 Rasmus Pedersen
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
using System.Collections;
using System.Collections.Generic;

using System.Threading;

using SorentoLib;

using AkvabaseLib;

namespace Akvabase.Addin
{
	public class Ajax : SorentoLib.Addins.IAjax
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
		public Ajax ()
		{
			AkvabaseLib.Runtime.DbConnection = SorentoLib.Services.Database.Connection;
			this._namespaces.Add ("akvabase");
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

		public SorentoLib.Ajax.Respons Process (SorentoLib.Session Session, string Fullname, string Method)
		{
			SorentoLib.Ajax.Respons result = new SorentoLib.Ajax.Respons ();
			SorentoLib.Ajax.Request request = new SorentoLib.Ajax.Request (Session.Request.QueryJar.Get ("data").Value);

			switch (Fullname.ToLower ())
			{
				#region Search
				case "akvabase.search":
					switch (Method.ToLower ())
					{
						case "suggestion":
						{
							AkvabaseLib.Search.Suggestion search = new AkvabaseLib.Search.Suggestion();
							search.Text = request.Data<string> ("text");;
							search.Language = request.Data<string> ("language");
							search.Do ();

							List<Hashtable> strings = new List<Hashtable> ();
							foreach (string test in search.Result)
							{
								Hashtable item = new Hashtable ();
								item.Add("text", test);
								strings.Add(item);
							}

							result.Data.Add("strings", strings);
							result.Data.Add("success", "true");

							break;
						}

						case "location":
						{
							AkvabaseLib.Search.Location search = new AkvabaseLib.Search.Location ();
							search.Text = request.Data<string> ("text");
							search.Language = request.Data<string> ("language");
							search.Do ();

							List<Hashtable> locations = new List<Hashtable> ();
							foreach (AkvabaseLib.Location location in search.Result)
							{
								Hashtable item = new Hashtable ();
								item.Add ("id", location.Id.ToString ());
								if (location.Parent != null)
								{
									item.Add ("parentid", location.Parent.Id);
								}
								else
								{
									item.Add ("parentid", "0");
								}
								item.Add ("name", location.Name[request.Data<string> ("language")]);
								item.Add ("fullname", location.FullName[request.Data<string> ("language")]);
								item.Add ("path", location.Path);

								locations.Add (item);
							}

							result.Data.Add ("locations", locations);
							result.Data.Add ("success", "true");

							break;
						}

						default:
							break;
					}
					break;
				#endregion

				#region Profile
				case "akvabase.profile":
					switch (Method.ToLower ())
					{
						case "update":
						{
							// RESTRICTIONS: ACCESSLEVEL = ADMINISTRATOR || ID = SESSION.USER.ID
							if ((Session.AccessLevel == SorentoLib.Enums.Accesslevel.Administrator) || (Session.User.Id == new Guid(request.Data<string> ("id"))))
							{
								Akvabase.Profile profile = new Akvabase.Profile ();
								if (profile.Load (new Guid(request.Data<string> ("id"))))
								{
									profile.Name = request.Data<string> ("name");
									profile.Attention = request.Data<string> ("attention");
									profile.Firstname = request.Data<string> ("firstname");
									profile.Lastname = request.Data<string> ("lastname");
									profile.Address = request.Data<string> ("address");
									profile.Postcode = request.Data<string> ("postcode");
									profile.City = request.Data<string> ("city");
									profile.Vatno = request.Data<string> ("vatno");
									profile.Email = request.Data<string> ("email");
									profile.Newsletter = Toolbox.Convert.StringToBool (request.Data<string> ("newsletter"));

									if (request.Data<string> ("avatarmediaid") == string.Empty)
									{
										if (profile.Avatar != null)
										{
											profile.Avatar.Delete ();
										}
										profile.Avatar = null;
									}
									else
									{
										if (profile.Avatar != null)
										{
											if (profile.Avatar.Id != new Guid (request.Data<string> ("avatarmediaid")))
											{
												profile.Avatar.Delete ();
											}
										}

										profile.Avatar = SorentoLib.Media.Load (new Guid (request.Data<string> ("avatarmediaid")));
										profile.Avatar.Status = SorentoLib.Enums.MediaStatus.Public;
										profile.Avatar.Save ();
									}

									if (profile.Save ())
									{
										result.Data.Add ("success", "true");
									}
								}

								profile = null;
							}

							break;
						}

						default:

							break;
					}

					break;
				#endregion

				#region Family
				case "akvabase.family":
					switch (Method.ToLower ())
					{
						case "get":
						{
							AkvabaseLib.Fish.Family family = new AkvabaseLib.Fish.Family ();
							if (family.Load (new Guid(request.Data<string> ("id"))))
							{
								result.Data.Add ("createtimestamp", family.CreateTimestamp.ToString ());
								result.Data.Add ("updatetimestamp", family.UpdateTimestamp.ToString ());
								result.Data.Add ("namelatin", family.Name["la"]);
								result.Data.Add ("namelocal", family.Name["no"]);

								result.Data.Add ("success", "true");
							}
							break;
						}

						case "list":
						{
							List<Hashtable> families = new List<Hashtable> ();
							Console.WriteLine (request.Data<string> ("orderid"));
							foreach (AkvabaseLib.Fish.Family family in AkvabaseLib.Fish.Family.List (new Guid (request.Data<string> ("orderid"))))
							{
								Hashtable item = new Hashtable ();
								item.Add ("id", family.Id.ToString ());
								item.Add ("namelatin", family.Name["la"]);
								item.Add ("namelocal", family.Name["no"]);

								families.Add(item);
							}

							result.Data.Add ("families", families);
							result.Data.Add ("success", "true");
							break;
						}

						default:

							break;
					}

					break;
				#endregion

				#region Family
				case "akvabase.genus":
					switch (Method.ToLower ())
					{
						case "get":
						{
							AkvabaseLib.Fish.Family family = new AkvabaseLib.Fish.Family ();
							if (family.Load (new Guid(request.Data<string> ("id"))))
							{
								result.Data.Add ("createtimestamp", family.CreateTimestamp.ToString ());
								result.Data.Add ("updatetimestamp", family.UpdateTimestamp.ToString ());
								result.Data.Add ("namelatin", family.Name["la"]);
								result.Data.Add ("namelocal", family.Name["no"]);

								result.Data.Add ("success", "true");
							}
							break;
						}

						case "list":
						{
							List<Hashtable> genera = new List<Hashtable> ();
							foreach (AkvabaseLib.Fish.Genus genus in AkvabaseLib.Fish.Genus.List (new Guid (request.Data<string> ("familyid"))))
							{
								Hashtable item = new Hashtable ();
								item.Add ("id", genus.Id.ToString ());
								item.Add ("namelatin", genus.Name["la"]);
//								item.Add ("namelocal", genus.Name["no"]);
								Console.WriteLine (genus.Name["la"]);
								genera.Add(item);
							}

							result.Data.Add ("genera", genera);
							result.Data.Add ("success", "true");
							break;
						}

						default:

							break;
					}

					break;
				#endregion

				#region Family
				case "akvabase.species":
					switch (Method.ToLower ())
					{
//						case "get":
//						{
//							AkvabaseLib.Fish.Species species = new AkvabaseLib.Fish.Species ();
//							if (species.Load (new Guid(request.Data<string> ("id"))))
//							{
//								result.Data.Add ("createtimestamp", family.CreateTimestamp.ToString ());
//								result.Data.Add ("updatetimestamp", family.UpdateTimestamp.ToString ());
//								result.Data.Add ("namelatin", family.Name["la"]);
//								result.Data.Add ("namelocal", family.Name["no"]);
//
//								result.Data.Add ("success", "true");
//							}
//							break;
//						}

//						case "list":
//						{
//							List<Hashtable> species = new List<Hashtable> ();
//							foreach (AkvabaseLib.Fish.Species _species in AkvabaseLib.Fish.Species.List (new Guid (request.Data<string> ("genusid"))))
//							{
//								Hashtable item = new Hashtable ();
//								item.Add ("id", _species.Id.ToString ());
//								item.Add ("namelatin", _species.Name["la"]);
////								item.Add ("namelocal", genus.Name["no"]);
//								species.Add();
//							}
//
//							result.Data.Add ("species", species);
//							result.Data.Add ("success", "true");
//							break;
//						}

						default:

							break;
					}

					break;
				#endregion

				#region Fish
				case "akvabase.fish":
					switch (Method.ToLower ())
					{
						case "get":
						{
							AkvabaseLib.Fish.Fish fish = new AkvabaseLib.Fish.Fish ();
							if (fish.Load (new Guid(request.Data<string> ("id"))))
							{
								result.Data.Add ("id", fish.Id.ToString ());
								result.Data.Add ("createtimestamp", fish.CreateTimestamp.ToString ());
								result.Data.Add ("updatetimestamp", fish.UpdateTimestamp.ToString ());
								result.Data.Add ("namelatin", fish.Name["la"]);
								result.Data.Add ("namelocal", fish.Name["no"]);

								List<Hashtable> order = new List<Hashtable> ();
								{
									Hashtable item = new Hashtable ();
									item.Add ("id", fish.Genus.Family.Order.ToString ());
									item.Add ("namelatin", fish.Genus.Family.Order.Name["la"]);
									item.Add ("namelocal", fish.Genus.Family.Order.Name["no"]);

									order.Add(item);
								}

								List<Hashtable> family = new List<Hashtable> ();
								{
									Hashtable item = new Hashtable ();
									item.Add ("id", fish.Genus.Family.Id.ToString ());
									item.Add ("namelatin", fish.Genus.Family.Name["la"]);
									item.Add ("namelocal", fish.Genus.Family.Name["no"]);
									family.Add(item);
								}

								List<Hashtable> genus = new List<Hashtable> ();
								{
									Hashtable item = new Hashtable ();
									item.Add ("id", fish.Genus.Id.ToString ());
									item.Add ("namelatin", fish.Genus.Name["la"]);
									genus.Add(item);
								}

								List<Hashtable> species = new List<Hashtable> ();
								{
									Hashtable item = new Hashtable ();
									item.Add ("id", fish.Species.Id.ToString ());
									item.Add ("namelatin", fish.Species.Name["la"]);
									species.Add(item);
								}

								result.Data.Add ("order", order);
								result.Data.Add ("family", family);
								result.Data.Add ("genus", genus);
								result.Data.Add ("species", species);
								result.Data.Add ("success", "true");
							}
							break;
						}

						case "list":
						{
							List<Hashtable> fish = new List<Hashtable> ();
							foreach (AkvabaseLib.Fish.Fish _fish in AkvabaseLib.Fish.Fish.List (new Guid (request.Data<string> ("genusid"))))
							{
								Hashtable item = new Hashtable ();
								item.Add ("id", _fish.Id.ToString ());
								item.Add ("namelatin", _fish.Name["la"]);
								item.Add ("namelocal", _fish.Name["no"]);
								item.Add ("speciesnamelatin", _fish.Species.Name["la"]);
								fish.Add(item);
							}

							result.Data.Add ("fish", fish);
							result.Data.Add ("success", "true");
							break;
						}

						default:

							break;
					}

					break;
				#endregion

				#region Family
				case "akvabase.order":
					switch (Method.ToLower ())
					{
						case "list":
						{
							List<Hashtable> orders = new List<Hashtable> ();
							foreach (AkvabaseLib.Fish.Order order in AkvabaseLib.Fish.Order.List ())
							{
								Hashtable item = new Hashtable ();
								item.Add ("id", order.Id.ToString ());
								item.Add ("namelatin", order.Name["la"]);
								item.Add ("namelocal", order.Name["no"]);

								orders.Add(item);
							}

							result.Data.Add ("orders", orders);
							result.Data.Add ("success", "true");
							break;
						}

						default:

							break;
					}

					break;
				#endregion

				#region Browser
				case "akvabase.browser":
					switch (Method.ToLower ())
					{
						case "get":
						{
							List<Hashtable> items = new List<Hashtable> ();

							switch (request.Data<string> ("type").ToLower ())
							{
								case "fish":

									switch (request.Data<string> ("level"))
									{
										case "2":
											foreach (AkvabaseLib.Fish.Order order in AkvabaseLib.Fish.Order.List ())
											{
												Hashtable item = new Hashtable ();
												item.Add ("type", "fish");
												item.Add ("id", order.Id.ToString ());
												item.Add ("text1", order.Name["la"]);
												item.Add ("text2", order.Name["no"]);
												items.Add (item);
											}
											break;

										case "3":
											foreach (AkvabaseLib.Fish.Family family in AkvabaseLib.Fish.Family.List (new Guid (request.Data<string> ("id"))))
											{
												Hashtable item = new Hashtable ();
												item.Add ("type", "fish");
												item.Add ("id", family.Id.ToString ());
												item.Add ("text1", family.Name["la"]);
												item.Add ("text2", family.Name["no"]);
												items.Add (item);
											}
											break;

										case "4":
											foreach (AkvabaseLib.Fish.Genus genus in AkvabaseLib.Fish.Genus.List (new Guid (request.Data<string> ("id"))))
											{
												Hashtable item = new Hashtable ();
												item.Add ("type", "fish");
												item.Add ("id", genus.Id.ToString ());
												item.Add ("text1", genus.Name ["la"]);
												item.Add ("text2", " ");
												items.Add (item);
											}
											break;

										case "5":
											foreach (AkvabaseLib.Fish.Fish _fish in AkvabaseLib.Fish.Fish.List (new Guid (request.Data<string> ("id"))))
											{
												Hashtable item = new Hashtable ();
												item.Add ("id", _fish.Id.ToString ());
												item.Add ("type", "fish");
												try
												{
													item.Add ("text1", _fish.Species.Name ["la"]);
												}
												catch
												{
													item.Add ("text1", "Error");
												}
													item.Add ("text2", _fish.Name ["no"]);
												items.Add (item);
											}
											break;

										case "6":
											break;

										case "7":
											break;
									}
									break;

								case "plants":
									break;

								case "shellfish":
									break;

								case "locations":
									switch (request.Data<string> ("level"))
									{
										case "2":
											foreach (AkvabaseLib.Location location in AkvabaseLib.Location.List ())
											{
												Hashtable item = new Hashtable ();
												item.Add ("type", "location");
												item.Add ("id", location.Id.ToString ());
												item.Add ("text1", location.Name["no"]);
												item.Add ("text2", " ");
												items.Add (item);
											}
											break;

										default:
											foreach (AkvabaseLib.Location location in AkvabaseLib.Location.List (new Guid (request.Data<string> ("id"))))
											{
												Hashtable item = new Hashtable ();
												item.Add ("type", "location");
												item.Add ("id", location.Id.ToString ());
												item.Add ("text1", location.Name["no"]);
												item.Add ("text2", " ");
												items.Add (item);
											}

											foreach (AkvabaseLib.Fish.Fish _fish in AkvabaseLib.Fish.Fish.ListByLocation (new Guid (request.Data<string> ("id"))))
											{
												Hashtable item = new Hashtable ();
												item.Add ("id", _fish.Id.ToString ());
												item.Add ("type", "fish");
												try
												{
													item.Add ("text1", _fish.Name ["la"]);
												}
												catch
												{
													item.Add ("text1", "Error");
												}
													item.Add ("text2", _fish.Name ["no"]);
												items.Add (item);
											}
											break;
									}
									break;
							}

							result.Data.Add ("items", items);
							result.Data.Add ("success", "true");


//							foreach (AkvabaseLib.Fish.Species _species in AkvabaseLib.Fish.Species.List (new Guid (request.Data<string> ("genusid"))))
//							{
//								Hashtable item = new Hashtable ();
//								item.Add ("id", _species.Id.ToString ());
//								item.Add ("namelatin", _species.Name["la"]);
//								item.Add ("namelocal", genus.Name["no"]);
//								species.Add(item);
//							}

							break;
						}

						default:

							break;
					}

					break;
				#endregion

				#region Location
				case "akvabase.location":
					switch (Method.ToLower ())
					{
						case "get":
						{
							AkvabaseLib.Location location = new AkvabaseLib.Location ();
							if (location.Load (new Guid(request.Data<string> ("id"))))
							{
								result.Data.Add ("id", location.Id.ToString ());
								result.Data.Add ("createtimestamp", location.CreateTimestamp.ToString ());
								result.Data.Add ("updatetimestamp", location.UpdateTimestamp.ToString ());
								result.Data.Add ("namelocal", location.Name["no"]);


								List<Hashtable> parents = new List<Hashtable> ();
								if (location.Parent != null)
								{
									AkvabaseLib.Location parent = location.Parent;

									Hashtable item = new Hashtable ();
									item.Add ("id", parent.Id.ToString ());
									item.Add ("namelocal", parent.Name["no"]);
									parents.Add(item);

									while (true)
									{
										if (parent.Parent != null)
										{
											parent = parent.Parent;
											item = new Hashtable ();
											item.Add ("id", parent.Id.ToString ());
											item.Add ("namelocal", parent.Name["no"]);
											parents.Add(item);
										}
										else
										{
											break;
										}
									}
								}

								parents.Reverse ();

								result.Data.Add ("parents", parents);
								result.Data.Add ("success", "true");
							}
							break;
						}

						default:

							break;
					}

					break;
				#endregion

			}

			return result;
		}
		#endregion
	}
}
