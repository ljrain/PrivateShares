using Microsoft.VisualStudio.TestTools.WebTesting;
using System.Net;

namespace CRM_Perf_BenchMark
{
	class DefaultUserManager : IUserManager
	{
	    public string RetrieveAuthToken(WebTest webTest, string userName, string password, string orgName, string orgBaseUrl, string urlPath)
	    {
	        webTest.UserName = userName;
	        webTest.Password = password;
			return string.Empty;
		}

		public void SetAuthToken(HttpWebRequest webRequest, string userName, string password, string orgName, string orgBaseUrl, string urlPath)
		{
			webRequest.Credentials = new NetworkCredential(userName, password);
		}
	}
}
