// 
// Profile.cs
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
using System.Collections.Generic;

using Toolbox;
using Toolbox.DBI;
using Toolbox.Enums;

using SorentoLib;

namespace Akvabase
{
	public class Profile : SorentoLib.User
	{
		#region Public Static Fields
		public static string DatabaseTableName = SorentoLib.Services.Database.Prefix + "akvabase_profiles";
		#endregion

		#region Private Fields
		private string _name;
		private string _attention;
		private string _firstname;
		private string _lastname;
		private string _address;
		private string _postcode;
		private string _city;
		private string _vatno;

		private Akvabase.Enums.ProfileType _type;		

		private bool _newsletter;

		private bool _isnew;
		#endregion

		#region Public Fields
		public string Name
		{
			get
			{
				return this._name;
			}

			set
			{
				this._name = value;
				if (this._type == Akvabase.Enums.ProfileType.Business)
				{
					base.Realname = this._name;
				}
			}
		}

		public string Attention
		{
			get
			{
				return this._attention;
			}

			set
			{
				this._attention = value;
			}
		}

		public string Firstname
		{
			get
			{
				return this._firstname;
			}

			set
			{
				this._firstname = value;
				if (this._type == Akvabase.Enums.ProfileType.Individual)
				{
					base.Realname = this._firstname + " " + this._lastname;
				}
			}
		}

		public string Lastname
		{
			get
			{
				return this._lastname;
			}

			set
			{
				this._lastname = value;
				if (this._type == Akvabase.Enums.ProfileType.Individual)
				{
					base.Realname = this._firstname + " " + this._lastname;
				}
			}
		}

		public string Address
		{
			get
			{
				return this._address;
			}

			set
			{
				this._address = value;
			}
		}

		public string Postcode
		{
			get
			{
				return this._postcode;
			}

			set
			{
				this._postcode = value;
			}
		}

		public string City
		{
			get
			{
				return this._city;
			}

			set
			{
				this._city = value;
			}
		}

		public string Vatno
		{
			get
			{
				return this._vatno;
			}

			set
			{
				this._vatno = value;
			}
		}

		public string Email
		{
			get
			{
				return base.Email;
			}

			set
			{
				base.Email = value;
				base.Username = base.Email;
			}
		}

		public Akvabase.Enums.ProfileType Type
		{
			get
			{
				return this._type;
			}

			set
			{
				this._type = value;
			}
		}

		public bool Newsletter
		{
			get
			{
				return this._newsletter;
			}

			set
			{
				this._newsletter = value;
			}
		}
		#endregion

		#region Constructor
		public Profile ()
		{
			base.Username = string.Empty;

			this._name = string.Empty;
			this._attention = string.Empty;
			this._firstname = string.Empty;
			this._lastname = string.Empty;
			this._address = string.Empty;
			this._postcode = string.Empty;
			this._city = string.Empty;
			this._vatno = string.Empty;

			_type = Akvabase.Enums.ProfileType.Individual;

			this._newsletter = false;

			this._isnew = true;
		}
		#endregion

		#region Public Methods
		public bool Load (string Email)
		{
			bool result = false;

			if (base.Load (Email))
			{
				result = this.Load (base.Id);
			}

			return result;
		}

		public bool Load (Guid id)
		{
			bool result = false;

			if (base.Load (id))
			{
				QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
				qb.Table (DatabaseTableName);
				qb.Columns ("name",
				            "attention",
				            "firstname",
				            "lastname",
				            "address",
				            "postcode",
				            "city",
				            "vatno",
				            "type",
				            "newsletter");
				qb.AddWhere ("id", "=", Id);

				Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
				if (query.Success)
				{
					if (query.NextRow ())
					{
						this._name = query.GetString (qb.ColumnPos ("name"));
						this._attention = query.GetString (qb.ColumnPos ("attention"));
						this._firstname = query.GetString (qb.ColumnPos ("firstname"));
						this._lastname = query.GetString (qb.ColumnPos ("lastname"));
						this._address = query.GetString (qb.ColumnPos ("address"));
						this._postcode = query.GetString (qb.ColumnPos ("postcode"));
						this._city = query.GetString (qb.ColumnPos ("city"));
						this._vatno = query.GetString (qb.ColumnPos ("vatno"));
						this._type = query.GetEnum<Akvabase.Enums.ProfileType> (qb.ColumnPos ("type"));
						this._newsletter = query.GetBool (qb.ColumnPos ("newsletter"));

						this._isnew = false;
						result = true;
					}
				}

				query.Dispose ();
				query = null;
				qb = null;
			}

			return result;
		}

		public bool Save ()
		{
			bool result = false;

			if (base.Save ())
			{
				QueryBuilder qb;
				if (this._isnew)
				{
					qb = new QueryBuilder (QueryBuilderType.Insert);
				}
				else
				{
					qb = new QueryBuilder (QueryBuilderType.Update);
					qb.AddWhere ("id", "=", base.Id);
				}

				qb.Table (DatabaseTableName);
				qb.Columns ("id",
				            "name",
				            "attention",
				            "firstname",
				            "lastname",
				            "address",
				            "postcode",
				            "city",
				            "vatno",
				            "type" ,
				            "newsletter");
				qb.Values (base.Id,
				           this._name,
				           this._attention,
				           this._firstname,
				           this._lastname,
				           this._address,
				           this._postcode,
				           this._city,
				           this._vatno,
				           this._type,
				           this._newsletter);

				Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
				if (query.AffectedRows > 0)
				{
					this._isnew = false;
					result = true;
				}

				query.Dispose ();
				query = null;
				qb = null;
			}

			return result;
		}

		public bool Delete ()
		{
			bool result = false;

			if (base.Delete ())
			{
				if (base.Status != SorentoLib.Enums.UserStatus.BuiltIn)
				{
					QueryBuilder qb = new QueryBuilder (QueryBuilderType.Delete);
					qb.Table (DatabaseTableName);
					qb.AddWhere ("id", "=", base.Id);

					Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);

					if (query.AffectedRows > 0)
					{
						this._isnew = true;
						result = true;
					}

					query.Dispose ();
					query = null;
					qb = null;
				}
			}

			return result;
		}
		#endregion

		#region Public Static Methods
		public static bool Delete (Guid Id)
		{
			bool result = false;

			Akvabase.Profile profile = new Akvabase.Profile ();

			if (profile.Load (Id))
			{
				result = profile.Delete ();
			}

			profile = null;

			return result;
		}

		public static List<Akvabase.Profile> List (Akvabase.Enums.ProfileType Type)
		{
			List<Akvabase.Profile> result = new List<Akvabase.Profile> ();

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table (DatabaseTableName);
			qb.Columns ("id");

			if (Type != null)
			{
				qb.AddWhere ("type", "=", Type);
			}

			Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				while (query.NextRow ())
				{
					Akvabase.Profile profile = new Akvabase.Profile ();
					if (profile.Load (query.GetGuid (qb.ColumnPos ("id"))))
					{
						result.Add (profile);
					}
					profile = null;
				}
			}

			query.Dispose();
			query = null;
			qb = null;

			return result;
		}
		#endregion

		#region Internal Static Methods
		internal static void Stats ()
		{
			SorentoLib.Services.Logging.LogDebug ("Stat: akvabase.profile");

			QueryBuilder qb = new QueryBuilder (QueryBuilderType.Select);
			qb.Table(Akvabase.Profile.DatabaseTableName);
			qb.Columns("id");
			qb.AddWhere ("type", "=", (int)Akvabase.Enums.ProfileType.Business);

			Query query = SorentoLib.Services.Database.Connection.Query (qb.QueryString);
			if (query.Success)
			{
				int business = 0;

				while (query.NextRow ())
				{
					business++;
				}

				SorentoLib.Services.Stats.Set ("akvabase.profile.totalbusiness", business);
			}

			query.Dispose ();
			query = null;
			qb = null;
		}
		#endregion
	}
}
