//THINGS TO MANUALLY UPDATE IN THE CONVERTED TEST.

//1.) Replace occurances of entity guid with appropriate entity Id variable.  
//This is necessary if referencing a specific entity in inserts, updates, lookups, etc. 
//Example: replace guid with variable code such as "+accountId+" 

//2.) Remove occurances of ownerid in crmFormSubmitXml when doing creates and updates.   
//Owner will automatically be set to the random test user executing the test. 
//Example: Remove "<ownerid name="First name Last name" type="8">{27663C66-D9E7-E111-A076-00155D016F03}</ownerid>") 

//3.) Replace field values in crmFormSubmitXml with dynamic values when doing creates and updates.  
//Example: replace field value with Util function such as "+Utils.GetRandomString(5, 10)+" or "+Utils.GetRandomNumber(2, 5)+"  

//4.) Add code to extract entityid for new records and insert record to local EntityManager Database.
//Extract the newly created id out of the post reply
//int idIdx = requestN.lastResponse.BodyString.IndexOf(newIdMarker);
//string tempNewId = requestN.lastResponse.BodyString.Substring(idIdx + (newIdMarker.Length), 36);
//newId = new Guid(tempNewId).ToString();

//5.) Add code to extract WRPC Token and Timestamp for Inserts and Updates.
//Extract WRPC Token and Timestamp
//string[] TokenValues = Utils.ExtractWRPCToken(request1.lastResponse);
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
    public class zz_1001_SampleRunUserSummaryReportCoded : WebTestBase
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

        public zz_1001_SampleRunUserSummaryReportCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1001_SampleRunUserSummaryReportCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1001_SampleRunUserSummaryReportCoded_PostWebTest);
        }
        public void zz_1001_SampleRunUserSummaryReportCoded_PreWebTest(object sender, PreWebTestEventArgs e)
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

        }
        public void zz_1001_SampleRunUserSummaryReportCoded_PostWebTest(object sender, PostWebTestEventArgs e)
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
            this.BeginTransaction("1001.1_Navigate to Reports View");
            this.AddCommentToResult("Navigate to Reports View");

            CrmRequest request1 = new CrmRequest(thisURL + "/_root/homepage.aspx");
            request1.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDist", "Version=1.1"));
            request1.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDistEx", "MinContentInformation=1.0, MaxContentInformation=2.0"));
            request1.QueryStringParameters.Add("etc", "9100");
            request1.QueryStringParameters.Add("pagemode", "iframe");
            request1.QueryStringParameters.Add("sitemappath", "SFA|Tools|nav_reports");
            yield return request1;
            //<td class="ms-crm-List-NonDataCell" aria-checked="false" tabindex="0"><input type="checkbox" class="ms-crm-RowCheckBox" id="checkBox_{900E2665-D668-ED11-9561-000D3A314807}" title="User&#32;Summary" style=" "><span id="gridBodyTable_lblTooltip_{900E2665-D668-ED11-9561-000D3A314807}_24" role="presentation" 
            string reportmarker = "filename=\"User&#32;Summary.rdl\" oid=\"{";
            int idIdx = request1.lastResponse.BodyString.IndexOf(reportmarker);
            string usersummaryreportId = request1.lastResponse.BodyString.Substring(idIdx + (reportmarker.Length), 36);
            
            request1 = null;
            this.EndTransaction("1001.1_Navigate to Reports View");
            this.BeginTransaction("1001.2_Click On User Summary Report");
            this.AddCommentToResult("Click On User Summary Report");

            CrmRequest request2 = new CrmRequest(thisURL + "/crmreports/viewer/viewer.aspx");
            request2.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDist", "Version=1.1"));
            request2.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDistEx", "MinContentInformation=1.0, MaxContentInformation=2.0"));
            request2.QueryStringParameters.Add("action", "filter");
            request2.QueryStringParameters.Add("helpID", "User Summary.rdl");
            request2.QueryStringParameters.Add("id", "{"+usersummaryreportId+"}");
            yield return request2;
            request2 = null;
            this.AddCommentToResult("Click On User Summary Report");

            CrmRequest request3 = new CrmRequest(thisURL + "/AppWebServices/ResourceManager.asmx");
            request3.Method = "POST";
            request3.Headers.Add(new WebTestRequestHeader("ReferrerReqId", "74948a6f-6d0b-4e68-bdbe-b16c28df1e6c"));
            request3.Headers.Add(new WebTestRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2009/WebServices/GetString"));
            request3.Headers.Add(new WebTestRequestHeader("Content-Type", "text/xml; charset=utf-8"));
            StringHttpBody request3Body = new StringHttpBody();
            request3Body.ContentType = "text/xml; charset=utf-8";
            request3Body.InsertByteOrderMark = false;
            request3Body.BodyString = @"<?xml version=""1.0"" encoding=""utf-8"" ?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><GetString xmlns=""http://schemas.microsoft.com/crm/2009/WebServices""><key>Web.Tools.ServiceManagement.CaseSettings.Label_MoveRight</key></GetString></soap:Body></soap:Envelope>";
            request3.Body = request3Body;
            yield return request3;
            request3 = null;
            this.AddCommentToResult("Click On User Summary Report");

            CrmRequest request4 = new CrmRequest(thisURL + "/AppWebServices/ResourceManager.asmx");
            request4.Method = "POST";
            request4.Headers.Add(new WebTestRequestHeader("ReferrerReqId", "74948a6f-6d0b-4e68-bdbe-b16c28df1e6c"));
            request4.Headers.Add(new WebTestRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2009/WebServices/GetString"));
            request4.Headers.Add(new WebTestRequestHeader("Content-Type", "text/xml; charset=utf-8"));
            StringHttpBody request4Body = new StringHttpBody();
            request4Body.ContentType = "text/xml; charset=utf-8";
            request4Body.InsertByteOrderMark = false;
            request4Body.BodyString = @"<?xml version=""1.0"" encoding=""utf-8"" ?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><GetString xmlns=""http://schemas.microsoft.com/crm/2009/WebServices""><key>Web.Tools.ServiceManagement.CaseSettings.Label_MoveLeft</key></GetString></soap:Body></soap:Envelope>";
            request4.Body = request4Body;
            yield return request4;
            request4 = null;
            this.AddCommentToResult("Click On User Summary Report");

            CrmRequest request5 = new CrmRequest(thisURL + "/AppWebServices/AdvancedFind.asmx");
            request5.Method = "POST";
            request5.Headers.Add(new WebTestRequestHeader("ReferrerReqId", "74948a6f-6d0b-4e68-bdbe-b16c28df1e6c"));
            request5.Headers.Add(new WebTestRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2009/WebServices/GetDefaultAdvancedFindView"));
            request5.Headers.Add(new WebTestRequestHeader("Content-Type", "text/xml; charset=utf-8"));
            StringHttpBody request5Body = new StringHttpBody();
            request5Body.ContentType = "text/xml; charset=utf-8";
            request5Body.InsertByteOrderMark = false;
            request5Body.BodyString = @"<?xml version=""1.0"" encoding=""utf-8"" ?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><GetDefaultAdvancedFindView xmlns=""http://schemas.microsoft.com/crm/2009/WebServices""><entityName>systemuser</entityName><viewIDOnly>false</viewIDOnly></GetDefaultAdvancedFindView></soap:Body></soap:Envelope>";
            request5.Body = request5Body;
            yield return request5;
            request5 = null;
            this.AddCommentToResult("Click On User Summary Report");

            CrmRequest request6 = new CrmRequest(thisURL + "/AppWebServices/AdvancedFind.asmx");
            request6.Method = "POST";
            request6.Headers.Add(new WebTestRequestHeader("ReferrerReqId", "74948a6f-6d0b-4e68-bdbe-b16c28df1e6c"));
            request6.Headers.Add(new WebTestRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2009/WebServices/GetEntityFieldList"));
            request6.Headers.Add(new WebTestRequestHeader("Content-Type", "text/xml; charset=utf-8"));
            StringHttpBody request6Body = new StringHttpBody();
            request6Body.ContentType = "text/xml; charset=utf-8";
            request6Body.InsertByteOrderMark = false;
            request6Body.BodyString = @"<?xml version=""1.0"" encoding=""utf-8"" ?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><GetEntityFieldList xmlns=""http://schemas.microsoft.com/crm/2009/WebServices""><entityName>systemuser</entityName></GetEntityFieldList></soap:Body></soap:Envelope>";
            request6.Body = request6Body;
            yield return request6;
            request6 = null;
            this.AddCommentToResult("Click On User Summary Report");

            CrmRequest request7 = new CrmRequest(thisURL + "/AppWebServices/AdvancedFind.asmx");
            request7.Method = "POST";
            request7.Headers.Add(new WebTestRequestHeader("ReferrerReqId", "74948a6f-6d0b-4e68-bdbe-b16c28df1e6c"));
            request7.Headers.Add(new WebTestRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2009/WebServices/GetLinkedEntityList"));
            request7.Headers.Add(new WebTestRequestHeader("Content-Type", "text/xml; charset=utf-8"));
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "text/xml; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = @"<?xml version=""1.0"" encoding=""utf-8"" ?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><GetLinkedEntityList xmlns=""http://schemas.microsoft.com/crm/2009/WebServices""><entityName>systemuser</entityName></GetLinkedEntityList></soap:Body></soap:Envelope>";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;
            this.EndTransaction("1001.2_Click On User Summary Report");
            this.BeginTransaction("1001.3_Click on Run Report");
            this.AddCommentToResult("Click on Run Report");

            CrmRequest request8 = new CrmRequest(thisURL + "/CRMReports/viewer/filterxmltosummary.xsl");
            request8.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDist", "Version=1.1"));
            request8.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDistEx", "MinContentInformation=1.0, MaxContentInformation=2.0"));
            request8.QueryStringParameters.Add("ver", "1459569779");
            yield return request8;
            request8 = null;
            this.AddCommentToResult("Click on Run Report");

            CrmRequest request9 = new CrmRequest(thisURL + "/CRMReports/rsviewer/ReportViewer.aspx");
            request9.Method = "POST";
            request9.Headers.Add(new WebTestRequestHeader("Content-Type", "application/x-www-form-urlencoded"));
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("id", "{"+usersummaryreportId+"}");
            request9Body.FormPostParameters.Add("uniquename", "org98da03aa");
            request9Body.FormPostParameters.Add("iscustomreport", "false");
            request9Body.FormPostParameters.Add("reportnameonsrs", "1033{469462CE-08B2-46E7-97DA-7B8D4BA7F19A}");
            request9Body.FormPostParameters.Add("signatureid", "1b308213-d735-45ff-a7d1-7a7394a3b1b3");
            request9Body.FormPostParameters.Add("reporttypecode", "1");
            request9Body.FormPostParameters.Add("reportName", "User Summary");
            request9Body.FormPostParameters.Add("isScheduledReport", "false");
            request9Body.FormPostParameters.Add("CRM_Filter", @"<ReportFilter><ReportEntity paramname=""CRM_FilteredSystemUser"" displayname=""Users""><fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false""><entity name=""systemuser""><all-attributes/></entity></fetch></ReportEntity></ReportFilter>");
            request9Body.FormPostParameters.Add("CRM_FilterText", "");
            request9.Body = request9Body;
            yield return request9;
            request9 = null;
            this.EndTransaction("1001.3_Click on Run Report");
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
