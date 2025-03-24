set nocount on
SELECT '
	<crmserver>
			<serverbaseurl value="https://'+UniqueName+'.crmpf2.crmliveperf.com" />
			<organizationbaseurl value="https://'+UniqueName+'.crmpf2.crmliveperf.com" />
			<organization name="'+UniqueName+'"/>
			<organizationserviceurl value="https://'+UniqueName+'.api.crmpf2.crmliveperf.com/XrmServices/2011/Organization.svc" />
			<discoveryserver value="disco.crmpf2.crmliveperf.com" />
			<sqlcnn value="Application Name=CRM_Perf_BenchMark;Server='+SqlServerName+';Initial Catalog='+DatabaseName+';Integrated Security=sspi"/>	
			<configsqlcnn value="Application Name=CRM_Perf_BenchMark;Server=PERF5GEOSQL1;Initial Catalog=MSCRM_CONFIG;Integrated Security=sspi"/>
			<passport>
				<userbase value="TestUsr" /><adminuser value="admin" />
				<start value="1" />
				<count value="3" />
				<outlookstart value="1" />
				<outlookcount value="3" />
				<passportdomain value="crmperfbase.ccsctp.net" />
				<password value="T!T@n1130" /> 
				<partner value="port.crmpf2.crmliveperf.com" />
			</passport>
	</crmserver>
'
FROM [mscrm_config].[dbo].[Organization] 
