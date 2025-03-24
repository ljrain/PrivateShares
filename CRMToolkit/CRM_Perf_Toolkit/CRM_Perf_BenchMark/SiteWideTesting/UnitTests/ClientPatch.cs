using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk;
using ServiceCreator;
using Microsoft.Xrm.Sdk.Client;

namespace CRM_Perf_BenchMark.Discovery
{
	/// <summary>
	/// Summary description for ClientPatch
	/// </summary>
	[TestClass]
	public class ClientPatch
	{
		public ClientPatch()
		{
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
		[TestInitialize()]
		public void MyTestInitialize()
		{
			user = EntityManager.Instance.GetNextUser();

			organizationId = new Guid(user["organizationid"]);
			userId = new Guid(user["systemuserid"]);

			fakeOrganizationId = Guid.NewGuid(); ;
			fakeUserId = Guid.NewGuid();

			serviceCreator = new XrmDiscoveryServiceCreator(string.Empty, user["passportname"], user["userpassword"], user["discoverserver"], AuthenticationProviderType.LiveId, false);
		}
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void RetrieveClientPatch()
		{
			// IDiscoveryService service1 = serviceCreator.DiscoveryService;
			//    try
			//    {
			//        RetrieveClientPatchesRequest PatchRequest = new RetrieveClientPatchesRequest();
			//        PatchRequest.ClientInfo = new ClientInfo();
			//        PatchRequest.ClientInfo.ClientType = ClientTypes.OutlookLaptop;
			//        PatchRequest.ClientInfo.OrganizationId = organizationId;
			//        PatchRequest.ClientInfo.UserId = userId;
			//        PatchRequest.ClientInfo.LanguageCode = 1033;
			//        PatchRequest.ClientInfo.PatchIds = new Guid[0];
			//        PatchRequest.ClientInfo.OSVersion = "5.2.3790.0";
			//        RetrieveClientPatchesResponse PatchResponse = (RetrieveClientPatchesResponse)service1.Execute(PatchRequest);
			//    }
			//    catch (Exception e)
			//    {
			//        System.Diagnostics.Trace.WriteLine("DiscoveryAuthentication:CRM Retrieve Client Patch Failed");
			//        throw e;
			//    }
			//}
		}

		[TestMethod]
		public void RetrieveClientPatchFailure()
		{
			//    IDiscoveryService service1 = serviceCreator.DiscoveryService;
			//    try
			//    {
			//        PatchRequest = new RetrieveClientPatchesRequest();
			//        PatchRequest.ClientInfo = new ClientInfo();
			//        PatchRequest.ClientInfo.ClientType = ClientTypes.OutlookLaptop;
			//        PatchRequest.ClientInfo.OrganizationId = fakeOrganizationId;
			//        PatchRequest.ClientInfo.UserId = fakeUserId;
			//        PatchRequest.ClientInfo.LanguageCode = 1033;
			//        PatchRequest.ClientInfo.PatchIds = new Guid[0];
			//        PatchRequest.ClientInfo.OSVersion = "5.2.3790.0";
			//    }
			//    catch (Exception e)
			//    {
			//        System.Diagnostics.Trace.WriteLine("DiscoveryAuthenticationFailure:CRM Retrieve Client Patch Failure Failed to create request");
			//        throw e;
			//    }
			//    try
			//    {
			//        RetrieveClientPatchesResponse PatchResponse = (RetrieveClientPatchesResponse)service1.Execute(PatchRequest);
			//    }
			//    catch (Exception e)
			//    {
			//        System.Diagnostics.Trace.WriteLine("DiscoveryAuthenticationFailure:CRM Retrieve Client Patch Failure returns " + e.ToString());
			//    }
			//}
		}


		private CRMEntity user;
		private Guid organizationId;
		private Guid userId;
		private Guid fakeOrganizationId;
		private Guid fakeUserId;
		private XrmDiscoveryServiceCreator serviceCreator;
	}
}
