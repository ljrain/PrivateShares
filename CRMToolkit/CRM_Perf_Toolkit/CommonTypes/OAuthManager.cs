using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.WebServiceClient;

namespace CommonTypes
{
    public class OAuthManager
    {
        private readonly string urlBase;
        private readonly string urlPath;
        private readonly string userName;
        private readonly string password;
        private readonly string appliationID = "ce9f9f18-dd0c-473e-b9b2-47812435e20d";
        private OrganizationWebProxyClient client;

        public OAuthManager(string urlBase, string urlPath, string userName, string password)
        {
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) == 0)
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            }            

            this.urlBase = urlBase;
            this.urlPath = urlPath;
            this.userName = userName;
            this.password = password;
        }

        public string Authenticate()
        {
            var baseUri = new Uri(this.urlBase);
            var serviceUri = new Uri(baseUri, urlPath);
            var resourceURL = new Uri(baseUri, "/api/data/v9.1/");
            this.client = new OrganizationWebProxyClient(serviceUri, false);
            string accessToken;
            try
            {
                if (userName.Contains("@"))
                {
                    //Generate authentication token for Standard Account
                    var authority = System.Threading.Tasks.Task.Run(async () => await AuthenticationParameters.CreateFromResourceUrlAsync(client.Endpoint.Address.Uri)).Result.Authority;
                    var authenticationContext = new AuthenticationContext(authority, false);
                    accessToken = authenticationContext.AcquireToken(this.urlBase, this.appliationID, new UserCredential(userName, password)).AccessToken;
                    this.client.HeaderToken = accessToken;
                   
                }
                else
                {
                    //Generate authentication token for Application User Account
                    ClientCredential clientcred = new ClientCredential(userName, password);
                    AuthenticationParameters authParam = AuthenticationParameters.CreateFromResourceUrlAsync(resourceURL).Result;
                    string authority = authParam.Authority.Replace("/oauth2/authorize", "");
                    AuthenticationResult authenticationResult = new AuthenticationContext(authority).AcquireTokenAsync(baseUri.ToString(), clientcred).Result;
                    accessToken = authenticationResult.AccessToken;
                    this.client.HeaderToken = authenticationResult.AccessToken;
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Failure in generating access token for org: {0} user: {1}", this.urlBase,
                    userName);
                throw new Exception(msg, ex);
                
            }
            var bearerToken = string.Format("Bearer {0}", accessToken);
            return bearerToken;
        }

        public OrganizationWebProxyClient GetServiceClient()
        {
            return this.client;
        }
    }
}
