		IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'EntityManager{ORGNAME}')
		DROP DATABASE [EntityManager{ORGNAME}]
		GO

		create Database [EntityManager{ORGNAME}]
		go

		use [EntityManager{ORGNAME}]
		go

		create Table SystemInfo
		(
			ServerCN nvarchar(200)
				PRIMARY KEY NOT NULL,
			Loaded datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		go

		create table Report
		(
		ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ReportId uniqueidentifier
			NOT NULL,
		FileName varchar(255)
			NOT NULL,
		ReportTypeCode int
			NOT NULL,
		ReportNameOnSRS varchar(255)
			NULL,
		State int
				DEFAULT(0) NOT NULL,
		TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		go
		Create Trigger TimeStampUpdateReport
			on Report
		AFTER UPDATE AS
			UPDATE Report set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		go

		create table TransactionCurrency
		(
		ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		TransactionCurrencyId uniqueidentifier
			NOT NULL,
		CurrencyName varchar(255)
			NOT NULL,
		CurrencySymbol varchar(30),
		ISOCurrencyCode varchar(30)
			NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
		TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		go

		SET QUOTED_IDENTIFIER ON
		GO

CREATE TABLE [dbo].[TeamMembership](
	[ID] [bigint] NOT NULL,
	[EntityManagerOwningUser] [uniqueidentifier] NOT NULL,
	[TeamMembershipId] [uniqueidentifier] NOT NULL,	
	[TeamId] [uniqueidentifier] NOT NULL,
	[SystemUserId] [uniqueidentifier] NOT NULL,	
	[State] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TeamMembership] ADD  DEFAULT ((0)) FOR [State]
GO

ALTER TABLE [dbo].[TeamMembership] ADD  DEFAULT (getdate()) FOR [TimeStamp]
GO

Create Trigger [dbo].[TimeStampUpdateTeamMembership]
			on [dbo].[TeamMembership]
		AFTER UPDATE AS
			UPDATE TeamMembership set TimeStamp=GetDate() WHERE ID in (select id from inserted)
GO


		create table Competitor
		(
		ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		CompetitorId uniqueidentifier
			NOT NULL,
		Name varchar(255)
			NOT NULL,
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL
		)
		go

		Create Trigger TimeStampUpdateTransactionCurrency
			on TransactionCurrency
		AFTER UPDATE AS
			UPDATE TransactionCurrency set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		go

		create table ConnectionRole
		(
		ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ConnectionRoleId uniqueidentifier
			NOT NULL,
		Name varchar(255)
			NOT NULL,
		Category int
			DEFAULT(0) NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
		TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		go
		Create Trigger TimeStampUpdateConnectionRole
			on ConnectionRole
		AFTER UPDATE AS
			UPDATE ConnectionRole set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		go

		create table Queue
		(
		ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		QueueId uniqueidentifier
			NOT NULL,
		Name varchar(255)
			NOT NULL,
	OwningBusinessUnit uniqueidentifier
		NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
		TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		go
		Create Trigger TimeStampUpdateQueue
			on Queue
		AFTER UPDATE AS
			UPDATE Queue set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		go

	  create table Solution
	  (
	ID bigint
			PRIMARY KEY,
	OrganizationId uniqueidentifier
			NOT NULL,
	EntityManagerOwningUser uniqueidentifier
			NOT NULL,
	SolutionId uniqueidentifier
		NOT NULL,
	OrganizationIdName varchar(255)
		NOT NULL,
	FriendlyName varchar(255)
		NOT NULL,
	  State int
			DEFAULT(0) NOT NULL,
	TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL
	  )
	  go
	  Create Trigger TimeStampUpdateSolution
		on Solution
	  AFTER UPDATE AS
		UPDATE Solution set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  go

		create table SetupUser
		(
			ID bigint PRIMARY KEY,
			OrganizationId uniqueidentifier
				NOT NULL,
			OrganizationName varchar(128),
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			DomainName varchar(255)
				 NOT NULL,
			UserPassword varchar(50),
			SystemUserId uniqueidentifier
				 NOT NULL,
			BusinessUnitId uniqueidentifier
				NOT NULL,
			Role varchar(255),
			SetupUser bit
				DEFAULT(0) NOT NULL,

		)
		go

		create table SystemUser
		(
			ID bigint PRIMARY KEY,
			OrganizationId uniqueidentifier
				NOT NULL,
			OrganizationName varchar(128),
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			DomainName varchar(255)
				 NOT NULL,
			UserPassword varchar(50),
			ExchangePassword varchar(32),
			SystemUserId uniqueidentifier
				 NOT NULL,
			BusinessUnitId uniqueidentifier
				NOT NULL,
			Role varchar(255),
			ServerBaseUrl varchar(255),
			OrganizationBaseUrl varchar(255),
			OrganizationServiceUrl varchar(255),
			DiscoveryServer varchar(255),
			ApiServer varchar(255),
			Partner varchar(255),
			PassportName varchar(128),
			PassportTicket varchar(MAX),
			PassportTicketExpiration varchar(128),
			CrmTicket varchar(MAX),
			CrmTicketExpiration varchar(128),
			IsSharepointUser int
				DEFAULT(0) NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			OutlookState int
				DEFAULT(0) NOT NULL,
			SetupUser bit
				DEFAULT(0) NOT NULL,
			InternalEmailAddress varchar(255),
			BusinessUnitIdName varchar(255),
			OutlookUser bit
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			EntityFormMode int,
			MoCAUser bit
				DEFAULT(1) NOT NULL,
			EnabledForACT bit
				DEFAULT(0) NOT NULL,
			IsUserConfiguredForMobileOffline bit DEFAULT(0),
			MobileOfflineProfileId uniqueidentifier,
			MobileOfflineDeviceId uniqueidentifier DEFAULT newid(),
			MobileOfflineModelVersion int DEFAULT(1),
			MobileOfflinePrevSID int DEFAULT(NULL),
			MobileOfflineRuleVersion int DEFAULT(1),
			MobileOfflineSyncEndPoint varchar(2048) DEFAULT(NULL),
			MobileOfflineUserState int DEFAULT(0)
		)
		go

		Create Trigger TimeStampUpdateSystemUser
			on SystemUser
		AFTER UPDATE AS
			UPDATE SystemUser set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		go

		Create Table Organization
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				 NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ServerBaseUrl varchar(255),
		SharepointServer varchar(255),

			Tokenkey nvarchar(90),
			FeatureSet nvarchar(Max),
			ExchangeMode bit
				DEFAULT(0),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			MobileOfflineSyncEndpoint nvarchar(2048),
			IsMobileOfflineOptedIn bit DEFAULT(0),
			AdxEnabled bit DEFAULT(0),
						EnabledForF1 bit DEFAULT(0),

			AdxCustomerPortalUrl nvarchar(2048),
			AdxCommunityPortalUrl nvarchar(2048),
			AdxPartnerPortalUrl nvarchar(2048)
		)
		GO

		Create Table FakeParentOrg
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				 NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateOrganization
			on Organization
		AFTER UPDATE AS
			UPDATE Organization set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Lead
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		LeadId uniqueidentifier
				 NOT NULL,
			FirstName nvarchar(150),
			LastName nvarchar(150),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateLead
			on Lead
		AFTER UPDATE AS
			UPDATE Lead set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Opportunity
		(
			ID bigint
				NOT NULL,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		OpportunityId uniqueidentifier
				 NOT NULL,
			Name nvarchar(160),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateOpportunity
			on Opportunity
		AFTER UPDATE AS
			UPDATE Opportunity set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		CREATE UNIQUE CLUSTERED INDEX [CL_ID_State] ON [dbo].[Opportunity]
		(
		[ID] ASC,
		[State] ASC
		)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

		Create Table Account
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			AccountId uniqueidentifier
				 NOT NULL,
				 		customertypecode int NULL,
			ParentAccountId uniqueidentifier,
			Name nvarchar(160),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			[New_HierarchyNodeLevel] [varchar](255) NULL
		)
		GO

		Create Trigger TimeStampUpdateAccount
			on Account
		AFTER UPDATE AS
			UPDATE Account set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table New_Simple_CustomAccount
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		New_Simple_CustomAccountId uniqueidentifier
				 NOT NULL,
			New_Name nvarchar(160),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateNew_Simple_CustomAccount
			on New_Simple_CustomAccount
		AFTER UPDATE AS
			UPDATE New_Simple_CustomAccount set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Contact
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			ContactId uniqueidentifier
				 NOT NULL,
			LastName nvarchar(50),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			adx_identity_username nvarchar(100),
			OrganizationId uniqueidentifier
		)
		GO

				Create Table msdyn_customerasset
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		msdyn_customerassetId uniqueidentifier
				 NOT NULL,
		msdyn_name nvarchar(100),
		msdyn_masterasset uniqueidentifier,
		msdyn_account uniqueidentifier,
		msdyn_parentasset uniqueidentifier,
		msdyn_product uniqueidentifier,
		msdyn_workorderproduct uniqueidentifier,

			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL 
		)
		GO

	   Create Table BookableResourceBooking
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		BookableResourceBookingId uniqueidentifier
				 NOT NULL,
		BookingStatus uniqueidentifier,
		name nvarchar(100),
		Resource uniqueidentifier
				 NOT NULL,
		starttime datetime,
		endtime datetime,
		msdyn_projectid uniqueidentifier null,
		msdyn_workorder uniqueidentifier,

			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO
		
		Create Table Territory
		(
			ID bigint
				PRIMARY KEY,
			OrganizationId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			territoryid uniqueidentifier
				NOT NULL,				
			name nvarchar(160),
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO
		
		Create Table msdyn_routingoptimizationrequest
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			msdyn_routingoptimizationrequestid uniqueidentifier
				NOT NULL,
			msdyn_requestid nvarchar(36),
			msdyn_optimizationstatus int,
			msdyn_routingconfiguration nvarchar(max),
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table msdyn_project
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			msdyn_projectId uniqueidentifier
				NOT NULL,
			msdyn_subject nvarchar(100),
			msdyn_istemplate bit Default(0),
			msdyn_projecttemplate uniqueidentifier,
			msdyn_bulkgenerationstatus int,
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table msdyn_workhourtemplate
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			msdyn_workhourtemplateId uniqueidentifier
				NOT NULL,
			msdyn_name nvarchar(200),
						State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO
		
		Create Table msdyn_resourcerequirement
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			msdyn_resourcerequirementId uniqueidentifier
				NOT NULL,
			msdyn_name nvarchar(100),
			msdyn_fromdate datetime,
			msdyn_todate datetime,
			msdyn_quantity decimal(23,10),
			msdyn_workhourtemplate uniqueidentifier,
			msdyn_projectid uniqueidentifier
				NOT NULL,
			msdyn_roleid uniqueidentifier
				NOT NULL,
							State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table msdyn_projectteam
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			msdyn_projectteamId uniqueidentifier
				NOT NULL,
			msdyn_name nvarchar(100),
			msdyn_from datetime,
			msdyn_to datetime,
			msdyn_bookableresourceid uniqueidentifier,
			msdyn_worktemplate uniqueidentifier,
			msdyn_project uniqueidentifier
				NOT NULL,
			msdyn_resourcecategory uniqueidentifier
				NOT NULL,
			msdyn_resourcerequirementid uniqueidentifier,
							State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO
		
		Create Table msdyn_projecttask
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			msdyn_projecttaskId uniqueidentifier
				NOT NULL,
			msdyn_subject nvarchar(200),
			msdyn_effort float,
			msdyn_parenttask uniqueidentifier,
			msdyn_project uniqueidentifier
				NOT NULL,
			msdyn_scheduledstart datetime,
			msdyn_scheduledend datetime,
			msdyn_WBSID nvarchar(100),
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO
		
		Create Table msdyn_WorkOrder
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		msdyn_workorderId uniqueidentifier
				 NOT NULL,
		msdyn_workordersummary nvarchar(100),
		msdyn_workordertype uniqueidentifier,
		msdyn_serviceterritory uniqueidentifier,
		msdyn_systemstatus int,

			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO


		
		Create Table BookingStatus
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		BookingStatusId uniqueidentifier
				 NOT NULL,

		name nvarchar(100),
		msdyn_fieldservicestatus int,
		msdyn_StatusColor nvarchar(100),

			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table msdyn_WorkOrderType
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		msdyn_workordertypeId uniqueidentifier
				 NOT NULL,
		msdyn_name nvarchar(100),
		msdyn_PriceList uniqueidentifier,

			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table msdyn_timeentry
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
					NOT NULL,
			msdyn_timeentryId uniqueidentifier
					 NOT NULL,
			msdyn_description nvarchar(100),
			msdyn_externaldescription nvarchar(100),
			msdyn_project uniqueidentifier,
			msdyn_entrystatus int,
			msdyn_bookableresource uniqueidentifier,
			msdyn_duration int,
			msdyn_date datetime,
			msdyn_type int,
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table msdyn_expense
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
					NOT NULL,
			msdyn_expenseid uniqueidentifier
					 NOT NULL,
			msdyn_name nvarchar(100),
			msdyn_externaldescription nvarchar(100),
			msdyn_project uniqueidentifier,
			msdyn_expensestatus int,
			msdyn_transactiondate datetime,
			msdyn_expensecategory uniqueidentifier,
			msdyn_amount float,
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table msdyn_expensecategory
		(
			ID bigint
				PRIMARY KEY,
			OrganizationId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
					NOT NULL,
			msdyn_expensecategoryid uniqueidentifier
				 NOT NULL,
			msdyn_expensetype int,
			msdyn_name nvarchar(100)
				NOT NULL,
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO
		
		Create Table msdyn_taxcode
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		msdyn_taxcodeId uniqueidentifier
				 NOT NULL,
		msdyn_name nvarchar(100),
		msdyn_TaxRate float, 

			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table msdyn_incidenttype
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		msdyn_incidenttypeid uniqueidentifier
				 NOT NULL,
		msdyn_name nvarchar(100),
		msdyn_Description nvarchar(100),
		msdyn_estimatedduration int,
		msdyn_DefaultWorkerOrderType uniqueidentifier, 

			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table BookableResource
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		BookableResourceId uniqueidentifier
				 NOT NULL,
		 UserId uniqueidentifier,

		  ContactId uniqueidentifier,


		msdyn_HourlyRate money,
		msdyn_PrimaryEmail nvarchar(100),

		msdyn_organizationalunit uniqueidentifier,

			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table BookableResourceCategory
		(
			ID bigint PRIMARY KEY,
			OwnerId uniqueidentifier NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			BookableResourceCategoryId uniqueidentifier
				NOT NULL,
			Name nvarchar(100),
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table Characteristic
		(
			ID bigint PRIMARY KEY,
			OwnerId uniqueidentifier NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			CharacteristicId uniqueidentifier
				NOT NULL,
			Name nvarchar(100),
			CharacteristicType int,
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table RatingValue
		(
			ID bigint PRIMARY KEY,
			OwnerId uniqueidentifier NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			RatingValueId uniqueidentifier
				NOT NULL,
			Name nvarchar(100),
			Value int,
			State int 
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO



		Create Trigger TimeStampUpdateContact
			on Contact
		AFTER UPDATE AS
			UPDATE Contact set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table ExchangeContact
		(
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			LastName nvarchar(100),
			ExchangeId nvarchar(200)
		)
		GO

		Create Trigger TimeStampUpdateNew_ExchangeContact
			on ExchangeContact
		AFTER UPDATE AS
			UPDATE ExchangeContact set TimeStamp=GetDate() WHERE ExchangeId in (select ExchangeId from inserted)

		Go


		Create Table ExchangeAppointment
		(
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			Subject nvarchar(100),
			ExchangeId nvarchar(200)
		)
		GO

		Create Trigger TimeStampUpdateNew_ExchangeAppointment
			on ExchangeAppointment
		AFTER UPDATE AS
			UPDATE ExchangeAppointment set TimeStamp=GetDate() WHERE ExchangeId in (select ExchangeId from inserted)
		GO


		Create Table GenericActivities
		(
			ID bigint
				PRIMARY KEY,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateGenericActivities
			on GenericActivities
		AFTER UPDATE AS
			UPDATE GenericActivities set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Service
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ServiceId uniqueidentifier
				 NOT NULL,
			Name nvarchar(160),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateService
			on Service
		AFTER UPDATE AS
			UPDATE Service set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Subject
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		SubjectId uniqueidentifier
				 NOT NULL,
			Title nvarchar(500),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateSubject
			on Subject
		AFTER UPDATE AS
			UPDATE Subject set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Task
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ActivityId uniqueidentifier
				 NOT NULL,
		Subject nvarchar(200),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL

		)
		GO

		Create Trigger TimeStampUpdateTask
			on Task
		AFTER UPDATE AS
			UPDATE Task set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO


		Create Table ExchangeTask
		(
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ExchangeId nvarchar(200)
				 NOT NULL,
		Subject nvarchar(200),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL

		)
		GO

		Create Trigger TimeStampUpdateNew_ExchangeTask
			on ExchangeTask
		AFTER UPDATE AS
			UPDATE ExchangeTask set TimeStamp=GetDate() WHERE ExchangeId in (select ExchangeId from inserted)
		GO


	  Create Table Appointment
	 (
		ID bigint
			PRIMARY KEY,
		OwnerId uniqueidentifier
			NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ActivityId uniqueidentifier
			 NOT NULL,
		State int
			DEFAULT(0) NOT NULL,
		Subject nvarchar(200),
		InstanceTypeCode int
			NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL

	  )
		GO

	  Create Trigger TimeStampUpdateAppointment
		on Appointment
	  AFTER UPDATE AS
		UPDATE Appointment set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO

		Create Table RecurringAppointmentMaster
	 (
		ID bigint
			PRIMARY KEY,
		OwnerId uniqueidentifier
			NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ActivityId uniqueidentifier
			 NOT NULL,
		Subject nvarchar(200),
		RecurrencePatternType int NOT NULL,
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL

	  )
		GO

	  Create Trigger TimeStampUpdateRecurringAppointmentMaster
		on RecurringAppointmentMaster
	  AFTER UPDATE AS
		UPDATE RecurringAppointmentMaster set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO

	  Create Table Email
	  (
		ID bigint
			PRIMARY KEY,
		OwnerId uniqueidentifier
			NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ActivityId uniqueidentifier
			 NOT NULL,
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL

	  )
	  GO

	  Create Trigger TimeStampUpdateEmail
		on Email
	  AFTER UPDATE AS
		UPDATE Email set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO

	  Create Table Fax
	  (
		ID bigint
			PRIMARY KEY,
		OwnerId uniqueidentifier
			NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ActivityId uniqueidentifier
			 NOT NULL,
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL

	  )
	  GO

	  Create Trigger TimeStampUpdateFax
		on Fax
	  AFTER UPDATE AS
		UPDATE Fax set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO

	  Create Table Letter
	  (
		ID bigint
			PRIMARY KEY,
		OwnerId uniqueidentifier
			NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ActivityId uniqueidentifier
			 NOT NULL,
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL,
		Subject nvarChar(200)
			NOT NULL
	  )
	  GO

	  Create Trigger TimeStampUpdateLetter
		on Letter
	  AFTER UPDATE AS
		UPDATE Letter set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO

	  Create Table Phonecall
	  (
		ID bigint
			PRIMARY KEY,
		OwnerId uniqueidentifier
			NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ActivityId uniqueidentifier
			 NOT NULL,
		RegardingObjectTypeCode int,
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL,
		Subject nvarchar(200)
			NOT NULL

	  )
	  GO

	  Create Trigger TimeStampUpdatePhonecall
		on Phonecall
	  AFTER UPDATE AS
		UPDATE Phonecall set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO

	   Create Table Quote
		(
			ID bigint
				PRIMARY KEY,
		OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		QuoteId uniqueidentifier
				 NOT NULL,
			Name nvarchar(300),
		CustomerIdName nvarchar(160),
		CustomerId uniqueidentifier,
		CustomerIdType int,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateQuote
			on Quote
		AFTER UPDATE AS
			UPDATE Quote set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table SalesOrder
		(
			ID bigint
				PRIMARY KEY,
		OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		SalesOrderId uniqueidentifier
				 NOT NULL,
			Name nvarchar(300),
		CustomerIdName nvarchar(160),
		CustomerId uniqueidentifier,
		CustomerIdType int,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateSalesOrder
			on SalesOrder
		AFTER UPDATE AS
			UPDATE SalesOrder set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Template
		(
			ID bigint
				PRIMARY KEY,
		OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		TemplateId uniqueidentifier
				 NOT NULL,
			TemplateTypeCode int
				NOT NULL,
			Subject ntext,
			Title nvarchar(200),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateTemplate
			on Template
		AFTER UPDATE AS
			UPDATE Template set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Incident
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			IncidentId uniqueidentifier
				 NOT NULL,
			Title nvarchar(200),
			CustomerID uniqueidentifier
				 NOT NULL,
			CustomerIdName nvarchar(4000)
				 NOT NULL,
			StateCode int
				DEFAULT(0) NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			PriorityCode int
				DEFAULT(2) NOT NULL,
			CaseTypeCode int null
		)
		GO

		Create Trigger TimeStampUpdateIncident
			on Incident
		AFTER UPDATE AS
			UPDATE Incident set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table KnowledgeArticle
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			KnowledgeArticleId uniqueidentifier
				 NOT NULL,
			Keywords nvarchar(max),
			ArticlePublicNumber nvarchar(127),
			Title nvarchar(500),
			Content nvarchar(max),
			StateCode int
				 NOT NULL,
			StatusCode int
				DEFAULT(0),
				[IsInternal] bit,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			[organizationid] [uniqueidentifier] NULL,
		)
		GO

		Create Trigger TimeStampUpdateKnowledgeArticle
			on KnowledgeArticle
		AFTER UPDATE AS
			UPDATE KnowledgeArticle set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Annotation
		(
			ID bigint
				PRIMARY KEY,
		OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		AnnotationId uniqueidentifier
				 NOT NULL,
		ObjectId uniqueidentifier,
			ObjectTypeCode int,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateAnnotation
			on Annotation
		AFTER UPDATE AS
			UPDATE Annotation set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		CREATE NONCLUSTERED INDEX [NC_state_objectid] ON [dbo].[Annotation]
		(
		[ObjectId] ASC,
		[State] ASC
		)WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
		GO

		Create Table PriceLevel
		(
			ID bigint
				PRIMARY KEY,
		Name nvarchar(100) NOT NULL,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			PriceLevelId uniqueidentifier
				 NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdatePriceLevel
			on PriceLevel
		AFTER UPDATE AS
			UPDATE PriceLevel set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Resource
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			ResourceId uniqueidentifier
				 NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO


		Create Table List
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ListId uniqueidentifier
				 NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateList
			on List
		AFTER UPDATE AS
			UPDATE List set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table BusinessUnit
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		BusinessUnitId uniqueidentifier
				NOT NULL,
			ParentBusinessUnitId uniqueidentifier,
			Name nvarchar(160)
				NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateBusinessUnit
			on BusinessUnit
		AFTER UPDATE AS
			UPDATE BusinessUnit set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Campaign
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		CampaignId uniqueidentifier
				 NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateCampaign
			on Campaign
		AFTER UPDATE AS
			UPDATE Campaign set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table ServiceAppointment
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ActivityId uniqueidentifier
				 NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			Subject nvarchar(200)
		)
		GO

		Create Trigger TimeStampUpdateServiceAppointment
			on ServiceAppointment
		AFTER UPDATE AS
			UPDATE ServiceAppointment set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Role
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		RoleId uniqueidentifier
				 NOT NULL,
			Name nvarchar(100)
				NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateRole
			on Role
		AFTER UPDATE AS
			UPDATE Role set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Privilege
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		PrivilegeId uniqueidentifier
				 NOT NULL,
			Name nvarchar(100),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdatePrivilege
			on Privilege
		AFTER UPDATE AS
			UPDATE Privilege set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

	  Create Table Equipment
	  (
		ID bigint
			PRIMARY KEY,
		OrganizationId uniqueidentifier
			NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		EquipmentId uniqueidentifier
			 NOT NULL,
		Name nvarchar(100)
			NOT NULL,
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL
	  )
	  GO

	  Create Trigger TimeStampUpdateEquipment
		on Equipment
	  AFTER UPDATE AS
		UPDATE Equipment set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO

	Create Table Team
	  (
		ID bigint
			PRIMARY KEY,

		OrganizationId uniqueidentifier
			NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		TeamId uniqueidentifier NOT NULL,
		BusinessUnitId uniqueidentifier
			NOT NULL,
		Name nvarchar(100)
			NOT NULL,
		IsDefault bit NOT NULL DEFAULT(0),
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL
	  )
	  GO

	  Create Trigger TimeStampUpdateTeam
		on Team
	  AFTER UPDATE AS
		UPDATE Team set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO

		Create Table SavedQueryVisualization
		(
			ID bigint
				NOT NULL,
			OrganizationId uniqueidentifier
			NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		SavedQueryVisualizationId uniqueidentifier
				 NOT NULL,
			Name nvarchar(100),
			PrimaryEntityTypeCode int
				NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateSavedQueryVisualization
			on SavedQueryVisualization
		AFTER UPDATE AS
			UPDATE SavedQueryVisualization set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO
		Create Table Goal
		(
			ID bigint
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			OwnerId uniqueidentifier
				NOT NULL,
			GoalId uniqueidentifier
				 NOT NULL,
			MetricId uniqueidentifier
				 NOT NULL,
			ParentGoalId uniqueidentifier
				  NULL,
			Title nvarchar(100),
			OwningBusinessUnit uniqueidentifier
			NOT NULL,
			GoalOwnerId uniqueidentifier
			NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateGoal
			on Goal
		AFTER UPDATE AS
			UPDATE Goal set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO



	Create Table SharePointSite
	  (
		ID bigint
			PRIMARY KEY,

		OrganizationId uniqueidentifier
				NOT NULL,

		SharePointSiteId uniqueidentifier NOT NULL,

		OwningBusinessUnit uniqueidentifier
			NOT NULL,

		AbsoluteURL nvarchar(160) NOT NULL ,

		RelativeUrl nvarchar(160) NOT NULL ,
		IsDefault bit NOT NULL DEFAULT(0),
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL
	  )
	  GO

	  Create Trigger TimeStampUpdateSpSite
		on SharePointSite
	  AFTER UPDATE AS
		UPDATE SharePointSite set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO

		Create Table SharePointDocumentLocation
	  (
		ID bigint
			PRIMARY KEY,

		OrganizationId uniqueidentifier
				NOT NULL,
		SharePointDocumentLocationId uniqueidentifier NOT NULL,
		ParentSiteOrLocation uniqueidentifier NOT NULL,
		OwningBusinessUnit uniqueidentifier
			NOT NULL,
		RelativeUrl nvarchar(255) NOT NULL,
		AbsoluteURL nvarchar(255) NOT NULL,
		RegardingObjectId uniqueidentifier NOT NULL,
		SiteCollectionId uniqueidentifier NOT NULL,

		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL
	  )
	  GO

	Create Trigger TimeStampUpdateSharePointDocumentLocation
		on SharePointDocumentLocation
	  AFTER UPDATE AS
		UPDATE SharePointDocumentLocation set TimeStamp=GetDate() WHERE ID in (select id from inserted)
	  GO


		Create Table Workflow
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			WorkflowId uniqueidentifier
				 NOT NULL,
			ActiveWorkflowId uniqueidentifier,
			ParentWorkflowId uniqueidentifier,
			Name nvarchar(200),
			IsCrmUIWorkflow int
				DEFAULT(1) NOT NULL,
			Type int,
			PrimaryEntity nvarchar(100)
				NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateWorkflow
			on Workflow
		AFTER UPDATE AS
			UPDATE Workflow set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO


		Create Table New_IM
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			ActivityId uniqueidentifier
				 NOT NULL,
			Subject nvarchar(200),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateNew_IM
			on New_IM
		AFTER UPDATE AS
			UPDATE New_IM set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table New_Prospect
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			New_ProspectId uniqueidentifier
				 NOT NULL,
			New_name nvarchar(200),
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateNew_Prospect
			on New_Prospect
		AFTER UPDATE AS
			UPDATE New_Prospect set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table UoM
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		UoMId uniqueidentifier
				NOT NULL,
		Name nvarchar(160)
				NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdate_UoM
			on UoM
		AFTER UPDATE AS
			UPDATE UoM set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table Product
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ProductId uniqueidentifier
				NOT NULL,
		ParentProductId uniqueidentifier,
		TransactionCurrencyId uniqueidentifier
				NOT NULL,
		Name nvarchar(160)
				NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdate_Product
			on Product
		AFTER UPDATE AS
			UPDATE Product set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table ProductPriceLevel
		(
			ID bigint
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		EntityManagerOwningUser uniqueidentifier
				NOT NULL,
		ProductPriceLevelId uniqueidentifier
				 NOT NULL,
		ProductId uniqueidentifier
				 NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Trigger TimeStampUpdateProductPriceLevel
			on ProductPriceLevel
		AFTER UPDATE AS
			UPDATE ProductPriceLevel set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO

		Create Table SocialInsightsConfiguration
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,

			SocialInsightsConfigurationId uniqueidentifier
				NOT NULL,
			FormId uniqueidentifier
				NOT NULL,
			ControlId nvarchar(160),
			SocialDataItemId nvarchar(160),
			SocialDataParameters nvarchar(160)
		)
		GO

		Create Trigger TimeStampUpdateSocialInsights
			on SocialInsightsConfiguration
		AFTER UPDATE AS
			UPDATE SocialInsightsConfiguration set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		GO


		Create Table ExchangeSyncSettings
		(
			MailboxId uniqueidentifier
				PRIMARY KEY,
		OrganizationId uniqueidentifier
				NOT NULL,
		SystemUserId uniqueidentifier
				NOT NULL,
	    OutgoingServerLocation nvarchar(1024)
			DEFAULT(NULL),
		UseAutoDiscover bit
			DEFAULT(0),
		EnabledForACT bit
			DEFAULT(0),
		State int
			DEFAULT(0) NOT NULL,
		TimeStamp datetime
			DEFAULT(GETDATE()) NOT NULL
		)
		GO

		Create Table TempUserRole
		(
			SystemUserId uniqueidentifier
				NOT NULL,
			EntityFormMode int NULL
		) On [Primary]
		GO

		Create Table TempSystemUserRole
		(
			DomainName Varchar(255) NULL,
			RoleName varchar(255) NULL
		) On [Primary]
		GO

		Create Table new_customaccount
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			new_customaccountid uniqueidentifier
				 NOT NULL,
			new_name nvarchar(160),
			new_parentid uniqueidentifier,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			State int
				DEFAULT(0) NOT NULL
		)
		GO

		Create Table new_customopportunity
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			new_customopportunityid uniqueidentifier
				 NOT NULL,
			new_name nvarchar(160),
			new_customerid uniqueidentifier,
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			State int
				DEFAULT(0) NOT NULL
		)
		GO

		Create Table new_custom1
		(
			ID bigint
				PRIMARY KEY,
			OwnerId uniqueidentifier
				NOT NULL,
			EntityManagerOwningUser uniqueidentifier
				NOT NULL,
			new_custom1id uniqueidentifier
				 NOT NULL,
			new_name nvarchar(160),
			TimeStamp datetime
				DEFAULT(GETDATE()) NOT NULL,
			State int
				DEFAULT(0) NOT NULL,
			new_priority int
				DEFAULT(0) NOT NULL
		)
		GO
		Create Trigger TimeStampUpdatenew_custom1
			on new_custom1
		AFTER UPDATE AS
			UPDATE new_custom1 set TimeStamp=GetDate() WHERE ID in (select id from inserted)
		go
		
		Create table [dbo].[CRMMailbox]
		(
			[Id] [uniqueidentifier] NOT NULL,
			[UserId] [uniqueidentifier] NOT NULL,
			[OrgName] [nvarchar](100) NOT NULL,
			[EmailAddress] [nvarchar](200) NOT NULL,
			[Password] [nvarchar](100) NOT NULL,
			[EndPoint] [nvarchar](200) NULL,
			[IsActive] [bit] NULL,
			[useAutoDiscover] [bit] NULL,
			[UseSSL] [bit] NULL,
			[IsUser] [bit] NULL
		) ON [PRIMARY]
		GO

		Create table [dbo].[CRMContact]
		(
			[Id] [uniqueidentifier] NOT NULL,
			[OrgName] [nvarchar](100) NOT NULL,
			[EmailAddress] [nvarchar](200) NOT NULL,
			[Password] [nvarchar](100) NOT NULL,
			[EndPoint] [nvarchar](200) NULL,
			[IsActiveUser] [bit] NULL,
			[useAutoDiscover] [bit] NULL,
			[UseSSL] [bit] NULL
		) ON [PRIMARY]
 		GO

GO

CREATE TABLE [dbo].[Adx_communityforum](
	[Adx_communityforumId] [uniqueidentifier] NOT NULL,
	[Adx_name] [nvarchar](100) NULL,
	[Adx_Description] [nvarchar](max) NULL,
	[Adx_DisplayOrder] [int] NULL,
	[Adx_PartialUrl] [nvarchar](100) NULL,
	[Adx_PostCount] [int] NULL,
	[Adx_ThreadCount] [int] NULL,
	[adx_publishingstateid] [uniqueidentifier] NULL,
	[ID] [bigint] NOT NULL,
	[EntityManagerOwningUser] [uniqueidentifier] NULL,
	[ownerid] [uniqueidentifier] NULL,
	[Adx_websiteid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Adx_communityforum] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Adx_communityforumpost](
	[Adx_communityforumpostId] [uniqueidentifier] NOT NULL,
	[Adx_name] [nvarchar](100) NULL,
	[Adx_Content] [nvarchar](max) NULL,
	[adx_regardingid] [uniqueidentifier] NULL,
	[adx_forumthreadid] [uniqueidentifier] NULL,
	[adx_publishingstateid] [uniqueidentifier] NULL,
	[ID] [bigint] NOT NULL,
	[EntityManagerOwningUser] [uniqueidentifier] NULL,
	[ownerid] [uniqueidentifier] NULL,
    [state] [int] NULL
 CONSTRAINT [PK_Adx_communityforumpost] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Adx_communityforumthread](
	[Adx_communityforumthreadId] [uniqueidentifier] NOT NULL,
	[Adx_name] [nvarchar](100) NULL,
	[Adx_PostCount] [int] NULL,
	[adx_forumid] [uniqueidentifier] NULL,
	[adx_publishingstateid] [uniqueidentifier] NULL,
	[ID] [bigint] NOT NULL,
	[EntityManagerOwningUser] [uniqueidentifier] NULL,
	[ownerid] [uniqueidentifier] NULL,
	[state] [int] NULL
 CONSTRAINT [PK_Adx_communityforumthread] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [EntityManager{ORGNAME}]
GO

CREATE TABLE [dbo].[KbArticle](
	[KbArticleId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NULL,
	[Number] [nvarchar](100) NULL,
	[Content] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Comments] [nvarchar](max) NULL,
	[StateCode] [int] NOT NULL,
	[StatusCode] [int] NULL,
	[id] [bigint] NOT NULL,
	[OrganizationID] [uniqueidentifier] NULL,
	[KeyWords] [nvarchar](max) NULL,
	[msa_publishtoweb] [bit] NULL,
	[EntityManagerOwningUser] [uniqueidentifier] NULL,
 CONSTRAINT [PK_KbArticleBase] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[Adx_website](
	[Adx_websiteId] [uniqueidentifier] NULL,
	[Adx_name] [nvarchar](100) NULL,
	[OwnerId] [uniqueidentifier] NULL,
	[ID] [bigint] NOT NULL,
	[EntityManagerOwningUser] [uniqueidentifier] NULL,
	[organizationid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Adx_website] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Adx_PortalComment](
	[ActivityId] [uniqueidentifier] NULL,
	[OwnerId] [uniqueidentifier] NULL,
	[ID] [bigint] NOT NULL,
	[EntityManagerOwningUser] [uniqueidentifier] NULL,
	[RegardingObjectID] [uniqueidentifier] NULL,
	[State] [int] NULL,
 CONSTRAINT [PK_Adx_PortalComment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[UserList](
       [Username] [nvarchar](200) NULL,
       [Password] [nvarchar](50) NULL,    
	   	[CrmOwinAuth] [nvarchar](max) NULL,
		[SystemUserId] [uniqueidentifier] NULL,
	[ApplicationId] [uniqueidentifier] NULL,
	[OrganizationId] [uniqueidentifier] NULL
)

GO

CREATE TABLE [dbo].[PortalUsers](
	[contactId] [uniqueidentifier] NOT NULL,
	[loginname] [nvarchar](160) NULL,
	[createdon] [datetime] NULL
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[ltaccount](
	[accountId] [uniqueidentifier] NULL,	
	[systemuserId] [uniqueidentifier] NULL,
	[state] [int] NULL,
	[inuse] [int] NULL,
	[uid] [uniqueidentifier] NULL
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ltcontact](
	[contactId] [uniqueidentifier] NULL,	
	[systemuserId] [uniqueidentifier] NULL,
	[state] [int] NULL,
	[inuse] [int] NULL,
	[uid] [uniqueidentifier] NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ltlead](
	[leadId] [uniqueidentifier] NULL,	
	[systemuserId] [uniqueidentifier] NULL,
	[state] [int] NULL,
	[inuse] [int] NULL,
	[uid] [uniqueidentifier] NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ltcase](
	[incidentId] [uniqueidentifier] NULL,	
	[systemuserId] [uniqueidentifier] NULL,
	[state] [int] NULL,
	[inuse] [int] NULL,
	[uid] [uniqueidentifier] NULL
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ltopportunity](
	[opportunityId] [uniqueidentifier] NULL,	
	[accountId] [uniqueidentifier] NULL,	
	[contactId] [uniqueidentifier] NULL,	
	[systemuserId] [uniqueidentifier] NULL,
	[state] [int] NULL,
	[inuse] [int] NULL,
	[uid] [uniqueidentifier] NULL
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[vwUserList]
AS
SELECT        Username, Password
FROM            dbo.UserList
GO

CREATE VIEW [dbo].[vwAppUserList]
AS
SELECT        Username, Password, ApplicationId
FROM            dbo.UserList
GO

CREATE TRIGGER [dbo].[TRG_UpdateUserNameWithAppID]
   ON  [dbo].[SystemUser]
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	IF (update(UserPassword) AND not(update(ServerBaseUrl)) AND not(update(DomainName)))
	BEGIN
		UPDATE [dbo].[SystemUser]
		SET DomainName = ul.ApplicationId
		from inserted i inner join dbo.UserList ul 
			on i.DomainName = ul.Username
			where SystemUser.SystemUserId = i.SystemUserId AND 
			ul.ApplicationId is not null
	END
END
GO


ALTER TABLE [dbo].[SystemUser] ENABLE TRIGGER [TRG_UpdateUserNameWithAppID]
GO

