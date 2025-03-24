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
//string contactid = requestN.lastResponse.Headers["Location"].Split('(')[1].Substring(0, 36); 

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

    public class zz_1008_CanvasAppSubmitTimeCoded : WebTestBase
    {
        private string thisURL;
        private CRMEntity user;
        private string crmServer;
        private string Token;
        private string Timestamp;
        private string orgName;
        private string systemuserId;
        private Guid orgId;
        private string newId;
        private string newIdMarker = @"entity"":{""Id"":""{";
        private string workorderId;
        private string expiration = "";
        private string contoso_service_order = "7671831";
        public zz_1008_CanvasAppSubmitTimeCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1008_CanvasAppSubmitTimeCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1008_CanvasAppSubmitTimeCoded_PostWebTest);
        }
        public void zz_1008_CanvasAppSubmitTimeCoded_PreWebTest(object sender, PreWebTestEventArgs e)
        {            
            user = WebTestBase_PreWebTest(sender, e)[EntityNames.Users] as CRMEntity;
            systemuserId = user["systemuserid"];

            crmServer = user["serverbaseurl"];
            e.WebTest.UserName = user["domainName"];
            e.WebTest.Context.Add("user", user["domainName"]);
            e.WebTest.Password = user["userpassword"];
            orgId = new Guid(user["organizationid"]);
            orgName = user["organizationname"];
            thisURL = crmServer;

            //Build the SQL for the current request
            using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
            {

                sqlConn.Open();
                using (System.Data.SqlClient.SqlTransaction tran = sqlConn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                    cmd.Transaction = tran;
                    cmd.CommandTimeout = 60;
                    cmd.Connection = sqlConn;

                    string ukey = Guid.NewGuid().ToString();

                    //Set User to InUse
                    cmd.CommandText = "UPDATE ltinstallwo set uid = '" + ukey + "' where workorderId in (select top 1 workorderId from ltinstallwo where inuse = 0 and state <=1 order by newid())";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = string.Format("SELECT workorderId from ltinstallwo(nolock) where uid = '" + ukey + "'");

                    System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                    cmd.Parameters.Clear();

                    //Lock this guy and commit the transaction.
                    while (true == reader.Read())
                    {
                        //Read User
                        workorderId = reader[0].ToString();
                    }
                    if (!reader.IsClosed)
                        reader.Close();

                    tran.Commit();
                }
                sqlConn.Close();
            }

        

        }
        public void zz_1008_CanvasAppSubmitTimeCoded_PostWebTest(object sender, PostWebTestEventArgs e)
        {
          
        }
        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            this.BeginTransaction("1008.01_Authenticate to Canvas App");
                string entryId = Guid.NewGuid().ToString();

                CrmRequest request59 = new CrmRequest("https://contoso-perf2.crm.dynamics.com/api/data/v9.0/canvasapps");
                request59.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                request59.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request59.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "d93d59e5-1f7a-4970-9b59-4b418d5f9c58"));
                request59.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "918079db-c902-4e29-b22c-9764410d0375"));
                request59.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=CustomQuery; Source=Xrm.WebApi.online.retrieveMultipleRecords"));
                request59.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
                request59.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "a7799d47-0a80-4fb0-bb14-e0ef9bdc9231"));
                request59.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "contoso_fsmapp"));
                request59.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                request59.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
                request59.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                request59.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "c939c1e2-926d-ec11-8941-000d3a3bc793"));
                request59.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "ba04de29-84b9-495c-88e5-e4f191776554"));
                request59.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9c874c73-ac4d-4567-bccc-4bce52c2eacb"));
                request59.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3204-2109.2 (Browser; AppName=contoso_fsmapp)"));
                request59.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
                request59.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
                request59.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                request59.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                request59.QueryStringParameters.Add("$select", "appopenuri,tags,appversion");
                request59.QueryStringParameters.Add("$filter", "name eq \'contoso_timeentrytocatsintegration_05887\'");
                yield return request59;
                request59 = null;
                this.AddCommentToResult("60_Authenticate to Canvas App");

                CrmRequest loginrequest1 = new CrmRequest("https://apps.powerapps.com/play/4f542db4-e286-43e9-99f8-6082ef553c30");
                loginrequest1.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest1.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
                loginrequest1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "none"));
                loginrequest1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
                loginrequest1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-User", "?1"));
                loginrequest1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
                loginrequest1.QueryStringParameters.Add("source", "modelDrivenApp");
                loginrequest1.QueryStringParameters.Add("screenColor", "rgba(239,243,246,1)");
                loginrequest1.QueryStringParameters.Add("CID", "3");
                loginrequest1.QueryStringParameters.Add("TimeEntries", "{" + entryId + "}");
                loginrequest1.QueryStringParameters.Add("redirectUrl", "https://contoso-perf2.crm.dynamics.com/{637702538740000145}/webresources/contoso_/html/canvas_dialog.html");
                yield return loginrequest1;
                loginrequest1 = null;
                this.AddCommentToResult("02_Authenticate to Canvas App");

                CrmRequest loginrequest2 = new CrmRequest("https://pa-static-ms.azureedge.net/resource/webplayerdynamic/publishedapp/preloadindex");
                loginrequest2.ThinkTime = 1;
                loginrequest2.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest2.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
                loginrequest2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                loginrequest2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
                loginrequest2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "iframe"));
                loginrequest2.QueryStringParameters.Add("preloadIndexPath", "https://content.powerapps.com/resource/app/0be5b73ilqtid/preloadindex.web.html");
                loginrequest2.QueryStringParameters.Add("PowerAppsLanguage", "en-US");
                loginrequest2.QueryStringParameters.Add("loader", "inline");
                loginrequest2.QueryStringParameters.Add("lv", "s2hlpqm9qedc1");
                loginrequest2.QueryStringParameters.Add("serviceWorkerUrl", "https://pa-static-ms.azureedge.net/resource/webplayer/hashedresources/c2n8b572acri8/js/PowerAppsServiceWorker.PublishedApp.js");
                loginrequest2.QueryStringParameters.Add("unregisterServiceWorkersHash", "61al1u62ufj72");
                loginrequest2.QueryStringParameters.Add("piv", "A86DEA27");
                loginrequest2.QueryStringParameters.Add("featureGates", "{\"publishedAppServiceWorker\":false}");
                yield return loginrequest2;
                loginrequest2 = null;
                this.AddCommentToResult("03_Authenticate to Canvas App");

                CrmRequest loginrequest3 = new CrmRequest("https://unitedstates.api.powerapps.com/api/invoke");
                loginrequest3.Method = "POST";
                loginrequest3.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest3.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "webplayer-1634929801832"));
                loginrequest3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest3.Headers.Add(new WebTestRequestHeader("x-ms-path-query", "/healthcheck/compute"));
                loginrequest3.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "webplayer-preload-1634929801832"));
                loginrequest3.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps (Web; Player)"));
                loginrequest3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest3.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-site"));
                loginrequest3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                yield return loginrequest3;
                loginrequest3 = null;
                this.AddCommentToResult("04_Authenticate to Canvas App");

                CrmRequest loginrequest4 = new CrmRequest("https://login.microsoftonline.com/common/discovery/instance");
                loginrequest4.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest4.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                loginrequest4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                loginrequest4.QueryStringParameters.Add("api-version", "1.1");
                loginrequest4.QueryStringParameters.Add("authorization_endpoint", "https://login.microsoftonline.com/common/oauth2/v2.0/authorize");
                yield return loginrequest4;
                loginrequest4 = null;
                this.AddCommentToResult("05_Authenticate to Canvas App");

                CrmRequest loginrequest5 = new CrmRequest("https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration");
                loginrequest5.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest5.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                loginrequest5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                yield return loginrequest5;
                loginrequest5 = null;
                this.AddCommentToResult("06_Authenticate to Canvas App");

                CrmRequest loginrequest6 = new CrmRequest("https://login.microsoftonline.com/common/oauth2/v2.0/authorize");
                loginrequest6.ThinkTime = 1;
                loginrequest6.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest6.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
                loginrequest6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                loginrequest6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
                loginrequest6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
                loginrequest6.FollowRedirects = false;
                loginrequest6.QueryStringParameters.Add("client_id", "3e62f81e-590b-425b-9531-cad6683656cf");
                loginrequest6.QueryStringParameters.Add("scope", "https://service.powerapps.com//.default openid profile offline_access");
                loginrequest6.QueryStringParameters.Add("redirect_uri", "https://apps.powerapps.com/auth/v2");
                loginrequest6.QueryStringParameters.Add("client-request-id", "512e1ecf-8f87-430e-84d8-f2afbad20aea");
                loginrequest6.QueryStringParameters.Add("response_mode", "fragment");
                loginrequest6.QueryStringParameters.Add("response_type", "code");
                loginrequest6.QueryStringParameters.Add("x-client-SKU", "msal.js.browser");
                loginrequest6.QueryStringParameters.Add("x-client-VER", "2.14.2");
                loginrequest6.QueryStringParameters.Add("x-client-OS", "");
                loginrequest6.QueryStringParameters.Add("x-client-CPU", "");
                loginrequest6.QueryStringParameters.Add("client_info", "1");
                loginrequest6.QueryStringParameters.Add("code_challenge", "IifbCshFMinggw1ZrYMTglpSQHQ1T7L71f-ehlXGlQU");
                loginrequest6.QueryStringParameters.Add("code_challenge_method", "S256");
                loginrequest6.QueryStringParameters.Add("nonce", "8c96e5e9-a890-4106-bb57-0e3498b909c1");
                loginrequest6.QueryStringParameters.Add("state", "eyJpZCI6ImU5ZmFkODQ1LTkzOGItNDViYS1iOTIwLTRjYmRhN2ZkMjc0MiIsIm1ldGEiOnsiaW50ZXJhY3Rpb25UeXBlIjoicmVkaXJlY3QifX0=");
                ExtractText er_apiCanary = new ExtractText();
                er_apiCanary.StartsWith = "apiCanary\":\"";
                er_apiCanary.EndsWith = "\"";
                er_apiCanary.IgnoreCase = false;
                er_apiCanary.UseRegularExpression = false;
                er_apiCanary.Required = false;
                er_apiCanary.ExtractRandomMatch = false;
                er_apiCanary.Index = 0;
                er_apiCanary.HtmlDecode = true;
                er_apiCanary.SearchInHeaders = false;
                er_apiCanary.ContextParameterName = "apiCanary";
                loginrequest6.ExtractValues += new EventHandler<ExtractionEventArgs>(er_apiCanary.Extract);

                ExtractText er_clientreqid = new ExtractText();
                er_clientreqid.StartsWith = "client-request-id=";
                er_clientreqid.EndsWith = "\"";
                er_clientreqid.IgnoreCase = false;
                er_clientreqid.UseRegularExpression = false;
                er_clientreqid.Required = false;
                er_clientreqid.ExtractRandomMatch = false;
                er_clientreqid.Index = 0;
                er_clientreqid.HtmlDecode = true;
                er_clientreqid.SearchInHeaders = false;
                er_clientreqid.ContextParameterName = "client-request-id";
                loginrequest6.ExtractValues += new EventHandler<ExtractionEventArgs>(er_clientreqid.Extract);

                ExtractText er_sessionid = new ExtractText();
                er_sessionid.StartsWith = "sessionId\":\"";
                er_sessionid.EndsWith = "\"";
                er_sessionid.IgnoreCase = false;
                er_sessionid.UseRegularExpression = false;
                er_sessionid.Required = false;
                er_sessionid.ExtractRandomMatch = false;
                er_sessionid.Index = 0;
                er_sessionid.HtmlDecode = true;
                er_sessionid.SearchInHeaders = false;
                er_sessionid.ContextParameterName = "sessionId";
                loginrequest6.ExtractValues += new EventHandler<ExtractionEventArgs>(er_sessionid.Extract);

                ExtractText er_ctx = new ExtractText();
                er_ctx.StartsWith = "ctx=";
                er_ctx.EndsWith = "\"";
                er_ctx.IgnoreCase = false;
                er_ctx.UseRegularExpression = false;
                er_ctx.Required = false;
                er_ctx.ExtractRandomMatch = false;
                er_ctx.Index = 0;
                er_ctx.HtmlDecode = true;
                er_ctx.SearchInHeaders = false;
                er_ctx.ContextParameterName = "ctx";
                loginrequest6.ExtractValues += new EventHandler<ExtractionEventArgs>(er_ctx.Extract);

                ExtractText er_sft = new ExtractText();
                er_sft.StartsWith = "sFT\":\"";
                er_sft.EndsWith = "\"";
                er_sft.IgnoreCase = false;
                er_sft.UseRegularExpression = false;
                er_sft.Required = false;
                er_sft.ExtractRandomMatch = false;
                er_sft.Index = 0;
                er_sft.HtmlDecode = true;
                er_sft.SearchInHeaders = false;
                er_sft.ContextParameterName = "sFT";
                loginrequest6.ExtractValues += new EventHandler<ExtractionEventArgs>(er_sft.Extract);

                ExtractText er_canary = new ExtractText();
                er_canary.StartsWith = "canary\":\"";
                er_canary.EndsWith = "\"";
                er_canary.IgnoreCase = false;
                er_canary.UseRegularExpression = false;
                er_canary.Required = false;
                er_canary.ExtractRandomMatch = false;
                er_canary.Index = 0;
                er_canary.HtmlDecode = true;
                er_canary.SearchInHeaders = false;
                er_canary.ContextParameterName = "canary";
                loginrequest6.ExtractValues += new EventHandler<ExtractionEventArgs>(er_canary.Extract);

                ExtractText er_authcode = new ExtractText();
                er_authcode.StartsWith = "code=";
                er_authcode.EndsWith = "&amp;";
                er_authcode.IgnoreCase = false;
                er_authcode.UseRegularExpression = false;
                er_authcode.Required = false;
                er_authcode.ExtractRandomMatch = false;
                er_authcode.Index = 0;
                er_authcode.HtmlDecode = true;
                er_authcode.SearchInHeaders = false;
                er_authcode.ContextParameterName = "authcode";
                loginrequest6.ExtractValues += new EventHandler<ExtractionEventArgs>(er_authcode.Extract);

                yield return loginrequest6;
                loginrequest6 = null;

                if (!Context.ContainsKey("authcode"))
                {
                    this.AddCommentToResult("07_Authenticate to Canvas App");

                    CrmRequest loginrequest7 = new CrmRequest("https://login.live.com/Me.htm");
                    loginrequest7.ThinkTime = 14;
                    loginrequest7.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                    loginrequest7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                    loginrequest7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                    loginrequest7.Headers.Add(new WebTestRequestHeader("Purpose", "prefetch"));
                    loginrequest7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                    loginrequest7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "no-cors"));
                    loginrequest7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                    loginrequest7.QueryStringParameters.Add("v", "3");
                    yield return loginrequest7;
                    loginrequest7 = null;
                    this.AddCommentToResult("08_Authenticate to Canvas App");


                    CrmRequest loginrequest9 = new CrmRequest("https://login.microsoftonline.com/common/GetCredentialType");
                    loginrequest9.ThinkTime = 6;
                    loginrequest9.Method = "POST";
                    loginrequest9.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));

                    loginrequest9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("hpgrequestid", "" + this.Context["sessionid"].ToString() + ""));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("client-request-id", "" + this.Context["client-request-id"].ToString() + ""));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("canary", "" + this.Context["apiCanary"].ToString() + ""));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("Content-type", "application/json; charset=UTF-8"));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("hpgid", "1104"));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("hpgact", "1800"));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("Origin", "https://login.microsoftonline.com"));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                    loginrequest9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                    loginrequest9.QueryStringParameters.Add("mkt", "en-US");
                    StringHttpBody loginrequest9Body = new StringHttpBody();
                    loginrequest9Body.ContentType = "application/json; charset=UTF-8";
                    loginrequest9Body.InsertByteOrderMark = false;
                    loginrequest9Body.BodyString = "{\"username\":\""+user["domainName"]+"\",\"isOtherIdpSupported\":false,\"checkPhones\":false,\"isRemoteNGCSupported\":true,\"isCookieBannerShown\":false,\"isFidoSupported\":true,\"originalRequest\":\"" + this.Context["ctx"].ToString() + "\",\"country\":\"US\",\"forceotclogin\":false,\"isExternalFederationDisallowed\":false,\"isRemoteConnectSupported\":false,\"federationFlags\":0,\"isSignup\":false,\"flowToken\":\"" + this.Context["sFT"].ToString() + "\",\"isAccessPassSupported\":true}";
                    loginrequest9.Body = loginrequest9Body;
                    loginrequest9.ExtractValues += new EventHandler<ExtractionEventArgs>(er_apiCanary.Extract);
                    yield return loginrequest9;
                    loginrequest9 = null;


                    CrmRequest loginrequest122 = new CrmRequest("https://login.microsoftonline.com/common/login");
                    loginrequest122.Method = "POST";
                    loginrequest122.FollowRedirects = false;
                    loginrequest122.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                    loginrequest122.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                    loginrequest122.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                    loginrequest122.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
                    loginrequest122.Headers.Add(new WebTestRequestHeader("Origin", "https://login.microsoftonline.com"));
                    loginrequest122.Headers.Add(new WebTestRequestHeader("Content-Type", "application/x-www-form-urlencoded"));
                    loginrequest122.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
                    loginrequest122.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
                    loginrequest122.Headers.Add(new WebTestRequestHeader("Sec-Fetch-User", "?1"));
                    loginrequest122.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
                    FormPostHttpBody loginrequest122Body = new FormPostHttpBody();
                    loginrequest122Body.FormPostParameters.Add("i13", "0");
                    loginrequest122Body.FormPostParameters.Add("login", ""+user["domainName"]+"");
                    loginrequest122Body.FormPostParameters.Add("loginfmt", ""+user["domainName"]+"");
                    loginrequest122Body.FormPostParameters.Add("type", "11");
                    loginrequest122Body.FormPostParameters.Add("LoginOptions", "3");
                    loginrequest122Body.FormPostParameters.Add("lrt", "");
                    loginrequest122Body.FormPostParameters.Add("lrtPartition", "");
                    loginrequest122Body.FormPostParameters.Add("hisRegion", "");
                    loginrequest122Body.FormPostParameters.Add("hisScaleUnit", "");
                    loginrequest122Body.FormPostParameters.Add("passwd", "SamplePassword");
                    loginrequest122Body.FormPostParameters.Add("ps", "2");
                    loginrequest122Body.FormPostParameters.Add("psRNGCDefaultType", "");
                    loginrequest122Body.FormPostParameters.Add("psRNGCEntropy", "");
                    loginrequest122Body.FormPostParameters.Add("psRNGCSLK", "");
                    loginrequest122Body.FormPostParameters.Add("canary", "" + this.Context["canary"].ToString() + "");
                    loginrequest122Body.FormPostParameters.Add("ctx", @"" + this.Context["ctx"].ToString() + @"");
                    loginrequest122Body.FormPostParameters.Add("hpgrequestid", "" + this.Context["sessionid"].ToString() + "");
                    loginrequest122Body.FormPostParameters.Add("flowToken", @"" + this.Context["sFT"].ToString() + @"");
                    loginrequest122Body.FormPostParameters.Add("PPSX", "");
                    loginrequest122Body.FormPostParameters.Add("NewUser", "1");
                    loginrequest122Body.FormPostParameters.Add("FoundMSAs", "");
                    loginrequest122Body.FormPostParameters.Add("fspost", "0");
                    loginrequest122Body.FormPostParameters.Add("i21", "0");
                    loginrequest122Body.FormPostParameters.Add("CookieDisclosure", "0");
                    loginrequest122Body.FormPostParameters.Add("IsFidoSupported", "1");
                    loginrequest122Body.FormPostParameters.Add("isSignupPost", "0");
                    loginrequest122Body.FormPostParameters.Add("i2", "1");
                    loginrequest122Body.FormPostParameters.Add("i17", "");
                    loginrequest122Body.FormPostParameters.Add("i18", "");
                    loginrequest122Body.FormPostParameters.Add("i19", "10704");
                    loginrequest122.Body = loginrequest122Body;

                    loginrequest122.ExtractValues += new EventHandler<ExtractionEventArgs>(er_authcode.Extract);
                    yield return loginrequest122;
                    loginrequest122 = null;
                    this.AddCommentToResult("11_Authenticate to Canvas App");
                }

                CrmRequest loginrequest10 = new CrmRequest("https://apps.powerapps.com/play/4f542db4-e286-43e9-99f8-6082ef553c30");
                loginrequest10.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest10.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
                loginrequest10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
                loginrequest10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
                loginrequest10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
                loginrequest10.QueryStringParameters.Add("source", "modelDrivenApp");
                loginrequest10.QueryStringParameters.Add("screenColor", "rgba(239,243,246,1)");
                loginrequest10.QueryStringParameters.Add("CID", "3");
                loginrequest10.QueryStringParameters.Add("TimeEntries", "{" + entryId + "}");
                loginrequest10.QueryStringParameters.Add("redirectUrl", "https://contoso-perf2.crm.dynamics.com/{637702538740000145}/webresources/contoso_/html/canvas_dialog.html");
                yield return loginrequest10;
                loginrequest10 = null;
                this.AddCommentToResult("12_Authenticate to Canvas App");

                CrmRequest loginrequest11 = new CrmRequest("https://unitedstates.api.powerapps.com/api/invoke");
                loginrequest11.Method = "POST";
                loginrequest11.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest11.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "webplayer-1634929824944"));
                loginrequest11.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest11.Headers.Add(new WebTestRequestHeader("x-ms-path-query", "/healthcheck/compute"));
                loginrequest11.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "webplayer-preload-1634929824944"));
                loginrequest11.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps (Web; Player)"));
                loginrequest11.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest11.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-site"));
                loginrequest11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                yield return loginrequest11;
                loginrequest11 = null;
                this.AddCommentToResult("13_Submit Time");


                CrmRequest loginrequest13 = new CrmRequest("https://login.microsoftonline.com/common/oauth2/v2.0/token");
                loginrequest13.Method = "POST";
                loginrequest13.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest13.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest13.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest13.Headers.Add(new WebTestRequestHeader("content-type", "application/x-www-form-urlencoded;charset=utf-8"));
                loginrequest13.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                loginrequest13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                FormPostHttpBody loginrequest13Body = new FormPostHttpBody();
                loginrequest13Body.FormPostParameters.Add("client_id", "3e62f81e-590b-425b-9531-cad6683656cf");
                loginrequest13Body.FormPostParameters.Add("redirect_uri", "https://apps.powerapps.com/auth/v2");
                loginrequest13Body.FormPostParameters.Add("scope", "https://service.powerapps.com//.default openid profile offline_access");
                loginrequest13Body.FormPostParameters.Add("code", @"" + this.Context["authcode"].ToString() + @"");
                loginrequest13Body.FormPostParameters.Add("x-client-SKU", "msal.js.browser");
                loginrequest13Body.FormPostParameters.Add("x-client-VER", "2.14.2");
                loginrequest13Body.FormPostParameters.Add("x-client-OS", "");
                loginrequest13Body.FormPostParameters.Add("x-client-CPU", "");
                loginrequest13Body.FormPostParameters.Add("x-ms-lib-capability", "retry-after, h429");
                loginrequest13Body.FormPostParameters.Add("x-client-current-telemetry", "2|865,0|,");
                loginrequest13Body.FormPostParameters.Add("x-client-last-telemetry", "2|0|||0,0");
                loginrequest13Body.FormPostParameters.Add("code_verifier", "dJYvtioA6nHAyuRGIv8MFJ7kX_kBkuBlTrZn3xAqmrQ");
                loginrequest13Body.FormPostParameters.Add("grant_type", "authorization_code");
                loginrequest13Body.FormPostParameters.Add("client_info", "1");
                loginrequest13Body.FormPostParameters.Add("client-request-id", "27486507-87bd-44cd-bbb2-de1b0072d572");
                loginrequest13.Body = loginrequest13Body;
                ExtractText er_refresh_token = new ExtractText();
                er_refresh_token.StartsWith = "\"refresh_token\":\"";
                er_refresh_token.EndsWith = "\"";
                er_refresh_token.IgnoreCase = false;
                er_refresh_token.UseRegularExpression = false;
                er_refresh_token.Required = true;
                er_refresh_token.ExtractRandomMatch = false;
                er_refresh_token.Index = 0;
                er_refresh_token.HtmlDecode = true;
                er_refresh_token.SearchInHeaders = false;
                er_refresh_token.ContextParameterName = "refresh_token";
                loginrequest13.ExtractValues += new EventHandler<ExtractionEventArgs>(er_refresh_token.Extract);

                ExtractText er_access_token2 = new ExtractText();
                er_access_token2.StartsWith = "access_token\":\"";
                er_access_token2.EndsWith = "\"";
                er_access_token2.IgnoreCase = false;
                er_access_token2.UseRegularExpression = false;
                er_access_token2.Required = true;
                er_access_token2.ExtractRandomMatch = false;
                er_access_token2.Index = 0;
                er_access_token2.HtmlDecode = true;
                er_access_token2.SearchInHeaders = false;
                er_access_token2.ContextParameterName = "powerapps_access_token";
                loginrequest13.ExtractValues += new EventHandler<ExtractionEventArgs>(er_access_token2.Extract);
                yield return loginrequest13;
                loginrequest13 = null;

                CrmRequest loginrequest14 = new CrmRequest("https://login.microsoftonline.com/common/oauth2/v2.0/token");
                loginrequest14.Method = "POST";
                loginrequest14.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest14.Headers.Add(new WebTestRequestHeader("content-type", "application/x-www-form-urlencoded;charset=utf-8"));
                loginrequest14.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                loginrequest14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                FormPostHttpBody loginrequest14Body = new FormPostHttpBody();
                loginrequest14Body.FormPostParameters.Add("client_id", "3e62f81e-590b-425b-9531-cad6683656cf");
                loginrequest14Body.FormPostParameters.Add("scope", "https://apihub.azure.com/.default openid profile offline_access");
                loginrequest14Body.FormPostParameters.Add("grant_type", "refresh_token");
                loginrequest14Body.FormPostParameters.Add("client_info", "1");
                loginrequest14Body.FormPostParameters.Add("x-client-SKU", "msal.js.browser");
                loginrequest14Body.FormPostParameters.Add("x-client-VER", "2.14.2");
                loginrequest14Body.FormPostParameters.Add("x-client-OS", "");
                loginrequest14Body.FormPostParameters.Add("x-client-CPU", "");
                loginrequest14Body.FormPostParameters.Add("x-ms-lib-capability", "retry-after, h429");
                loginrequest14Body.FormPostParameters.Add("x-client-current-telemetry", "2|61,0|,");
                loginrequest14Body.FormPostParameters.Add("x-client-last-telemetry", "2|1|||0,0");
                loginrequest14Body.FormPostParameters.Add("client-request-id", "784feaef-2197-407b-bac5-895b3d40b987");
                loginrequest14Body.FormPostParameters.Add("refresh_token", @"" + this.Context["refresh_token"].ToString() + @"");
                loginrequest14.Body = loginrequest14Body;

                ExtractText er_refreshtoken2 = new ExtractText();
                er_refreshtoken2.StartsWith = "\"refresh_token\":\"";
                er_refreshtoken2.EndsWith = "\"";
                er_refreshtoken2.IgnoreCase = false;
                er_refreshtoken2.UseRegularExpression = false;
                er_refreshtoken2.Required = true;
                er_refreshtoken2.ExtractRandomMatch = false;
                er_refreshtoken2.Index = 0;
                er_refreshtoken2.HtmlDecode = true;
                er_refreshtoken2.SearchInHeaders = false;
                er_refreshtoken2.ContextParameterName = "refresh_token2";
                loginrequest14.ExtractValues += new EventHandler<ExtractionEventArgs>(er_refreshtoken2.Extract);
                            
                ExtractText er_accesstoken = new ExtractText();
                er_accesstoken.StartsWith = "access_token\":\"";
                er_accesstoken.EndsWith = "\"";
                er_accesstoken.IgnoreCase = false;
                er_accesstoken.UseRegularExpression = false;
                er_accesstoken.Required = true;
                er_accesstoken.ExtractRandomMatch = false;
                er_accesstoken.Index = 0;
                er_accesstoken.HtmlDecode = true;
                er_accesstoken.SearchInHeaders = false;
                er_accesstoken.ContextParameterName = "apihub_access_token";
                loginrequest14.ExtractValues += new EventHandler<ExtractionEventArgs>(er_accesstoken.Extract);
                yield return loginrequest14;
                loginrequest14 = null;

                CrmRequest loginrequest144 = new CrmRequest("https://apps.powerapps.com/auth/session");
                loginrequest144.Method = "POST";
                loginrequest144.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest144.Headers.Add(new WebTestRequestHeader("x-ms-correlation-request-id", "fd94fb3c-4ceb-453d-bfd0-6a0c1f07e63e"));
                loginrequest144.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest144.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=UTF-8"));
                loginrequest144.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "fd94fb3c-4ceb-453d-bfd0-6a0c1f07e63e"));
                loginrequest144.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest144.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest144.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
                loginrequest144.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest144.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                StringHttpBody loginrequest144Body = new StringHttpBody();
                loginrequest144Body.ContentType = "application/json; charset=UTF-8";
                loginrequest144Body.InsertByteOrderMark = false;
                loginrequest144Body.BodyString = "{\"appId\":\"/providers/Microsoft.PowerApps/apps/4f542db4-e286-43e9-99f8-6082ef553c30\",\"fetchAppMetadata\":true}";
                loginrequest144.Body = loginrequest144Body;
                yield return loginrequest144;
                loginrequest144 = null;
           
                CrmRequest loginrequest200 = new CrmRequest("https://login.microsoftonline.com/common/oauth2/v2.0/token");
                loginrequest200.Method = "POST";
                loginrequest200.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest200.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest200.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest200.Headers.Add(new WebTestRequestHeader("content-type", "application/x-www-form-urlencoded;charset=utf-8"));
                loginrequest200.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest200.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                loginrequest200.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest200.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                FormPostHttpBody loginrequest200Body = new FormPostHttpBody();
                loginrequest200Body.FormPostParameters.Add("client_id", "3e62f81e-590b-425b-9531-cad6683656cf");
                loginrequest200Body.FormPostParameters.Add("scope", "https://contoso-perf2.crm.dynamics.com//.default openid profile offline_access");
                loginrequest200Body.FormPostParameters.Add("grant_type", "refresh_token");
                loginrequest200Body.FormPostParameters.Add("client_info", "1");
                loginrequest200Body.FormPostParameters.Add("x-client-SKU", "msal.js.browser");
                loginrequest200Body.FormPostParameters.Add("x-client-VER", "2.14.2");
                loginrequest200Body.FormPostParameters.Add("x-client-OS", "");
                loginrequest200Body.FormPostParameters.Add("x-client-CPU", "");
                loginrequest200Body.FormPostParameters.Add("x-ms-lib-capability", "retry-after, h429");
                loginrequest200Body.FormPostParameters.Add("x-client-current-telemetry", "2|61,0|,");
                loginrequest200Body.FormPostParameters.Add("x-client-last-telemetry", "2|0|||0,0");
                loginrequest200Body.FormPostParameters.Add("client-request-id", "48d17521-ddc7-489d-9422-8c4af684abdb");
                loginrequest200Body.FormPostParameters.Add("refresh_token", @"" + this.Context["refresh_token2"].ToString() + @"");
                loginrequest200.Body = loginrequest200Body;
                ExtractText er_accesstoken3 = new ExtractText();
                er_accesstoken3.StartsWith = "access_token\":\"";
                er_accesstoken3.EndsWith = "\"";
                er_accesstoken3.IgnoreCase = false;
                er_accesstoken3.UseRegularExpression = false;
                er_accesstoken3.Required = true;
                er_accesstoken3.ExtractRandomMatch = false;
                er_accesstoken3.Index = 0;
                er_accesstoken3.HtmlDecode = true;
                er_accesstoken3.SearchInHeaders = false;
                er_accesstoken3.ContextParameterName = "d365org_access_token";
                loginrequest200.ExtractValues += new EventHandler<ExtractionEventArgs>(er_accesstoken3.Extract);
                yield return loginrequest200;
                loginrequest200 = null;
                this.EndTransaction("1008.01_Authenticate to Canvas App");

                this.BeginTransaction("1008.02_Click Submit on Canvas App");               

                CrmRequest loginrequest23 = new CrmRequest("https://contoso-perf2.crm.dynamics.com//api/data/v9.0/workflows(6f26723b-e718-ec11-b6e6-002248084ec6)/Microsoft.Dynamics.CRM.install");
                loginrequest23.Method = "OPTIONS";
                loginrequest23.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Method", "POST"));
                loginrequest23.Headers.Add(new WebTestRequestHeader("Access-Control-Request-Headers", "authorization,client-session-id,content-type,request-id,x-ms-app-id,x-ms-client-request-id,x-ms-client-session-id,x-ms-user-agent"));
                loginrequest23.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                loginrequest23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                yield return loginrequest23;
                loginrequest23 = null;

                CrmRequest loginrequest18 = new CrmRequest("https://contoso-perf2.crm.dynamics.com//api/data/v9.0/workflows(6f26723b-e718-ec11-b6e6-002248084ec6)/Microsoft.Dynamics.CRM.install");
                loginrequest18.Method = "POST";
                loginrequest18.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + this.Context["access_token3"].ToString());
                loginrequest18.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest18.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "ab969cda-4659-4c92-8ba3-9b3ed83a7b9f"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("Request-Id", "aa3054fd-d787-40a4-9e52-bcfefffa5505"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "4f542db4-e286-43e9-99f8-6082ef553c30"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "aa3054fd-d787-40a4-9e52-bcfefffa5505"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("Client-Session-Id", "ab969cda-4659-4c92-8ba3-9b3ed83a7b9f"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps/3.21101.21 (Web; Player; isReactNative: false)"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest18.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "cross-site"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                StringHttpBody loginrequest18Body = new StringHttpBody();
                loginrequest18Body.ContentType = "application/json";
                loginrequest18Body.InsertByteOrderMark = false;
                loginrequest18Body.BodyString = @"{""connectionReferences"":""{\""shared_commondataserviceforapps\"":{\""connectionName\"":\""9c10e2ac0a944146ad4e6e7c1a7edc2d\"",\""id\"":\""/providers/microsoft.powerapps/apis/shared_commondataserviceforapps\""},\""shared_saperp\"":{\""connectionName\"":\""2e1e46100a554fff82498386dad1dc15\"",\""id\"":\""/providers/microsoft.powerapps/apis/shared_saperp\""}}""}";
                loginrequest18.Body = loginrequest18Body;
                yield return loginrequest18;
                loginrequest18 = null;
                this.AddCommentToResult("20_Click Submit on Canvas App");

                CrmRequest loginrequest19 = new CrmRequest("https://unitedstates.api.powerapps.com/api/invoke");
                loginrequest19.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Chromium\";v=\"94\", \"Google Chrome\";v=\"94\", \";Not A Brand\";v=\"99\""));
                loginrequest19.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "ab969cda-4659-4c92-8ba3-9b3ed83a7b9f"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("x-ms-client-app-version", "2021-10-19T22:33:24Z"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("x-ms-client-environment-id", "/providers/Microsoft.PowerApps/environments/8c49d479-ce93-4f93-a077-2786e2db4570"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("x-ms-client-app-id", "/providers/Microsoft.PowerApps/apps/4f542db4-e286-43e9-99f8-6082ef553c30"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("x-ms-client-object-id", "60df2fb7-2b8a-498d-a868-55a5efce8b56"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
                loginrequest19.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("x-ms-path-query", "/providers/Microsoft.PowerApps/connections?%2524expand=api&api-version=2020-06-01&%2524filter=environment+eq+%25278c49d479-ce93-4f93-a077-2786e2db4570%2527"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "d89a66d3-0d04-4585-917e-ed6f55561e4e"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps/3.21101.21 (Web; Player; isReactNative: false)"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("x-ms-client-tenant-id", "918079db-c902-4e29-b22c-9764410d0375"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("Origin", "https://apps.powerapps.com"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-site"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
                loginrequest19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
                yield return loginrequest19;
                loginrequest19 = null;

                this.EndTransaction("1008.02_Click Submit on Canvas App");

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
