using System.Diagnostics.CodeAnalysis;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Win32;


using Microsoft.Crm;
using Microsoft.Crm.Authentication;
using Microsoft.Crm.Data;
using Microsoft.Crm.SharedDatabase;
using Microsoft.Crm.ConfigurationDatabase;
//using Microsoft.Crm.BusinessEntities;
using Microsoft.Crm.ObjectModel;

//using ExecutionContext = Microsoft.Crm.BusinessEntities.ExecutionContext;
using Microsoft.Crm.CrmLive.Provisioning;

namespace Microsoft.Crm.CrmLive.Provisioning
{
	public class DriverAddInitialUserToConfigDBAction : ProvisioningAction
	{
		public OrganizationProperties m_organizationProperties;

		public DriverAddInitialUserToConfigDBAction(OrganizationProperties organizationProperties)
			: base(organizationProperties)
		{
			m_organizationProperties = organizationProperties;
		}

		public override void Do()
		{
			TraceDoMethodEntry(m_organizationProperties.OrganizationData.Id);

			SetSystemUserPuid(m_organizationProperties.OrganizationData.Id, m_organizationProperties.InitialUser.Puid);

			TraceDoMethodExit(m_organizationProperties.OrganizationData.Id);
		}

		private void SetSystemUserPuid(Guid organizationId, string initialUserPuid)
		{
			if (!LiveProvisioning)
			{
				return;
			}

			Guid crmUserId = Guid.NewGuid();

			if (crmUserId == Guid.Empty)
			{
				string message = String.Format(CultureInfo.InvariantCulture, "GetInitialCrmUserId returned Guid.Empty for Organization Id = {0}",
					organizationId);
				throw new CrmException(message, ErrorCodes.CrmLiveOrganizationProvisioningFailed);

			}

			string authInfo = Microsoft.Crm.CrmUtility.FormatPuid(initialUserPuid);
			CrmPrincipal.AddPrincipalToConfigDB(organizationId, crmUserId, authInfo);
		}
	}
}
