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
    public class zz_1051_UCI_SearchandOpenContactCoded : WebTestBase
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

        public zz_1051_UCI_SearchandOpenContactCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1051_UCI_SearchandOpenContactCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1051_UCI_SearchandOpenContactCoded_PostWebTest);
        }
        public void zz_1051_UCI_SearchandOpenContactCoded_PreWebTest(object sender, PreWebTestEventArgs e)
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
        public void zz_1051_UCI_SearchandOpenContactCoded_PostWebTest(object sender, PostWebTestEventArgs e)
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
            this.BeginTransaction("1051.01_Navigate to My Active Contacts");
            this.AddCommentToResult("Navigate to My Active Contacts");

            CrmRequest request1 = new CrmRequest(thisURL+"/api/data/v9.0/contacts");
            request1.ThinkTime = 1;
            request1.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request1.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request1.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request1.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request1.Headers.Add(new WebTestRequestHeader("client-activity-id", "81d613a9-58f4-4094-bce6-8c57087c3e77"));
            request1.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request1.Headers.Add(new WebTestRequestHeader("request-id", "d51b02a6-ef41-44b7-8acd-8ccd4946492c"));
            request1.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request1.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""50"" no-lock=""false""><entity name=""contact""><attribute name=""statecode""/><attribute name=""entityimage_url""/><attribute name=""fullname""/><order attribute=""fullname"" descending=""false""/><attribute name=""parentcustomerid""/><filter type=""and""><condition attribute=""ownerid"" operator=""eq-userid""/><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><attribute name=""telephone1""/><attribute name=""emailaddress1""/><attribute name=""contactid""/></entity></fetch>");
            yield return request1;
            string resresult = request1.lastResponse.BodyString;
            request1 = null;
            string contactid = Utils.extractUCIgridresponse("contactid\":\"", resresult);
            this.AddCommentToResult("Navigate to My Active Contacts");

            CrmRequest request2 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request2.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request2.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request2.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request2.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request2.Headers.Add(new WebTestRequestHeader("client-activity-id", "1b2ec2e0-587d-442f-86f5-d2ae6ce79969"));
            request2.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request2.Headers.Add(new WebTestRequestHeader("request-id", "0277065c-e3d8-4ae4-8a38-4b42a3bd38d7"));
            request2.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request2.QueryStringParameters.Add("@ClientMetadataQuery", @"{""MetadataType"":""metadataversion"",""MetadataSubtype"":null,""EntityLogicalName"":null,""MetadataId"":null,""MetadataNames"":[],""GetDefault"":false,""DependencyDepth"":""OnDemandWithoutContext"",""ChangedAfter"":""3975641"",""Exclude"":[],""AppId"":""b8ac196d-94c5-e811-a96c-000d3a16a650""}");
            request2.QueryStringParameters.Add("umv", "3975641");
            request2.QueryStringParameters.Add("mv", "3976327");
            yield return request2;
            request2 = null;
            this.AddCommentToResult("Navigate to My Active Contacts");

            CrmRequest request3 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request3.ThinkTime = 12;
            request3.Method = "POST";
            request3.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request3.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request3.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request3.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request3.Headers.Add(new WebTestRequestHeader("client-activity-id", "1b2ec2e0-587d-442f-86f5-d2ae6ce79969"));
            request3.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request3.Headers.Add(new WebTestRequestHeader("request-id", "f90a73f9-7587-4650-b1c1-e3d18cc585ee"));
            request3.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request3.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            StringHttpBody request3Body = new StringHttpBody();
            request3Body.ContentType = "application/json";
            request3Body.InsertByteOrderMark = false;
            request3Body.BodyString = "{\"items\":[{\"EntityTypeName\":\"contact\",\"LastAccessedStr\":\"2018-11-01T18:23:43.068Z\",\"ObjectId\":\"{00000000-0000-0000-00AA-000010001003}\",\"PinStatus\":false,\"RecordType\":\"Grid\",\"IsUserView\":false,\"Title\":\"My Active Contacts\",\"IsDeleted\":false}]}";
            request3.Body = request3Body;
            yield return request3;
            request3 = null;
            this.EndTransaction("1051.01_Navigate to My Active Contacts");
            this.BeginTransaction("1051.02_Quick Find Search for Contact");
            this.AddCommentToResult("Quick Find Search for Contact");

            CrmRequest request4 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request4.Method = "POST";
            request4.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request4.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request4.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request4.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request4.Headers.Add(new WebTestRequestHeader("client-activity-id", "d61711f9-dd0f-4364-a3b9-393bf9792f68"));
            request4.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request4.Headers.Add(new WebTestRequestHeader("request-id", "5d34617c-07ee-4de8-80c1-856f6ab805ce"));
            request4.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096636959"));
            StringHttpBody request4Body = new StringHttpBody();
            request4Body.ContentType = "multipart/mixed;boundary=batch_1541096636959";
            request4Body.InsertByteOrderMark = false;
            request4Body.BodyString = "--batch_1541096636959\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/contacts?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%2250%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22contact%22%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22entityimage_url%22%2F%3E%3Cattribute%20name%3D%22fullname%22%2F%3E%3Cattribute%20name%3D%22parentcustomerid%22%2F%3E%3Cattribute%20name%3D%22address1_city%22%2F%3E%3Cattribute%20name%3D%22address1_telephone1%22%2F%3E%3Cattribute%20name%3D%22telephone1%22%2F%3E%3Cattribute%20name%3D%22emailaddress1%22%2F%3E%3Cattribute%20name%3D%22contactid%22%2F%3E%3Corder%20attribute%3D%22fullname%22%20descending%3D%22false%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%220%22%2F%3E%3C%2Ffilter%3E%3Cfilter%20type%3D%22or%22%20isquickfindfields%3D%221%22%3E%3Ccondition%20attribute%3D%22fullname%22%20operator%3D%22like%22%20value%3D%22lt%25%22%2F%3E%3Ccondition%20attribute%3D%22firstname%22%20operator%3D%22like%22%20value%3D%22lt%25%22%2F%3E%3Ccondition%20attribute%3D%22lastname%22%20operator%3D%22like%22%20value%3D%22lt%25%22%2F%3E%3Ccondition%20attribute%3D%22middlename%22%20operator%3D%22like%22%20value%3D%22lt%25%22%2F%3E%3Ccondition%20attribute%3D%22emailaddress1%22%20operator%3D%22like%22%20value%3D%22lt%25%22%2F%3E%3Ccondition%20attribute%3D%22parentcustomeridname%22%20operator%3D%22like%22%20value%3D%22lt%25%22%2F%3E%3Ccondition%20attribute%3D%22telephone1%22%20operator%3D%22like%22%20value%3D%22lt%25%22%2F%3E%3Ccondition%20attribute%3D%22mobilephone%22%20operator%3D%22like%22%20value%3D%22lt%25%22%2F%3E%3C%2Ffilter%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: d61711f9-dd0f-4364-a3b9-393bf9792f68\nRequest-Id: 587dc87b-6bc5-4256-922c-c5d456ed74db\n\n--batch_1541096636959--\r\n\0";
            request4.Body = request4Body;
            yield return request4;
            request4 = null;
            this.AddCommentToResult("Quick Find Search for Contact");

            CrmRequest request5 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request5.ThinkTime = 18;
            request5.Method = "POST";
            request5.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request5.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request5.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request5.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
            request5.Headers.Add(new WebTestRequestHeader("client-activity-id", "c72e29df-e7e5-436e-b56e-8ffabc49e477"));
            request5.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
            request5.Headers.Add(new WebTestRequestHeader("request-id", "7b2328d9-9de0-4516-babd-37acfd23dbd5"));
            request5.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request5.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            StringHttpBody request5Body = new StringHttpBody();
            request5Body.ContentType = "application/json";
            request5Body.InsertByteOrderMark = false;
            request5Body.BodyString = "{\"items\":[{\"EntityTypeName\":\"contact\",\"LastAccessedStr\":\"2018-11-01T18:23:56.914Z\",\"ObjectId\":\"{8DF19B44-A073-40C3-9D6D-EE1355D8C4BA}\",\"PinStatus\":false,\"RecordType\":\"Grid\",\"IsUserView\":false,\"Title\":\"Quick Find Active Contacts\",\"IsDeleted\":false}]}";
            request5.Body = request5Body;
            yield return request5;
            request5 = null;
            this.EndTransaction("1051.02_Quick Find Search for Contact");
                      

            if (contactid == null)
            {

                this.AddCommentToResult("No Contact Records were found for the user " + UserName + "! Create contact records for the user and try again.");
                Outcome = Outcome.Fail;
            }
            else
            {
                this.BeginTransaction("1051.03_Open Contact Record");
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request7 = new CrmRequest(thisURL + "/api/data/v9.0/$batch");
                request7.Method = "POST";
                request7.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request7.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request7.Headers.Add(new WebTestRequestHeader("Origin", thisURL + ""));
                request7.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request7.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request7.Headers.Add(new WebTestRequestHeader("client-activity-id", "d22166a4-ca32-4db6-a041-ff936e120b38"));
                request7.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request7.Headers.Add(new WebTestRequestHeader("request-id", "a7b2a5e4-8754-402b-a030-63093f1e3740"));
                request7.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096655678"));
                StringHttpBody request7Body = new StringHttpBody();
                request7Body.ContentType = "multipart/mixed;boundary=batch_1541096655678";
                request7Body.InsertByteOrderMark = false;
                request7Body.BodyString = "--batch_1541096655678\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22false%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%225%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22opportunity%22%3E%3Corder%20attribute%3D%22statecode%22%20descending%3D%22false%22%2F%3E%3Corder%20attribute%3D%22estimatedclosedate%22%20descending%3D%22true%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Cfilter%20type%3D%22or%22%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%220%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22actualclosedate%22%20operator%3D%22last-x-months%22%20value%3D%2212%22%2F%3E%3Ccondition%20attribute%3D%22statecode%22%20operator%3D%22eq%22%20value%3D%221%22%2F%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3C%2Ffilter%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22estimatedvalue%22%2F%3E%3Cattribute%20name%3D%22estimatedclosedate%22%2F%3E%3Cattribute%20name%3D%22actualvalue%22%2F%3E%3Cattribute%20name%3D%22actualclosedate%22%2F%3E%3Cattribute%20name%3D%22name%22%2F%3E%3Cattribute%20name%3D%22opportunityid%22%2F%3E%3Cattribute%20name%3D%22parentcontactid%22%2F%3E%3Cattribute%20name%3D%22parentaccountid%22%2F%3E%3Cattribute%20name%3D%22modifiedon%22%2F%3E%3Corder%20attribute%3D%22modifiedon%22%20descending%3D%22true%22%2F%3E%3Clink-entity%20name%3D%22contact%22%20from%3D%22contactid%22%20to%3D%22customerid%22%20alias%3D%22bb%22%3E%0A%09%3Cfilter%20type%3D%22and%22%3E%0A%09%09%3Ccondition%20attribute%3D%22contactid%22%20operator%3D%22eq%22%20uitype%3D%22contact%22%20value%3D%22" + contactid + "%22%2F%3E%0A%09%3C%2Ffilter%3E%0A%3C%2Flink-entity%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: d22166a4-ca32-4db6-a041-ff936e120b38\nRequest-Id: 32ec64a6-3075-416e-a465-aaacea1f3b46\n\n--batch_1541096655678--\r\n\0";
                request7.Body = request7Body;
                yield return request7;
                request7 = null;
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request8 = new CrmRequest(thisURL + "/api/data/v9.0/$batch");
                request8.Method = "POST";
                request8.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request8.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request8.Headers.Add(new WebTestRequestHeader("Origin", thisURL + ""));
                request8.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request8.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request8.Headers.Add(new WebTestRequestHeader("client-activity-id", "d22166a4-ca32-4db6-a041-ff936e120b38"));
                request8.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request8.Headers.Add(new WebTestRequestHeader("request-id", "9e0b3a52-c699-4cdf-8cc2-51555271d291"));
                request8.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096655680"));
                StringHttpBody request8Body = new StringHttpBody();
                request8Body.ContentType = "multipart/mixed;boundary=batch_1541096655680";
                request8Body.InsertByteOrderMark = false;
                request8Body.BodyString = "--batch_1541096655680\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/incidents?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22false%22%20returntotalrecordcount%3D%22true%22%20page%3D%221%22%20count%3D%225%22%20no-lock%3D%22false%22%3E%3Centity%20name%3D%22incident%22%3E%3Corder%20attribute%3D%22statecode%22%20descending%3D%22true%22%2F%3E%3Corder%20attribute%3D%22prioritycode%22%20descending%3D%22false%22%2F%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22createdon%22%20operator%3D%22last-x-months%22%20value%3D%2212%22%2F%3E%3C%2Ffilter%3E%3Cattribute%20name%3D%22statecode%22%2F%3E%3Cattribute%20name%3D%22createdon%22%2F%3E%3Cattribute%20name%3D%22prioritycode%22%2F%3E%3Cattribute%20name%3D%22title%22%2F%3E%3Cattribute%20name%3D%22incidentid%22%2F%3E%3Cattribute%20name%3D%22caseorigincode%22%2F%3E%3Cattribute%20name%3D%22ticketnumber%22%2F%3E%3Cattribute%20name%3D%22customerid%22%2F%3E%3Cattribute%20name%3D%22ownerid%22%2F%3E%3Cattribute%20name%3D%22statuscode%22%2F%3E%3Clink-entity%20name%3D%22contact%22%20from%3D%22contactid%22%20to%3D%22customerid%22%20alias%3D%22bb%22%3E%0A%09%3Cfilter%20type%3D%22and%22%3E%0A%09%09%3Ccondition%20attribute%3D%22contactid%22%20operator%3D%22eq%22%20uitype%3D%22contact%22%20value%3D%22" + contactid + "%22%2F%3E%0A%09%3C%2Ffilter%3E%0A%3C%2Flink-entity%3E%3C%2Fentity%3E%3C%2Ffetch%3E HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nClient-Session-Id: bc5af60f-66db-4c5f-ad10-978292a0d40e\nClient-Activity-Id: d22166a4-ca32-4db6-a041-ff936e120b38\nRequest-Id: 380bac06-08a8-4ab6-9dff-73d6fbd0a392\n\n--batch_1541096655680--\r\n\0";
                request8.Body = request8Body;
                yield return request8;
                request8 = null;
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request10 = new CrmRequest(thisURL + "/api/data/v9.0/$batch");
                request10.Method = "POST";
                request10.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request10.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request10.Headers.Add(new WebTestRequestHeader("Origin", thisURL + ""));
                request10.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request10.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request10.Headers.Add(new WebTestRequestHeader("client-activity-id", "d22166a4-ca32-4db6-a041-ff936e120b38"));
                request10.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request10.Headers.Add(new WebTestRequestHeader("request-id", "d15796da-8061-4db0-9c51-f80a3839cd46"));
                request10.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096655644"));
                StringHttpBody request10Body = new StringHttpBody();
                request10Body.ContentType = "multipart/mixed;boundary=batch_1541096655644";
                request10Body.InsertByteOrderMark = false;
                request10Body.BodyString = "--batch_1541096655644\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/contacts(" + contactid + ")?$select=firstname,middlename,lastname,jobtitle,_parentcustomerid_value,emailaddress1,telephone1,mobilephone,fax,preferredcontactmethodcode,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,gendercode,familystatuscode,spousesname,birthdate,anniversary,description,_originatingleadid_value,lastusedincampaign,donotsendmm,donotemail,donotbulkemail,donotphone,donotfax,donotpostalmail,_transactioncurrencyid_value,creditlimit,creditonhold,paymenttermscode,address1_shippingmethodcode,address1_freighttermscode,address1_latitude,address1_longitude,_ownerid_value,address1_addressid,address2_addressid,fullname,statecode,contactid,entityimage_url HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\n\n--batch_1541096655644\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers(" + systemuserId + ")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@tid)?@tid=%7B%22%40odata.id%22%3A%22contacts(" + contactid + ")%22%7D HTTP/1.1\nAccept: application/json\n\n--batch_1541096655644\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target={\"@odata.id\":\"contacts(" + contactid + ")\"}&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\n\n--batch_1541096655644--\r\n\0";
                request10.Body = request10Body;
                yield return request10;
                request10 = null;
                this.AddCommentToResult("Open Contact Record");


                CrmRequest request12 = new CrmRequest(thisURL + "/api/data/v9.0/systemusers(" + systemuserId + ")");
                request12.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request12.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request12.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
                request12.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request12.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request12.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
                request12.Headers.Add(new WebTestRequestHeader("client-activity-id", "d22166a4-ca32-4db6-a041-ff936e120b38"));
                request12.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request12.Headers.Add(new WebTestRequestHeader("request-id", "dcc65d2d-f8b6-4517-ba96-48527cbf89d4"));
                request12.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
                request12.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"3810056\""));
                request12.QueryStringParameters.Add("$select", "entityimage");
                yield return request12;
                request12 = null;
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request13 = new CrmRequest(thisURL + "/api/data/v9.0/organizations(" + orgId + ")");
                request13.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request13.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request13.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
                request13.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request13.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request13.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
                request13.Headers.Add(new WebTestRequestHeader("client-activity-id", "d22166a4-ca32-4db6-a041-ff936e120b38"));
                request13.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request13.Headers.Add(new WebTestRequestHeader("request-id", "4d5c23ff-22ee-4566-9108-00cc5b87e52f"));
                request13.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
                request13.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"3976562\""));
                request13.QueryStringParameters.Add("$select", "yammernetworkpermalink,yammergroupid,yammeroauthaccesstokenexpired");
                yield return request13;
                request13 = null;
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request14 = new CrmRequest(thisURL + "/api/data/v9.0/$batch");
                request14.Method = "POST";
                request14.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request14.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request14.Headers.Add(new WebTestRequestHeader("Origin", thisURL + ""));
                request14.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request14.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request14.Headers.Add(new WebTestRequestHeader("client-activity-id", "d22166a4-ca32-4db6-a041-ff936e120b38"));
                request14.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request14.Headers.Add(new WebTestRequestHeader("request-id", "43ef3c25-eff8-4792-b57d-2f44684be924"));
                request14.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096656119"));
                StringHttpBody request14Body = new StringHttpBody();
                request14Body.ContentType = "multipart/mixed;boundary=batch_1541096656119";
                request14Body.InsertByteOrderMark = false;
                request14Body.BodyString = "--batch_1541096656119\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts(" + contactid + ")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22false%22%20page%3d%221%22%20count%3d%2210%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22subject%22%2f%3e%3cattribute%20name%3d%22activitytypecode%22%2f%3e%3cattribute%20name%3d%22statecode%22%2f%3e%3cattribute%20name%3d%22statuscode%22%2f%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22description%22%2f%3e%3cattribute%20name%3d%22modifiedby%22%2f%3e%3cattribute%20name%3d%22ownerid%22%2f%3e%3cattribute%20name%3d%22allparties%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3corder%20attribute%3d%22modifiedon%22%20descending%3d%22true%22%2f%3e%3corder%20attribute%3d%22activityid%22%20descending%3d%22false%22%2f%3e%3clink-entity%20name%3d%22email%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22email%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22senton%22%20alias%3d%22SentOn%22%2f%3e%3cattribute%20name%3d%22delayedemailsendtime%22%20alias%3d%22DelayedemailSendTime%22%2f%3e%3cattribute%20name%3d%22lastopenedtime%22%20alias%3d%22LastOpenedTime%22%2f%3e%3cattribute%20name%3d%22isemailfollowed%22%20alias%3d%22IsEmailFollowed%22%2f%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22email_modifiedon%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22task%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22task%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22task_modifiedon%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22incidentresolution%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22incidentresolution%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22incidentresolution_modifiedon%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22opportunityclose%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22opportunityclose%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22actualrevenue%22%20alias%3d%22opportunityclose_actualrevenue%22%2f%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22opportunityclose_modifiedon%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22appointment%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22appointment%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22regardingobjectid%22%20alias%3d%22appointment_regardingobjectid%22%2f%3e%3cattribute%20name%3d%22prioritycode%22%20alias%3d%22appointment_prioritycode%22%2f%3e%3cattribute%20name%3d%22location%22%20alias%3d%22appointment_location%22%2f%3e%3cattribute%20name%3d%22scheduledstart%22%20alias%3d%22appointment_scheduledstart%22%2f%3e%3cattribute%20name%3d%22scheduleddurationminutes%22%20alias%3d%22appointment_scheduleddurationminutes%22%2f%3e%3c%2flink-entity%3e%3clink-entity%20name%3d%22phonecall%22%20from%3d%22activityid%22%20to%3d%22activityid%22%20alias%3d%22phonecall%22%20link-type%3d%22outer%22%3e%3cattribute%20name%3d%22modifiedon%22%20alias%3d%22phonecall_modifiedon%22%2f%3e%3cattribute%20name%3d%22directioncode%22%20alias%3d%22phonecall_directioncode%22%2f%3e%3c%2flink-entity%3e%3cfilter%20type%3d%22or%22%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22activitytypecode%22%20operator%3d%22in%22%3e%3cvalue%3e4201%3c%2fvalue%3e%3c%2fcondition%3e%3c%2ffilter%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%20%22activitytypecode%22%20operator%3d%20%22eq%22%20value%3d%20%224220%22%20%2f%3e%3ccondition%20attribute%3d%22regardingobjectid%22%20operator%3d%20%22eq%22%20value%3d%20%22%7b" + contactid + "%7d%22%20%2f%3e%3ccondition%20attribute%3d%22regardingobjecttypecode%22%20operator%3d%20%22eq%22%20value%3d%20%222%22%20%2f%3e%3c%2ffilter%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096656119--\r\n\0";
                request14.Body = request14Body;
                yield return request14;
                request14 = null;
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request15 = new CrmRequest(thisURL + "/api/data/v9.0/$batch");
                request15.ThinkTime = -1;
                request15.Method = "POST";
                request15.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request15.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request15.Headers.Add(new WebTestRequestHeader("Origin", thisURL + ""));
                request15.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request15.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request15.Headers.Add(new WebTestRequestHeader("client-activity-id", "d22166a4-ca32-4db6-a041-ff936e120b38"));
                request15.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request15.Headers.Add(new WebTestRequestHeader("request-id", "a9a0efac-4097-4f4f-82f7-665e3c4e5cb4"));
                request15.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1541096656120"));
                StringHttpBody request15Body = new StringHttpBody();
                request15Body.ContentType = "multipart/mixed;boundary=batch_1541096656120";
                request15Body.InsertByteOrderMark = false;
                request15Body.BodyString = "--batch_1541096656120\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts(" + contactid + ")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22true%22%20page%3d%221%22%20count%3d%221%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3cfilter%20type%3d%22or%22%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22activitytypecode%22%20operator%3d%22in%22%3e%3cvalue%3e4201%3c%2fvalue%3e%3c%2fcondition%3e%3c%2ffilter%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%20%22activitytypecode%22%20operator%3d%20%22eq%22%20value%3d%20%224220%22%20%2f%3e%3ccondition%20attribute%3d%22regardingobjectid%22%20operator%3d%20%22eq%22%20value%3d%20%22%7b" + contactid + "%7d%22%20%2f%3e%3ccondition%20attribute%3d%22regardingobjecttypecode%22%20operator%3d%20%22eq%22%20value%3d%20%222%22%20%2f%3e%3c%2ffilter%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096656120\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts(" + contactid + ")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22true%22%20page%3d%221%22%20count%3d%221%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22statecode%22%20operator%3d%22eq%22%20value%3d%220%22%2f%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096656120\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts(" + contactid + ")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22true%22%20page%3d%221%22%20count%3d%221%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22statecode%22%20operator%3d%22eq%22%20value%3d%221%22%2f%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096656120\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)?@id={\'@odata.id\':\'contacts(" + contactid + ")\'}&@xml=\'%3cfetch%20version%3d%221.0%22%20output-format%3d%22xml-platform%22%20mapping%3d%22logical%22%20distinct%3d%22true%22%20returntotalrecordcount%3d%22true%22%20page%3d%221%22%20count%3d%221%22%20no-lock%3d%22true%22%3e%3centity%20name%3d%22activitypointer%22%3e%3cattribute%20name%3d%22activityid%22%2f%3e%3cattribute%20name%3d%22exchangeweblink%22%2f%3e%3cattribute%20name%3d%22exchangeitemid%22%2f%3e%3cfilter%20type%3d%22and%22%3e%3ccondition%20attribute%3d%22scheduledend%22%20operator%3d%22lt%22%20value%3d%222018-11-01T18%3a24%3a16.119Z%22%2f%3e%3ccondition%20attribute%3d%22statecode%22%20operator%3d%22ne%22%20value%3d%221%22%2f%3e%3c%2ffilter%3e%3c%2fentity%3e%3c%2ffetch%3e\'&@rollupType=2 HTTP/1.1\nAccept: application/json\nContent-Type: application/json; charset=utf-8\nPrefer: odata.include-annotations=*\nOData-MaxVersion: 4.0\nOData-Version: 4.0\n\n--batch_1541096656120--\r\n\0";
                request15.Body = request15Body;
                yield return request15;
                request15 = null;
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request16 = new CrmRequest(thisURL + "/api/data/v9.0/organizations(" + orgId + ")");
                request16.ThinkTime = 1;
                request16.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request16.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request16.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
                request16.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request16.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request16.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
                request16.Headers.Add(new WebTestRequestHeader("client-activity-id", "d22166a4-ca32-4db6-a041-ff936e120b38"));
                request16.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request16.Headers.Add(new WebTestRequestHeader("request-id", "557b4369-625f-4642-a865-b720ccd13b16"));
                request16.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
                request16.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"3976562\""));
                request16.QueryStringParameters.Add("$select", "yammernetworkpermalink,yammergroupid,yammeroauthaccesstokenexpired");
                yield return request16;
                request16 = null;
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request17 = new CrmRequest(thisURL + "/api/data/v9.0/organizations(" + orgId + ")");
                request17.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request17.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request17.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
                request17.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request17.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request17.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
                request17.Headers.Add(new WebTestRequestHeader("client-activity-id", "d22166a4-ca32-4db6-a041-ff936e120b38"));
                request17.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request17.Headers.Add(new WebTestRequestHeader("request-id", "0d331cfa-6110-4f62-85d1-c635573e03dc"));
                request17.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
                request17.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"3976562\""));
                request17.QueryStringParameters.Add("$select", "yammernetworkpermalink,yammergroupid,yammeroauthaccesstokenexpired,yammerpostmethod");
                yield return request17;
                request17 = null;
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request21 = new CrmRequest(thisURL + "/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
                request21.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request21.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request21.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
                request21.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request21.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request21.Headers.Add(new WebTestRequestHeader("client-activity-id", "b2edc8b0-f2c7-4789-b8eb-74e5837fda08"));
                request21.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request21.Headers.Add(new WebTestRequestHeader("request-id", "5436470b-0974-4495-83bd-670481e2c2df"));
                request21.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
                request21.QueryStringParameters.Add("@ClientMetadataQuery", @"{""MetadataType"":""metadataversion"",""MetadataSubtype"":null,""EntityLogicalName"":null,""MetadataId"":null,""MetadataNames"":[],""GetDefault"":false,""DependencyDepth"":""OnDemandWithoutContext"",""ChangedAfter"":""3975641"",""Exclude"":[],""AppId"":""b8ac196d-94c5-e811-a96c-000d3a16a650""}");
                request21.QueryStringParameters.Add("umv", "3975641");
                request21.QueryStringParameters.Add("mv", "3976327");
                yield return request21;
                request21 = null;
                this.AddCommentToResult("Open Contact Record");

                CrmRequest request22 = new CrmRequest(thisURL + "/api/data/v9.0/UpdateRecentItems");
                request22.Method = "POST";
                request22.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
                request22.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
                request22.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
                request22.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
                request22.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "b8ac196d-94c5-e811-a96c-000d3a16a650"));
                request22.Headers.Add(new WebTestRequestHeader("client-activity-id", "b2edc8b0-f2c7-4789-b8eb-74e5837fda08"));
                request22.Headers.Add(new WebTestRequestHeader("client-session-id", "bc5af60f-66db-4c5f-ad10-978292a0d40e"));
                request22.Headers.Add(new WebTestRequestHeader("request-id", "ca0523cb-b257-463c-87cf-bdc90b66bc3a"));
                request22.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
                request22.Headers.Add(new WebTestRequestHeader("Origin", thisURL + ""));
                StringHttpBody request22Body = new StringHttpBody();
                request22Body.ContentType = "application/json";
                request22Body.InsertByteOrderMark = false;
                request22Body.BodyString = "{\"items\":[{\"EntityTypeName\":\"contact\",\"LastAccessedStr\":\"2018-11-01T18:24:16.269Z\",\"ObjectId\":\"{" + contactid + "}\",\"PinStatus\":false,\"RecordType\":\"Entity\",\"Title\":\"lt_E6mCoI2NE lt_epQ9D7Th8\",\"IconPath\":null,\"IsDeleted\":false}]}";
                request22.Body = request22Body;
                yield return request22;
                request22 = null;
                this.EndTransaction("1051.03_Open Contact Record");
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
