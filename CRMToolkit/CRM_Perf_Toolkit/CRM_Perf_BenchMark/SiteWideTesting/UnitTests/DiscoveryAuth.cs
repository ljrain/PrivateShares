using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CrmBackCompat.Sdk2007.Discovery;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk;
using ServiceCreator;

namespace CRM_Perf_BenchMark
{
	/// <summary>
	/// Summary description for DiscoveryAuth
	/// </summary>
	[TestClass]
	public class DiscoveryAuth
	{
		public DiscoveryAuth()
		{
			service1 = new CrmDiscoveryService();
		}

		[TestInitialize()]
		public void Initialize()
		{
			user = EntityManager.Instance.GetNextUser();
			organizationName = user["organizationname"];
			passportTicket = user["passportticket"];

			username = user["passportname"];
			password = user["userpassword"];

			DateTime expiration = DateTime.Parse(user["passportticketexpiration"]);
			if (DateTime.Now > expiration)
			{
				//If  passport ticket has expired, request for new crmticket, store in EMDB and retrieve it.
				passportTicket = EntityManager.Instance.UpdatePassportTicket(user);
			}


			fakePassportTicket = Utils.GetRandomString(500, 700);
			fakeOrganizationName = Utils.GetRandomString(10, 20);

			service1.Url = string.Format("{0}/MSCRMServices/2009/Passport/CrmDiscoveryService.asmx", user["discoveryserver"]);
		}


		[TestMethod]
		public void DiscoveryAuthentication()
		{
			//retrieve policy
			try
			{
				RetrievePolicyRequest policyRequest = new RetrievePolicyRequest();
				RetrievePolicyResponse policyResponse = (RetrievePolicyResponse)service1.Execute(policyRequest);
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("DiscoveryAuthentication:CRM Retrieve Policy Failed");
				throw e;
			}

			//get authentication

			try
			{
				RetrieveCrmTicketRequest ticketRequest = new RetrieveCrmTicketRequest();
				ticketRequest.OrganizationName = organizationName;
				ticketRequest.PassportTicket = passportTicket;
				RetrieveCrmTicketResponse ticketResponse = (RetrieveCrmTicketResponse)service1.Execute(ticketRequest);
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("DiscoveryAuthentication:CRM Authentication Failed");
				throw e;
			}

		}

		[TestMethod]
		public void DiscoveryAuthenticationFailure()
		{
			RetrieveCrmTicketRequest ticketRequest;

			//retrieve policy
			try
			{
				RetrievePolicyRequest policyRequest = new RetrievePolicyRequest();
				RetrievePolicyResponse policyResponse = (RetrievePolicyResponse)service1.Execute(policyRequest);
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("DiscoveryAuthenticationFailure:CRM Retrieve Policy Failed");
				throw e;
			}

			//get authentication

			try
			{
				ticketRequest = new RetrieveCrmTicketRequest();
				ticketRequest.OrganizationName = fakeOrganizationName;
				ticketRequest.PassportTicket = fakePassportTicket;
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("DiscoveryAuthenticationFailure:CRM Authentication Failure Failed to generate request");
				throw e;
			}

			try
			{
				RetrieveCrmTicketResponse ticketResponse = (RetrieveCrmTicketResponse)service1.Execute(ticketRequest);
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("DiscoveryAuthenticationFailure:CRM Authentication Failure returnes " + e.ToString());
			}

		}


		private CRMEntity user;
		private string username;
		private string password;
		private string organizationName;
		private string passportTicket;
		private string fakeOrganizationName;
		private string fakePassportTicket;

		private CrmDiscoveryService service1;
	}
}
