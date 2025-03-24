using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CRM_Perf_BenchMark
{
	/// <summary>
	/// Class to represent a CRM Entity object.
	/// </summary>
	public class CRMEntity
	{
		/// <summary>
		/// Marker used to determine if the Entity is in use for testing or not.
		/// </summary>
		private bool m_InUse = false;

		/// <summary>
		/// Collection of properties that can be used to describe this entity.
		/// </summary>
		private Hashtable m_Props = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Props">Hashtable of properties for this entity.</param>
		public CRMEntity(Hashtable Props)
		{
			m_Props = (Hashtable)Props.Clone();
		}

		public CRMEntity(System.Data.DataTable table, int rowNumber, string Type)
		{
			m_Props = new Hashtable();

			System.Data.DataRow row = table.Rows[rowNumber];

			for (int i = 0; i < table.Columns.Count; i++)
			{
				this[table.Columns[i].ColumnName] = row[i].ToString();
			}
			this["type"] = Type;
		}

		public CRMEntity(System.Data.SqlClient.SqlDataReader reader, string Type)
		{
			m_Props = new Hashtable();

			for (int i = 0; i < reader.FieldCount; i++)
				this[reader.GetName(i)] = reader[i].ToString();

			this["type"] = Type;

		}

		/// <summary>
		/// Accessor to determine if entity is in use for testing.
		/// </summary>
		public bool InUse
		{
			get
			{
				return m_InUse;
			}
			set
			{
				m_InUse = value;
			}
		}

		/// <summary>
		/// Accessor to get/set properties for entity. Only operates on string valules / keys.
		/// </summary>
		/// <param name="Property">Name of property. Not case sensitive</param>
		/// <returns>string representing property. Throws if property does not exist</returns>
		public string this[string Property]
		{
			get
			{
				return (string)m_Props[Property.ToLower()];
			}
			set
			{
				if (false == m_Props.ContainsKey(Property.ToLower()))
					m_Props.Add(Property.ToLower(), value);
				else
					m_Props[Property.ToLower()] = value;
			}
		}

		/// <summary>
		/// Method to determine if an entity has a property defined.
		/// </summary>
		/// <param name="PropName">Name of property</param>
		/// <returns>true if property is set on entity, false otherwise.</returns>
		public bool HasProp(string PropName)
		{
			return m_Props.ContainsKey(PropName.ToLower());
		}

		public override string ToString()
		{
			string EntityString = "Dumping Entity\n";

			EntityString += "\tIn Use: " + this.m_InUse.ToString() + "\n";
			EntityString += "\tProp Count: " + this.m_Props.Count + "\n";
			EntityString += "\tProps:\n";
			foreach (string Key in new ArrayList(m_Props.Keys))
			{
				EntityString += "\t\tKey: " + Key + "\n";
				EntityString += "\t\tValue: '" + this.m_Props[Key] + "'\n\n";
			}
			EntityString += "\n";

			return EntityString;
		}
	}

	/// <summary>
	/// Class to wrap hashtable collection of entities. Has index marker to assist in free entity searches.
	/// </summary>
	public class EntityList
	{
		public System.Collections.Hashtable Entities = new System.Collections.Hashtable();
		public int nNextIndex = 0;
		public int maxEntities = 1000;
		public int Available
		{
			get
			{
				int Avail = 0;
				//Need to know how many free entities are around.
				System.Collections.ArrayList al = new ArrayList(Entities.Values);
				foreach (CRMEntity Entity in al)
					if (false == Entity.InUse)
						Avail++;

				return Avail;
			}
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public virtual string ToString(bool bRecurse)
		{
			//First, dump the current structure.

			string Ret = "Dumping EntityList\n";
			Ret += "==========================================================================\n";
			Ret += "\tEntity Count (Total): " + this.Entities.Count + "\n";
			Ret += "\tEntity Count (Avail): " + this.Available + "\n";
			Ret += "\tEntity Count (Max)  : " + this.maxEntities + "\n";
			Ret += "\tNext Entity: " + this.nNextIndex + "\n";

			if (bRecurse)
			{
				Ret += "Dumping all Entities in list:\n";

				int i = 0;
				foreach (CRMEntity Entity in new ArrayList(this.Entities.Values))
				{
					Ret += "Entity Index: " + i + "\n";
					Ret += Entity.ToString();
				}
			}
			Ret += "==========================================================================\n\n";

			return Ret;
		}
	}

	public class EntityRequest
	{
		public string Type = null;
		public string ReturnAs = null;
		public Guid ParentID = Guid.Empty;
		public Guid GrandParentID = Guid.Empty;
		public Hashtable Props = new Hashtable();
		public bool DoNotUseIDLookup = false;
	}
}
