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
    public class zz_1006_ViewSalesPipelinePowerBIDashboardCoded : WebTestBase
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

        public zz_1006_ViewSalesPipelinePowerBIDashboardCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1006_ViewSalesPipelinePowerBIDashboardCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1006_ViewSalesPipelinePowerBIDashboardCoded_PostWebTest);
        }
        public void zz_1006_ViewSalesPipelinePowerBIDashboardCoded_PreWebTest(object sender, PreWebTestEventArgs e)
        {
            
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
                                   
            List<string> powerplatformscopes = new List<string>();
            powerplatformscopes.Add("https://api.powerplatform.com//.default");
            AuthenticationResult powerplatformresult = auth1.GetToken(tenantid, UserName, securePass, powerplatformscopes).GetAwaiter().GetResult();
            var powerplatformtoken = powerplatformresult.AccessToken;
            Context.Add("powerplatforms_access_token", powerplatformtoken);

            List<string> d365scopes = new List<string>();
            d365scopes.Add("https://" + orgName + ".crm.dynamics.com//.default");
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
        public void zz_1006_ViewSalesPipelinePowerBIDashboardCoded_PostWebTest(object sender, PostWebTestEventArgs e)
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
            this.BeginTransaction("1006.01_Open Sales Pipeline");
            this.AddCommentToResult("01_Open Sales Pipeline");

            CrmRequest request1 = new CrmRequest("https://apps.powerapps.com/fpi/powerBIRuntime");
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request1.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "iframe"));
            request1.QueryStringParameters.Add("powerBIUrl", @"https://app.powerbi.com/dashboardEmbed?dashboardId=659e1554-2edd-4d2e-8f39-4abdfa252272&appId=211de0d8-1954-40f9-8562-56c1b4b85767&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVVTLUVBU1QyLUItUFJJTUFSWS1yZWRpcmVjdC5hbmFseXNpcy53aW5kb3dzLm5ldCIsImVtYmVkRmVhdHVyZXMiOnsibW9kZXJuRW1iZWQiOmZhbHNlfX0%3d");
            request1.QueryStringParameters.Add("loginLabel", "Sign in");
            request1.QueryStringParameters.Add("componentId", "929211");
            request1.QueryStringParameters.Add("direction", "ltr");
            request1.QueryStringParameters.Add("env", "prod");
            request1.QueryStringParameters.Add("loginText", "Authentication wasn’t successful. You need to sign in to your Power BI account to view or edit Power BI tiles. ");
            request1.QueryStringParameters.Add("client", "web");
            request1.QueryStringParameters.Add("mode", "dashboard");
            request1.QueryStringParameters.Add("cookieName", "powerbi");
            request1.QueryStringParameters.Add("altText", "Power BI");
            request1.QueryStringParameters.Add("forcemobile", "false");
            request1.QueryStringParameters.Add("powerbiJSEnabled", "true");
            request1.QueryStringParameters.Add("background", "0");
            request1.QueryStringParameters.Add("tenant", "9a926422-5ce1-424f-bf67-ef768c96ae5b");
            request1.QueryStringParameters.Add("usePowerAppsAuthFlow", "false");
            yield return request1;
            request1 = null;
            this.AddCommentToResult("04_Open Sales Pipeline");

            CrmRequest request2 = new CrmRequest("https://app.powerbi.com/dashboardEmbed");
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request2.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "iframe"));
            request2.QueryStringParameters.Add("uid", "p9ezt7");
            yield return request2;
            request2 = null;
            this.AddCommentToResult("05_Open Sales Pipeline");

            CrmRequest request3 = new CrmRequest("https://api.powerbi.com/metadata/cluster");
            request3.Method = "OPTIONS";
            request3.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "GET"));
            request3.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "activityid,authorization,requestid,x-powerbi-hostenv"));
            request3.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-site"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request3;
            request3 = null;
            this.AddCommentToResult("06_Open Sales Pipeline");

            CrmRequest request4 = new CrmRequest("https://api.powerbi.com/metadata/cluster");
            request4.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your customers"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request4.Headers.Add(new WebTestRequestHeader("RequestId", "6f03457a-a5be-4368-bd92-19bd2c890414"));
            request4.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request4.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-site"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request4;
            request4 = null;
            this.AddCommentToResult("07_Open Sales Pipeline");

            CrmRequest request5 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net//metadata/embed/dashboard/659e1554-2edd-4d2e-8f39-4abdfa252272");
            request5.Method = "OPTIONS";
            request5.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "GET"));
            request5.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "activityid,authorization,requestid,x-powerbi-hostenv"));
            request5.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request5;
            request5 = null;
            this.AddCommentToResult("08_Open Sales Pipeline");

            CrmRequest request6 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net//metadata/embed/dashboard/659e1554-2edd-4d2e-8f39-4abdfa252272");
            request6.ThinkTime = 1;
            request6.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your customers"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request6.Headers.Add(new WebTestRequestHeader("RequestId", "1581bdef-da12-42c8-99c3-40c2a72771fa"));
            request6.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request6.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request6;
            request6 = null;
            this.AddCommentToResult("09_Open Sales Pipeline");

            CrmRequest request7 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/powerbi/metadata/dashboards/491014");
            request7.Method = "OPTIONS";
            request7.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "GET"));
            request7.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "activityid,authorization,requestid,x-powerbi-hostenv"));
            request7.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request7.QueryStringParameters.Add("preferReadOnlySession", "true");
            yield return request7;
            request7 = null;
            this.AddCommentToResult("10_Open Sales Pipeline");


            CrmRequest request8 = new CrmRequest("https://wabi-singapore-a-primary-redirect.analysis.windows.net/explore/reports/659e1554-2edd-4d2e-8f39-4abdfa252272/modelsAndExploration");
            request8.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Simplified Embedding"));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Microsoft Edge\";v=\"113\", \"Chromium\";v=\"113\", \"Not-A.Brand\";v=\"24\""));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request8.Headers.Add(new WebTestRequestHeader("RequestId", "1637d25c-2f30-2a91-4ca6-6c8e32ab0bc4"));
            request8.Headers.Add(new WebTestRequestHeader("ActivityId", "227b10ef-c5c8-6d53-8a1e-23460e75434f"));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request8.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request8.QueryStringParameters.Add("preferReadOnlySession", "true");
            ExtractText lr2er4 = new ExtractText();
            lr2er4.StartsWith = "mwcToken\":\"";
            lr2er4.EndsWith = "\"";
            lr2er4.IgnoreCase = false;
            lr2er4.UseRegularExpression = false;
            lr2er4.Required = true;
            lr2er4.ExtractRandomMatch = false;
            lr2er4.Index = 0;
            lr2er4.HtmlDecode = true;
            lr2er4.SearchInHeaders = false;
            lr2er4.ContextParameterName = "pbidedicated_access_token";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(lr2er4.Extract);
            yield return request8;
            request8 = null;
            this.AddCommentToResult("10_Open Sales Pipeline");


            CrmRequest request9 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/metadata/informationProtection/artifacts");
            request9.Method = "OPTIONS";
            request9.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "POST"));
            request9.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "activityid,authorization,content-type,requestid,x-powerbi-hostenv"));
            request9.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request9;
            request9 = null;
            this.AddCommentToResult("12_Open Sales Pipeline");

            CrmRequest request10 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/metadata/informationProtection/artifacts");
            request10.Method = "POST";
            request10.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request10.Headers.Add(new WebTestRequestHeader("RequestId", "afb51b25-36a9-08a5-ab39-84acaa3f07ce"));
            request10.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request10.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json;charset=UTF-8"));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request10.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json;charset=UTF-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = "{\"dashboards\":[{\"artifactId\":\"659e1554-2edd-4d2e-8f39-4abdfa252272\"}],\"reports\":[{\"artifactId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\"}],\"models\":[{\"artifactId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\"}],\"artifacts\":[],\"flowType\":6,\"workloadType\":1}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;
            this.AddCommentToResult("13_Open Sales Pipeline");

            CrmRequest request11 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/reports/2298833/exploration/sections/40135014");
            request11.Method = "OPTIONS";
            request11.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "GET"));
            request11.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "activityid,authorization,requestid,x-powerbi-hostenv"));
            request11.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request11;
            request11 = null;
            this.AddCommentToResult("14_Open Sales Pipeline");

            CrmRequest request12 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/reports/2298833/exploration/sections/40135014");
            request12.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request12.Headers.Add(new WebTestRequestHeader("RequestId", "c52af06d-d24b-052d-0524-0698920917cf"));
            request12.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request12.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request12;
            request12 = null;
            this.AddCommentToResult("15_Open Sales Pipeline");

            CrmRequest request13 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/conceptualschema");
            request13.Method = "OPTIONS";
            request13.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "POST"));
            request13.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "activityid,authorization,content-type,requestid,x-powerbi-hostenv"));
            request13.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request13;
            request13 = null;
            this.AddCommentToResult("16_Open Sales Pipeline");

            CrmRequest request14 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/conceptualschema");
            request14.Method = "POST";
            request14.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request14.Headers.Add(new WebTestRequestHeader("RequestId", "fd7d8954-0840-03f2-d850-5912f95f754c"));
            request14.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request14.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json;charset=UTF-8"));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request14.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request14Body = new StringHttpBody();
            request14Body.ContentType = "application/json;charset=UTF-8";
            request14Body.InsertByteOrderMark = false;
            request14Body.BodyString = "{\"modelIds\":[1922721],\"userPreferredLocale\":\"en-US\"}";
            request14.Body = request14Body;
            yield return request14;
            request14 = null;
            this.AddCommentToResult("17_Open Sales Pipeline");

            CrmRequest request15 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/powerbi/metadata/userSettings");
            request15.Method = "OPTIONS";
            request15.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "GET"));
            request15.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "activityid,authorization,requestid,x-powerbi-hostenv"));
            request15.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request15;
            request15 = null;
            this.AddCommentToResult("18_Open Sales Pipeline");

            CrmRequest request16 = new CrmRequest("https://appsource.powerbi.com/visuals.json");
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request16.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-site"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request16;
            request16 = null;
            this.AddCommentToResult("19_Open Sales Pipeline");

            CrmRequest request17 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/powerbi/metadata/userSettings");
            request17.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request17.Headers.Add(new WebTestRequestHeader("RequestId", "27831f8d-ac6c-f65b-73ca-1649ceabab03"));
            request17.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request17.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request17;
            request17 = null;
            this.AddCommentToResult("20_Open Sales Pipeline");

            CrmRequest request18 = new CrmRequest("https://app.powerbi.com/undefined");
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "no-cors"));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "image"));
            yield return request18;
            request18 = null;
            this.AddCommentToResult("21_Open Sales Pipeline");

            CrmRequest request19 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/powerbi/metadata/refreshusermetadata");
            request19.Method = "OPTIONS";
            request19.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "PUT"));
            request19.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "activityid,authorization,content-type,requestid,x-powerbi-hostenv"));
            request19.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request19;
            request19 = null;
            this.AddCommentToResult("22_Open Sales Pipeline");

            CrmRequest request20 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/powerbi/metadata/userSettings");
            request20.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request20.Headers.Add(new WebTestRequestHeader("RequestId", "39f10d69-0931-0470-2bf3-b5761aa0e724"));
            request20.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request20.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request20;
            request20 = null;
            this.AddCommentToResult("23_Open Sales Pipeline");

            CrmRequest request21 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/powerbi/metadata/refreshusermetadata");
            request21.ThinkTime = -1;
            request21.Method = "PUT";
            request21.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request21.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request21.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request21.Headers.Add(new WebTestRequestHeader("RequestId", "e1d39891-c2d4-cdbe-b209-c68aaae8fc85"));
            request21.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request21.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json;charset=UTF-8"));
            request21.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request21.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request21.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request21.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request21.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request21;
            request21 = null;
            this.AddCommentToResult("24_Open Sales Pipeline");

            CrmRequest request22 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/querydata");
            request22.Method = "OPTIONS";
            request22.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "POST"));
            request22.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "activityid,authorization,content-type,requestid,x-powerbi-hostenv"));
            request22.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request22.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request22.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request22.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request22;
            request22 = null;
            this.AddCommentToResult("25_Open Sales Pipeline");

            CrmRequest request23 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/querydata");
            request23.Method = "POST";
            request23.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request23.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request23.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request23.Headers.Add(new WebTestRequestHeader("RequestId", "135e9657-9c0c-5b69-e494-9ed632fc1fdf"));
            request23.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request23.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json;charset=UTF-8"));
            request23.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request23.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request23Body = new StringHttpBody();
            request23Body.ContentType = "application/json;charset=UTF-8";
            request23Body.InsertByteOrderMark = false;
            request23Body.BodyString = "{\"version\":\"1.0.0\",\"queries\":[{\"Query\":{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"o\",\"Entity\":\"Opportunity\",\"Type\":0}],\"Select\":[{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Pipeline Phase\"},\"Name\":\"Opportunity.Pipeline Phase\"},{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Revenue\"},\"Name\":\"Opportunity.Open Revenue\"}],\"Where\":[{\"Condition\":{\"In\":{\"Expressions\":[{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Status\"}}],\"Values\":[[{\"Literal\":{\"Value\":\"\'Open\'\"}}]]}}}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0,1]}]},\"DataReduction\":{\"DataVolume\":3,\"Primary\":{\"Top\":{}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]},\"CacheKey\":\"{\\\"Commands\\\":[{\\\"SemanticQueryDataShapeCommand\\\":{\\\"Query\\\":{\\\"Version\\\":2,\\\"From\\\":[{\\\"Name\\\":\\\"o\\\",\\\"Entity\\\":\\\"Opportunity\\\",\\\"Type\\\":0}],\\\"Select\\\":[{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Pipeline Phase\\\"},\\\"Name\\\":\\\"Opportunity.Pipeline Phase\\\"},{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Revenue\\\"},\\\"Name\\\":\\\"Opportunity.Open Revenue\\\"}],\\\"Where\\\":[{\\\"Condition\\\":{\\\"In\\\":{\\\"Expressions\\\":[{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Status\\\"}}],\\\"Values\\\":[[{\\\"Literal\\\":{\\\"Value\\\":\\\"\'Open\'\\\"}}]]}}}]},\\\"Binding\\\":{\\\"Primary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[0,1]}]},\\\"DataReduction\\\":{\\\"DataVolume\\\":3,\\\"Primary\\\":{\\\"Top\\\":{}}},\\\"Version\\\":1},\\\"ExecutionMetricsKind\\\":1}}]}\",\"QueryId\":\"\",\"ApplicationContext\":{\"DatasetId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\",\"Sources\":[{\"ReportId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\",\"VisualId\":\"VisualContainer\"}]}}],\"cancelQueries\":[],\"modelId\":1922721,\"userPreferredLocale\":\"en-US\"}";
            request23.Body = request23Body;
            yield return request23;
            request23 = null;
            this.AddCommentToResult("26_Open Sales Pipeline");

            CrmRequest request24 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/querydata");
            request24.Method = "POST";
            request24.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request24.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request24.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request24.Headers.Add(new WebTestRequestHeader("RequestId", "5c20060a-42ab-32b8-492c-904ac82ba6b2"));
            request24.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request24.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json;charset=UTF-8"));
            request24.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request24.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request24.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request24.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request24.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request24Body = new StringHttpBody();
            request24Body.ContentType = "application/json;charset=UTF-8";
            request24Body.InsertByteOrderMark = false;
            request24Body.BodyString = "{\"version\":\"1.0.0\",\"queries\":[{\"Query\":{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"o\",\"Entity\":\"Opportunity\",\"Type\":0}],\"Select\":[{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Won Revenue\"},\"Name\":\"Opportunity.Won Revenue\"}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0]}]},\"DataReduction\":{\"DataVolume\":3,\"Primary\":{\"Top\":{}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]},\"CacheKey\":\"{\\\"Commands\\\":[{\\\"SemanticQueryDataShapeCommand\\\":{\\\"Query\\\":{\\\"Version\\\":2,\\\"From\\\":[{\\\"Name\\\":\\\"o\\\",\\\"Entity\\\":\\\"Opportunity\\\",\\\"Type\\\":0}],\\\"Select\\\":[{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Won Revenue\\\"},\\\"Name\\\":\\\"Opportunity.Won Revenue\\\"}]},\\\"Binding\\\":{\\\"Primary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[0]}]},\\\"DataReduction\\\":{\\\"DataVolume\\\":3,\\\"Primary\\\":{\\\"Top\\\":{}}},\\\"Version\\\":1},\\\"ExecutionMetricsKind\\\":1}}]}\",\"QueryId\":\"\",\"ApplicationContext\":{\"DatasetId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\",\"Sources\":[{\"ReportId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\",\"VisualId\":\"VisualContainer8\"}]}},{\"Query\":{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"o\",\"Entity\":\"Opportunity\",\"Type\":0}],\"Select\":[{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Win Rate - Closed\"},\"Name\":\"Opportunity.Win Rate - Closed\"}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0]}]},\"DataReduction\":{\"DataVolume\":3,\"Primary\":{\"Top\":{}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]},\"CacheKey\":\"{\\\"Commands\\\":[{\\\"SemanticQueryDataShapeCommand\\\":{\\\"Query\\\":{\\\"Version\\\":2,\\\"From\\\":[{\\\"Name\\\":\\\"o\\\",\\\"Entity\\\":\\\"Opportunity\\\",\\\"Type\\\":0}],\\\"Select\\\":[{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Win Rate - Closed\\\"},\\\"Name\\\":\\\"Opportunity.Win Rate - Closed\\\"}]},\\\"Binding\\\":{\\\"Primary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[0]}]},\\\"DataReduction\\\":{\\\"DataVolume\\\":3,\\\"Primary\\\":{\\\"Top\\\":{}}},\\\"Version\\\":1},\\\"ExecutionMetricsKind\\\":1}}]}\",\"QueryId\":\"\",\"ApplicationContext\":{\"DatasetId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\",\"Sources\":[{\"ReportId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\",\"VisualId\":\"VisualContainer9\"}]}}],\"cancelQueries\":[],\"modelId\":1922721,\"userPreferredLocale\":\"en-US\"}";
            request24.Body = request24Body;
            yield return request24;
            request24 = null;
            this.AddCommentToResult("27_Open Sales Pipeline");

            CrmRequest request25 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/querydata");
            request25.Method = "POST";
            request25.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request25.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request25.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request25.Headers.Add(new WebTestRequestHeader("RequestId", "41a3d028-2e67-4c50-6b41-ec347e3eafde"));
            request25.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request25.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json;charset=UTF-8"));
            request25.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request25.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request25.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request25.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request25.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request25Body = new StringHttpBody();
            request25Body.ContentType = "application/json;charset=UTF-8";
            request25Body.InsertByteOrderMark = false;
            request25Body.BodyString = "{\"version\":\"1.0.0\",\"queries\":[{\"Query\":{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"a\",\"Entity\":\"Account\",\"Type\":0},{\"Name\":\"o\",\"Entity\":\"Opportunity\",\"Type\":0}],\"Select\":[{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"a\"}},\"Property\":\"Industry\"},\"Name\":\"Account.Industry\"},{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Revenue\"},\"Name\":\"Opportunity.Open Revenue\"}],\"OrderBy\":[{\"Direction\":2,\"Expression\":{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Revenue\"}}}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0,1]}]},\"DataReduction\":{\"DataVolume\":4,\"Primary\":{\"Window\":{\"Count\":200}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]},\"CacheKey\":\"{\\\"Commands\\\":[{\\\"SemanticQueryDataShapeCommand\\\":{\\\"Query\\\":{\\\"Version\\\":2,\\\"From\\\":[{\\\"Name\\\":\\\"a\\\",\\\"Entity\\\":\\\"Account\\\",\\\"Type\\\":0},{\\\"Name\\\":\\\"o\\\",\\\"Entity\\\":\\\"Opportunity\\\",\\\"Type\\\":0}],\\\"Select\\\":[{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"a\\\"}},\\\"Property\\\":\\\"Industry\\\"},\\\"Name\\\":\\\"Account.Industry\\\"},{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Revenue\\\"},\\\"Name\\\":\\\"Opportunity.Open Revenue\\\"}],\\\"OrderBy\\\":[{\\\"Direction\\\":2,\\\"Expression\\\":{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Revenue\\\"}}}]},\\\"Binding\\\":{\\\"Primary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[0,1]}]},\\\"DataReduction\\\":{\\\"DataVolume\\\":4,\\\"Primary\\\":{\\\"Window\\\":{\\\"Count\\\":200}}},\\\"Version\\\":1},\\\"ExecutionMetricsKind\\\":1}}]}\",\"QueryId\":\"\",\"ApplicationContext\":{\"DatasetId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\",\"Sources\":[{\"ReportId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\",\"VisualId\":\"VisualContainer6\"}]}},{\"Query\":{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"o\",\"Entity\":\"Opportunity\",\"Type\":0}],\"Select\":[{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Rating\"},\"Name\":\"Opportunity.Rating\"},{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Revenue\"},\"Name\":\"Opportunity.Open Revenue\"}],\"Where\":[{\"Condition\":{\"In\":{\"Expressions\":[{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Status\"}}],\"Values\":[[{\"Literal\":{\"Value\":\"\'Open\'\"}}]]}}}],\"OrderBy\":[{\"Direction\":2,\"Expression\":{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Revenue\"}}}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0,1]}]},\"DataReduction\":{\"DataVolume\":4,\"Primary\":{\"Window\":{\"Count\":200}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]},\"CacheKey\":\"{\\\"Commands\\\":[{\\\"SemanticQueryDataShapeCommand\\\":{\\\"Query\\\":{\\\"Version\\\":2,\\\"From\\\":[{\\\"Name\\\":\\\"o\\\",\\\"Entity\\\":\\\"Opportunity\\\",\\\"Type\\\":0}],\\\"Select\\\":[{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Rating\\\"},\\\"Name\\\":\\\"Opportunity.Rating\\\"},{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Revenue\\\"},\\\"Name\\\":\\\"Opportunity.Open Revenue\\\"}],\\\"Where\\\":[{\\\"Condition\\\":{\\\"In\\\":{\\\"Expressions\\\":[{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Status\\\"}}],\\\"Values\\\":[[{\\\"Literal\\\":{\\\"Value\\\":\\\"\'Open\'\\\"}}]]}}}],\\\"OrderBy\\\":[{\\\"Direction\\\":2,\\\"Expression\\\":{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Revenue\\\"}}}]},\\\"Binding\\\":{\\\"Primary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[0,1]}]},\\\"DataReduction\\\":{\\\"DataVolume\\\":4,\\\"Primary\\\":{\\\"Window\\\":{\\\"Count\\\":200}}},\\\"Version\\\":1},\\\"ExecutionMetricsKind\\\":1}}]}\",\"QueryId\":\"\",\"ApplicationContext\":{\"DatasetId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\",\"Sources\":[{\"ReportId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\",\"VisualId\":\"VisualContainer3\"}]}}],\"cancelQueries\":[],\"modelId\":1922721,\"userPreferredLocale\":\"en-US\"}";
            request25.Body = request25Body;
            yield return request25;
            request25 = null;
            this.AddCommentToResult("28_Open Sales Pipeline");

            CrmRequest request26 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/querydata");
            request26.Method = "POST";
            request26.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request26.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request26.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request26.Headers.Add(new WebTestRequestHeader("RequestId", "7284e6bc-da1f-a56d-2cd5-a4c64e22b991"));
            request26.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request26.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json;charset=UTF-8"));
            request26.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request26.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request26.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request26.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request26.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request26Body = new StringHttpBody();
            request26Body.ContentType = "application/json;charset=UTF-8";
            request26Body.InsertByteOrderMark = false;
            request26Body.BodyString = "{\"version\":\"1.0.0\",\"queries\":[{\"Query\":{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"o\",\"Entity\":\"Opportunity\",\"Type\":0}],\"Select\":[{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Revenue\"},\"Name\":\"Opportunity.Open Revenue\"}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0]}]},\"DataReduction\":{\"DataVolume\":3,\"Primary\":{\"Top\":{}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]},\"CacheKey\":\"{\\\"Commands\\\":[{\\\"SemanticQueryDataShapeCommand\\\":{\\\"Query\\\":{\\\"Version\\\":2,\\\"From\\\":[{\\\"Name\\\":\\\"o\\\",\\\"Entity\\\":\\\"Opportunity\\\",\\\"Type\\\":0}],\\\"Select\\\":[{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Revenue\\\"},\\\"Name\\\":\\\"Opportunity.Open Revenue\\\"}]},\\\"Binding\\\":{\\\"Primary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[0]}]},\\\"DataReduction\\\":{\\\"DataVolume\\\":3,\\\"Primary\\\":{\\\"Top\\\":{}}},\\\"Version\\\":1},\\\"ExecutionMetricsKind\\\":1}}]}\",\"QueryId\":\"\",\"ApplicationContext\":{\"DatasetId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\",\"Sources\":[{\"ReportId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\",\"VisualId\":\"VisualContainer1\"}]}},{\"Query\":{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"o\",\"Entity\":\"Opportunity\",\"Type\":0}],\"Select\":[{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Count\"},\"Name\":\"Opportunity.Open Count\"}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0]}]},\"DataReduction\":{\"DataVolume\":3,\"Primary\":{\"Top\":{}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]},\"CacheKey\":\"{\\\"Commands\\\":[{\\\"SemanticQueryDataShapeCommand\\\":{\\\"Query\\\":{\\\"Version\\\":2,\\\"From\\\":[{\\\"Name\\\":\\\"o\\\",\\\"Entity\\\":\\\"Opportunity\\\",\\\"Type\\\":0}],\\\"Select\\\":[{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Count\\\"},\\\"Name\\\":\\\"Opportunity.Open Count\\\"}]},\\\"Binding\\\":{\\\"Primary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[0]}]},\\\"DataReduction\\\":{\\\"DataVolume\\\":3,\\\"Primary\\\":{\\\"Top\\\":{}}},\\\"Version\\\":1},\\\"ExecutionMetricsKind\\\":1}}]}\",\"QueryId\":\"\",\"ApplicationContext\":{\"DatasetId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\",\"Sources\":[{\"ReportId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\",\"VisualId\":\"VisualContainer2\"}]}}],\"cancelQueries\":[],\"modelId\":1922721,\"userPreferredLocale\":\"en-US\"}";
            request26.Body = request26Body;
            yield return request26;
            request26 = null;
            this.AddCommentToResult("29_Open Sales Pipeline");

            CrmRequest request27 = new CrmRequest("https://wabi-us-east2-b-primary-redirect.analysis.windows.net/explore/querydata");
            request27.Method = "POST";
            request27.Headers.Add(new WebTestRequestHeader("X-PowerBI-HostEnv", "Embedding for your organization"));
            request27.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request27.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request27.Headers.Add(new WebTestRequestHeader("RequestId", "cb65d705-5c1d-bb4e-de9f-8950c3371b0a"));
            request27.Headers.Add(new WebTestRequestHeader("ActivityId", "b45faad9-cf4d-4268-8e91-cad05e848037"));
            request27.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json;charset=UTF-8"));
            request27.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request27.Headers.Add(new WebTestRequestHeader("Origin", "https://app.powerbi.com"));
            request27.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
            request27.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request27.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request27Body = new StringHttpBody();
            request27Body.ContentType = "application/json;charset=UTF-8";
            request27Body.InsertByteOrderMark = false;
            request27Body.BodyString = "{\"version\":\"1.0.0\",\"queries\":[{\"Query\":{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"o\",\"Entity\":\"Opportunity\",\"Type\":0}],\"Select\":[{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Created On Month Year Value\"},\"Name\":\"Opportunity.Created On Month Year Value\"},{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Count\"},\"Name\":\"Opportunity.Open Count\"},{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Rating\"},\"Name\":\"Opportunity.Rating\"},{\"Aggregation\":{\"Expression\":{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Est. Revenue (Base)\"}},\"Function\":0},\"Name\":\"Sum(Opportunity.Est. Revenue (Base))\"}],\"OrderBy\":[{\"Direction\":1,\"Expression\":{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Created On Month Year Value\"}}},{\"Direction\":1,\"Expression\":{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Created On Month Year Value\"}}}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0,1,3]}]},\"Secondary\":{\"Groupings\":[{\"Projections\":[2],\"SuppressedProjections\":[3]}]},\"DataReduction\":{\"DataVolume\":4,\"Primary\":{\"Window\":{\"Count\":200}},\"Secondary\":{\"Top\":{\"Count\":60}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]},\"CacheKey\":\"{\\\"Commands\\\":[{\\\"SemanticQueryDataShapeCommand\\\":{\\\"Query\\\":{\\\"Version\\\":2,\\\"From\\\":[{\\\"Name\\\":\\\"o\\\",\\\"Entity\\\":\\\"Opportunity\\\",\\\"Type\\\":0}],\\\"Select\\\":[{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Created On Month Year Value\\\"},\\\"Name\\\":\\\"Opportunity.Created On Month Year Value\\\"},{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Count\\\"},\\\"Name\\\":\\\"Opportunity.Open Count\\\"},{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Rating\\\"},\\\"Name\\\":\\\"Opportunity.Rating\\\"},{\\\"Aggregation\\\":{\\\"Expression\\\":{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Est. Revenue (Base)\\\"}},\\\"Function\\\":0},\\\"Name\\\":\\\"Sum(Opportunity.Est. Revenue (Base))\\\"}],\\\"OrderBy\\\":[{\\\"Direction\\\":1,\\\"Expression\\\":{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Created On Month Year Value\\\"}}},{\\\"Direction\\\":1,\\\"Expression\\\":{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Created On Month Year Value\\\"}}}]},\\\"Binding\\\":{\\\"Primary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[0,1,3]}]},\\\"Secondary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[2],\\\"SuppressedProjections\\\":[3]}]},\\\"DataReduction\\\":{\\\"DataVolume\\\":4,\\\"Primary\\\":{\\\"Window\\\":{\\\"Count\\\":200}},\\\"Secondary\\\":{\\\"Top\\\":{\\\"Count\\\":60}}},\\\"Version\\\":1},\\\"ExecutionMetricsKind\\\":1}}]}\",\"QueryId\":\"\",\"ApplicationContext\":{\"DatasetId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\",\"Sources\":[{\"ReportId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\",\"VisualId\":\"VisualContainer7\"}]}},{\"Query\":{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"u\",\"Entity\":\"User\",\"Type\":0},{\"Name\":\"o\",\"Entity\":\"Opportunity\",\"Type\":0}],\"Select\":[{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"u\"}},\"Property\":\"Full Name\"},\"Name\":\"User.Full Name\"},{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Revenue\"},\"Name\":\"Opportunity.Open Revenue\"}],\"OrderBy\":[{\"Direction\":2,\"Expression\":{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"o\"}},\"Property\":\"Open Revenue\"}}}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0,1]}]},\"DataReduction\":{\"DataVolume\":4,\"Primary\":{\"Window\":{\"Count\":200}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]},\"CacheKey\":\"{\\\"Commands\\\":[{\\\"SemanticQueryDataShapeCommand\\\":{\\\"Query\\\":{\\\"Version\\\":2,\\\"From\\\":[{\\\"Name\\\":\\\"u\\\",\\\"Entity\\\":\\\"User\\\",\\\"Type\\\":0},{\\\"Name\\\":\\\"o\\\",\\\"Entity\\\":\\\"Opportunity\\\",\\\"Type\\\":0}],\\\"Select\\\":[{\\\"Column\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"u\\\"}},\\\"Property\\\":\\\"Full Name\\\"},\\\"Name\\\":\\\"User.Full Name\\\"},{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Revenue\\\"},\\\"Name\\\":\\\"Opportunity.Open Revenue\\\"}],\\\"OrderBy\\\":[{\\\"Direction\\\":2,\\\"Expression\\\":{\\\"Measure\\\":{\\\"Expression\\\":{\\\"SourceRef\\\":{\\\"Source\\\":\\\"o\\\"}},\\\"Property\\\":\\\"Open Revenue\\\"}}}]},\\\"Binding\\\":{\\\"Primary\\\":{\\\"Groupings\\\":[{\\\"Projections\\\":[0,1]}]},\\\"DataReduction\\\":{\\\"DataVolume\\\":4,\\\"Primary\\\":{\\\"Window\\\":{\\\"Count\\\":200}}},\\\"Version\\\":1},\\\"ExecutionMetricsKind\\\":1}}]}\",\"QueryId\":\"\",\"ApplicationContext\":{\"DatasetId\":\"e6d0e929-7e02-4fd9-b013-95a9288549fe\",\"Sources\":[{\"ReportId\":\"85531de0-6656-4db2-a7b5-30394e9c1d26\",\"VisualId\":\"VisualContainer4\"}]}}],\"cancelQueries\":[],\"modelId\":1922721,\"userPreferredLocale\":\"en-US\"}";
            request27.Body = request27Body;
            yield return request27;
            request27 = null;
            this.EndTransaction("1006.01_Open Sales Pipeline");
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
