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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRM_Perf_BenchMark.SiteWideTesting.UnitTests
{
	/// <summary>
	/// Summary description for InviteUser
	/// </summary>
	[TestClass]
	public class InviteUser
	{
		SiteWideUtils utils = null;
		public InviteUser()
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
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext) {}        
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		//[TestInitialize()]
		//public void MyTestInitialize() 
		//{
		//    // Get CRM Sdk Service Url for the Initial User for the named organization from Discovery Service
		//    utils.RetrieveOrgDetails();
		//    // Get the User Id for the Initial User for the named organization from Discover Service
		//    utils.RetrieveUserId();
		//}
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void InviteNewUser()
		{
			//
			// TODO: Add test logic	here
			//
			//MetadataCacheConfig.LoadMethod = LoadMethod.Database;

			Guid newUserId = Guid.NewGuid();
			string newUserEmail = Utils.GetRandomString(5, 10) + "@" + Utils.GetRandomString(5, 10) + ".com";
			Puid newUserPuid = Utils.GetRandomPuid();
			string auth = Microsoft.Crm.CrmUtility.FormatPuid(newUserPuid.Text);
			CrmLiveContext context = new CrmLiveContext("CreateUserInvitation");
			InvitationService invitationService = new InvitationService(context);

			//TODO: Ensure that provisioned orgs are the ones that the users are added.
			// or remove all dependency on the organization dbs and only touch config db.
			Microsoft.Crm.CrmLive.Services.InvitationToken invitationToken = invitationService.CreateInvitation(newUserId, newUserEmail, Utils.GetRandomString(5, 10), Utils.GetRandomString(5, 10), utils.InitialUserId, utils.InitialUserEmail, utils.InitialUserFirstName, utils.InitialUserLastName, utils.OrgId);
			invitationService.ValidateInvitation(auth, newUserEmail, invitationToken.ToString());
			invitationService.RespondToInvitation(auth, newUserEmail, invitationToken.ToString(), true);
		}
	}
}
