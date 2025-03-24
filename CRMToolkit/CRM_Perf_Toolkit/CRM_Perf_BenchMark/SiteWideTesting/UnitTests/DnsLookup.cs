using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Xml;
using System.Data.SqlClient;

namespace CRM_Perf_BenchMark.Dbdns
{
	/// <summary>
	/// Summary description for DnsLookup
	/// </summary>
	[TestClass]
	public class DnsLookup
	{
		public DnsLookup()
		{
			XmlDocument xmlDoc = new XmlDocument();
			try
			{
				xmlDoc.Load("c:\\crmstress\\ConfigSettings.xml");
				XmlNode configsettings = xmlDoc.DocumentElement;


				XmlNode currentNode = configsettings.SelectSingleNode("sitewideconfig/dnslookup/details");

				if (currentNode != null)
				{
					if (currentNode.Attributes["domainname"] != null)
					{
						domainName = currentNode.Attributes["domainname"].Value;
					}
					if (currentNode.Attributes["configsqlcnn"] != null)
					{
						configDBCnn = currentNode.Attributes["configsqlcnn"].Value;
					}
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		[TestMethod]
		public void DNSLookupTest()
		{
			try
			{

				//we need to create dummy org name first, then add entry by calling spadddnsrecord
				string orgName = "RandomOrg" + Utils.GetRandomString(5, 10);
				string newOrgName = orgName + "." + domainName;
				string vip = "131.107.37.60";

				SqlConnection configDBCon = new SqlConnection(configDBCnn);
				configDBCon.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = configDBCon;
				cmd.CommandText = "exec dbo.spadddnsrecord N'" + newOrgName + "', N'" + vip + "', 3600, 1";
				cmd.ExecuteNonQuery();
				configDBCon.Close();

				IPHostEntry hostEntry = Dns.GetHostEntry(newOrgName);
				IPAddress address = hostEntry.AddressList[0];

				if (address.ToString() != vip)
				{
					throw (new Exception("Can not resolve the IP address for" + newOrgName));
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("DNSLookup:Failed to resolve");
				throw e;
			}
		}


		[TestMethod]
		public void DNSLookupFailureTest()
		{
			try
			{

				string orgName = "RandomOrg" + Utils.GetRandomString(5, 10);
				string newOrgName = orgName + "." + domainName;

				IPHostEntry hostEntry = Dns.GetHostEntry(newOrgName);

			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("DNSLookupFailure:nslookup returns " + e.ToString());
			}
		}


		private string configDBCnn;
		private string domainName;

	}
}
