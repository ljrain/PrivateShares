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
    public class zz_1005_SampleUploadAttachmentScriptCoded : WebTestBase
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

        public zz_1005_SampleUploadAttachmentScriptCoded()
        {
            WebRequest.RegisterPrefix("crm", new crmRequestFactory());
            this.Proxy = null;
            PreWebTest += new EventHandler<PreWebTestEventArgs>(zz_1005_SampleUploadAttachmentScriptCoded_PreWebTest);
            PostWebTest += new EventHandler<PostWebTestEventArgs>(zz_1005_SampleUploadAttachmentScriptCoded_PostWebTest);
        }
        public void zz_1005_SampleUploadAttachmentScriptCoded_PreWebTest(object sender, PreWebTestEventArgs e)
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
        public void zz_1005_SampleUploadAttachmentScriptCoded_PostWebTest(object sender, PostWebTestEventArgs e)
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
            this.BeginTransaction("1005.1_Open Contact Homepage");
            this.AddCommentToResult("Open Contact Homepage");

            CrmRequest request1 = new CrmRequest(thisURL + "/_root/homepage.aspx");
            request1.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDist", "Version=1.1"));
            request1.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDistEx", "MinContentInformation=1.0, MaxContentInformation=2.0"));
            request1.QueryStringParameters.Add("etc", "2");
            request1.QueryStringParameters.Add("pagemode", "iframe");
            request1.QueryStringParameters.Add("sitemappath", "SFA|Customers|nav_conts");
            yield return request1;
            string resresult = request1.lastResponse.BodyString;
            request1 = null;
            string contactId = Utils.extractgridresponse(resresult);
            

            this.EndTransaction("1005.1_Open Contact Homepage");
            this.BeginTransaction("1005.2_Quick Find for Contact");
            this.AddCommentToResult("Quick Find for Contact");

            CrmRequest request2 = new CrmRequest(thisURL + "/AppWebServices/AppGridWebService.ashx");
            request2.Method = "POST";
            request2.Headers.Add(new WebTestRequestHeader("referer", thisURL));
            request2.Headers.Add(new WebTestRequestHeader("ReferrerReqId", "e9295c11-558b-4f23-b0b2-6dc13680dc76"));
            request2.Headers.Add(new WebTestRequestHeader("Content-Type", "text/plain;charset=UTF-8"));
            request2.QueryStringParameters.Add("id", "crmGrid");
            request2.QueryStringParameters.Add("operation", "Reset");
            StringHttpBody request2Body = new StringHttpBody();
            request2Body.ContentType = "text/plain;charset=UTF-8";
            request2Body.InsertByteOrderMark = false;
            request2Body.BodyString = "<grid><sortColumns>fullname&#58;1</sortColumns><pageNum>1</pageNum><recsPerPage>50</recsPerPage><dataProvider>Microsoft.Crm.Application.Platform.Grid.GridDataProviderQueryBuilder</dataProvider><uiProvider>Microsoft.Crm.Application.Controls.GridUIProvider</uiProvider><cols/><max>-1</max><refreshAsync>False</refreshAsync><pagingCookie/><enableMultiSort>true</enableMultiSort><enablePagingWhenOnePage>true</enablePagingWhenOnePage><parameters><autorefresh>1</autorefresh><isGridHidden>false</isGridHidden><isGridFilteringEnabled>1</isGridFilteringEnabled><viewid>&#123;8DF19B44-A073-40C3-9D6D-EE1355D8C4BA&#125;</viewid><viewtype>1039</viewtype><RecordsPerPage>50</RecordsPerPage><viewTitle>Active Contacts</viewTitle><otc>2</otc><otn>contact</otn><entitydisplayname>Contact</entitydisplayname><titleformat>&#123;0&#125; &#123;1&#125;</titleformat><entitypluraldisplayname>Contacts</entitypluraldisplayname><isWorkflowSupported>true</isWorkflowSupported><fetchXmlForFilters>&#60;fetch version&#61;&#34;1.0&#34; output-format&#61;&#34;xml-platform&#34; mapping&#61;&#34;logical&#34;&#62;&#60;entity name&#61;&#34;contact&#34;&#62;&#60;attribute name&#61;&#34;fullname&#34; &#47;&#62;&#60;order attribute&#61;&#34;fullname&#34; descending&#61;&#34;false&#34; &#47;&#62;&#60;attribute name&#61;&#34;parentcustomerid&#34; &#47;&#62;&#60;filter type&#61;&#34;and&#34;&#62;&#60;condition attribute&#61;&#34;statecode&#34; operator&#61;&#34;eq&#34; value&#61;&#34;0&#34; &#47;&#62;&#60;&#47;filter&#62;&#60;attribute name&#61;&#34;telephone1&#34; &#47;&#62;&#60;attribute name&#61;&#34;emailaddress1&#34; &#47;&#62;&#60;attribute name&#61;&#34;ownerid&#34; &#47;&#62;&#60;link-entity alias&#61;&#34;a_dc9b80f8c78146d89fd6a3b610836975&#34; name&#61;&#34;account&#34; from&#61;&#34;accountid&#34; to&#61;&#34;parentcustomerid&#34; link-type&#61;&#34;outer&#34; visible&#61;&#34;false&#34;&#62;&#60;attribute name&#61;&#34;address1_country&#34; &#47;&#62;&#60;&#47;link-entity&#62;&#60;attribute name&#61;&#34;address1_city&#34; &#47;&#62;&#60;attribute name&#61;&#34;int_kloutscore&#34; &#47;&#62;&#60;attribute name&#61;&#34;contactid&#34; &#47;&#62;&#60;attribute name&#61;&#34;processid&#34; &#47;&#62;&#60;link-entity alias&#61;&#34;processidworkflowworkflowid&#34; name&#61;&#34;workflow&#34; from&#61;&#34;workflowid&#34; to&#61;&#34;processid&#34; link-type&#61;&#34;outer&#34; visible&#61;&#34;false&#34;&#62;&#60;attribute name&#61;&#34;versionnumber&#34; &#47;&#62;&#60;&#47;link-entity&#62;&#60;&#47;entity&#62;&#60;&#47;fetch&#62;</fetchXmlForFilters><isFetchXmlNotFinal>False</isFetchXmlNotFinal><effectiveFetchXml>&#60;fetch distinct&#61;&#34;false&#34; no-lock&#61;&#34;false&#34; mapping&#61;&#34;logical&#34; page&#61;&#34;1&#34; count&#61;&#34;50&#34; returntotalrecordcount&#61;&#34;true&#34;&#62;&#60;entity name&#61;&#34;contact&#34;&#62;&#60;attribute name&#61;&#34;fullname&#34; &#47;&#62;&#60;attribute name&#61;&#34;parentcustomerid&#34; &#47;&#62;&#60;attribute name&#61;&#34;telephone1&#34; &#47;&#62;&#60;attribute name&#61;&#34;emailaddress1&#34; &#47;&#62;&#60;attribute name&#61;&#34;ownerid&#34; &#47;&#62;&#60;attribute name&#61;&#34;address1_city&#34; &#47;&#62;&#60;attribute name&#61;&#34;int_kloutscore&#34; &#47;&#62;&#60;attribute name&#61;&#34;contactid&#34; &#47;&#62;&#60;attribute name&#61;&#34;processid&#34; &#47;&#62;&#60;attribute name&#61;&#34;fullname&#34; &#47;&#62;&#60;attribute name&#61;&#34;emailaddress1&#34; &#47;&#62;&#60;attribute name&#61;&#34;parentcustomerid&#34; &#47;&#62;&#60;attribute name&#61;&#34;telephone1&#34; &#47;&#62;&#60;attribute name&#61;&#34;ownerid&#34; &#47;&#62;&#60;attribute name&#61;&#34;address1_city&#34; &#47;&#62;&#60;attribute name&#61;&#34;int_kloutscore&#34; &#47;&#62;&#60;filter type&#61;&#34;and&#34;&#62;&#60;condition attribute&#61;&#34;statecode&#34; operator&#61;&#34;eq&#34; value&#61;&#34;0&#34; &#47;&#62;&#60;&#47;filter&#62;&#60;order attribute&#61;&#34;fullname&#34; descending&#61;&#34;false&#34; &#47;&#62;&#60;link-entity name&#61;&#34;account&#34; to&#61;&#34;parentcustomerid&#34; from&#61;&#34;accountid&#34; link-type&#61;&#34;outer&#34; alias&#61;&#34;a_dc9b80f8c78146d89fd6a3b610836975&#34;&#62;&#60;attribute name&#61;&#34;address1_country&#34; &#47;&#62;&#60;&#47;link-entity&#62;&#60;link-entity name&#61;&#34;workflow&#34; to&#61;&#34;processid&#34; from&#61;&#34;workflowid&#34; link-type&#61;&#34;outer&#34; alias&#61;&#34;processidworkflowworkflowid&#34;&#62;&#60;attribute name&#61;&#34;versionnumber&#34; &#47;&#62;&#60;&#47;link-entity&#62;&#60;&#47;entity&#62;&#60;&#47;fetch&#62;</effectiveFetchXml><LayoutStyle>GridList</LayoutStyle><enableFilters>1</enableFilters><quickfind>lt_</quickfind><filter/><filterDisplay/></parameters></grid>";
            request2.Body = request2Body;
            yield return request2;
            request2 = null;
            this.EndTransaction("1005.2_Quick Find for Contact");
            this.BeginTransaction("1005.3_Open Contact Record");
            this.AddCommentToResult("Open Contact Record");

            CrmRequest request3 = new CrmRequest(thisURL + "/form/Data.aspx");
            request3.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDist", "Version=1.1"));
            request3.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDistEx", "MinContentInformation=1.0, MaxContentInformation=2.0"));
            request3.QueryStringParameters.Add("_CreateFromId", "");
            request3.QueryStringParameters.Add("_CreateFromType", "");
            request3.QueryStringParameters.Add("_gridType", "2");
            request3.QueryStringParameters.Add("counter", "1493215307621");
            request3.QueryStringParameters.Add("etc", "2");
            request3.QueryStringParameters.Add("extraqs", "?_gridType=2&etc=2&id=%7b"+contactId+"%7d&process=&rskey=%7b8DF19B44-A073-40C3-9D6D-EE1355D8C4BA%7d");
            request3.QueryStringParameters.Add("formid", "1fed44d1-ae68-4a41-bd2b-f13acac4acfa");
            request3.QueryStringParameters.Add("id", "{"+contactId+"}");
            request3.QueryStringParameters.Add("oid", ""+contactId+"");
            request3.QueryStringParameters.Add("pagemode", "iframe");
            request3.QueryStringParameters.Add("pagetype", "entityrecord");
            request3.QueryStringParameters.Add("process", "");
            request3.QueryStringParameters.Add("rskey", "{8DF19B44-A073-40C3-9D6D-EE1355D8C4BA}");

         
            CrmRequest request3Dependent2 = new CrmRequest(thisURL + "/AppWebServices/AppGridWebService.ashx");
            request3Dependent2.Method = "POST";
            request3Dependent2.Headers.Add(new WebTestRequestHeader("referer", thisURL));
            request3Dependent2.Headers.Add(new WebTestRequestHeader("FormLoadId", "{106213d4-9aa7-b32e-c9d8-a7a01e1028e2}"));
            request3Dependent2.Headers.Add(new WebTestRequestHeader("Content-Type", "text/plain;charset=UTF-8"));
            request3Dependent2.QueryStringParameters.Add("operation", "Refresh");
            StringHttpBody request3Dependent2Body = new StringHttpBody();
            request3Dependent2Body.ContentType = "text/plain;charset=UTF-8";
            request3Dependent2Body.InsertByteOrderMark = false;
            request3Dependent2Body.BodyString = "<grid><sortColumns>statecode&#58;0&#59;prioritycode&#58;1</sortColumns><pageNum>1</pageNum><recsPerPage>5</recsPerPage><dataProvider>Microsoft.Crm.Application.Platform.Grid.GridDataProviderQueryBuilder</dataProvider><uiProvider>Microsoft.Crm.Application.Controls.GridUIProvider</uiProvider><cols/><max>1</max><refreshAsync>True</refreshAsync><pagingCookie/><enableMultiSort>true</enableMultiSort><enablePagingWhenOnePage>true</enablePagingWhenOnePage><refreshCalledFromRefreshButton>1</refreshCalledFromRefreshButton><returntotalrecordcount>True</returntotalrecordcount><getParameters>getFetchXmlForFilters</getParameters><parameters><viewid>&#123;EBD1D24A-EEA7-E211-9FB6-00155DD0EA05&#125;</viewid><autorefresh>1</autorefresh><isGridHidden>false</isGridHidden><LayoutStyle>LiteGridList</LayoutStyle><maxselectableitems>1</maxselectableitems><isGridFilteringEnabled>1</isGridFilteringEnabled><viewtype>1039</viewtype><viewts>822500</viewts><RecordsPerPage>5</RecordsPerPage><viewTitle>Recent Cases</viewTitle><layoutXml>&#60;grid name&#61;&#34;resultset&#34; object&#61;&#34;112&#34; jump&#61;&#34;title&#34; select&#61;&#34;1&#34; preview&#61;&#34;1&#34; icon&#61;&#34;1&#34;&#62;&#60;row name&#61;&#34;result&#34; id&#61;&#34;incidentid&#34;&#62;&#60;cell name&#61;&#34;title&#34; width&#61;&#34;150&#34; &#47;&#62;&#60;cell name&#61;&#34;ticketnumber&#34; width&#61;&#34;150&#34; &#47;&#62;&#60;cell name&#61;&#34;prioritycode&#34; width&#61;&#34;100&#34; &#47;&#62;&#60;cell name&#61;&#34;caseorigincode&#34; width&#61;&#34;100&#34; &#47;&#62;&#60;cell name&#61;&#34;customerid&#34; width&#61;&#34;150&#34; &#47;&#62;&#60;cell name&#61;&#34;ownerid&#34; width&#61;&#34;150&#34; &#47;&#62;&#60;cell name&#61;&#34;statecode&#34; width&#61;&#34;100&#34; &#47;&#62;&#60;cell name&#61;&#34;statuscode&#34; width&#61;&#34;150&#34; &#47;&#62;&#60;cell name&#61;&#34;createdon&#34; width&#61;&#34;100&#34; &#47;&#62;&#60;&#47;row&#62;&#60;&#47;grid&#62;</layoutXml><otc>112</otc><otn>incident</otn><entitydisplayname>Case</entitydisplayname><titleformat>&#123;0&#125; &#123;1&#125;</titleformat><entitypluraldisplayname>Cases</entitypluraldisplayname><expandable>1</expandable><showjumpbar>0</showjumpbar><maxrowsbeforescroll>5</maxrowsbeforescroll><tabindex>1670</tabindex><refreshasynchronous>1</refreshasynchronous><subgridAutoExpand>0</subgridAutoExpand><relName>incident_customer_contacts</relName><roleOrd>-1</roleOrd><oType></oType><relationshipType>1</relationshipType><ribbonContext>SubGridStandard</ribbonContext><GridType>SubGrid</GridType><enableContextualActions>True</enableContextualActions><teamTemplateId></teamTemplateId><isWorkflowSupported>true</isWorkflowSupported><LoadOnDemand_GridEmptyMessage>To load &#123;0&#125; records, click here.</LoadOnDemand_GridEmptyMessage><enableFilters></enableFilters><RenderAsync>0</RenderAsync><oId>"+contactId+"</oId><oType>2</oType><isTurboForm>1</isTurboForm></parameters><columns><column width=\"150\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Case&#32;Title\" fieldname=\"title\" entityname=\"incident\" renderertype=\"Crm.PrimaryField\">title</column><column width=\"150\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Case&#32;Number\" fieldname=\"ticketnumber\" entityname=\"incident\">ticketnumber</column><column width=\"100\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Priority\" fieldname=\"prioritycode\" entityname=\"incident\">prioritycode</column><column width=\"100\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Origin\" fieldname=\"caseorigincode\" entityname=\"incident\">caseorigincode</column><column width=\"150\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Customer\" fieldname=\"customerid\" entityname=\"incident\">customerid</column><column width=\"150\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Owner\" fieldname=\"ownerid\" entityname=\"incident\">ownerid</column><column width=\"100\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Status\" fieldname=\"statecode\" entityname=\"incident\">statecode</column><column width=\"150\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Status&#32;Reason\" fieldname=\"statuscode\" entityname=\"incident\">statuscode</column><column width=\"100\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Created&#32;On\" fieldname=\"createdon\" entityname=\"incident\">createdon</column></columns></grid>";
            request3Dependent2.Body = request3Dependent2Body;
            request3.DependentRequests.Add(request3Dependent2);

            CrmRequest request3Dependent3 = new CrmRequest(thisURL + "/AppWebServices/AppGridWebService.ashx");
            request3Dependent3.Method = "POST";
            request3Dependent3.Headers.Add(new WebTestRequestHeader("referer", thisURL));
            request3Dependent3.Headers.Add(new WebTestRequestHeader("FormLoadId", "{106213d4-9aa7-b32e-c9d8-a7a01e1028e2}"));
            request3Dependent3.Headers.Add(new WebTestRequestHeader("Content-Type", "text/plain;charset=UTF-8"));
            request3Dependent3.QueryStringParameters.Add("operation", "Refresh");
            StringHttpBody request3Dependent3Body = new StringHttpBody();
            request3Dependent3Body.ContentType = "text/plain;charset=UTF-8";
            request3Dependent3Body.InsertByteOrderMark = false;
            request3Dependent3Body.BodyString = "<grid><sortColumns>statecode&#58;1&#59;estimatedclosedate&#58;0&#59;modifiedon&#58;0</sortColumns><pageNum>1</pageNum><recsPerPage>5</recsPerPage><dataProvider>Microsoft.Crm.Application.Platform.Grid.GridDataProviderQueryBuilder</dataProvider><uiProvider>Microsoft.Crm.Application.Controls.GridUIProvider</uiProvider><cols/><max>1</max><refreshAsync>True</refreshAsync><pagingCookie/><enableMultiSort>true</enableMultiSort><enablePagingWhenOnePage>true</enablePagingWhenOnePage><refreshCalledFromRefreshButton>1</refreshCalledFromRefreshButton><returntotalrecordcount>True</returntotalrecordcount><getParameters>getFetchXmlForFilters</getParameters><parameters><viewid>&#123;9C3F7EE8-ECA7-E211-9FB6-00155DD0EA05&#125;</viewid><autorefresh>1</autorefresh><isGridHidden>false</isGridHidden><LayoutStyle>LiteGridList</LayoutStyle><maxselectableitems>1</maxselectableitems><isGridFilteringEnabled>1</isGridFilteringEnabled><viewtype>1039</viewtype><viewts>822499</viewts><RecordsPerPage>5</RecordsPerPage><viewTitle>Recent Opportunities</viewTitle><layoutXml>&#60;grid name&#61;&#34;resultset&#34; object&#61;&#34;3&#34; jump&#61;&#34;name&#34; select&#61;&#34;1&#34; preview&#61;&#34;1&#34; icon&#61;&#34;1&#34;&#62;&#60;row name&#61;&#34;result&#34; id&#61;&#34;opportunityid&#34;&#62;&#60;cell name&#61;&#34;name&#34; width&#61;&#34;150&#34; &#47;&#62;&#60;cell name&#61;&#34;statecode&#34; width&#61;&#34;100&#34; &#47;&#62;&#60;cell name&#61;&#34;actualclosedate&#34; width&#61;&#34;100&#34; &#47;&#62;&#60;cell name&#61;&#34;actualvalue&#34; width&#61;&#34;100&#34; &#47;&#62;&#60;cell name&#61;&#34;estimatedclosedate&#34; width&#61;&#34;100&#34; &#47;&#62;&#60;cell name&#61;&#34;estimatedvalue&#34; width&#61;&#34;100&#34; &#47;&#62;&#60;&#47;row&#62;&#60;&#47;grid&#62;</layoutXml><otc>3</otc><otn>opportunity</otn><entitydisplayname>Opportunity</entitydisplayname><titleformat>&#123;0&#125; &#123;1&#125;</titleformat><entitypluraldisplayname>Opportunities</entitypluraldisplayname><expandable>1</expandable><showjumpbar>0</showjumpbar><maxrowsbeforescroll>5</maxrowsbeforescroll><tabindex>1680</tabindex><refreshasynchronous>1</refreshasynchronous><subgridAutoExpand>0</subgridAutoExpand><relName>opportunity_customer_contacts</relName><roleOrd>-1</roleOrd><oType></oType><relationshipType>1</relationshipType><ribbonContext>SubGridStandard</ribbonContext><GridType>SubGrid</GridType><enableContextualActions>true</enableContextualActions><teamTemplateId></teamTemplateId><isWorkflowSupported>true</isWorkflowSupported><LoadOnDemand_GridEmptyMessage>To load &#123;0&#125; records, click here.</LoadOnDemand_GridEmptyMessage><enableFilters></enableFilters><RenderAsync>0</RenderAsync><oId>"+contactId+"</oId><oType>2</oType><isTurboForm>1</isTurboForm></parameters><columns><column width=\"150\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Topic\" fieldname=\"name\" entityname=\"opportunity\" renderertype=\"Crm.PrimaryField\">name</column><column width=\"100\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Status\" fieldname=\"statecode\" entityname=\"opportunity\">statecode</column><column width=\"100\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Actual&#32;Close&#32;Date\" fieldname=\"actualclosedate\" entityname=\"opportunity\">actualclosedate</column><column width=\"100\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Actual&#32;Revenue\" fieldname=\"actualvalue\" entityname=\"opportunity\">actualvalue</column><column width=\"100\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Est.&#32;Close&#32;Date\" fieldname=\"estimatedclosedate\" entityname=\"opportunity\">estimatedclosedate</column><column width=\"100\" isHidden=\"false\" isMetadataBound=\"true\" isSortable=\"true\" label=\"Est.&#32;Revenue\" fieldname=\"estimatedvalue\" entityname=\"opportunity\">estimatedvalue</column></columns></grid>";
            request3Dependent3.Body = request3Dependent3Body;
            request3.DependentRequests.Add(request3Dependent3);

            yield return request3;
                //Extract WRPCToken for the InlineEditWebService
                string[] TokenValues = Utils.ExtractWRPCToken(request3.lastResponse);
                Token = TokenValues[0];
                Timestamp = TokenValues[1];
                Context.Add("Token", Token);
                Context.Add("TokenTimeStamp", Timestamp);
            request3 = null;

            CrmRequest request4 = new CrmRequest(thisURL + "/AppWebServices/LookupService.asmx");
            request4.Method = "POST";
            request4.Headers.Add(new WebTestRequestHeader("ReferrerReqId", "1d392671-3fff-4123-b690-0037209c93fc"));
            request4.Headers.Add(new WebTestRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2009/WebServices/RetrieveItem"));
            request4.Headers.Add(new WebTestRequestHeader("Content-Type", "text/xml; charset=utf-8"));
            request4.Headers.Add(new WebTestRequestHeader("FormLoadId", "{106213d4-9aa7-b32e-c9d8-a7a01e1028e2}"));
            StringHttpBody request4Body = new StringHttpBody();
            request4Body.ContentType = "text/xml; charset=utf-8";
            request4Body.InsertByteOrderMark = false;
            request4Body.BodyString = @"<?xml version=""1.0"" encoding=""utf-8"" ?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><RetrieveItem xmlns=""http://schemas.microsoft.com/crm/2009/WebServices""><typesArray><int>8</int></typesArray><guidValues><string>a748045d-8675-4388-9955-92101f447fe1</string></guidValues><lookupValues></lookupValues><positions></positions><additionalParameters></additionalParameters></RetrieveItem></soap:Body></soap:Envelope>";
            request4.Body = request4Body;
            yield return request4;
            request4 = null;
            this.EndTransaction("1005.3_Open Contact Record");
            this.BeginTransaction("1005.4_Attach Text File to Contact");
            this.AddCommentToResult("Attach Text File to Contact");

            CrmRequest request5 = new CrmRequest(thisURL + "/AppWebServices/Annotation.asmx");
            request5.Method = "POST";
            request5.Headers.Add(new WebTestRequestHeader("ReferrerReqId", "1d392671-3fff-4123-b690-0037209c93fc"));
            request5.Headers.Add(new WebTestRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2009/WebServices/RetrieveNotesCollection"));
            request5.Headers.Add(new WebTestRequestHeader("Content-Type", "text/xml; charset=utf-8"));
            StringHttpBody request5Body = new StringHttpBody();
            request5Body.ContentType = "text/xml; charset=utf-8";
            request5Body.InsertByteOrderMark = false;
            request5Body.BodyString = @"<?xml version=""1.0"" encoding=""utf-8"" ?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><RetrieveNotesCollection xmlns=""http://schemas.microsoft.com/crm/2009/WebServices""><parentEntityId>&#123;"+contactId+"&#125;</parentEntityId><pageNumber>1</pageNumber><pageSize>10</pageSize><pagingCookie></pagingCookie></RetrieveNotesCollection></soap:Body></soap:Envelope>";
            request5.Body = request5Body;
            yield return request5;
            request5 = null;
            this.AddCommentToResult("Attach Text File to Contact");

            CrmRequest request6 = new CrmRequest(thisURL + "/notes/notesv2attach.aspx");
            request6.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDist", "Version=1.1"));
            request6.Headers.Add(new WebTestRequestHeader("X-P2P-PeerDistEx", "MinContentInformation=1.0, MaxContentInformation=2.0"));
            yield return request6;

                //Extract WRPCToken for the upload.aspx
                string[] TokenValues2 = Utils.ExtractUploadWRPCToken(request6.lastResponse);
                string UploadToken = TokenValues2[0];
                string UploadTimestamp = TokenValues2[1];
            request6 = null;
            this.AddCommentToResult("Attach Text File to Contact");
                        
            string filename = "C:\\CRMToolkitCSA\\CRM_Perf_Toolkit\\CRM_Perf_BenchMark\\Xtras\\TestAsset\\attachment_50kb.txt";            
            string contenttype = "text/plain";
            CrmRequest request7 = new CrmRequest(thisURL + "/Activities/Attachment/upload.aspx");
            request7.Timeout = 60;
            request7.Method = "POST";
            request7.Headers.Add(new WebTestRequestHeader("Content-Type", "multipart/form-data; boundary=---------------------------7e1db2b2026c"));
            request7.Headers.Add(new WebTestRequestHeader("Pragma", "no-cache"));
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "multipart/form-data; boundary=---------------------------7e1db2b2026c";
            request7Body.InsertByteOrderMark = false;
            FormPostHttpBody request7aBody = new FormPostHttpBody();
            request7aBody.FormPostParameters.Add("CRMWRPCToken", UploadToken);
            request7aBody.FormPostParameters.Add("CRMWRPCTokenTimeStamp", UploadTimestamp);
            request7aBody.FormPostParameters.Add("appSolutionId", "");
            request7aBody.FormPostParameters.Add("CloseWindow", "False");
            request7aBody.FormPostParameters.Add("AttachmentType", "5");
            request7aBody.FormPostParameters.Add("AttachmentId", "");
            request7aBody.FormPostParameters.Add("UserId", systemuserId);
            request7aBody.FormPostParameters.Add("crmFormSubmitXml", "<annotation><isdocument>0</isdocument><notetext>UploadAttachment</notetext><objectid>&#123;" + contactId + "&#125;</objectid><objecttypecode>contact</objecttypecode><subject></subject><ownerid type='8'>" + systemuserId + "</ownerid></annotation>");
            request7aBody.FormPostParameters.Add("IsSPDocument", "false");
            request7aBody.FormPostParameters.Add("OverwriteExistingFile", "false");
            request7aBody.FormPostParameters.Add("hideDesc", "");
            request7aBody.FormPostParameters.Add("MerchantId", "{28260555-16E7-E611-80CE-00155D0D3207}");
            request7aBody.FormPostParameters.Add("ErrorURL", "/_common/error/uploadFailure.aspx");
            request7aBody.FormPostParameters.Add("SuccessURL", "/Notes/NotesV2Attach.aspx?cw=False&id=%7bFDFE4E55-DB25-E711-80E6-00155D99F202%7d&pId=&pType=&refreshGrid=1&rg=1");
            request7aBody.FormPostParameters.Add("PopupErrorUrl", "");
            request7aBody.FormPostParameters.Add(new FileUploadParameter("userFile", filename, contenttype));
            request7.Body = request7aBody;
            yield return request7;
            request7 = null;

            this.EndTransaction("1005.4_Attach Text File to Contact");
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
