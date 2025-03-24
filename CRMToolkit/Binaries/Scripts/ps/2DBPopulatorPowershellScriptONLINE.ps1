
$ThreadCount="10"
$InputFile="C:\CRMToolkit\2DBPopulatorSampleFileCustomEntity.xml"
$OrgName="jemorl365"
$CRMUrl="https://jemorl365.crm.dynamics.com"
$DomainName="jemorl365.onmicrosoft.com"
$Userbase="crmusr"
$Userpassword="test"
$Userstart="1"
$Usercount="1"
$Authentication="OnlineFederation"


Cd C:\CRMToolkit\CRM_Perf_Toolkit\dbPopulator\bin\Debug\
.\NewDbPopulator.exe $InputFile /discoveryserver:$CRMUrl /webserver:$CRMUrl /orgname:$OrgName /domain:$DomainName /organizationserviceurl:$CRMUrl/XrmServices/2011/Organization.svc /threadcount:$ThreadCount /password:$Userpassword /userbase:$Userbase /userstartindex:$Userstart /usercount:$Usercount /authentication:$Authentication
