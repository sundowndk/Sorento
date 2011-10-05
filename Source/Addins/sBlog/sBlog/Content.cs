//
// MyClass.cs
//  
// Author:
//       sundown <>
// 
// Copyright (c) 2009 sundown
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

using Toolbox.CGI;
using Toolbox.DBI;

using SorentoLib;

namespace sBlog
{
	public class Content
	{
		#region Static Fields
		public static string DatabaseTableName = SorentoLib.Services.Config.Get("database", "prefix") + "_blog_content";
		#endregion

		#region Private Fields
		private Guid _id;

		private Guid _entryid;

		private int _createtimestamp = Toolbox.Date.CurrentDateTimeToTimestamp();
		private int _updatetimestamp = Toolbox.Date.CurrentDateTimeToTimestamp();

		private string _language = string.Empty;
		private string _text = string.Empty;

		private List<Media> _media = new List<Media>();

		private bool _isnew = false;
		#endregion

		#region Public Fields
		/// <summary>
		/// ID.
		/// </summary>
		public Guid Id
		{
			get
			{
				return this._id;
			}
		}

		public Guid EntryId
		{
			get
			{
				return this._entryid;
			}

			set
			{
				this._entryid = value;
			}
		}

		/// <summary>
		/// Timestamp when User object was created.
		/// </summary>
		public int CreateTimestamp
		{
			get
			{
				return this._createtimestamp;
			}
		}

		/// <summary>
		/// Timestamp when User object was last updated.
		/// </summary>
		public int UpdateTimestamp
		{
			get
			{
				return this._updatetimestamp;
			}
		}

		public string Language
		{
			get
			{
				return this._language;
			}

			set
			{
				this._language = value;
			}
		}

		public string Text
		{
			get
			{
				return this._text;
			}

			set
			{
				this._text = value;
			}
		}

		public List<Media> Media
		{
			get
			{
				return this._media;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Class's constructor.
		/// </summary>
		public Content ()
		{
			this._id = System.Guid.NewGuid();
			this._isnew = true;
		}
		#endregion

		#region Public Methods
		public bool Load(Guid id)
		{
			// Definiations
			bool success = false;

			// QueryBuilder
			SelectQueryBuilder qb = new SelectQueryBuilder();
			qb.Table(DatabaseTableName);
			qb.Columns(
			           "entryid",
			           "createtimestamp",
			           "updatetimestamp",
			           "language",
			           "text",
			           "media"
			           );
			qb.AddWhere("id", "=", id.ToString());

			// Query
			DbiData query = SorentoLib.Services.Database.Connection.Query(qb.Build());

			if (query.Success)
			{
				if (query.NextRow())
				{
					this._id = id;
					this._createtimestamp = query.GetInt(qb.ColumnPos("createtimestamp"));
					this._updatetimestamp = query.GetInt(qb.ColumnPos("updatetimestamp"));
					this._language = query.GetString(qb.ColumnPos("language"));
					this._text = query.GetString(qb.ColumnPos("text"));
					this._isnew = false;
					success = true;
				}
			}

			// PostQuery

			if (success)
			{
				try
				{

				}
				catch
				{
				}
			}

			query.Dispose();

			// Cleanup
			query = null;
			qb = null;

			// Finish
			return success;
		}

		public bool Save()
		{
			// Definitions
			bool success = false;

			// PreQuery

			// QueryBuilder
			QueryBuilder qb;
			if (this._isnew)
			{
				qb = new InsertQueryBuilder();
				this._createtimestamp = Toolbox.Date.CurrentDateTimeToTimestamp();
			}
			else
			{
				qb = new UpdateQueryBuilder();
				qb.AddWhere("id", "=", this._id.ToString());
				this._updatetimestamp = Toolbox.Date.CurrentDateTimeToTimestamp();
			}

			qb.Table(DatabaseTableName);

			qb.Columns(
			           "id",
			           "entryid",
			           "createtimestamp",
		    	       "updatetimestamp",
			           "language",
			           "text",
			           "media"
			           );

			qb.Values(
			          this._id.ToString(),
			          this._entryid.ToString(),
			          this._createtimestamp.ToString(),
			          this._updatetimestamp.ToString(),
			          this._language,
			          this._text
		    	      );

			// Query
			DbiData query = SorentoLib.Services.Database.Connection.Query(qb.Build());

			// PostQuery
			if (query.AffectedRows > 0)
			{
				success = true;
			}
			query.Dispose();

			// Cleanup
			query = null;
			qb = null;

			// Finish
			return success;
		}

		public bool Delete()
		{
			// Definitions
			bool success = false;

			// QueryBuilder
			DeleteQueryBuilder qb = new DeleteQueryBuilder();
			qb.Table(DatabaseTableName);
			qb.AddWhere("id", "=", this._id.ToString());

			// Query
			DbiData query = SorentoLib.Services.Database.Connection.Query(qb.Build());

			// PostQuery
			if (query.AffectedRows > 0)
			{
				success = true;
				this._isnew = success;
			}
			query.Dispose();

			// Cleanup
			query = null;
			qb = null;

			// Finish
			return success;
		}
		#endregion

		#region Public Static Methods
		public static bool Delete(Guid id)
		{
			// Definitions
			bool success = false;

			// Delete
			Content content = new Content();
			if (content.Load(id))
			{
				success = content.Delete();
			}

			// Cleanup
			content = null;

			// Finish
			return success;
		}

//		public static List<Content> List()
//		{
//			// Definitions
//			List<Entry> result = new List<Entry>();
//
//			// QueryBuilder
//			SelectQueryBuilder qb = new SelectQueryBuilder();
//			qb.Table(DatabaseTableName);
//			qb.Columns("id");
//
//			// Query
//			DbiData query = Toolbox.Global.Variables.Get<DbiConnection>("dbconnection").Query(qb.Build());
//			if (query.Success)
//			{
//				while (query.NextRow())
//				{
//					Entry entry = new Entry();
//					if (entry.Load(new Guid(query.GetString(qb.ColumnPos("id")))))
//					{
//						result.Add(entry);
//					}
//
//					// Cleanup
//					entry = null;
//				}
//			}
//			query.Dispose();
//
//			// Cleanup
//			query = null;
//			qb = null;
//
//			// Finish
//			return result;
//		}
		#endregion
	}
}
