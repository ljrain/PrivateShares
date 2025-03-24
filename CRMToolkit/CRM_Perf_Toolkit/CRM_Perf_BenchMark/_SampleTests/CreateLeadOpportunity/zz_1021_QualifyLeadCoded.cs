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
    public class zz_1021_QualifyLeadCoded : WebTestBase
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
        private string leadId;
        public zz_1021_QualifyLeadCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1021_QualifyLeadCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1021_QualifyLeadCoded_PostWebTest);
        }
        public void zz_1021_QualifyLeadCoded_PreWebTest(object sender, PreWebTestEventArgs e)
        {
            //Select Existing Lead to be Qualified
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

                    ////Set User to InUse
                    cmd.CommandText = "UPDATE ltlead set inuse='1', uid = '" + ukey + "' where leadid in (select top 1 leadId from ltlead where state = 0 and inuse = 0 order by newid())";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = string.Format("SELECT top 1 rtrim(leadId),rtrim(systemuserId) from ltlead(nolock) where uid = '" + ukey + "'");

                    System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                    cmd.Parameters.Clear();

                    //Lock this guy and commit the transaction.
                    while (true == reader.Read())
                    {
                        //Read User
                        leadId = reader[0].ToString();
                        systemuserId = reader[1].ToString();

                        //Filter User to the person that originally created the lead
                        Context.Add("systemuserid", systemuserId);
                    }
                    if (!reader.IsClosed)
                        reader.Close();

                    tran.Commit();
                }
                sqlConn.Close();
            }

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
        public void zz_1021_QualifyLeadCoded_PostWebTest(object sender, PostWebTestEventArgs e)
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
            this.BeginTransaction("1021.01_Navigate to My Open Leads View");
            this.AddCommentToResult("01_Navigate to My Open Leads View");

            CrmRequest request1 = new CrmRequest(thisURL+"/api/data/v9.0/leads");
            request1.ThinkTime = 1;
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request1.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Grid; Source=Default"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "e4a0745b-8b49-48dd-9768-87101426fc88"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request1.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "00000000-0000-0000-00aa-000010001005"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "8d958742-1fd1-4f3b-a586-03ffe19c9867"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request1.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request1.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""50"" no-lock=""false""><entity name=""lead""><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""fullname""/><order attribute=""createdon"" descending=""true""/><filter type=""and""><condition attribute=""ownerid"" operator=""eq-userid""/><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><attribute name=""statuscode""/><attribute name=""createdon""/><attribute name=""subject""/><attribute name=""leadid""/><attribute name=""companyname""/></entity></fetch>");
            yield return request1;
            request1 = null;
            this.AddCommentToResult("02_Navigate to My Open Leads View");

            CrmRequest request2 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request2.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-metadatareductionlevel", "5"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "6434c5fa-6066-410e-a3af-dd01dde9a5b2"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request2.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "b1678050-e9a9-4c9f-9213-64efa55a5fa4"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request2.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request2.QueryStringParameters.Add("@ClientMetadataQuery", "{\"MetadataType\":\"metadataversion\",\"GetDefault\":false,\"ChangedAfter\":\"2151247\",\"AppId\":\"a9a86d48-06ad-ec11-9841-000d3a3bc2af\"}");
            request2.QueryStringParameters.Add("api-version", "9.1");
            yield return request2;
            request2 = null;
            this.AddCommentToResult("03_Navigate to My Open Leads View");

            CrmRequest request3 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request3.ThinkTime = 10;
            request3.Method = "POST";
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request3.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "bfb909e1-69a2-4bb6-9715-a7c80d14dbf2"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request3.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "b1678050-e9a9-4c9f-9213-64efa55a5fa4"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request3.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request3.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request3Body = new StringHttpBody();
            request3Body.ContentType = "application/json";
            request3Body.InsertByteOrderMark = false;
            request3Body.BodyString = "{\"items\":[{\"LastAccessedStr\":\"2022-03-29T15:00:30.542Z\",\"EntityTypeName\":\"lead\",\"ObjectId\":\"{00000000-0000-0000-00AA-000010001005}\",\"RecordType\":\"Grid\",\"Title\":\"My Open Leads\",\"PinStatus\":null,\"IconPath\":null,\"IsDeleted\":false,\"IsUserView\":false}]}";
            request3.Body = request3Body;
            yield return request3;
            request3 = null;
            this.EndTransaction("1021.01_Navigate to My Open Leads View");
            this.BeginTransaction("1021.02_Quick Find Search for Lead");
            this.AddCommentToResult("04_Quick Find Search for Lead");

            CrmRequest request4 = new CrmRequest(thisURL+"/api/data/v9.0/leads");
            request4.ThinkTime = 1;
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request4.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Quickfind; Source=Default"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "be7bb1ef-4187-4130-a99c-05d4e3c43366"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request4.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "00000000-0000-0000-00aa-000010001005"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9dd3245d-5668-4635-aa12-64b6b00bccd8"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request4.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request4.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""50"" no-lock=""false""><entity name=""lead""><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""fullname""/><order attribute=""createdon"" descending=""true""/><filter type=""and""><condition attribute=""ownerid"" operator=""eq-userid""/><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><attribute name=""statuscode""/><attribute name=""createdon""/><attribute name=""subject""/><attribute name=""leadid""/><attribute name=""companyname""/><filter type=""or"" isquickfindfields=""1""><condition attribute=""subject"" operator=""like"" value=""ltlead%""/><condition attribute=""fullname"" operator=""like"" value=""ltlead%""/><condition attribute=""firstname"" operator=""like"" value=""ltlead%""/><condition attribute=""lastname"" operator=""like"" value=""ltlead%""/><condition attribute=""companyname"" operator=""like"" value=""ltlead%""/></filter></entity></fetch>");
            yield return request4;
            request4 = null;
            this.AddCommentToResult("05_Quick Find Search for Lead");

            CrmRequest request5 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request5.ThinkTime = 8;
            request5.Method = "POST";
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request5.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "ccc155ca-f02e-4757-a019-8c1c7e717291"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request5.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "6a607f45-bb71-4ed6-b343-9a2d391f9dc2"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request5.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request5.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request5Body = new StringHttpBody();
            request5Body.ContentType = "application/json";
            request5Body.InsertByteOrderMark = false;
            request5Body.BodyString = "{\"items\":[{\"LastAccessedStr\":\"2022-03-29T15:00:41.985Z\",\"EntityTypeName\":\"lead\",\"ObjectId\":\"{00000000-0000-0000-00AA-000010001005}\",\"RecordType\":\"Grid\",\"Title\":\"My Open Leads\",\"PinStatus\":null,\"IconPath\":null,\"IsDeleted\":false,\"IsUserView\":null}]}";
            request5.Body = request5Body;
            yield return request5;
            request5 = null;
            this.EndTransaction("1021.02_Quick Find Search for Lead");
            this.BeginTransaction("1021.03_Open Lead Record");
            this.AddCommentToResult("06_Open Lead Record");

            CrmRequest request6 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request6.Method = "POST";
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request6.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "69015da2-1c54-4500-bff7-99096eadf25a"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "0695dec8-263d-45ba-8507-fec2ef701a2a"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request6.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566051066"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request6.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request6Body = new StringHttpBody();
            request6Body.ContentType = "multipart/mixed;boundary=batch_1648566051066";
            request6Body.InsertByteOrderMark = false;
            request6Body.BodyString = "--batch_1648566051066\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/leads("+leadId+")?$select=subject,firstname,middlename,lastname,jobtitle,telephone1,mobilephone,emailaddress1,companyname,websiteurl,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,businesscard,description,industrycode,revenue,numberofemployees,sic,_transactioncurrencyid_value,_campaignid_value,donotsendmm,lastusedincampaign,preferredcontactmethodcode,donotemail,followemail,donotbulkemail,donotphone,donotpostalmail,leadsourcecode,leadqualitycode,statuscode,_ownerid_value,_parentaccountid_value,_parentcontactid_value,businesscardattributes,fullname,statecode,leadid,entityimage_url HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 2b8f6bc7-a500-4809-b662-3d1fd5e208f1\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 69015da2-1c54-4500-bff7-99096eadf25a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566051066\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: beeef484-7e25-43c9-9c73-57491ec5f008\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 69015da2-1c54-4500-bff7-99096eadf25a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566051066\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: a6b9be6a-915c-4ea8-8634-b1912c7ff0d6\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 69015da2-1c54-4500-bff7-99096eadf25a\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566051066--\r\n\0";
            request6.Body = request6Body;
            yield return request6;
            string resresult6 = request6.lastResponse.BodyString;
            request6 = null;
            string bpfId = Utils.extractUCIgridresponse("businessprocessflowinstanceid\":\"", resresult6);
            this.AddCommentToResult("07_Open Lead Record");

            CrmRequest request7 = new CrmRequest(thisURL+"/api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)");
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request7.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "798e46ae-88e3-4533-8b83-b6c3191e5b1f"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request7.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request7.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=*"));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "69015da2-1c54-4500-bff7-99096eadf25a"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request7.Headers.Add(new WebTestRequestHeader("content-type", "application/json; charset=utf-8"));
            request7.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request7.QueryStringParameters.Add("@id", "null");
            request7.QueryStringParameters.Add("@xml", "\'<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\" returntotalrecordcount=\"false\" page=\"1\" count=\"10\" no-lock=\"true\"><entity name=\"activitypointer\"><attribute name=\"subject\"/><attribute name=\"activitytypecode\"/><attribute name=\"statecode\"/><attribute name=\"statuscode\"/><attribute name=\"activityid\"/><attribute name=\"description\"/><attribute name=\"modifiedby\"/><attribute name=\"ownerid\"/><attribute name=\"allparties\"/><attribute name=\"modifiedon\"/><attribute name=\"scheduledstart\"/><attribute name=\"createdby\"/><attribute name=\"scheduledend\"/><order attribute=\"modifiedon\" descending=\"true\"/><order attribute=\"activityid\" descending=\"false\"/><link-entity name=\"email\" from=\"activityid\" to=\"activityid\" alias=\"email\" link-type=\"outer\"><attribute name=\"senton\" alias=\"EmailSentOn\"/><attribute name=\"delayedemailsendtime\" alias=\"DelayedemailSendTime\"/><attribute name=\"lastopenedtime\" alias=\"LastOpenedTime\"/><attribute name=\"isemailfollowed\" alias=\"IsEmailFollowed\"/><attribute name=\"baseconversationindexhash\" alias=\"BaseConversationIndexHash\"/></link-entity><link-entity name=\"opportunityclose\" from=\"activityid\" to=\"activityid\" alias=\"opportunityclose\" link-type=\"outer\"><attribute name=\"actualrevenue\" alias=\"opportunityclose_actualrevenue\"/></link-entity><link-entity name=\"phonecall\" from=\"activityid\" to=\"activityid\" alias=\"phonecall\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"phonecall_directioncode\"/></link-entity><link-entity name=\"letter\" from=\"activityid\" to=\"activityid\" alias=\"letter\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"letter_directioncode\"/></link-entity><link-entity name=\"fax\" from=\"activityid\" to=\"activityid\" alias=\"fax\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"fax_directioncode\"/></link-entity><link-entity name=\"activityparty\" from=\"activityid\" to=\"activityid\" alias=\"activityparty\" link-type=\"in\"><filter type=\"and\"><filter type=\"and\"><condition attribute=\"partyid\" operator=\"eq\" value=\""+leadId+"\"/><condition attribute=\"partyobjecttypecode\" operator=\"eq\" value=\"4\"/></filter></filter></link-entity><filter type=\"and\"><condition attribute=\"activitytypecode\" operator=\"in\"><value>4402</value><value>4206</value><value>4202</value><value>4204</value><value>4207</value><value>4201</value><value>4208</value><value>4209</value><value>4210</value><value>4211</value><value>4251</value><value>4216</value><value>4212</value><value>10315</value><value>10325</value><value>10327</value></condition></filter></entity></fetch>\'");
            request7.QueryStringParameters.Add("@rollupType", "-1");
            yield return request7;
            request7 = null;
            this.AddCommentToResult("08_Open Lead Record");

            CrmRequest request8 = new CrmRequest(thisURL+"/api/data/v9.0/RetrieveTenantInfo");
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "69015da2-1c54-4500-bff7-99096eadf25a"));
            request8.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "eb64806a-ba66-46ca-bc14-8844709a0996"));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request8;
            request8 = null;
            this.AddCommentToResult("09_Open Lead Record");

            CrmRequest request9 = new CrmRequest(thisURL+"/api/data/v9.0/roles");
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "69015da2-1c54-4500-bff7-99096eadf25a"));
            request9.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "a1fdcf37-fbcb-4e9a-ba18-43dfc8448ec9"));
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request9.QueryStringParameters.Add("$filter", "_roletemplateid_value eq \'627090ff-40a3-4053-8790-584edc5be201\'");
            request9.QueryStringParameters.Add("$select", "roleid");
            yield return request9;
            request9 = null;
            this.AddCommentToResult("10_Open Lead Record");

            CrmRequest request10 = new CrmRequest(thisURL+"/api/data/v9.0/annotations");
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request10.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=RetrieveMultipleOther; Source=Default"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "84ef1c68-d196-4409-b0d0-ef459c39b413"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request10.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "undefined"));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "69015da2-1c54-4500-bff7-99096eadf25a"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request10.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request10.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""true"" returntotalrecordcount=""false"" page=""1"" count=""10"" no-lock=""false""><entity name=""annotation""><attribute name=""annotationid""/><attribute name=""subject""/><attribute name=""notetext""/><attribute name=""filename""/><attribute name=""filesize""/><attribute name=""isdocument""/><attribute name=""createdby""/><attribute name=""createdon""/><attribute name=""modifiedby""/><attribute name=""modifiedon""/><attribute name=""mimetype""/><order attribute=""modifiedon"" descending=""true""/><order attribute=""annotationid"" descending=""false""/><link-entity name=""systemuser"" from=""systemuserid"" to=""modifiedby"" alias=""systemuser"" link-type=""outer""><attribute name=""entityimage_url""/><attribute name=""systemuserid""/><attribute name=""fullname""/></link-entity><filter type=""and""><filter type=""and""><condition attribute=""objectid"" operator=""eq"" value="""+leadId+@"""/></filter></filter></entity></fetch>");
            yield return request10;
            request10 = null;
            this.AddCommentToResult("11_Open Lead Record");

            CrmRequest request11 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request11.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "b51cf8b7-ad5d-4e04-9d2b-aea6b67ba56a"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request11.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "69015da2-1c54-4500-bff7-99096eadf25a"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request11.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request11.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""lead"" from=""leadid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""leadid"" operator=""eq"" uitype=""lead"" value="""+leadId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request11;
            request11 = null;
            this.AddCommentToResult("12_Open Lead Record");

            CrmRequest request12 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request12.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "781594ef-5908-4b93-8a9c-c13e7812b3a6"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request12.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "69015da2-1c54-4500-bff7-99096eadf25a"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request12.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request12.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""leadcompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""lead"" from=""leadid"" to=""leadid"" alias=""bb""> <filter type=""and"">  <condition attribute=""leadid"" operator=""eq"" uitype=""lead"" value="""+leadId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request12;
            request12 = null;
            this.AddCommentToResult("13_Open Lead Record");

            CrmRequest request13 = new CrmRequest(thisURL+"/api/data/v9.0/systemusers("+systemuserId+")");
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request13.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request13.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "24030566-6c8e-47ad-b2ae-01f9d43e9432"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request13.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "69015da2-1c54-4500-bff7-99096eadf25a"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request13.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request13.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"2150652\""));
            yield return request13;
            request13 = null;
            this.AddCommentToResult("14_Open Lead Record");

            CrmRequest request14 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request14.ThinkTime = 1;
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request14.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "69384e8a-1320-4865-937e-3f16db5d4757"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request14.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "69015da2-1c54-4500-bff7-99096eadf25a"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request14.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));            
            request14.QueryStringParameters.Add("fetchXml", WebUtility.UrlEncode("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><filter type=\"and\"><condition attribute=\"expirydate\" operator=\"ge\" value=\"03/29/2022 15:00\"/></filter><filter type=\"and\"><condition attribute=\"recordid\" operator=\"eq\" value=\""+leadId+"\"/></filter><filter type=\"and\"><condition attribute=\"recordidobjecttypecode2\" operator=\"eq\" value=\"4\"/></filter></filter></entity></fetch>"));
            yield return request14;
            request14 = null;
            this.AddCommentToResult("15_Open Lead Record");

            CrmRequest request15 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request15.Method = "POST";
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request15.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "1ad977d3-4092-4486-973e-ecde0d91da3e"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request15.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "5bc8ba9a-64de-4025-8326-ba9cd2e4dc26"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request15.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request15.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request15Body = new StringHttpBody();
            request15Body.ContentType = "application/json";
            request15Body.InsertByteOrderMark = false;
            request15Body.BodyString = "{\"items\":[{\"LastAccessedStr\":\"2022-03-29T15:00:51.872Z\",\"EntityTypeName\":\"lead\",\"ObjectId\":\"{"+leadId+"}\",\"RecordType\":\"Entity\",\"Title\":\"ltlead959 ltlead959\",\"PinStatus\":null,\"IconPath\":null,\"IsDeleted\":false,\"IsUserView\":null}]}";
            request15.Body = request15Body;
            yield return request15;
            request15 = null;
            this.AddCommentToResult("16_Open Lead Record");

            CrmRequest request16 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request16.ThinkTime = 20;
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request16.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-metadatareductionlevel", "5"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "c2e1e4f6-6464-4305-97f9-df5a620d0c70"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request16.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "5bc8ba9a-64de-4025-8326-ba9cd2e4dc26"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request16.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request16.QueryStringParameters.Add("@ClientMetadataQuery", "{\"MetadataType\":\"metadataversion\",\"GetDefault\":false,\"ChangedAfter\":\"2151247\",\"AppId\":\"a9a86d48-06ad-ec11-9841-000d3a3bc2af\"}");
            request16.QueryStringParameters.Add("api-version", "9.1");
            yield return request16;
            request16 = null;
            this.EndTransaction("1021.03_Open Lead Record");
            this.BeginTransaction("1021.04_Update Lead and Click Save");
            this.AddCommentToResult("17_Update Lead and Click Save");

            CrmRequest request17 = new CrmRequest(thisURL+"/api/data/v9.0/leads("+leadId+")");
            request17.Method = "PATCH";
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request17.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request17.Headers.Add(new WebTestRequestHeader("mscrm.suppressduplicatedetection", "false"));
            request17.Headers.Add(new WebTestRequestHeader("if-match", "*"));
            request17.Headers.Add(new WebTestRequestHeader("autodisassociate", "true"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "cfd5a092-720c-442c-9d42-2d681c144c33"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request17.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "1399b76f-864b-43a2-97bf-9f9a25875dc9"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request17.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request17.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request17Body = new StringHttpBody();
            request17Body.ContentType = "application/json";
            request17Body.InsertByteOrderMark = false;
            request17Body.BodyString = "{\"jobtitle\":\"IT Manager\",\"address1_country\":\"USA\"}";
            request17.Body = request17Body;
            yield return request17;
            request17 = null;
            this.AddCommentToResult("18_Update Lead and Click Save");

            CrmRequest request18 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request18.Method = "POST";
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request18.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "1399b76f-864b-43a2-97bf-9f9a25875dc9"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "6d2cd8b9-dfda-42e1-93d1-8cfa9efa20b6"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request18.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566095962"));
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request18.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request18Body = new StringHttpBody();
            request18Body.ContentType = "multipart/mixed;boundary=batch_1648566095962";
            request18Body.InsertByteOrderMark = false;
            request18Body.BodyString = "--batch_1648566095962\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/leads("+leadId+")?$select=subject,firstname,middlename,lastname,jobtitle,telephone1,mobilephone,emailaddress1,companyname,websiteurl,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,address1_composite,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,businesscard,description,industrycode,revenue,numberofemployees,sic,_transactioncurrencyid_value,_campaignid_value,donotsendmm,lastusedincampaign,preferredcontactmethodcode,donotemail,followemail,donotbulkemail,donotphone,donotpostalmail,leadsourcecode,leadqualitycode,statuscode,_ownerid_value,_parentaccountid_value,_parentaccountid_value,_parentcontactid_value,_parentcontactid_value,businesscardattributes,statecode,leadid,entityimage_url,fullname,businesscard,businesscardattributes,statecode,purchasetimeframe,budgetamount,purchaseprocess,decisionmaker HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 64aecc6c-9008-44d1-8679-b4ffbffdd217\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 1399b76f-864b-43a2-97bf-9f9a25875dc9\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566095962\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 37f5e9a4-0f91-4604-bc77-a7cb3a911ab4\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 1399b76f-864b-43a2-97bf-9f9a25875dc9\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566095962\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 67055774-2187-482c-bc18-20d431e2611c\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 1399b76f-864b-43a2-97bf-9f9a25875dc9\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566095962--\r\n\0";
            request18.Body = request18Body;
            yield return request18;
            request18 = null;
            this.AddCommentToResult("19_Update Lead and Click Save");

            CrmRequest request19 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request19.Method = "POST";
            request19.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request19.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request19.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "1399b76f-864b-43a2-97bf-9f9a25875dc9"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "d3271c88-5cd6-45c4-a58e-2ddaf6c4914c"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request19.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566096384"));
            request19.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request19.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request19Body = new StringHttpBody();
            request19Body.ContentType = "multipart/mixed;boundary=batch_1648566096384";
            request19Body.InsertByteOrderMark = false;
            request19Body.BodyString = "--batch_1648566096384\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/leads("+leadId+")?$select=subject,firstname,middlename,lastname,jobtitle,telephone1,mobilephone,emailaddress1,companyname,websiteurl,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,businesscard,description,industrycode,revenue,numberofemployees,sic,_transactioncurrencyid_value,_campaignid_value,donotsendmm,lastusedincampaign,preferredcontactmethodcode,donotemail,followemail,donotbulkemail,donotphone,donotpostalmail,leadsourcecode,leadqualitycode,statuscode,_ownerid_value,_parentaccountid_value,_parentcontactid_value,businesscardattributes,statecode,purchasetimeframe,budgetamount,purchaseprocess,decisionmaker,statecode,leadid,entityimage_url,fullname HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 1fbc5597-b6f9-457f-8a02-5a922fce8b36\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 1399b76f-864b-43a2-97bf-9f9a25875dc9\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566096384\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 1eb19983-79bd-494b-8807-9ee6a693df3a\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 1399b76f-864b-43a2-97bf-9f9a25875dc9\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566096384\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: c570c8e0-ed49-45f5-9edf-b9459de33296\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 1399b76f-864b-43a2-97bf-9f9a25875dc9\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566096384--\r\n\0";
            request19.Body = request19Body;
            yield return request19;
            request19 = null;
            this.AddCommentToResult("20_Update Lead and Click Save");

            CrmRequest request20 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request20.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "168f2c2d-f97b-4366-a726-775382fe436a"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request20.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "1399b76f-864b-43a2-97bf-9f9a25875dc9"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request20.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request20.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""lead"" from=""leadid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""leadid"" operator=""eq"" uitype=""lead"" value="""+leadId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request20;
            request20 = null;
            this.AddCommentToResult("21_Update Lead and Click Save");

            CrmRequest request21 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request21.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request21.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "99a32c53-695e-4eb3-b815-5387c26ed835"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request21.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request21.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request21.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "1399b76f-864b-43a2-97bf-9f9a25875dc9"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request21.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request21.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request21.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request21.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request21.QueryStringParameters.Add("fetchXml", WebUtility.UrlEncode("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><filter type=\"and\"><condition attribute=\"startdate\" operator=\"le\" value=\"03/29/2022 15:00\"/></filter><filter type=\"and\"><condition attribute=\"expirydate\" operator=\"ge\" value=\"03/29/2022 15:00\"/></filter><filter type=\"and\"><condition attribute=\"recordid\" operator=\"eq\" value=\"" +leadId+"\"/></filter><filter type=\"and\"><condition attribute=\"recordidobjecttypecode2\" operator=\"eq\" value=\"4\"/></filter></filter></entity></fetch>"));
            yield return request21;
            request21 = null;
            this.AddCommentToResult("22_Update Lead and Click Save");

            CrmRequest request22 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request22.ThinkTime = 20;
            request22.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request22.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "9264e1bb-a509-41ba-bed6-59d3f7b92cc5"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request22.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request22.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request22.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "1399b76f-864b-43a2-97bf-9f9a25875dc9"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request22.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request22.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request22.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request22.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request22.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""leadcompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""lead"" from=""leadid"" to=""leadid"" alias=""bb""> <filter type=""and"">  <condition attribute=""leadid"" operator=""eq"" uitype=""lead"" value="""+leadId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request22;
            request22 = null;
            this.EndTransaction("1021.04_Update Lead and Click Save");
            this.BeginTransaction("1021.05_Click Qualify Button");
            this.AddCommentToResult("23_Click Qualify Button");

            CrmRequest request23 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request23.Method = "POST";
            request23.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request23.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request23.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "36cbc017-e268-4dd5-80d9-6e81f82a455b"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request23.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566182231"));
            request23.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request23.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request23Body = new StringHttpBody();
            request23Body.ContentType = "multipart/mixed;boundary=batch_1648566182231";
            request23Body.InsertByteOrderMark = false;
            request23Body.BodyString = "--batch_1648566182231\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/leads("+leadId+")?$select=subject,firstname,middlename,lastname,jobtitle,telephone1,mobilephone,emailaddress1,companyname,websiteurl,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,address1_composite,businesscard,description,industrycode,revenue,numberofemployees,sic,_transactioncurrencyid_value,_campaignid_value,donotsendmm,lastusedincampaign,preferredcontactmethodcode,donotemail,followemail,donotbulkemail,donotphone,donotpostalmail,leadsourcecode,leadqualitycode,statuscode,_ownerid_value,_parentaccountid_value,_parentcontactid_value,businesscardattributes,statecode,purchasetimeframe,budgetamount,purchaseprocess,decisionmaker,statecode,leadid,entityimage_url,fullname HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 3bb04bfc-ec7a-4722-b5eb-4c716d19e1c2\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: cc9ab558-bbdc-42d9-90ed-dffb112ab7d1\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566182231\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: f138e241-331a-457f-9dc2-1ec972175d0b\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: cc9ab558-bbdc-42d9-90ed-dffb112ab7d1\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566182231\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22leads("+leadId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 52e94f79-8c4b-4d15-979a-f8c999d74e7b\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: cc9ab558-bbdc-42d9-90ed-dffb112ab7d1\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566182231--\r\n\0";
            request23.Body = request23Body;
            yield return request23;
            request23 = null;
            this.AddCommentToResult("24_Click Qualify Button");

            CrmRequest request24 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request24.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request24.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "d587158c-28a6-48a4-ba3e-483464d69e80"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request24.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request24.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request24.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request24.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request24.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request24.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request24.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request24.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""lead"" from=""leadid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""leadid"" operator=""eq"" uitype=""lead"" value="""+leadId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request24;
            request24 = null;
            this.AddCommentToResult("25_Click Qualify Button");

            CrmRequest request25 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request25.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request25.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "f03b8f5f-0c79-4cf2-80c5-8ddf9f9eec78"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request25.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request25.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request25.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request25.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request25.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request25.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request25.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request25.QueryStringParameters.Add("fetchXml", WebUtility.UrlEncode("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><filter type=\"and\"><condition attribute=\"startdate\" operator=\"le\" value=\"03/29/2022 15:00\"/></filter><filter type=\"and\"><condition attribute=\"expirydate\" operator=\"ge\" value=\"03/29/2022 15:00\"/></filter><filter type=\"and\"><condition attribute=\"recordid\" operator=\"eq\" value=\"" +leadId+"\"/></filter><filter type=\"and\"><condition attribute=\"recordidobjecttypecode2\" operator=\"eq\" value=\"4\"/></filter></filter></entity></fetch>"));
            yield return request25;
            request25 = null;
            this.AddCommentToResult("26_Click Qualify Button");

            CrmRequest request26 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request26.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request26.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "f6f37291-ebfa-4f64-87f9-1ddeff21f77a"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request26.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request26.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request26.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request26.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request26.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request26.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request26.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request26.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""leadcompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""lead"" from=""leadid"" to=""leadid"" alias=""bb""> <filter type=""and"">  <condition attribute=""leadid"" operator=""eq"" uitype=""lead"" value="""+leadId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request26;
            request26 = null;
            this.AddCommentToResult("27_Click Qualify Button");

            CrmRequest request27 = new CrmRequest(thisURL+"/api/data/v9.0/leads("+leadId+")/Microsoft.Dynamics.CRM.QualifyLead");
            request27.ThinkTime = 5;
            request27.Method = "POST";
            request27.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request27.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "0f1f090f-fc91-446d-a264-5c37b7f87439"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request27.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request27.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request27.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request27.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request27.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request27.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request27.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request27.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request27Body = new StringHttpBody();
            request27Body.ContentType = "application/json";
            request27Body.InsertByteOrderMark = false;            
            request27Body.BodyString = @"{""entity"":{""@odata.type"":""Microsoft.Dynamics.CRM.lead"",""leadid"":"""+leadId+@"""},""CreateAccount"":true,""CreateContact"":true,""CreateOpportunity"":true,""OpportunityCustomerId"":{},""Status"":-1,""SuppressDuplicateDetection"":false,""SourceCampaignId"":{},""ProcessInstanceId"":{""@odata.type"":""Microsoft.Dynamics.CRM.businessprocessflowinstance"",""businessprocessflowinstanceid"":"""+bpfId+@"""}}";
            request27.Body = request27Body;
            yield return request27;
            //opportunityid":""+opportunityId+"
            string resresult = request27.lastResponse.BodyString;
            request27 = null;
            string opportunityId = Utils.extractUCIgridresponse("opportunityid\":\"", resresult);
            string contactId = Utils.extractUCIgridresponse("contactid\":\"", resresult);
            string accountId = Utils.extractUCIgridresponse("accountid\":\"", resresult);
            this.AddCommentToResult("28_Click Qualify Button");

            CrmRequest request28 = new CrmRequest(thisURL+"/api/data/v9.0/organizations("+orgId+")");
            request28.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request28.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request28.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "e0d0bd6e-50b8-4c74-9931-f68a7589006e"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request28.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request28.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request28.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request28.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request28.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request28.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request28.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request28.QueryStringParameters.Add("$select", "enhancedoqoiaddproductssettings");
            yield return request28;
            request28 = null;
            this.AddCommentToResult("29_Click Qualify Button");

            CrmRequest request29 = new CrmRequest(thisURL+"/api/data/v9.0/GetRecentItems");
            request29.Method = "POST";
            request29.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request29.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "397d9d12-8dad-4f9a-ba4b-cd18bac4959f"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request29.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request29.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request29.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request29.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request29.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request29.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request29.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request29.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request29Body = new StringHttpBody();
            request29Body.ContentType = "application/json";
            request29Body.InsertByteOrderMark = false;
            request29Body.BodyString = "{\"entityNames\":[\"opportunity\"],\"preferredPinnedItems\":null,\"preferredPinnedItemsPerEntity\":null,\"preferredUnpinnedItems\":null,\"preferredUnpinnedItemsPerEntity\":null,\"appId\":\"00000000-0000-0000-0000-000000000000\"}";
            request29.Body = request29Body;
            yield return request29;
            request29 = null;
            this.AddCommentToResult("30_Click Qualify Button");

            CrmRequest request30 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request30.Method = "POST";
            request30.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request30.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request30.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "63b26b9d-f5e2-4614-98b5-30e1442765b6"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request30.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566199756"));
            request30.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request30.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request30.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request30.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request30.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request30Body = new StringHttpBody();
            request30Body.ContentType = "multipart/mixed;boundary=batch_1648566199756";
            request30Body.InsertByteOrderMark = false;
            request30Body.BodyString = "--batch_1648566199756\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities("+opportunityId+")?$select=name,_parentcontactid_value,_parentaccountid_value,purchasetimeframe,_transactioncurrencyid_value,budgetamount,purchaseprocess,msdyn_forecastcategory,description,currentsituation,customerneed,proposedsolution,_pricelevelid_value,isrevenuesystemcalculated,totallineitemamount,discountpercentage,discountamount,totalamountlessfreight,freightamount,totaltax,totalamount,estimatedclosedate,estimatedvalue,statuscode,_ownerid_value,statecode,opportunityid HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 018b0ed8-a9ae-4fd1-b4da-215a547dcdbd\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: cc9ab558-bbdc-42d9-90ed-dffb112ab7d1\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566199756\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: e22e0133-a0f6-43a8-af05-72e1b66d2088\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: cc9ab558-bbdc-42d9-90ed-dffb112ab7d1\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566199756\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 8a79ae91-edaa-488d-b9c6-09d9c81f3ddf\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: cc9ab558-bbdc-42d9-90ed-dffb112ab7d1\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566199756--\r\n\0";
            request30.Body = request30Body;
            yield return request30;
            request30 = null;
            this.AddCommentToResult("31_Click Qualify Button");

            CrmRequest request31 = new CrmRequest(thisURL+"/api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)");
            request31.ThinkTime = -1;
            request31.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request31.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "1b1bcf21-1415-4f5f-a544-efe547e14e29"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request31.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request31.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request31.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=*"));
            request31.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request31.Headers.Add(new WebTestRequestHeader("content-type", "application/json; charset=utf-8"));
            request31.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request31.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request31.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request31.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request31.QueryStringParameters.Add("@id", "{\'@odata.id\':\'opportunities("+opportunityId+")\'}");
            request31.QueryStringParameters.Add("@xml", "\'<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\" returntotalrecordcount=\"false\" page=\"1\" count=\"10\" no-lock=\"true\"><entity name=\"activitypointer\"><attribute name=\"subject\"/><attribute name=\"activitytypecode\"/><attribute name=\"statecode\"/><attribute name=\"statuscode\"/><attribute name=\"activityid\"/><attribute name=\"description\"/><attribute name=\"modifiedby\"/><attribute name=\"ownerid\"/><attribute name=\"allparties\"/><attribute name=\"modifiedon\"/><attribute name=\"scheduledstart\"/><attribute name=\"createdby\"/><attribute name=\"scheduledend\"/><order attribute=\"modifiedon\" descending=\"true\"/><order attribute=\"activityid\" descending=\"false\"/><link-entity name=\"email\" from=\"activityid\" to=\"activityid\" alias=\"email\" link-type=\"outer\"><attribute name=\"senton\" alias=\"EmailSentOn\"/><attribute name=\"delayedemailsendtime\" alias=\"DelayedemailSendTime\"/><attribute name=\"lastopenedtime\" alias=\"LastOpenedTime\"/><attribute name=\"isemailfollowed\" alias=\"IsEmailFollowed\"/><attribute name=\"baseconversationindexhash\" alias=\"BaseConversationIndexHash\"/></link-entity><link-entity name=\"opportunityclose\" from=\"activityid\" to=\"activityid\" alias=\"opportunityclose\" link-type=\"outer\"><attribute name=\"actualrevenue\" alias=\"opportunityclose_actualrevenue\"/></link-entity><link-entity name=\"phonecall\" from=\"activityid\" to=\"activityid\" alias=\"phonecall\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"phonecall_directioncode\"/></link-entity><link-entity name=\"letter\" from=\"activityid\" to=\"activityid\" alias=\"letter\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"letter_directioncode\"/></link-entity><link-entity name=\"fax\" from=\"activityid\" to=\"activityid\" alias=\"fax\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"fax_directioncode\"/></link-entity><filter type=\"and\"><condition attribute=\"activitytypecode\" operator=\"in\"><value>4402</value><value>4206</value><value>4202</value><value>4204</value><value>4207</value><value>4201</value><value>4208</value><value>4209</value><value>4210</value><value>4211</value><value>4251</value><value>4216</value><value>4212</value><value>10315</value><value>10325</value><value>10327</value></condition></filter></entity></fetch>\'");
            request31.QueryStringParameters.Add("@rollupType", "0");
            yield return request31;
            request31 = null;
            this.AddCommentToResult("32_Click Qualify Button");

            CrmRequest request32 = new CrmRequest(thisURL+"/api/data/v9.0/annotations");
            request32.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request32.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=RetrieveMultipleOther; Source=Default"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "43211eab-91f6-4ddd-becc-8ea1d7513c7b"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request32.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request32.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "undefined"));
            request32.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request32.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request32.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request32.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request32.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request32.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""true"" returntotalrecordcount=""false"" page=""1"" count=""10"" no-lock=""false""><entity name=""annotation""><attribute name=""annotationid""/><attribute name=""subject""/><attribute name=""notetext""/><attribute name=""filename""/><attribute name=""filesize""/><attribute name=""isdocument""/><attribute name=""createdby""/><attribute name=""createdon""/><attribute name=""modifiedby""/><attribute name=""modifiedon""/><attribute name=""mimetype""/><order attribute=""modifiedon"" descending=""true""/><order attribute=""annotationid"" descending=""false""/><link-entity name=""systemuser"" from=""systemuserid"" to=""modifiedby"" alias=""systemuser"" link-type=""outer""><attribute name=""entityimage_url""/><attribute name=""systemuserid""/><attribute name=""fullname""/></link-entity><filter type=""and""><filter type=""and""><condition attribute=""objectid"" operator=""eq"" value="""+opportunityId+@"""/></filter></filter></entity></fetch>");
            yield return request32;
            request32 = null;
            this.AddCommentToResult("33_Click Qualify Button");

            CrmRequest request33 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request33.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request33.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "b2a54d05-4e8d-457e-b942-75153c94952e"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request33.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request33.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request33.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request33.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request33.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request33.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request33.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request33.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request33;
            request33 = null;
            this.AddCommentToResult("34_Click Qualify Button");

            CrmRequest request34 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request34.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request34.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "a0a6a0e1-ea76-4edf-9f1e-bbf5c2b0cd72"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request34.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request34.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "fe4bc089-8901-466c-a41b-1c1090f204d4"));
            request34.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request34.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request34.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request34.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request34.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request34.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><link-entity name=""systemuser"" from=""systemuserid"" to=""record2id"" alias=""systemuser""><attribute name=""internalemailaddress""/></link-entity><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""8""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1001""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request34;
            request34 = null;
            this.AddCommentToResult("35_Click Qualify Button");

            CrmRequest request35 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request35.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request35.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "3e305783-2ad0-4fdc-82f8-d105b21a5215"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request35.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request35.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request35.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request35.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request35.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request35.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request35.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request35.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""opportunitycompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""opportunity"" from=""opportunityid"" to=""opportunityid"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request35;
            request35 = null;
            this.AddCommentToResult("36_Click Qualify Button");

            CrmRequest request36 = new CrmRequest(thisURL+"/api/data/v9.0/RetrieveTenantInfo");
            request36.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request36.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request36.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "675b2fe3-489d-4a68-b59d-90dc6a91b2a8"));
            request36.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request36.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request36.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request36.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request36;
            request36 = null;
            this.AddCommentToResult("37_Click Qualify Button");

            CrmRequest request37 = new CrmRequest(thisURL+"/api/data/v9.0/roles");
            request37.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request37.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request37.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "549e2076-7a49-433b-9900-e97a8849ff15"));
            request37.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request37.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request37.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request37.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request37.QueryStringParameters.Add("$filter", "_roletemplateid_value eq \'627090ff-40a3-4053-8790-584edc5be201\'");
            request37.QueryStringParameters.Add("$select", "roleid");
            yield return request37;
            request37 = null;
            this.AddCommentToResult("38_Click Qualify Button");

            CrmRequest request38 = new CrmRequest(thisURL+"/api/data/v9.0/systemusers("+systemuserId+")");
            request38.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request38.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request38.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "c0a6ffd2-1d01-4b07-9845-146a91c3c460"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request38.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request38.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request38.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request38.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request38.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request38.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request38.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request38.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"2150652\""));
            yield return request38;
            request38 = null;
            this.AddCommentToResult("39_Click Qualify Button");

            CrmRequest request39 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request39.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request39.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "71783443-dbab-4d0d-9850-f315cac8bc8f"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request39.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request39.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request39.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request39.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request39.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request39.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request39.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request39.QueryStringParameters.Add("fetchXml", WebUtility.UrlEncode("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><filter type=\"and\"><condition attribute=\"startdate\" operator=\"le\" value=\"03/29/2022 15:03\"/></filter><filter type=\"and\"><condition attribute=\"expirydate\" operator=\"ge\" value=\"03/29/2022 15:03\"/></filter><filter type=\"and\"><condition attribute=\"recordid\" operator=\"eq\" value=\"" +opportunityId+"\"/></filter><filter type=\"and\"><condition attribute=\"recordidobjecttypecode2\" operator=\"eq\" value=\"3\"/></filter></filter></entity></fetch>"));
            yield return request39;
            request39 = null;
            this.AddCommentToResult("40_Click Qualify Button");

            CrmRequest request40 = new CrmRequest(thisURL+"/api/data/v9.0/contacts("+contactId+")");
            request40.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request40.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request40.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "d6210fc5-c676-41db-b9f8-47a3790cd95a"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request40.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request40.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request40.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request40.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request40.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request40.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request40.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request40.QueryStringParameters.Add("$select", "emailaddress1");
            yield return request40;
            request40 = null;
            this.AddCommentToResult("41_Click Qualify Button");

            CrmRequest request42 = new CrmRequest(thisURL+"/api/data/v9.0/contacts("+contactId+")");
            request42.ThinkTime = 1;
            request42.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request42.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request42.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "0fb94c32-53f3-4eff-ab26-ae29523e78c1"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request42.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request42.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request42.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request42.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request42.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request42.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request42.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request42.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"2188709\""));
            request42.QueryStringParameters.Add("$select", "emailaddress1");
            yield return request42;
            request42 = null;
            this.AddCommentToResult("43_Click Qualify Button");

            CrmRequest request43 = new CrmRequest(thisURL+"/api/data/v9.0/msdyn_postconfigs");
            request43.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request43.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request43.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request43.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request43.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request43.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request43.QueryStringParameters.Add("$select", "msdyn_entityname,statecode");
            request43.QueryStringParameters.Add("$filter", "msdyn_entityname eq \'opportunity\'");
            yield return request43;
            request43 = null;
            this.AddCommentToResult("44_Click Qualify Button");

            CrmRequest request44 = new CrmRequest(thisURL+"/api/data/v9.0/IsPdfEnabledForEntity");
            request44.Method = "POST";
            request44.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request44.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "3824e8f9-1896-4f01-885a-7993fe3e61d1"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request44.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request44.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request44.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request44.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request44.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request44.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request44.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request44.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request44Body = new StringHttpBody();
            request44Body.ContentType = "application/json";
            request44Body.InsertByteOrderMark = false;
            request44Body.BodyString = "{\"PDFSettingsEntityName\":\"opportunity\"}";
            request44.Body = request44Body;
            yield return request44;
            request44 = null;
            this.AddCommentToResult("45_Click Qualify Button");

            CrmRequest request45 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request45.Method = "POST";
            request45.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request45.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "2bffe067-9218-4929-9c40-7c5dea3321a1"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request45.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request45.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request45.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request45.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request45.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request45.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request45.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request45.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request45Body = new StringHttpBody();
            request45Body.ContentType = "application/json";
            request45Body.InsertByteOrderMark = false;
            request45Body.BodyString = "{\"items\":[{\"LastAccessedStr\":\"2022-03-29T15:03:20.599Z\",\"EntityTypeName\":\"opportunity\",\"ObjectId\":\"{"+opportunityId+"}\",\"RecordType\":\"Entity\",\"Title\":\"ltlead959\",\"PinStatus\":null,\"IconPath\":null,\"IsDeleted\":false,\"IsUserView\":null}]}";
            request45.Body = request45Body;
            yield return request45;
            request45 = null;
            this.AddCommentToResult("46_Click Qualify Button");

            CrmRequest request46 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request46.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request46.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-metadatareductionlevel", "5"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "bb937a8a-5678-4e72-a7d4-194084a66523"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request46.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request46.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request46.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "cc9ab558-bbdc-42d9-90ed-dffb112ab7d1"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request46.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request46.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request46.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request46.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request46.QueryStringParameters.Add("@ClientMetadataQuery", "{\"MetadataType\":\"metadataversion\",\"GetDefault\":false,\"ChangedAfter\":\"2151247\",\"AppId\":\"a9a86d48-06ad-ec11-9841-000d3a3bc2af\"}");
            request46.QueryStringParameters.Add("api-version", "9.1");
            yield return request46;
            request46 = null;
            this.EndTransaction("1021.05_Click Qualify Button");

            //Insert Opportunity, Account, and Contact into ltopportunity table for future processing
            using (System.Data.SqlClient.SqlConnection emsqlCon = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
            {
                emsqlCon.Open();
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = emsqlCon;
                cmd.CommandText = "insert into ltopportunity values ('" + opportunityId + "','" + accountId + "','" + contactId + "','" + systemuserId + "','0','0','" + Guid.Empty.ToString() + "')";
                cmd.ExecuteNonQuery();


                //Update Lead showing thats its been converted and no longer being used. 
                cmd.CommandText = "Update ltlead set inuse = '0', state='1' where leadid = '"+leadId+"'";
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
