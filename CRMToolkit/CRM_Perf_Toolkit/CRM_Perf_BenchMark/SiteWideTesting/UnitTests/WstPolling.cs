using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRM_Perf_BenchMark.SiteWideTesting.UnitTests
{
	public class DeploymentInfo
	{
		public string UID;
		public DateTime TicketIn;
		public DateTime FSSLatencyTicket;
		public DateTime DeleteTicket;
	}

	public class WstPollingContext
	{
		private static WstPollingContext m_Instance = new WstPollingContext();

		public string AssemblyName;
		public DeploymentInfo DepWstCfg;
		public DeploymentInfo DepSwCfg;
		public DeploymentInfo DepSgCfg;
		public DeploymentInfo DepSgStats;
		public DeploymentInfo DepSgOrg1;
		public DateTime MaxDeleteTicket;
		public DateTime EnvTicket;
		public DateTime EnvServerTicket;
		public DateTime EnvSelfAdminTicket;

		private WstPollingContext()
		{
			AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

			DepWstCfg = GetDeploymentInfo(SiteWideUtils.Instance.WebStoreWstCfgDepName);
			DepSwCfg = GetDeploymentInfo(SiteWideUtils.Instance.WebStoreSwCfgDepName);
			DepSgCfg = GetDeploymentInfo(SiteWideUtils.Instance.WebStoreSgCfgDepName);
			DepSgStats = GetDeploymentInfo(SiteWideUtils.Instance.WebStoreSgStatsDepName);
			DepSgOrg1 = GetDeploymentInfo(SiteWideUtils.Instance.WebStoreSgOrg1DepName);

			MaxDeleteTicket = DepSgCfg.DeleteTicket;
			if (MaxDeleteTicket < DepSgOrg1.DeleteTicket)
				MaxDeleteTicket = DepSgOrg1.DeleteTicket;
			if (MaxDeleteTicket < DepSgStats.DeleteTicket)
				MaxDeleteTicket = DepSgStats.DeleteTicket;
			if (MaxDeleteTicket < DepSwCfg.DeleteTicket)
				MaxDeleteTicket = DepSwCfg.DeleteTicket;
			if (MaxDeleteTicket < DepWstCfg.DeleteTicket)
				MaxDeleteTicket = DepWstCfg.DeleteTicket;

			LoadEnvTicketInfo();
		}

		public static WstPollingContext Instance
		{
			get { return m_Instance; }
		}

		private DeploymentInfo GetDeploymentInfo(string depName)
		{
			DeploymentInfo depInfo = new DeploymentInfo();
			SqlConnection wstConn = new SqlConnection(SiteWideUtils.Instance.WebStoreConfigCNN);
			wstConn.Open();
			SqlCommand cmdGetDep = wstConn.CreateCommand();
			cmdGetDep.CommandText = "SELECT DeploymentUID, CASE WHEN (MaxTicket >= MaxPartitionTicket) THEN MaxTicket ELSE MaxPartitionTicket END AS TicketIn, MaxFSSLatencyTicket, MaxDeleteTicket FROM dbo.Deployment WHERE DeploymentName = @depName";
			cmdGetDep.Parameters.Add(new SqlParameter("@depName", depName));
			using (SqlDataReader drDepResult = cmdGetDep.ExecuteReader())
			{
				drDepResult.Read();
				depInfo = new DeploymentInfo();
				depInfo.UID = drDepResult.GetGuid(0).ToString();
				depInfo.TicketIn = drDepResult.GetDateTime(1);
				depInfo.FSSLatencyTicket = drDepResult.GetDateTime(2);
				depInfo.DeleteTicket = drDepResult.GetDateTime(3);
			}
			wstConn.Close();
			return depInfo;
		}

		private void LoadEnvTicketInfo()
		{
			SqlConnection wstConn = new SqlConnection(SiteWideUtils.Instance.WebStoreConfigCNN);
			wstConn.Open();
			SqlCommand cmdGetEnv = wstConn.CreateCommand();
			cmdGetEnv.CommandText = "SELECT Ticket, SiteWideServerTicket, MaxSelfAdminInfoTicket FROM dbo.Environment";
			using (SqlDataReader drEnvResult = cmdGetEnv.ExecuteReader())
			{
				drEnvResult.Read();
				EnvTicket = drEnvResult.GetDateTime(0);
				EnvServerTicket = drEnvResult.GetDateTime(1);
				EnvSelfAdminTicket = drEnvResult.GetDateTime(2);
			}
			wstConn.Close();
		}
	}

	[TestClass]
	public class WstPolling
	{
		private SqlConnection m_WstConn;

		public WstPolling()
		{
			m_WstConn = new SqlConnection(SiteWideUtils.Instance.WebStoreConfigCNN);
		}

		//Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize()
		{
			m_WstConn.Open();
		}

		// Use TestCleanup to run code after each test has run
		[TestCleanup()]
		public void MyTestCleanup()
		{
			m_WstConn.Close();
		}

		[TestMethod()]
		public void WspGetDeploymentInfoByUid_V5_6_WstCfg()
		{
			SqlCommand cmdPollWstCfgDb = m_WstConn.CreateCommand();
			cmdPollWstCfgDb.CommandType = System.Data.CommandType.StoredProcedure;
			cmdPollWstCfgDb.CommandText = "dbo.WspGetDeploymentInfoByUid_V5_6";
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@DeploymentUIDIn", WstPollingContext.Instance.DepWstCfg.UID));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@TicketIn", WstPollingContext.Instance.DepWstCfg.TicketIn));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@FSSLatencyTicketIn", WstPollingContext.Instance.DepWstCfg.FSSLatencyTicket));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@SettingsTicketIn", WstPollingContext.Instance.DepWstCfg.TicketIn));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerRole", WstPollingContext.Instance.AssemblyName));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerName", Environment.MachineName));

			cmdPollWstCfgDb.ExecuteNonQuery();
		}

		[TestMethod()]
		public void WspGetDeploymentInfoByUid_V5_6_SwCfg()
		{
			SqlCommand cmdPollWstCfgDb = m_WstConn.CreateCommand();
			cmdPollWstCfgDb.CommandType = System.Data.CommandType.StoredProcedure;
			cmdPollWstCfgDb.CommandText = "dbo.WspGetDeploymentInfoByUid_V5_6";
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@DeploymentUIDIn", WstPollingContext.Instance.DepSwCfg.UID));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@TicketIn", WstPollingContext.Instance.DepSwCfg.TicketIn));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@FSSLatencyTicketIn", WstPollingContext.Instance.DepSwCfg.FSSLatencyTicket));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@SettingsTicketIn", ""));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerRole", WstPollingContext.Instance.AssemblyName));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerName", Environment.MachineName));

			cmdPollWstCfgDb.ExecuteNonQuery();
		}

		[TestMethod()]
		public void WspGetDeploymentInfoByUid_V5_6_SgCfg()
		{
			SqlCommand cmdPollWstCfgDb = m_WstConn.CreateCommand();
			cmdPollWstCfgDb.CommandType = System.Data.CommandType.StoredProcedure;
			cmdPollWstCfgDb.CommandText = "dbo.WspGetDeploymentInfoByUid_V5_6";
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@DeploymentUIDIn", WstPollingContext.Instance.DepSgCfg.UID));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@TicketIn", WstPollingContext.Instance.DepSgCfg.TicketIn));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@FSSLatencyTicketIn", WstPollingContext.Instance.DepSgCfg.FSSLatencyTicket));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@SettingsTicketIn", ""));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerRole", WstPollingContext.Instance.AssemblyName));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerName", Environment.MachineName));

			cmdPollWstCfgDb.ExecuteNonQuery();
		}

		[TestMethod()]
		public void WspGetDeploymentInfoByUid_V5_6_SgStats()
		{
			SqlCommand cmdPollWstCfgDb = m_WstConn.CreateCommand();
			cmdPollWstCfgDb.CommandType = System.Data.CommandType.StoredProcedure;
			cmdPollWstCfgDb.CommandText = "dbo.WspGetDeploymentInfoByUid_V5_6";
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@DeploymentUIDIn", WstPollingContext.Instance.DepSgStats.UID));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@TicketIn", WstPollingContext.Instance.DepSgStats.TicketIn));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@FSSLatencyTicketIn", WstPollingContext.Instance.DepSgStats.FSSLatencyTicket));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@SettingsTicketIn", ""));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerRole", WstPollingContext.Instance.AssemblyName));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerName", Environment.MachineName));

			cmdPollWstCfgDb.ExecuteNonQuery();
		}

		[TestMethod()]
		public void WspGetDeploymentInfoByUid_V5_6_SgOrg1()
		{
			SqlCommand cmdPollWstCfgDb = m_WstConn.CreateCommand();
			cmdPollWstCfgDb.CommandType = System.Data.CommandType.StoredProcedure;
			cmdPollWstCfgDb.CommandText = "dbo.WspGetDeploymentInfoByUid_V5_6";
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@DeploymentUIDIn", WstPollingContext.Instance.DepSgOrg1.UID));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@TicketIn", WstPollingContext.Instance.DepSgOrg1.TicketIn));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@FSSLatencyTicketIn", WstPollingContext.Instance.DepSgOrg1.FSSLatencyTicket));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@SettingsTicketIn", ""));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerRole", WstPollingContext.Instance.AssemblyName));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@ServerName", Environment.MachineName));

			cmdPollWstCfgDb.ExecuteNonQuery();
		}

		[TestMethod()]
		public void WspGetEnvironmentAndServerInfoV5_02()
		{
			SqlCommand cmdPollWstCfgDb = m_WstConn.CreateCommand();
			cmdPollWstCfgDb.CommandType = System.Data.CommandType.StoredProcedure;
			cmdPollWstCfgDb.CommandText = "dbo.WspGetEnvironmentAndServerInfoV5_02";
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@TicketInEnv", WstPollingContext.Instance.EnvTicket));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@TicketInServer", WstPollingContext.Instance.EnvServerTicket));
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@TicketInSelfAdminInfo", WstPollingContext.Instance.EnvSelfAdminTicket));

			cmdPollWstCfgDb.ExecuteNonQuery();
		}

		[TestMethod()]
		public void WspGetDeploymentsShallowInitInfo()
		{
			SqlCommand cmdPollWstCfgDb = m_WstConn.CreateCommand();
			cmdPollWstCfgDb.CommandType = System.Data.CommandType.StoredProcedure;
			cmdPollWstCfgDb.CommandText = "dbo.WspGetDeploymentsShallowInitInfo";
			cmdPollWstCfgDb.Parameters.Add(new SqlParameter("@TicketIn", WstPollingContext.Instance.MaxDeleteTicket));

			cmdPollWstCfgDb.ExecuteNonQuery();
		}
	}
}
