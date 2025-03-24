using System.Diagnostics.CodeAnalysis;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
using System.Collections;

using Microsoft.Crm;
using Microsoft.Crm.Authentication;
using Microsoft.Crm.Data;
using Microsoft.Crm.ConfigurationDatabase;
//using Microsoft.Crm.BusinessEntities;
using Microsoft.Crm.ObjectModel;
//using Microsoft.Crm.Metadata;
using Microsoft.Crm.Setup.Server.Utility;

namespace Microsoft.Crm.CrmLive.Provisioning
{
	/// <summary>
	/// Create the support user, add it to CRM DB. The user is also added to config DB under SystemUser and SystemUserOrganization.
	/// When the support user login through Passport, the user's info will be added to config DB SystemUserAuthetication to get access tickets
	/// </summary>
	public class DriverAddSupportUserAction : ProvisioningAction
	{
		private String supportSystemUserNameFormat = "support_user_{0}";
		private OrganizationProperties m_organizationProperties;
		public DriverAddSupportUserAction(OrganizationProperties organizationProperties)
			: base(organizationProperties)
		{
			m_organizationProperties = organizationProperties;
		}

		/// <summary>
		/// This action should be idempotent. It should create the necessary entries only if they dont exist.
		/// </summary>
		public override void Do()
		{
			TraceDoMethodEntry(m_organizationProperties.OrganizationData.Id);

			Guid supportCrmUserId = Guid.NewGuid();
			CreateSupportSystemUserInConfigDatabase(m_organizationProperties.OrganizationData.Id, m_organizationProperties.OrganizationData.UniqueName, supportCrmUserId);

			TraceDoMethodExit(m_organizationProperties.OrganizationData.Id);
		}

		private void CreateSupportSystemUserInConfigDatabase(Guid organizationId, String organizationUniqueName, Guid supportCrmUserId)
		{
			using (ConfigurationDatabaseService service = new ConfigurationDatabaseService())
			{
				PropertyBag bag = new PropertyBag();
				bag["Name"] = String.Format(CultureInfo.InvariantCulture, supportSystemUserNameFormat, organizationUniqueName.ToUpperInvariant());

				PropertyBagCollection bags = service.Retrieve("SystemUser", new String[] { "Id" }, new PropertyBag[] { bag });

				Guid supportSystemUserId = Guid.Empty;

				// Create a new entry or retrieve an existing entry for support system user
				if (bags == null || bags.Values == null || bags.Values.Count == 0)
				{
					bag["DefaultOrganizationId"] = organizationId;
					bag["Id"] = Guid.NewGuid();
					supportSystemUserId = (Guid)service.Create("SystemUser", bag);
				}
				else if (bags.Values.Count == 1)
				{
					foreach (PropertyBag b in bags.Values)
					{
						supportSystemUserId = (Guid)b["Id"];
					}
				}

				if (supportSystemUserId != Guid.Empty)
				{
					PropertyBag sysOrgBag = new PropertyBag();
					sysOrgBag["CrmUserId"] = supportCrmUserId;

					PropertyBagCollection sysOrgBags = service.Retrieve("SystemUserOrganizations", new String[] { "Id" }, new PropertyBag[] { sysOrgBag });

					// Create a new entry if it does not exists for sysmte user organization
					if (sysOrgBags == null || sysOrgBags.Values == null || sysOrgBags.Values.Count == 0)
					{
						sysOrgBag["OrganizationId"] = organizationId;
						sysOrgBag["UserId"] = supportSystemUserId;
						sysOrgBag["Id"] = Guid.NewGuid();
						service.Create("SystemUserOrganizations", sysOrgBag);
					}
				}
			}
		}
	}
}
