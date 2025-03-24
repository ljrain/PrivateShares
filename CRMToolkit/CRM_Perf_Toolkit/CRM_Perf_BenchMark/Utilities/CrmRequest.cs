namespace CRM_Perf_BenchMark
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Microsoft.VisualStudio.TestTools.WebTesting;
	using System.Net;

	[Serializable]
	public class CrmRequest : WebTestRequest
	{
		public string Name { get; set; }
		// We should not call into this after we move to multi server
		public CrmRequest(string url)
			: base(url)
		{            
			this.ParseDependentRequests = false;
			if (ConfigSettings.Default.ParseDependentRequests != null)
			{
				try
				{
					this.ParseDependentRequests = bool.Parse(ConfigSettings.Default.ParseDependentRequests);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine(e.ToString());
				}
			}
			this.ValidateResponse += new EventHandler<ValidationEventArgs>(CrmRequest_ValidateResponse);
		}

		public CrmRequest(string url, CRMEntity user)
			: this(url)
		{
			this.Url = Utils.UrlPathCombine(user["OrganizationBaseUrl"], url);
		}

		public CrmRequest(Uri uri)
			: base(uri)
		{
			// IMPORTANT:  if we come in through this constructor (typically used in the WSDL hijacking (crmrequest.GetResponse())
			// check to see if the uri prefix is crm (again, this is used to enable the WSDL hijacking).  If so, convert it to http
			Url = Url.Replace("crm://", "http://");

			this.ParseDependentRequests = false;
			this.ValidateResponse += new EventHandler<ValidationEventArgs>(CrmRequest_ValidateResponse);
		}

		public List<HttpStatusCode> ValidHttpStatusCodes = new List<HttpStatusCode>();

		public void RequestTypePost()
		{
			formBody = new FormPostHttpBody();
			Method = "POST";
			Body = formBody;
		}

		public QueryStringParameter AddQueryStringParameter(string name, string value)
		{
			QueryStringParameter p = new QueryStringParameter();
			QueryStringParameters.Add(p);
			p.Name = name;
			p.Value = value;
			return p;
		}

		public QueryStringParameter AddQueryStringParameter(string name, string value, bool urlEncode)
		{
			QueryStringParameter p = new QueryStringParameter();
			QueryStringParameters.Add(p);
			p.Name = name;
			p.Value = value;
			p.UrlEncode = urlEncode;
			return p;
		}

		public void AddFormParameter(string name, string value)
		{
			// if we haven't been here yet, allocate the FormPostHttpBody
			// ASSUME:  if we set a form parameter, we must be doing a POST
			if (formBody == null)
			{
				formBody = new FormPostHttpBody();
				Method = "POST";
				Body = formBody;
			}
			formBody.FormPostParameters.Add(new FormPostParameter(name, value));
		}

		public virtual void CrmRequest_ValidateResponse(object sender, ValidationEventArgs e)
		{
			e.IsValid = true;

            if (e.Response.StatusCode != HttpStatusCode.OK & e.Response.StatusCode != HttpStatusCode.NoContent & e.Response.StatusCode != HttpStatusCode.Created & e.Response.StatusCode != HttpStatusCode.NotFound & e.Response.StatusCode != HttpStatusCode.NotModified & e.Response.StatusCode != HttpStatusCode.Redirect)
			{                
				if (!ValidHttpStatusCodes.Contains(e.Response.StatusCode))
				{
                    if (!e.Response.BodyString.Contains("No unit found with supplied "))
                    {
                        e.IsValid = false;
                    }
				}
				lastResponse = e.Response;
				return;
			}

			if (e.Response.Headers.AllKeys.Contains("Set-Cookie") && e.Response.Headers["Set-Cookie"].Contains("CrmOwinAuth"))
			{
				int tokenlocation = e.Response.Headers["Set-Cookie"].IndexOf("CrmOwinAuth=") + 12;
				int tokenlocationend = e.Response.Headers["Set-Cookie"].IndexOf(";", tokenlocation);
				string CrmOwinAuth = e.Response.Headers["Set-Cookie"].Substring(tokenlocation, tokenlocationend - tokenlocation);
				e.WebTest.Context.Add("CrmOwinAuth", CrmOwinAuth);

				try{
					using (System.Data.SqlClient.SqlConnection emsqlCon = new System.Data.SqlClient.SqlConnection(ConfigSettings.Default.EMSQLCNN))
					{
						emsqlCon.Open();
						System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
						cmd.Connection = emsqlCon;
						cmd.CommandText = "Update userlist set crmowinauth = '" + CrmOwinAuth + "' where username = '" + e.WebTest.UserName + "'";
						cmd.ExecuteNonQuery();
					}
				}
				catch(Exception error){}
			}

			if (e.Response.BodyString == null)
			{
				lastResponse = e.Response;
				return;
			}

			if (e.Response.BodyString.Contains("<title>Microsoft CRM Application Error Report</title>"))
			{
				e.IsValid = false;
				lastResponse = e.Response;
				return;
			}

            if (e.Response.BodyString.Contains("INVALID_WRPC_TOKEN"))
			{
				e.IsValid = false;
				lastResponse = e.Response;
				return;
			}
			if (e.Response.BodyString.Contains("<title>Microsoft CRM Platform Error Report</title>"))
			{
				e.IsValid = false;
				lastResponse = e.Response;
				return;
			}

            if (e.Response.Statistics.MillisecondsToLastByte > 120000)
            {
                e.IsValid = false;
                lastResponse = e.Response;
                e.Message = "Request Exceeded 120 Second Threshold";
                return;
            } 

			if (e.Response.ResponseUri.AbsolutePath.Contains("errorhandler.aspx"))
			{
				e.IsValid = false;
				lastResponse = e.Response;
                
				return;
			}

			lastResponse = e.Response;
		}

		private string clientAppName;
		public string ClientAppName 
		{
			get 
			{
				return clientAppName;
			}

			set 
			{
				this.clientAppName = value;

				// Add client trackers, will be logged into IIS logs
				if (!String.IsNullOrEmpty(this.clientAppName))
				{
					this.Cookies.Add(new Cookie("ReqClientId", this.clientAppName));
					this.Headers.Add("ClientAppName", this.clientAppName);
				}
			}
		}

		public WebTestResponse lastResponse = null;
		protected FormPostHttpBody formBody = null;
	}
}
