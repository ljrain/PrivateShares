#REM ==========================================================================================
#REM This script is used to assign the CRM security role to pre created user in O365 CRM org
#REM ==========================================================================================
$WEBSERVER="<WEBSERVERNAME>"
$ORGANIZATION="<ORGNAME>"
$DOMAIN="<CRM DOMAIN>"
$USERNAME="<ADMINUSER>"
$PASSWORD="<PASSWORD>"
$CRMUSERBASE="<USERBASE>"
$CRMUSERSTART="<USERSTART>"
$CRMUSERCOUNT="<NUMOFUSER>"
$discoveryserver="<DISCOVERYSERVER>"




	
	#REM DbPopulator\SystemUserRoleGenerator.exe	/domain:thephonecorp.ccsctp.net /webserver:thephonecorp.crm.crmlivetie.com /discoveryserver:disco.crm.crmlivetie.com /userbase:TestUsr /password:T!T@n1130 /threadcount:1 /count:400 /organization:thephonecorp /passportdomain:thephonecorp.ccsctp.net /adminname:administrator /authentication:LiveId	
	DbPopulator\SystemUserRoleGenerator.exe	/domain:$DOMAIN /webserver:$WEBSERVER /discoveryserver:disco.crm.crmlivetie.com /userbase:$CRMUSERBASE /password:$PASSWORD /threadcount:1 /crmuserstart:$CRMUSERSTART /count:$CRMUSERCOUNT /organization:$ORGANIZATION /passportdomain:thephonecorp.ccsctp.net /adminname:admin /authentication:LiveId	/ssl:true

	DbPopulator\dbPopulator.exe UserRole.xml 
	
