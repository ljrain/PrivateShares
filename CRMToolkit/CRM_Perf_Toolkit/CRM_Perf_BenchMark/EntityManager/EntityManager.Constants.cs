using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace CRM_Perf_BenchMark
{
    
    /// <summary>
    /// List of enty names. The naming scheme corresponds to the CRM 2.0 database table naming scheme.
    /// </summary>
    public static class EntityNames
    {
        public const string Users = "SystemUser";
        public const string Leads = "Lead";
        public const string Opportunities = "Opportunity";
        public const string Accounts = "Account";
        public const string Appointments = "Appointment";
        public const string ExchangeContact = "ExchangeContact";
        public const string ExchangeTask = "ExchangeTask";
        public const string ExchangeAppointment = "ExchangeAppointment";
        public const string ExchangeRecurringAppointment = "ExchangeRecurringAppointmentMaster";
        public const string RecurringAppointments = "RecurringAppointmentMaster";
        public const string Emails = "Email";
        public const string Phonecalls = "Phonecall";
        public const string Letters = "Letter";
        public const string Faxes = "Fax";
        public const string Contacts = "Contact";
        public const string Goals = "Goal";
        public const string GenericActivities = "GenericActivities";
        public const string Service = "Service";
        public const string Subject = "Subject";
        public const string Tasks = "Task";
        public const string Quotes = "Quote";
        public const string Templates = "Template";
        public const string Roles = "Role";
        public const string Incidents = "Incident";
        public const string Notes = "Annotation";
        public const string PriceLevels = "PriceLevel";
        public const string Lists = "List";
        public const string OwningBusinessUnit = "OwningBusinessUnit";
        public const string BusinessUnits = "BusinessUnit";
        public const string Campaigns = "Campaign";
        public const string ServiceAppointments = "ServiceAppointment";
        public const string Privileges = "Privilege";
        public const string Organizations = "Organization";
        public const string Equipment = "Equipment";
        public const string Team = "Team";
        public const string SavedQueryVisualizations = "SavedQueryVisualization";
        public const string SpSite = "SharePointSite";
        public const string SharePointDocumentLocation = "SharePointDocumentLocation";
        public const string Solution = "Solution";
        public const string Reports = "Report";
        public const string New_Simple_CustomAccounts = "New_Simple_CustomAccount";
        public const string TransactionCurrency = "TransactionCurrency";
        public const string ConnectionRole = "ConnectionRole";
        public const string Queue = "Queue";
        public const string Workflow = "Workflow";
        public const string New_IM = "new_IM";
        public const string New_Prospect = "new_Prospect";
        public const string Competitor = "Competitor";
        public const string Contracts = "Contract";
        public const string Products = "Product";
        public const string SalesOrders = "SalesOrder";
        public const string Invoice = "Invoice";
        public const string QuoteDetails = "QuoteDetail";
        public const string InvoiceDetails = "InvoiceDetail";
        public const string CampaignActivities = "Campaignactivity";
        public const string CampaignResponse = "CampaignResponse";
        public const string UoM = "UoM";
        public const string SalesOrderDetails = "SalesOrderDetail";
        public const string Connections = "Connection";
        public const string ProductPriceLevels = "ProductPriceLevel";
        public const string SLA = "SLA";
        public const string SocialInsightsConfiguration = "SocialInsightsConfiguration";
        public const string Resource = "Resource";
        public const string OpportunityProduct = "OpportunityProduct";
        public const string New_CustomAccount = "new_customaccount";
        public const string New_CustomOpportunity = "new_customopportunity";
        public const string Entitlements = "Entitlement";
        public const string HostedApplication = "UII_hostedapplication";
        public const string KnowledgeArticle = "KnowledgeArticle";
        public const string KbArticle = "KbArticle";
        public const string BookableResource = "BookableResource";
        public const string ForumBase = "adx_communityforum";
        public const string ForumPostBase = "adx_communityforumpost";
        public const string ForumThreadBase = "adx_communityforumthread";
        public const string ADXWebsite = "Adx_website";
        public const string New_Custom1 = "new_custom1";
        public static string AdxComment = "adx_portalcomment";
        public const string BookableResourceCategory = "BookableResourceCategory";
        public const string Characteristic = "Characteristic";
        public const string RatingValue = "RatingValue";
        public const string TeamMembership = "TeamMembership";
       
        #region F1 Custom
        //F1 custom
        public const string F1_agreement = "msdyn_agreement";
        public const string F1_agreementincident = "msdyn_agreementincident";
        public const string F1_agreementinvoicedate = "msdyn_agreementinvoicedate";
        public const string F1_agreementinvoiceproduct = "msdyn_agreementinvoiceproduct";
        public const string F1_agreementinvoicesetup = "msdyn_agreementinvoicesetup";
        public const string F1_agreementproduct = "msdyn_agreementproduct";
        public const string F1_agreementscheduledate = "msdyn_agreementscheduledate";
        public const string F1_agreementschedulesetup = "msdyn_agreementschedulesetup";
        public const string F1_agreementservice = "msdyn_agreementservice";
        public const string F1_agreementservicetask = "msdyn_agreementservicetask";
        public const string F1_agreementsubstatus = "msdyn_agreementsubstatus";
        public const string F1_customerequipment = "msdyn_customerasset";
        public const string F1_fieldonepricelistitem = "msdyn_fieldonepricelistitem";
        public const string F1_fieldonesetting = "msdyn_fieldonesetting";
        public const string F1_fieldonesystemjob = "msdyn_fieldonesystemjob";
        public const string F1_glaccount = "msdyn_glaccount";
        public const string F1_incidentproduct = "msdyn_incidentproduct";
        public const string F1_incidentservice = "msdyn_incidentservice";
        public const string F1_incidentservicetask = "msdyn_incidentservicetask";
        public const string F1_incidentskill = "msdyn_incidentskill";
        public const string F1_incidenttype = "msdyn_incidenttype";
        public const string F1_inventoryadjstproduct = "msdyn_inventoryadjstproduct";
        public const string F1_inventoryadjustment = "msdyn_inventoryadjustment";
        public const string F1_inventoryjournal = "msdyn_inventoryjournal";
        public const string F1_inventorytransfer = "msdyn_inventorytransfer";
        public const string F1_ivractivity = "msdyn_ivractivity";
        public const string F1_payment = "msdyn_payment";
        public const string F1_paymentdetails = "msdyn_paymentdetails";
        public const string F1_paymentmethod = "msdyn_paymentmethod";
        public const string F1_pobill = "msdyn_pobill";
        public const string F1_poolwogeneration = "msdyn_poolwogeneration";
        public const string F1_poreceipt = "msdyn_poreceipt";
        public const string F1_poreceiptproduct = "msdyn_poreceiptproduct";
        public const string F1_posubstatus = "msdyn_posubstatus";
        public const string F1_potype = "msdyn_potype";
        public const string F1_priority = "msdyn_priority";
        public const string F1_productinventory = "msdyn_productinventory";
        public const string F1_purchaseorder = "msdyn_purchaseorder";
        public const string F1_purchaseorderproduct = "msdyn_purchaseorderproduct";
        public const string F1_resourcecategory = "msdyn_resourcecategory";
        public const string F1_resourcegroupmember = "msdyn_resourcegroupmember";
        public const string F1_resourcepaytype = "msdyn_resourcepaytype";
        public const string F1_resourceskill = "msdyn_resourceskill";
        public const string F1_resourceterritory = "msdyn_resourceterritory";
        public const string F1_rma = "msdyn_rma";
        public const string F1_rmaproduct = "msdyn_rmaproduct";
        public const string F1_rmaproductreason = "msdyn_rmaproductreason";
        public const string F1_rmareceipt = "msdyn_rmareceipt";
        public const string F1_rmareceiptproduct = "msdyn_rmareceiptproduct";
        public const string F1_rmasubstatus = "msdyn_rmasubstatus";
        public const string F1_rmatype = "msdyn_rmatype";
        public const string F1_routingoptimizationrequest = "msdyn_routingoptimizationrequest";
        public const string F1_rtv = "msdyn_rtv";
        public const string F1_rtvproduct = "msdyn_rtvproduct";
        public const string F1_rtvproductreason = "msdyn_rtvproductreason";
        public const string F1_rtvsubstatus = "msdyn_rtvsubstatus";
        public const string F1_rtvtype = "msdyn_rtvtype";
        public const string F1_scheduleboardsettings = "msdyn_scheduleboardsettings";
        public const string F1_scheduletimestamp = "msdyn_scheduletimestamp";
        public const string F1_schedulingrule = "msdyn_schedulingrule";
        public const string F1_servicetasktype = "msdyn_servicetasktype";
        public const string F1_skilltype = "msdyn_skilltype";
        public const string F1_smsactivity = "msdyn_smsactivity";
        public const string F1_systemuserschedulersettings = "msdyn_systemuserschedulersettings";
        public const string F1_taxcode = "msdyn_taxcode";
        public const string F1_taxcodedetail = "msdyn_taxcodedetail";
        public const string F1_timeoffentity = "msdyn_timeoffentity";
        public const string F1_timeoffreason = "msdyn_timeoffreason";
        public const string F1_warehouse = "msdyn_warehouse";
        public const string F1_workorder = "msdyn_workorder";
        public const string F1_workorderincident = "msdyn_workorderincident";
        public const string F1_workorderproduct = "msdyn_workorderproduct";
        public const string F1_workorderresource = "msdyn_workorderresource";
        public const string F1_workorderschedulechange = "msdyn_workorderschedulechange";
        public const string F1_workorderschedulejournal = "msdyn_workorderschedulejournal";
        public const string F1_workorderschedulestatus = "msdyn_workorderschedulestatus";
        public const string F1_workorderservice = "msdyn_workorderservice";
        public const string F1_workorderservicetask = "msdyn_workorderservicetask";
        public const string F1_workorderskill = "msdyn_workorderskill";
        public const string F1_workorderstatus = "msdyn_workorderstatus";
        public const string F1_workordertype = "msdyn_workordertype";
        public const string F1_zipcode = "msdyn_zipcode";
        public const string F1_bookableresourcebooking = "bookableresourcebooking";
        public const string F1_bookingstatus = "bookingstatus";

        public const string Msdyn_accountpricelist = "msdyn_accountpricelist";
        public const string Msdyn_actual = "msdyn_actual";
        public const string Msdyn_approval = "msdyn_approval";
        public const string Msdyn_assignment = "msdyn_assignment";
        public const string Msdyn_batchjob = "msdyn_batchjob";
        public const string Msdyn_characteristicreqforteammember = "msdyn_characteristicreqforteammember";
        public const string Msdyn_contactpricelist = "msdyn_contactpricelist";
        public const string Msdyn_contractlineinvoiceschedule = "msdyn_contractlineinvoiceschedule";
        public const string Msdyn_contractlinescheduleofvalue = "msdyn_contractlinescheduleofvalue";
        public const string Msdyn_dataexport = "msdyn_dataexport";
        public const string Msdyn_equipmentpricelevel = "msdyn_equipmentpricelevel";
        public const string Msdyn_estimate = "msdyn_estimate";
        public const string Msdyn_estimateline = "msdyn_estimateline";
        public const string Msdyn_expense = "msdyn_expense";
        public const string Msdyn_expensecategory = "msdyn_expensecategory";
        public const string Msdyn_expensereceipt = "msdyn_expensereceipt";
        public const string Msdyn_fact = "msdyn_fact";
        public const string Msdyn_fieldcomputation = "msdyn_fieldcomputation";
        public const string Msdyn_findworkevent = "msdyn_findworkevent";
        public const string Msdyn_invoicefrequency = "msdyn_invoicefrequency";
        public const string Msdyn_invoicefrequencydetail = "msdyn_invoicefrequencydetail";
        public const string Msdyn_invoicelinetransaction = "msdyn_invoicelinetransaction";
        public const string Msdyn_journal = "msdyn_journal";
        public const string Msdyn_journalline = "msdyn_journalline";
        public const string Msdyn_mlresultcache = "msdyn_mlresultcache";
        public const string Msdyn_msdyn_datamanager_booking = "msdyn_msdyn_datamanager_booking";
        public const string Msdyn_opportunitylineequipment = "msdyn_opportunitylineequipment";
        public const string Msdyn_opportunitylineresourcecategory = "msdyn_opportunitylineresourcecategory";
        public const string Msdyn_opportunitylinetransaction = "msdyn_opportunitylinetransaction";
        public const string Msdyn_opportunitylinetransactioncategory = "msdyn_opportunitylinetransactioncategory";
        public const string Msdyn_opportunitylinetransactionclassificatio = "msdyn_opportunitylinetransactionclassificatio";
        public const string Msdyn_opportunitylineuser = "msdyn_opportunitylineuser";
        public const string Msdyn_opportunitypricelist = "msdyn_opportunitypricelist";
        public const string Msdyn_orderlineequipment = "msdyn_orderlineequipment";
        public const string Msdyn_orderlineresourcecategory = "msdyn_orderlineresourcecategory";
        public const string Msdyn_orderlinetransaction = "msdyn_orderlinetransaction";
        public const string Msdyn_orderlinetransactioncategory = "msdyn_orderlinetransactioncategory";
        public const string Msdyn_orderlinetransactionclassification = "msdyn_orderlinetransactionclassification";
        public const string Msdyn_orderlineuser = "msdyn_orderlineuser";
        public const string Msdyn_orderpricelist = "msdyn_orderpricelist";
        public const string Msdyn_organizationalunit = "msdyn_organizationalunit";
        public const string Msdyn_organizationalunit_pricelevel = "msdyn_organizationalunit_pricelevel";
        public const string Msdyn_postalbum = "msdyn_postalbum";
        public const string Msdyn_postconfig = "msdyn_postconfig";
        public const string Msdyn_postruleconfig = "msdyn_postruleconfig";
        public const string Msdyn_processnotes = "msdyn_processnotes";
        public const string Msdyn_project = "msdyn_project";
        public const string Msdyn_projectapproval = "msdyn_projectapproval";
        public const string Msdyn_projectintegrationheader = "msdyn_projectintegrationheader";
        public const string Msdyn_projectintegrationline = "msdyn_projectintegrationline";
        public const string Msdyn_projectparameter = "msdyn_projectparameter";
        public const string Msdyn_projectparameterpricelist = "msdyn_projectparameterpricelist";
        public const string Msdyn_projectpricelist = "msdyn_projectpricelist";
        public const string Msdyn_projecttask = "msdyn_projecttask";
        public const string Msdyn_projecttaskdependency = "msdyn_projecttaskdependency";
        public const string Msdyn_projecttaskstatususer = "msdyn_projecttaskstatususer";
        public const string Msdyn_projectteam = "msdyn_projectteam";
        public const string Msdyn_projectteammembersignup = "msdyn_projectteammembersignup";
        public const string Msdyn_projecttransactioncategory = "msdyn_projecttransactioncategory";
        public const string Msdyn_quotelineanalyticsbreakdown = "msdyn_quotelineanalyticsbreakdown";
        public const string Msdyn_quotelineequipment = "msdyn_quotelineequipment";
        public const string Msdyn_quotelineinvoiceschedule = "msdyn_quotelineinvoiceschedule";
        public const string Msdyn_quotelineresourcecategory = "msdyn_quotelineresourcecategory";
        public const string Msdyn_quotelinescheduleofvalue = "msdyn_quotelinescheduleofvalue";
        public const string Msdyn_quotelinetransaction = "msdyn_quotelinetransaction";
        public const string Msdyn_quotelinetransactioncategory = "msdyn_quotelinetransactioncategory";
        public const string Msdyn_quotelinetransactionclassification = "msdyn_quotelinetransactionclassification";
        public const string Msdyn_quotelineuser = "msdyn_quotelineuser";
        public const string Msdyn_quotepricelist = "msdyn_quotepricelist";
        public const string Msdyn_resourceassignment = "msdyn_resourceassignment";
        public const string Msdyn_resourceassignmentdetail = "msdyn_resourceassignmentdetail";
        public const string Msdyn_resourcebooking = "msdyn_resourcebooking";
        public const string Msdyn_resourcebooking_resourcerequirement = "msdyn_resourcebooking_resourcerequirement";
        public const string Msdyn_resourcebookingdetail = "msdyn_resourcebookingdetail";
        public const string Msdyn_resourcecategorypricelevel = "msdyn_resourcecategorypricelevel";
        public const string Msdyn_resourcerequest = "msdyn_resourcerequest";
        public const string Msdyn_resourcerequirement = "msdyn_resourcerequirement";
        public const string Msdyn_resourcerequirement_bookableresource = "msdyn_resourcerequirement_bookableresource";
        public const string Msdyn_resourcerequirement_bookingheader = "msdyn_resourcerequirement_bookingheader";
        public const string Msdyn_resourcerequirement_organizationunit = "msdyn_resourcerequirement_organizationunit";
        public const string Msdyn_resourcerequirement_systemuser = "msdyn_resourcerequirement_systemuser";
        public const string Msdyn_resourcerequirementdetail = "msdyn_resourcerequirementdetail";
        public const string Msdyn_reviewprocesssetup = "msdyn_reviewprocesssetup";
        public const string Msdyn_rolecompetencyrequirement = "msdyn_rolecompetencyrequirement";
        public const string Msdyn_roleutilization = "msdyn_roleutilization";
        public const string Msdyn_timedimension = "msdyn_timedimension";
        public const string Msdyn_timeentry = "msdyn_timeentry";
        public const string Msdyn_timeoffcalendar = "msdyn_timeoffcalendar";
        public const string Msdyn_transactioncategory = "msdyn_transactioncategory";
        public const string Msdyn_transactioncategoryclassification = "msdyn_transactioncategoryclassification";
        public const string Msdyn_transactioncategoryhierarchyelement = "msdyn_transactioncategoryhierarchyelement";
        public const string Msdyn_transactioncategorypricelevel = "msdyn_transactioncategorypricelevel";
        public const string Msdyn_transactionconnection = "msdyn_transactionconnection";
        public const string Msdyn_transactionorigin = "msdyn_transactionorigin";
        public const string Msdyn_transactiontype = "msdyn_transactiontype";
        public const string Msdyn_userpricelevel = "msdyn_userpricelevel";
        public const string Msdyn_userworkhistory = "msdyn_userworkhistory";
        public const string Msdyn_wallsavedquery = "msdyn_wallsavedquery";
        public const string Msdyn_wallsavedqueryusersettings = "msdyn_wallsavedqueryusersettings";
        public const string Msdyn_workhourtemplate = "msdyn_workhourtemplate";
        //end custom 
        #endregion

        #region ARE custom
        public const string Territory = "territory";
        #endregion
    }

    /// <summary>
    /// Names of the primary key for each entity type.
    /// </summary>
    public static class EntityIDNames
    {
        public const string User = "SystemUserId";
        public const string Lead = "LeadId";
        public const string Opportunities = "OpportunityId";
        public const string Account = "AccountId";
        public const string Appointment = "ActivityId";
        public const string ExchangeSync = "ObjectId";
        public const string ExchangeTask = "ExchangeId";
        public const string RecurringAppointmentMaster = "ActivityId";
        public const string Phonecall = "ActivityId";
        public const string Fax = "ActivityId";
        public const string Email = "ActivityId";
        public const string Letter = "ActivityId";
        public const string Contact = "ContactId";
        public const string Task = "ActivityId";
        public const string Service = "ServiceId";
        public const string Subject = "SubjectId";
        public const string Quote = "QuoteId";
        public const string Template = "TemplateId";
        public const string Role = "RoleId";
        public const string Incident = "IncidentId";
        public const string KnowledgeArticle = "KnowledgeArticleId";
        public const string Note = "AnnotationId";
        public const string PriceLevel = "PriceLevelId";
        public const string List = "ListId";
        public const string OwningBusinessUnit = "BusinessUnitId";
        public const string BusinessUnit = "BusinessUnitId";
        public const string Campaign = "CampaignId";
        public const string ServiceAppointment = "ActivityId";
        public const string Privilege = "Name";
        public const string Organization = "OrganizationId";
        public const string Goal = "GoalId";
        public const string Equipment = "EquipmentId";
        public const string Team = "TeamId";
        public const string SavedQueryVisualizationId = "SavedQueryVisualizationId";
        public const string SpSite = "SharePointSiteId";
        public const string SharePointDocumentLocation = "SharePointDocumentLocationId";
        public const string Solution = "SolutionId";
        public const string Report = "ReportId";
        public const string TransactionCurrency = "TransactionCurrencyId";
        public const string ConnectionRole = "ConnectionRoleId";
        public const string Queue = "QueueId";
        public const string Workflow = "WorkflowId";
        public const string New_IM = "ActivityId";
        public const string New_CustomAccount = "new_customaccountId";
        public const string New_CustomOpportunity = "new_customopportunityId";
        public const string New_Prospect = "new_ProspectId";
        public const string Competitor = "CompetitorId";
        public const string Contracts = "ContractId";
        public const string Products = "ProductId";
        public const string SalesOrders = "SalesOrderId";
        public const string Invoice = "InvoiceId";
        public const string QuoteDetails = "QuoteDetailId";
        public const string InvoiceDetails = "InvoiceDetailId";
        public const string CampaignActivities = "CampaignactivityId";
        public const string CampaignResponse = "CampaignResponseId";
        public const string UoM = "UoMId";
        public const string SalesOrderDetails = "SalesOrderDetailId";
        public const string Connections = "ConnectionId";
        public const string ProductPriceLevels = "ProductPriceLevelId";
        public const string SLA = "SlaId";
        public const string Resource = "ResourceId";
        public const string OpportunityProduct = "OpportunityProduct";
        public const string HostedApplication = "UII_hostedapplicationId";
        public const string KbArticle = "KbArticleId";
        public const string BookableResource = "BookableResourceId";
        public const string ForumBase = "adx_communityforumid";
        public const string ForumPostBase = "adx_communityforumpostid";
        public const string ForumThreadBase = "adx_communityforumthreadid";
        public const string ADXWebsite = "Adx_websiteId";
        public const string New_Custom1 = "New_Custom1Id";
        public const string AdxComment = "ActivityId";
        public const string BookableResourceCategory = "BookableResourceCategoryId";
        public const string Characteristic = "CharacteristicId";
        public const string RatingValue = "RatingValueId";
        public const string TeamMembership = "TeamMembershipId";
        
        #region F1 Custom
        public const string F1_agreement = "msdyn_agreementId";
        public const string F1_agreementincident = "msdyn_agreementincidentId";
        public const string F1_agreementinvoicedate = "msdyn_agreementinvoicedateId";
        public const string F1_agreementinvoiceproduct = "msdyn_agreementinvoiceproductId";
        public const string F1_agreementinvoicesetup = "msdyn_agreementinvoicesetupId";
        public const string F1_agreementproduct = "msdyn_agreementproductId";
        public const string F1_agreementscheduledate = "msdyn_agreementscheduledateId";
        public const string F1_agreementschedulesetup = "msdyn_agreementschedulesetupId";
        public const string F1_agreementservice = "msdyn_agreementserviceId";
        public const string F1_agreementservicetask = "msdyn_agreementservicetaskId";
        public const string F1_agreementsubstatus = "msdyn_agreementsubstatusId";
        public const string F1_customerequipment = "msdyn_customerassetId";
        public const string F1_fieldonepricelistitem = "msdyn_fieldonepricelistitemId";
        public const string F1_fieldonesetting = "msdyn_fieldonesettingId";
        public const string F1_fieldonesystemjob = "msdyn_fieldonesystemjobId";
        public const string F1_glaccount = "msdyn_glaccountId";
        public const string F1_incidentproduct = "msdyn_incidentproductId";
        public const string F1_incidentservice = "msdyn_incidentserviceId";
        public const string F1_incidentservicetask = "msdyn_incidentservicetaskId";
        public const string F1_incidentskill = "msdyn_incidentskillId";
        public const string F1_incidenttype = "msdyn_incidenttypeId";
        public const string F1_inventoryadjstproduct = "msdyn_inventoryadjstproductId";
        public const string F1_inventoryadjustment = "msdyn_inventoryadjustmentId";
        public const string F1_inventoryjournal = "msdyn_inventoryjournalId";
        public const string F1_inventorytransfer = "msdyn_inventorytransferId";
        public const string F1_ivractivity = "msdyn_ivractivityId";
        public const string F1_payment = "msdyn_paymentId";
        public const string F1_paymentdetails = "msdyn_paymentdetailsId";
        public const string F1_paymentmethod = "msdyn_paymentmethodId";
        public const string F1_pobill = "msdyn_pobillId";
        public const string F1_poolwogeneration = "msdyn_poolwogenerationId";
        public const string F1_poreceipt = "msdyn_poreceiptId";
        public const string F1_poreceiptproduct = "msdyn_poreceiptproductId";
        public const string F1_posubstatus = "msdyn_posubstatusId";
        public const string F1_potype = "msdyn_potypeId";
        public const string F1_priority = "msdyn_priorityId";
        public const string F1_productinventory = "msdyn_productinventoryId";
        public const string F1_purchaseorder = "msdyn_purchaseorderId";
        public const string F1_purchaseorderproduct = "msdyn_purchaseorderproductId";
        public const string F1_resourcecategory = "msdyn_resourcecategoryId";
        public const string F1_resourcegroupmember = "msdyn_resourcegroupmemberId";
        public const string F1_resourcepaytype = "msdyn_resourcepaytypeId";
        public const string F1_resourceskill = "msdyn_resourceskillId";
        public const string F1_resourceterritory = "msdyn_resourceterritoryId";
        public const string F1_rma = "msdyn_rmaId";
        public const string F1_rmaproduct = "msdyn_rmaproductId";
        public const string F1_rmaproductreason = "msdyn_rmaproductreasonId";
        public const string F1_rmareceipt = "msdyn_rmareceiptId";
        public const string F1_rmareceiptproduct = "msdyn_rmareceiptproductId";
        public const string F1_rmasubstatus = "msdyn_rmasubstatusId";
        public const string F1_rmatype = "msdyn_rmatypeId";
        public const string F1_routingoptimizationrequest = "msdyn_routingoptimizationrequestId";
        public const string F1_rtv = "msdyn_rtvId";
        public const string F1_rtvproduct = "msdyn_rtvproductId";
        public const string F1_rtvproductreason = "msdyn_rtvproductreasonId";
        public const string F1_rtvsubstatus = "msdyn_rtvsubstatusId";
        public const string F1_rtvtype = "msdyn_rtvtypeId";
        public const string F1_scheduleboardsettings = "msdyn_scheduleboardsettingsId";
        public const string F1_scheduletimestamp = "msdyn_scheduletimestampId";
        public const string F1_schedulingrule = "msdyn_schedulingruleId";
        public const string F1_servicetasktype = "msdyn_servicetasktypeId";
        public const string F1_skilltype = "msdyn_skilltypeId";
        public const string F1_smsactivity = "msdyn_smsactivityId";
        public const string F1_systemuserschedulersettings = "msdyn_systemuserschedulersettingsId";
        public const string F1_taxcode = "msdyn_taxcodeId";
        public const string F1_taxcodedetail = "msdyn_taxcodedetailId";
        public const string F1_timeoffentity = "msdyn_timeoffentityId";
        public const string F1_timeoffreason = "msdyn_timeoffreasonId";
        public const string F1_warehouse = "msdyn_warehouseId";
        public const string F1_workorder = "msdyn_workorderId";
        public const string F1_workorderincident = "msdyn_workorderincidentId";
        public const string F1_workorderproduct = "msdyn_workorderproductId";
        public const string F1_workorderresource = "msdyn_workorderresourceId";
        public const string F1_workorderschedulechange = "msdyn_workorderschedulechangeId";
        public const string F1_workorderschedulejournal = "msdyn_workorderschedulejournalId";
        public const string F1_workorderschedulestatus = "msdyn_workorderschedulestatusId";
        public const string F1_workorderservice = "msdyn_workorderserviceId";
        public const string F1_workorderservicetask = "msdyn_workorderservicetaskId";
        public const string F1_workorderskill = "msdyn_workorderskillId";
        public const string F1_workorderstatus = "msdyn_workorderstatusId";
        public const string F1_workordertype = "msdyn_workordertypeId";
        public const string F1_zipcode = "msdyn_zipcodeId";
        public const string F1_bookableresourcebooking = "bookableresourcebookingId";
        public const string F1_bookingstatus = "bookingstatusId";
        #endregion

        #region ARE Custom
        public const string Territory = "territoryid";
        #endregion

        #region PSA Custom
        public const string Msdyn_accountpricelist = "msdyn_accountpricelistid";
        public const string Msdyn_actual = "msdyn_actualid";
        public const string Msdyn_approval = "activityid";
        public const string Msdyn_assignment = "msdyn_assignmentid";
        public const string Msdyn_batchjob = "msdyn_batchjobid";
        public const string Msdyn_characteristicreqforteammember = "msdyn_characteristicreqforteammemberid";
        public const string Msdyn_contactpricelist = "msdyn_contactpricelistid";
        public const string Msdyn_contractlineinvoiceschedule = "msdyn_contractlineinvoicescheduleid";
        public const string Msdyn_contractlinescheduleofvalue = "msdyn_contractlinescheduleofvalueid";
        public const string Msdyn_dataexport = "msdyn_dataexportid";
        public const string Msdyn_equipmentpricelevel = "msdyn_equipmentpricelevelid";
        public const string Msdyn_estimate = "msdyn_estimateid";
        public const string Msdyn_estimateline = "msdyn_estimatelineid";
        public const string Msdyn_expense = "msdyn_expenseid";
        public const string Msdyn_expensecategory = "msdyn_expensecategoryid";
        public const string Msdyn_expensereceipt = "msdyn_expensereceiptid";
        public const string Msdyn_fact = "msdyn_factid";
        public const string Msdyn_fieldcomputation = "msdyn_fieldcomputationid";
        public const string Msdyn_findworkevent = "msdyn_findworkeventid";
        public const string Msdyn_invoicefrequency = "msdyn_invoicefrequencyid";
        public const string Msdyn_invoicefrequencydetail = "msdyn_invoicefrequencydetailid";
        public const string Msdyn_invoicelinetransaction = "msdyn_invoicelinetransactionid";
        public const string Msdyn_journal = "msdyn_journalid";
        public const string Msdyn_journalline = "msdyn_journallineid";
        public const string Msdyn_mlresultcache = "msdyn_mlresultcacheid";
        public const string Msdyn_msdyn_datamanager_booking = "msdyn_msdyn_datamanager_bookingid";
        public const string Msdyn_opportunitylineequipment = "msdyn_opportunitylineequipmentid";
        public const string Msdyn_opportunitylineresourcecategory = "msdyn_opportunitylineresourcecategoryid";
        public const string Msdyn_opportunitylinetransaction = "msdyn_opportunitylinetransactionid";
        public const string Msdyn_opportunitylinetransactioncategory = "msdyn_opportunitylinetransactioncategoryid";
        public const string Msdyn_opportunitylinetransactionclassificatio = "msdyn_opportunitylinetransactionclassificatioid";
        public const string Msdyn_opportunitylineuser = "msdyn_opportunitylineuserid";
        public const string Msdyn_opportunitypricelist = "msdyn_opportunitypricelistid";
        public const string Msdyn_orderlineequipment = "msdyn_orderlineequipmentid";
        public const string Msdyn_orderlineresourcecategory = "msdyn_orderlineresourcecategoryid";
        public const string Msdyn_orderlinetransaction = "msdyn_orderlinetransactionid";
        public const string Msdyn_orderlinetransactioncategory = "msdyn_orderlinetransactioncategoryid";
        public const string Msdyn_orderlinetransactionclassification = "msdyn_orderlinetransactionclassificationid";
        public const string Msdyn_orderlineuser = "msdyn_orderlineuserid";
        public const string Msdyn_orderpricelist = "msdyn_orderpricelistid";
        public const string Msdyn_organizationalunit = "msdyn_organizationalunitid";
        public const string Msdyn_organizationalunit_pricelevel = "msdyn_organizationalunit_pricelevelid";
        public const string Msdyn_postalbum = "msdyn_postalbumid";
        public const string Msdyn_postconfig = "msdyn_postconfigid";
        public const string Msdyn_postruleconfig = "msdyn_postruleconfigid";
        public const string Msdyn_processnotes = "msdyn_processnotesid";
        public const string Msdyn_project = "msdyn_projectid";
        public const string Msdyn_projectapproval = "msdyn_projectapprovalid";
        public const string Msdyn_projectintegrationheader = "msdyn_projectintegrationheaderid";
        public const string Msdyn_projectintegrationline = "msdyn_projectintegrationlineid";
        public const string Msdyn_projectparameter = "msdyn_projectparameterid";
        public const string Msdyn_projectparameterpricelist = "msdyn_projectparameterpricelistid";
        public const string Msdyn_projectpricelist = "msdyn_projectpricelistid";
        public const string Msdyn_projecttask = "msdyn_projecttaskid";
        public const string Msdyn_projecttaskdependency = "msdyn_projecttaskdependencyid";
        public const string Msdyn_projecttaskstatususer = "msdyn_projecttaskstatususerid";
        public const string Msdyn_projectteam = "msdyn_projectteamid";
        public const string Msdyn_projectteammembersignup = "msdyn_projectteammembersignupid";
        public const string Msdyn_projecttransactioncategory = "msdyn_projecttransactioncategoryid";
        public const string Msdyn_quotelineanalyticsbreakdown = "msdyn_quotelineanalyticsbreakdownid";
        public const string Msdyn_quotelineequipment = "msdyn_quotelineequipmentid";
        public const string Msdyn_quotelineinvoiceschedule = "msdyn_quotelineinvoicescheduleid";
        public const string Msdyn_quotelineresourcecategory = "msdyn_quotelineresourcecategoryid";
        public const string Msdyn_quotelinescheduleofvalue = "msdyn_quotelinescheduleofvalueid";
        public const string Msdyn_quotelinetransaction = "msdyn_quotelinetransactionid";
        public const string Msdyn_quotelinetransactioncategory = "msdyn_quotelinetransactioncategoryid";
        public const string Msdyn_quotelinetransactionclassification = "msdyn_quotelinetransactionclassificationid";
        public const string Msdyn_quotelineuser = "msdyn_quotelineuserid";
        public const string Msdyn_quotepricelist = "msdyn_quotepricelistid";
        public const string Msdyn_resourceassignment = "msdyn_resourceassignmentid";
        public const string Msdyn_resourceassignmentdetail = "msdyn_resourceassignmentdetailid";
        public const string Msdyn_resourcebooking = "msdyn_resourcebookingid";
        public const string Msdyn_resourcebooking_resourcerequirement = "msdyn_resourcebooking_resourcerequirementid";
        public const string Msdyn_resourcebookingdetail = "msdyn_resourcebookingdetailid";
        public const string Msdyn_resourcecategorypricelevel = "msdyn_resourcecategorypricelevelid";
        public const string Msdyn_resourcerequest = "msdyn_resourcerequestid";
        public const string Msdyn_resourcerequirement = "msdyn_resourcerequirementid";
        public const string Msdyn_resourcerequirement_bookableresource = "msdyn_resourcerequirement_bookableresourceid";
        public const string Msdyn_resourcerequirement_bookingheader = "msdyn_resourcerequirement_bookingheaderid";
        public const string Msdyn_resourcerequirement_organizationunit = "msdyn_resourcerequirement_organizationunitid";
        public const string Msdyn_resourcerequirement_systemuser = "msdyn_resourcerequirement_systemuserid";
        public const string Msdyn_resourcerequirementdetail = "msdyn_resourcerequirementdetailid";
        public const string Msdyn_reviewprocesssetup = "msdyn_reviewprocesssetupid";
        public const string Msdyn_rolecompetencyrequirement = "msdyn_rolecompetencyrequirementid";
        public const string Msdyn_roleutilization = "msdyn_roleutilizationid";
        public const string Msdyn_timedimension = "msdyn_timedimensionid";
        public const string Msdyn_timeentry = "msdyn_timeentryid";
        public const string Msdyn_timeoffcalendar = "msdyn_timeoffcalendarid";
        public const string Msdyn_transactioncategory = "msdyn_transactioncategoryid";
        public const string Msdyn_transactioncategoryclassification = "msdyn_transactioncategoryclassificationid";
        public const string Msdyn_transactioncategoryhierarchyelement = "msdyn_transactioncategoryhierarchyelementid";
        public const string Msdyn_transactioncategorypricelevel = "msdyn_transactioncategorypricelevelid";
        public const string Msdyn_transactionconnection = "msdyn_transactionconnectionid";
        public const string Msdyn_transactionorigin = "msdyn_transactionoriginid";
        public const string Msdyn_transactiontype = "msdyn_transactiontypeid";
        public const string Msdyn_userpricelevel = "msdyn_userpricelevelid";
        public const string Msdyn_userworkhistory = "msdyn_userworkhistoryid";
        public const string Msdyn_wallsavedquery = "msdyn_wallsavedqueryid";
        public const string Msdyn_wallsavedqueryusersettings = "msdyn_wallsavedqueryusersettingsid";
        public const string Msdyn_workhourtemplate = "msdyn_workhourtemplateid";
        #endregion


        public static string GetByEntityName(string entityName)
        {
            switch (entityName)
            {
                case EntityNames.Accounts:
                    return EntityIDNames.Account;
                case EntityNames.Opportunities:
                    return EntityIDNames.Opportunities;
                case EntityNames.Contacts:
                    return EntityIDNames.Contact;
                case EntityNames.Appointments:
                    return EntityIDNames.Appointment;
                case EntityNames.Phonecalls:
                    return EntityIDNames.Phonecall;
                case EntityNames.Tasks:
                    return EntityIDNames.Task;
                case EntityNames.Emails:
                    return EntityIDNames.Email;
                case EntityNames.Leads:
                    return EntityIDNames.Lead;
                case EntityNames.KnowledgeArticle:
                    return EntityNames.KnowledgeArticle;

                //f1 custom
                // The following is jsut for testing. not sure yet if this is needed at all
                case EntityNames.F1_potype:
                    return EntityIDNames.F1_potype;
                case EntityNames.F1_workorder:
                    return EntityIDNames.F1_workorder;
                //end custom

                //Project custom
                // The following is just for testing. not sure yet if this is needed at all
                case EntityNames.Msdyn_timeentry:
                    return EntityIDNames.Msdyn_timeentry;
                //end custom

            }

            // TODO: implement the rest
            throw new Exception(string.Format("Entity id attribute name was not defined for {0}.", entityName));
        }
    }

    /// <summary>
    /// List of view names
    /// </summary>
    public static class EntityViewNames
    {
        //activities
        public const string AllActivities = "AllActivitiesView";
        public const string MyActivities = "MyActivitiesView";
        public const string ClosedActivities = "ClosedActivitiesView";
        public const string AllTasks = "AllTasksView";
        public const string MyTasks = "MyTasksView";
        public const string AllApointments = "AllApointmentsView";
        public const string MyAppointments = "MyAppointmentsView";
        public const string MyCompletedAppointments = "MyCompletedAppointmentsView";
        public const string MyLetters = "MyLettersView";
        public const string MyFaxes = "MyFaxesView";
        public const string MyDraftEmails = "MyDraftEmailsView";
        public const string AllRecurringAppointments = "AllRecurringAppointmentsView";
        public const string OpenRecurringAppointments = "OpenRecurringAppointmentsView";
        public const string MyRecurringAppointments = "MyRecurringAppointmentsView";
        public const string ClosedRecurringAppointments = "ClosedRecurringAppointmentsView";
        public const string MyClosedRecurringAppointments = "MyClosedRecurringAppointmentsView";
        public const string ActivityAttachmentAssociated = "ActivityAttachmentAssociatedView";
        public const string AllPhoneCalls = "AllPhoneCallsView";
        public const string MyPhoneCalls = "MyPhoneCallsView";
        public const string AllServiceActivities = "AllServiceActivitiesView";
        public const string MyServiceActivities = "MyServiceActivitiesView";
        public const string AllNewIM = "AllNewIMView";
        public const string MyOpenNewIM = "MyOpenNewIMView";

        //sales entities
        public const string MyActiveAcounts = "MyActiveAccountsView";
        public const string ActiveAcounts = "ActiveAccountsView";
        public const string MyActiveContacts = "MyActiveContactsView";
        public const string ActiveContacts = "ActiveContactsView";
        public const string MyOpenOpportunities = "MyOpenOpportunitiesView";
        public const string ActiveContactsSubgrid = "ActiveContactsSubgridView";
        public const string AccountsNoOrdersInLast6Months = "AccountsNoOrdersInLast6MonthsView";
        public const string AccountsRespondedToCampaingsInLast6Months = "AccountsRespondedToCampaingsInLast6MonthsView";
        public const string AccountsNoCampaignActivitiesInLast3Months = "AccountsNoCampaignActivitiesInLast3MonthsView";
        public const string InActiveAccounts = "InActiveAccountsView";
        public const string MyOpenLeads = "MyOpenLeadsView";

        public const string OpenLeads = "OpenLeadsView";
        public const string ClosedLeads = "ClosedLeadsView";
        public const string LeadsOlderThan6Months = "LeadsOlderThan6MonthsView";
        public const string LeadsWithNoCampaignSent = "LeadsWithNoCampaignSentView";
        public const string NewLeadsLastWeek = "NewLeadsLastWeekView";
        public const string NewLeadsThisWeek = "NewLeadsThisWeekView";
        public const string ContactsNoOrdersinLast6Months = "ContactsNoOrdersinLast6MonthsView";
        public const string ContactsRespondedToCampignsIn6months = "ContactsRespondedToCampignsIn6monthsView";
        public const string ContactsNoCampaignSentInLast3Months = "ContactsNoCampaignSentInLast3MonthsView";
        public const string InActiveContacts = "InActiveContactsView";
        public const string ClosedOpportunities = "ClosedOpportunitiesView";
        public const string OpenOpportunities = "OpenOpportunitiesView";
        public const string OpportunitiesClosingNextMonth = "OpportunitiesClosingNextMonthView";
        public const string OpportunitiesOpenedLastWeek = "OpportunitiesOpenedLastWeekView";
        public const string OpportunitiesOpenedThisWeek = "OpportunitiesOpenedThisWeekView";
        public const string AllStakeholders = "AllStakeholdersView";
        public const string AllCompetitors = "AllCompetitorsView";
        public const string AllSalesTeamMembers = "AllSalesTeamMembersView";
        public const string AllOrderProducts = "AllOrderProductsView";
        public const string AllInvoiceProducts = "AllInvoiceProductsView";
        public const string AllLeads = "AllLeadsView";
        public const string OpportunityProductInlineEditView = "OpportunityProductInlineEditView";
        public const string OpportunityQuotesView = "OpportunityQuotesView";
        public const string SuggestionsGridQuery = "SuggestionsGridQueryView";
        public const string QuoteProductInlineEditView = "QuoteProductInlineEditView";
        public const string AllQuotes = "AllQuotesView";
        public const string OrderProductInlineEdit = "OrderProductInlineEditView";
        public const string MyOrders = "MyOrdersView";
        public const string ContactEntitlements = "ContactEntitlementsView";

        //marketing entities
        public const string ActiveMarketingLists = "ActiveMarketingListsView";
        public const string MyActiveMarketingLists = "MyActiveMarketingListsView";

        public const string CampaignActivities = "CampaignActivitiesView";
        public const string CampaignLeads = "CampaignLeadsView";
        public const string CampaignResponses = "CampaignResponsesView";
        public const string CampaignMarketingLists = "CampaignMarketingListsView";
        public const string AllCampaignResponses = "AllCampaignResponsesView";
        public const string MyCampaignsResponses = "MyCampaignsResponsesView";
        public const string MyCampaigns = "MyCampaignsView";
        public const string OpenCampaignResponses = "OpenCampaignResponsesView";
        public const string ListCampaigns = "ListCampaignsView";
        public const string MyQuickCampaigns = "MyQuickCampaignsView";
        public const string LeadAssociated = "LeadAssociatedView";
        public const string BasicMarketingList = "BasicMarketingListView";
        public const string MyActiveMarketingList = "MyActiveMarketingListView";
        public const string AllCampaignActivities = "AllCampaignActivitiesView";
        public const string LeadListMembers = "LeadListMembersView";
        public const string OpenPlanningActivityAssosiated = "OpenPlanningActivityAssosiatedView";
        public const string ContactListMembers = "ContactListMembersView";
        public const string AccountListMembers = "AccountListMembersView";

        //service entities
        public const string ActiveCases = "ActiveCasesView";
        public const string MyActiveCases = "MyActiveCasesView";
        public const string AllActivitiesforCustomer = "AllActivitiesforCustomerView";
        public const string SimilarResolvedCases = "SimilarResolvedCasesView";

        public const string RecentCasesSubgrid = "RecentCasesSubgridView";
        public const string RecentActivitiesSubgrid = "RecentActivitiesSubgridView";
        public const string CaseAccountSubgrid = "CaseSubgrid3";
        public const string CaseAccountSubgrid2 = "CaseSubgrid4";
        public const string RelatedSolutionsSubGrid = "RelatedSolutionsSubGridView";
        public const string CaseRefreshData1 = "CaseRefreshData1";
        public const string CaseRefreshData2 = "CaseRefreshData2";
        public const string AllCases = "AllCasesView";
        public const string EntitlementSubgrid = "EntitlementSubgridView";
        public const string AccountEntitlements = "AccountEntitlementsView";
        public const string AssosiatedCases = "AssosiatedCasesView";
        public const string AllKBArticles = "AllKBArticlesView";
        public const string SlaKpiInstancesList = "SlaKpiInstancesListView";

        //others
        public const string AllCasesforCustomer = "AllCasesforCustomerView";
        public const string RelatedSolutions = "RelatedSolutionsView";
        public const string UserLookup = "UserLookupView";
        public const string EnabledUsers = "EnabledUsersView";
        public const string MyTeams = "MyTeamsView";
        public const string AccountLookup = "AccountLookupView";
        public const string RecentCases = "RecentCasesView";
        public const string RecentOpportunities = "RecentOpportunitiesView";
        public const string TeamMembers = "TeamMembersView";
        public const string SharePointAssociated = "SharePointDocumentAssociatedView";

        #region F1 Custom
        public const string ActiveF1_PoTypes = "ActivePOTypesView";
        public const string ActiveF1_WorkOrders = "ActiveWorkOrdersView";
        public const string ActiveF1_WorkOrderResources = "ActiveWOSchedulesView";
        public const string ActiveF1_CustomerEquipment = "ActiveCustomerAssetView";
        public const string ActiveF1_BookableResourceBookings = "ActiveBookableResourceBookings";
        #endregion

        #region Project
        public const string ProjectContractandContractLinesSubgridView = "ProjectContractandContractLinesSubgridView";
        public const string ProjectQuoteandQuoteLinesSubgridView = "ProjectQuoteandQuoteLinesSubgridView";
        public const string OpportunitiesAndOpportuniityLinesSubgridView = "OpportunitiesAndOpportuniityLinesSubgridView";
        public const string ProjectTeamMembersSubgridView = "ProjectTeamMembersSubgridView";
        public const string SchedulePerformanceByRoleChart = "SchedulePerformanceByRoleChart";
        public const string CostPerformanceByRoleChart = "CostPerformanceByRoleChart";
        #endregion
    }

    /// <summary>
    /// List of OOB charts names
    /// </summary>
    public static class ChartNames
    {
        public const string SalesPipelineChart = "SalesPipelineChart";
        public const string LeadsbySourceCampaignChart = "LeadsbySourceCampaignChart";
        public const string CasesByPriorityPerDayChart = "CasesByPriorityPerDayChart";
        public const string ResolveCaseSatisfactionChart = "ResolveCaseSatisfactionChart";
        public const string ActivitiesbyOwnerandPriorityChart = "ActivitiesbyOwnerandPriorityChart";
        public const string ServiceLeaderboardChart = "ServiceLeaderboardChart";
        public const string CasesByOriginPerDay = "CasesByOriginPerDay";
        public const string ArticlesByStatusChart = "ArticlesByStatusChart";
        public const string CaseResolutionTrendByDayChart = "CaseResolutionTrendByDayChart";
        public const string ProgressAgainstCountBasedGoalsChart = "ProgressAgainstCountBasedGoalsChart";
        public const string CaseMixbyOriginChart = "CaseMixbyOriginChart";
        public const string CampaignBudgetvsActualCostsByFiscalChart = "CampaignBudgetvsActualCostsByFiscalChart";
        public const string RevenueGeneratedbyCampaignChart = "RevenueGeneratedbyCampaignChart";
        public const string CampaignTypeMixChart = "CampaignTypeMixChart";
        public const string LeadsbySourceChart = "LeadsbySourceChart";
        public const string Top10OpportunitiesChart = "Top10OpportunitiesChart";
        public const string Top10CustomersChart = "Top10CustomersChart";
        public const string PercentageAchievedAgainstGoalsChart = "PercentageAchievedAgainstGoalsChart";
        public const string SalesLeaderboardChart = "SalesLeaderboardChart";
        public const string SalesProgressagainstRevenueGoalsChart = "SalesProgressagainstRevenueGoalsChart";
        public const string DealsWonvsDealsLostByFiscalPeriodChart = "DealsWonvsDealsLostByFiscalPeriodChart";
    }
}

