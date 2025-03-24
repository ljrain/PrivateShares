#set-executionpolicy remotesigned
$OrgName="contoso"
$CRMUrl="contoso.crm.dynamics.com"
$CRMDomainName="crm.dynamics.com"

$CrmAdminName="jmorlock"
$CrmAdminPassword=""
$DomainName="contoso.onmicrosoft.com"

$EntityManagerSQLServer ="."

$Userbase="crmusr"
$Userpassword="access123"
$Userstart="1"
$Usercount="5"


cd C:\CRMToolkit\Setup

#online
.\ToolkitSetup.ps1 -adminName $CrmAdminName@$DomainName  -adminPassword $CrmAdminPassword -domainName $DomainName -crmServerName $CRMUrl -CRMdomain $CRMDomainName -sqlServerName . -orgName $OrgName -configSku online -release CRM2016 -userBase:$Userbase -userPassword: $Userpassword -userStart:$Userstart -userCount:$Usercount -emSqlServer $EntityManagerSQLServer
