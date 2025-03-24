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
    public class zz_1020_CreateLeadRecordCoded : WebTestBase
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

        public zz_1020_CreateLeadRecordCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1020_CreateLeadRecordCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1020_CreateLeadRecordCoded_PostWebTest);
        }
        public void zz_1020_CreateLeadRecordCoded_PreWebTest(object sender, PreWebTestEventArgs e)
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
        public void zz_1020_CreateLeadRecordCoded_PostWebTest(object sender, PostWebTestEventArgs e)
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
            this.BeginTransaction("1020.01_Navigate to My Open Leads View");
            this.AddCommentToResult("01_Navigate to My Open Leads View");

            CrmRequest request1 = new CrmRequest(thisURL+"/api/data/v9.0/leads");
            request1.ThinkTime = -8;
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request1.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Grid; Source=Default"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "f0960e69-5984-4a7c-99e0-2677e286934c"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request1.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "00000000-0000-0000-00aa-000010001005"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "8ef43e4b-b5d2-4a9b-906d-66231fc4cd6d"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request1.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request1.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""50"" no-lock=""false""><entity name=""lead""><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""fullname""/><order attribute=""createdon"" descending=""true""/><filter type=""and""><condition attribute=""ownerid"" operator=""eq-userid""/><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><attribute name=""statuscode""/><attribute name=""createdon""/><attribute name=""subject""/><attribute name=""leadid""/><attribute name=""companyname""/></entity></fetch>");
            yield return request1;
            request1 = null;
            this.AddCommentToResult("02_Navigate to My Open Leads View");

            CrmRequest request2 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request2.ThinkTime = -1;
            request2.Method = "POST";
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request2.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "fa1308d5-c79f-4ed0-a144-4e668a39dff5"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request2.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "8ef43e4b-b5d2-4a9b-906d-66231fc4cd6d"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request2.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request2.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request2Body = new StringHttpBody();
            request2Body.ContentType = "application/json";
            request2Body.InsertByteOrderMark = false;
            request2Body.BodyString = "{\"items\":[{\"LastAccessedStr\":\"2022-03-29T14:58:17.695Z\",\"EntityTypeName\":\"lead\",\"ObjectId\":\"{00000000-0000-0000-00AA-000010001005}\",\"RecordType\":\"Grid\",\"Title\":\"My Open Leads\",\"PinStatus\":null,\"IconPath\":null,\"IsDeleted\":false,\"IsUserView\":false}]}";
            request2.Body = request2Body;
            yield return request2;
            request2 = null;
            this.AddCommentToResult("03_Navigate to My Open Leads View");

            CrmRequest request3 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request3.ThinkTime = 20;
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request3.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-metadatareductionlevel", "5"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "8f84c85c-776c-4c8b-9773-b4b247e23580"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request3.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "8ef43e4b-b5d2-4a9b-906d-66231fc4cd6d"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request3.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request3.QueryStringParameters.Add("@ClientMetadataQuery", "{\"MetadataType\":\"metadataversion\",\"GetDefault\":false,\"ChangedAfter\":\"2151247\",\"AppId\":\"a9a86d48-06ad-ec11-9841-000d3a3bc2af\"}");
            request3.QueryStringParameters.Add("api-version", "9.1");
            yield return request3;
            request3 = null;
            this.EndTransaction("1020.01_Navigate to My Open Leads View");
            this.BeginTransaction("1020.02_Click New Lead Button");
            this.AddCommentToResult("04_Click New Lead Button");

            CrmRequest request4 = new CrmRequest(thisURL+"/api/data/v9.0/roles");
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "d7ee8650-6e9b-4088-8cfd-6b87a60426d6"));
            request4.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "1830220a-4b1b-44d1-a17c-995b35aa9f45"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request4.QueryStringParameters.Add("$filter", "_roletemplateid_value eq \'627090ff-40a3-4053-8790-584edc5be201\'");
            request4.QueryStringParameters.Add("$select", "roleid");
            yield return request4;
            request4 = null;
            this.AddCommentToResult("05_Click New Lead Button");

            CrmRequest request5 = new CrmRequest(thisURL+"/api/data/v9.0/systemusers("+systemuserId+")");
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request5.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request5.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "1c39bb28-fae5-4d91-b5d3-33c6884f8d66"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request5.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "d7ee8650-6e9b-4088-8cfd-6b87a60426d6"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request5.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request5.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"2150652\""));
            yield return request5;
            request5 = null;
            this.AddCommentToResult("06_Click New Lead Button");

            CrmRequest request6 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request6.ThinkTime = 1;
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request6.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "b18b11ee-7f09-40db-92d5-942f8a26864e"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request6.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "d7ee8650-6e9b-4088-8cfd-6b87a60426d6"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request6.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request6.QueryStringParameters.Add("fetchXml", "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><condition attribute=\"actioncardid\" operator=\"eq\" value=\"fa147913-304d-4aa5-94aa-e2b0cdeb4239\"/></filter></entity></fetch>");
            yield return request6;
            request6 = null;
            this.AddCommentToResult("07_Click New Lead Button");

            CrmRequest request7 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request7.ThinkTime = 20;
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request7.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-metadatareductionlevel", "5"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "12162a72-7811-410d-b0de-aad87c3d23a0"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request7.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "d05e4a91-083d-4d1f-ab3c-33f2a20708e7"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request7.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request7.QueryStringParameters.Add("@ClientMetadataQuery", "{\"MetadataType\":\"metadataversion\",\"GetDefault\":false,\"ChangedAfter\":\"2151247\",\"AppId\":\"a9a86d48-06ad-ec11-9841-000d3a3bc2af\"}");
            request7.QueryStringParameters.Add("api-version", "9.1");
            yield return request7;
            request7 = null;
            this.EndTransaction("1020.02_Click New Lead Button");
            this.BeginTransaction("1020.03_Click Save to Create Lead Record");
            this.AddCommentToResult("08_Click Save to Create Lead Record");

            CrmRequest request8 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request8.Method = "POST";
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request8.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "fe579b71-b312-4bfb-9765-0d4ed47fb360"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request8.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566010201"));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request8.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "multipart/mixed;boundary=batch_1648566010201";
            request8Body.InsertByteOrderMark = false;
            string leadId = Guid.NewGuid().ToString();
            request8Body.BodyString = "--batch_1648566010201\nContent-Type: multipart/mixed;boundary=changeset_1648566010201\n\n--changeset_1648566010201\nContent-Type: application/http\nContent-Transfer-Encoding: binary\nContent-ID: 1\n\nPOST /api/data/v9.0/leads HTTP/1.1\nAccept: application/json\nMSCRM.SuppressDuplicateDetection: false\nContent-Type: application/json\nPrefer: odata.include-annotations=\"*\"\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 02c2f847-012e-4669-9d9a-dc28c0c3bab2\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 56a559d5-798c-42e8-bb32-c5b45257d70a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n{\"companyname\":\"ltlead"+Utils.GetRandomString(5,10)+ "\",\"mobilephone\":\""+Utils.GetRandomString(5,10)+"\",\"emailaddress1\":\"ltlead" + Utils.GetRandomString(5, 10) + "@cebenchmark.onmicrosoft.com\",\"lastname\":\"ltlead" + Utils.GetRandomString(5, 10) + "\",\"firstname\":\"ltlead" + Utils.GetRandomString(5, 10) + "\",\"subject\":\"ltlead" + Utils.GetRandomString(5, 10) + "\",\"decisionmaker\":false,\"leadqualitycode\":2,\"donotpostalmail\":false,\"donotphone\":false,\"donotbulkemail\":false,\"followemail\":true,\"donotemail\":false,\"preferredcontactmethodcode\":1,\"donotsendmm\":false,\"statuscode\":1,\"statecode\":0,\"ownerid@odata.bind\":\"/systemusers("+systemuserId+")\",\"ownerid@OData.Community.Display.V1.FormattedValue\":\"crmusr four\",\"processid\":\"00000000-0000-0000-0000-000000000000\",\"leadid\":\""+leadId+"\"}\n--changeset_1648566010201\nContent-Type: application/http\nContent-Transfer-Encoding: binary\nContent-ID: 2\n\nPOST /api/data/v9.0/leadtoopportunitysalesprocesses HTTP/1.1\nAccept: application/json\nMSCRM.SuppressDuplicateDetection: false\nContent-Type: application/json\nPrefer: odata.include-annotations=\"*\"\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 335a346e-b729-45fa-9f26-840c1dcc5a89\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 56a559d5-798c-42e8-bb32-c5b45257d70a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n{\"processid@odata.bind\":\"/workflows(919e14d1-6489-4852-abd0-a63a6ecaac5d)\",\"leadId@odata.bind\":\"/leads("+leadId+")\",\"traversedpath\":\"f99b4d48-7aad-456e-864a-8e7d543f7495\",\"activestageid@odata.bind\":\"/processstages(f99b4d48-7aad-456e-864a-8e7d543f7495)\"}\n--changeset_1648566010201--\n\n--batch_1648566010201--\r\n\0";
            request8.Body = request8Body;
            yield return request8;
           
            request8 = null;
            this.AddCommentToResult("09_Click Save to Create Lead Record");

            CrmRequest request9 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request9.Method = "POST";
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request9.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "fe1b4085-272b-4296-b1d4-dbd537a51551"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request9.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566012779"));
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request9.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "multipart/mixed;boundary=batch_1648566012779";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "--batch_1648566012779\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/leads("+leadId+")?$select=subject,firstname,middlename,lastname,jobtitle,telephone1,mobilephone,emailaddress1,companyname,websiteurl,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,address1_composite,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,businesscard,description,industrycode,revenue,numberofemployees,sic,_transactioncurrencyid_value,_campaignid_value,donotsendmm,lastusedincampaign,preferredcontactmethodcode,donotemail,followemail,donotbulkemail,donotphone,donotpostalmail,leadsourcecode,leadqualitycode,statuscode,_ownerid_value,_parentaccountid_value,_parentaccountid_value,_parentcontactid_value,_parentcontactid_value,businesscardattributes,statecode,leadid,entityimage_url,fullname,businesscard,businesscardattributes,statecode,purchasetimeframe,budgetamount,purchaseprocess,decisionmaker HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: fe845874-2477-49b2-8b0d-c992c92798fc\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 56a559d5-798c-42e8-bb32-c5b45257d70a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566012779\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: a2a6278f-1b6b-4a3e-9031-eaa416450c11\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 56a559d5-798c-42e8-bb32-c5b45257d70a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566012779\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 8fc1a5c7-4e12-4c8d-bf61-c492890f4e19\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 56a559d5-798c-42e8-bb32-c5b45257d70a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566012779--\r\n\0";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;
            this.AddCommentToResult("10_Click Save to Create Lead Record");

            CrmRequest request10 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request10.ThinkTime = 1;
            request10.Method = "POST";
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request10.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "de319644-2e1e-47dd-9ff6-c55c957d0d52"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request10.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566013518"));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request10.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "multipart/mixed;boundary=batch_1648566013518";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = "--batch_1648566013518\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/leads("+leadId+")?$select=subject,firstname,middlename,lastname,jobtitle,telephone1,mobilephone,emailaddress1,companyname,websiteurl,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,businesscard,description,industrycode,revenue,numberofemployees,sic,_transactioncurrencyid_value,_campaignid_value,donotsendmm,lastusedincampaign,preferredcontactmethodcode,donotemail,followemail,donotbulkemail,donotphone,donotpostalmail,leadsourcecode,leadqualitycode,statuscode,_ownerid_value,_parentaccountid_value,_parentcontactid_value,businesscardattributes,statecode,purchasetimeframe,budgetamount,purchaseprocess,decisionmaker,statecode,leadid,entityimage_url,fullname HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: ac3ea798-ff21-41ac-8412-509ae05d222b\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 56a559d5-798c-42e8-bb32-c5b45257d70a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566013518\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 9b0355d8-25be-4706-bdfc-b2050afff569\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 56a559d5-798c-42e8-bb32-c5b45257d70a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566013518\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 636446a7-5c0d-42b7-ab72-f183a0eb260b\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 56a559d5-798c-42e8-bb32-c5b45257d70a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566013518--\r\n\0";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;
            this.AddCommentToResult("11_Click Save to Create Lead Record");

            CrmRequest request11 = new CrmRequest(thisURL+"/api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)");
            request11.ThinkTime = -1;
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request11.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "e6cc2f1e-8bc8-495a-ab8b-8df6dba42da5"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request11.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request11.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=*"));
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request11.Headers.Add(new WebTestRequestHeader("content-type", "application/json; charset=utf-8"));
            request11.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request11.QueryStringParameters.Add("@id", "null");
            request11.QueryStringParameters.Add("@xml", "\'<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\" returntotalrecordcount=\"false\" page=\"1\" count=\"10\" no-lock=\"true\"><entity name=\"activitypointer\"><attribute name=\"subject\"/><attribute name=\"activitytypecode\"/><attribute name=\"statecode\"/><attribute name=\"statuscode\"/><attribute name=\"activityid\"/><attribute name=\"description\"/><attribute name=\"modifiedby\"/><attribute name=\"ownerid\"/><attribute name=\"allparties\"/><attribute name=\"modifiedon\"/><attribute name=\"scheduledstart\"/><attribute name=\"createdby\"/><attribute name=\"scheduledend\"/><order attribute=\"modifiedon\" descending=\"true\"/><order attribute=\"activityid\" descending=\"false\"/><link-entity name=\"email\" from=\"activityid\" to=\"activityid\" alias=\"email\" link-type=\"outer\"><attribute name=\"senton\" alias=\"EmailSentOn\"/><attribute name=\"delayedemailsendtime\" alias=\"DelayedemailSendTime\"/><attribute name=\"lastopenedtime\" alias=\"LastOpenedTime\"/><attribute name=\"isemailfollowed\" alias=\"IsEmailFollowed\"/><attribute name=\"baseconversationindexhash\" alias=\"BaseConversationIndexHash\"/></link-entity><link-entity name=\"opportunityclose\" from=\"activityid\" to=\"activityid\" alias=\"opportunityclose\" link-type=\"outer\"><attribute name=\"actualrevenue\" alias=\"opportunityclose_actualrevenue\"/></link-entity><link-entity name=\"phonecall\" from=\"activityid\" to=\"activityid\" alias=\"phonecall\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"phonecall_directioncode\"/></link-entity><link-entity name=\"letter\" from=\"activityid\" to=\"activityid\" alias=\"letter\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"letter_directioncode\"/></link-entity><link-entity name=\"fax\" from=\"activityid\" to=\"activityid\" alias=\"fax\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"fax_directioncode\"/></link-entity><link-entity name=\"activityparty\" from=\"activityid\" to=\"activityid\" alias=\"activityparty\" link-type=\"in\"><filter type=\"and\"><filter type=\"and\"><condition attribute=\"partyid\" operator=\"eq\" value=\""+leadId+"\"/><condition attribute=\"partyobjecttypecode\" operator=\"eq\" value=\"4\"/></filter></filter></link-entity><filter type=\"and\"><condition attribute=\"activitytypecode\" operator=\"in\"><value>4402</value><value>4206</value><value>4202</value><value>4204</value><value>4207</value><value>4201</value><value>4208</value><value>4209</value><value>4210</value><value>4211</value><value>4251</value><value>4216</value><value>4212</value><value>10315</value><value>10325</value><value>10327</value></condition></filter></entity></fetch>\'");
            request11.QueryStringParameters.Add("@rollupType", "-1");
            yield return request11;
            request11 = null;
            this.AddCommentToResult("12_Click Save to Create Lead Record");

            CrmRequest request12 = new CrmRequest(thisURL+"/api/data/v9.0/annotations");
            request12.ThinkTime = -1;
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request12.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=RetrieveMultipleOther; Source=Default"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "52a4832d-9d8a-4887-b9fa-a2c419d2e82f"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request12.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "undefined"));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request12.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request12.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""true"" returntotalrecordcount=""false"" page=""1"" count=""10"" no-lock=""false""><entity name=""annotation""><attribute name=""annotationid""/><attribute name=""subject""/><attribute name=""notetext""/><attribute name=""filename""/><attribute name=""filesize""/><attribute name=""isdocument""/><attribute name=""createdby""/><attribute name=""createdon""/><attribute name=""modifiedby""/><attribute name=""modifiedon""/><attribute name=""mimetype""/><order attribute=""modifiedon"" descending=""true""/><order attribute=""annotationid"" descending=""false""/><link-entity name=""systemuser"" from=""systemuserid"" to=""modifiedby"" alias=""systemuser"" link-type=""outer""><attribute name=""entityimage_url""/><attribute name=""systemuserid""/><attribute name=""fullname""/></link-entity><filter type=""and""><filter type=""and""><condition attribute=""objectid"" operator=""eq"" value="""+leadId+@"""/></filter></filter></entity></fetch>");
            yield return request12;
            request12 = null;
            this.AddCommentToResult("13_Click Save to Create Lead Record");

            CrmRequest request13 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request13.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "3e609b9e-69e9-42c4-8c40-fac185b75c8c"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request13.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request13.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request13.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""leadcompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""lead"" from=""leadid"" to=""leadid"" alias=""bb""> <filter type=""and"">  <condition attribute=""leadid"" operator=""eq"" uitype=""lead"" value="""+leadId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request13;
            request13 = null;
            this.AddCommentToResult("14_Click Save to Create Lead Record");

            CrmRequest request14 = new CrmRequest(thisURL+"/api/data/v9.0/msdyn_postconfigs");
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request14.QueryStringParameters.Add("$select", "msdyn_entityname,statecode");
            request14.QueryStringParameters.Add("$filter", "msdyn_entityname eq \'lead\'");
            yield return request14;
            request14 = null;
            this.AddCommentToResult("15_Click Save to Create Lead Record");

            CrmRequest request15 = new CrmRequest(thisURL+"/api/data/v9.0/IsPdfEnabledForEntity");
            request15.ThinkTime = -1;
            request15.Method = "POST";
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request15.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "288fb0e7-a786-4dca-9fb0-1f0c862dcd9e"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request15.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request15.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request15.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request15Body = new StringHttpBody();
            request15Body.ContentType = "application/json";
            request15Body.InsertByteOrderMark = false;
            request15Body.BodyString = "{\"PDFSettingsEntityName\":\"lead\"}";
            request15.Body = request15Body;
            yield return request15;
            request15 = null;
            this.AddCommentToResult("16_Click Save to Create Lead Record");

            CrmRequest request16 = new CrmRequest(thisURL+"/api/data/v9.0/msdyn_GetSalesAccelerationConfigurationStatus");
            request16.ThinkTime = -1;
            request16.Method = "POST";
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request16.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "6ad5550a-4cda-495b-8a17-c49069e5e71c"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request16.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request16.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request16.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request16Body = new StringHttpBody();
            request16Body.ContentType = "application/json";
            request16Body.InsertByteOrderMark = false;
            request16Body.BodyString = "{}";
            request16.Body = request16Body;
            yield return request16;
            request16 = null;
            this.AddCommentToResult("17_Click Save to Create Lead Record");

            CrmRequest request17 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request17.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "9ff406a6-c2ee-42a7-bcec-648841c35461"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request17.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request17.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request17.QueryStringParameters.Add("fetchXml", "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><condition attribute=\"actioncardid\" operator=\"eq\" value=\"fa147913-304d-4aa5-94aa-e2b0cdeb4239\"/></filter></entity></fetch>");
            yield return request17;
            request17 = null;
            this.AddCommentToResult("18_Click Save to Create Lead Record");

            CrmRequest request18 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request18.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "9fa37e1f-436b-411f-b78f-d640db4ce8e3"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request18.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request18.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request18.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""lead"" from=""leadid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""leadid"" operator=""eq"" uitype=""lead"" value="""+leadId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request18;
            request18 = null;
            this.AddCommentToResult("19_Click Save to Create Lead Record");

            CrmRequest request19 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request19.ThinkTime = 1;
            request19.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request19.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "44c5a7c1-5b1b-4f4f-a29d-8b51e95afef1"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request19.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request19.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request19.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "56a559d5-798c-42e8-bb32-c5b45257d70a"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request19.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request19.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""lead"" from=""leadid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""leadid"" operator=""eq"" uitype=""lead"" value="""+leadId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request19;
            request19 = null;
            this.AddCommentToResult("20_Click Save to Create Lead Record");

            CrmRequest request20 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request20.Method = "POST";
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request20.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "8308706f-04a9-4fd6-9387-9a5685a7f11e"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request20.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "dea6be84-71a8-4487-b1d3-e069bac1bcd9"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request20.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request20.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request20Body = new StringHttpBody();
            request20Body.ContentType = "application/json";
            request20Body.InsertByteOrderMark = false;
            request20Body.BodyString = "{\"items\":[{\"LastAccessedStr\":\"2022-03-29T15:00:13.505Z\",\"EntityTypeName\":\"lead\",\"ObjectId\":\"{"+leadId+"}\",\"RecordType\":\"Entity\",\"Title\":\"ltlead959 ltlead959\",\"PinStatus\":null,\"IconPath\":null,\"IsDeleted\":false,\"IsUserView\":null}]}";
            request20.Body = request20Body;
            yield return request20;
            request20 = null;
            this.EndTransaction("1020.03_Click Save to Create Lead Record");

            //Insert Lead into ltlead table for future processing
            using (System.Data.SqlClient.SqlConnection emsqlCon = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
            {
                emsqlCon.Open();
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = emsqlCon;
                cmd.CommandText = "insert into ltlead values ('" + leadId + "','" + systemuserId + "','0','0','"+Guid.Empty.ToString()+"')";
                cmd.ExecuteNonQuery();
            }
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
