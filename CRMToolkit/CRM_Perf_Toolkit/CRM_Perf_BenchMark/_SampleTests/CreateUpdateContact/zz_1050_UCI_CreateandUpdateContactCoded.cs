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

//7.) UCI: Add code to extract record from UCI View/QuickFind/Lookup. Update Code to reflect primarykey of entity
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
    public class zz_1050_UCI_CreateandUpdateContactCoded : WebTestBase
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

        public zz_1050_UCI_CreateandUpdateContactCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1050_UCI_CreateandUpdateContactCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1050_UCI_CreateandUpdateContactCoded_PostWebTest);
        }
        public void zz_1050_UCI_CreateandUpdateContactCoded_PreWebTest(object sender, PreWebTestEventArgs e)
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
        public void zz_1050_UCI_CreateandUpdateContactCoded_PostWebTest(object sender, PostWebTestEventArgs e)
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
            this.BeginTransaction("1050.01_Navigate to My Active Contact View");
            this.AddCommentToResult("Navigate to My Active Contact View");

            CrmRequest request1 = new CrmRequest(thisURL+"/api/data/v9.0/contacts");
            request1.ThinkTime = 1;
            request1.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request1.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request1.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request1.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request1.Headers.Add(new WebTestRequestHeader("client-activity-id", "d0fd4621-d89e-4a10-a63b-807b16f1ebbc"));
            request1.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request1.Headers.Add(new WebTestRequestHeader("request-id", "9a851ee6-a8be-4227-9c1e-219e09b5e423"));
            request1.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request1.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""50"" no-lock=""false""><entity name=""contact""><attribute name=""statecode""/><attribute name=""entityimage_url""/><attribute name=""fullname""/><order attribute=""fullname"" descending=""false""/><attribute name=""parentcustomerid""/><filter type=""and""><condition attribute=""ownerid"" operator=""eq-userid""/><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><attribute name=""telephone1""/><attribute name=""emailaddress1""/><attribute name=""contactid""/></entity></fetch>");
            yield return request1;
            request1 = null;
            this.AddCommentToResult("Navigate to My Active Contact View");

            CrmRequest request2 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request2.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request2.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request2.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request2.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request2.Headers.Add(new WebTestRequestHeader("client-activity-id", "fe296eab-e572-4726-a117-8bd73e67988d"));
            request2.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request2.Headers.Add(new WebTestRequestHeader("request-id", "d7ad0aee-1f8a-4b3e-b6de-ac5a9fe89c43"));
            request2.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request2.QueryStringParameters.Add("@ClientMetadataQuery", @"{""MetadataType"":""metadataversion"",""MetadataSubtype"":null,""EntityLogicalName"":null,""MetadataId"":null,""MetadataNames"":[],""GetDefault"":false,""DependencyDepth"":""OnDemandWithoutContext"",""ChangedAfter"":""3975641"",""Exclude"":[],""AppId"":""b8ac196d-94c5-e811-a96c-000d3a16a650""}");
            request2.QueryStringParameters.Add("umv", "3975641");
            request2.QueryStringParameters.Add("mv", "3976327");
            yield return request2;
            request2 = null;
            this.AddCommentToResult("Navigate to My Active Contact View");

            CrmRequest request3 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request3.ThinkTime = 20;
            request3.Method = "POST";
            request3.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request3.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request3.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request3.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request3.Headers.Add(new WebTestRequestHeader("client-activity-id", "fe296eab-e572-4726-a117-8bd73e67988d"));
            request3.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request3.Headers.Add(new WebTestRequestHeader("request-id", "42f91041-309f-430a-8231-e1c1f2d1db79"));
            request3.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request3.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            StringHttpBody request3Body = new StringHttpBody();
            request3Body.ContentType = "application/json";
            request3Body.InsertByteOrderMark = false;
            request3Body.BodyString = "{\"items\":[{\"EntityTypeName\":\"contact\",\"LastAccessedStr\":\"2018-11-01T18:20:20.132Z\",\"ObjectId\":\"{00000000-0000-0000-00AA-000010001003}\",\"PinStatus\":false,\"RecordType\":\"Grid\",\"IsUserView\":false,\"Title\":\"My Active Contacts\",\"IsDeleted\":false}]}";
            request3.Body = request3Body;
            yield return request3;
            request3 = null;
            this.EndTransaction("1050.01_Navigate to My Active Contact View");
            this.BeginTransaction("1050.02_Click New Contact Button");
            this.AddCommentToResult("Click New Contact Button");

            CrmRequest request4 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request4.Method = "POST";
            request4.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request4.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request4.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request4.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request4.Headers.Add(new WebTestRequestHeader("client-activity-id", "a3271be8-2c2c-4ccc-9c04-7ed284c4430f"));
            request4.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request4.Headers.Add(new WebTestRequestHeader("request-id", "a05175ce-90d5-4f32-a065-aba952c6ec32"));
            request4.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096454681"));
            StringHttpBody request4Body = new StringHttpBody();
            request4Body.ContentType = "multipart/mixed;boundary=batch_1541096454681";
            request4Body.InsertByteOrderMark = false;
            request4Body.BodyString = "--batch_1541096454681\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22false%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%225%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22opportunity%22%3E%3Corder%20attribute%3D%22statecode%22%20descending%3D%22false%22%2F%3E%3Corder%20attribute%3D%22estimatedclosedate%22%20descending%3D%22true%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Cfilter%20type%3D%22or%22%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%220%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22actualclosedate%22%20operator%3D%22last-x-months%22%20value%3D%2212%22%2F%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%221%22%2F%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22estimatedvalue%22%2F%3E%3Cattribute%20name%3D%22estimatedclosedate%22%2F%3E%3Cattribute%20name%3D%22actualvalue%22%2F%3E%3Cattribute%20name%3D%22actualclosedate%22%2F%3E%3Cattribute%20name%3D%22name%22%2F%3E%3Cattribute%20name%3D%22opportunityid%22%2F%3E%3Cattribute%20name%3D%22parentcontactid%22%2F%3E%3Cattribute%20name%3D%22parentaccountid%22%2F%3E%3Cattribute%20name%3D%22modifiedon%22%2F%3E%3Corder%20attribute%3D%22modifiedon%22%20descending%3D%22true%22%2F%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: a3271be8-2c2c-4ccc-9c04-7ed284c4430f\nRequest-Id: 1eadfff0-f678-479f-b255-51c9a8c50565\n\n--batch_1541096454681--\r\n\0";
            request4.Body = request4Body;
            yield return request4;
            request4 = null;
            this.AddCommentToResult("Click New Contact Button");

            CrmRequest request5 = new CrmRequest(thisURL+"/api/data/v9.0/incidents");
            request5.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request5.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request5.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request5.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request5.Headers.Add(new WebTestRequestHeader("client-activity-id", "a3271be8-2c2c-4ccc-9c04-7ed284c4430f"));
            request5.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request5.Headers.Add(new WebTestRequestHeader("request-id", "04d7239f-b9e8-4f5a-afdf-982b82af9c6c"));
            request5.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request5.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""5"" no-lock=""false""><entity name=""incident""><order attribute=""statecode"" descending=""true""/><order attribute=""prioritycode"" descending=""false""/><filter type=""and""><condition attribute=""createdon"" operator=""last-x-months"" value=""12""/></filter><attribute name=""statecode""/><attribute name=""createdon""/><attribute name=""prioritycode""/><attribute name=""title""/><attribute name=""incidentid""/><attribute name=""caseorigincode""/><attribute name=""ticketnumber""/><attribute name=""customerid""/><attribute name=""ownerid""/><attribute name=""statuscode""/></entity></fetch>");
            yield return request5;
            request5 = null;
            this.AddCommentToResult("Click New Contact Button");

            CrmRequest request6 = new CrmRequest(thisURL+"/api/data/v9.0/entitlements");
            request6.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request6.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request6.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request6.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request6.Headers.Add(new WebTestRequestHeader("client-activity-id", "a3271be8-2c2c-4ccc-9c04-7ed284c4430f"));
            request6.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request6.Headers.Add(new WebTestRequestHeader("request-id", "161e9ef4-5728-4911-9eb8-2e7bbb3de2be"));
            request6.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request6.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""entitlement""><attribute name=""name""/><order attribute=""name"" descending=""false""/><attribute name=""statecode""/><attribute name=""customerid""/><attribute name=""startdate""/><attribute name=""enddate""/><attribute name=""slaid""/><attribute name=""allocationtypecode""/><attribute name=""remainingterms""/><attribute name=""entitlementid""/></entity></fetch>");
            yield return request6;
            request6 = null;
            this.AddCommentToResult("Click New Contact Button");

            CrmRequest request7 = new CrmRequest(thisURL+"//api/data/v9.0/webresourceset");
            request7.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request7.QueryStringParameters.Add("$select", "content");
            request7.QueryStringParameters.Add("$filter", "displayname eq \'MapControlIframeContent.html\'");
            yield return request7;
            request7 = null;
            this.AddCommentToResult("Click New Contact Button");

            CrmRequest request8 = new CrmRequest(thisURL+"/api/data/v9.0/systemusers("+systemuserId+")");
            request8.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request8.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request8.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request8.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request8.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request8.Headers.Add(new WebTestRequestHeader("client-activity-id", "a3271be8-2c2c-4ccc-9c04-7ed284c4430f"));
            request8.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request8.Headers.Add(new WebTestRequestHeader("request-id", "90924cd9-1a40-4d12-b93e-b8efcc4ffdbd"));
            request8.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request8.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"3810056\""));
            request8.QueryStringParameters.Add("$select", "entityimage");
            yield return request8;
            request8 = null;
            this.AddCommentToResult("Click New Contact Button");

            CrmRequest request9 = new CrmRequest(thisURL+"/api/data/v9.0/organizations("+orgId+")");
            request9.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request9.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request9.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request9.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request9.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request9.Headers.Add(new WebTestRequestHeader("client-activity-id", "a3271be8-2c2c-4ccc-9c04-7ed284c4430f"));
            request9.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request9.Headers.Add(new WebTestRequestHeader("request-id", "6991f1c3-74f6-46c8-89c2-8b8170a3efd7"));
            request9.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request9.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"3976562\""));
            request9.QueryStringParameters.Add("$select", "yammernetworkpermalink,yammergroupid,yammeroauthaccesstokenexpired");
            yield return request9;
            request9 = null;
            this.AddCommentToResult("Click New Contact Button");

            CrmRequest request10 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request10.ThinkTime = 20;
            request10.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request10.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request10.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request10.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request10.Headers.Add(new WebTestRequestHeader("client-activity-id", "2df44b48-7d2f-42f8-93e9-f09f8fc029c1"));
            request10.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request10.Headers.Add(new WebTestRequestHeader("request-id", "af127c29-2500-4a05-a075-1836b5b01dd8"));
            request10.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request10.QueryStringParameters.Add("@ClientMetadataQuery", @"{""MetadataType"":""metadataversion"",""MetadataSubtype"":null,""EntityLogicalName"":null,""MetadataId"":null,""MetadataNames"":[],""GetDefault"":false,""DependencyDepth"":""OnDemandWithoutContext"",""ChangedAfter"":""3975641"",""Exclude"":[],""AppId"":""b8ac196d-94c5-e811-a96c-000d3a16a650""}");
            request10.QueryStringParameters.Add("umv", "3975641");
            request10.QueryStringParameters.Add("mv", "3976327");
            yield return request10;
            request10 = null;
            this.EndTransaction("1050.02_Click New Contact Button");
            this.BeginTransaction("1050.03_Click Save to Create Contact");
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request11 = new CrmRequest(thisURL+"/api/data/v9.0/contacts");
            request11.Method = "POST";
            request11.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request11.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request11.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request11.Headers.Add(new WebTestRequestHeader("mscrm.suppressduplicatedetection", "false"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request11.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request11.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request11.Headers.Add(new WebTestRequestHeader("request-id", "f5ed065a-e97d-4c99-91a8-64462273a41b"));
            request11.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request11.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            StringHttpBody request11Body = new StringHttpBody();
            request11Body.ContentType = "application/json";
            request11Body.InsertByteOrderMark = false;
            request11Body.BodyString = @"{""emailaddress1"":"""+Utils.GetRandomString(5,10)+ "@" + Utils.GetRandomString(5, 10) + @".com"",""lastname"":""LT_Contact_" + Utils.GetRandomString(5, 10) + @""",""firstname"":""LT_Contact_" + Utils.GetRandomString(5, 10) + @""",""creditonhold"":false,""donotpostalmail"":false,""donotfax"":false,""donotphone"":false,""donotbulkemail"":false,""donotemail"":false,""donotsendmm"":false,""preferredcontactmethodcode"":1,""statuscode"":1,""statecode"":0,""ownerid@odata.bind"":""/systemusers("+systemuserId+@")""}";
            request11.Body = request11Body;            
            yield return request11;
            string contactid = request11.lastResponse.Headers["Location"].Split('(')[1].Substring(0, 36); 
            request11 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request12 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request12.Method = "POST";
            request12.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request12.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request12.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request12.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request12.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request12.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request12.Headers.Add(new WebTestRequestHeader("request-id", "9a9a2d49-95c6-41ca-b67d-47e22d7f41e6"));
            request12.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096515651"));
            StringHttpBody request12Body = new StringHttpBody();
            request12Body.ContentType = "multipart/mixed;boundary=batch_1541096515651";
            request12Body.InsertByteOrderMark = false;
            request12Body.BodyString = "--batch_1541096515651\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/contacts("+contactid+")?$select=firstname,middlename,lastname,jobtitle,_parentcustomerid_value,emailaddress1,telephone1,mobilephone,fax,preferredcontactmethodcode,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,address1_composite,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,_parentcustomerid_value,gendercode,familystatuscode,spousesname,birthdate,anniversary,description,_originatingleadid_value,lastusedincampaign,donotsendmm,preferredcontactmethodcode,donotemail,donotbulkemail,donotphone,donotfax,donotpostalmail,_transactioncurrencyid_value,creditlimit,creditonhold,paymenttermscode,address1_shippingmethodcode,address1_freighttermscode,address1_latitude,address1_longitude,_ownerid_value,address1_addressid,address1_addressid,address2_addressid,address2_addressid,statecode,contactid,entityimage_url,fullname HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\n\n--batch_1541096515651\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@tid)?@tid=%7B%22%40odata.id%22%3A%22contacts("+contactid+")%22%7D HTTP/1.1\nAccept: application/json\n\n--batch_1541096515651\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target={\"@odata.id\":\"contacts("+contactid+")\"}&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\n\n--batch_1541096515651--\r\n\0";
            request12.Body = request12Body;
            yield return request12;
            request12 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request13 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request13.Method = "POST";
            request13.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request13.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request13.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request13.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request13.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request13.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request13.Headers.Add(new WebTestRequestHeader("request-id", "8355f916-dec6-419c-96f8-e8c28c764b89"));
            request13.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096515890"));
            StringHttpBody request13Body = new StringHttpBody();
            request13Body.ContentType = "multipart/mixed;boundary=batch_1541096515890";
            request13Body.InsertByteOrderMark = false;
            request13Body.BodyString = "--batch_1541096515890\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/contacts("+contactid+")?$select=firstname,middlename,lastname,jobtitle,_parentcustomerid_value,emailaddress1,telephone1,mobilephone,fax,preferredcontactmethodcode,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,gendercode,familystatuscode,spousesname,birthdate,anniversary,description,_originatingleadid_value,lastusedincampaign,donotsendmm,donotemail,donotbulkemail,donotphone,donotfax,donotpostalmail,_transactioncurrencyid_value,creditlimit,creditonhold,paymenttermscode,address1_shippingmethodcode,address1_freighttermscode,address1_latitude,address1_longitude,_ownerid_value,address1_addressid,address2_addressid,fullname,statecode,contactid,entityimage_url,fullname HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\n\n--batch_1541096515890\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@tid)?@tid=%7B%22%40odata.id%22%3A%22contacts("+contactid+")%22%7D HTTP/1.1\nAccept: application/json\n\n--batch_1541096515890\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target={\"@odata.id\":\"contacts("+contactid+")\"}&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\n\n--batch_1541096515890--\r\n\0";
            request13.Body = request13Body;
            yield return request13;
            request13 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request14 = new CrmRequest(thisURL+"/api/data/v9.0/organizations("+orgId+")");
            request14.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request14.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request14.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request14.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request14.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request14.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request14.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request14.Headers.Add(new WebTestRequestHeader("request-id", "a24bea0d-f5fa-4d9a-a86d-2e98555bc94b"));
            request14.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request14.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"3976562\""));
            request14.QueryStringParameters.Add("$select", "yammernetworkpermalink,yammergroupid,yammeroauthaccesstokenexpired");
            yield return request14;
            request14 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request15 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request15.Method = "POST";
            request15.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request15.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request15.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request15.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request15.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request15.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request15.Headers.Add(new WebTestRequestHeader("request-id", "ae9d6f82-dad6-4aca-98f8-c1ef1cc5ca79"));
            request15.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096516218"));
            StringHttpBody request15Body = new StringHttpBody();
            request15Body.ContentType = "multipart/mixed;boundary=batch_1541096516218";
            request15Body.InsertByteOrderMark = false;
            request15Body.BodyString = "--batch_1541096516218\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22false%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%225%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22opportunity%22%3E%3Corder%20attribute%3D%22statecode%22%20descending%3D%22false%22%2F%3E%3Corder%20attribute%3D%22estimatedclosedate%22%20descending%3D%22true%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Cfilter%20type%3D%22or%22%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%220%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22actualclosedate%22%20operator%3D%22last-x-months%22%20value%3D%2212%22%2F%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%221%22%2F%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22estimatedvalue%22%2F%3E%3Cattribute%20name%3D%22estimatedclosedate%22%2F%3E%3Cattribute%20name%3D%22actualvalue%22%2F%3E%3Cattribute%20name%3D%22actualclosedate%22%2F%3E%3Cattribute%20name%3D%22name%22%2F%3E%3Cattribute%20name%3D%22opportunityid%22%2F%3E%3Cattribute%20name%3D%22parentcontactid%22%2F%3E%3Cattribute%20name%3D%22parentaccountid%22%2F%3E%3Cattribute%20name%3D%22modifiedon%22%2F%3E%3Corder%20attribute%3D%22modifiedon%22%20descending%3D%22true%22%2F%3E%3Clink-entity%20name%3D%22contact%22%20from%3D%22contactid%22%20to%3D%22customerid%22%20alias%3D%22bb%22%3E%0A%09%3Cfilter%20type%3D%22and%22%3E%0A%09%09%3Ccondition%20attribute%3D%22contactid%22%20operator%3D%22eq%22%20uitype%3D%22contact%22%20value%3D%22"+contactid+"%22%2F%3E%0A%09%3C%2Ffilter%3E%0A%3C%2Flink-entity%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: d72f848d-5dfd-4196-90fb-248fb8494259\nRequest-Id: 064582dc-7285-422f-a2bb-1d8a9d175587\n\n--batch_1541096516218--\r\n\0";
            request15.Body = request15Body;
            yield return request15;
            request15 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request16 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request16.Method = "POST";
            request16.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request16.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request16.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request16.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request16.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request16.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request16.Headers.Add(new WebTestRequestHeader("request-id", "0e1ef168-f72c-4e47-b7d6-9f0183a06aea"));
            request16.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096516219"));
            StringHttpBody request16Body = new StringHttpBody();
            request16Body.ContentType = "multipart/mixed;boundary=batch_1541096516219";
            request16Body.InsertByteOrderMark = false;
            request16Body.BodyString = "--batch_1541096516219\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/incidents?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22false%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%225%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22incident%22%3E%3Corder%20attribute%3D%22statecode%22%20descending%3D%22true%22%2F%3E%3Corder%20attribute%3D%22prioritycode%22%20descending%3D%22false%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22createdon%22%20operator%3D%22last-x-months%22%20value%3D%2212%22%2F%3E%3C%2Ffilter%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22createdon%22%2F%3E%3Cattribute%20name%3D%22prioritycode%22%2F%3E%3Cattribute%20name%3D%22title%22%2F%3E%3Cattribute%20name%3D%22incidentid%22%2F%3E%3Cattribute%20name%3D%22caseorigincode%22%2F%3E%3Cattribute%20name%3D%22ticketnumber%22%2F%3E%3Cattribute%20name%3D%22customerid%22%2F%3E%3Cattribute%20name%3D%22ownerid%22%2F%3E%3Cattribute%20name%3D%22statuscode%22%2F%3E%3Clink-entity%20name%3D%22contact%22%20from%3D%22contactid%22%20to%3D%22customerid%22%20alias%3D%22bb%22%3E%0A%09%3Cfilter%20type%3D%22and%22%3E%0A%09%09%3Ccondition%20attribute%3D%22contactid%22%20operator%3D%22eq%22%20uitype%3D%22contact%22%20value%3D%22"+contactid+"%22%2F%3E%0A%09%3C%2Ffilter%3E%0A%3C%2Flink-entity%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: d72f848d-5dfd-4196-90fb-248fb8494259\nRequest-Id: 76db69ee-f983-46e4-b93c-e9f1c9710121\n\n--batch_1541096516219--\r\n\0";
            request16.Body = request16Body;
            yield return request16;
            request16 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request17 = new CrmRequest(thisURL+"/api/data/v9.0/entitlements");
            request17.ThinkTime = -1;
            request17.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request17.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request17.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request17.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request17.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request17.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request17.Headers.Add(new WebTestRequestHeader("request-id", "e222dac3-a3de-4b5b-aac1-9f99fc725526"));
            request17.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request17.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""entitlement""><attribute name=""name""/><order attribute=""name"" descending=""false""/><attribute name=""statecode""/><attribute name=""customerid""/><attribute name=""startdate""/><attribute name=""enddate""/><attribute name=""slaid""/><attribute name=""allocationtypecode""/><attribute name=""remainingterms""/><attribute name=""entitlementid""/><link-entity name=""contact"" from=""contactid"" to=""customerid"" alias=""bb""> <filter type=""and"">  <condition attribute=""contactid"" operator=""eq"" uitype=""contact"" value="""+contactid+@"""/> </filter></link-entity></entity></fetch>");
            yield return request17;
            request17 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request18 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request18.ThinkTime = -1;
            request18.Method = "POST";
            request18.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request18.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request18.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request18.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request18.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request18.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request18.Headers.Add(new WebTestRequestHeader("request-id", "1e766d15-becb-4d6b-a7e1-db25fd477ad1"));
            request18.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096516094"));
            StringHttpBody request18Body = new StringHttpBody();
            request18Body.ContentType = "multipart/mixed;boundary=batch_1541096516094";
            request18Body.InsertByteOrderMark = false;
            request18Body.BodyString = "--batch_1541096516094\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts("+contactid+")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22false%22%20page%3d%221%22%20count%3d%2210%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22subject%22%2f%3e%3cattribute%20name%3d%22activitytypecode%22%2f%3e%3cattribute%20name%3d%22statecode%22%2f%3e%3cattribute%20name%3d%22statuscode%22%2f%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22description%22%2f%3e%3cattribute%20name%3d%22modifiedby%22%2f%3e%3cattribute%20name%3d%22ownerid%22%2f%3e%3cattribute%20name%3d%22allparties%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3corder%20attribute%3d%22modifiedon%22%20descending%3d%22true%22%2f%3e%3corder%20attribute%3d%22activityid%22%20descending%3d%22false%22%2f%3e%3clink-entity%20name%3d%22email%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22email%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22senton%22%20alias%3d%22SentOn%22%2f%3e%3cattribute%20name%3d%22delayedemailsendtime%22%20alias%3d%22DelayedemailSendTime%22%2f%3e%3cattribute%20name%3d%22lastopenedtime%22%20alias%3d%22LastOpenedTime%22%2f%3e%3cattribute%20name%3d%22isemailfollowed%22%20alias%3d%22IsEmailFollowed%22%2f%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22email_modifiedon%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22task%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22task%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22task_modifiedon%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22incidentresolution%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22incidentresolution%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22incidentresolution_modifiedon%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22opportunityclose%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22opportunityclose%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22actualrevenue%22%20alias%3d%22opportunityclose_actualrevenue%22%2f%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22opportunityclose_modifiedon%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22appointment%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22appointment%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22regardingobjectid%22%20alias%3d%22appointment_regardingobjectid%22%2f%3e%3cattribute%20name%3d%22prioritycode%22%20alias%3d%22appointment_prioritycode%22%2f%3e%3cattribute%20name%3d%22location%22%20alias%3d%22appointment_location%22%2f%3e%3cattribute%20name%3d%22scheduledstart%22%20alias%3d%22appointment_scheduledstart%22%2f%3e%3cattribute%20name%3d%22scheduleddurationminutes%22%20alias%3d%22appointment_scheduleddurationminutes%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22phonecall%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22phonecall%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22phonecall_modifiedon%22%2f%3e%3cattribute%20name%3d%22directioncode%22%20alias%3d%22phonecall_directioncode%22%2f%3e%3c%2flink-entity%3e%3cfilter%20type%3d%22or%22%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22activitytypecode%22%20operator%3d%22in%22%3e%3cvalue%3e4201%3c%2fvalue%3e%3c%2fcondition%3e%3c%2ffilter%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%20%22activitytypecode%22%20operator%3d%20%22eq%22%20value%3d%20%224220%22%20%2f%3e%3ccondition%20attribute%3d%22regardingobjectid%22%20operator%3d%20%22eq%22%20value%3d%20%22%7b"+contactid+"%7d%22%20%2f%3e%3ccondition%20attribute%3d%22regardingobjecttypecode%22%20operator%3d%20%22eq%22%20value%3d%20%222%22%20%2f%3e%3c%2ffilter%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096516094--\r\n\0";
            request18.Body = request18Body;
            yield return request18;
            request18 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request19 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request19.ThinkTime = -1;
            request19.Method = "POST";
            request19.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request19.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request19.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request19.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request19.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request19.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request19.Headers.Add(new WebTestRequestHeader("request-id", "748c457b-05e5-46d8-8e03-23beeca96165"));
            request19.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096516099"));
            StringHttpBody request19Body = new StringHttpBody();
            request19Body.ContentType = "multipart/mixed;boundary=batch_1541096516099";
            request19Body.InsertByteOrderMark = false;
            request19Body.BodyString = "--batch_1541096516099\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts("+contactid+")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22true%22%20page%3d%221%22%20count%3d%221%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3cfilter%20type%3d%22or%22%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22activitytypecode%22%20operator%3d%22in%22%3e%3cvalue%3e4201%3c%2fvalue%3e%3c%2fcondition%3e%3c%2ffilter%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%20%22activitytypecode%22%20operator%3d%20%22eq%22%20value%3d%20%224220%22%20%2f%3e%3ccondition%20attribute%3d%22regardingobjectid%22%20operator%3d%20%22eq%22%20value%3d%20%22%7b"+contactid+"%7d%22%20%2f%3e%3ccondition%20attribute%3d%22regardingobjecttypecode%22%20operator%3d%20%22eq%22%20value%3d%20%222%22%20%2f%3e%3c%2ffilter%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096516099\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts("+contactid+")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22true%22%20page%3d%221%22%20count%3d%221%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22statecode%22%20operator%3d%22eq%22%20value%3d%220%22%2f%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096516099\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts("+contactid+")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22true%22%20page%3d%221%22%20count%3d%221%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22statecode%22%20operator%3d%22eq%22%20value%3d%221%22%2f%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096516099\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts("+contactid+")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22true%22%20page%3d%221%22%20count%3d%221%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22scheduledend%22%20operator%3d%22lt%22%20value%3d%222018-11-01T18%3a21%3a56.099Z%22%2f%3e%3ccondition%20attribute%3d%22statecode%22%20operator%3d%22ne%22%20value%3d%221%22%2f%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096516099--\r\n\0";
            request19.Body = request19Body;
            yield return request19;
            request19 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request20 = new CrmRequest(thisURL+"/api/data/v9.0//RelationshipDefinitions/Microsoft.Dynamics.CRM.OneToManyRelationshipMetadata");
            request20.Headers.Add(new WebTestRequestHeader("Client-Activity-Id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request20.Headers.Add(new WebTestRequestHeader("Prefer", "odata.include-annotations=\"*\""));
            request20.Headers.Add(new WebTestRequestHeader("Client-Session-Id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request20.Headers.Add(new WebTestRequestHeader("Request-Id", "e21dd123-82f7-41e1-84c5-3fd7f8f9ba66"));
            request20.QueryStringParameters.Add("$filter", "ReferencedEntity eq \'contact\' and IsHierarchical eq true");
            yield return request20;
            request20 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request21 = new CrmRequest(thisURL+"/api/data/v9.0/organizations("+orgId+")");
            request21.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request21.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request21.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request21.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request21.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request21.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request21.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request21.Headers.Add(new WebTestRequestHeader("request-id", "54b344a7-af47-41c6-bb48-c65bd7334eb3"));
            request21.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request21.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"3976562\""));
            request21.QueryStringParameters.Add("$select", "yammernetworkpermalink,yammergroupid,yammeroauthaccesstokenexpired,yammerpostmethod");
            yield return request21;
            request21 = null;
            this.AddCommentToResult("Click Save to Create Contact");
                       
            CrmRequest request23 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request23.Method = "POST";
            request23.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request23.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request23.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request23.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request23.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request23.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request23.Headers.Add(new WebTestRequestHeader("request-id", "d543b6d8-628b-49ea-844d-3b6138d05d83"));
            request23.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096516778"));
            StringHttpBody request23Body = new StringHttpBody();
            request23Body.ContentType = "multipart/mixed;boundary=batch_1541096516778";
            request23Body.InsertByteOrderMark = false;
            request23Body.BodyString = "--batch_1541096516778\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22false%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%225%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22opportunity%22%3E%3Cfilter%20type%3D%22and%22%3E%3Cfilter%20type%3D%22or%22%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%220%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22actualclosedate%22%20operator%3D%22last-x-months%22%20value%3D%2212%22%2F%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%221%22%2F%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22estimatedvalue%22%2F%3E%3Cattribute%20name%3D%22estimatedclosedate%22%2F%3E%3Cattribute%20name%3D%22actualvalue%22%2F%3E%3Cattribute%20name%3D%22actualclosedate%22%2F%3E%3Cattribute%20name%3D%22name%22%2F%3E%3Cattribute%20name%3D%22opportunityid%22%2F%3E%3Cattribute%20name%3D%22parentcontactid%22%2F%3E%3Cattribute%20name%3D%22parentaccountid%22%2F%3E%3Cattribute%20name%3D%22modifiedon%22%2F%3E%3Corder%20attribute%3D%22statecode%22%20descending%3D%22false%22%2F%3E%3Corder%20attribute%3D%22estimatedclosedate%22%20descending%3D%22true%22%2F%3E%3Clink-entity%20name%3D%22contact%22%20from%3D%22contactid%22%20to%3D%22customerid%22%20alias%3D%22bb%22%3E%0A%09%3Cfilter%20type%3D%22and%22%3E%0A%09%09%3Ccondition%20attribute%3D%22contactid%22%20operator%3D%22eq%22%20uitype%3D%22contact%22%20value%3D%22"+contactid+"%22%2F%3E%0A%09%3C%2Ffilter%3E%0A%3C%2Flink-entity%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: d72f848d-5dfd-4196-90fb-248fb8494259\nRequest-Id: 43df5d0b-63fe-4882-b515-3f207389c4eb\n\n--batch_1541096516778--\r\n\0";
            request23.Body = request23Body;
            yield return request23;
            request23 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request24 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request24.Method = "POST";
            request24.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request24.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request24.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request24.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request24.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request24.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request24.Headers.Add(new WebTestRequestHeader("request-id", "27d556c5-4b41-4030-b545-8d4b62450d8b"));
            request24.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096516780"));
            StringHttpBody request24Body = new StringHttpBody();
            request24Body.ContentType = "multipart/mixed;boundary=batch_1541096516780";
            request24Body.InsertByteOrderMark = false;
            request24Body.BodyString = "--batch_1541096516780\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/incidents?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22false%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%225%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22incident%22%3E%3Corder%20attribute%3D%22statecode%22%20descending%3D%22true%22%2F%3E%3Corder%20attribute%3D%22prioritycode%22%20descending%3D%22false%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22createdon%22%20operator%3D%22last-x-months%22%20value%3D%2212%22%2F%3E%3C%2Ffilter%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22createdon%22%2F%3E%3Cattribute%20name%3D%22prioritycode%22%2F%3E%3Cattribute%20name%3D%22title%22%2F%3E%3Cattribute%20name%3D%22incidentid%22%2F%3E%3Cattribute%20name%3D%22caseorigincode%22%2F%3E%3Cattribute%20name%3D%22ticketnumber%22%2F%3E%3Cattribute%20name%3D%22customerid%22%2F%3E%3Cattribute%20name%3D%22ownerid%22%2F%3E%3Cattribute%20name%3D%22statuscode%22%2F%3E%3Clink-entity%20name%3D%22contact%22%20from%3D%22contactid%22%20to%3D%22customerid%22%20alias%3D%22bb%22%3E%0A%09%3Cfilter%20type%3D%22and%22%3E%0A%09%09%3Ccondition%20attribute%3D%22contactid%22%20operator%3D%22eq%22%20uitype%3D%22contact%22%20value%3D%22"+contactid+"%22%2F%3E%0A%09%3C%2Ffilter%3E%0A%3C%2Flink-entity%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: d72f848d-5dfd-4196-90fb-248fb8494259\nRequest-Id: 3e11a342-4f51-47f4-acad-7caf457a43b8\n\n--batch_1541096516780--\r\n\0";
            request24.Body = request24Body;
            yield return request24;
            request24 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request26 = new CrmRequest(thisURL+"/api/data/v9.0/entitlements");
            request26.ThinkTime = 1;
            request26.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request26.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request26.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request26.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request26.Headers.Add(new WebTestRequestHeader("client-activity-id", "d72f848d-5dfd-4196-90fb-248fb8494259"));
            request26.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request26.Headers.Add(new WebTestRequestHeader("request-id", "e87784bf-4f0c-4b81-855f-20f2549f7841"));
            request26.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request26.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""entitlement""><attribute name=""name""/><order attribute=""name"" descending=""false""/><attribute name=""statecode""/><attribute name=""customerid""/><attribute name=""startdate""/><attribute name=""enddate""/><attribute name=""slaid""/><attribute name=""allocationtypecode""/><attribute name=""remainingterms""/><attribute name=""entitlementid""/><link-entity name=""contact"" from=""contactid"" to=""customerid"" alias=""bb""> <filter type=""and"">  <condition attribute=""contactid"" operator=""eq"" uitype=""contact"" value="""+contactid+@"""/> </filter></link-entity></entity></fetch>");
            yield return request26;
            request26 = null;
            this.AddCommentToResult("Click Save to Create Contact");

            CrmRequest request29 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request29.ThinkTime = 16;
            request29.Method = "POST";
            request29.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request29.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request29.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request29.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request29.Headers.Add(new WebTestRequestHeader("client-activity-id", "afa34850-db06-42e6-8637-d7b455a2a8c2"));
            request29.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request29.Headers.Add(new WebTestRequestHeader("request-id", "73794eea-bd6a-485c-9c19-df717812b150"));
            request29.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request29.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            StringHttpBody request29Body = new StringHttpBody();
            request29Body.ContentType = "application/json";
            request29Body.InsertByteOrderMark = false;
            request29Body.BodyString = "{\"items\":[{\"EntityTypeName\":\"contact\",\"LastAccessedStr\":\"2018-11-01T18:21:55.856Z\",\"ObjectId\":\"{"+contactid+"}\",\"PinStatus\":false,\"RecordType\":\"Entity\",\"Title\":\"LT_Contact LT_Contact\",\"IconPath\":null,\"IsDeleted\":false}]}";
            request29.Body = request29Body;
            yield return request29;
            request29 = null;
            this.EndTransaction("1050.03_Click Save to Create Contact");
            this.BeginTransaction("1050.04_Enter Parent Account and Click Save");
            
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request32 = new CrmRequest(thisURL+"/api/data/v9.0/contacts");
            request32.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request32.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request32.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request32.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request32.Headers.Add(new WebTestRequestHeader("client-activity-id", "201be793-fadd-407c-94c3-929802a74476"));
            request32.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request32.Headers.Add(new WebTestRequestHeader("request-id", "e088bd6a-bc40-441f-98e2-27244554cda7"));
            request32.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request32.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""25"" no-lock=""false""><entity name=""contact""><attribute name=""statecode""/><attribute name=""fullname""/><attribute name=""parentcustomerid""/><attribute name=""address1_city""/><attribute name=""address1_telephone1""/><attribute name=""telephone1""/><attribute name=""emailaddress1""/><attribute name=""contactid""/><attribute name=""fax""/><attribute name=""address1_name""/><attribute name=""address1_fax""/><filter type=""and""><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><order attribute=""fullname"" descending=""false""/></entity></fetch>");
            yield return request32;
            request32 = null;
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request33 = new CrmRequest(thisURL+"/api/data/v9.0/accounts");
            request33.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request33.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request33.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request33.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request33.Headers.Add(new WebTestRequestHeader("client-activity-id", "201be793-fadd-407c-94c3-929802a74476"));
            request33.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request33.Headers.Add(new WebTestRequestHeader("request-id", "69c9fdab-fe71-4831-9687-e2e66bf0761c"));
            request33.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request33.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""25"" no-lock=""false""><entity name=""account""><attribute name=""statecode""/><attribute name=""name""/><attribute name=""accountnumber""/><attribute name=""primarycontactid""/><attribute name=""address1_city""/><attribute name=""telephone1""/><attribute name=""emailaddress1""/><attribute name=""accountid""/><attribute name=""fax""/><attribute name=""address1_name""/><attribute name=""address1_fax""/><filter type=""and""><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter></entity></fetch>");
            yield return request33;
            string resresult = request33.lastResponse.BodyString;
            request33 = null;
            string accountId = Utils.extractUCIgridresponse("accountid\":\"", resresult);
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request34 = new CrmRequest(thisURL+"/api/data/v9.0/accounts");
            request34.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request34.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request34.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request34.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request34.Headers.Add(new WebTestRequestHeader("client-activity-id", "201be793-fadd-407c-94c3-929802a74476"));
            request34.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request34.Headers.Add(new WebTestRequestHeader("request-id", "1d82b8fe-3b3b-41d1-83bd-f3a04f68233a"));
            request34.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request34.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""25"" no-lock=""false""><entity name=""account""><attribute name=""statecode""/><attribute name=""name""/><attribute name=""accountnumber""/><attribute name=""primarycontactid""/><attribute name=""address1_city""/><attribute name=""telephone1""/><attribute name=""emailaddress1""/><attribute name=""accountid""/><attribute name=""fax""/><attribute name=""address1_name""/><attribute name=""address1_fax""/><filter type=""and""><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><order attribute=""name"" descending=""false""/></entity></fetch>");
            yield return request34;
            request34 = null;
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request35 = new CrmRequest(thisURL+"/api/data/v9.0/contacts");
            request35.ThinkTime = 4;
            request35.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request35.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request35.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request35.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request35.Headers.Add(new WebTestRequestHeader("client-activity-id", "201be793-fadd-407c-94c3-929802a74476"));
            request35.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request35.Headers.Add(new WebTestRequestHeader("request-id", "9e84eb86-cf74-4ae1-8123-fecb6eaca2af"));
            request35.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request35.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""25"" no-lock=""false""><entity name=""contact""><attribute name=""statecode""/><attribute name=""fullname""/><attribute name=""parentcustomerid""/><attribute name=""address1_city""/><attribute name=""address1_telephone1""/><attribute name=""telephone1""/><attribute name=""emailaddress1""/><attribute name=""contactid""/><attribute name=""fax""/><attribute name=""address1_name""/><attribute name=""address1_fax""/><filter type=""and""><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><order attribute=""fullname"" descending=""false""/></entity></fetch>");
            yield return request35;
            request35 = null;
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request36 = new CrmRequest(thisURL+"/api/data/v9.0/accounts");
            request36.ThinkTime = 5;
            request36.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request36.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request36.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request36.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request36.Headers.Add(new WebTestRequestHeader("client-activity-id", "55f6324d-fdd6-444d-98d2-c048f1b979d6"));
            request36.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request36.Headers.Add(new WebTestRequestHeader("request-id", "f760613d-c9eb-4c53-96d8-d02b9505c7d7"));
            request36.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request36.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""25"" no-lock=""false""><entity name=""account""><attribute name=""statecode""/><attribute name=""name""/><attribute name=""accountnumber""/><attribute name=""primarycontactid""/><attribute name=""address1_city""/><attribute name=""telephone1""/><attribute name=""emailaddress1""/><attribute name=""accountid""/><attribute name=""fax""/><attribute name=""address1_name""/><attribute name=""address1_fax""/><filter type=""and""><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><order attribute=""name"" descending=""false""/></entity></fetch>");
            yield return request36;
            request36 = null;
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request37 = new CrmRequest(thisURL+"/api/data/v9.0/contacts("+contactid+")");
            request37.Method = "PATCH";
            request37.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request37.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request37.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request37.Headers.Add(new WebTestRequestHeader("mscrm.suppressduplicatedetection", "false"));
            request37.Headers.Add(new WebTestRequestHeader("if-match", "*"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request37.Headers.Add(new WebTestRequestHeader("autodisassociate", "true"));
            request37.Headers.Add(new WebTestRequestHeader("client-activity-id", "c00f91dc-6b2b-4b25-8094-f04acd82da48"));
            request37.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request37.Headers.Add(new WebTestRequestHeader("request-id", "c5b4f5b7-5e70-47f5-a918-421da7d0f609"));
            request37.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request37.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            StringHttpBody request37Body = new StringHttpBody();
            request37Body.ContentType = "application/json";
            request37Body.InsertByteOrderMark = false;
            request37Body.BodyString = "{\"parentcustomerid_account@odata.bind\":\"/accounts("+accountId+")\",\"telephone1\":\""+Utils.GetRandomPhoneNumber()+"\"}";
            request37.Body = request37Body;
            yield return request37;
            request37 = null;
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request38 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request38.Method = "POST";
            request38.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request38.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request38.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request38.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request38.Headers.Add(new WebTestRequestHeader("client-activity-id", "c00f91dc-6b2b-4b25-8094-f04acd82da48"));
            request38.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request38.Headers.Add(new WebTestRequestHeader("request-id", "ada5b298-0818-4718-a04b-aa2cc3fddcc7"));
            request38.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096548860"));
            StringHttpBody request38Body = new StringHttpBody();
            request38Body.ContentType = "multipart/mixed;boundary=batch_1541096548860";
            request38Body.InsertByteOrderMark = false;
            request38Body.BodyString = "--batch_1541096548860\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/contacts("+contactid+")?$select=firstname,middlename,lastname,jobtitle,_parentcustomerid_value,emailaddress1,telephone1,mobilephone,fax,preferredcontactmethodcode,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,address1_composite,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,_parentcustomerid_value,gendercode,familystatuscode,spousesname,birthdate,anniversary,description,_originatingleadid_value,lastusedincampaign,donotsendmm,preferredcontactmethodcode,donotemail,donotbulkemail,donotphone,donotfax,donotpostalmail,_transactioncurrencyid_value,creditlimit,creditonhold,paymenttermscode,address1_shippingmethodcode,address1_freighttermscode,address1_latitude,address1_longitude,_ownerid_value,address1_addressid,address1_addressid,address2_addressid,address2_addressid,statecode,contactid,entityimage_url,fullname HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\n\n--batch_1541096548860\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@tid)?@tid=%7B%22%40odata.id%22%3A%22contacts("+contactid+")%22%7D HTTP/1.1\nAccept: application/json\n\n--batch_1541096548860\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target={\"@odata.id\":\"contacts("+contactid+")\"}&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\n\n--batch_1541096548860--\r\n\0";
            request38.Body = request38Body;
            yield return request38;
            request38 = null;
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request39 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request39.Method = "POST";
            request39.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request39.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request39.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request39.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request39.Headers.Add(new WebTestRequestHeader("client-activity-id", "c00f91dc-6b2b-4b25-8094-f04acd82da48"));
            request39.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request39.Headers.Add(new WebTestRequestHeader("request-id", "454de886-9a3f-4a0c-9dee-f5b1fcb3ce44"));
            request39.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096549215"));
            StringHttpBody request39Body = new StringHttpBody();
            request39Body.ContentType = "multipart/mixed;boundary=batch_1541096549215";
            request39Body.InsertByteOrderMark = false;
            request39Body.BodyString = "--batch_1541096549215\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22false%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%225%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22opportunity%22%3E%3Cfilter%20type%3D%22and%22%3E%3Cfilter%20type%3D%22or%22%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%220%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22actualclosedate%22%20operator%3D%22last-x-months%22%20value%3D%2212%22%2F%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%221%22%2F%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22estimatedvalue%22%2F%3E%3Cattribute%20name%3D%22estimatedclosedate%22%2F%3E%3Cattribute%20name%3D%22actualvalue%22%2F%3E%3Cattribute%20name%3D%22actualclosedate%22%2F%3E%3Cattribute%20name%3D%22name%22%2F%3E%3Cattribute%20name%3D%22opportunityid%22%2F%3E%3Cattribute%20name%3D%22parentcontactid%22%2F%3E%3Cattribute%20name%3D%22parentaccountid%22%2F%3E%3Cattribute%20name%3D%22modifiedon%22%2F%3E%3Corder%20attribute%3D%22statecode%22%20descending%3D%22false%22%2F%3E%3Corder%20attribute%3D%22estimatedclosedate%22%20descending%3D%22true%22%2F%3E%3Clink-entity%20name%3D%22contact%22%20from%3D%22contactid%22%20to%3D%22customerid%22%20alias%3D%22bb%22%3E%0A%09%3Cfilter%20type%3D%22and%22%3E%0A%09%09%3Ccondition%20attribute%3D%22contactid%22%20operator%3D%22eq%22%20uitype%3D%22contact%22%20value%3D%22"+contactid+"%22%2F%3E%0A%09%3C%2Ffilter%3E%0A%3C%2Flink-entity%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: c00f91dc-6b2b-4b25-8094-f04acd82da48\nRequest-Id: dc54edb5-7ac7-4dde-ac18-142e0d6318ac\n\n--batch_1541096549215--\r\n\0";
            request39.Body = request39Body;
            yield return request39;
            request39 = null;
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request40 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request40.Method = "POST";
            request40.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request40.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request40.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request40.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request40.Headers.Add(new WebTestRequestHeader("client-activity-id", "c00f91dc-6b2b-4b25-8094-f04acd82da48"));
            request40.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request40.Headers.Add(new WebTestRequestHeader("request-id", "88d033f7-7d5f-4b7f-8501-5751bad1559a"));
            request40.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096549217"));
            StringHttpBody request40Body = new StringHttpBody();
            request40Body.ContentType = "multipart/mixed;boundary=batch_1541096549217";
            request40Body.InsertByteOrderMark = false;
            request40Body.BodyString = "--batch_1541096549217\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/incidents?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22false%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%225%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22incident%22%3E%3Corder%20attribute%3D%22statecode%22%20descending%3D%22true%22%2F%3E%3Corder%20attribute%3D%22prioritycode%22%20descending%3D%22false%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22createdon%22%20operator%3D%22last-x-months%22%20value%3D%2212%22%2F%3E%3C%2Ffilter%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22createdon%22%2F%3E%3Cattribute%20name%3D%22prioritycode%22%2F%3E%3Cattribute%20name%3D%22title%22%2F%3E%3Cattribute%20name%3D%22incidentid%22%2F%3E%3Cattribute%20name%3D%22caseorigincode%22%2F%3E%3Cattribute%20name%3D%22ticketnumber%22%2F%3E%3Cattribute%20name%3D%22customerid%22%2F%3E%3Cattribute%20name%3D%22ownerid%22%2F%3E%3Cattribute%20name%3D%22statuscode%22%2F%3E%3Clink-entity%20name%3D%22contact%22%20from%3D%22contactid%22%20to%3D%22customerid%22%20alias%3D%22bb%22%3E%0A%09%3Cfilter%20type%3D%22and%22%3E%0A%09%09%3Ccondition%20attribute%3D%22contactid%22%20operator%3D%22eq%22%20uitype%3D%22contact%22%20value%3D%22"+contactid+"%22%2F%3E%0A%09%3C%2Ffilter%3E%0A%3C%2Flink-entity%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: c00f91dc-6b2b-4b25-8094-f04acd82da48\nRequest-Id: 4d7b6d1c-e149-4817-9ea1-882e4f104daf\n\n--batch_1541096549217--\r\n\0";
            request40.Body = request40Body;
            yield return request40;
            request40 = null;
            this.AddCommentToResult("Enter Parent Account and Click Save");

            CrmRequest request41 = new CrmRequest(thisURL+"/api/data/v9.0/entitlements");
            request41.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request41.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request41.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request41.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request41.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request41.Headers.Add(new WebTestRequestHeader("client-activity-id", "c00f91dc-6b2b-4b25-8094-f04acd82da48"));
            request41.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request41.Headers.Add(new WebTestRequestHeader("request-id", "6866e0c0-a205-4ee9-8b19-a62fbdbc484f"));
            request41.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request41.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""entitlement""><attribute name=""name""/><order attribute=""name"" descending=""false""/><attribute name=""statecode""/><attribute name=""customerid""/><attribute name=""startdate""/><attribute name=""enddate""/><attribute name=""slaid""/><attribute name=""allocationtypecode""/><attribute name=""remainingterms""/><attribute name=""entitlementid""/><link-entity name=""contact"" from=""contactid"" to=""customerid"" alias=""bb""> <filter type=""and"">  <condition attribute=""contactid"" operator=""eq"" uitype=""contact"" value="""+contactid+@"""/> </filter></link-entity></entity></fetch>");
            yield return request41;
            request41 = null;
            this.EndTransaction("1050.04_Enter Parent Account and Click Save");
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
