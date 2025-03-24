using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace CRM_Perf_BenchMark.SiteWideTesting.UnitTests
{
	/// <summary>
	/// Summary description for DnsCacheLookup
	/// </summary>
	[TestClass]
	public class DnsCacheLookup
	{
		public DnsCacheLookup()
		{
			utils = SiteWideUtils.Instance;
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
			int max = utils.OrgList.Length;
			int index = Convert.ToInt32(Utils.GetRandomNumber(0, max - 1));
			orgName = utils.OrgList[index];
		}

		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void DnsCacheLookupTest()
		{

			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(orgName);
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("DNSCacheLookup:Failed to resolve");
				throw e;
			}
		}

		SiteWideUtils utils = null;
		private string orgName = null;
	}
}
