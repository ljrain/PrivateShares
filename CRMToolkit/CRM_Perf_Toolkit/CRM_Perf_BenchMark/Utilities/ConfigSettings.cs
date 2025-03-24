using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.Xml;
using Microsoft.Xrm.Sdk.Client;
using System.IO;

namespace CRM_Perf_BenchMark
{
	public partial class ConfigSettings
	{
		private static ConfigSettings m_instance = null;

		private string m_WebServiceNS;
		private string m_EMSQLCNN;
		private string m_alltasks;
		private string m_BulkImportDir;
		private string m_ReportServer;
		private string m_BandWidthCapture;
		private string m_NetCapDir;
		private string m_BandWidthOutputDir;
		private string m_ParseDependentRequests;
		private string m_RunAsUser;
		private string m_RunAsPassword;
		private string m_CRMDomain;
		//this is the server we are going to use for metabase
		private string m_MetabaseServer;
		//we will use this property to see if we need to reload the entitymanager database
		private string m_ReloadEntityManager = "false";

		private string m_authenticationType = "activedirectory";
		private string m_userManagerClass;

		//these are to support multi server scenarios 
		public int m_numServers;
		public MultipleServersData[] m_MulServers = null;

		//this is where we are going to store all the Sync outlook xml files
		public string m_OutlookSyncDir = string.Empty;

		//this stores the option to wait for async operation completion
		private bool m_WaitForAsyncCompletion = true;

		// this stores the option whether to deactivate the rule at the end of the Unittest of to keep it.
		private bool m_deactivateRules = true;

		//Flag to enable tracing
		private bool m_trace = false;

		private bool m_isMsodsAuth = false;

		private string m_exchangePassword;

		private bool m_turboFormsEnabled = false;
		private int m_outlookUserPercentage = 100;

		private ExecuteMultipleTestSettings m_executeMultipleTestSettings = new ExecuteMultipleTestSettings();

		public ExecuteMultipleTestSettings ExecuteMultipleTestSettings
		{
			get { return m_executeMultipleTestSettings; }
		}

		public bool Trace
		{
			get
			{
				return m_trace;
			}
		}

		public static ConfigSettings Default
		{
			get
			{
				return m_instance;
			}
		}



		public string ReportServer
		{
			get
			{
				return m_ReportServer;
			}
		}

		public string ReloadEntityManager
		{
			get
			{
				return m_ReloadEntityManager;
			}
		}

		public string WebServiceNS
		{
			get
			{
				return m_WebServiceNS;
			}
		}


		public string EMSQLCNN
		{
			get
			{
				return m_EMSQLCNN;
			}
		}

		public bool alltasks
		{
			get
			{
				return bool.Parse(m_alltasks);
			}
		}

		public string BulkImportDir
		{
			get
			{
				return m_BulkImportDir;
			}
		}

		public string BandWidthCapture
		{
			get
			{
				return m_BandWidthCapture;
			}
		}

		public string NetCapDir
		{
			get
			{
				return m_NetCapDir;
			}
		}

		public string BandWidthOutputDir
		{
			get
			{
				return m_BandWidthOutputDir;
			}
		}

		public string ParseDependentRequests
		{
			get
			{
				return m_ParseDependentRequests;
			}
		}

		public string OutlookSyncDir
		{
			get
			{
				return m_OutlookSyncDir;
			}
		}

		public int OutlookUserPercentage
		{
			get
			{
				return m_outlookUserPercentage;
			}
		}

		public bool WaitForAsyncCompletion
		{
			get
			{
				return m_WaitForAsyncCompletion;
			}
		}

		public string CRMDomain
		{
			get
			{
				return m_CRMDomain;
			}
		}

		public string RunAsUser
		{
			get
			{
				return m_RunAsUser;
			}
		}

		public string RunAsPassword
		{
			get
			{
				return m_RunAsPassword;
			}
		}


		public bool DeactivateRules
		{
			get
			{
				return m_deactivateRules;
			}
		}

		public bool MsodsAuth
		{
			get
			{
				return m_isMsodsAuth;
			}
		}

		public string ExchangePassword
		{
			get
			{
				return m_exchangePassword;
			}
		}

		private string m_toolkitRoot;
		public string CRMToolkitRoot
		{
			get
			{
				return m_toolkitRoot;
			}
		}

		public AuthenticationProviderType AuthenticationType
		{
			get
			{
				var authenticationType = AuthenticationProviderType.ActiveDirectory;
				if (!Enum.TryParse(m_authenticationType, true, out authenticationType))
				{
					throw new System.ArgumentException("Authentation type - {0}, is not valid ", m_authenticationType);
				}
				return authenticationType;
			}
		}

		public string UserManagerClass
		{
			get
			{
				return m_userManagerClass;
			}
		}

		public bool TurboFormsEnabled
		{
			get
			{
				return m_turboFormsEnabled;
			}
		}

		static ConfigSettings()
		{
			m_instance = new ConfigSettings();
		}

		public ConfigSettings()
		{
			var currentDir = Directory.GetCurrentDirectory();

          if (currentDir.Contains("QTAgent"))
            {
                currentDir = "C:\\CRMToolkitNew\\CRM_Perf_Toolkit";
            }

            var dir = new DirectoryInfo(currentDir);
			while (dir != null)
			{
				if (Directory.Exists(Path.Combine(dir.FullName, "CRM_Perf_Toolkit")))
					break;
				dir = dir.Parent;
			}
			if (dir == null)
			{
				throw new InvalidOperationException(string.Format("Unable to find CRM_Perf_Toolkit along the parent path from {0}", Directory.GetCurrentDirectory()));
			}
			m_toolkitRoot = dir.FullName;
			XmlDocument xmlDoc = new XmlDocument();
			try
			{
				var filePath = string.Format(@"{0}\ConfigFiles\ConfigSettings.xml", m_toolkitRoot);
				var reader = Utils.GetXmlReaderForXmlFile(filePath);
				xmlDoc.Load(reader);

				XmlNode configsettings = xmlDoc.DocumentElement;
				XmlNode currentNode = configsettings.SelectSingleNode("webservicens");
				m_WebServiceNS = currentNode.Attributes["value"].Value;

				currentNode = configsettings.SelectSingleNode("emsqlcnn");
				m_EMSQLCNN = currentNode.Attributes["value"].Value;

				currentNode = configsettings.SelectSingleNode("reportserver");
				m_ReportServer = currentNode.Attributes["value"].Value;

				if (configsettings.SelectSingleNode("parsedependentrequests") != null)
				{
					currentNode = configsettings.SelectSingleNode("parsedependentrequests");
					m_ParseDependentRequests = currentNode.Attributes["value"].Value;
				}

				if (configsettings.SelectSingleNode("outlooksyncdir") != null)
				{
					currentNode = configsettings.SelectSingleNode("outlooksyncdir");
					m_OutlookSyncDir = currentNode.Attributes["value"].Value;
				}

				if (configsettings.SelectSingleNode("bandwidthcapture") != null)
				{
					XmlNode bandwidthCapture = configsettings.SelectSingleNode("bandwidthcapture");
					m_BandWidthCapture = bandwidthCapture.Attributes["value"].Value;

					if (bandwidthCapture.SelectSingleNode("netcapdir") != null)
					{
						XmlNode iterator = bandwidthCapture.SelectSingleNode("netcapdir");
						m_NetCapDir = iterator.Attributes["value"].Value;
					}

					if (bandwidthCapture.SelectSingleNode("bandwidthoutputdir") != null)
					{
						XmlNode iterator = bandwidthCapture.SelectSingleNode("bandwidthoutputdir");
						m_BandWidthOutputDir = iterator.Attributes["value"].Value;
					}

				}


				if (configsettings.SelectSingleNode("runas") != null)
				{
					currentNode = configsettings.SelectSingleNode("runas");
					if (currentNode.Attributes["userid"] != null)
						m_RunAsUser = currentNode.Attributes["userid"].Value;
					else
						System.Diagnostics.Trace.WriteLine("UserName missing of the run as user");

					if (currentNode.Attributes["password"] != null)
						m_RunAsPassword = currentNode.Attributes["password"].Value;
					else
						System.Diagnostics.Trace.WriteLine("Password of the run as user missing");
				}

				if (configsettings.SelectSingleNode("crmdomain") != null)
				{
					currentNode = configsettings.SelectSingleNode("crmdomain");
					m_CRMDomain = currentNode.Attributes["value"].Value;
				}
				if (configsettings.SelectSingleNode("metabaseserver") != null)
				{
					currentNode = configsettings.SelectSingleNode("metabaseserver");
					m_MetabaseServer = currentNode.Attributes["value"].Value;
				}

				if (configsettings.SelectSingleNode("reloadentitymanager") != null)
				{
					currentNode = configsettings.SelectSingleNode("reloadentitymanager");
					m_ReloadEntityManager = currentNode.Attributes["value"].Value;
				}

				if (configsettings.SelectSingleNode("trace") != null)
				{
					currentNode = configsettings.SelectSingleNode("trace");
					m_trace = bool.Parse(currentNode.Attributes["value"].Value);
				}

				if (configsettings.SelectSingleNode("authentication") != null)
				{
					currentNode = configsettings.SelectSingleNode("authentication");
					m_authenticationType = currentNode.Attributes["type"].Value;
				}

				if (configsettings.SelectSingleNode("crmonlinedllpath") != null)
				{
					currentNode = configsettings.SelectSingleNode("crmonlinedllpath");
					m_userManagerClass = currentNode.Attributes["value"].Value;
				}

				if (configsettings.SelectSingleNode("datamigration") != null)
				{
					currentNode = configsettings.SelectSingleNode("datamigration");
					XmlNode waitForAsyncCompletion = currentNode.SelectSingleNode("waitforasynccompletion");
					if (waitForAsyncCompletion != null)
					{
						m_WaitForAsyncCompletion = bool.Parse(waitForAsyncCompletion.Attributes["value"].Value);
					}

					XmlNode deactivateRules = currentNode.SelectSingleNode("deactivaterules");
					if (deactivateRules != null)
					{
						m_deactivateRules = bool.Parse(deactivateRules.Attributes["value"].Value);
					}
				}

				if (configsettings.SelectSingleNode("msods") != null)
				{
					currentNode = configsettings.SelectSingleNode("msods");
					m_isMsodsAuth = bool.Parse(currentNode.Attributes["value"].Value);
				}
			
				currentNode = configsettings.SelectSingleNode("crmservers");
				XmlNodeList crmServerNodes = currentNode.SelectNodes("crmserver");

				m_MulServers = new MultipleServersData[crmServerNodes.Count];

				for (int iter = 0; iter < crmServerNodes.Count; iter++)
				{
					m_MulServers[iter] = new MultipleServersData();
					XmlNode crmServer = crmServerNodes[iter];


					//currentNode = crmServer.SelectSingleNode("sqlcnn");
					//m_MulServers[iter].SQLCNN = currentNode.Attributes["value"].Value;

					//currentNode = crmServer.SelectSingleNode("configsqlcnn");
					//m_MulServers[iter].ConfigSqlCNN = currentNode.Attributes["value"].Value;

					currentNode = crmServer.SelectSingleNode("serverbaseurl");
					m_MulServers[iter].ServerBaseUrl = currentNode.Attributes["value"].Value;

					currentNode = crmServer.SelectSingleNode("organizationbaseurl");
					m_MulServers[iter].OrganizationBaseUrl = currentNode.Attributes["value"].Value;

					currentNode = crmServer.SelectSingleNode("organizationserviceurl");
					m_MulServers[iter].OrganizationServiceUrl = currentNode.Attributes["value"].Value;

					currentNode = crmServer.SelectSingleNode("discoveryserver");
					if (currentNode != null)
					{
						m_MulServers[iter].DiscoveryServer = currentNode.Attributes["value"].Value;
					}
					else
					{
						m_MulServers[iter].DiscoveryServer = m_MulServers[iter].ServerBaseUrl;
					}

					currentNode = crmServer.SelectSingleNode("organization");
					m_MulServers[iter].OrgName = currentNode.Attributes["name"].Value;

					XmlNode AD = crmServer.SelectSingleNode("AD");
					if (AD != null)
					{
						currentNode = AD.SelectSingleNode("userpassword");
						var currentNodeValue = currentNode.Attributes["value"].Value;
						if (!string.IsNullOrWhiteSpace(currentNodeValue))
						{
							m_MulServers[iter].userPassword = currentNodeValue;
						}
						
						currentNode = AD.SelectSingleNode("userbase");
						m_MulServers[iter].userBase = currentNode.Attributes["value"].Value;

						currentNode = AD.SelectSingleNode("start");
						m_MulServers[iter].userStart = currentNode.Attributes["value"].Value;

						currentNode = AD.SelectSingleNode("count");
						m_MulServers[iter].userCount = currentNode.Attributes["value"].Value;
					}

					XmlNode passport = crmServer.SelectSingleNode("passport");
					if (passport != null)
					{
						currentNode = passport.SelectSingleNode("passportdomain");
						m_MulServers[iter].userDomain = currentNode.Attributes["value"].Value;

						currentNode = passport.SelectSingleNode("userbase");
						m_MulServers[iter].userBase = currentNode.Attributes["value"].Value;

						currentNode = passport.SelectSingleNode("start");
						m_MulServers[iter].userStart = currentNode.Attributes["value"].Value;

						currentNode = passport.SelectSingleNode("count");
						m_MulServers[iter].userCount = currentNode.Attributes["value"].Value;

						currentNode = passport.SelectSingleNode("password");
						m_MulServers[iter].userPassword = currentNode.Attributes["value"].Value;

						currentNode = passport.SelectSingleNode("outlookpercentage");
						if (currentNode != null)
							m_MulServers[iter].outlookpercentage = int.Parse(currentNode.Attributes["value"].Value);
						else
							m_MulServers[iter].outlookpercentage = 100;

					}
					
				}

				var executemultipleNode = configsettings.SelectSingleNode("executemultiple");
				if (executemultipleNode != null)
				{
					var totalEntitiesNode = executemultipleNode.SelectSingleNode("totalEntities");
					if (totalEntitiesNode != null)
					{
						ExecuteMultipleTestSettings.TotalEntities = Int32.Parse(totalEntitiesNode.InnerText);
					}

					var maxThreadsNode = executemultipleNode.SelectSingleNode("maxThreads");
					if (maxThreadsNode != null)
					{
						ExecuteMultipleTestSettings.MaxThreads = Int32.Parse(maxThreadsNode.InnerText);
					}

					var batchSizeNode = executemultipleNode.SelectSingleNode("batchSize");
					if (batchSizeNode != null)
					{
						ExecuteMultipleTestSettings.BatchSize = Int32.Parse(batchSizeNode.InnerText);
					}
				}

				XmlNode turboFormsEnabled = configsettings.SelectSingleNode("turboformsenabled");
				if (turboFormsEnabled != null)
				{
					m_turboFormsEnabled = bool.Parse(turboFormsEnabled.Attributes["value"].Value);
				}
			}
			catch (XmlException e)
			{
				System.Diagnostics.Trace.WriteLine("XML Load Error:" + e.Message);
			}
			catch (Exception E)
			{
				System.Diagnostics.Trace.WriteLine(E.Message);
			}
		}

	}

}
