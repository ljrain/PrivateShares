using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.Xrm.Sdk.Discovery;
using ServiceCreator;
using Microsoft.Xrm.Sdk.Client;

namespace CRM_Perf_BenchMark.Discovery
{
	/// <summary>
	/// Summary description for RetriveOrganization
	/// </summary>
	[TestClass]
	public class RetrieveOrganization
	{
		public RetrieveOrganization()
		{
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
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

			passportTicket = user["passportticket"];

			DateTime expiration = DateTime.Parse(user["passportticketexpiration"]);
			if (DateTime.Now > expiration)
			{
				//If passport ticket has expired, request for new crmticket, store in EMDB and retrieve it.
				passportTicket = EntityManager.Instance.UpdatePassportTicket(user);
			}

			username = user["passportname"];
			password = user["userpassword"];


			fakePassportTicket = Utils.GetRandomString(500, 700);

			fakeUsername = Utils.GetRandomString(5, 10) + "@hotmail.com";
			fakePassword = Utils.GetRandomString(5, 10);

			discoveryServiceCreator = new XrmDiscoveryServiceCreator(string.Empty, username, password, user["discoveryserver"], AuthenticationProviderType.LiveId, false);

			//service1.Url = string.Format("{0}/MSCRMServices/2009/Passport/CrmDiscoveryService.asmx", user["discoveryserver"]);                
		}
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void RetriveOrganizationInfo()
		{
			IDiscoveryService service1 = discoveryServiceCreator.DiscoveryService;
			try
			{
				RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();

				RetrieveOrganizationsResponse orgResponse = (RetrieveOrganizationsResponse)service1.Execute(orgRequest);
				string orgInfo = orgResponse.Details.ToString();
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("DiscoveryAuthentication:CRM Retrieve Organization Info Failed");
				throw e;
			}
		}

		[TestMethod]
		public void RetriveOrganizationInfoFailure()
		{
			IDiscoveryService service1 = discoveryServiceCreator.DiscoveryService;
			RetrieveOrganizationsRequest orgRequest;

			try
			{
				orgRequest = new RetrieveOrganizationsRequest();
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("RetrieveOrganizationFailure:CRM Retrieve Organization Info Failure Failed in creating request");
				throw e;
			}

			try
			{
				RetrieveOrganizationsResponse orgResponse = (RetrieveOrganizationsResponse)service1.Execute(orgRequest);

			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("RetrieveOrganizationFailure:CRM Retrieve Organization Info Failure returns " + e.ToString());
			}
		}


		private CRMEntity user;
		private string username;
		private string password;
		private string passportTicket;
		private string fakeUsername;
		private string fakePassword;
		private string fakePassportTicket;

		//private CrmDiscoveryService service1;
		private XrmDiscoveryServiceCreator discoveryServiceCreator;
	}
}
