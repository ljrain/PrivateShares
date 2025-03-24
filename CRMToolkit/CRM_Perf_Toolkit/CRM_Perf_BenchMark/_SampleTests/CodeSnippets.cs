//CODE SAMPLES

//Increase how much of response body is ready by Visual Studio. By Default, Visual Studio will truncate after 1.5MB. This is needed for extraction rule to work properly on large response bodies. 
//this.ResponseBodyCaptureLimit = 5000000;

//Mark the test as failed while setting an error message. This can be used for validation
//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Manually fail the test.");

//Another option to mark a test as failed is by setting the Outcome property. 
//this.Outcome = Outcome.Fail;


////PrewebTest Code for Filtering to Specific User
//Context.Add("domainname", "CRMTest001@contoso.onmicrosoft.com");

//RegEx Remove Line Return Base64 Responses
//\r\n.{78}


////PrewebTest Code for Filtering to List of Users
//List<string> users = new List<string>();
//users.Add("CRMTest002@contoso.onmicrosoft.com");
//users.Add("CRMTest003@contoso.onmicrosoft.com");
//users.Add("CRMTest004@contoso.onmicrosoft.com");
//users.Add("CRMTest008@contoso.onmicrosoft.com");
//users.Add("CRMTest009@contoso.onmicrosoft.com");
//users.Add("CRMTest010@contoso.onmicrosoft.com");
//users.Add("CRMTest015@contoso.onmicrosoft.com");
//users.Add("CRMTest016@contoso.onmicrosoft.com");
//users.Add("CRMTest017@contoso.onmicrosoft.com");
//users.Add("CRMTest018@contoso.onmicrosoft.com");
//users.Add("CRMTest019@contoso.onmicrosoft.com");
//users.Add("CRMTest020@contoso.onmicrosoft.com");
//Context.Add("domainname", users[Utils.GetRandomInt(0,users.Count)]);


////Extract record from View/Quick Find Response.
//string resresult = requestx.lastResponse.BodyString;
//requestx = null;
//string leadId = Utils.extractgridresponse(resresult);


////Extract Record from Lookup results
//string resresult = requestx.lastResponse.BodyString;
//requestx = null;
//string contactId = Utils.extractlookupresponse("2", resresult);



////Extract ActivityID for Newly created Activity Records
//string valuemarker3 = "<b:ActivityId>";
//int idIdx3 = requestx.lastResponse.BodyString.IndexOf(valuemarker3);
//string appointmentId = requestx.lastResponse.BodyString.Substring(idIdx3 + (valuemarker3.Length), 36);


////UCI Extract New Record ID
//string newcontactid = request17.lastResponse.Headers["Location"].Split('(')[1].Substring(0, 36);


//UCI Extract Record from UCI View/QuickFind
//string resresult = requestx.lastResponse.BodyString;
//requestx = null;
//string contactId = Utils.extractUCIgridresponse("contactid\":\"",resresult);


////Sample to Retrieve Records with WebAPI requests. This can be used to retrieve random records to work with instead of using a view or quick find. 
//this.BeginTransaction("01.005_GetSampleAccount");
//string accountId;

//CrmRequest request01_5 = new CrmRequest(thisURL + "/api/data/v9.0/accounts");
//request01_5.Headers.Add(new WebTestRequestHeader("OData-MaxVersion", "4.0"));
//request01_5.Headers.Add(new WebTestRequestHeader("OData-Version", "4.0"));
//request01_5.Headers.Add(new WebTestRequestHeader("Content-Type", "application/json; charset=utf-8"));
//request01_5.Headers.Add(new WebTestRequestHeader("Prefer", "odata.include-annotations=\"*\""));
//request01_5.QueryStringParameters.Add("$select", "name,accountid");
//request01_5.QueryStringParameters.Add("$top", "10");
//request01_5.QueryStringParameters.Add("$filter", "statecode eq 0");
//yield return request01_5;
//accountId = "894FAB72-2D12-E811-A830-000D3A37C8E1";
////use newtonsoft to deserialize json into a dict
//Dictionary<string, object> rootNode = JsonConvert.DeserializeObject<Dictionary<string, object>>(request01_5.lastResponse.BodyString);
//List<Dictionary<string, object>> accountlist = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(rootNode["value"].ToString());
//request01_5 = null;
//if (accountlist.Count > 0)
//{
//    accountId = accountlist[Utils.GetRandomInt(0, accountlist.Count)]["accountid"].ToString();
//}
//this.EndTransaction("01.005_GetSampleAccount");



////Extract WRPC Token and Timestamp
//string[] TokenValues = Utils.ExtractWRPCToken(requestx.lastResponse);
//Token = TokenValues[0];
//Timestamp = TokenValues[1];
//Context.Add("Token", Token);
//Context.Add("TokenTimeStamp", Timestamp);


////Extract GUID from Lookup on form such as parentcustomerid, regardingobjectid,customerid, etc... This can be added to the data.aspx when the record is opened. 
//string parentcustomerid = Utils.ExtractFormField("parentcustomerid", request3.lastResponse.BodyString);


////Get Random Date Value. This can be used to generate random date and hour for activity dates. 
//Random gen = new Random();
//int range = 5 * 365; //5 years
//string activityDate = DateTime.Today.AddDays(gen.Next(range)).ToString("yyyy-MM-ddThh");


////Generate Random String
//"+Utils.GetRandomString(10,20)+"


////Generate Random Number 
//" + Utils.GetRandomNumber(1000,9999)+"


////Extraction Rule - Hidden Fields. This is commonly needed for Portals, Manually handling Authentication and some Custom Web Pages. 
//ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
//extractionRule1.Required = true;
//extractionRule1.HtmlDecode = true;
//extractionRule1.ContextParameterName = "req1";
//requestx.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);

////Use Hidden Field
//" + Context["$HIDDENreq1.wresult"].ToString() + "




////Extraction Rule - Text. This is another method of extracting data from Portals, and custom pages. 
//ExtractText lr2er4 = new ExtractText();
//lr2er4.StartsWith = "ctx=";
//lr2er4.EndsWith = "\"";
//lr2er4.IgnoreCase = false;
//lr2er4.UseRegularExpression = false;
//lr2er4.Required = true;
//lr2er4.ExtractRandomMatch = false;
//lr2er4.Index = 0;
//lr2er4.HtmlDecode = true;
//lr2er4.SearchInHeaders = false;
//lr2er4.ContextParameterName = "ctx";
//requestx.ExtractValues += new EventHandler<ExtractionEventArgs>(lr2er4.Extract);

//"+this.Context["ctx"].ToString()+@"




////Directly Selecting Data from EntityManager Database. This is one method of randomly selecting portal users which are stored as Contacts in CRM. 
//private string loginname = "pfe_lt1";

//string cmdText = string.Format("SELECT top 1 rtrim([loginname]) from [portalusers] order by createdon desc");
//SqlConnection sqlConnteam = new SqlConnection(ConfigSettings.Default.EMSQLCNN);
//            sqlConnteam.Open();
//            SqlCommand getUsers = new SqlCommand(cmdText, sqlConnteam);            
//            using (SqlDataReader reader = getUsers.ExecuteReader())
//            {
//                while (reader.Read())
//                {                  
//                    int number = int.Parse(((reader[0].ToString().Split('@'))[0]).Substring(6)) + 1;
//                    loginname = "pfe_lt" + number.ToString();
//                }
//            }


////Directly Inserting Data into EntityManager Database. This can be used to insert new reords such as portal users as they are created. 
//try
//   {
//       using (System.Data.SqlClient.SqlConnection emsqlCon = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
//       {
//           emsqlCon.Open();
//           System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
//           cmd.Connection = emsqlCon;
//           cmd.CommandText = "insert into PortalUsers values ('" + contactId + "','" + loginname + "@outlook.com" + "','" + DateTime.Now.ToString() + "')";
//           cmd.ExecuteNonQuery();
//       }
//   }
//   catch (Exception e)
//   {
//   }



////Bulk Insert Random Users into SQL Table. Use when Sequential users or common passwords were not used.
//BULK INSERT userlist
//FROM 'C:\CRMToolkit\users.csv' 
//WITH(
//FIELDTERMINATOR = ',',
//ROWTERMINATOR = '\n'
//);



//Create SQL Tables to store records for future processing. 
//CREATE TABLE[dbo].[ltlead](


//   [leadId][uniqueidentifier] NULL,	
//    	[systemuserId] [uniqueidentifier] NULL,
//    	[state] [int] NULL,
//    	[inuse] [int] NULL,
//    	[uid] [uniqueidentifier] NULL
//    ) ON[PRIMARY]
//GO


//CREATE TABLE [dbo].[ltopportunity](

//[opportunityId][uniqueidentifier] NULL,	
//	[accountId] [uniqueidentifier] NULL,	
//	[contactId] [uniqueidentifier] NULL,	
//	[systemuserId] [uniqueidentifier] NULL,
//	[state] [int] NULL,
//	[inuse] [int] NULL,
//	[uid] [uniqueidentifier] NULL
//) ON[PRIMARY]
//GO

//Insert Record into EntityManager for future processing
//using (System.Data.SqlClient.SqlConnection emsqlCon = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
//{
//    emsqlCon.Open();
//    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
//    cmd.Connection = emsqlCon;
//    cmd.CommandText = "insert into ltlead values ('" + leadId + "','" + systemuserId + "','0','0','" + Guid.Empty.ToString() + "')";
//    cmd.ExecuteNonQuery();
//}


//Select Record from EntityManager for processing
//using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
//{

//    sqlConn.Open();
//    using (System.Data.SqlClient.SqlTransaction tran = sqlConn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
//    {
//        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
//        cmd.Transaction = tran;
//        cmd.CommandTimeout = 60;
//        cmd.Connection = sqlConn;

//        string ukey = Guid.NewGuid().ToString();

//        ////Set User to InUse
//        cmd.CommandText = "UPDATE ltlead set inuse='1', uid = '" + ukey + "' where leadid in (select top 1 leadId from ltlead where state = 0 and inuse = 0 order by newid())";
//        cmd.ExecuteNonQuery();

//        cmd.CommandText = string.Format("SELECT top 1 rtrim(leadId) from ltlead(nolock) where uid = '" + ukey + "'");

//        System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
//        cmd.Parameters.Clear();

//        //Lock this guy and commit the transaction.
//        while (true == reader.Read())
//        {
//            //Read User
//            leadId = reader[0].ToString();
//        }
//        if (!reader.IsClosed)
//            reader.Close();

//        tran.Commit();
//    }
//    sqlConn.Close();
//}


//Update Record in EntityManager once processing is finished
//using (System.Data.SqlClient.SqlConnection emsqlCon = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
//{
//    emsqlCon.Open();
//    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
//    cmd.Connection = emsqlCon;      
//    cmd.CommandText = "Update ltlead set inuse = '0', state='1' where leadid = '" + leadId + "'";
//    cmd.ExecuteNonQuery();
//}


//Use with MFA or when EMDBLoader is not funtional. 
//Select User from UserList and add CRMOwinAuth to Context
//string crmowinauth = null;
//#region auth
//string crmowinauth = null;
//string d365token = null;
//string powerplatformtoken = null;
//string expiration = null;
//string cmdText = string.Format("SELECT top 1 rtrim([username]),rtrim([password]),rtrim([crmowinauth]),rtrim([d365token]),rtrim([powerplatformtoken]),rtrim([systemuserid]),rtrim([expiration]) from [userlist](nolock) order by newid()");
//try
//{
//    using (System.Data.SqlClient.SqlConnection sqlConnteam = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
//    {
//        sqlConnteam.Open();
//        System.Data.SqlClient.SqlCommand getUsers = new System.Data.SqlClient.SqlCommand(cmdText, sqlConnteam);
//        using (System.Data.SqlClient.SqlDataReader reader = getUsers.ExecuteReader())
//        {
//            while (reader.Read())
//            {
//                e.WebTest.UserName = reader[0].ToString();
//                crmowinauth = reader[2].ToString();
//                d365token = reader[3].ToString();
//                powerplatformtoken = reader[4].ToString();
//                systemuserId = reader[5].ToString();
//                expiration = reader[6].ToString();
//            }
//            reader.Close();
//        }
//        sqlConnteam.Close();
//    }
//    Context.Add("systemuserId", systemuserId);
//}
//catch (Exception ex) { }

//if (string.IsNullOrEmpty(crmowinauth) || string.IsNullOrEmpty(expiration) || DateTime.Parse(expiration) < DateTime.Now.AddMinutes(-15))
//{
//    user = WebTestBase_PreWebTest(sender, e)[EntityNames.Users] as CRMEntity;
//    systemuserId = user["systemuserid"];

//    crmServer = user["serverbaseurl"];
//    e.WebTest.UserName = user["domainName"];
//    e.WebTest.Password = user["userpassword"];
//    orgId = new Guid(user["organizationid"]);
//    orgName = user["organizationname"];
//    Context.Add("UserName", e.WebTest.UserName);

//    //Canvas App Authentication
//    //bearer tokens are programatically generated for different canvas app components

//    //TenantId must be updated
//    string tenantid = "9a926422-5ce1-424f-bf67-ef768c96ae5b";
//    Authenticator auth1 = new Authenticator();
//    var securePass = new System.Security.SecureString();
//    foreach (char c in Password.ToCharArray())
//    {
//        securePass.AppendChar(c);
//    }

//    List<string> powerplatformscopes = new List<string>();
//    powerplatformscopes.Add("https://api.powerplatform.com//.default");
//    AuthenticationResult powerplatformresult = auth1.GetToken(tenantid, UserName, securePass, powerplatformscopes).GetAwaiter().GetResult();
//    var powerplatformtoken = powerplatformresult.AccessToken;
//    Context.Add("powerplatforms_access_token", powerplatformtoken);

//    List<string> d365scopes = new List<string>();
//    d365scopes.Add("https://" + orgName + ".crm.dynamics.com//.default");
//    AuthenticationResult d365result = auth1.GetToken(tenantid, UserName, securePass, d365scopes).GetAwaiter().GetResult();
//    var d365token = d365result.AccessToken;
//    Context.Add("d365org_access_token", d365token);

//    try
//    {
//        using (System.Data.SqlClient.SqlConnection emsqlCon = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
//        {
//            emsqlCon.Open();
//            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
//            cmd.Connection = emsqlCon;

//            cmd.CommandText = "Update userlist set CrmOwinAuth = '" + tokenKey.Substring(7) + "',d365token = '" + d365token + "',powerplatformtoken = '" + powerplatformtoken + "', expiration = '" + DateTime.Now.ToString() + "' where systemuserid = '" + systemuserId + "'";
//            cmd.ExecuteNonQuery();
//        }
//    }
//    catch (Exception ex) { }
//}
//else
//{
//    Context.Add("CrmOwinAuth", crmowinauth);
//    Context.Add("d365org_access_token", d365token);
//    Context.Add("powerplatforms_access_token", powerplatformtoken);
//}
//#endregion auth