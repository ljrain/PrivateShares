using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;
using CommonTypes;
using Microsoft.Xrm.Sdk.WebServiceClient;

namespace ServiceCreator
{
	public static class XrmServiceCreator
	{
		public static IOrganizationService CreateOrganizationService(string organizationServiceUrl, string userName, string password, AuthenticationProviderType authType, TimeSpan? timeout = null, bool useSSL = false)
		{
            if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) == 0)
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            }
            ClientCredentials userCredentials = new ClientCredentials();
			try
			{
				switch (authType)
				{
					case AuthenticationProviderType.OnlineFederation:
						{
							userCredentials.UserName.UserName = userName;
							userCredentials.UserName.Password = password;
							var baseUri = new Uri(organizationServiceUrl);
							var oAuthManger = new OAuthManager(baseUri.GetLeftPart(UriPartial.Authority).ToString(), "/XrmServices/2011/Organization.svc/web?SdkClientVersion=8.0", userName, password);							
							var token = oAuthManger.Authenticate();
							var client = oAuthManger.GetServiceClient();
						
							return client as IOrganizationService;
						}
					case AuthenticationProviderType.ActiveDirectory:
						{
							ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertificatePolicy);

							var organizationConfiguration = ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(new Uri(organizationServiceUrl));
							OrganizationServiceProxy organizationService = null;

							Guid callerId = Guid.Empty;

							userCredentials.Windows.ClientCredential = new NetworkCredential(userName, password);
							organizationService = new OrganizationServiceProxy(organizationConfiguration, userCredentials)
							{
								CallerId = callerId
							};
							organizationService.EnableProxyTypes();
							organizationService.EndpointAutoSwitchEnabled = true;  // Enable automatic disaster recovery
							if (timeout.HasValue)
							{
								organizationService.Timeout = timeout.Value;

							}
							else
							{
								//set the defaut timeout to 10 minutes
								organizationService.Timeout = new TimeSpan(0, 10, 0);
							}

							return organizationService as IOrganizationService;
						}
					default:
						{
							Console.WriteLine("Authentication Type " + authType.ToString() + " is not implemented yet");
							return null;
						}
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return null;
			}
		}

		private static bool AcceptAllCertificatePolicy(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
	}

	internal static class LiveIdTokenManager
	{
		public static readonly string DeviceUserName = "11" + Environment.MachineName;
		public static readonly string DevicePassword;
		public const int MaxPasswordLength = 24;

		static LiveIdTokenManager()
		{
			string password = Environment.MachineName;
			if (password.Length > MaxPasswordLength)
			{
				DevicePassword = password.Substring(0, MaxPasswordLength);
			}
			else
			{
				DevicePassword = password.PadRight(MaxPasswordLength - password.Length, 'x');
			}
		}


		private static class LiveIdConstants
		{
			public const string RequestSecurityTokenResponseXPath = @"S:Envelope/S:Body/wst:RequestSecurityTokenResponse";
			public const string SecurityTokenResponseXPath = @"S:Envelope/S:Body/wst:RequestSecurityTokenResponse/wst:RequestedSecurityToken";
			public const string ExpirationTimeXPath = @"S:Envelope/S:Body/wst:RequestSecurityTokenResponse/wst:Lifetime/wsu:Expires";

			public static readonly string[] WindowsLiveDeviceUrls = { @"https://login.live.com/ppsecure/DeviceAddCredential.srf", @"https://login.live-int.com/ppsecure/DeviceAddCredential.srf" };

			public const string DeviceRegistrationTemplate = @"<DeviceAddRequest><ClientInfo name=""{0:clientId}"" version=""1.0""/><Authentication><Membername>{1:machineName}</Membername><Password>{2:password}</Password></Authentication></DeviceAddRequest>";
			public const string DeviceTokenTemplate = @"<?xml version=""1.0"" encoding=""UTF-8""?><s:Envelope 
		  xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" 
		  xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" 
		  xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"" 
		  xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" 
		  xmlns:wsa=""http://www.w3.org/2005/08/addressing"" 
		  xmlns:wst=""http://schemas.xmlsoap.org/ws/2005/02/trust"">
		   <s:Header>
			<wsa:Action s:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>
			<wsa:To s:mustUnderstand=""1"">http://Passport.NET/tb</wsa:To>    
			<wsse:Security>
			  <wsse:UsernameToken wsu:Id=""devicesoftware"">
				<wsse:Username>{0:deviceName}</wsse:Username>
				<wsse:Password>{1:password}</wsse:Password>
			  </wsse:UsernameToken>
			</wsse:Security>
		  </s:Header>
		  <s:Body>
			<wst:RequestSecurityToken Id=""RST0"">
				 <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>
				 <wsp:AppliesTo>
					<wsa:EndpointReference>
					   <wsa:Address>http://Passport.NET/tb</wsa:Address>
					</wsa:EndpointReference>
				 </wsp:AppliesTo>
			  </wst:RequestSecurityToken>
		  </s:Body>
		</s:Envelope>
		";

			public const string UserTokenTemplate = @"<?xml version=""1.0"" encoding=""UTF-8""?>
		<s:Envelope 
		  xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" 
		  xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" 
		  xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"" 
		  xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" 
		  xmlns:wsa=""http://www.w3.org/2005/08/addressing"" 
		  xmlns:wst=""http://schemas.xmlsoap.org/ws/2005/02/trust"">
		   <s:Header>
			<wsa:Action s:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>
			<wsa:To s:mustUnderstand=""1"">http://Passport.NET/tb</wsa:To>    
		   <ps:AuthInfo Id=""PPAuthInfo"" xmlns:ps=""http://schemas.microsoft.com/LiveID/SoapServices/v1"">
				 <ps:HostingApp>{0:clientId}</ps:HostingApp>
			  </ps:AuthInfo>
			  <wsse:Security>
				 <wsse:UsernameToken wsu:Id=""user"">
					<wsse:Username>{1:userName}</wsse:Username>
					<wsse:Password>{2:password}</wsse:Password>
				 </wsse:UsernameToken>
				 <wsse:BinarySecurityToken ValueType=""urn:liveid:device"">
				   {3:deviceTokenValue}
				 </wsse:BinarySecurityToken>
			  </wsse:Security>
		  </s:Header>
		  <s:Body>
			<wst:RequestSecurityToken Id=""RST0"">
				 <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>
				 <wsp:AppliesTo>
					<wsa:EndpointReference>
					   <wsa:Address>{4:partnerUrl}</wsa:Address>
					</wsa:EndpointReference>
				 </wsp:AppliesTo>
				 <wsp:PolicyReference URI=""{5:policy}""/>
			  </wst:RequestSecurityToken>
		  </s:Body>
		</s:Envelope>
		";

			public const int MaxPasswordLength = 24;
		}

		public enum LiveIdEnvironment
		{
			//Production = 0,
			Int = 1
		}


		public static void RegisterMachine(LiveIdEnvironment environment)
		{
			string soapEnvelope = CreateDeviceRegistrationSoapEnvelope();

			//Loop through the possible environments and attempt to register on each one
			try
			{
				XmlDocument doc = ExecuteLiveIdServicesRequest(LiveIdConstants.WindowsLiveDeviceUrls[(int)environment], "POST", soapEnvelope);
				if (null != doc)
				{
					XmlNodeList nodes = doc.SelectNodes(@"DeviceAddResponse");
					if (nodes != null && nodes.Count > 0)
					{
						if (string.Equals(nodes[0].Attributes[0].Value, bool.FalseString, StringComparison.OrdinalIgnoreCase))
						{
							throw new InvalidOperationException("Unable to register the machine.");
						}
					}
				}
			}
			catch (WebException ex)
			{
				// Bad Request typically means that the machine is already registered, so we are ok with swallowing this exception and unfortunately
				// there is no other distingushing details on this exception
				if (!ex.Message.Contains("Bad Request"))
				{
					throw;
				}
			}
		}

		private static string CreateDeviceRegistrationSoapEnvelope()
		{
			return string.Format(CultureInfo.InvariantCulture, LiveIdConstants.DeviceRegistrationTemplate,
						Guid.NewGuid().ToString(), DeviceUserName, DevicePassword);
		}

		[SuppressMessage("Microsoft.Security", "CA9876:AvoidLoadXmlUsage", MessageId = "", Justification = "BASELINE: Backlog")]
		private static XmlDocument ExecuteLiveIdServicesRequest(string serviceUrl, string method, string soapMessageEnvelope)
		{
			//Create the request
			WebRequest request = WebRequest.Create(serviceUrl);
			request.Method = method;
			request.Timeout = 180000;
			if (method == "POST")
			{
				// If we are "posting" then this is always a SOAP message
				request.ContentType = "application/soap+xml; charset=UTF-8";
			}

			// If a SOAP envelope is supplied, then we need to write to the request stream
			if (!string.IsNullOrEmpty(soapMessageEnvelope))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(soapMessageEnvelope);
				using (Stream stream = request.GetRequestStream())
				{
					stream.Write(bytes, 0, bytes.Length);
				}
			}

			// Read the response into an XmlDocument and return that doc
			string xml;
			using (WebResponse response = request.GetResponse())
			{
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					xml = reader.ReadToEnd();
				}
			}

			//Load the XML
			var settings = new XmlReaderSettings();
			settings.XmlResolver = null;
			settings.DtdProcessing = DtdProcessing.Prohibit;
			var xmlReader = XmlReader.Create(xml, settings);
			XmlDocument document = new XmlDocument();
			document.Load(xmlReader);
			return document;
		}

	}

	internal static class SecurityTokenManager
	{
		public static SecurityToken GetSecurityToken<TChannel>(IServiceConfiguration<TChannel> serviceConfiguration, ClientCredentials userCredentials)
					where TChannel : class
		{
			SecurityToken token = null;

			switch (serviceConfiguration.AuthenticationType)
			{
				case AuthenticationProviderType.LiveId:
					ClientCredentials deviceCredentials = new ClientCredentials();
					deviceCredentials.UserName.UserName = LiveIdTokenManager.DeviceUserName;
					deviceCredentials.UserName.Password = LiveIdTokenManager.DevicePassword;
					SecurityTokenResponse deviceResponseWrapper;
					try
					{
						deviceResponseWrapper = serviceConfiguration.AuthenticateDevice(deviceCredentials);
					}
					catch (System.ServiceModel.Security.MessageSecurityException)
					{
						Console.WriteLine("Got System.ServiceModel.Security.MessageSecurityException when trying to authenticate device. Most probably this device (machine {0}) was not registered with LiveId. Trying to register now",
									Environment.MachineName);
						LiveIdTokenManager.RegisterMachine(LiveIdTokenManager.LiveIdEnvironment.Int);
						deviceResponseWrapper = serviceConfiguration.AuthenticateDevice(deviceCredentials);
						Console.WriteLine("Authenticated the device {0} with registering the machine {1}", deviceCredentials.UserName.UserName, Environment.MachineName);
					}

					token = serviceConfiguration.Authenticate(userCredentials, deviceResponseWrapper).Token;
					break;

				case AuthenticationProviderType.OnlineFederation:
					try
					{
						token = serviceConfiguration.Authenticate(userCredentials).Token;
					}
					catch (Exception e)
					{

						Console.WriteLine(e.ToString());
						return null;
					}
					break;

				case AuthenticationProviderType.Federation:
					token = serviceConfiguration.Authenticate(userCredentials).Token;
					break;
			}

			return token;
		}
	}
}
