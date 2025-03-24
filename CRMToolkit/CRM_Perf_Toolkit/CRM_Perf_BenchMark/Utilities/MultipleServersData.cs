using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRM_Perf_BenchMark
{
	public class MultipleServersData
	{
		private string m_orgName;
		private string m_serverBaseUrl;
		private string m_discoveryServer;
		private string m_SQLCNN;
		private string m_userpassword;
		private string m_userbase;
		private string m_start;
		private string m_count;
		private string m_domain;
		private int m_outlookpercentage;		
		private string m_organizationServiceUrl;
		private string m_organizationBaseUrl;
			

		public string OrgName
		{
			get
			{
				return m_orgName;
			}
			set
			{
				m_orgName = value;
			}
		}

		public string ServerBaseUrl
		{
			get
			{
				return m_serverBaseUrl;
			}
			set
			{
				m_serverBaseUrl = value;
			}
		}

		public string OrganizationBaseUrl
		{
			get
			{
				return m_organizationBaseUrl;
			}
			set
			{
				m_organizationBaseUrl = value;
			}
		}

		public string OrganizationServiceUrl
		{
			get
			{
				return m_organizationServiceUrl;
			}
			set
			{
				m_organizationServiceUrl = value;
			}
		}

		public string DiscoveryServer
		{
			get
			{
				return m_discoveryServer;
			}
			set
			{
				m_discoveryServer = value;
			}
		}

		public string SQLCNN
		{
			get
			{
				return m_SQLCNN;
			}
			set
			{
				m_SQLCNN = value;
			}
		}

		public string userPassword
		{
			get
			{
				return m_userpassword;
			}
			set
			{
				m_userpassword = value;
			}
		}

		public string userBase
		{
			get
			{
				return m_userbase;
			}
			set
			{
				m_userbase = value;
			}
		}

		public string userStart
		{
			get
			{
				return m_start;
			}
			set
			{
				m_start = value;
			}
		}

		public string userCount
		{
			get
			{
				return m_count;
			}
			set
			{
				m_count = value;
			}
		}

		public string userDomain
		{
			get 
			{ 
				return m_domain; 
			}
			set 
			{ 
				m_domain = value; 
			}
		}

		public int outlookpercentage
		{
			get
			{
				return m_outlookpercentage;
			}
			set
			{
				m_outlookpercentage = value;
			}
		}

	}
}
