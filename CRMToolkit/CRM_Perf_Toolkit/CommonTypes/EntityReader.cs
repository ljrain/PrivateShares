using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;
using System.Data;

namespace CommonTypes
{
	public enum EntityType
	{
		Normal = 0,
		Special = 1
	}

	public static class EntityReader
	{
		private static readonly string ORGANIZATION_ID_NAME = "organizationid";
		private static readonly string EntityManagerID = "ID";
		private static readonly int BitShift = 25;
		[ThreadStaticAttribute]
		private static int _retryCount = 0;
		private static Dictionary<string, EntityMetadata> mapEntityMetadata = null;
		private static Guid organizationId = Guid.Empty;
		private static object _lock = new object();
        private static int hiearchyfailures = 0;


        public static uint ReadEntityFromTable(IOrganizationService proxy, string emdbSqlConnStr, string entityName, string[] props, FilterExpression where, string strORDERBY, int? topRows, Guid orgId, Dictionary<Guid, Hashtable> owners, string inEntityName = "", EntityType entityType = EntityType.Normal, Guid? callingUserId = null)
        {
           
            string insertentityName = entityName;
            if (inEntityName != "")
            {
                insertentityName = inEntityName;
            }

            // Counter of entities read.
            uint totalRecordsProcessed = 0;
            using (SqlConnection emdbSqlConn = new SqlConnection(emdbSqlConnStr))
            {
                emdbSqlConn.Open();

                string ORDERBY = null;
                if (null != strORDERBY)
                    ORDERBY = strORDERBY.ToLower();

                QueryExpression query;

                bool expectedException = false;
                do
                {
                    if (expectedException)
                    {
                        System.Threading.Thread.Sleep((int)Math.Pow(10, _retryCount) * 100);
                    }

                    expectedException = false;
                    try
                    {
                        //JM Use SQLBulkCopy to improve Performance
                        using (DataTable table = new DataTable())
                        {
                            //SDK call to get data from org db
                            EntityCollection results = null;
                            bool getmorerows = true;
                            query = BuildQueryExpression(entityType, entityName, props, where, ORDERBY);

                            while (getmorerows)
                            {
                                results = proxy.RetrieveMultiple(query);
                                query.PageInfo.PageNumber++;
                                query.PageInfo.PagingCookie = results.PagingCookie;
                                getmorerows = results.MoreRecords;

                                for (int k = 0; k < results.Entities.Count; k++)
                                {
                                    DataRow r = table.NewRow();
                                    Entity en = results[k];

                                    //First need to determine the owner's number.
                                    Hashtable Owner = null;
                                    Guid OwnerId = Guid.Empty;

                                    if (entityType == EntityType.Normal)
                                    {
                                        if (null == props[0])
                                        {
                                            //to support multi server scenario, we are going to have a fake org
                                            //it the first property is null => we are reading orgs
                                            Guid fakeguid = new Guid("19007b7e-d9a3-da11-b5fc-001143d30bf2");
                                            Owner = owners[fakeguid];
                                            OwnerId = fakeguid;
                                        }
                                        else
                                        {
                                            //if the owner is an Organization, then we do not get the ownerid
                                            //we get the new owner id if the owner is a user
                                            if (en.Attributes[props[0].ToLower()] is EntityReference)
                                            {
                                                EntityReference er = (EntityReference)en.Attributes[props[0].ToLower()];
                                                if (string.Compare(props[0], ORGANIZATION_ID_NAME, true) == 0)
                                                {
                                                    OwnerId = (Guid)er.Id;
                                                }
                                                else if (props[0] == "TeamMembershipId")
                                                    OwnerId = orgId;
                                                else
                                                {
                                                    var userId = callingUserId ?? er.Id;
                                                    OwnerId = (Guid)GetOwnerGuid(userId, orgId);
                                                }
                                            }
                                            else
                                            {
                                                if (string.Compare(props[0], ORGANIZATION_ID_NAME, true) == 0)
                                                {
                                                    OwnerId = (Guid)en.Attributes[props[0].ToLower()];
                                                }
                                                else if (props[0] == "TeamMembershipId")
                                                    OwnerId = orgId;
                                                else
                                                {
                                                    OwnerId = GetOwnerGuid((Guid)en.Attributes[props[0].ToLower()], orgId);
                                                }
                                            }

                                            if (!owners.ContainsKey(OwnerId))
                                            {
                                                Trace.TraceError(string.Format("Owner '{0}' for one of the '{1}' entities was not found.", OwnerId, entityName));
                                                totalRecordsProcessed++;

                                                Trace.WriteLineIf(0 == (totalRecordsProcessed % 1000), "Read " + totalRecordsProcessed + " " + entityName + " (More to Come)");

                                                if (topRows.HasValue && totalRecordsProcessed >= topRows)
                                                {
                                                    getmorerows = false;
                                                    break;
                                                }
                                                continue;
                                            }
                                            Owner = owners[OwnerId];
                                        }
                                    }
                                    else
                                    {
                                        Owner = owners[orgId];
                                    }

                                    System.Int64 NextID = GetNextId(Owner, entityName);
                                    if (!table.Columns.Contains(EntityManagerID))
                                        table.Columns.Add(EntityManagerID);
                                    r[EntityManagerID] = NextID;

                                    if (entityType == EntityType.Normal)
                                    {
                                        if (!table.Columns.Contains("EntityManagerOwningUser"))
                                            table.Columns.Add("EntityManagerOwningUser", Type.GetType("System.Guid"));
                                        r["EntityManagerOwningUser"] = OwnerId;
                                    }


                                    foreach (string Prop in props)
                                    {
                                        if (Prop != null)
                                        {
                                            if (Prop == "OrganizationId" && entityType == EntityType.Special)
                                            {
                                                if (!table.Columns.Contains(Prop))
                                                    table.Columns.Add("OrganizationId", Type.GetType("System.Guid"));
                                                r["OrganizationId"] = orgId;
                                            }
                                            else if (Prop == "EntityManagerOwningUser" && entityType == EntityType.Special)
                                            {
                                                if (!table.Columns.Contains(Prop))
                                                    table.Columns.Add("EntityManagerOwningUser", Type.GetType("System.Guid"));
                                                r["EntityManagerOwningUser"] = orgId;
                                            }
                                    
                                            else if (!en.Attributes.ContainsKey(Prop.ToLower()))
                                            {                                               
                                                if (Prop.Equals("organizationidname", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    EntityReference er = (EntityReference)en.Attributes["organizationid"];
                                                    if (!table.Columns.Contains(Prop))
                                                        table.Columns.Add(Prop);
                                                    r[Prop] = er.Name;
                                                }
                                                else if (Prop.Equals("businessunitidname", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    EntityReference er = (EntityReference)en.Attributes["businessunitid"];
                                                    if (!table.Columns.Contains(Prop))
                                                        table.Columns.Add(Prop);
                                                    r[Prop] = er.Name;
                                                }                                              
                                                
                                                if (Prop.Equals("transactioncurrencyid", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    if (!table.Columns.Contains(Prop))
                                                        table.Columns.Add(Prop, Type.GetType("System.Guid"));
                                                    r[Prop] = Guid.Empty;
                                                }
                                                else
                                                {
                                                    //this loop get added because field contain null data
                                                    if (!table.Columns.Contains(Prop))
                                                        table.Columns.Add(Prop);
                                                    r[Prop] = DBNull.Value;
                                                }
                                            }
                                            else if (Prop.ToLower().Equals("objecttypecode") || Prop.ToLower().Equals("primaryentitytypecode") || Prop.ToLower().Equals("primaryentity"))
                                            {
                                                string strLogicalName = en.Attributes[Prop.ToLower()].ToString();
                                                int objectcodetype = mapEntityMetadata[strLogicalName].ObjectTypeCode.Value;
                                                if (!table.Columns.Contains(Prop))
                                                    table.Columns.Add(Prop);
                                                r[Prop] = objectcodetype;
                                            }
                                            else if (en.Attributes[Prop.ToLower()].GetType().ToString().Equals("System.Boolean", StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                bool b = bool.Parse(en.Attributes[Prop.ToLower()].ToString());
                                                if (true == b)
                                                {
                                                    if (!table.Columns.Contains(Prop))
                                                        table.Columns.Add(Prop, Type.GetType("System.Int32"));
                                                    r[Prop] = 1;
                                                }
                                                else
                                                {
                                                    if (!table.Columns.Contains(Prop))
                                                        table.Columns.Add(Prop, Type.GetType("System.Int32"));
                                                    r[Prop] = 0;
                                                }
                                            }
                                            else if (en.Attributes[Prop.ToLower()] is EntityReference)
                                            {
                                                EntityReference er = (EntityReference)en.Attributes[Prop.ToLower()];
                                                if (!table.Columns.Contains(Prop))
                                                    table.Columns.Add(Prop, Type.GetType("System.Guid"));
                                                r[Prop] = er.Id;
                                            }
                                            else if (en.Attributes[Prop.ToLower()] is OptionSetValue)
                                            {
                                                OptionSetValue op = (OptionSetValue)en.Attributes[Prop.ToLower()];
                                                if (!table.Columns.Contains(Prop))
                                                    table.Columns.Add(Prop);
                                                r[Prop] = op.Value;
                                            }
                                            else if (en.Attributes[Prop.ToLower()] is Guid)
                                            {
                                                if (!table.Columns.Contains(Prop))
                                                    table.Columns.Add(Prop, Type.GetType("System.Guid"));
                                                r[Prop] = en.Attributes[Prop.ToLower()];
                                            }
                                            else
                                            {
                                                if (!table.Columns.Contains(Prop))
                                                    table.Columns.Add(Prop);
                                                r[Prop] = en.Attributes[Prop.ToLower()];
                                            }
                                        }
                                    }

                                    table.Rows.Add(r);

                                    totalRecordsProcessed++;

                                    Trace.WriteLineIf(0 == (totalRecordsProcessed % 1000), "Read " + totalRecordsProcessed + " " + entityName + " (More to Come)");

                                    if (topRows.HasValue && totalRecordsProcessed >= topRows)
                                    {
                                        getmorerows = false;
                                        break;
                                    }
                                }
                            }
                            //SQLBULK
                            if (totalRecordsProcessed > 0)
                            {
                                using (SqlBulkCopy copy = new SqlBulkCopy(emdbSqlConn))
                                {
                                    copy.BulkCopyTimeout = 600;
                                    copy.DestinationTableName = insertentityName;
                                    copy.NotifyAfter = 15;
                                    foreach (DataColumn column in table.Columns)
                                    {
                                        copy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                                    }
                                    copy.WriteToServer(table);
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {


                        if (!string.IsNullOrEmpty(e.Message) && e.Message.Contains("SQL timeout expired"))
                        {
                            _retryCount++;
                            expectedException = true;
                        }
                        else if (e.Message.Contains("The given ColumnMapping does not match up with any column in the source or destination"))
                        {
                            Console.WriteLine("Entity: " + entityName + "-  Exception:\n" + e.ToString());
                            Console.WriteLine("ColumnNames are Case Sensitive");
                            throw;
                        }
                        else
                        {
                            Console.WriteLine("Exception:\n" + e.ToString());
                            //Console.WriteLine("SQL " + EMCmd.CommandText);
                            //throw;
                        }
                    }
                }
                while (expectedException && _retryCount < 3);
            }
            return totalRecordsProcessed;
        }
        
     
		/// <summary>
		/// Build Query Expression based on Entity Name
		/// </summary>
		/// <param name="enEntityType">EntityType (Normal, Special)</param>
		/// <param name="entityName">Entiry Name</param>
		/// <param name="props">Array of column name user want to retrive</param>
		/// <param name="where">Filter Condition</param>
		/// <param name="OrderBy">Order by column</param>
		/// <returns>Return QueryExpression based on above parameter</returns>
		public static QueryExpression BuildQueryExpression(EntityType enEntityType, string entityName, string[] props, FilterExpression where, string OrderBy)
		{
			QueryExpression query = new QueryExpression(entityName.ToLower());
			query.PageInfo.Count = 5000;
			query.PageInfo.PageNumber = 1;
			if (props == null)
			{
				query.ColumnSet.AllColumns = true;
			}
			else
			{
				for (int i = 0; i < props.Length; i++)
				{
					if (props[i] != null && ((enEntityType == EntityType.Normal && props[i].ToLower() != "customeridname" && props[i].ToLower() != "organizationidname" && props[i].ToLower() != "businessunitidname") || (enEntityType == EntityType.Special && props[i] != "OrganizationId" && props[i] != "EntityManagerOwningUser")))
					{
						query.ColumnSet.AddColumn(props[i].ToLower());
					}
				}			
			}

			if (OrderBy != null)
			{
				query.AddOrder(OrderBy, OrderType.Ascending);
			}
			if (where != null)
			{
				query.Criteria = where;
			}
			return query;
		}

        	
		public static Guid GetOwnerGuid(Guid user, Guid Org)
		{
			Byte[] b1 = user.ToByteArray();
			Byte[] b2 = Org.ToByteArray();
			Byte[] b3 = new byte[16];

			for (int i = 0; i < 16; i++)
				b3[i] = (byte)((int)b1[i] + (int)b2[i]);

			Guid g = new Guid(b3);
			return g;
		}

		private static Int64 GetNextId(Hashtable htOwner, string strentityName)
		{
			if (false == htOwner.ContainsKey(strentityName))
			{
				htOwner.Add(strentityName, (System.Int64)0);
			}

			System.Int64 BaseVal = System.Int64.Parse(htOwner[EntityManagerID].ToString());
            if (strentityName.ToLower() == "team")
            {
                BaseVal += 10;
            }
            BaseVal = BaseVal << BitShift;

			System.Int64 NextID = (System.Int64)htOwner[strentityName];
			NextID++;

			if (System.Int64.MaxValue == NextID)
			{
				throw new Exception("Attempt to use reserved const of Int64.MaxValue");
			}

			htOwner[strentityName] = NextID;
			NextID += BaseVal;

			return NextID;
		}



	}
}
