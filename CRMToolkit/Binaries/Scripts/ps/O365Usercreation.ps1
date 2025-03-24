#REM =================================================================================
#REM This script is used to create the new users in O365 environment  with no password change required after first login. 
#REM The lines which are commented out for reading the file are because in Exchange online testing with CRM scalegroup we have to have 1400 exchange tenants in O365 list of all tenants are given in the file which is read to have the users created in those tenants
#REM User creation can be done in all the services of O365 like (CRM, Exchange,Lync,sharepoint) depending on the account SKU the proper license need to be given at the time of user creation
#REM the below commented lines are required in case we wish to assign the Global admin role to the newly created users. But in case the users created are in CRM service and we wish to assign some other role to users then RoleAssignToCRMUser.ps1 script should be used.
#REM =================================================================================

#REM [xml]$userfile = Get-Content CompanyFile_CRM1_S.xml

#REM foreach( $orgs in $userfile.Companies.Company)
#REM {
	#REM $adminuser = $orgs.adminuser;
	#REM Write-host $adminuser;
	#REM $splitteddata = $adminuser.split("@");
	#REM $domainName = $splitteddata[1]	
	#REM =============================================================
	$domainName = ""
	$adminuser = ""
	$secure_string_pwd = convertto-securestring "<PASSWORD>" -asplaintext -force
	$credentials = new-object -typename System.Management.Automation.PSCredential -argumentlist $adminuser,$secure_string_pwd
	
	Import-module msonline
	connect-msolservice -credential  $credentials
	
	Get-module
	Get-msoluser
	$accountSKUID = Get-msolaccountsku 

	for ($i = 1; $i -le 400; $i+=1){Write-host TestUsr$i@$domainName; New-MsolUser -UserPrincipalName TestUsr$i@$domainName -DisplayName "Test User$i" -FirstName "Test" -LastName User$i -password <PASSWORD> -passwordneverexpires $True -licenseassignment $accountSKUID.accountskuid -forcechangepassword $false -usagelocation "US"}	
#REM =============================================================================
#REM remove the commented lines in case you want to assign an administrator role to users.(This script is not only for CRM but to also have the users created in exchange online also which might require administrator privilige)

#REM	$role = Get-MsolRole -RoleName "Company Administrator"	
#REM	Get-MsolRoleMember -RoleObjectId $role.ObjectId
#REM	for ($i = 1; $i -le 400; $i+=1) {Add-MsolRoleMember -RoleName "Company Administrator" -RoleMemberEmailAddress TestUsr$i@$domainName}
#REM }