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
    public class zz_1022_UpdateOpportunity_CloseasWonCoded : WebTestBase
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
        private string opportunityId;
        private string contactId;

        public zz_1022_UpdateOpportunity_CloseasWonCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1022_UpdateOpportunity_CloseasWonCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1022_UpdateOpportunity_CloseasWonCoded_PostWebTest);
        }
        public void zz_1022_UpdateOpportunity_CloseasWonCoded_PreWebTest(object sender, PreWebTestEventArgs e)
        {
            //Select Existing Opportunity to be Closed
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
                    cmd.CommandText = "UPDATE ltopportunity set inuse='1', uid = '" + ukey + "' where opportunityid in (select top 1 opportunityid from ltopportunity where state = 0 and inuse = 0 order by newid())";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = string.Format("SELECT top 1 rtrim(opportunityid),rtrim(contactid),rtrim(systemuserid) from ltopportunity(nolock) where uid = '" + ukey + "'");

                    System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                    cmd.Parameters.Clear();

                    //Lock this guy and commit the transaction.
                    while (true == reader.Read())
                    {
                        //Read User
                        opportunityId = reader[0].ToString();
                        contactId = reader[1].ToString();
                        systemuserId = reader[2].ToString();

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
        public void zz_1022_UpdateOpportunity_CloseasWonCoded_PostWebTest(object sender, PostWebTestEventArgs e)
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
            this.BeginTransaction("1022.01_Navigate to My Open Opportunities View");
            this.AddCommentToResult("01_Navigate to My Open Opportunities View");

            CrmRequest request1 = new CrmRequest(thisURL+"/api/data/v9.0/opportunities");
            request1.ThinkTime = 1;
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request1.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Grid; Source=Default"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "18a7d928-7aba-46f3-bcf7-c8c561fe2a1a"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request1.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "00000000-0000-0000-00aa-000010003000"));
            request1.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "58e3defe-9d5e-4505-8e14-63949d64d5c4"));
            request1.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request1.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request1.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request1.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""50"" no-lock=""false""><entity name=""opportunity""><attribute name=""statecode""/><attribute name=""name""/><attribute name=""estimatedvalue""/><attribute name=""estimatedclosedate""/><order attribute=""estimatedclosedate"" descending=""false""/><filter type=""and""><condition attribute=""ownerid"" operator=""eq-userid""/><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><attribute name=""closeprobability""/><attribute name=""opportunityratingcode""/><link-entity alias=""opportunitycustomeridcontactcontactid"" name=""contact"" from=""contactid"" to=""customerid"" link-type=""outer"" visible=""false""><attribute name=""emailaddress1""/></link-entity><attribute name=""parentcontactid""/><attribute name=""parentaccountid""/><attribute name=""customerid""/><attribute name=""opportunityid""/></entity></fetch>");
            yield return request1;
            request1 = null;
            this.AddCommentToResult("02_Navigate to My Open Opportunities View");

            CrmRequest request2 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request2.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-metadatareductionlevel", "5"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "cd4a3d18-80e3-4877-aeb5-7c32b6dbc5a8"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request2.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request2.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "4c97f4f1-254d-49af-b731-d87f1320c36e"));
            request2.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request2.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request2.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request2.QueryStringParameters.Add("@ClientMetadataQuery", "{\"MetadataType\":\"metadataversion\",\"GetDefault\":false,\"ChangedAfter\":\"2151247\",\"AppId\":\"a9a86d48-06ad-ec11-9841-000d3a3bc2af\"}");
            request2.QueryStringParameters.Add("api-version", "9.1");
            yield return request2;
            request2 = null;
            this.AddCommentToResult("03_Navigate to My Open Opportunities View");

            CrmRequest request3 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request3.ThinkTime = 20;
            request3.Method = "POST";
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request3.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "eed55c81-8aad-40ff-889c-d29562b16c89"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request3.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request3.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "4c97f4f1-254d-49af-b731-d87f1320c36e"));
            request3.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request3.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request3.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request3.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request3Body = new StringHttpBody();
            request3Body.ContentType = "application/json";
            request3Body.InsertByteOrderMark = false;
            request3Body.BodyString = @"{""items"":[{""LastAccessedStr"":""2022-03-29T15:04:05.343Z"",""EntityTypeName"":""opportunity"",""ObjectId"":""{00000000-0000-0000-00AA-000010003000}"",""RecordType"":""Grid"",""Title"":""My Open Opportunities"",""PinStatus"":null,""IconPath"":null,""IsDeleted"":false,""IsUserView"":false}]}";
            request3.Body = request3Body;
            yield return request3;
            request3 = null;
            this.EndTransaction("1022.01_Navigate to My Open Opportunities View");
            this.BeginTransaction("1022.02_Quick Find Seach for Opportunity");
            this.AddCommentToResult("04_Quick Find Seach for Opportunity");

            CrmRequest request4 = new CrmRequest(thisURL+"/api/data/v9.0/opportunities");
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request4.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Quickfind; Source=Default"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "d46f930e-de1e-4f22-af62-c277b02516fe"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request4.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "00000000-0000-0000-00aa-000010003000"));
            request4.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "15e9230c-334d-4c25-b3a5-6cbb3fae3240"));
            request4.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request4.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request4.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request4.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""50"" no-lock=""false""><entity name=""opportunity""><attribute name=""statecode""/><attribute name=""name""/><attribute name=""estimatedvalue""/><attribute name=""estimatedclosedate""/><order attribute=""estimatedclosedate"" descending=""false""/><filter type=""and""><condition attribute=""ownerid"" operator=""eq-userid""/><condition attribute=""statecode"" operator=""eq"" value=""0""/></filter><attribute name=""closeprobability""/><attribute name=""opportunityratingcode""/><link-entity alias=""opportunitycustomeridcontactcontactid"" name=""contact"" from=""contactid"" to=""customerid"" link-type=""outer"" visible=""false""><attribute name=""emailaddress1""/></link-entity><attribute name=""parentcontactid""/><attribute name=""parentaccountid""/><attribute name=""customerid""/><attribute name=""opportunityid""/><filter type=""or"" isquickfindfields=""1""><condition attribute=""name"" operator=""like"" value=""ltlead%""/><condition attribute=""parentaccountidname"" operator=""like"" value=""ltlead%""/><condition attribute=""parentcontactidname"" operator=""like"" value=""ltlead%""/></filter></entity></fetch>");
            yield return request4;
            request4 = null;
            this.AddCommentToResult("05_Quick Find Seach for Opportunity");

            CrmRequest request5 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request5.ThinkTime = 20;
            request5.Method = "POST";
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request5.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "23c3a4f5-8099-433a-bbe8-c85d0a817367"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request5.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request5.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "b0d60098-8751-42ca-85e5-1bfa922d1501"));
            request5.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request5.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request5.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request5.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request5Body = new StringHttpBody();
            request5Body.ContentType = "application/json";
            request5Body.InsertByteOrderMark = false;
            request5Body.BodyString = @"{""items"":[{""LastAccessedStr"":""2022-03-29T15:05:19.028Z"",""EntityTypeName"":""opportunity"",""ObjectId"":""{00000000-0000-0000-00AA-000010003000}"",""RecordType"":""Grid"",""Title"":""My Open Opportunities"",""PinStatus"":null,""IconPath"":null,""IsDeleted"":false,""IsUserView"":null}]}";
            request5.Body = request5Body;
            yield return request5;
            request5 = null;
            this.EndTransaction("1022.02_Quick Find Seach for Opportunity");
            this.BeginTransaction("1022.03_Open Opportunity");
            this.AddCommentToResult("06_Open Opportunity");

            CrmRequest request6 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request6.Method = "POST";
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request6.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "552d8a4b-07b6-4db6-8660-72f763aa898a"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request6.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request6.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566355736"));
            request6.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request6.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request6.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request6Body = new StringHttpBody();
            request6Body.ContentType = "multipart/mixed;boundary=batch_1648566355736";
            request6Body.InsertByteOrderMark = false;
            request6Body.BodyString = "--batch_1648566355736\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities("+opportunityId+")?$select=name,_parentcontactid_value,_parentaccountid_value,purchasetimeframe,_transactioncurrencyid_value,budgetamount,purchaseprocess,msdyn_forecastcategory,description,currentsituation,customerneed,proposedsolution,_pricelevelid_value,isrevenuesystemcalculated,totallineitemamount,discountpercentage,discountamount,totalamountlessfreight,freightamount,totaltax,totalamount,estimatedclosedate,estimatedvalue,statuscode,_ownerid_value,statecode,opportunityid HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: d2a6ccf2-94bc-4f54-807e-9fc397fc8db9\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 9edd0dd3-e3f0-4307-89e7-9cc1b3489394\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566355736\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: eed7131a-e47b-4687-96b5-bd976c3de915\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 9edd0dd3-e3f0-4307-89e7-9cc1b3489394\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566355736\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 659ffc4d-2543-426a-856f-e473cb52e435\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 9edd0dd3-e3f0-4307-89e7-9cc1b3489394\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566355736--\r\n\0";
            request6.Body = request6Body;
            yield return request6;
            //businessprocessflowinstanceid":""+bpfId+"","
            string resresult = request6.lastResponse.BodyString;
            request6 = null;
            string bpfId = Utils.extractUCIgridresponse("businessprocessflowinstanceid\":\"", resresult);
            this.AddCommentToResult("07_Open Opportunity");

            CrmRequest request7 = new CrmRequest(thisURL+"/api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)");
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request7.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "095bee9b-a235-40b6-9955-3d07e4637d19"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request7.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request7.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=*"));
            request7.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request7.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request7.Headers.Add(new WebTestRequestHeader("content-type", "application/json; charset=utf-8"));
            request7.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request7.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request7.QueryStringParameters.Add("@id", "{\'@odata.id\':\'opportunities("+opportunityId+")\'}");
            request7.QueryStringParameters.Add("@xml", "\'<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\" returntotalrecordcount=\"false\" page=\"1\" count=\"10\" no-lock=\"true\"><entity name=\"activitypointer\"><attribute name=\"subject\"/><attribute name=\"activitytypecode\"/><attribute name=\"statecode\"/><attribute name=\"statuscode\"/><attribute name=\"activityid\"/><attribute name=\"description\"/><attribute name=\"modifiedby\"/><attribute name=\"ownerid\"/><attribute name=\"allparties\"/><attribute name=\"modifiedon\"/><attribute name=\"scheduledstart\"/><attribute name=\"createdby\"/><attribute name=\"scheduledend\"/><order attribute=\"modifiedon\" descending=\"true\"/><order attribute=\"activityid\" descending=\"false\"/><link-entity name=\"email\" from=\"activityid\" to=\"activityid\" alias=\"email\" link-type=\"outer\"><attribute name=\"senton\" alias=\"EmailSentOn\"/><attribute name=\"delayedemailsendtime\" alias=\"DelayedemailSendTime\"/><attribute name=\"lastopenedtime\" alias=\"LastOpenedTime\"/><attribute name=\"isemailfollowed\" alias=\"IsEmailFollowed\"/><attribute name=\"baseconversationindexhash\" alias=\"BaseConversationIndexHash\"/></link-entity><link-entity name=\"opportunityclose\" from=\"activityid\" to=\"activityid\" alias=\"opportunityclose\" link-type=\"outer\"><attribute name=\"actualrevenue\" alias=\"opportunityclose_actualrevenue\"/></link-entity><link-entity name=\"phonecall\" from=\"activityid\" to=\"activityid\" alias=\"phonecall\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"phonecall_directioncode\"/></link-entity><link-entity name=\"letter\" from=\"activityid\" to=\"activityid\" alias=\"letter\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"letter_directioncode\"/></link-entity><link-entity name=\"fax\" from=\"activityid\" to=\"activityid\" alias=\"fax\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"fax_directioncode\"/></link-entity><filter type=\"and\"><condition attribute=\"activitytypecode\" operator=\"in\"><value>4402</value><value>4206</value><value>4202</value><value>4204</value><value>4207</value><value>4201</value><value>4208</value><value>4209</value><value>4210</value><value>4211</value><value>4251</value><value>4216</value><value>4212</value><value>10315</value><value>10325</value><value>10327</value></condition></filter></entity></fetch>\'");
            request7.QueryStringParameters.Add("@rollupType", "0");
            yield return request7;
            request7 = null;
            this.AddCommentToResult("08_Open Opportunity");

            CrmRequest request8 = new CrmRequest(thisURL+"/api/data/v9.0/annotations");
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request8.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=RetrieveMultipleOther; Source=Default"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "91e80542-d1af-4b5a-a76e-4a2b57729b97"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request8.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "undefined"));
            request8.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request8.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request8.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request8.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request8.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""true"" returntotalrecordcount=""false"" page=""1"" count=""10"" no-lock=""false""><entity name=""annotation""><attribute name=""annotationid""/><attribute name=""subject""/><attribute name=""notetext""/><attribute name=""filename""/><attribute name=""filesize""/><attribute name=""isdocument""/><attribute name=""createdby""/><attribute name=""createdon""/><attribute name=""modifiedby""/><attribute name=""modifiedon""/><attribute name=""mimetype""/><order attribute=""modifiedon"" descending=""true""/><order attribute=""annotationid"" descending=""false""/><link-entity name=""systemuser"" from=""systemuserid"" to=""modifiedby"" alias=""systemuser"" link-type=""outer""><attribute name=""entityimage_url""/><attribute name=""systemuserid""/><attribute name=""fullname""/></link-entity><filter type=""and""><filter type=""and""><condition attribute=""objectid"" operator=""eq"" value="""+opportunityId+@"""/></filter></filter></entity></fetch>");
            yield return request8;
            request8 = null;
            this.AddCommentToResult("09_Open Opportunity");

            CrmRequest request9 = new CrmRequest(thisURL+"/api/data/v9.0/contacts("+contactId+")");
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request9.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request9.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "ddeb24c4-b0d7-4db1-b177-5fe582cf92b5"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request9.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request9.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request9.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request9.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request9.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request9.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"2188709\""));
            request9.QueryStringParameters.Add("$select", "emailaddress1");
            yield return request9;
            request9 = null;
            this.AddCommentToResult("10_Open Opportunity");

            CrmRequest request10 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request10.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "1e6ca7b1-6d57-42e6-96c9-77b32a59a31d"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request10.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request10.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request10.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request10.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request10.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request10.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request10;
            request10 = null;
            this.AddCommentToResult("11_Open Opportunity");

            CrmRequest request11 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request11.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "675f94ea-152d-4bd5-b8ac-350bc1ef70b5"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request11.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "fe4bc089-8901-466c-a41b-1c1090f204d4"));
            request11.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request11.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request11.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request11.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request11.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><link-entity name=""systemuser"" from=""systemuserid"" to=""record2id"" alias=""systemuser""><attribute name=""internalemailaddress""/></link-entity><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""8""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1001""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request11;
            request11 = null;
            this.AddCommentToResult("12_Open Opportunity");

            CrmRequest request12 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request12.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "ea1b82ba-a11c-4abd-a4c4-a4c8746ef414"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request12.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request12.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request12.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request12.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request12.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request12.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""opportunitycompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""opportunity"" from=""opportunityid"" to=""opportunityid"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request12;
            request12 = null;
            this.AddCommentToResult("13_Open Opportunity");

            CrmRequest request13 = new CrmRequest(thisURL+"/api/data/v9.0/RetrieveTenantInfo");
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request13.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request13.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "8426cccf-0c50-4898-a003-a49e6065e11b"));
            request13.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request13.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            yield return request13;
            request13 = null;
            this.AddCommentToResult("14_Open Opportunity");

            CrmRequest request14 = new CrmRequest(thisURL+"/api/data/v9.0/roles");
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request14.Headers.Add(new WebTestRequestHeader("X-Requested-With", "XMLHttpRequest"));
            request14.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "af902286-c9dc-46ab-981e-3491feb4dbd9"));
            request14.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request14.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request14.QueryStringParameters.Add("$filter", "_roletemplateid_value eq \'627090ff-40a3-4053-8790-584edc5be201\'");
            request14.QueryStringParameters.Add("$select", "roleid");
            yield return request14;
            request14 = null;
            this.AddCommentToResult("15_Open Opportunity");

            CrmRequest request15 = new CrmRequest(thisURL+"/api/data/v9.0/contacts("+contactId+")");
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request15.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request15.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "7b1ab8dc-4d5a-4a6f-ad06-ff7c6b3eae88"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request15.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request15.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request15.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request15.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request15.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request15.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"2188709\""));
            request15.QueryStringParameters.Add("$select", "emailaddress1");
            yield return request15;
            request15 = null;
            this.AddCommentToResult("16_Open Opportunity");

            CrmRequest request16 = new CrmRequest(thisURL+"/api/data/v9.0/systemusers("+systemuserId+")");
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request16.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request16.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "7ba80037-f689-4522-97b5-f273e40ea826"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request16.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request16.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request16.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request16.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request16.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request16.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"2150652\""));
            yield return request16;
            request16 = null;
            this.AddCommentToResult("17_Open Opportunity");

            CrmRequest request17 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request17.ThinkTime = 2;
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request17.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "8a386bff-4a3c-4f6f-8e7b-514be0ca5f92"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request17.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request17.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "9edd0dd3-e3f0-4307-89e7-9cc1b3489394"));
            request17.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request17.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request17.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request17.QueryStringParameters.Add("fetchXml", WebUtility.UrlEncode("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><filter type=\"and\"><condition attribute=\"startdate\" operator=\"le\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"expirydate\" operator=\"ge\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"recordid\" operator=\"eq\" value=\"" +opportunityId+"\"/></filter><filter type=\"and\"><condition attribute=\"recordidobjecttypecode2\" operator=\"eq\" value=\"3\"/></filter></filter></entity></fetch>"));
            yield return request17;
            request17 = null;
            this.AddCommentToResult("18_Open Opportunity");

            CrmRequest request18 = new CrmRequest(thisURL+"/api/data/v9.0/UpdateRecentItems");
            request18.Method = "POST";
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request18.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "eb8e3025-cd2a-440e-9b6b-45de039fb651"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request18.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request18.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "337b81c7-7fff-4068-9fb9-cf6d27096999"));
            request18.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request18.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request18.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request18.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request18Body = new StringHttpBody();
            request18Body.ContentType = "application/json";
            request18Body.InsertByteOrderMark = false;
            request18Body.BodyString = "{\"items\":[{\"LastAccessedStr\":\"2022-03-29T15:05:56.731Z\",\"EntityTypeName\":\"opportunity\",\"ObjectId\":\"{"+opportunityId+"}\",\"RecordType\":\"Entity\",\"Title\":\"ltlead959\",\"PinStatus\":null,\"IconPath\":null,\"IsDeleted\":false,\"IsUserView\":null}]}";
            request18.Body = request18Body;
            yield return request18;
            request18 = null;
            this.AddCommentToResult("19_Open Opportunity");

            CrmRequest request19 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request19.ThinkTime = 20;
            request19.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request19.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-metadatareductionlevel", "5"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "3491e815-c526-492b-b3c7-32f01bfef132"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request19.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request19.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request19.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "337b81c7-7fff-4068-9fb9-cf6d27096999"));
            request19.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request19.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request19.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request19.QueryStringParameters.Add("@ClientMetadataQuery", "{\"MetadataType\":\"metadataversion\",\"GetDefault\":false,\"ChangedAfter\":\"2151247\",\"AppId\":\"a9a86d48-06ad-ec11-9841-000d3a3bc2af\"}");
            request19.QueryStringParameters.Add("api-version", "9.1");
            yield return request19;
            request19 = null;
            this.EndTransaction("1022.03_Open Opportunity");
            this.BeginTransaction("1022.04_Enter Develop Stage Data and Click Next Stage");
            this.AddCommentToResult("20_Enter Develop Stage Data and Click Next Stage");

            CrmRequest request20 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request20.Method = "POST";
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request20.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "592023d4-26c9-4d7b-9697-d355a5cec4c8"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "87fbc98d-bb2d-4655-ba87-b163375b7a86"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request20.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request20.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566382904"));
            request20.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request20.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request20.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request20Body = new StringHttpBody();
            request20Body.ContentType = "multipart/mixed;boundary=batch_1648566382904";
            request20Body.InsertByteOrderMark = false;
            request20Body.BodyString = "--batch_1648566382904\nContent-Type: multipart/mixed;boundary=changeset_1648566382904\n\n--changeset_1648566382904\nContent-Type: application/http\nContent-Transfer-Encoding: binary\nContent-ID: 1\n\nPATCH /api/data/v9.0/opportunities("+opportunityId+") HTTP/1.1\nMSCRM.SuppressDuplicateDetection: false\nIf-match: *\nAutoDisassociate: true\nContent-Type: application/json\nPrefer: odata.include-annotations=\"*\"\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: b8fde498-0dd4-4ef6-9642-35661bdec402\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 592023d4-26c9-4d7b-9697-d355a5cec4c8\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n{\"identifycompetitors\":true,\"identifycustomercontacts\":true,\"proposedsolution\":\"test\",\"customerneed\":\"test\"}\n--changeset_1648566382904\nContent-Type: application/http\nContent-Transfer-Encoding: binary\nContent-ID: 2\n\nPATCH /api/data/v9.0/leadtoopportunitysalesprocesses("+bpfId+") HTTP/1.1\nMSCRM.SuppressDuplicateDetection: false\nIf-match: *\nAutoDisassociate: true\nContent-Type: application/json\nPrefer: odata.include-annotations=\"*\"\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: c1ae01cf-8777-43a6-8faf-9bbfb24f26d5\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 592023d4-26c9-4d7b-9697-d355a5cec4c8\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n{\"traversedpath\":\"f99b4d48-7aad-456e-864a-8e7d543f7495,bfc9108c-8389-406b-9166-2c3298a2e41f,3a275c22-fc45-4e89-97fc-41e5ec578743\",\"activestageid@odata.bind\":\"/processstages(3a275c22-fc45-4e89-97fc-41e5ec578743)\"}\n--changeset_1648566382904--\n\n--batch_1648566382904--\r\n\0";
            request20.Body = request20Body;
            yield return request20;
            request20 = null;
            this.AddCommentToResult("21_Enter Develop Stage Data and Click Next Stage");

            CrmRequest request21 = new CrmRequest(thisURL+"/api/data/v9.0/leadtoopportunitysalesprocesses("+bpfId+")");
            request21.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request21.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request21.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "16492608-03b1-4a95-b66b-df313abdd387"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request21.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request21.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request21.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "592023d4-26c9-4d7b-9697-d355a5cec4c8"));
            request21.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request21.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request21.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request21.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request21.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request21.QueryStringParameters.Add("$select", "traversedpath,_activestageid_value,activestagestartedon,completedon,_leadid_value,_opportunityid_value");
            yield return request21;
            request21 = null;
            this.AddCommentToResult("22_Enter Develop Stage Data and Click Next Stage");

            CrmRequest request22 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request22.Method = "POST";
            request22.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request22.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request22.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "592023d4-26c9-4d7b-9697-d355a5cec4c8"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "270495ad-6f68-4e94-b49b-ce06594377ff"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request22.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request22.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566383413"));
            request22.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request22.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request22.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request22.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request22.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request22Body = new StringHttpBody();
            request22Body.ContentType = "multipart/mixed;boundary=batch_1648566383413";
            request22Body.InsertByteOrderMark = false;
            request22Body.BodyString = "--batch_1648566383413\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities("+opportunityId+")?$select=name,_parentcontactid_value,_parentaccountid_value,purchasetimeframe,_transactioncurrencyid_value,budgetamount,purchaseprocess,msdyn_forecastcategory,description,currentsituation,customerneed,proposedsolution,_pricelevelid_value,isrevenuesystemcalculated,totallineitemamount,discountpercentage,discountamount,totalamountlessfreight,freightamount,totaltax,totalamount,estimatedclosedate,estimatedvalue,statuscode,_ownerid_value,statecode,statecode,statecode,opportunityid,_transactioncurrencyid_value,_transactioncurrencyid_value,identifycustomercontacts,identifycompetitors,identifypursuitteam,developproposal,completeinternalreview,presentproposal,completefinalproposal,presentfinalproposal,finaldecisiondate,sendthankyounote,filedebrief HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 8a86fc48-faec-4f69-b398-e13d75b348b4\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 592023d4-26c9-4d7b-9697-d355a5cec4c8\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566383413\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: d13c0b21-d3df-4f59-9245-2f8ba8829afb\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 592023d4-26c9-4d7b-9697-d355a5cec4c8\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566383413\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 3d58dd59-44c4-4fec-a2c6-4d2ef1d3f9a8\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 592023d4-26c9-4d7b-9697-d355a5cec4c8\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566383413--\r\n\0";
            request22.Body = request22Body;
            yield return request22;
            request22 = null;
            this.AddCommentToResult("23_Enter Develop Stage Data and Click Next Stage");

            CrmRequest request23 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request23.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request23.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "ddb796ed-2348-4dd1-9f78-66bcb429df0a"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request23.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request23.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request23.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "592023d4-26c9-4d7b-9697-d355a5cec4c8"));
            request23.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request23.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request23.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request23.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request23;
            request23 = null;
            this.AddCommentToResult("24_Enter Develop Stage Data and Click Next Stage");

            CrmRequest request24 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request24.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request24.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "9b03ce18-de46-4a00-bcb6-561ab151d177"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request24.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request24.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "fe4bc089-8901-466c-a41b-1c1090f204d4"));
            request24.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "592023d4-26c9-4d7b-9697-d355a5cec4c8"));
            request24.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request24.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request24.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request24.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request24.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request24.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><link-entity name=""systemuser"" from=""systemuserid"" to=""record2id"" alias=""systemuser""><attribute name=""internalemailaddress""/></link-entity><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""8""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1001""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request24;
            request24 = null;
            this.AddCommentToResult("25_Enter Develop Stage Data and Click Next Stage");

            CrmRequest request25 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request25.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request25.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "62d9e8b0-b2e9-4ac1-967a-cca19383cdb9"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request25.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request25.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request25.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "592023d4-26c9-4d7b-9697-d355a5cec4c8"));
            request25.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request25.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request25.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request25.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request25.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request25.QueryStringParameters.Add("fetchXml", WebUtility.UrlEncode("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><filter type=\"and\"><condition attribute=\"startdate\" operator=\"le\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"expirydate\" operator=\"ge\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"recordid\" operator=\"eq\" value=\"" +opportunityId+"\"/></filter><filter type=\"and\"><condition attribute=\"recordidobjecttypecode2\" operator=\"eq\" value=\"3\"/></filter></filter></entity></fetch>"));
            yield return request25;
            request25 = null;
            this.AddCommentToResult("26_Enter Develop Stage Data and Click Next Stage");

            CrmRequest request26 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request26.ThinkTime = 20;
            request26.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request26.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "b1c19e5d-aa63-4820-a83c-faaf8392cb6d"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request26.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request26.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request26.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "592023d4-26c9-4d7b-9697-d355a5cec4c8"));
            request26.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request26.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request26.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request26.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request26.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request26.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""opportunitycompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""opportunity"" from=""opportunityid"" to=""opportunityid"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request26;
            request26 = null;
            this.EndTransaction("1022.04_Enter Develop Stage Data and Click Next Stage");
            this.BeginTransaction("1022.05_Enter Propose Stage Data and Click Next Stage");
            this.AddCommentToResult("27_Enter Propose Stage Data and Click Next Stage");

            CrmRequest request27 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request27.Method = "POST";
            request27.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request27.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request27.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "dcc8344b-865c-4964-900f-5cfbf451a5cd"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "3633a912-a234-47e4-a7aa-77ff97912fef"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request27.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request27.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566438245"));
            request27.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request27.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request27.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request27.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request27.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request27Body = new StringHttpBody();
            request27Body.ContentType = "multipart/mixed;boundary=batch_1648566438245";
            request27Body.InsertByteOrderMark = false;
            request27Body.BodyString = "--batch_1648566438245\nContent-Type: multipart/mixed;boundary=changeset_1648566438245\n\n--changeset_1648566438245\nContent-Type: application/http\nContent-Transfer-Encoding: binary\nContent-ID: 1\n\nPATCH /api/data/v9.0/opportunities("+opportunityId+") HTTP/1.1\nMSCRM.SuppressDuplicateDetection: false\nIf-match: *\nAutoDisassociate: true\nContent-Type: application/json\nPrefer: odata.include-annotations=\"*\"\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 5fb93e51-b220-4824-a909-af695e91cbbe\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: dcc8344b-865c-4964-900f-5cfbf451a5cd\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n{\"presentproposal\":true,\"completeinternalreview\":true,\"developproposal\":true,\"identifypursuitteam\":true}\n--changeset_1648566438245\nContent-Type: application/http\nContent-Transfer-Encoding: binary\nContent-ID: 2\n\nPATCH /api/data/v9.0/leadtoopportunitysalesprocesses("+bpfId+") HTTP/1.1\nMSCRM.SuppressDuplicateDetection: false\nIf-match: *\nAutoDisassociate: true\nContent-Type: application/json\nPrefer: odata.include-annotations=\"*\"\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: f01f37e5-7250-478b-a48b-e79413bf36b3\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: dcc8344b-865c-4964-900f-5cfbf451a5cd\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n{\"traversedpath\":\"f99b4d48-7aad-456e-864a-8e7d543f7495,bfc9108c-8389-406b-9166-2c3298a2e41f,3a275c22-fc45-4e89-97fc-41e5ec578743,7f5247fe-cfc3-42bc-aa77-b1d836d9b7c0\",\"activestageid@odata.bind\":\"/processstages(7f5247fe-cfc3-42bc-aa77-b1d836d9b7c0)\"}\n--changeset_1648566438245--\n\n--batch_1648566438245--\r\n\0";
            request27.Body = request27Body;
            yield return request27;
            request27 = null;
            this.AddCommentToResult("28_Enter Propose Stage Data and Click Next Stage");

            CrmRequest request28 = new CrmRequest(thisURL+"/api/data/v9.0/leadtoopportunitysalesprocesses("+bpfId+")");
            request28.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request28.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request28.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "7f7999ef-abaf-417b-ac04-ae0ffc0d880e"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request28.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request28.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request28.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "dcc8344b-865c-4964-900f-5cfbf451a5cd"));
            request28.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request28.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request28.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request28.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request28.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request28.Headers.Add(new WebTestRequestHeader("If-None-Match", "W/\"2188756\""));
            request28.QueryStringParameters.Add("$select", "traversedpath,_activestageid_value,activestagestartedon,completedon,_leadid_value,_opportunityid_value");
            yield return request28;
            request28 = null;
            this.AddCommentToResult("29_Enter Propose Stage Data and Click Next Stage");

            CrmRequest request29 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request29.ThinkTime = 1;
            request29.Method = "POST";
            request29.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request29.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request29.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "dcc8344b-865c-4964-900f-5cfbf451a5cd"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "cf65b31e-1936-4e6d-99eb-1c6a3e17d1e9"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request29.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request29.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566438779"));
            request29.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request29.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request29.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request29.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request29.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request29Body = new StringHttpBody();
            request29Body.ContentType = "multipart/mixed;boundary=batch_1648566438779";
            request29Body.InsertByteOrderMark = false;
            request29Body.BodyString = "--batch_1648566438779\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities("+opportunityId+")?$select=name,_parentcontactid_value,_parentaccountid_value,purchasetimeframe,_transactioncurrencyid_value,budgetamount,purchaseprocess,msdyn_forecastcategory,description,currentsituation,customerneed,proposedsolution,_pricelevelid_value,isrevenuesystemcalculated,totallineitemamount,discountpercentage,discountamount,totalamountlessfreight,freightamount,totaltax,totalamount,estimatedclosedate,estimatedvalue,statuscode,_ownerid_value,statecode,statecode,statecode,opportunityid,_transactioncurrencyid_value,_transactioncurrencyid_value,identifycustomercontacts,identifycompetitors,identifypursuitteam,developproposal,completeinternalreview,presentproposal,completefinalproposal,presentfinalproposal,finaldecisiondate,sendthankyounote,filedebrief HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 1530764f-de05-4735-9459-11b7478b52a6\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: dcc8344b-865c-4964-900f-5cfbf451a5cd\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566438779\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 704f1c89-0244-4f16-840c-7bbf2e6a8759\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: dcc8344b-865c-4964-900f-5cfbf451a5cd\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566438779\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: f0c090f8-09f0-47cd-806c-771da4ae4e1d\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: dcc8344b-865c-4964-900f-5cfbf451a5cd\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566438779--\r\n\0";
            request29.Body = request29Body;
            yield return request29;
            request29 = null;
            this.AddCommentToResult("30_Enter Propose Stage Data and Click Next Stage");

            CrmRequest request30 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request30.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request30.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "b3fbee68-71b9-43f4-8c42-bca7c145c13c"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request30.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request30.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request30.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "dcc8344b-865c-4964-900f-5cfbf451a5cd"));
            request30.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request30.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request30.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request30.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request30.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request30.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request30;
            request30 = null;
            this.AddCommentToResult("31_Enter Propose Stage Data and Click Next Stage");

            CrmRequest request31 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request31.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request31.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "f0bec5e7-9c78-4e36-afef-c24bcd4d671a"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request31.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request31.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "fe4bc089-8901-466c-a41b-1c1090f204d4"));
            request31.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "dcc8344b-865c-4964-900f-5cfbf451a5cd"));
            request31.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request31.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request31.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request31.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request31.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request31.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><link-entity name=""systemuser"" from=""systemuserid"" to=""record2id"" alias=""systemuser""><attribute name=""internalemailaddress""/></link-entity><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""8""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1001""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request31;
            request31 = null;
            this.AddCommentToResult("32_Enter Propose Stage Data and Click Next Stage");

            CrmRequest request32 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request32.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request32.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "c8d4cca2-28e4-4aee-ac3b-17143507b533"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request32.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request32.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request32.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "dcc8344b-865c-4964-900f-5cfbf451a5cd"));
            request32.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request32.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request32.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request32.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request32.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request32.QueryStringParameters.Add("fetchXml", WebUtility.UrlEncode("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><filter type=\"and\"><condition attribute=\"startdate\" operator=\"le\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"expirydate\" operator=\"ge\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"recordid\" operator=\"eq\" value=\"" +opportunityId+"\"/></filter><filter type=\"and\"><condition attribute=\"recordidobjecttypecode2\" operator=\"eq\" value=\"3\"/></filter></filter></entity></fetch>"));
            yield return request32;
            request32 = null;
            this.AddCommentToResult("33_Enter Propose Stage Data and Click Next Stage");

            CrmRequest request33 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request33.ThinkTime = 20;
            request33.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request33.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "fe002109-d57c-414b-8d58-0c5d2fc773b7"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request33.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request33.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request33.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "dcc8344b-865c-4964-900f-5cfbf451a5cd"));
            request33.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request33.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request33.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request33.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request33.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request33.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""opportunitycompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""opportunity"" from=""opportunityid"" to=""opportunityid"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request33;
            request33 = null;
            this.EndTransaction("1022.05_Enter Propose Stage Data and Click Next Stage");
            this.BeginTransaction("1022.06_Enter Close Stage Details and Click Finish");
            this.AddCommentToResult("34_Enter Close Stage Details and Click Finish");

            CrmRequest request34 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request34.Method = "POST";
            request34.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request34.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request34.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "fc5e46a6-b562-462d-812b-45fc489759f3"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "66953c71-abca-4b63-9d9f-aba8ee550d55"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request34.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request34.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566465140"));
            request34.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request34.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request34.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request34.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request34.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request34Body = new StringHttpBody();
            request34Body.ContentType = "multipart/mixed;boundary=batch_1648566465140";
            request34Body.InsertByteOrderMark = false;
            request34Body.BodyString = "--batch_1648566465140\nContent-Type: multipart/mixed;boundary=changeset_1648566465141\n\n--changeset_1648566465141\nContent-Type: application/http\nContent-Transfer-Encoding: binary\nContent-ID: 1\n\nPATCH /api/data/v9.0/opportunities("+opportunityId+") HTTP/1.1\nMSCRM.SuppressDuplicateDetection: false\nIf-match: *\nAutoDisassociate: true\nContent-Type: application/json\nPrefer: odata.include-annotations=\"*\"\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: ee60594b-398c-4070-b6c9-19448180f25d\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: fc5e46a6-b562-462d-812b-45fc489759f3\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n{\"filedebrief\":true,\"sendthankyounote\":true,\"finaldecisiondate\":\"2022-03-29\",\"presentfinalproposal\":true,\"completefinalproposal\":true}\n--changeset_1648566465141\nContent-Type: application/http\nContent-Transfer-Encoding: binary\nContent-ID: 2\n\nPATCH /api/data/v9.0/leadtoopportunitysalesprocesses("+bpfId+") HTTP/1.1\nMSCRM.SuppressDuplicateDetection: false\nIf-match: *\nAutoDisassociate: true\nContent-Type: application/json\nPrefer: odata.include-annotations=\"*\"\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 199104ba-32e3-4320-a6af-c9b61fb8dc97\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: fc5e46a6-b562-462d-812b-45fc489759f3\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n{\"statecode\":1,\"statuscode\":2}\n--changeset_1648566465141--\n\n--batch_1648566465140--\r\n\0";
            request34.Body = request34Body;
            yield return request34;
            request34 = null;
            this.AddCommentToResult("35_Enter Close Stage Details and Click Finish");

            CrmRequest request35 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request35.ThinkTime = 12;
            request35.Method = "POST";
            request35.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request35.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request35.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "fc5e46a6-b562-462d-812b-45fc489759f3"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "186aed5a-96fe-4b80-aa50-c114759d92e4"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request35.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request35.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566465731"));
            request35.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request35.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request35.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request35.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request35.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request35Body = new StringHttpBody();
            request35Body.ContentType = "multipart/mixed;boundary=batch_1648566465731";
            request35Body.InsertByteOrderMark = false;
            request35Body.BodyString = "--batch_1648566465731\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities("+opportunityId+")?$select=name,_parentcontactid_value,_parentaccountid_value,purchasetimeframe,_transactioncurrencyid_value,budgetamount,purchaseprocess,msdyn_forecastcategory,description,currentsituation,customerneed,proposedsolution,_pricelevelid_value,isrevenuesystemcalculated,totallineitemamount,discountpercentage,discountamount,totalamountlessfreight,freightamount,totaltax,totalamount,estimatedclosedate,estimatedvalue,statuscode,_ownerid_value,statecode,statecode,statecode,opportunityid,_transactioncurrencyid_value,_transactioncurrencyid_value,identifycustomercontacts,identifycompetitors,identifypursuitteam,developproposal,completeinternalreview,presentproposal,completefinalproposal,presentfinalproposal,finaldecisiondate,sendthankyounote,filedebrief HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 2e0f0259-7c5a-4694-89d7-d0050300833e\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: fc5e46a6-b562-462d-812b-45fc489759f3\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566465731\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: b2bb39ab-44f5-4ba5-8f58-3b6037461bf2\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: fc5e46a6-b562-462d-812b-45fc489759f3\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566465731\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D&@ProcessId=%7B%22%40odata.id%22%3A%22businessprocessflowinstances(919e14d1-6489-4852-abd0-a63a6ecaac5d)%22%7D&@ProcessInstanceId=%7B%22%40odata.id%22%3A%22businessprocessflowinstances("+bpfId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 6de8fc34-3483-4717-a985-796447c487c1\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: fc5e46a6-b562-462d-812b-45fc489759f3\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566465731--\r\n\0";
            request35.Body = request35Body;
            yield return request35;
            request35 = null;
            this.EndTransaction("1022.06_Enter Close Stage Details and Click Finish");
            this.BeginTransaction("1022.07_Click Close as Won");
            this.AddCommentToResult("36_Click Close as Won");

            CrmRequest request36 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request36.Method = "POST";
            request36.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request36.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request36.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "81f00426-56d4-486a-82b5-914a879306b5"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "c29f08e6-4999-4823-8c48-62fb4fe3359e"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request36.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request36.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566478025"));
            request36.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request36.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request36.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request36.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request36.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request36Body = new StringHttpBody();
            request36Body.ContentType = "multipart/mixed;boundary=batch_1648566478025";
            request36Body.InsertByteOrderMark = false;
            request36Body.BodyString = "--batch_1648566478025\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities("+opportunityId+")?$select=name,_parentcontactid_value,_parentaccountid_value,purchasetimeframe,_transactioncurrencyid_value,budgetamount,purchaseprocess,msdyn_forecastcategory,description,currentsituation,customerneed,proposedsolution,_pricelevelid_value,isrevenuesystemcalculated,totallineitemamount,discountpercentage,discountamount,totalamountlessfreight,freightamount,totaltax,totalamount,estimatedclosedate,estimatedvalue,statuscode,_ownerid_value,statecode,identifycustomercontacts,identifycompetitors,identifypursuitteam,developproposal,completeinternalreview,presentproposal,completefinalproposal,presentfinalproposal,finaldecisiondate,sendthankyounote,filedebrief,statecode,opportunityid HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 5870b76c-3d7d-4d8e-8846-fc7ace04cd4d\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 81f00426-56d4-486a-82b5-914a879306b5\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566478025\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: abdb0df5-a7be-4950-b86a-0c20898fc54a\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 81f00426-56d4-486a-82b5-914a879306b5\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566478025\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 36b8437f-00cc-4fff-b733-ef509d196cca\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: 81f00426-56d4-486a-82b5-914a879306b5\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566478025--\r\n\0";
            request36.Body = request36Body;
            yield return request36;
            request36 = null;
            this.AddCommentToResult("37_Click Close as Won");

            CrmRequest request37 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request37.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request37.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "b6038fc3-5dda-455a-b963-59f07e98d357"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request37.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request37.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request37.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "81f00426-56d4-486a-82b5-914a879306b5"));
            request37.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request37.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request37.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request37.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request37.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request37.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request37;
            request37 = null;
            this.AddCommentToResult("38_Click Close as Won");

            CrmRequest request38 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request38.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request38.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "4cb83835-4a81-4113-bb7c-ba1b575d0d1d"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request38.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request38.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "fe4bc089-8901-466c-a41b-1c1090f204d4"));
            request38.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "81f00426-56d4-486a-82b5-914a879306b5"));
            request38.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request38.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request38.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request38.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request38.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request38.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><link-entity name=""systemuser"" from=""systemuserid"" to=""record2id"" alias=""systemuser""><attribute name=""internalemailaddress""/></link-entity><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""8""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1001""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request38;
            request38 = null;
            this.AddCommentToResult("39_Click Close as Won");

            CrmRequest request39 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request39.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request39.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "a403775e-5e2d-4dd7-aa8d-bbd1e175d0ca"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request39.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request39.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request39.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "81f00426-56d4-486a-82b5-914a879306b5"));
            request39.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request39.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request39.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request39.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request39.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request39.QueryStringParameters.Add("fetchXml", WebUtility.UrlEncode("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><filter type=\"and\"><condition attribute=\"startdate\" operator=\"le\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"expirydate\" operator=\"ge\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"recordid\" operator=\"eq\" value=\"" +opportunityId+"\"/></filter><filter type=\"and\"><condition attribute=\"recordidobjecttypecode2\" operator=\"eq\" value=\"3\"/></filter></filter></entity></fetch>"));
            yield return request39;
            request39 = null;
            this.AddCommentToResult("40_Click Close as Won");

            CrmRequest request40 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request40.ThinkTime = 2;
            request40.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request40.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "7de33a19-b21a-46bb-8323-ce92520d92c8"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request40.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request40.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request40.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "81f00426-56d4-486a-82b5-914a879306b5"));
            request40.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request40.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request40.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request40.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request40.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request40.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""opportunitycompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""opportunity"" from=""opportunityid"" to=""opportunityid"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request40;
            request40 = null;
            this.AddCommentToResult("41_Click Close as Won");

            CrmRequest request41 = new CrmRequest(thisURL+"/api/data/v9.0/opportunities("+opportunityId+")");
            request41.ThinkTime = 1;
            request41.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request41.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request41.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request41.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request41.Headers.Add(new WebTestRequestHeader("mscrm.returnnotifications", "true"));
            request41.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "4846bb64-0f18-4388-8ae6-3e8b5fa1266a"));
            request41.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request41.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request41.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request41.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request41.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request41.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request41.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "81f00426-56d4-486a-82b5-914a879306b5"));
            request41.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request41.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request41.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request41.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request41.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request41.QueryStringParameters.Add("$select", "statecode,estimatedvalue");
            request41.QueryStringParameters.Add("$expand", "transactioncurrencyid($select=currencyname,currencysymbol,currencyprecision)");
            yield return request41;
            request41 = null;
            this.AddCommentToResult("42_Click Close as Won");

            CrmRequest request42 = new CrmRequest(thisURL+"/api/data/v9.0/GetClientMetadata(ClientMetadataQuery=@ClientMetadataQuery)");
            request42.ThinkTime = 20;
            request42.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request42.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-metadatareductionlevel", "5"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "02436e32-789f-45ec-869e-1004fa9bfd73"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request42.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request42.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request42.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "ffc3ecf4-aa64-42b1-a6b8-8cb4c2a8dba6"));
            request42.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request42.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request42.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request42.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request42.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request42.QueryStringParameters.Add("@ClientMetadataQuery", "{\"MetadataType\":\"metadataversion\",\"GetDefault\":false,\"ChangedAfter\":\"2151247\",\"AppId\":\"a9a86d48-06ad-ec11-9841-000d3a3bc2af\"}");
            request42.QueryStringParameters.Add("api-version", "9.1");
            yield return request42;
            request42 = null;
            this.EndTransaction("1022.07_Click Close as Won");
            this.BeginTransaction("1022.08_Enter Opportunity Close Details and Click OK");
            this.AddCommentToResult("43_Enter Opportunity Close Details and Click OK");

            CrmRequest request43 = new CrmRequest(thisURL+"/api/data/v9.0/WinOpportunity");
            request43.Method = "POST";
            request43.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request43.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request43.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request43.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request43.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "70a9792c-1b63-447a-a8e7-db9cd847c50b"));
            request43.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request43.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request43.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request43.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request43.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request43.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request43.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "c68e07b3-76db-4834-abfc-c2202638ca56"));
            request43.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request43.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request43.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request43.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request43.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request43.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request43Body = new StringHttpBody();
            request43Body.ContentType = "application/json";
            request43Body.InsertByteOrderMark = false;
            request43Body.BodyString = "{\"OpportunityClose\":{\"opportunityid@odata.bind\":\"/opportunities("+opportunityId+")\",\"actualrevenue\":25000,\"actualend\":\"2022-03-29T16:08:37.487Z\"},\"Status\":3}";
            request43.Body = request43Body;
            yield return request43;
            request43 = null;
            this.AddCommentToResult("44_Enter Opportunity Close Details and Click OK");

            CrmRequest request44 = new CrmRequest(thisURL+"/api/data/v9.0/$batch");
            request44.ThinkTime = 1;
            request44.Method = "POST";
            request44.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request44.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request44.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "c68e07b3-76db-4834-abfc-c2202638ca56"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "459a0801-2b54-4bcb-9c98-3b8e7b93a7b7"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request44.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request44.Headers.Add(new WebTestRequestHeader("content-type", "multipart/mixed;boundary=batch_1648566519592"));
            request44.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request44.Headers.Add(new WebTestRequestHeader("Origin", thisURL+""));
            request44.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request44.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request44.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            StringHttpBody request44Body = new StringHttpBody();
            request44Body.ContentType = "multipart/mixed;boundary=batch_1648566519592";
            request44Body.InsertByteOrderMark = false;
            request44Body.BodyString = "--batch_1648566519592\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/opportunities("+opportunityId+")?$select=name,_parentcontactid_value,_parentaccountid_value,purchasetimeframe,_transactioncurrencyid_value,budgetamount,purchaseprocess,msdyn_forecastcategory,description,currentsituation,customerneed,proposedsolution,_pricelevelid_value,isrevenuesystemcalculated,totallineitemamount,discountpercentage,discountamount,totalamountlessfreight,freightamount,totaltax,totalamount,estimatedclosedate,estimatedvalue,statuscode,_ownerid_value,statecode,identifycustomercontacts,identifycompetitors,identifypursuitteam,developproposal,completeinternalreview,presentproposal,completefinalproposal,presentfinalproposal,finaldecisiondate,sendthankyounote,filedebrief,statecode,opportunityid HTTP/1.1\nPrefer: odata.include-annotations=\"*\"\nAccept: application/json\nMSCRM.ReturnNotifications: true\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 181b7c23-ab01-45df-a159-3798b4d3267f\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: c68e07b3-76db-4834-abfc-c2202638ca56\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566519592\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/systemusers("+systemuserId+")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@Target)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: 28468952-3b9a-410f-8f36-f4d94ad2146c\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: c68e07b3-76db-4834-abfc-c2202638ca56\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566519592\nContent-Type: application/http\nContent-Transfer-Encoding: binary\n\nGET /api/data/v9.0/RetrieveProcessControlData(Target=@Target,ProcessId=@ProcessId,ProcessInstanceId=@ProcessInstanceId)?@Target=%7B%22%40odata.id%22%3A%22opportunities("+opportunityId+")%22%7D&@ProcessId=null&@ProcessInstanceId=null HTTP/1.1\nAccept: application/json\nPrefer: odata.include-annotations=\"*\"\nContent-Type: application/json\nx-ms-app-id: a9a86d48-06ad-ec11-9841-000d3a3bc2af\nx-ms-app-name: msdynce_saleshub\nx-ms-client-request-id: b76c7323-41b9-4ad1-9f3a-a87547fb25f1\nx-ms-client-session-id: 542de7bc-c49e-450c-a94d-e0c56a5a1f0b\nx-ms-correlation-id: c68e07b3-76db-4834-abfc-c2202638ca56\nx-ms-sw-objectid: 40f400ec-b5d3-460a-8977-fad321eb2583\nx-ms-sw-tenantid: 31259ba9-4181-48d9-aea8-a8e53c06f73a\nx-ms-user-agent: PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)\nClientHost: Browser\n\n--batch_1648566519592--\r\n\0";
            request44.Body = request44Body;
            yield return request44;
            request44 = null;
            this.AddCommentToResult("45_Enter Opportunity Close Details and Click OK");

            CrmRequest request45 = new CrmRequest(thisURL+"/api/data/v9.0/activitypointers/Microsoft.Dynamics.CRM.RetrieveTimelineWallRecords(FetchXml=@xml,Target=@id,RollupType=@rollupType)");
            request45.ThinkTime = -1;
            request45.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request45.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "23a6e163-85df-4cf6-8d57-56e893f988a5"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request45.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request45.Headers.Add(new WebTestRequestHeader("odata-version", "4.0"));
            request45.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=*"));
            request45.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "c68e07b3-76db-4834-abfc-c2202638ca56"));
            request45.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request45.Headers.Add(new WebTestRequestHeader("content-type", "application/json; charset=utf-8"));
            request45.Headers.Add(new WebTestRequestHeader("odata-maxversion", "4.0"));
            request45.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request45.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request45.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request45.QueryStringParameters.Add("@id", "{\'@odata.id\':\'opportunities("+opportunityId+")\'}");
            request45.QueryStringParameters.Add("@xml", "\'<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"true\" returntotalrecordcount=\"false\" page=\"1\" count=\"10\" no-lock=\"true\"><entity name=\"activitypointer\"><attribute name=\"subject\"/><attribute name=\"activitytypecode\"/><attribute name=\"statecode\"/><attribute name=\"statuscode\"/><attribute name=\"activityid\"/><attribute name=\"description\"/><attribute name=\"modifiedby\"/><attribute name=\"ownerid\"/><attribute name=\"allparties\"/><attribute name=\"modifiedon\"/><attribute name=\"scheduledstart\"/><attribute name=\"createdby\"/><attribute name=\"scheduledend\"/><order attribute=\"modifiedon\" descending=\"true\"/><order attribute=\"activityid\" descending=\"false\"/><link-entity name=\"email\" from=\"activityid\" to=\"activityid\" alias=\"email\" link-type=\"outer\"><attribute name=\"senton\" alias=\"EmailSentOn\"/><attribute name=\"delayedemailsendtime\" alias=\"DelayedemailSendTime\"/><attribute name=\"lastopenedtime\" alias=\"LastOpenedTime\"/><attribute name=\"isemailfollowed\" alias=\"IsEmailFollowed\"/><attribute name=\"baseconversationindexhash\" alias=\"BaseConversationIndexHash\"/></link-entity><link-entity name=\"opportunityclose\" from=\"activityid\" to=\"activityid\" alias=\"opportunityclose\" link-type=\"outer\"><attribute name=\"actualrevenue\" alias=\"opportunityclose_actualrevenue\"/></link-entity><link-entity name=\"phonecall\" from=\"activityid\" to=\"activityid\" alias=\"phonecall\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"phonecall_directioncode\"/></link-entity><link-entity name=\"letter\" from=\"activityid\" to=\"activityid\" alias=\"letter\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"letter_directioncode\"/></link-entity><link-entity name=\"fax\" from=\"activityid\" to=\"activityid\" alias=\"fax\" link-type=\"outer\"><attribute name=\"directioncode\" alias=\"fax_directioncode\"/></link-entity><filter type=\"and\"><condition attribute=\"activitytypecode\" operator=\"in\"><value>4402</value><value>4206</value><value>4202</value><value>4204</value><value>4207</value><value>4201</value><value>4208</value><value>4209</value><value>4210</value><value>4211</value><value>4251</value><value>4216</value><value>4212</value><value>10315</value><value>10325</value><value>10327</value></condition></filter></entity></fetch>\'");
            request45.QueryStringParameters.Add("@rollupType", "0");
            yield return request45;
            request45 = null;
            this.AddCommentToResult("46_Enter Opportunity Close Details and Click OK");

            CrmRequest request46 = new CrmRequest(thisURL+"/api/data/v9.0/annotations");
            request46.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request46.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=RetrieveMultipleOther; Source=Default"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "459019cc-24c0-4bc8-9371-c1704f413d97"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request46.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request46.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "undefined"));
            request46.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "c68e07b3-76db-4834-abfc-c2202638ca56"));
            request46.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request46.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request46.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request46.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request46.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request46.QueryStringParameters.Add("fetchXml", @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""true"" returntotalrecordcount=""false"" page=""1"" count=""10"" no-lock=""false""><entity name=""annotation""><attribute name=""annotationid""/><attribute name=""subject""/><attribute name=""notetext""/><attribute name=""filename""/><attribute name=""filesize""/><attribute name=""isdocument""/><attribute name=""createdby""/><attribute name=""createdon""/><attribute name=""modifiedby""/><attribute name=""modifiedon""/><attribute name=""mimetype""/><order attribute=""modifiedon"" descending=""true""/><order attribute=""annotationid"" descending=""false""/><link-entity name=""systemuser"" from=""systemuserid"" to=""modifiedby"" alias=""systemuser"" link-type=""outer""><attribute name=""entityimage_url""/><attribute name=""systemuserid""/><attribute name=""fullname""/></link-entity><filter type=""and""><filter type=""and""><condition attribute=""objectid"" operator=""eq"" value="""+opportunityId+@"""/></filter></filter></entity></fetch>");
            yield return request46;
            request46 = null;
            this.AddCommentToResult("47_Enter Opportunity Close Details and Click OK");

            CrmRequest request47 = new CrmRequest(thisURL+"/api/data/v9.0/actioncards");
            request47.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request47.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "a789f7dd-467e-4379-89e3-d9143d816684"));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request47.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request47.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "92afd454-0f2e-4397-a1c8-05e37c6ad699"));
            request47.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "c68e07b3-76db-4834-abfc-c2202638ca56"));
            request47.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request47.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request47.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request47.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request47.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request47.QueryStringParameters.Add("fetchXml", WebUtility.UrlEncode("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" count=\"4\" returntotalrecordcount=\"true\" page=\"1\" no-lock=\"false\"><entity name=\"actioncard\"><attribute name=\"actioncardid\"/><attribute name=\"title\"/><attribute name=\"description\"/><attribute name=\"cardtype\"/><attribute name=\"cardtypeid\"/><attribute name=\"priority\"/><attribute name=\"regardingobjectid\"/><attribute name=\"recordidobjecttypecode2\"/><attribute name=\"data\"/><attribute name=\"recordid\"/><attribute name=\"startdate\"/><attribute name=\"expirydate\"/><attribute name=\"referencetokens\"/><attribute name=\"parentregardingobjectid\"/><order attribute=\"priority\" descending=\"true\"/><order attribute=\"expirydate\" descending=\"false\"/><link-entity name=\"cardtype\" from=\"cardtypeid\" to=\"cardtypeid\" link-type=\"inner\" alias=\"CardTypeLink\"/><filter type=\"and\"><filter type=\"or\"><condition attribute=\"expirydate\" operator=\"next-x-days\" value=\"90\"/><condition attribute=\"expirydate\" operator=\"today\"/></filter><filter type=\"or\"><condition attribute=\"source\" operator=\"eq\" value=\"1\"/><condition attribute=\"source\" operator=\"eq\" value=\"2\"/></filter><filter type=\"and\"><condition attribute=\"ownerid\" operator=\"eq-useroruserteams\"/></filter><condition attribute=\"state\" operator=\"eq\" value=\"0\"/><condition entityname=\"CardTypeLink\" attribute=\"clientavailability\" operator=\"in\"><value>2</value><value>3</value></condition></filter><filter type=\"and\"><filter type=\"and\"><condition attribute=\"startdate\" operator=\"le\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"expirydate\" operator=\"ge\" value=\"03/29/2022 15:05\"/></filter><filter type=\"and\"><condition attribute=\"recordid\" operator=\"eq\" value=\"" +opportunityId+"\"/></filter><filter type=\"and\"><condition attribute=\"recordidobjecttypecode2\" operator=\"eq\" value=\"3\"/></filter></filter></entity></fetch>"));
            yield return request47;
            request47 = null;
            this.AddCommentToResult("48_Enter Opportunity Close Details and Click OK");

            CrmRequest request48 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request48.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request48.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "58ea5528-5d73-4507-b7a3-1e7a6d1c2b1e"));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request48.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request48.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "4e3600fa-b9c8-49f4-b69a-51eba06d9bdf"));
            request48.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "c68e07b3-76db-4834-abfc-c2202638ca56"));
            request48.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request48.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request48.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request48.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request48.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request48.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""2""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1000""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request48;
            request48 = null;
            this.AddCommentToResult("49_Enter Opportunity Close Details and Click OK");

            CrmRequest request49 = new CrmRequest(thisURL+"/api/data/v9.0/connections");
            request49.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request49.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "c6dda377-f38b-4b11-8a74-03c9d289c942"));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request49.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request49.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "fe4bc089-8901-466c-a41b-1c1090f204d4"));
            request49.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "c68e07b3-76db-4834-abfc-c2202638ca56"));
            request49.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request49.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request49.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request49.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request49.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request49.QueryStringParameters.Add("fetchXml", @"<fetch mapping=""logical"" distinct=""false"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""connection""><attribute name=""name""/><attribute name=""entityimage_url""/><attribute name=""statecode""/><attribute name=""record2id""/><attribute name=""record2roleid""/><attribute name=""connectionid""/><link-entity name=""systemuser"" from=""systemuserid"" to=""record2id"" alias=""systemuser""><attribute name=""internalemailaddress""/></link-entity><order attribute=""record2id"" descending=""false""/><filter type=""and""><condition attribute=""record2objecttypecode"" operator=""eq"" value=""8""/></filter><link-entity name=""connectionrole"" from=""connectionroleid"" to=""record2roleid"" alias=""ac""><filter type=""and""><condition attribute=""category"" operator=""eq"" value=""1001""/></filter></link-entity><link-entity name=""opportunity"" from=""opportunityid"" to=""record1id"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></entity></fetch>");
            yield return request49;
            request49 = null;
            this.AddCommentToResult("50_Enter Opportunity Close Details and Click OK");

            CrmRequest request50 = new CrmRequest(thisURL+"/api/data/v9.0/competitors");
            request50.Headers.Add(new WebTestRequestHeader("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\""));
            request50.Headers.Add(new WebTestRequestHeader("clienthost", "Browser"));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-client-session-id", "542de7bc-c49e-450c-a94d-e0c56a5a1f0b"));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-sw-tenantid", "31259ba9-4181-48d9-aea8-a8e53c06f73a"));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-mdl-operation-info", "Type=RetrieveMultiple; Subtype=Subgrid; Source=Default"));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-client-request-id", "de1214fa-21c0-4f90-80c7-e6eaa4590beb"));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-app-name", "msdynce_saleshub"));
            request50.Headers.Add(new WebTestRequestHeader("sec-ch-ua-platform", "\"Windows\""));
            request50.Headers.Add(new WebTestRequestHeader("prefer", "odata.include-annotations=\"*\""));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-source-id", "57bca9ac-87a0-4c28-adc8-2d0a4645f29e"));
            request50.Headers.Add(new WebTestRequestHeader("sec-ch-ua-mobile", "?0"));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-app-id", "a9a86d48-06ad-ec11-9841-000d3a3bc2af"));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-sw-objectid", "40f400ec-b5d3-460a-8977-fad321eb2583"));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-correlation-id", "c68e07b3-76db-4834-abfc-c2202638ca56"));
            request50.Headers.Add(new WebTestRequestHeader("x-ms-user-agent", "PowerApps-UCI/1.4.3983-2202.3 (Browser; AppName=msdynce_saleshub)"));
            request50.Headers.Add(new WebTestRequestHeader("content-type", "application/json"));
            request50.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Site", "same-origin"));
            request50.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Mode", "cors"));
            request50.Headers.Add(new WebTestRequestHeader("Sec-Fetch-Dest", "empty"));
            request50.QueryStringParameters.Add("fetchXml", @"<fetch distinct=""false"" mapping=""logical"" returntotalrecordcount=""true"" page=""1"" count=""6"" no-lock=""false""><entity name=""competitor""><attribute name=""entityimage_url""/><attribute name=""name""/><attribute name=""websiteurl""/><attribute name=""competitorid""/><order attribute=""name"" descending=""false""/><link-entity name=""opportunitycompetitors"" intersect=""true"" visible=""false"" to=""competitorid"" from=""competitorid""> <link-entity name=""opportunity"" from=""opportunityid"" to=""opportunityid"" alias=""bb""> <filter type=""and"">  <condition attribute=""opportunityid"" operator=""eq"" uitype=""opportunity"" value="""+opportunityId+@"""/> </filter> </link-entity></link-entity></entity></fetch>");
            yield return request50;
            request50 = null;
            this.EndTransaction("1022.08_Enter Opportunity Close Details and Click OK");

            //Insert Opportunity, Account, and Contact into ltopportunity table for future processing
            using (System.Data.SqlClient.SqlConnection emsqlCon = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
            {
                emsqlCon.Open();
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = emsqlCon;
              
                //Update Lead showing thats its been converted and no longer being used. 
                cmd.CommandText = "Update ltopportunity set inuse = '0', state='1' where opportunityid = '" + opportunityId + "'";
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
