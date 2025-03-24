namespace CRM_Perf_BenchMark
{
	using System;
	using System.Collections.Generic;
	using Microsoft.VisualStudio.TestTools.WebTesting;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Collections;
	using System.Xml;

	/// <summary>
	/// The base class for Macro web tests. It has implementation for the followings
	/// - Retrieve test users and test entities from EMDB
	/// - Initialize the common test properties that will be used across all Macro web test cases
	/// - Common transactions which can be used for implementing various scenarios based on the needs of individual test case
	/// - AppGridWebService reset and refresh operation, which are used a lot for web service simulation
	/// 
	/// All Macro web tests should inherit from this base class.
	/// </summary>

	public abstract class WebTestBase : CrmWebTest
	{
		protected readonly string crmGrid = "crmGrid";
		protected CRMEntity user;
		protected CRMEntity user2;
		protected string tokenKey;
		protected WRPCInfo rpcInfo;
		protected WebTestRequestHeader headerItem;
		protected abstract string entityName { get; }
		protected abstract string siteMapPath { get; }
		protected readonly int thinkTime = 25;

        // Concrete classes can set this attribute, which will then be used for type code
        // instead of the static dictionary in WebTestHelp.cs
        protected int entityObjectTypeCode = 0;

		static WebTestBase()
		{
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            System.Threading.ThreadPool.SetMinThreads(100, 100);
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;

			WebRequest.RegisterPrefix("crm", new crmRequestFactory());
		}

		#region PreWebTest
		/// <summary>
		/// Pre-WebTest: retrieve test users and test entities from EMDB
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected Hashtable WebTestBase_PreWebTest(object sender, PreWebTestEventArgs e, EntityRequest[] subEntityRequest = null, string entityName = "")
		{
			Guid randomOrgId = Guid.Empty;
           
            randomOrgId = EntityManager.Instance.GetRandomOrg();           
            
			EntityRequest usersEntityRequest = new EntityRequest()
			{
				Type = EntityNames.Users,
				ReturnAs = EntityNames.Users,
				ParentID = randomOrgId
			};
            
			var orgInfo = EntityManager.Organizations.FirstOrDefault(o => o.OrganizationId.Equals(randomOrgId));
			if (usersEntityRequest.ParentID == Guid.Empty)
			{
				//no organization is free, stop the test
				FileWriter.Instance.WriteToFile(string.Format("[{0}]{1}: No Organization is free. Stopping test.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), this.ToString()));
				throw new Exception(string.Format("No organization is free for {0}", this.ToString()));
			}

			Hashtable userProps = new Hashtable();

			if (e.WebTest.Context.ContainsKey("UserRole"))
			{
				userProps.Add("role", e.WebTest.Context["UserRole"]);
			}
            if (e.WebTest.Context.ContainsKey("domainname"))
            {
                userProps.Add("domainname", e.WebTest.Context["domainname"]);
            }

			if (e.WebTest.Context.ContainsKey("systemuserid"))
			{
				userProps.Add("systemuserid", e.WebTest.Context["systemuserid"]);
			}

			if (e.WebTest.Context.ContainsKey("IsMoCAUser"))
			{
				userProps.Add("MoCAUser", 1);
			}
			if (e.WebTest.Context.ContainsKey("EnabledForACT"))
			{
				userProps.Add("EnabledForACT", 1);
			}

			usersEntityRequest.Props = userProps;

			if (subEntityRequest != null && !string.IsNullOrEmpty(entityName))
			{
				userProps.Add(entityName, subEntityRequest);
			}
			// Get a test user from EMDB
			Hashtable entities = null;
			entities = EntityManager.Instance.GetEntities(usersEntityRequest);

			user = entities[EntityNames.Users] as CRMEntity;

			if (e.WebTest.Name != "")
			{
				user["webtest"] = e.WebTest.Name;
			}
			this.tokenKey = this.SetAuthToken(user, e);

			if (ConfigSettings.Default.Trace)
			{
				FileWriter.Instance.WriteToFile("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff" + "]" + this.ToString() + ":" + user["domainname"] + " was selected"));
			}
			return entities;
		}

        protected string GetTokenKey()
        {
            return tokenKey;
        }
		#endregion

		#region PostWebTest
		/// <summary>
		/// Post-WebTest: unlock users that were locked during PreWebTest
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void WebTestBase_PostWebTest(object sender, PostWebTestEventArgs e)
		{
			if (null != user)
			{
				EntityManager.Instance.FreeEntity(user);
			}
			if (null != user2)
			{
				EntityManager.Instance.FreeEntity(user2);
			}
		}
		#endregion
						
    }
}

