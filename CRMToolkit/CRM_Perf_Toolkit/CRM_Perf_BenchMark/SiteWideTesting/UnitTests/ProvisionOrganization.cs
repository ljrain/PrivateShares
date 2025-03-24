using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Diagnostics;

using Microsoft.Crm;
using Microsoft.Crm.CrmLive.Provisioning;
using Microsoft.Crm.Admin.AdminService;
using Microsoft.Crm.Setup.Database;
//using Microsoft.Crm.Metadata;

using Microsoft.Crm.CrmLive;
using Microsoft.Crm.CrmLive.BillingService;
using Microsoft.Crm.CrmLive.Services;
using Microsoft.Crm.Passport.IdCrl;

using UserProperties = Microsoft.Crm.Tools.Admin.UserProperties;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRM_Perf_BenchMark.SiteWideTesting.UnitTests
{
	/// <summary>
	/// Summary description for ProvisionOrganization
	/// </summary>
	[TestClass]
	public sealed class ProvisionOrganization
	{
		SiteWideUtils utils = null;
		private string m_orgName = null;
		private string m_orgFriendlyName = null;
		private Guid m_orgId = Guid.Empty;

		public ProvisionOrganization()
		{
			//
			// TODO: Add constructor logic here
			//
			utils = SiteWideUtils.Instance;
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//

		//Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize()
		{
			m_orgName = "SiteWide" + Utils.GetRandomString(5, 10);
			m_orgFriendlyName = m_orgName + "FriendlyName";
		}

		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void ProvisionNewOrganization()
		{
			CreateOrgLifeCycle();
			ProvisionOrg();
		}

		public void CreateOrgLifeCycle()
		{
			CrmLiveContext context = new CrmLiveContext("CreateOrgLifeCycle");
			OrganizationLifecycleService orgLifeCycleService = new OrganizationLifecycleService(context);
			OrganizationInfo orgInfo = new OrganizationInfo();

			Microsoft.Crm.CrmLive.Puid m_InitialUserP = Puid.CreateTextPuid(utils.InitialUserPuid);
			orgInfo.InitialUserPuid = m_InitialUserP;
			orgInfo.InitialUserEmail = utils.InitialUserEmail;
			orgInfo.InitialUserFirstName = utils.InitialUserFirstName;
			orgInfo.InitialUserLastName = utils.InitialUserLastName;
			orgInfo.InitialUserLiveId = utils.InitialUserLiveId;
			orgInfo.OrganizationFriendlyName = m_orgFriendlyName;
			orgInfo.OrganizationUrl = "http://" + m_orgName + ".crmapp.local-titan.com";

			m_orgId = orgLifeCycleService.CreateOrganizationLifecycle(orgInfo, Guid.NewGuid(), Microsoft.Crm.Config.Wrapper.OrganizationLifecycleStatus.Provisioning);
		}

		public void ProvisionOrg()
		{
			//MetadataCacheConfig.LoadMethod = LoadMethod.Database;
			OrganizationProperties organizationProperties = new OrganizationProperties();
			organizationProperties.OrganizationData.Id = m_orgId;
			organizationProperties.OrganizationData.UniqueName = m_orgName;
			organizationProperties.OrganizationData.FriendlyName = m_orgFriendlyName;
			organizationProperties.OrganizationData.Type = utils.OrgType;
			organizationProperties.OrganizationData.ScaleGroupId = utils.ScaleGroupId;

			organizationProperties.OrganizationData.SqlServerName = utils.SqlServerName;
			organizationProperties.OrganizationData.SrsUrl = utils.ReportServerUrl;
			organizationProperties.OrganizationData.DatabaseName = utils.DatabaseName = utils.GenerateDatabaseName(m_orgName);

			organizationProperties.InitialUser = new UserProperties(utils.InitialUserDomainName, utils.InitialUserFirstName, utils.InitialUserLastName, utils.InitialUserEmail, utils.InitialUserPuid, utils.InitialUserLiveId);
			organizationProperties.InstallPath = utils.InstallPath;

			ProvisioningAction[] actions = 
			{
// This action removed from v4_live_main                new SetScaleGroupInfoAction(organizationProperties),
				new CreateOrganizationInConfigDBAction(organizationProperties),
				new CreateDnsRecordAction(organizationProperties),
				new DriverAddInitialUserToConfigDBAction(organizationProperties),
				//new AddInitialUserToConfigDBAction(organizationProperties), // this action depends on ConfigureOrganizationAction.
				new CreateMonitoringOrganizationInConfigDBAction(organizationProperties), // this action depends on AddInitialUserToConfigDBAction
				new DriverAddSupportUserAction(organizationProperties),
				//new AddSupportUserAction(organizationProperties),
				new ConfigureOrganizationFeaturesAction(organizationProperties), // TODO: This will need to move for Upgrades/Downgrades.
				new RegisterOrganizationWithWebStoreAction(organizationProperties),
				//new OrganizationDatabaseHardLimitAction(organizationProperties),
				//new EnableOrganizationAction(organizationProperties),
				new UpdateLifecycleAction(organizationProperties),
				// The org is open for business. So send the owner an email				
				new SendOrganizationReadyEmailAction(organizationProperties)
			};

			int attemptedActionIndex = 0;  // need to have it in scope in case we need to handle an exception and do an undo loop
			try
			{
				for (attemptedActionIndex = 0; attemptedActionIndex < actions.Length; attemptedActionIndex++)
				{
					actions[attemptedActionIndex].Do();
				}
			}
			catch (Exception exception)
			{
				string message = String.Format(
					CultureInfo.InvariantCulture,
					"Provisioning For Organization: {0} Failed with Exception: {1}",
					utils.OrgId, exception);

				System.Diagnostics.Debug.WriteLine(message);

				// Undo the actions that were done
				for (int j = attemptedActionIndex; j >= 0; j--)
				{
					// Ignore any exceptions hit during undo as all the actions are re-triable.
					// Otherwise, the original exception that triggered the provisioning failure
					// will get masked by exceptions hit during undo and makes troubleshooting harder.
					try
					{
						actions[j].Undo();
					}
					catch (Exception undoException)
					{
						string undoMessage = String.Format(
							CultureInfo.InvariantCulture,
							"Undo of Provisioning For Organization: {0} Failed with Exception: {1}",
							m_orgId, undoException);

						System.Diagnostics.Debug.WriteLine(undoMessage);
					}
				}
				throw (exception);
			}
			finally
			{
				// Clear up the Metadata Cache for all orgs (specified with Guid.Empty)
				// since we don't really intend to work with any org multiple times.
				// Otherwise, the memory consumption of the Provisioning service
				// will grow quite large as additional orgs are provisioned.
				//Microsoft.Crm.Metadata.MetadataCache.Flush(Guid.Empty);
			}

		}
	}
}
