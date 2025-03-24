using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.WebTesting;
using System.Net;


namespace CRM_Perf_BenchMark
{
	public interface IUserManager
	{
	    string RetrieveAuthToken(WebTest webTest, string userName, string password, string orgName, string orgBaseUrl, string urlPath);
		void SetAuthToken(HttpWebRequest webRequest, string userName, string password, string orgName, string orgBaseUrl, string urlPath);
	}
}
