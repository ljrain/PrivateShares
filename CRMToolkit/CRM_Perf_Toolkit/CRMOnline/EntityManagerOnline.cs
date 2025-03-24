using CRM_Perf_BenchMark;
using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.Net;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using CommonTypes;


namespace CRMOnline
{
    public class EntityManagerOnline : IUserManager
    {
        public string RetrieveAuthToken(WebTest webTest, string userName, string password, string orgName, string orgBaseUrl, string urlPath)
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) == 0)
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            }
            var oauthManager = new OAuthManager(orgBaseUrl, urlPath, userName, password);
            var bearerToken = oauthManager.Authenticate();
            return bearerToken;
        }

        public void SetAuthToken(HttpWebRequest webRequest, string userName, string password, string orgName, string orgBaseUrl, string urlPath)
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) == 0)
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            }
            var oauthManager = new OAuthManager(orgBaseUrl, urlPath, userName, password);
            var bearerToken = oauthManager.Authenticate();
            webRequest.Headers.Add(HttpRequestHeader.Authorization, bearerToken);
        }
    }
}
