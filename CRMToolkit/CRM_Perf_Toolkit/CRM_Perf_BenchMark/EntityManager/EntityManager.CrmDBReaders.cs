using System;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using ServiceCreator;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace CRM_Perf_BenchMark
{
	internal delegate void SyncCRMDataDelegate(MultipleServersData serverData, Guid organizationId);
	public partial class EntityManager
	{
        //JM: Defined at class level so its created once per thread
        public static IOrganizationService service;
        
		/// <summary>
		/// Overloaded ReadEntityFromTable. Assumes no optional WHERE filter is needed.
		/// </summary>
		/// <param name="EntityName">Name of entity to read. This MUST correspond to the SQL table name for the entity.</param>
		/// <param name="Props">String array of properties to read, i.e. {"OwnerID", "CreatedBy"} etc.
		///                     Prop[0] MUST be the owerID for the entity.
		///                     Prop[1] MUST be the primary key for the entity.
		///                     Additional properties optional.</param>
		/// <param name="UserOwned">Is the entityType owned by a ORG or a user.</param>
		/// <returns>Count of read entities.</returns>
		private static uint ReadEntityFromTable(MultipleServersData serverData, string EntityName, string[] Props, Guid orgId)
		{
			return ReadEntityFromTable(serverData, EntityName, Props, null, null, null, orgId);
		}

       
		/// <summary>
		/// Primary ReadEntityFromTable.
		/// </summary>
		/// <param name="EntityName">Name of entity to read. This MUST correspond to the SQL table name for the entity.</param>
		/// <param name="Props">String array of properties to read, i.e. {"OwnerID", "CreatedBy"} etc.
		///     Prop[0] MUST be the owerID for the entity.
		///     Prop[1] MUST be the primary key for the entity.
		///     Additional properties optional.</param>
		/// <param name="WHERE">WHERE clause added to end of query to filter. "WHERE" keyword provided by caller.</param>
		/// <param name="strORDERBY"></param>
		/// <param name="topClause"></param>
		/// <param name="UserOwned">Is the entityType owned by a ORG or a user.</param>
		/// <returns>Count of read entities.</returns>
        private static uint ReadEntityFromTable(MultipleServersData serverData, string EntityName, string[] Props, FilterExpression Where, string strORDERBY, int? topRows, Guid orgId, Guid? callingUserId = null, string inEntityName = "", CommonTypes.EntityType entityType = CommonTypes.EntityType.Normal)
        {
            if (serverData == null)
            {
                return CommonTypes.EntityReader.ReadEntityFromTable(adminUserProxy, ConfigSettings.Default.EMSQLCNN, EntityName, Props, Where, strORDERBY, topRows, orgId, m_Owners, inEntityName, entityType, callingUserId);
            }
            else
            {
                //JM Updated for Performance            
                if (service == null)
                {
                    service = GetAdminUserProxy(serverData);
                }
                return CommonTypes.EntityReader.ReadEntityFromTable(service, ConfigSettings.Default.EMSQLCNN, EntityName, Props, Where, strORDERBY, topRows, orgId, m_Owners, inEntityName, entityType, callingUserId);
            }
        }

		private static void ReadOrgs(MultipleServersData serverData, Guid orgId)
		{
			string[] Props = { null, EntityIDNames.Organization, "FeatureSet" };

			double NumOrgs = ReadEntityFromTable(serverData, EntityNames.Organizations, Props, orgId);

			if (1 != NumOrgs)
				throw new Exception(NumOrgs + " Were read from SQL. The number is not 1.");

			SqlCommand cmd = new SqlCommand();
			cmd.Connection = m_EMSQLCon;
			
			cmd.CommandText = string.Format("UPDATE {0} SET ServerBaseUrl=@ServerBaseUrl where {1} =  @orgId",
			EntityNames.Organizations, EntityIDNames.Organization);
			
			cmd.Parameters.AddWithValue("ServerBaseUrl", serverData.ServerBaseUrl);

			cmd.Parameters.AddWithValue("orgId", orgId);
			Trace.WriteLine("Issuing SQL Command to set default password " + cmd.CommandText);
			cmd.ExecuteNonQuery();

			//Need to enter the org as a owner in our global table.
			string SqlCommand = string.Format("SELECT {0}, {1} FROM {2} where {3} = @orgId", EntityIDNames.Organization,
				EntityManagerID, EntityNames.Organizations, EntityIDNames.Organization);
			SqlCommand getOrg = new SqlCommand(SqlCommand, m_EMSQLCon);
			getOrg.Parameters.AddWithValue("orgId", orgId);
			SqlDataReader reader = getOrg.ExecuteReader();
			try
			{
				while (reader.Read())
				{
					Hashtable Owner = new Hashtable();

					Owner.Add(EntityManagerID, System.Int64.Parse(reader[EntityManagerID].ToString()));
					if (m_Owners.ContainsKey(reader.GetGuid(0)))
					{
						continue;
					}

					m_Owners.Add(reader.GetGuid(0), Owner);
				}
			}
			finally
			{
				reader.Close();
			}
		}


		private static void ReadSetupUser(MultipleServersData serverData, Guid orgId)
		{
			if (serverData.userBase == null)
				return;

			string[] Props = { EntityIDNames.Organization, EntityIDNames.User, "DomainName", "BusinessUnitId", "SetupUser" };
			string EntityName = EntityNames.Users;
			FilterExpression where = new FilterExpression();
			where.AddCondition("isdisabled", ConditionOperator.Equal, true);
			where.AddCondition("setupuser", ConditionOperator.Equal, false);
			where.AddCondition("fullname", ConditionOperator.Equal, "SYSTEM");
			//use default system admin user
            Trace.WriteLine("Read: " + ReadEntityFromTable(serverData, EntityName, Props, where, EntityIDNames.User, null, orgId, null, "SetupUser") + " Users");
		}

		private static FilterExpression GetUserFilter(MultipleServersData serverData)
		{
			FilterExpression where = new FilterExpression();
			where.FilterOperator = LogicalOperator.Or;

			// This is the main query
			string userNameBase = serverData.userBase;
			int userStart = int.Parse(serverData.userStart);
			int userCount = int.Parse(serverData.userCount);
			int userEnd = userStart + userCount - 1;

			// Create user list for user in range(like crmusr1 to crmusr320)
			List<string> m_FilterUserSet = new List<string>();
			QueryExpression query;            

            FilterExpression where2 = new FilterExpression();
            where2.FilterOperator = LogicalOperator.Or;
            ConditionExpression usernamefilter = new ConditionExpression("domainname", ConditionOperator.Like, userNameBase+'%');
            where2.AddCondition(usernamefilter);


            //Get list of users from custom table in db if users are not sequential
            List<string> m_FilterUserList = new List<string>();
            string cmdText = string.Format("SELECT rtrim([Username]) from [UserLIst]");
            SqlCommand getUsers = new SqlCommand(cmdText, m_EMSQLCon);
            using (SqlDataReader reader = getUsers.ExecuteReader())
            {                
                while (reader.Read())
                {
					m_FilterUserList.Add(reader[0].ToString().ToLower());
                }                
            }

            if (m_FilterUserList.Count > 0)
            {
                where2.FilterOperator = LogicalOperator.Or;
                ConditionExpression usernamefilter2 = new ConditionExpression("domainname", ConditionOperator.In, m_FilterUserList.ToArray());
                where2.AddCondition(usernamefilter2);
            }
            
            EntityCollection results = null;
			try
			{
				query = new QueryExpression();
				query.EntityName = "systemuser";
				string[] Props = { "systemuserid", "domainname" };

				bool getmorerows = true;
				query = CommonTypes.EntityReader.BuildQueryExpression(CommonTypes.EntityType.Normal, query.EntityName.ToString(), Props, where2, null);

				while (getmorerows)
				{
					results = adminUserProxy.RetrieveMultiple(query);
					query.PageInfo.PageNumber++;
					query.PageInfo.PagingCookie = results.PagingCookie;
					getmorerows = results.MoreRecords;

					for (int k = 0; k < results.Entities.Count; k++)
					{
						Entity en = results[k];
						string domainName = en.Attributes["domainname"].ToString();

						if (m_FilterUserList.Contains(domainName.ToLower()))
						{
							m_FilterUserSet.Add(en.Attributes["systemuserid"].ToString());
						}
						else if (domainName.IndexOf(userNameBase, StringComparison.InvariantCultureIgnoreCase) >= 0)
						{
							var temp = domainName.Split('@');
							domainName = temp[0];
							try
							{
								int count = Int32.Parse(domainName.Substring(domainName.IndexOf(userNameBase, StringComparison.InvariantCultureIgnoreCase) + userNameBase.Length, domainName.Length - (domainName.IndexOf(userNameBase, StringComparison.InvariantCultureIgnoreCase) + userNameBase.Length)));
								if (count >= userStart && count <= userEnd)
								{
									m_FilterUserSet.Add(en.Attributes["systemuserid"].ToString());
								}
							}
							catch { }
						}
						
					}
				}
			}
			catch (System.Exception e)
			{
				Trace.WriteLine("Exception:\n" + e.ToString());
				throw;
			}
            if (m_FilterUserSet.Count > 0)
            {
                ConditionExpression userFilterCondition = new ConditionExpression("systemuserid", ConditionOperator.In, m_FilterUserSet.ToArray());
                where.AddCondition(userFilterCondition);
            }
            else {
                ConditionExpression userFilterCondition = new ConditionExpression("domainname", ConditionOperator.Equal, "noprefixmatches");
                where.AddCondition(userFilterCondition);
                Trace.WriteLine("");
                Trace.WriteLine("No users were found that match prefix " + userNameBase + " or the users listed in the UserList Table. Verify Test users are enabled in CRM and are properly referenced in Config File or UserList Table");
                Trace.WriteLine("");
            }
			return where;
		}

		private static void ReadUsers(MultipleServersData serverData, Guid orgId)
		{
			string[] Props = { "OrganizationId", "SystemUserId", "DomainName", "BusinessUnitId", "SetupUser", "InternalEmailAddress", "BusinessUnitIdName" };
			var userFilter = GetUserFilter(serverData);
			var usersRead = ReadEntityFromTable(serverData, "SystemUser", Props, userFilter, EntityIDNames.User, null, orgId);
			Trace.WriteLine(string.Format("Read: {0} users", usersRead));
			if (usersRead == 0)
			{                
               throw new Exception("Failed to read CRM users to EMDB");
			}

			string cmdText = string.Format("UPDATE SystemUser SET ServerBaseUrl='{0}', OrganizationBaseUrl='{1}', OrganizationServiceUrl='{2}', 			DiscoveryServer='{3}',  UserPassword='{4}',  OrganizationName='{5}' "
			, serverData.ServerBaseUrl, serverData.OrganizationBaseUrl, serverData.OrganizationServiceUrl, serverData.DiscoveryServer, serverData.userPassword, serverData.OrgName);
			cmdText += string.Format(" where {0} = '{1}'", EntityIDNames.Organization, orgId);
			SqlCommand cmd = new SqlCommand(cmdText, m_EMSQLCon);
			cmd.Parameters.AddWithValue("serverBaseUrl", serverData.ServerBaseUrl);
			cmd.Parameters.AddWithValue("organizationBaseUrl", serverData.OrganizationBaseUrl);
			cmd.Parameters.AddWithValue("organizationServiceUrl", serverData.OrganizationServiceUrl);
			cmd.Parameters.AddWithValue("discoveryServer", serverData.DiscoveryServer);
			cmd.Parameters.AddWithValue("userPassword", serverData.userPassword);
			cmd.Parameters.AddWithValue("OrganizationName", serverData.OrgName);
			cmd.Parameters.AddWithValue("orgId", orgId);			
			cmd.ExecuteNonQuery();

			var userManager = GetUserManager();

            ////read passwords from custom table and set user passwords if passwords are different
            string cmdText2 = string.Format("update systemuser set UserPassword = ltrim(rtrim(Password)) from systemuser inner join UserLIst on systemuser.DomainName = rtrim(UserLIst.Username)");
            SqlCommand getUsers = new SqlCommand(cmdText2, m_EMSQLCon);
            getUsers.ExecuteNonQuery();

            //Need to add the users as owners and set the org as a owner in our global tables
            cmdText = string.Format("SELECT SystemUserId, {0} from SystemUser {1} where {1} = @orgId", EntityManagerID, EntityIDNames.Organization);
			SqlCommand getOrg = new SqlCommand(cmdText, m_EMSQLCon);
			getOrg.Parameters.AddWithValue("orgId", orgId);
			using (SqlDataReader reader = getOrg.ExecuteReader())
			{
				while (reader.Read())
				{
					Hashtable owner = new Hashtable();
					owner.Add(EntityManagerID, System.Int64.Parse(reader[EntityManagerID].ToString()));
					//get a new user guid from user id and org id
					Guid g = GetOwnerGuid(reader.GetGuid(0), orgId);
					m_Owners.Add(g, owner);
				}
			}

		}
    }
}
