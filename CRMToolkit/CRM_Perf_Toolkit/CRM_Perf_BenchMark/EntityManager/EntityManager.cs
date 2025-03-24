using Microsoft.Exchange.WebServices.Data;
//included to clean up registries when we are loading the entitymanager database
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using ServiceCreator;
using Microsoft.Xrm.Sdk.Client;
using System.Configuration;
using System.Collections.Concurrent;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Messages;

namespace CRM_Perf_BenchMark
{
	/// <summary>
	/// Class to manage CRM Entities for perf testing.
	/// </summary>
	public partial class EntityManager
	{
		private static TimeSpan DefaultTimeout = new TimeSpan(0, 59, 0);
		
		private static Lazy<EntityManager> m_instance = new Lazy<EntityManager>(() =>
		{
			return new EntityManager();
		});

		private static Lazy<IUserManager> m_userManager = new Lazy<IUserManager>(() => CreateUserManagerInstance(ConfigSettings.Default.EMSQLCNN));
			

		public static IUserManager GetUserManager()
		{
			return m_userManager.Value;
		}


		// Singleton instance
		public static EntityManager Instance
		{
			get
			{
				return m_instance.Value;
			}
		}

		/// <summary>
		/// Private constructor
		/// </summary>
		private EntityManager()
		{
			Init();
		}
		

		/// <summary>
		/// Initializes manager by reading app settings from app.config.
		/// Then loads information about all entities from the CRM SQL Server.
		/// It is up to callers to provide thread protection, this function is not multi-thread friendly.
		/// </summary>
		private void Init()
		{
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            System.Threading.ThreadPool.SetMinThreads(100, 100);
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;

			LoadFromSQL();
		}

		private static Lazy<Dictionary<string, string>> _Context = new Lazy<Dictionary<string, string>>(() => new Dictionary<string, string>());
		public static Dictionary<string, string> Context { get { return _Context.Value; } }

		private static object OwnerLock = new object();

		public const string EntityManagerID = "ID";
		public const string EntityManagerStateCode = "State";

		public const string AppendFilter = "AppendFilter";
		public const string IsDeletableFlag = "deletable";

		//we are changing the BitShift from 46 to 25 to support multi org scenarios
		//we are assuming we are not going to have more than 1000 orgs => 10 bits
		//Remaining bits = 64- 10 = 54. We do shift twice - one for Org owned entities
		//and another for user owned entities

		//In future we are going to have different BitShift value for Org owned entities and
		//user owned entities and these will be read from ConfigSettings.xml so that
		//we will be able to change them run time

		//with Bit shift == 25, each user can have 2^25 = 33,554,432 entities
		private const int BitShift = 25;

		/// <summary>
		/// Connection to SQL Server, used for Init()
		/// </summary>
		//private static SqlConnection m_CRMSQLCon = null;
		private static SqlConnection m_EMSQLCon = null;

		private System.Random m_RanNum = new Random();

		private static Dictionary<Guid, Hashtable> m_Owners = new Dictionary<Guid, Hashtable>();

		private static List<EntityMetadata> m_EntityMatadatas = new List<EntityMetadata>();
		
		private static ConcurrentDictionary<string, IOrganizationService> m_userXrmServiceTable;

		private bool m_isUnitTestCase = false;
		public bool IsUnitTestCase
		{
			get
			{
				return m_isUnitTestCase;
			}
			set
			{
				m_isUnitTestCase = value;
			}
		}

		private bool m_isPerUserPerRun = false;
		public bool IsPerUserPerRun
		{
			get
			{
				return m_isPerUserPerRun;
			}
			set
			{
				m_isPerUserPerRun = value;
			}
		}

		private static IOrganizationService adminUserProxy = null;

		public ConcurrentDictionary<string, IOrganizationService> UserXrmServiceTable
		{
			get
			{
				return m_userXrmServiceTable;
			}
		}

		#region Organization ID
		/// <summary>
		/// Makes life easier to just expose the ORG's GUID.
		/// we are using this in case of multi org scenario too - we set this value
		/// for every org we read to read entities for that org
		/// </summary>
		private static int m_numOrgs = 0;

		//private ArrayList m_OrganizationCount = new ArrayList();
		//private ArrayList m_OrganizationIds = new ArrayList();

		public static List<OrganizationInfo> Organizations = new List<OrganizationInfo>();

		//to suport multiple Orgs, we will assume a fake parent org
		public static Guid m_FakeOrgID = Guid.Empty;
	

		public Guid GetRandomOrg()
		{
			Guid orgId = Guid.Empty;

			try
			{
				EntityRequest orgRequest = new EntityRequest();
				orgRequest.Type = EntityNames.Organizations;
				orgRequest.ReturnAs = EntityNames.Organizations;
				orgRequest.ParentID = Guid.Empty;

				System.Collections.Hashtable entities = EntityManager.Instance.GetEntities(orgRequest);
				CRMEntity organization = (CRMEntity)entities[EntityNames.Organizations];

				orgId = new Guid(organization[EntityIDNames.Organization]);
			}
			catch (Exception ex)
			{
				FileWriter.Instance.WriteToFile(ex.ToString());
			}
			return orgId;
		}

				
		public CRMEntity GetRandomUserNoLock(string userRole, Guid orgId, Guid buID, Guid lockedUser)
		{
			CRMEntity user = null;
			SqlConnection sqlconn = new SqlConnection(ConfigSettings.Default.EMSQLCNN);
			SqlCommand sqlcmd = new SqlCommand();
			SqlDataReader reader = null;

			StringBuilder sb = new StringBuilder();
			sb.Append("select top 1 * from SystemUser WITH (NOLOCK) where organizationid = @orgId and systemuserid != @userId");
			sqlcmd.Parameters.AddWithValue("orgId", orgId.ToString());
			sqlcmd.Parameters.AddWithValue("userId", lockedUser.ToString());
			if (userRole != string.Empty)
			{
				sb.Append(" and role=@role");
				sqlcmd.Parameters.AddWithValue("role", userRole);
				if (buID != Guid.Empty)
				{
					sb.Append(" and businessunitid=@buId ");
					sqlcmd.Parameters.AddWithValue("buId", buID);
				}
			}
			sb.Append(" ORDER BY NEWID()");
			sqlconn = new SqlConnection(ConfigSettings.Default.EMSQLCNN);
			try
			{
				sqlconn.Open();
				sqlcmd.CommandText = sb.ToString();
				sqlcmd.Connection = sqlconn;

				DataTable table = null;
				table = new DataTable();
				try
				{
					reader = sqlcmd.ExecuteReader();
					table.Load(reader);
				}
				finally
				{
					if (null != reader)
						reader.Close();
				}

				user = new CRMEntity(table, 0, EntityNames.Users);
			}
			catch (SqlException sex)
			{
				FileWriter.Instance.WriteToFile(sex.ToString());
			}
			catch (Exception ex)
			{
				FileWriter.Instance.WriteToFile(ex.ToString());
			}
			finally
			{
				sqlconn.Close();
			}

			return user;
		}

		public static Guid GetOwnerGuid(Guid userId, Guid orgId)
		{
			return CommonTypes.EntityReader.GetOwnerGuid(userId, orgId);
		}



		#endregion


		public static string GetEntityIDName(string Type)
		{
			switch (Type)
			{
				case EntityNames.Accounts:
					return EntityIDNames.Account;
				case EntityNames.BusinessUnits:
					return EntityIDNames.BusinessUnit;
				case EntityNames.Campaigns:
					return EntityIDNames.Campaign;
				case EntityNames.Contacts:
					return EntityIDNames.Contact;
				case EntityNames.Equipment:
					return EntityIDNames.Equipment;
				case EntityNames.Team:
					return EntityIDNames.Team;
				case EntityNames.SavedQueryVisualizations:
					return EntityIDNames.SavedQueryVisualizationId;
				case EntityNames.SpSite:
					return EntityIDNames.SpSite;
				case EntityNames.SharePointDocumentLocation:
					return EntityIDNames.SharePointDocumentLocation;
				case EntityNames.Incidents:
					return EntityIDNames.Incident;
				case EntityNames.Leads:
					return EntityIDNames.Lead;
				case EntityNames.Lists:
					return EntityIDNames.List;
				case EntityNames.Notes:
					return EntityIDNames.Note;
				case EntityNames.Opportunities:
					return EntityIDNames.Opportunities;
				case EntityNames.Organizations:
					return EntityIDNames.Organization;
				case EntityNames.PriceLevels:
					return EntityIDNames.PriceLevel;
				case EntityNames.Privileges:
					return EntityIDNames.Privilege;
				case EntityNames.Quotes:
					return EntityIDNames.Quote;
				case EntityNames.Roles:
					return EntityIDNames.Role;
				case EntityNames.Service:
					return EntityIDNames.Service;
				case EntityNames.ServiceAppointments:
					return EntityIDNames.ServiceAppointment;
				case EntityNames.RecurringAppointments:
					return EntityIDNames.RecurringAppointmentMaster;
				case EntityNames.Subject:
					return EntityIDNames.Subject;
				case EntityNames.Tasks:
					return EntityIDNames.Task;
				case EntityNames.Templates:
					return EntityIDNames.Template;
				case EntityNames.Users:
					return EntityIDNames.User;
				case EntityNames.Reports:
					return EntityIDNames.Report;
				case EntityNames.TransactionCurrency:
					return EntityIDNames.TransactionCurrency;
				case EntityNames.ConnectionRole:
					return EntityIDNames.ConnectionRole;
				case EntityNames.Queue:
					return EntityIDNames.Queue;
				case EntityNames.Workflow:
					return EntityIDNames.Workflow;
				case EntityNames.Products:
					return EntityIDNames.Products;
				case EntityNames.ProductPriceLevels:
					return EntityIDNames.ProductPriceLevels;
				case EntityNames.New_IM:
					return EntityIDNames.New_IM;
				case EntityNames.New_Prospect:
					return EntityIDNames.New_Prospect;
                case EntityNames.TeamMembership:
                    return EntityIDNames.TeamMembership;
				default:
					throw new NotImplementedException("Add Type " + Type.ToString());
			}
		}


		/// <summary>
		/// Truncate the SQL tables for a new set of data.
		/// </summary>
		private static void TruncateTables()
		{
			SqlCommand TruncateCommand = new SqlCommand();

			TruncateCommand.Connection = m_EMSQLCon;
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Accounts;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.BusinessUnits;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Campaigns;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Contacts;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Equipment;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Team;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.SavedQueryVisualizations;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.SpSite;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.SharePointDocumentLocation;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.GenericActivities;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Incidents;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.KnowledgeArticle;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Leads;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Lists;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Notes;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Opportunities;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Organizations;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.PriceLevels;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Privileges;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Queue;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Quotes;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Reports;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Roles;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Service;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.ServiceAppointments;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Subject;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Tasks;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.TransactionCurrency;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Appointments;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.RecurringAppointments;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Emails;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Letters;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Faxes;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Goals;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Phonecalls;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.ConnectionRole;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Templates;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Users;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Solution;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Workflow;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.New_IM;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.New_Prospect;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + "FakeParentOrg";
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Competitor;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Products;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.ProductPriceLevels;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.Resource;
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.SalesOrders;
			TruncateCommand.ExecuteNonQuery();
            TruncateCommand.CommandText = "TRUNCATE TABLE " + EntityNames.TeamMembership;
            TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE SetupUser";
			TruncateCommand.ExecuteNonQuery();
		 
		
		
			//Temp tables in EMDB side will be truncated.
			TruncateCommand.CommandText = "TRUNCATE TABLE tempSystemUserRole";
			TruncateCommand.ExecuteNonQuery();
			TruncateCommand.CommandText = "TRUNCATE TABLE tempUserRole";
			TruncateCommand.ExecuteNonQuery();
		}

		/// <summary>
		/// EMDB is loaded already. Load the data from EMDB into memory
		/// </summary>
		private void LoadFromSQL()
		{
			// Try block to wrap SQL access to ensure connection is closed. No Exception handling.
			try
			{
				if (ConfigSettings.Default.m_MulServers == null || ConfigSettings.Default.m_MulServers.Length == 0)
				{
					throw new Exception("No server configurations found");
				}
				// Load the entities into memory from the EM DB
				Trace.WriteLine("Connecting to EntityManager SQL: " + ConfigSettings.Default.EMSQLCNN);
				m_EMSQLCon = new SqlConnection(ConfigSettings.Default.EMSQLCNN);
				m_EMSQLCon.Open();
				Trace.WriteLine("Connected to Entity Manager SQL Database");
				BuildFromSQL();

			}
			catch (Exception ex)
			{
				string msg = string.Format("Failure in loading EMDB from SQL. Error msg: {0}", ex.Message);
				Trace.WriteLine(msg);
				//throw new Exception(msg, ex);
			}
		}

		/// <summary>
		/// Load data from CRM DB to EMDB
		/// </summary>
		public static void LoadFromCRM()
		{
			// Try block to wrap SQL access to ensure connection is closed. No Exception handling.
			try
			{
				if (ConfigSettings.Default.m_MulServers == null || ConfigSettings.Default.m_MulServers.Length == 0)
				{
					throw new Exception("No server configurations found");
				}
				CheckEMDB();
				if (ConfigSettings.Default.OutlookSyncDir != string.Empty)
				{
					Directory.CreateDirectory(ConfigSettings.Default.OutlookSyncDir + "\\SyncToOutlook");
					Directory.CreateDirectory(ConfigSettings.Default.OutlookSyncDir + "\\ManagedABPSYnc");
					Directory.CreateDirectory(ConfigSettings.Default.OutlookSyncDir + "\\GoOffline");
				}
				RefreshEmDbFromCrmDbs();
			}
			catch (Exception ex)
			{
				string msg = string.Format("Failure in loading EMDB from CRM. Error msg: {0}", ex.Message);
				Trace.WriteLine(msg);
				throw new Exception(msg, ex);
			}
		}

		public static void RefreshEmDbFromCrmDbs(string buildNumber = null)
		{
			// Reset the owners
			m_Owners.Clear();

			//we have to read how many crm servers we are reading from
			Trace.WriteLine("Connecting to EntityManager SQL: " + ConfigSettings.Default.EMSQLCNN);
			m_EMSQLCon = new SqlConnection(ConfigSettings.Default.EMSQLCNN);
			m_EMSQLCon.Open();
			Trace.WriteLine("Connected to Entity Manager SQL Database");

			TruncateTables();

			PopulateFakeParentOrg();

			m_userXrmServiceTable = new ConcurrentDictionary<string, IOrganizationService>();

			m_numOrgs = ConfigSettings.Default.m_MulServers.Length;

			foreach (var serverInfo in ConfigSettings.Default.m_MulServers)
			{
				adminUserProxy = GetAdminUserProxy(serverInfo);
				ReadAllEntitesFromCrmDBIntoEmDB(serverInfo);
			}

		}

		public static void RefreshAllAuthTokens()
		{
			var userManager = GetUserManager();

			for (int i = 0; i < ConfigSettings.Default.m_MulServers.Length; i++)
			{
				string start = ConfigSettings.Default.m_MulServers[i].userStart;
				string count = ConfigSettings.Default.m_MulServers[i].userCount;
				string userBase = ConfigSettings.Default.m_MulServers[i].userBase;
				string domain = ConfigSettings.Default.m_MulServers[i].userDomain;
				string password = ConfigSettings.Default.m_MulServers[i].userPassword;
				string orgName = ConfigSettings.Default.m_MulServers[i].OrgName;
				string orgBaseUrl = ConfigSettings.Default.m_MulServers[i].OrganizationBaseUrl;
			}
		}

		/// <summary>
		/// Load CRM Data into EM Database
		/// </summary>
		/// <param name="serverData"></param>
		private static void ReadAllEntitesFromCrmDBIntoEmDB(MultipleServersData serverData)
		{
			Trace.WriteLine("Connected to CRM SQL Database");
			Guid orgID = UpdateCurrentOrgIdFromCrmDb();

			ReadOrgs(serverData, orgID);
			ReadSetupUser(serverData, orgID);
			ReadUsers(serverData, orgID);		
		}

		/// <summary>
		/// help methond to check if EMDB exists on the client
		/// If EMDB exists, do nothing. Otherwise create EMDB on the client
		/// </summary>
		private static void CheckEMDB()
		{
            		//check if EMDB exists, if not, create one
			 //check if EMDB exists, if not, create one
            string ICMarker = "Initial Catalog=";
            int IC = ConfigSettings.Default.EMSQLCNN.IndexOf(ICMarker) + ICMarker.Length;
            int ICend = ConfigSettings.Default.EMSQLCNN.IndexOf(';',IC);
            string emdbName = ConfigSettings.Default.EMSQLCNN.Substring(IC,ICend-IC).Replace("'","");

            string ServerMarker = ";Server=";
            int SIC = ConfigSettings.Default.EMSQLCNN.IndexOf(ServerMarker) + ServerMarker.Length;
            int SICend = ConfigSettings.Default.EMSQLCNN.IndexOf(';', SIC);
            string instanceName = ConfigSettings.Default.EMSQLCNN.Substring(SIC, SICend - SIC);

            bool dbfound = false;
			using (SqlCommand emdbCheck = new SqlCommand("SELECT name FROM master.sys.databases WHERE name = @name"))
			{
				emdbCheck.Parameters.AddWithValue("name", emdbName);
				using (var sqlConn = new SqlConnection(ConfigSettings.Default.EMSQLCNN.Replace(emdbName,"master")))
				{
					sqlConn.Open();
					emdbCheck.Connection = sqlConn;

					using (var emdbCheckReader = emdbCheck.ExecuteReader())
					{
						while (emdbCheckReader.Read())
						{
                            dbfound = true;
                        }
					}
				}
			}

			//Create EMDB
			if (!dbfound)
			{
				string argument = string.Format(@" -S {1} -i ""{0}"" ", Path.Combine(ConfigSettings.Default.CRMToolkitRoot, @"Binaries\Scripts\SQL\EMDB_Setup.sql"), instanceName);
				var process = Process.Start("sqlcmd.exe", argument);
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = false;
				try
				{
					process.Start();
					while (true)
					{
						if (process.HasExited)
						{
							break;
						}
						System.Threading.Thread.Sleep(2000);
					}
                    if (process.ExitCode != 0)
                    {
                        throw new Exception("Create EMDB failed; Please setup EMDB manually and try to run the test again");
                    }
                    else {
                        Trace.WriteLine("Created EntityManager Database " + emdbName + ".");
                    }
				}
				catch (Exception ex)
				{
					Trace.WriteLine("Failed to create EMDB.");
					Trace.WriteLine("Exception Message: {0}", ex.Message);
					throw ex;
				}
			}
            
		}

        private static void BuildFromSQL()
        {
            // Load all the Organizations
            ResetStateCode("organization");
            LoadAllOrganizations();
            m_userXrmServiceTable = new ConcurrentDictionary<string, IOrganizationService>();

            // Load all the users
            LoadAllUsers();
            BuildEntityFromSQL(EntityNames.Users, "OrganizationID");
        }

        private static void ResetStateCode(string EntityName)
		{
			int rowCnt = 0;
			using (SqlConnection emsqlCon = new SqlConnection(ConfigSettings.Default.EMSQLCNN))
			{
				emsqlCon.Open();
				string cmdText = string.Format("UPDATE {0} SET {1}=@state WHERE {2}=@stateCode", EntityName, EntityManagerStateCode, EntityManagerStateCode);
                
                if (EntityName.ToLower() == "systemuser"){
                    cmdText += string.Format(" UPDATE systemuser SET state = 1 WHERE domainname != ''");
                }
				using (SqlCommand cmd = new SqlCommand(cmdText, emsqlCon))
				{
					cmd.Parameters.AddWithValue("state", (int)EntityStates.Free);
					cmd.Parameters.AddWithValue("stateCode", (int)EntityStates.InUse);
					rowCnt = cmd.ExecuteNonQuery();
				}
			}
		}
			
	
		private static void BuildEntityFromSQL(string EntityName, string KeyName)
		{
			Trace.WriteLine("Building memory tables for entity: " + EntityName);

			SqlCommand cmd = new SqlCommand();
			cmd.Connection = m_EMSQLCon;
			cmd.CommandTimeout = 0;

			cmd.CommandText = "SELECT count(*) as Count, " + KeyName + " FROM " + EntityName + " GROUP BY " + KeyName;
			SqlDataReader reader = cmd.ExecuteReader();

			ulong totalRecords = 0;
			try
			{
				while (reader.Read())
				{
					var key = reader[KeyName].ToString();
					Hashtable Owner = m_Owners[new Guid(key)];

					System.Int64 numRead = System.Int64.Parse(reader["Count"].ToString());

					totalRecords += (UInt64)numRead;

					if (Owner.ContainsKey(EntityName))
					{
						var old = (System.Int64)Owner[EntityName];
						Owner[EntityName] = old + numRead;
					}
					else
					{
						Owner.Add(EntityName, numRead);
					}
				}
			}
			finally
			{
				reader.Close();
			}

			Trace.WriteLine(string.Format("\tTotal: {0}", totalRecords));

			ResetStateCode(EntityName);
		}

	
		#region Entity Loading


		private static void PopulateFakeParentOrg()
		{
			Guid fakeguid = new Guid("19007b7e-d9a3-da11-b5fc-001143d30bf2");

			string SqlCommand = "Insert into FakeParentOrg (ID,OrganizationId) Values (0, @fakeguid)";
			SqlCommand getOrg = new SqlCommand(SqlCommand, m_EMSQLCon);
			getOrg.Parameters.AddWithValue("fakeguid", fakeguid);
			getOrg.ExecuteNonQuery();

			Hashtable orgOwner = new Hashtable();

			orgOwner.Add(EntityManagerID, (System.Int64)0);

			//Need to enter the org as a owner in our global table.
			SqlCommand = "SELECT  Top 1 " + EntityIDNames.Organization + " FROM FakeParentOrg";
			getOrg = new SqlCommand(SqlCommand, m_EMSQLCon);
			SqlDataReader reader = getOrg.ExecuteReader();
			try
			{
				while (reader.Read())
				{

					//if (false == reader.Read())
					//    throw new Exception("Failed to read any rows while searching for the org.");

					m_Owners.Add(reader.GetGuid(0), orgOwner);
					m_FakeOrgID = reader.GetGuid(0);
				}
			}
			finally
			{
				reader.Close();
			}
		}

		private static void LoadAllOrganizations()
		{
			// Load all the Organizations
			//to support multi server scenario, we are going to read crmServer name also from Org as well as user table
			//Read from the EM DB and setup the memory structures we need to run the test cases.
			string cmdText = string.Format("SELECT [{0}], [OrganizationId], [ServerBaseUrl], [FeatureSet], [ExchangeMode], [{1}] FROM {2}", EntityManagerID, EntityManagerStateCode, EntityNames.Organizations);
			using (SqlCommand cmd = new SqlCommand(cmdText, m_EMSQLCon))
			{
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						//Build the owners table.
						Hashtable Owner = new Hashtable();
						System.Int64 ID = System.Int64.Parse(reader[0].ToString());
						var orgInfo = new OrganizationInfo
						{
							OrganizationId = reader.GetGuid(1),
							ServerBaseUrl = reader.GetString(2),
							ExchangeMode = reader.GetBoolean(4),
							State = (EntityStates)reader.GetInt32(5)
						};
						// Return the first matching server
						orgInfo.ServerInfo = ConfigSettings.Default.m_MulServers.Where(s =>
						string.Compare(s.ServerBaseUrl, orgInfo.ServerBaseUrl, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();

						// Store the organizations
						Organizations.Add(orgInfo);
						Owner.Add(EntityManagerID, ID);
						if (m_Owners.ContainsKey(orgInfo.OrganizationId))
							continue;

						m_Owners.Add(orgInfo.OrganizationId, Owner);
						m_numOrgs++;
					}
				}
			}
		}

		private static void LoadAllUsers()
		{
			SqlCommand cmd = new SqlCommand();
			cmd.Connection = m_EMSQLCon;
			cmd.CommandText = "SELECT " + EntityIDNames.User + " as OwnerID ," + EntityIDNames.Organization + " as OrgID," + EntityManagerID + " FROM " + EntityNames.Users;

			SqlDataReader reader = cmd.ExecuteReader();

			try
			{
				while (reader.Read())
				{
					//Build the owners table.
					Hashtable Owner = new Hashtable();
					Guid g = GetOwnerGuid(reader.GetGuid(0), reader.GetGuid(1));
					if (m_Owners.ContainsKey(g))
						continue;
					System.Int64 ID = System.Int64.Parse(reader[2].ToString());
					Owner.Add(EntityManagerID, ID);

					m_Owners.Add(g, Owner);
				}
			}
			finally
			{
				reader.Close();
			}

            //TODO: Add TEAMS TO OWNERS TABLE
            string SqlCommand2 = "SELECT " + EntityIDNames.Team + "," + EntityManagerID + "," + EntityIDNames.Organization + " as OrgID FROM " + EntityNames.Team;
            SqlCommand GetOrg = new SqlCommand(SqlCommand2, m_EMSQLCon);
            SqlDataReader readerteam = GetOrg.ExecuteReader();
            try
            {
                while (readerteam.Read())
                {
                    Hashtable Owner = new Hashtable();

                    Owner.Add(EntityManagerID, System.Int64.Parse(readerteam[EntityManagerID].ToString()));

                    //get a new user guid from user id and org id
                    Guid g = GetOwnerGuid(readerteam.GetGuid(0), readerteam.GetGuid(2));
                    m_Owners.Add(g, Owner);
                }
            }
            finally
            {
                readerteam.Close();
            }        
		}

		private static Guid UpdateCurrentOrgIdFromCrmDb()
		{
			try
			{
				EntityCollection results = null;
				bool getmorerows = true;

				QueryExpression query = new QueryExpression("organization");
				query.PageInfo.Count = 5000;
				query.PageInfo.PageNumber = 1;
				query.ColumnSet.AddColumn("organizationid");

				while (getmorerows)
				{
					results = adminUserProxy.RetrieveMultiple(query);
					query.PageInfo.PageNumber++;
					query.PageInfo.PagingCookie = results.PagingCookie;
					getmorerows = results.MoreRecords;

					for (int k = 0; k < results.Entities.Count; k++)
					{
						Entity en = results[k];
						if (en.Attributes.Contains("organizationid"))
						{
							return (Guid)en.Attributes["organizationid"];
						}
					}
				}
			}
			catch (System.Exception e)
			{
				Trace.WriteLine("Exception:\n" + e.ToString());
				throw;
			}
			return Guid.Empty;
		}
		#endregion

		private static IOrganizationService GetAdminUserProxy(MultipleServersData serverData)
		{
			IOrganizationService retService = XrmServiceCreator.CreateOrganizationService(serverData.OrganizationServiceUrl,
					ConfigSettings.Default.RunAsUser,
					ConfigSettings.Default.RunAsPassword, ConfigSettings.Default.AuthenticationType,
					DefaultTimeout);
			int maxTry = 0;
			while (retService == null && maxTry < 20)
			{
				retService = XrmServiceCreator.CreateOrganizationService(serverData.OrganizationServiceUrl,
					ConfigSettings.Default.RunAsUser,
					ConfigSettings.Default.RunAsPassword, ConfigSettings.Default.AuthenticationType,
					DefaultTimeout);
				maxTry++;
			}


			return retService;
		}
	

		private string GetSqlQuery(SqlCommand command)
		{
			string commandText = command.CommandText;
			if (command.CommandType == System.Data.CommandType.Text)
			{
				foreach (SqlParameter param in command.Parameters)
				{
					string replacement;
					switch (param.DbType)
					{
						case System.Data.DbType.Boolean:
							replacement = Convert.ToBoolean(param.Value) == true ? "1" : "0";
							break;
						case System.Data.DbType.String:
							replacement = String.Format("'{0}'", param.Value);
							break;
						default:
							replacement = param.Value.ToString();
							break;
					}
					commandText = commandText.Replace("@" + param.ParameterName, replacement);
				}
			}
			return commandText;
		}

		/// <summary>
		/// Used to Get Setup user related data
		/// </summary>
		/// <returns></returns>
		public Hashtable GetSetupUser()
		{

			CRMEntity FoundEntity = null;
			Hashtable RetEntities = new Hashtable();

			SqlConnection SqlCon = new SqlConnection(ConfigSettings.Default.EMSQLCNN);

			try
			{
				SqlCon.Open();
				//Build the SQL for the current request
				SqlTransaction tran = SqlCon.BeginTransaction(System.Data.IsolationLevel.Serializable);

				SqlCommand cmd = new SqlCommand();
				cmd.Transaction = tran;
				cmd.Connection = SqlCon;
				cmd.CommandText = "SELECT top 1 systemuserid FROM SetupUser";
				SqlDataReader reader = cmd.ExecuteReader();

				DataTable table = new DataTable();
				table.Load(reader);
				int numberOfRows = table.Rows.Count;

				if (numberOfRows <= 0)
				{
					throw new EntityNotFoundException("No rows found for entity: " + GetSqlQuery(cmd));
				}

				cmd.Parameters.Clear();
				tran.Dispose();
				reader.Close();
				cmd.Connection.Dispose();
				FoundEntity = new CRMEntity(table, 0, "setupuser");
				RetEntities.Add("setupuser", FoundEntity);

			}
			finally
			{
				SqlCon.Close();
			}

			return RetEntities;

		}
			
		public Hashtable GetEntities(EntityRequest er)
		{
			System.Int64 BaseVal = System.Int64.MinValue;
			System.Int64 MaxVal = System.Int64.MinValue;
			System.Int64 StartVal = System.Int64.MinValue;
			System.Int64 CurrentVal = 0;

			CRMEntity FoundEntity = null;
			String entityTypeFormFailure = null;
			Hashtable RetEntities = new Hashtable();

			SqlConnection SqlCon = new SqlConnection(ConfigSettings.Default.EMSQLCNN);

			try
			{
				SqlCon.Open();

				Guid ParentID = Guid.Empty;
				//If a parent is specified, get a start and max value.
				if (Guid.Empty != er.ParentID)
				{

					if (Guid.Empty != er.GrandParentID)
					{
						//this is a user owned entity, grand parent is the Org
						ParentID = GetOwnerGuid(er.ParentID, er.GrandParentID);

					}
					else
						ParentID = er.ParentID;
					System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
					nfi.NumberGroupSeparator = "";

					if (false == m_Owners.ContainsKey(ParentID))
						throw new EntityNotFoundException("Failed to find owner : " + er.ParentID.ToString());
					if (false == m_Owners[ParentID].ContainsKey(er.Type))
						throw new EntityNotFoundException("Owner : " + er.ParentID.ToString() + " Does not own any " + er.Type, er.Type);

					BaseVal = System.Int64.Parse(m_Owners[ParentID][EntityManagerID].ToString());
					BaseVal = (BaseVal << BitShift) + 1;

					//Now get the max val we should look at.
					MaxVal = System.Int64.Parse(m_Owners[ParentID][er.Type].ToString());
					MaxVal += BaseVal;

					StartVal = System.Int64.Parse(m_Owners[ParentID][er.Type].ToString());
					Double RanNum = m_RanNum.NextDouble();
					Double dbl = RanNum * StartVal;
					StartVal = System.Int64.Parse(dbl.ToString("n0", nfi));

					StartVal += BaseVal;
					if (StartVal >= MaxVal)
					{
						StartVal = MaxVal - 1;
					}
				}

				CurrentVal = StartVal;
				bool hasProperties = false;
				bool notFound = true;
				bool hasSubEntityRequest = false;
				do
				{
					//Build the SQL for the current request
					SqlTransaction tran = SqlCon.BeginTransaction(System.Data.IsolationLevel.Serializable);

					SqlCommand cmd = new SqlCommand();
					cmd.Transaction = tran;
					cmd.Connection = SqlCon;

					string where = " where ";
					if (!IsPerUserPerRun)
					{
                        if (!er.Type.Equals(EntityNames.Users))
                        {
                            where += EntityManagerStateCode + "=@" + EntityManagerStateCode + " AND ";
                            cmd.Parameters.AddWithValue(EntityManagerStateCode, (int)EntityStates.Free);
                        }
					}
					else
					{
						where += EntityManagerStateCode + "<>@" + EntityManagerStateCode + " AND ";
						cmd.Parameters.AddWithValue(EntityManagerStateCode, (int)EntityStates.Deleted);
					}

                    if (0 != er.Props.Count)
                    {
                        foreach (string Key in new ArrayList(er.Props.Keys))
                        {
                            object Prop = er.Props[Key];

                            if (!Prop.GetType().Name.Equals("EntityRequest[]"))
                            {
                                hasProperties = true;
                                //Attribute new_name added as it is used as filter condition in new_customAccount and new_customOpportunity
                                //entities for RollupCustomOpportunityCRUDUnitTest and RollupCustomAccountCRUDUnitTest

                                if (Key.ToLower().Equals("name") || Key.ToLower().Equals("lastname") || Key.ToLower().Equals("subject") || Key.ToLower().Equals("title") || Key.ToLower().Equals("domainname") || Key.ToLower().Equals("new_name"))
                                {
                                    where += Key + " like '%" + ((string)Prop) + "%' AND ";
                                }
                                else if (Prop.ToString().Equals("not-null"))
                                {
                                    where += Key + " IS NOT NULL AND ";
                                }
                                else if (Prop.ToString().Equals("null"))
                                {
                                    where += Key + " IS NULL AND ";
                                }
                                else
                                {
                                    where += Key + "=@" + Key + " AND ";
                                    cmd.Parameters.AddWithValue(Key, Prop.ToString());
                                }
                            }
                            else
                            {
                                hasSubEntityRequest = true;
                            }
                        }
                    }


					if (System.Int64.MinValue != StartVal)
					{
						where += EntityManagerID + ">= @BaseVal AND ";
						cmd.Parameters.AddWithValue("BaseVal", BaseVal.ToString());
						where += EntityManagerID + "<= @MaxVal AND ";
						cmd.Parameters.AddWithValue("MaxVal", MaxVal.ToString());

						if (hasProperties == false && !er.DoNotUseIDLookup)
						{
							if (System.Int64.MinValue != CurrentVal)
							{
								if (!er.Type.Equals(EntityNames.Users))
								{
									where += EntityManagerID + "= @CurrentVal AND ";
									cmd.Parameters.AddWithValue("CurrentVal", CurrentVal.ToString());
								}
							}
						}
					}
									
					//Trim off the the " AND " at the end.
					where = where.Remove(where.Length - 5);

					// If there is a Sub Entity Request we need to get all the entities but if there is no sub entity request then we can only get top 10
					if (hasSubEntityRequest)
					{
						cmd.CommandText = "SELECT top 10 * FROM " + er.Type;
					}
					else
					{
						cmd.CommandText = "SELECT TOP 1 * FROM " + er.Type;
					}
					cmd.CommandText += where;
					                   
					cmd.CommandText += " ORDER BY NEWID()";
					

					int retryCount = 13;
					int retryTime = 1000;
					int numberOfRows = 0;
					SqlDataReader reader = null;
					DataTable table = null;
					int randomRow = -1;
					try
					{
						if (er.Type.Equals("SystemUser"))
						{
							while (retryCount > 0 && numberOfRows <= 0)
							{
								reader = cmd.ExecuteReader();
								table = new DataTable();
								table.Load(reader);
								numberOfRows = table.Rows.Count;
								retryCount--;
								if (retryCount >= 1 && numberOfRows <= 0)
								{
									reader.Close();
									cmd.Connection.Dispose();

									Trace.WriteLine(string.Format("Waiting for an available {0} ...", er.Type));
									System.Threading.Thread.Sleep(retryTime);
									if (retryCount >= 8)
									{
										retryTime += 500;
									}
									else if (retryCount < 8 && retryCount > 3)
									{
										retryTime += 1500;
									}
									else
									{
										retryTime += 2000;
									}
									SqlCon = new SqlConnection(m_EMSQLCon.ConnectionString);
									SqlCon.Open();
									tran = SqlCon.BeginTransaction(System.Data.IsolationLevel.Serializable);
									cmd.Transaction = tran;
									cmd.Connection = SqlCon;
								}
							}
						}
						else
						{
							reader = cmd.ExecuteReader();
							table = new DataTable();
							table.Load(reader);
							numberOfRows = table.Rows.Count;
						}
					}
					finally
					{
						//cmd.Parameters.Clear();
						tran.Dispose();
						reader.Close();
						cmd.Connection.Dispose();
					}
					if (numberOfRows <= 0)
					{
						if (er.Type.Equals("SystemUser"))
						{
							throw new EntityNotFoundException(string.Format(@"Failed to get Free System User. Retried for {0} ms: {1}", retryTime, GetSqlQuery(cmd)), er.Type);
						}
						else
						{
							throw new EntityNotFoundException("No rows found for entity: " + GetSqlQuery(cmd), er.Type);
						}
					}

					randomRow = 0;
					if (numberOfRows > 1)
					{
						randomRow = m_RanNum.Next(numberOfRows);
					}
					FoundEntity = new CRMEntity(table, randomRow, er.Type);

					int startRow = randomRow;
					while (null != FoundEntity)
					{
						try
						{
							//Now we need to check and see if recursion is necessary.
							foreach (string Key in new ArrayList(er.Props.Keys))
							{
								object Prop = er.Props[Key];
								if (Prop.GetType().Name.Equals("EntityRequest[]"))
								{
									EntityRequest[] EntitesRequested = (EntityRequest[])Prop;
									foreach (EntityRequest SubEr in EntitesRequested)
									{
										if (SubEr.ParentID != Guid.Empty)
											SubEr.ParentID = new Guid(FoundEntity[GetEntityIDName(er.Type)]);
										else
										{
											//Modify the requested SubEr to use a strange foreign key relationship.
											if (true == SubEr.Props.ContainsKey(Key))
												SubEr.Props.Remove(Key);

											SubEr.Props.Add(Key, FoundEntity[GetEntityIDName(er.Type)]);

										}
										SubEr.GrandParentID = ParentID;
										Hashtable NewRet = GetEntities(SubEr);

										foreach (string SubKey in new ArrayList(NewRet.Keys))
											RetEntities.Add(SubKey, NewRet[SubKey]);
									}
								}
							}
						}
						catch (EntityNotFoundException enfEx)
						{
							RetEntities.Clear();
							FoundEntity = null;
							entityTypeFormFailure = enfEx.EntityType;
						}

						if (null == FoundEntity)
						{
							if (numberOfRows == 1)
							{
								//Advance to the next position.
								if (CurrentVal++ == MaxVal)
									CurrentVal = BaseVal;
							}
							else if (numberOfRows > 1)
							{
								if (++randomRow == numberOfRows)
								{
									Trace.WriteLine("Unable to find Sub Entities for Entity, going to next Entity");
									randomRow = 0;
								}
								if (randomRow == startRow)
								{
									// Did not find any sub entities for all top level entities. need to break out of outer loop as well
									notFound = false;
									Trace.WriteLine("Did not find sub entities for range of entities, breaking out");
									break;
								}
								FoundEntity = new CRMEntity(table, randomRow, er.Type);
							}
						}
						else
						{
							RetEntities.Add(er.ReturnAs, FoundEntity);
							// Only lock the user if sub requests were also successful and the user is not for unit tests
							if (er.Type.Equals(EntityNames.Users) && !IsUnitTestCase)
							//if (er.Type.Equals(EntityNames.Users) )
							{
								InUseEntity(FoundEntity);
							}
							notFound = false;
							break;
						}
					}

				} while (StartVal != CurrentVal && hasProperties == false && notFound == true);

				if (null == FoundEntity)
				{
					throw new EntityNotFoundException("Failed to find any entities for Request: " + er.ToString(), entityTypeFormFailure);
				}
			}
			finally
			{
				SqlCon.Close();
			}

			return RetEntities;
		}

		

		#region Free / Delete

	
				/// <summary>
		/// Updates an Entity's State to "InUse"
		/// </summary>
		/// <param name="Entity">The entity to be updated in SetEntityStateCode</param>
		public void InUseEntity(CRMEntity Entity)
		{
			SetEntityStateCode(Entity, EntityStates.InUse);
		}



		public void FreeEntity(CRMEntity Entity)
		{
			SetEntityStateCode(Entity, EntityStates.Free);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Entity">The entity whose state will be updated</param>
		/// <param name="State">The state the entity is set to</param>
		/// <param name="cmd">The sql command to use. If this command 
		/// is part of a transaction, it will already have the correct parameters,
		/// we let the caller take care of the connection.</param>
		private void SetEntityStateCode(CRMEntity Entity, EntityStates State, SqlCommand cmd = null)
		{
			int rowCnt = 0;
			bool standaloneCommand = false;
			if (cmd == null)
			{
				standaloneCommand = true;
				cmd = new SqlCommand();            
				cmd.Connection = new SqlConnection(ConfigSettings.Default.EMSQLCNN);
				cmd.Connection.Open();
			}
			cmd.CommandText = "UPDATE " + Entity["type"] + " SET " + EntityManagerStateCode + "=" + (int)State;            
			if (Entity[EntityManagerID] != null)
			{
				cmd.CommandText += " WHERE " + EntityManagerID + "=" + Entity[EntityManagerID];
			}
			else
			{
				cmd.CommandText += " WHERE ExchangeId ='" + Entity["exchangeid"] + "'";
			}

			try
			{
				rowCnt = cmd.ExecuteNonQuery();
				//why does this check for 2 Exchange entities are returning 1 but being updated. Please review
				if (!Entity["type"].ToString().Contains("new_customaccount")
					&& !Entity["type"].ToString().Contains("new_customopportunity")
					&& !Entity["type"].ToString().Contains("Exchange")
					&& !Entity["type"].ToString().Contains("Task")
					&& !Entity["type"].ToString().Contains("QueueItem")
					&& !Entity["type"].ToString().Contains("Incident")
					&& !Entity["type"].ToString().Contains("msdyn_projectteam")
					&& !Entity["type"].ToString().Contains("msdyn_projecttask")
					&& !Entity["type"].ToString().Contains("msdyn_resourcerequirement")
					&& !Entity["type"].ToString().Contains("bookableresourcebooking")

					&& rowCnt != 2)
				{
					Trace.WriteLine("Row Count Error: " + rowCnt);
					throw new Exception("ExecuteNonQuery Wrong # of rows " + cmd.CommandText);
				}
			}
			catch (SqlException e)
			{
				FileWriter.Instance.WriteToFile(string.Format("Error when setting entity state code - {0}", cmd.CommandText));
				FileWriter.Instance.WriteToFile(string.Format("Error message: {0}", e.ToString()));
				throw;
			}
			catch (Exception ex)
			{
				FileWriter.Instance.WriteToFile(string.Format("Error when setting entity state code - {0}", cmd.CommandText));
				FileWriter.Instance.WriteToFile(string.Format("Error message: {0}", ex.ToString()));
				throw ex;
			}
			finally
			{
				if (standaloneCommand)
				{ 
					cmd.Connection.Close();
				}
			}
		}

		public static IUserManager CreateUserManagerInstance(string _emdbSqlConnStr)
		{
			if (string.IsNullOrEmpty(ConfigSettings.Default.UserManagerClass))
			{
				return new DefaultUserManager();
			}

			var parts = ConfigSettings.Default.UserManagerClass.Split(new[] { ',' });
			var assembly = parts[0].Trim();
			var typeName = parts[1].Trim();
			Assembly asm;
			try
			{
				asm = Assembly.LoadFile(assembly);
				//To load Microsoft.IdentityModel.Clients.ActiveDirectory assebmly for CRMOnline
				var authenticationParameters = new AuthenticationParameters();
			}
			catch (Exception ex)
			{
				var msg = string.Format(@"UserManager library {0} failed to be loaded. Please double check it is placed in {1}", parts[0], parts[1]);
				Trace.WriteLine(msg);
				throw new Exception(msg, ex);
			}

			var type = asm.GetType(typeName);

			object obj;
			try
			{
				obj = Activator.CreateInstance(type);
			}
			catch (Exception ex)
			{
				var msg = string.Format(@"Could not instantiate type {0} from assembly {1}", typeName, assembly);
				Trace.WriteLine(msg);
				throw new Exception(msg, ex);
			}

			if (!(obj is IUserManager))
			{
				var msg = string.Format(@"Type {0} from assembly {1} needs to implement {2}", typeName, assembly, typeof(IUserManager).ToString());
				Trace.WriteLine(msg);
				throw new Exception(msg);
			}

			return obj as IUserManager;
		}


		#endregion
	}

	public class EntityNotFoundException : System.Exception
	{
		public string EntityType { get; private set; }

		public EntityNotFoundException(string Message)
			: base(Message)
		{ }

		public EntityNotFoundException(string Message, string entityType)
			: this(Message)
		{
			this.EntityType = entityType;
		}
	}

	public enum EntityStates
	{
		Free = 0,
		InUse = 1,
		Deleted = 2
	}

	public class OrganizationInfo
	{
		public Guid OrganizationId { get; set; }
		public string ServerBaseUrl { get; set; }
		public string FeatureSet { get; set; }
		public bool ExchangeMode { get; set; }
		public EntityStates State { get; set; }
		public MultipleServersData ServerInfo { get; set; }
	}
}