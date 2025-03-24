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

    public class zz_1019_Portal_SearchandOpenWorkOrderCoded : WebTestBase
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
        private string portalusername = "portaluser1@domain.com";
        private string portalpassword = "samplepassword";

        public zz_1019_Portal_SearchandOpenWorkOrderCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1019_Portal_SearchandOpenWorkOrderCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1019_Portal_SearchandOpenWorkOrderCoded_PostWebTest);
        }
        public void zz_1019_Portal_SearchandOpenWorkOrderCoded_PreWebTest(object sender, PreWebTestEventArgs e)
        {

            //user = WebTestBase_PreWebTest(sender, e)[EntityNames.Users] as CRMEntity;
            //systemuserId = user["systemuserid"];

            //crmServer = user["serverbaseurl"];
            //e.WebTest.UserName = user["domainName"];
            //e.WebTest.Context.Add("user", user["domainName"]);
            //e.WebTest.Password = user["userpassword"];
            //orgId = new Guid(user["organizationid"]);
            //orgName = user["organizationname"];
            //thisURL = crmServer;
                

        }
        public void zz_1019_Portal_SearchandOpenWorkOrderCoded_PostWebTest(object sender, PostWebTestEventArgs e)
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

            this.BeginTransaction("1019.01_Browse to Portal");
            this.AddCommentToResult("01_Browse to Portal");

            CrmRequest request1 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/");
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request1.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "none"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-User", "?1"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
            yield return request1;
            request1 = null;
            this.AddCommentToResult("02_Browse to Portal");

            CrmRequest request2 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/SignIn");
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request2.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
            request2.QueryStringParameters.Add("returnUrl", "/");
            yield return request2;
            request2 = null;
            this.AddCommentToResult("03_Browse to Portal");

            CrmRequest request3 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_layout/tokenhtml");
            request3.ThinkTime = 20;
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request3.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request3.QueryStringParameters.Add("_", "1672853785458");
            ExtractHiddenFields er_requestverificationtoken = new ExtractHiddenFields();
            er_requestverificationtoken.Required = true;
            er_requestverificationtoken.HtmlDecode = true;
            er_requestverificationtoken.ContextParameterName = "requestverificationtoken";
            request3.ExtractValues += new EventHandler<ExtractionEventArgs>(er_requestverificationtoken.Extract);

            yield return request3;
            request3 = null;
            this.EndTransaction("1019.01_Browse to Portal");
            this.BeginTransaction("1019.02_Enter Credentials and Sign in");
            this.AddCommentToResult("05_Enter Credentials and Sign in");

            CrmRequest request4 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/SignIn");
            request4.ThinkTime = 19;
            request4.Method = "POST";
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request4.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
            request4.Headers.Add(new WebTestRequestHeader("Origin", "https://contosoperf.microsoftcrmportals.com"));
            request4.Headers.Add(new WebTestRequestHeader("Content-Type", "application/x-www-form-urlencoded"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-User", "?1"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
            request4.QueryStringParameters.Add("ReturnUrl", "/");
            FormPostHttpBody request4Body = new FormPostHttpBody();
            request4Body.FormPostParameters.Add("__RequestVerificationToken", Context["$HIDDENrequestverificationtoken.__RequestVerificationToken"].ToString());
            request4Body.FormPostParameters.Add("Username", "portalusername");
            request4Body.FormPostParameters.Add("PasswordValue", "portalpassword");
            request4Body.FormPostParameters.Add("RememberMe", "false");
            request4.Body = request4Body;
            yield return request4;
            request4 = null;
            this.EndTransaction("1019.02_Enter Credentials and Sign in");

            this.BeginTransaction("1019.03_Navigate to WorkOrders View");
            this.AddCommentToResult("06_Navigate to Work Order View");

            CrmRequest request5 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/workorder-management/");
            request5.ThinkTime = 3;
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request5.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-User", "?1"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
            yield return request5;
            request5 = null;
            this.AddCommentToResult("07_Navigate to Work Order View");

            CrmRequest request6 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/workorder-management/workorders/");
            request6.ThinkTime = 1;
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request6.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-User", "?1"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
            //Add these extraction rules before the yield return
            ExtractText erbase64_0_n53Sn = new ExtractText();
            erbase64_0_n53Sn.StartsWith = "data-view-layouts=\"";
            erbase64_0_n53Sn.EndsWith = "\"";
            erbase64_0_n53Sn.IgnoreCase = false;
            erbase64_0_n53Sn.UseRegularExpression = false;
            erbase64_0_n53Sn.Required = true;
            erbase64_0_n53Sn.ExtractRandomMatch = false;
            erbase64_0_n53Sn.Index = 0;
            erbase64_0_n53Sn.HtmlDecode = true;
            erbase64_0_n53Sn.SearchInHeaders = false;
            erbase64_0_n53Sn.ContextParameterName = "erbase64_0_n53Sn";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(erbase64_0_n53Sn.Extract);

            yield return request6;
            request6 = null;

            //Add this code after the yield return
            //"ViewName":"Portal - Active Work Orders"
            string base64secureconnection_0_n53Sn = Utils.extractviewfromlayoutbase64("Portal - Active Work Orders", this.Context["erbase64_0_n53Sn"].ToString());


            this.AddCommentToResult("08_Navigate to Work Order View");

            CrmRequest request7 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_layout/tokenhtml");
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request7.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request7.QueryStringParameters.Add("_", "1672853832183");
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(er_requestverificationtoken.Extract);
            yield return request7;
            request7 = null;
            this.AddCommentToResult("09_Navigate to Work Order View");

            CrmRequest request8 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_services/entity-grid-data.json/d78574f9-20c3-4dcc-8d8d-85cf5b7ac141");
            request8.ThinkTime = 14;
            request8.Method = "POST";
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request8.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=UTF-8"));
            request8.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request8.Headers.Add(new WebTestRequestHeader("Request-Id", "|tk6W.aZCkK"));
            request8.Headers.Add(new WebTestRequestHeader("__RequestVerificationToken", Context["$HIDDENrequestverificationtoken.__RequestVerificationToken"].ToString()));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request8.Headers.Add(new WebTestRequestHeader("Origin", "https://contosoperf.microsoftcrmportals.com"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=UTF-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"base64SecureConfiguration\":\"" + base64secureconnection_0_n53Sn + "\",\"sortExpression\":\"createdon DESC\",\"search\":\"\",\"page\":1,\"pageSize\":20,\"filter\":null,\"metaFilter\":null,\"timezoneOffset\":360,\"customParameters\":[]}";
            request8.Body = request8Body;
            yield return request8;
            string valuemarker = "Records\":[{\"Id\":\"";
            int idIdx = request8.lastResponse.BodyString.IndexOf(valuemarker);
            string workorderId = request8.lastResponse.BodyString.Substring(idIdx + (valuemarker.Length), 36);

            request8 = null;
            this.EndTransaction("1019.03_Navigate to WorkOrders View");

            this.BeginTransaction("1019.04_Search for WorkOrder Record");
            this.AddCommentToResult("10_Search for Work Order");

            CrmRequest request9 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_services/entity-grid-data.json/d78574f9-20c3-4dcc-8d8d-85cf5b7ac141");
            request9.ThinkTime = -49074745;
            request9.Method = "POST";
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\""));
            request9.Headers.Add(new WebTestRequestHeader("Request-Context", "appId=cid-v1:b0cd111a-a30f-48cd-8bd4-16e23c093af7"));
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request9.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=UTF-8"));
            request9.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request9.Headers.Add(new WebTestRequestHeader("Request-Id", "|tk6W.IEYlp"));
            request9.Headers.Add(new WebTestRequestHeader("__RequestVerificationToken", Context["$HIDDENrequestverificationtoken.__RequestVerificationToken"].ToString()));
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request9.Headers.Add(new WebTestRequestHeader("Origin", "https://contosoperf.microsoftcrmportals.com"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=UTF-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"base64SecureConfiguration\":\"" + base64secureconnection_0_n53Sn + "\",\"sortExpression\":\"createdon DESC\",\"search\":\"ready\",\"page\":1,\"pageSize\":20,\"filter\":null,\"metaFilter\":null,\"timezoneOffset\":360,\"customParameters\":[]}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            this.EndTransaction("1019.04_Search for WorkOrder Record");

            this.BeginTransaction("1019.05_Open WorkOrder Record");
            this.AddCommentToResult("11_Open Work Order Record");

            CrmRequest request10 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/en-US/workorder-management/workorders/");
            request10.ThinkTime = 1;
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not;A Brand\";v=\"99\", \"Google Chrome\";v=\"91\", \"Chromium\";v=\"91\""));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request10.Headers.Add(new WebTestRequestHeader("Upgrade-Insecure-Requests", "1"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "navigate"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-User", "?1"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "document"));
            request10.QueryStringParameters.Add("id", "" + workorderId + "");
            //Add these extraction rules before the yield return
            ExtractText erbase64_4_tj7Dg = new ExtractText();
            erbase64_4_tj7Dg.StartsWith = "data-view-layouts=\"";
            erbase64_4_tj7Dg.EndsWith = "\"";
            erbase64_4_tj7Dg.IgnoreCase = false;
            erbase64_4_tj7Dg.UseRegularExpression = false;
            erbase64_4_tj7Dg.Required = true;
            erbase64_4_tj7Dg.ExtractRandomMatch = false;
            erbase64_4_tj7Dg.Index = 4;
            erbase64_4_tj7Dg.HtmlDecode = true;
            erbase64_4_tj7Dg.SearchInHeaders = false;
            erbase64_4_tj7Dg.ContextParameterName = "erbase64_4_tj7Dg";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(erbase64_4_tj7Dg.Extract);
            ExtractText erbase64_5_ZDzbZ = new ExtractText();
            erbase64_5_ZDzbZ.StartsWith = "data-view-layouts=\"";
            erbase64_5_ZDzbZ.EndsWith = "\"";
            erbase64_5_ZDzbZ.IgnoreCase = false;
            erbase64_5_ZDzbZ.UseRegularExpression = false;
            erbase64_5_ZDzbZ.Required = true;
            erbase64_5_ZDzbZ.ExtractRandomMatch = false;
            erbase64_5_ZDzbZ.Index = 5;
            erbase64_5_ZDzbZ.HtmlDecode = true;
            erbase64_5_ZDzbZ.SearchInHeaders = false;
            erbase64_5_ZDzbZ.ContextParameterName = "erbase64_5_ZDzbZ";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(erbase64_5_ZDzbZ.Extract);
            ExtractText erbase64_9_1JbpH = new ExtractText();
            erbase64_9_1JbpH.StartsWith = "data-view-layouts=\"";
            erbase64_9_1JbpH.EndsWith = "\"";
            erbase64_9_1JbpH.IgnoreCase = false;
            erbase64_9_1JbpH.UseRegularExpression = false;
            erbase64_9_1JbpH.Required = true;
            erbase64_9_1JbpH.ExtractRandomMatch = false;
            erbase64_9_1JbpH.Index = 9;
            erbase64_9_1JbpH.HtmlDecode = true;
            erbase64_9_1JbpH.SearchInHeaders = false;
            erbase64_9_1JbpH.ContextParameterName = "erbase64_9_1JbpH";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(erbase64_9_1JbpH.Extract);
            ExtractText erbase64_10_nOiVx = new ExtractText();
            erbase64_10_nOiVx.StartsWith = "data-view-layouts=\"";
            erbase64_10_nOiVx.EndsWith = "\"";
            erbase64_10_nOiVx.IgnoreCase = false;
            erbase64_10_nOiVx.UseRegularExpression = false;
            erbase64_10_nOiVx.Required = true;
            erbase64_10_nOiVx.ExtractRandomMatch = false;
            erbase64_10_nOiVx.Index = 10;
            erbase64_10_nOiVx.HtmlDecode = true;
            erbase64_10_nOiVx.SearchInHeaders = false;
            erbase64_10_nOiVx.ContextParameterName = "erbase64_10_nOiVx";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(erbase64_10_nOiVx.Extract);
            ExtractText erbase64_11_uVnJD = new ExtractText();
            erbase64_11_uVnJD.StartsWith = "data-view-layouts=\"";
            erbase64_11_uVnJD.EndsWith = "\"";
            erbase64_11_uVnJD.IgnoreCase = false;
            erbase64_11_uVnJD.UseRegularExpression = false;
            erbase64_11_uVnJD.Required = true;
            erbase64_11_uVnJD.ExtractRandomMatch = false;
            erbase64_11_uVnJD.Index = 11;
            erbase64_11_uVnJD.HtmlDecode = true;
            erbase64_11_uVnJD.SearchInHeaders = false;
            erbase64_11_uVnJD.ContextParameterName = "erbase64_11_uVnJD";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(erbase64_11_uVnJD.Extract);

            yield return request10;
            request10 = null;

            //Add this code after the yield return
            //"ViewName":"Work Order Related View-Partner Portal"
            string base64secureconnection_4_tj7Dg = Utils.extractviewfromlayoutbase64("Work Order Related View-Partner Portal", this.Context["erbase64_4_tj7Dg"].ToString());
            //"ViewName":"Partner Portal - Related Knowledge Article WO"
            string base64secureconnection_5_ZDzbZ = Utils.extractviewfromlayoutbase64("Partner Portal - Related Knowledge Article WO", this.Context["erbase64_5_ZDzbZ"].ToString());
            //"ViewName":"Active Signatures"
            string base64secureconnection_9_1JbpH = Utils.extractviewfromlayoutbase64("Active Signatures", this.Context["erbase64_9_1JbpH"].ToString());
            //"ViewName":"Forms - Partner Portal Work Order"
            string base64secureconnection_10_nOiVx = Utils.extractviewfromlayoutbase64("Forms - Partner Portal Work Order", this.Context["erbase64_10_nOiVx"].ToString());
            //"ViewName":"Additional Partner Payments-Portal"
            string base64secureconnection_11_uVnJD = Utils.extractviewfromlayoutbase64("Additional Partner Payments-Portal", this.Context["erbase64_11_uVnJD"].ToString());

            this.AddCommentToResult("12_Open Work Order Record");

            CrmRequest request11 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_layout/tokenhtml");
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not;A Brand\";v=\"99\", \"Google Chrome\";v=\"91\", \"Chromium\";v=\"91\""));
            request11.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request11.QueryStringParameters.Add("_", "1623775513968");
            request11.ExtractValues += new EventHandler<ExtractionEventArgs>(er_requestverificationtoken.Extract);
            yield return request11;
            request11 = null;
            this.AddCommentToResult("13_Open Work Order Record");

            CrmRequest request12 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_portal/6d6b3012-e709-4c45-a00d-df4b3befc518/EntityActivity/GetActivities");
            request12.ThinkTime = -6;
            request12.Method = "POST";
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not;A Brand\";v=\"99\", \"Google Chrome\";v=\"91\", \"Chromium\";v=\"91\""));
            request12.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request12.Headers.Add(new WebTestRequestHeader("__RequestVerificationToken", Context["$HIDDENrequestverificationtoken.__RequestVerificationToken"].ToString()));
            request12.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json"));
            request12.Headers.Add(new WebTestRequestHeader("Origin", "https://contosoperf.microsoftcrmportals.com"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request12Body = new StringHttpBody();
            request12Body.ContentType = "application/json";
            request12Body.InsertByteOrderMark = false;
            request12Body.BodyString = "{\"regarding\":{\"Id\":\"" + workorderId + "\",\"LogicalName\":\"msdyn_workorder\",\"Name\":null,\"KeyAttributes\":[],\"RowVersion\":null},\"orders\":[{\"Attribute\":\"createdon\",\"Alias\":null,\"Direction\":null}],\"page\":1,\"pageSize\":\"15\"}";
            request12.Body = request12Body;
            yield return request12;
            request12 = null;
            this.AddCommentToResult("14_Open Work Order Record");

            CrmRequest request13 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_services/entity-subgrid-data.json/6d6b3012-e709-4c45-a00d-df4b3befc518");
            request13.ThinkTime = -5;
            request13.Method = "POST";
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not;A Brand\";v=\"99\", \"Google Chrome\";v=\"91\", \"Chromium\";v=\"91\""));
            request13.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request13.Headers.Add(new WebTestRequestHeader("__RequestVerificationToken", Context["$HIDDENrequestverificationtoken.__RequestVerificationToken"].ToString()));
            request13.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=UTF-8"));
            request13.Headers.Add(new WebTestRequestHeader("Origin", "https://contosoperf.microsoftcrmportals.com"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request13Body = new StringHttpBody();
            request13Body.ContentType = "application/json; charset=UTF-8";
            request13Body.InsertByteOrderMark = false;
            request13Body.BodyString = "{\"base64SecureConfiguration\":\"" + base64secureconnection_4_tj7Dg + "\",\"sortExpression\":\"new_trackingnumbertext ASC\",\"search\":null,\"page\":1,\"pageSize\":4,\"filter\":null,\"metaFilter\":null,\"timezoneOffset\":300,\"customParameters\":[],\"entityName\":\"msdyn_workorder\",\"entityId\":\"" + workorderId + "\"}";
            request13.Body = request13Body;
            yield return request13;
            request13 = null;
            this.AddCommentToResult("15_Open Work Order Record");

            CrmRequest request14 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_services/entity-subgrid-data.json/6d6b3012-e709-4c45-a00d-df4b3befc518");
            request14.ThinkTime = -7;
            request14.Method = "POST";
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not;A Brand\";v=\"99\", \"Google Chrome\";v=\"91\", \"Chromium\";v=\"91\""));
            request14.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request14.Headers.Add(new WebTestRequestHeader("__RequestVerificationToken", Context["$HIDDENrequestverificationtoken.__RequestVerificationToken"].ToString()));
            request14.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=UTF-8"));
            request14.Headers.Add(new WebTestRequestHeader("Origin", "https://contosoperf.microsoftcrmportals.com"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request14Body = new StringHttpBody();
            request14Body.ContentType = "application/json; charset=UTF-8";
            request14Body.InsertByteOrderMark = false;
            request14Body.BodyString = "{\"base64SecureConfiguration\":\"" + base64secureconnection_9_1JbpH + "\",\"sortExpression\":\"new_name ASC\",\"search\":null,\"page\":1,\"pageSize\":3,\"filter\":null,\"metaFilter\":null,\"timezoneOffset\":300,\"customParameters\":[],\"entityName\":\"msdyn_workorder\",\"entityId\":\"" + workorderId + "\"}";
            request14.Body = request14Body;
            yield return request14;
            request14 = null;
            this.AddCommentToResult("16_Open Work Order Record");

            CrmRequest request15 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_services/entity-subgrid-data.json/6d6b3012-e709-4c45-a00d-df4b3befc518");
            request15.ThinkTime = -6;
            request15.Method = "POST";
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not;A Brand\";v=\"99\", \"Google Chrome\";v=\"91\", \"Chromium\";v=\"91\""));
            request15.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request15.Headers.Add(new WebTestRequestHeader("__RequestVerificationToken", Context["$HIDDENrequestverificationtoken.__RequestVerificationToken"].ToString()));
            request15.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=UTF-8"));
            request15.Headers.Add(new WebTestRequestHeader("Origin", "https://contosoperf.microsoftcrmportals.com"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request15Body = new StringHttpBody();
            request15Body.ContentType = "application/json; charset=UTF-8";
            request15Body.InsertByteOrderMark = false;
            request15Body.BodyString = "{\"base64SecureConfiguration\":\"" + base64secureconnection_5_ZDzbZ + "\",\"sortExpression\":\"\",\"search\":null,\"page\":1,\"pageSize\":4,\"filter\":null,\"metaFilter\":null,\"timezoneOffset\":300,\"customParameters\":[],\"entityName\":\"msdyn_workorder\",\"entityId\":\"" + workorderId + "\"}";
            request15.Body = request15Body;
            yield return request15;
            request15 = null;
            this.AddCommentToResult("17_Open Work Order Record");

            CrmRequest request16 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_services/entity-subgrid-data.json/6d6b3012-e709-4c45-a00d-df4b3befc518");
            request16.Method = "POST";
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not;A Brand\";v=\"99\", \"Google Chrome\";v=\"91\", \"Chromium\";v=\"91\""));
            request16.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request16.Headers.Add(new WebTestRequestHeader("__RequestVerificationToken", Context["$HIDDENrequestverificationtoken.__RequestVerificationToken"].ToString()));
            request16.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=UTF-8"));
            request16.Headers.Add(new WebTestRequestHeader("Origin", "https://contosoperf.microsoftcrmportals.com"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request16Body = new StringHttpBody();
            request16Body.ContentType = "application/json; charset=UTF-8";
            request16Body.InsertByteOrderMark = false;
            request16Body.BodyString = "{\"base64SecureConfiguration\":\"" + base64secureconnection_10_nOiVx + "\",\"sortExpression\":\"datesent DESC\",\"search\":null,\"page\":1,\"pageSize\":4,\"filter\":null,\"metaFilter\":null,\"timezoneOffset\":300,\"customParameters\":[],\"entityName\":\"msdyn_workorder\",\"entityId\":\"" + workorderId + "\"}";
            request16.Body = request16Body;
            yield return request16;
            request16 = null;
            this.AddCommentToResult("18_Open Work Order Record");

            CrmRequest request17 = new CrmRequest("https://contosoperf.microsoftcrmportals.com/_services/entity-subgrid-data.json/6d6b3012-e709-4c45-a00d-df4b3befc518");
            request17.Method = "POST";
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not;A Brand\";v=\"99\", \"Google Chrome\";v=\"91\", \"Chromium\";v=\"91\""));
            request17.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request17.Headers.Add(new WebTestRequestHeader("__RequestVerificationToken", Context["$HIDDENrequestverificationtoken.__RequestVerificationToken"].ToString()));
            request17.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=UTF-8"));
            request17.Headers.Add(new WebTestRequestHeader("Origin", "https://contosoperf.microsoftcrmportals.com"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request17Body = new StringHttpBody();
            request17Body.ContentType = "application/json; charset=UTF-8";
            request17Body.InsertByteOrderMark = false;
            request17Body.BodyString = "{\"base64SecureConfiguration\":\"" + base64secureconnection_11_uVnJD + "\",\"sortExpression\":\"new_additionalpaymenttypecode ASC\",\"search\":null,\"page\":1,\"pageSize\":4,\"filter\":null,\"metaFilter\":null,\"timezoneOffset\":300,\"customParameters\":[],\"entityName\":\"msdyn_workorder\",\"entityId\":\"" + workorderId + "\"}";
            request17.Body = request17Body;
            yield return request17;
            request17 = null;
            this.EndTransaction("1019.05_Open WorkOrder Record");
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
