namespace CRM_Perf_BenchMark
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Microsoft.VisualStudio.TestTools.WebTesting;
	using System.Net;
	using System.Runtime.Serialization;
	using System.IO;
	using System.Reflection;
	using Microsoft.Xrm.Sdk.Client;
	using Microsoft.VisualStudio.TestTools.WebTesting.Rules;

	[Serializable]
	public abstract class CrmWebTest : WebTest
	{
		public static Version CrmVersionV6Ur1 = new Version(6, 0, 1, 0);
		public static Version CrmVersionV6Ur2 = new Version(6, 0, 2, 0);
		public static Version CrmVersionV6Leo = new Version(6, 1, 0, 0);
		private string token = string.Empty;

		private static List<string> TracedTransactions = new List<string>
		{
			"CategoryNavigation",
			"KnowledgeBaseArticleNavigation",
			"KnowledgeNavigation",
			"SearchKnowledgeBase",
			"SearchKnowledgeBasekbquery"
		};

		public CrmWebTest()
		{
			this.PreAuthenticate = false;
			this.Guid = new Guid();
            this.StopOnError = true;
			this.PreRequest += new EventHandler<PreRequestEventArgs>(CrmWebTest_PreRequest);
			this.PostRequest += new EventHandler<PostRequestEventArgs>(CrmWebTest_PostRequest);
			this.PreWebTest += new EventHandler<PreWebTestEventArgs>(CrmWebTest_PreWebTest);
			this.PostWebTest += new EventHandler<PostWebTestEventArgs>(CrmWebTest_PostWebTest);
			this.PreTransaction += new EventHandler<PreTransactionEventArgs>(CrmWebTest_PreTransaction);
			this.PostTransaction += new EventHandler<PostTransactionEventArgs>(CrmWebTest_PostTransaction);
			this.PrePage += new EventHandler<PrePageEventArgs>(CrmWebTest_PrePage);
			this.PostPage += new EventHandler<PostPageEventArgs>(CrmWebTest_PostPage);
		}

		void CrmWebTest_PrePage(object sender, PrePageEventArgs e)
		{
			this.LogToTraceFile(new Dictionary<string, object> 
				{
					{"Page Started", e.Request.UrlWithQueryString}
				});
		}

		void CrmWebTest_PostPage(object sender, PostPageEventArgs e)
		{
			this.LogToTraceFile(new Dictionary<string, object> 
				{
					{"Page Finished", e.Request.UrlWithQueryString}
				});
		}

		void CrmWebTest_PreTransaction(object sender, PreTransactionEventArgs e)
		{
			this.LogToTraceFile(new Dictionary<string, object> 
				{
					{"Transaction Started", e.TransactionName}
				});
		}

		void CrmWebTest_PostTransaction(object sender, PostTransactionEventArgs e)
		{
			this.LogToTraceFile(new Dictionary<string, object> 
				{
					{"Transaction Finished", e.TransactionName},
					{"Duration", e.Duration},
				});

			if (TracedTransactions.Contains(e.TransactionName))
			{
				LogToTraceFile(new Dictionary<string, object>
				{
					{"\n" + e.TransactionName, string.Format("           {0}           {1} \n",e.WebTest.LastResponse.ResponseUri,e.Duration)}
				});
			}

		}

		private void LogToTraceFile(string log)
		{
			FileWriter.Instance.WriteToFile(
					string.Format("[{0}], TestName: {1}, TestGuid: {2}, {3}",
						DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
						this.ToString(),
						this.Guid.ToString(),
						log));
		}

		protected void LogToTraceFile(Dictionary<string, object> fields)
		{
			this.LogToTraceFile(
				string.Join(", ",
					fields.Select(f =>
						string.Format("{0}: {1}", f.Key, f.Value.ToString()))));
		}

		void CrmWebTest_PreWebTest(object sender, PreWebTestEventArgs e)
		{
			if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) == 0)
			{
				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
			}

			//Clear Authentication Cookies to avoid credential sharing between threads
			this.Context.CookieContainer = new System.Net.CookieContainer(); 
			this.LogToTraceFile("Started");
		}

		void CrmWebTest_PostWebTest(object sender, PostWebTestEventArgs e)
		{
			this.LogToTraceFile("Finished");
		}

		private const string ReferrerReqIdHeader = "ReferrerReqId";
		private const string TrackerIdHeader = "TrackerId";

		void CrmWebTest_PreRequest(object sender, PreRequestEventArgs e)
		{

			if (e.Request.Url.Contains("_layout/tokenhtml")) {
				ExtractHiddenFields er_requestverificationtoken = new ExtractHiddenFields();
				er_requestverificationtoken.Required = true;
				er_requestverificationtoken.HtmlDecode = true;
				er_requestverificationtoken.ContextParameterName = "requestverificationtoken";
				e.Request.ExtractValues += new EventHandler<ExtractionEventArgs>(er_requestverificationtoken.Extract);
			}

			if (e == null || e.Request == null || e.Request.Headers == null)
			{
				return;
			}

			if (e.Request.Url.Contains("dynamics.com") && this.Context.Keys.Contains("CrmOwinAuth"))
			{
				Cookie cval = new Cookie("CrmOwinAuth", this.Context["CrmOwinAuth"].ToString().Replace("CrmOwinAuth=", ""));
				e.Request.Cookies.Add(cval);

				if (e.Request.HasDependentRequests)
				{
					foreach (WebTestRequest wtr in e.Request.DependentRequests)
					{
						if (!wtr.Headers.Contains(HttpRequestHeader.Authorization.ToString()))
						{
							wtr.Cookies.Add(cval);
						}
					}
				}

			}

            ///auth/session
            if ((e.Request.Url.Contains("unitedstates-002.azure-apim") || e.Request.Url.Contains("apps.powerapps.com")) && e.Request.Method != "OPTIONS" && !e.Request.Headers.Contains("HttpRequestHeader.Authorization.ToString()"))
            {
                if (Context.ContainsKey("apihub_access_token"))
                {
                    e.Request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + this.Context["apihub_access_token"].ToString().Replace("Bearer ", ""));
                }
            }
						
			if ((e.Request.Url.Contains("api.powerapps.com") || e.Request.Url.Contains("bap.microsoft.com")) && e.Request.Method != "OPTIONS")
			{
				if (Context.ContainsKey("powerapps_access_token"))
				{
					e.Request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + this.Context["powerapps_access_token"].ToString().Replace("Bearer ", ""));
				}
			}

			if (e.Request.Url.Contains("api.powerplatform.com") && e.Request.Method != "OPTIONS")
			{
				if (Context.ContainsKey("powerplatforms_access_token"))
				{
					e.Request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + this.Context["powerplatforms_access_token"].ToString().Replace("Bearer ", ""));
				}
			}


			if (e.Request.Url.Contains("api.flow.microsoft.com") && e.Request.Method != "OPTIONS")
            {
                if (Context.ContainsKey("flow_access_token"))
                {
                    e.Request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + this.Context["flow_access_token"].ToString().Replace("Bearer ", ""));
                }
            }


            if ((e.Request.Url.Contains("analysis.windows.net") || e.Request.Url.Contains("api.powerbi.com")) && e.Request.Method != "OPTIONS")
            {
                if (Context.ContainsKey("pbi_access_token"))
                {
                    e.Request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + this.Context["pbi_access_token"].ToString().Replace("Bearer ", ""));
                }
            }

            if (e.Request.Url.Contains("pbidedicated.windows.net")&& Context.ContainsKey("pbidedicated_access_token"))
            {				
				if (e.Request.Method != "OPTIONS")
				{
					e.Request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "MWCToken " + this.Context["pbidedicated_access_token"].ToString());
				}
				if (e.Request.HasDependentRequests)
				{
					foreach (WebTestRequest wtr in e.Request.DependentRequests)
					{
						if (wtr.Url.Contains("pbidedicated.windows.net") && wtr.Method != "OPTIONS")
						{
							if (!wtr.Headers.Contains(HttpRequestHeader.Authorization.ToString()))
							{
								wtr.Headers.Add(HttpRequestHeader.Authorization.ToString(), "MWCToken " + this.Context["pbidedicated_access_token"].ToString());

							}
						}
					}
				}
            }

            if (e.Request.Url.Contains("dynamics.com") && e.Request.Method != "OPTIONS" && string.IsNullOrEmpty(this.token))
			{
				if (Context.ContainsKey("d365org_access_token"))
				{
					e.Request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + this.Context["d365org_access_token"].ToString().Replace("Bearer ", ""));
				}
			}
					 	
			if (e.Request.Method == "POST" && e.Request.Body == null)
				{
                		StringHttpBody request4Body = new StringHttpBody();
						request4Body.BodyString = "";
						e.Request.Body = request4Body;
            	}

			if (!string.IsNullOrEmpty(this.token))
			{

                if (!e.Request.Headers.Contains(HttpRequestHeader.Authorization.ToString()) && e.Request.Url.Contains("dynamics.com"))
                {
                    e.Request.Headers.Add(HttpRequestHeader.Authorization.ToString(), this.token);
                }
                if (e.Request.HasDependentRequests) {
                    foreach (WebTestRequest wtr in e.Request.DependentRequests)
                    {
                        if (!wtr.Headers.Contains(HttpRequestHeader.Authorization.ToString()))
                        {
                            wtr.Headers.Add(HttpRequestHeader.Authorization.ToString(), this.token);
                        }
                    }
                }
			}

			var trackerId = Guid.NewGuid().ToString();
			e.Request.Headers.Add(TrackerIdHeader, trackerId);

			this.LogToTraceFile(new Dictionary<string, object> 
				{
					{"Request Started", e.Request.UrlWithQueryString},
					{TrackerIdHeader, trackerId}
				});

			if (!this.Context.ContainsKey(ReferrerReqIdHeader))
			{
				var referrerReqId = e.WebTest.Guid.ToString();
				this.Context[ReferrerReqIdHeader] = referrerReqId;
				e.Request.Headers.Add(ReferrerReqIdHeader, referrerReqId);
			}
		}

		void CrmWebTest_PostRequest(object sender, PostRequestEventArgs e)
		{
			if (e == null || !e.ResponseExists || e.Response == null || e.Response.Headers == null)
			{
				return;
			}

			var logFields = new Dictionary<string, object> 
				{
					{"Request Finished", e.Request.UrlWithQueryString},
					{"StatusCode", e.Response.StatusCode},
					{"MillisecondsToFirstByte", e.Response.Statistics.MillisecondsToFirstByte},
					{"MillisecondsToLastByte", e.Response.Statistics.MillisecondsToLastByte},
					{"ContentLength", e.Response.Statistics.ContentLength}
				};

			var trackerIdHeader = e.Request.Headers.FirstOrDefault(h => h.Name.Equals(TrackerIdHeader));
			if (trackerIdHeader != null)
			{
				logFields.Add(TrackerIdHeader, trackerIdHeader.Value);
			}

			var reqId = e.Response.Headers["REQ_ID"];
			if (!string.IsNullOrEmpty(reqId))
			{
				logFields.Add("REQ_ID", reqId);
				logFields.Add(ReferrerReqIdHeader, this.Context[ReferrerReqIdHeader]);
			}

			var crmRequest = e.Request as CrmRequest;
			if (crmRequest != null)
			{
				if (crmRequest.Name != null)
				{
					logFields.Add("REQ_Name", crmRequest.Name);
				}
			}

			this.LogToTraceFile(logFields);

		}

		public override abstract IEnumerator<WebTestRequest> GetRequestEnumerator();

		public string SetAuthToken(CRMEntity user, PreWebTestEventArgs e)
		{
			var userManager = EntityManager.GetUserManager();
			this.token = userManager.RetrieveAuthToken(e.WebTest, user["domainname"], user["userpassword"], user["organizationname"], user["organizationbaseurl"], "/XrmServices/2011/Organization.svc/web?SdkClientVersion=8.1");
			return this.token;
		}

		public new void BeginTransaction(string trac)
		{			
			base.BeginTransaction(trac);
		}

		public new void EndTransaction(string trac)
		{
			base.EndTransaction(trac);			
		}
	}
}
