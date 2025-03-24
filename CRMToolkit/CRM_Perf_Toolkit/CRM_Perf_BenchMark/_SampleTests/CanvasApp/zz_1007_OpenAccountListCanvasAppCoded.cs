//THINGS TO MANUALLY UPDATE IN THE CONVERTED TEST.

//1.) Replace occurances of entity guid with appropriate entity Id variable.  
//This is necessary if referencing a specific entity in inserts, updates, lookups, etc. 
//Example: replace guid with variable code such as "+accountId+" 

//2.) Replace occurances of ownerid Guid with systemuserId variable in inlineeditwebservice.asmx when doing creates and updates.   
//Example: Replace Owner Guid "<ownerid name="First name Last name" type="8">{"+systemuserId+"}</ownerid>") 

//3.) Replace field values in crmFormSubmitXml with dynamic values when doing creates and updates.  
//Example: replace field value with Util function such as "+Utils.GetRandomString(5, 10)+" or "+Utils.GetRandomNumber(2, 5)+"  

//4.) Sample code to extract record guids from response
//entity":{ "Id":"{E7A6DA6C-3634-E811-814D-70106FA11B81};
//string valuemarker = "entity\":{\"Id\":\"{";
//int idIdx = requestN.lastResponse.BodyString.IndexOf(valuemarker);
//string tempNewId = requestN.lastResponse.BodyString.Substring(idIdx + (valuemarker.Length), 36); 

//5.) Add code to extract record from Views/Quickfinds.
//string resresult = requestN.lastResponse.BodyString;
//requestN = null;
//string contactId = Utils.extractgridresponse(resresult); 

//6.) Add code to extract record from LookupView. Update ETC passed into function
//string resresult = requestN.lastResponse.BodyString;
//requestN = null;
//string contactId = Utils.extractlookupresponse("2", resresult); 

//7.) UCI: Add code to extract record from UCI View/QuickFind. Update Code to reflect primarykey of entity
//string resresult = requestN.lastResponse.BodyString;
//requestN = null;
//string contactId = Utils.extractUCIgridresponse("contactid\":\"", resresult); 

//8.) UCI: Extract Id After Saving New Record
//string contactId = requestN.lastResponse.Headers["Location"].Split('(')[1].Substring(0, 36); 

//9.) Add code to extract WRPC Token and Timestamp for Inserts and Updates.
//Extract WRPC Token and Timestamp
//string[] TokenValues = Utils.ExtractWRPCToken(requestN.lastResponse);
//Token = TokenValues[0];
//Timestamp = TokenValues[1];
//Context.Add("Token", Token);
//Context.Add("TokenTimeStamp", Timestamp);

namespace CRM_Perf_BenchMark
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using System.Xml;
    using System.Net;
    using System.Data.SqlClient;
    using Microsoft.Identity.Client;

    public class zz_1007_OpenAccountListCanvasAppCoded : WebTestBase
    {
        private CRMEntity user;
        private string crmServer;
        private string Token;
        private string Timestamp;
        private string orgName;
        private string systemuserId;
        private Guid orgId;
        private string newId;
        private string newIdMarker = @"entity"":{""Id"":""{";

        public zz_1007_OpenAccountListCanvasAppCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1007_OpenAccountListCanvasAppCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1007_OpenAccountListCanvasAppCoded_PostWebTest);
        }
        public void zz_1007_OpenAccountListCanvasAppCoded_PreWebTest(object sender, PreWebTestEventArgs e)
        {
            Context.Add("domainname", "crmusr"+Utils.GetRandomNumber(1,5)+"@csabenchmark1122.onmicrosoft.com");
            user = WebTestBase_PreWebTest(sender, e)[EntityNames.Users] as CRMEntity;
            systemuserId = user["systemuserid"];

            crmServer = user["serverbaseurl"];
            e.WebTest.UserName = user["domainName"];
            e.WebTest.Context.Add("user", user["domainName"]);
            e.WebTest.Password = user["userpassword"];
            orgId = new Guid(user["organizationid"]);
            orgName = user["organizationname"];
               

            //Canvas App Authentication
            //bearer tokens are programatically generated for different canvas app components
            //TenantId must be updated
            string tenantid = "9a926422-5ce1-424f-bf67-ef768c96ae5b";

            Authenticator auth1 = new Authenticator();
            var securePass = new System.Security.SecureString();
            foreach (char c in Password.ToCharArray())
            {
                securePass.AppendChar(c);
            }
             
            List<string> apihubscopes = new List<string>();
            apihubscopes.Add("https://apihub.azure.com/.default");
            AuthenticationResult apihubresult = auth1.GetToken(tenantid, UserName, securePass, apihubscopes).GetAwaiter().GetResult();
            var apihubtoken = apihubresult.AccessToken;
            Context.Add("apihub_access_token", apihubtoken);

            List<string> powerappscopes = new List<string>();
            powerappscopes.Add("https://service.powerapps.com//.default");
            AuthenticationResult result = auth1.GetToken(tenantid, UserName, securePass, powerappscopes).GetAwaiter().GetResult();
            var powerapptoken = result.AccessToken;
            Context.Add("powerapps_access_token", powerapptoken);

            List<string> d365scopes = new List<string>();
            d365scopes.Add("https://"+ orgName +".crm.dynamics.com//.default");
            AuthenticationResult d365result = auth1.GetToken(tenantid, UserName, securePass, d365scopes).GetAwaiter().GetResult();
            var d365token = d365result.AccessToken;
            Context.Add("d365org_access_token", d365token);

            List<string> flowsscopes = new List<string>();
            flowsscopes.Add("https://service.flow.microsoft.com//.default");
            AuthenticationResult flowsresult = auth1.GetToken(tenantid, UserName, securePass, flowsscopes).GetAwaiter().GetResult();
            var flowstoken = flowsresult.AccessToken;
            Context.Add("flow_access_token", flowstoken);

            List<string> pbiscopes = new List<string>();
            pbiscopes.Add("https://analysis.windows.net/powerbi/api/.default");
            AuthenticationResult pbiresult = auth1.GetToken(tenantid, UserName, securePass, pbiscopes).GetAwaiter().GetResult();
            var pbitoken = pbiresult.AccessToken;
            Context.Add("pbi_access_token", pbitoken);

		    List<string> graphscopes = new List<string>();
            graphscopes.Add("https://graph.microsoft.com//.default");
            AuthenticationResult graphsresult = auth1.GetToken(tenantid, UserName, securePass, graphscopes).GetAwaiter().GetResult();
            var graphstoken = graphsresult.AccessToken;
            Context.Add("graph_access_token", graphstoken);

        }
        public void zz_1007_OpenAccountListCanvasAppCoded_PostWebTest(object sender, PostWebTestEventArgs e)
        {

            try
            {
                if (null != user)
                {
                    EntityManager.Instance.FreeEntity(user);
                }
                if (null != user2)
                {
                    EntityManager.Instance.FreeEntity(user2);
                }
                if (newId != null)
                {
                }
            }
            finally
            {
            }
        }
        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            this.Proxy = "default";
            this.BeginTransaction("1007.01_Open Account List Canvas App");
            this.AddCommentToResult("01_Open Account List Canvas App");

            CrmRequest request1 = new CrmRequest("https://apps.powerapps.com/play/e/8b57d1d3-7a5e-ef2b-b5ef-c31dc2dd0c10/a/a62ee0c4-bf8f-4118-8b04-56d9c1f63781");
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\""));
            request1.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request1.QueryStringParameters.Add("tenantId", "9a926422-5ce1-424f-bf67-ef768c96ae5b");            
            yield return request1;
            string resresult = request1.lastResponse.BodyString;
            request1 = null;
            string AppEnvironmentId = Utils.extractUCIgridresponse("AppEnvironmentId\":\"", resresult);
            this.AddCommentToResult("02_Open Account List Canvas App");

            CrmRequest request2 = new CrmRequest("https://unitedstates.api.powerapps.com/api/invoke");
            request2.Method = "POST";
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\""));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "webplayer-1665605751731"));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-path-query", "/healthcheck/compute"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "webplayer-preload-1665605751731"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps (Web; Player)"));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request2.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-site"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request2;
            request2 = null;
            this.AddCommentToResult("03_Open Account List Canvas App");

            CrmRequest request3 = new CrmRequest("https://apps.powerapps.com/auth/session");
            request3.Method = "POST";
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\""));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-correlation-request-id", "232971e5-73b8-4305-801d-56ec4f5a9851"));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request3.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=UTF-8"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "232971e5-73b8-4305-801d-56ec4f5a9851"));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request3.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request3Body = new StringHttpBody();
            request3Body.ContentType = "application/json; charset=UTF-8";
            request3Body.InsertByteOrderMark = false;
            request3Body.BodyString = "{\"appId\":\"/providers/Microsoft.PowerApps/apps/a62ee0c4-bf8f-4118-8b04-56d9c1f63781\",\"environmentName\":\"8b57d1d3-7a5e-ef2b-b5ef-c31dc2dd0c10\",\"fetchAppMetadata\":false}";

            request3.Body = request3Body;
            yield return request3;
            request3 = null;
            this.AddCommentToResult("04_Open Account List Canvas App");

            CrmRequest request4 = new CrmRequest("https://apps.powerapps.com/play/e/8b57d1d3-7a5e-ef2b-b5ef-c31dc2dd0c10/updateOrgSettings/a62ee0c4-bf8f-4118-8b04-56d9c1f63781");
            request4.Method = "POST";
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\""));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-correlation-request-id", "3b3306bd-7f92-4956-b673-7e6dea471fcb"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "3b3306bd-7f92-4956-b673-7e6dea471fcb"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request4.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request4;
            request4 = null;
            this.AddCommentToResult("05_Open Account List Canvas App");

            CrmRequest request5 = new CrmRequest("https://unitedstates.api.powerapps.com/api/invoke");
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\""));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-path-query", "/environments/8b57d1d3-7a5e-ef2b-b5ef-c31dc2dd0c10/apps/a62ee0c4-bf8f-4118-8b04-56d9c1f63781/appPlayNotifications?api-version=2020-06-01"));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request5.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-site"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request5;
            request5 = null;
            this.AddCommentToResult("06_Open Account List Canvas App");
        
            CrmRequest request6 = new CrmRequest("https://org5f110f1a.crm.dynamics.com/api/data/v9.0/$batch");
            request6.ThinkTime = 1;
            request6.Method = "POST";
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\""));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "94ef5aa3-09ed-49d6-af60-4bc5f3a23100"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "06d7b2c7-c77e-46fe-9eff-7462341b8cb6"));
            request6.Headers.Add(new WebTestRequestHeader("Request-Id", "06d7b2c7-c77e-46fe-9eff-7462341b8cb6"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request6.Headers.Add(new WebTestRequestHeader("Prefer", "odata.continue-on-error"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a62ee0c4-bf8f-4118-8b04-56d9c1f63781"));
            request6.Headers.Add(new WebTestRequestHeader("Client-Session-Id", "94ef5aa3-09ed-49d6-af60-4bc5f3a23100"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps/3.22101.14 (Web Player; AppName=a62ee0c4-bf8f-4118-8b04-56d9c1f63781)"));
            request6.Headers.Add(new WebTestRequestHeader("Content-Type", "multipart/mixed;boundary=batch_062dfcb2-f67a-46b1-8ad5-c69e09d6441a"));
            request6.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request6Body = new StringHttpBody();
            request6Body.ContentType = "multipart/mixed;boundary=batch_062dfcb2-f67a-46b1-8ad5-c69e09d6441a";
            request6Body.InsertByteOrderMark = false;
            request6Body.BodyString = "--batch_062dfcb2-f67a-46b1-8ad5-c69e09d6441a\nContent-Type: application/http\r\nContent-Transfer-Encoding: binary\r\n\nGET accounts?%24select=accountid%2Cname%2Ctelephone1%2Caddress1_city%2Cprimarycontactid%2C_primarycontactid_value HTTP/1.1\r\nAccept: application/json\r\nPrefer: odata.maxpagesize=500,odata.include-annotations=*\r\n\n--batch_062dfcb2-f67a-46b1-8ad5-c69e09d6441a--\r\n\0";
            request6.Body = request6Body;
            yield return request6;
            request6 = null;
            this.AddCommentToResult("07_Open Account List Canvas App");

            CrmRequest request7 = new CrmRequest("https://unitedstates.api.powerapps.com/api/invoke");
            request7.ThinkTime = 18;
            request7.Method = "POST";
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\""));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "94ef5aa3-09ed-49d6-af60-4bc5f3a23100"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-app-version", "2022-10-12T20:03:02Z"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-environment-id", "/providers/Microsoft.PowerApps/environments/8b57d1d3-7a5e-ef2b-b5ef-c31dc2dd0c10"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-app-id", "/providers/Microsoft.PowerApps/apps/a62ee0c4-bf8f-4118-8b04-56d9c1f63781"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-object-id", "07017ca3-0023-4ed0-a187-3df7b13549d3"));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-path-query", "/providers/Microsoft.PowerApps/apps/a62ee0c4-bf8f-4118-8b04-56d9c1f63781/launchApp?api-version=2020-06-01&%2524expand=consent%252Cenvironment&record=true&_blocking=false"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "ffcf62bd-c765-4580-a178-00371a3a3ee5"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps/3.22101.14 (Web; Player; isReactNative: false)"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-tenant-id", "9a926422-5ce1-424f-bf67-ef768c96ae5b"));
            request7.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-site"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request7;
            request7 = null;
           
            this.EndTransaction("1007.01_Open Account List Canvas App");
        }
        protected override string entityName
        {
            get
            {
                return EntityNames.Contacts;
            }
        }
        protected override string siteMapPath
        {
            get
            {
                return WebTestHelp.SalesSiteMapPath[EntityNames.Contacts];
            }
        }
    }
}
