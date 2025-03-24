namespace CRM_Perf_BenchMark
{
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Dynamic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Web.Services.Protocols;
    using System.Xml;

    internal class Rexp
	{
		// NOTE - the below are simplified version of rfc 2046 specification
		// X: Y
		// Y = z(;|\n)z(;|\n)z
		// z = text | a = b
		public static string reText = @"[\w0-9./_<>=-]+";
		public static string reSep = @"(;|\r\n)+";
		public static string reLHS = @"(\w+\s*)";
		public static string reRHS = @"(""" + reText + @""")";
		public static string reStmt = reLHS + "=" + reRHS;
		public static string reZ = "(" + reText + "|" + reStmt + ")+";
		public static string reY = "(" + reZ + @"\s*" + reSep + @"\s*" + ")+";
		public static string reXY = reText + @"\s*:\s*" + reY; // complex, consumes lots of time :(
		public static string reSimpleXY = reText + @"\s*:\s*.+" + reSep;
	}

	public sealed class ExecuteMultipleTestSettings
	{
		private int m_maxThreads = 1;
		public int MaxThreads
		{
			get { return m_maxThreads; }
			internal set { m_maxThreads = value; }
		}
		private int m_batchSize = 10;
		public int BatchSize
		{
			get { return m_batchSize; }
			internal set { m_batchSize = value; }
		}
		private int m_totalEntities = 10;
		public int TotalEntities
		{
			get { return m_totalEntities; }
			internal set { m_totalEntities = value; }
		}
	}

	public class WRPCInfo
	{
		public string token;
		public long timeStamp;
	}

	[Serializable]
	public class CrmDelRequest : CrmRequest
	{
		public CrmDelRequest(string url)
			: base(url)
		{
		}

		public CrmDelRequest(string url, CRMEntity user)
			: base(url, user)
		{
		}

		public CrmDelRequest(Uri uri)
			: base(uri)
		{
		}

		public override void CrmRequest_ValidateResponse(object sender, ValidationEventArgs e)
		{
			e.IsValid = true;

			if (e.Response.StatusCode != HttpStatusCode.OK)
			{
				e.IsValid = false;
				lastResponse = e.Response;
				return;
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

			if (e.Response.BodyString.Contains("<title>Microsoft CRM Platform Error Report</title>"))
			{
				e.IsValid = false;
				lastResponse = e.Response;
				return;
			}

			// redirects in CRM indicate an error.  "Found" is the code you get after being redirected.
			if (e.Response.StatusCode == HttpStatusCode.Found)
			{
				e.IsValid = false;
				lastResponse = e.Response;
				return;
			}

			if (e.Response.ResponseUri.AbsolutePath.Contains("errorhandler.aspx"))
			{
				e.IsValid = false;
				lastResponse = e.Response;
				return;
			}

			if (e.Response.BodyString.Contains("<details>Exception from HRESULT:"))
			{
				e.IsValid = false;
				lastResponse = e.Response;
				return;
			}

			lastResponse = e.Response;
			return;
		}
	}

	public class Utils
	{
        private const int minYearNum = 2014;
        private const int maxYearNum = 2015;

		/// <summary>
		/// Combine UrlPath
		/// </summary>
		/// <param name="path1"></param>
		/// <param name="path2"></param>
		/// <returns></returns>
		public static string UrlPathCombine(string baseUrl, string relativeUrl)
		{
			baseUrl = baseUrl.EndsWith("/") ? baseUrl : string.Format("{0}/", baseUrl);
			relativeUrl = relativeUrl.StartsWith("/") ? relativeUrl.TrimStart('/') : relativeUrl;
			var uri = new Uri(new Uri(baseUrl), relativeUrl);
			return uri.ToString();
		}
		

        //ExtractBase64 for Portal Views
        public static List<string> extractbase64(string resresult6)
        {
            //Identify the characters that identify a row in the grid
            string recordMarker1 = "Base64SecureConfiguration&quot;:&quot;";
            string recordMarker2 = "Columns&quot;";
            int recordstofind = 30;
            string recordId = null;

            //create array to keep track of each occurence;
            // int[] recordoccurences = new int[recordstofind];
            List<string> recordoccurences = new List<string>();

            //verify the result contains records
            if (resresult6.Contains(recordMarker1))
            {
                //find first record. 
                int idIdx6 = resresult6.IndexOf(recordMarker1) + recordMarker1.Length;
                int idIdx6b = resresult6.IndexOf(recordMarker2, idIdx6);
                recordoccurences.Add(resresult6.Substring(idIdx6, idIdx6b - idIdx6));

                //Loop thru result and fine the first 10 records. 
                for (int i = 1; i < recordstofind; i++)
                {
                    //location of next occurence. offset location to skip last location
                    idIdx6 = resresult6.IndexOf(recordMarker1, idIdx6b);
                    if (idIdx6 != -1)
                    {
                        idIdx6 += recordMarker1.Length;
                        idIdx6b = resresult6.IndexOf(recordMarker2, idIdx6);
                        recordoccurences.Add(resresult6.Substring(idIdx6, idIdx6b - idIdx6));
                    }
                    else { break; }
                }
            }
            return recordoccurences;
        }

		//ExtractBase64 for Portal Views Decoded

		//ExtractText exviewlayouts = new ExtractText();
		//exviewlayouts.StartsWith = "-view-layouts=\"";
		//    exviewlayouts.EndsWith = "\"";
		//    exviewlayouts.IgnoreCase = false;
		//    exviewlayouts.UseRegularExpression = false;
		//    exviewlayouts.Required = true;
		//    exviewlayouts.ExtractRandomMatch = false;
		//    exviewlayouts.Index = 0;
		//    exviewlayouts.HtmlDecode = true;
		//    exviewlayouts.SearchInHeaders = false;
		//    exviewlayouts.ContextParameterName = "viewlayouts";
		//    request1.ExtractValues += new EventHandler<ExtractionEventArgs>(exviewlayouts.Extract);

		//    yield return request1;            
		//    request1 = null;
		//    string viewlayouts = Encoding.UTF8.GetString(Convert.FromBase64String(this.Context["viewlayouts"].ToString()));
		//List<string> request6base64 = Utils.extractbase64dec(viewlayouts);


		//        string r8 = "";
		//            foreach (string b64 in request6base64)
		//            {
		//                if (b64.Contains("Portal - Active Details"))
		//                {
		//                    int a = b64.IndexOf("\"");
		//        r8 = b64.Substring(0, a);
		//                    break;
		//                }
		//}
		//request3Body.BodyString = "{\"base64SecureConfiguration\":\""+r8+"\",\"sortExpression\":\"new_date DESC\",\"search\":\"\",\"page\":1,\"pageSize\":20,\"filter\":null,\"metaFilter\":null,\"timezoneOffset\":360,\"customParameters\":[]}";



		public static List<string> extractbase64dec(string resresult6)
		{
			//Identify the characters that identify a row in the grid
			string recordMarker1 = "Base64SecureConfiguration\":\"";
			string recordMarker2 = "Columns\"";
			int recordstofind = 30;
			string recordId = null;

			//create array to keep track of each occurence;
			List<string> recordoccurences = new List<string>();

			//verify the result contains records
			if (resresult6.Contains(recordMarker1))
			{
				//find first record. 
				int idIdx6 = resresult6.IndexOf(recordMarker1) + recordMarker1.Length;
				int idIdx6b = resresult6.IndexOf(recordMarker2, idIdx6);
				recordoccurences.Add(resresult6.Substring(idIdx6, idIdx6b - idIdx6));

				//Loop thru result and fine the first 10 records. 
				for (int i = 1; i < recordstofind; i++)
				{
					//location of next occurence. offset location to skip last location
					idIdx6 = resresult6.IndexOf(recordMarker1, idIdx6b);
					if (idIdx6 != -1)
					{
						idIdx6 += recordMarker1.Length;
						idIdx6b = resresult6.IndexOf(recordMarker2, idIdx6);
						recordoccurences.Add(resresult6.Substring(idIdx6, idIdx6b - idIdx6));
					}
					else { break; }
				}
			}
			return recordoccurences;
		}

		public static string extractviewfromlayoutbase64(string viewname, string layoutbase64)
		{
			string decodedlayoutbase64 = Encoding.UTF8.GetString(Convert.FromBase64String(layoutbase64));

			string base64secureconnection = "";
			foreach (string b64 in Utils.extractbase64dec(decodedlayoutbase64))
			{
				if (b64.Contains(viewname))
				{
					int a = b64.IndexOf("\"");
					base64secureconnection = b64.Substring(0, a);
					break;
				}
			}
			return base64secureconnection;
		}

		//Extract Form Fields
		public static string ExtractFormField(string fieldname, string bodystring)
        {
            string recordId = null;
            string valuemarker = fieldname + "\":{";
            string valuemarker2 = "oid\":\"{";
            if (bodystring.Contains(valuemarker))
            {
                int idIdx = bodystring.IndexOf(valuemarker);
                int idIdx2 = bodystring.IndexOf(valuemarker2, idIdx);
                recordId = bodystring.Substring(idIdx2 + (valuemarker2.Length), 36);
            }
            return recordId;
        }

      
        //Extract WRPC Token from upload.aspx for adding attachment to notes.
        public static string[] ExtractUploadWRPCToken(WebTestResponse response)
        {
            string[] TokenValues = new string[2];
            string tokenindex = "=\"CRMWRPCToken\"  value=\"";
            string tokentimestampindex = "CRMWRPCTokenTimeStamp\"  value=\"";

            int start = response.BodyString.IndexOf("/Attachment/upload.aspx\"");
            int Tokenstart = response.BodyString.IndexOf(tokenindex, start) + tokenindex.Length;
            int Tokenstop = response.BodyString.IndexOf("\"", Tokenstart);
            int Timestampstart = response.BodyString.IndexOf(tokentimestampindex, start) + tokentimestampindex.Length;

            TokenValues[0] = response.BodyString.Substring(Tokenstart, Tokenstop - Tokenstart).Replace("\\x2f", "/").Replace("\\x2b", "+").Replace("\\/", "/");
            TokenValues[1] = Int64.Parse(response.BodyString.Substring(Timestampstart, 18)).ToString();
            return TokenValues;
        }
        

        //Extract WRPC Token from upload.aspx for adding attachment to notes.
        public static string[] ExtractDialogToken(WebTestResponse response)
        {
            string[] TokenValues = new string[2];

            //int start = response.BodyString.IndexOf("INTERACTIVEWORKFLOWWEBSERVICE.ASMX") - 110;
            //int Tokenstart = response.BodyString.IndexOf("Token\":\"", start) + 8;
            //int Tokenstop = response.BodyString.IndexOf("\"", Tokenstart);
            //int Timestampstart = response.BodyString.IndexOf("Timestamp\":\"", start) + 12;

            int start = response.BodyString.IndexOf("INTERACTIVEWORKFLOWWEBSERVICE.ASMX");
            int Tokenstart = response.BodyString.IndexOf("Token: '", start) + 8;
            int Tokenstop = response.BodyString.IndexOf("'", Tokenstart);
            int Timestampstart = response.BodyString.IndexOf("Timestamp: \"", start) + 12;

            TokenValues[0] = response.BodyString.Substring(Tokenstart, Tokenstop - Tokenstart).Replace("\\x2f", "/").Replace("\\x2b", "+").Replace("\\/", "/");
            TokenValues[1] = Int64.Parse(response.BodyString.Substring(Timestampstart, 18)).ToString();
            return TokenValues;
        }

        //otype="10422" oid="{
        public static string extractlookupresponse(string etc, string resresult)
        {
            //Identify the characters that identify a row in the grid
            string recordMarker = "otype=\""+etc+"\" oid=\"{";
            int recordstofind = 10;
            string recordId = null;

            //create array to keep track of each occurence;
            // int[] recordoccurences = new int[recordstofind];
            List<int> recordoccurences = new List<int>();

            //verify the result contains records
            if (resresult.Contains(recordMarker))
            {

                //find first record. 
                int idIdx = resresult.IndexOf(recordMarker);
                recordoccurences.Add(idIdx);

                //Loop thru result and fine the first 10 records. 
                for (int i = 1; i < recordstofind; i++)
                {
                    //location of next occurence. offset location to skip last location
                    idIdx = resresult.IndexOf(recordMarker, idIdx + 2);
                    if (idIdx != -1)
                    {
                        recordoccurences.Add(idIdx);
                    }
                    else { break; }
                }

                recordId = resresult.Substring(recordoccurences[Utils.GetRandomInt(0, recordoccurences.Count)] + (recordMarker.Length), 36);
            }
            return recordId;
        }




        //string contactId = Utils.extractUCIgridresponse("contactid\":\"",resresult);

        public static string extractUCIgridresponse(string recordidentifier, string resresult)
        {
            //Identify the characters that identify a row in the grid
            string recordMarker = recordidentifier;
            int recordstofind = 30;
            string recordId = null;

            //create array to keep track of each occurence;
            // int[] recordoccurences = new int[recordstofind];
            List<int> recordoccurences = new List<int>();

            //verify the result contains records
            if (resresult.Contains(recordMarker))
            {

                //find first record. 
                int idIdx = resresult.IndexOf(recordMarker);
                recordoccurences.Add(idIdx);

                //Loop thru result and fine the first 10 records. 
                for (int i = 1; i < recordstofind; i++)
                {
                    //location of next occurence. offset location to skip last location
                    idIdx = resresult.IndexOf(recordMarker, idIdx + 2);
                    if (idIdx != -1)
                    {
                        recordoccurences.Add(idIdx);
                    }
                    else { break; }
                }

                recordId = resresult.Substring(recordoccurences[Utils.GetRandomInt(0, recordoccurences.Count)] + (recordMarker.Length), 36);
            }
            return recordId;
        }


		//Extract a records and return the entire list of results
		public static List<string> extractUCIgridresponselist(string recordidentifier, string resresult)
		{
			//Identify the characters that identify a row in the grid
			string recordMarker = recordidentifier;
			int recordstofind = 30;
			string recordId = null;

			//create array to keep track of each occurence;
			// int[] recordoccurences = new int[recordstofind];
			List<string> recordoccurences = new List<string>();

			//verify the result contains records
			if (resresult.Contains(recordMarker))
			{
				//find first record. 
				int idIdx = resresult.IndexOf(recordMarker);
				recordoccurences.Add(resresult.Substring(idIdx + (recordMarker.Length), 36));

				//Loop thru result and fine the first 10 records. 
				for (int i = 1; i < recordstofind; i++)
				{
					//location of next occurence. offset location to skip last location
					idIdx = resresult.IndexOf(recordMarker, idIdx + 2);
					if (idIdx != -1)
					{
						recordoccurences.Add(resresult.Substring(idIdx + (recordMarker.Length), 36));
					}
					else { break; }
				}
								
			}
			return recordoccurences;
		}

        public static string extractgridresponse(string resresult)
        {
            //Identify the characters that identify a row in the grid
            string recordMarker = "ms-crm-List-Row\" oid=\"{";
            int recordstofind = 30;
            string recordId = null;

            //create array to keep track of each occurence;
            // int[] recordoccurences = new int[recordstofind];
            List<int> recordoccurences = new List<int>();

            //verify the result contains records
            if (resresult.Contains(recordMarker))
            {

                //find first record. 
                int idIdx = resresult.IndexOf(recordMarker);
                recordoccurences.Add(idIdx);

                //Loop thru result and fine the first 10 records. 
                for (int i = 1; i < recordstofind; i++)
                {
                    //location of next occurence. offset location to skip last location
                    idIdx = resresult.IndexOf(recordMarker, idIdx + 2);
                    if (idIdx != -1)
                    {
                        recordoccurences.Add(idIdx);
                    }
                    else { break; }
                }

                recordId = resresult.Substring(recordoccurences[Utils.GetRandomInt(0, recordoccurences.Count)] + (recordMarker.Length), 36);               
            }
            return recordId;
        }

		//JM Added for Extracting Tokens
		public static string[] ExtractWRPCToken(WebTestResponse response)
		{
			string[] TokenValues = new string[2];

			int start = response.BodyString.IndexOf("INLINEEDITWEBSERVICE.ASMX") - 140;
			int Tokenstart = response.BodyString.IndexOf("Token\":\"", start) + 8;
			int Tokenstop = response.BodyString.IndexOf("\"", Tokenstart);
			int Timestampstart = response.BodyString.IndexOf("Timestamp\":\"", start) + 12;

			//int start = response.BodyString.IndexOf("INLINEEDITWEBSERVICE.ASMX");
			//int Tokenstart = response.BodyString.IndexOf("Token: '", start) + 8;
			//int Tokenstop = response.BodyString.IndexOf("'", Tokenstart);
			//int Timestampstart = response.BodyString.IndexOf("Timestamp: \"", start) + 12;

			TokenValues[0] = response.BodyString.Substring(Tokenstart, Tokenstop - Tokenstart).Replace("\\x2f", "/").Replace("\\x2b", "+").Replace("\\/", "/");
			TokenValues[1] = Int64.Parse(response.BodyString.Substring(Timestampstart, 18)).ToString();

			return TokenValues;
		}


		//custom, testing
		public static DateTime GetRandomDateTime(int minYear = minYearNum, int maxYear = maxYearNum)
        {
            return new DateTime(GetRandomInt(minYear, maxYear), GetRandomInt(1, 12), GetRandomInt(1, 28), GetRandomInt(7, 20), 0, 0);
        }

		//this functions replace the server name with the server name from crm server
		//we are assuming url is like http://perf1web:5555/test.asmx and crmserver is like http://perf5web
		public static string ReplaceServerName(string url, CRMEntity user)
		{
			var uri = new Uri(url);
			url = Utils.UrlPathCombine(user["OrganizationBaseUrl"], uri.AbsolutePath);
			return url;
		}
			
		public static string GetRandomString(int minLen, int maxLen)
		{

			int alphabetLength = Alphabet.Length;
			int stringLength;
			lock (m_randLock) { stringLength = m_randomInstance.Next(minLen, maxLen); }
			char[] str = new char[stringLength];

			// max length of the randomizer array is 5
			int randomizerLength = (stringLength > 5) ? 5 : stringLength;

			int[] rndInts = new int[randomizerLength];
			int[] rndIncrements = new int[randomizerLength];

			// Prepare a "randomizing" array
			for (int i = 0; i < randomizerLength; i++)
			{
				int rnd = m_randomInstance.Next(alphabetLength);
				rndInts[i] = rnd;
				rndIncrements[i] = rnd;
			}

			// Generate "random" string out of the alphabet used
			for (int i = 0; i < stringLength; i++)
			{
				int indexRnd = i % randomizerLength;
				int indexAlphabet = rndInts[indexRnd] % alphabetLength;
				str[i] = Alphabet[indexAlphabet];

				// Each rndInt "cycles" characters from the array, 
				// so we have more or less random string as a result
				rndInts[indexRnd] += rndIncrements[indexRnd];
			}
			return (new string(str));
		}

		public static double GetRandomDouble()
		{
			return (m_randomInstance.NextDouble());
		}

		public static string GetRandomNumber(int min, int max)
		{
			return (m_randomInstance.Next(min, max)).ToString();
		}

		public static int GetRandomInt()
		{
			return (m_randomInstance.Next());
		}

		public static int GetRandomInt(int min, int max)
		{
			return (m_randomInstance.Next(min, max));
		}

		public static string GetRandomPhoneNumber()
		{
			int area_code, first3, last4;

			lock (m_randLock)
			{
				area_code = 100 + m_randomInstance.Next(899);
				first3 = 100 + m_randomInstance.Next(899);
				last4 = m_randomInstance.Next(9999);
			}

			return string.Format("({0:D3}) {1:D3}-{2:D4}", area_code, first3, last4);
		}

		public static void GenerateTextFile(int sizeInMB, string filePath)
		{
			var sizeInBytes = sizeInMB * 1024 * 1024;
			var content = "This is a line of text.";
			var contentBytes = Encoding.UTF8.GetBytes(content);
			var contentSize = contentBytes.Length;
			using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				for (int i = 0; i < sizeInBytes; i += contentSize)
				{
					fileStream.Write(contentBytes, 0, contentSize);
					content = "This is another line of text.";
					contentBytes = Encoding.UTF8.GetBytes(content);
					contentSize = contentBytes.Length;
				}
			}
		}

		public static XmlReader GetXmlReaderForXmlFile(string xmlFilePath)
		{
			var settings = new XmlReaderSettings();
			settings.XmlResolver = null;
			settings.DtdProcessing = DtdProcessing.Prohibit;
			return XmlReader.Create(xmlFilePath, settings);
		}


		private static char[] Alphabet = ("ABCDEFGHIJKLMNOPQRSTUVWXYZabcefghijklmnopqrstuvwxyz0123456789").ToCharArray();
		private static Random m_randomInstance = new Random();
		private static Object m_randLock = new object();
	}

	[Serializable]
	public class crmRequestFactory : System.Net.IWebRequestCreate
	{
		public WebRequest Create(Uri uri)
		{
			return (new crmrequest(uri));
		}

		public crmRequestFactory()
		{

		}
	}

	[Serializable]
	public class crmrequest : WebRequest
	{
		public crmrequest(Uri uri)
		{
			m_uri = uri;
			m_cocStream = new copyoncloseStream();
			m_whc = new WebHeaderCollection();
		}
				
		private Uri m_uri;
		private WebHeaderCollection m_whc;			
		private copyoncloseStream m_cocStream;				
	}

	[Serializable]
	public class copyoncloseStream : MemoryStream
	{	
		public byte[] ba;
		public long length;
	}
}