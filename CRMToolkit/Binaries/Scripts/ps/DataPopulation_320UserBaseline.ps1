param(
[parameter(Mandatory=$true, HelpMessage="CRM2013SP1, CRM2015, CRM2015SP1 , ARA etc...")]
[string]
$release,
[parameter(Mandatory=$false, HelpMessage="Full path of the ConfigSettings.xml file")]
[string]
$configFile

)

function StartProcess([parameter(Mandatory=$true)] [string] $processName, [parameter(Mandatory=$true)] [string] $params)
{
    $processInfo = New-Object System.Diagnostics.ProcessStartInfo
    $processInfo.FileName = $processName
    $processInfo.RedirectStandardError = $true
    $processInfo.RedirectStandardOutput = $false
    $processInfo.UseShellExecute = $false
    $processInfo.Arguments = $params

    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $processInfo
    $process.Start()
    $process.WaitForExit();
}

function PopulateData ([string] $dbPopExe, [string] $dataFile, [psobject] $configProp)
{
    $params = "$dataFile /discoveryserver:$($configProp.webserver) /webserver:$($configProp.webserver) /orgname:$($configProp.orgName) /domain:$($configProp.domainName) /organizationserviceurl:$($configProp.organizationServiceUrl) /threadcount:$($threadcount) /password:$($configProp.crmUserPassword) /userbase:$($configProp.crmUserBase)" 
    $time = Get-Date -Format MM-dd_HH:mm    
    Write-Host "$time -- started populating $dataFile"
    StartProcess $dbPopExe $params
        
}


function ImportSolution([string] $utilityExe, [psobject] $configProp, [string] $solution)
{
    $params = "/actiontype:ImportCustomization $solution /username:$($configProp.userName) /password:$($configProp.password) /organizationserviceurl:$($configProp.organizationServiceUrl)"
    $time = Get-Date -Format MM-dd_HH:mm    
    Write-Host "$time -- started importing $solution"    
    StartProcess $utilityExe $params
}

function SetPrivilege([string] $utilityExe, [psobject] $configProp, [string] $role, [string] $privileges, [string] $privilegeDepth)
{
    $params = "/actiontype:setprivileges /role:`"$role`" /privileges:$privileges /privilegeDepth:$privilegeDepth /orgname:$($configProp.orgName) /username:$($configProp.userName) /password:$($configProp.password) /organizationserviceurl:$($configProp.organizationServiceUrl)"
    $time = Get-Date -Format MM-dd_HH:mm    
    Write-Host "$time -- started setting privileges for role $role "      
    StartProcess $utilityExe $params
}

#Set privileges
function PostDbpop([string] $utilityExe, [psobject] $configProp)
{
    $privileges ="prvCreatenew_COOpportunity;prvDeletenew_COOpportunity;prvCreatennew_coprospect;prvDeletenew_coprospect;prv"
    SetPrivilege $utilityExe $configProp "sales associate" $privileges "Basic"

    $privileges ="prvReadnew_COOpportunity;prvWritenew_COOpportunity;prvAppendnew_COOpportunity;prvAppendTonew_COOpportunity;prvReadnew_coprospect;prvWritenew_coprospect;prvAppendnew_coprospect;prvAppendTonew_coprospect"
    SetPrivilege $utilityExe $configProp "sales associate" $privileges "Global"

    $privileges = "prvCreatenew_pluginverifier;prvReadnew_pluginverifier;prvWritenew_pluginverifier;prvDeletenew_pluginverifier;prvAppendnew_pluginverifier;prvAppendTonew_pluginverifier;prvAssignnew_pluginverifier;prvSharenew_pluginverifier"
    SetPrivilege $utilityExe $configProp "csr manager" $privileges "Local"

    $privileges = "prvCreatenew_pluginverifier;prvReadnew_pluginverifier;prvWritenew_pluginverifier;prvDeletenew_pluginverifier;prvAppendnew_pluginverifier;prvAppendTonew_pluginverifier;prvAssignnew_pluginverifier;prvSharenew_pluginverifier"
    SetPrivilege $utilityExe $configProp "customer service representative" $privileges "Local"

    $privileges = "prvCreatenew_pluginverifier;prvReadnew_pluginverifier;prvWritenew_pluginverifier;prvDeletenew_pluginverifier;prvAppendnew_pluginverifier;prvAppendTonew_pluginverifier;prvAssignnew_pluginverifier;prvSharenew_pluginverifier"
    SetPrivilege $utilityExe $configProp "marketing manager" $privileges "Local"

    $privileges = "prvCreatenew_pluginverifier;prvReadnew_pluginverifier;prvWritenew_pluginverifier;prvDeletenew_pluginverifier;prvAppendnew_pluginverifier;prvAppendTonew_pluginverifier;prvAssignnew_pluginverifier;prvSharenew_pluginverifier"
    SetPrivilege $utilityExe $configProp "marketing professional" $privileges "Local"

    $privileges = "prvCreatenew_pluginverifier;prvReadnew_pluginverifier;prvWritenew_pluginverifier;prvDeletenew_pluginverifier;prvAppendnew_pluginverifier;prvAppendTonew_pluginverifier;prvAssignnew_pluginverifier;prvSharenew_pluginverifier"
    SetPrivilege $utilityExe $configProp "sales associate" $privileges "Local"

    $privileges = "prvCreatenew_pluginverifier;prvReadnew_pluginverifier;prvWritenew_pluginverifier;prvDeletenew_pluginverifier;prvAppendnew_pluginverifier;prvAppendTonew_pluginverifier;prvAssignnew_pluginverifier;prvSharenew_pluginverifier"
    SetPrivilege $utilityExe $configProp "sales manager" $privileges "Local"

    $privileges = "prvCreatenew_pluginverifier;prvReadnew_pluginverifier;prvWritenew_pluginverifier;prvDeletenew_pluginverifier;prvAppendnew_pluginverifier;prvAppendTonew_pluginverifier;prvAssignnew_pluginverifier;prvSharenew_pluginverifier"
    SetPrivilege $utilityExe $configProp "salesperson" $privileges "Local"

    $privileges = "prvCreatenew_pluginverifier;prvReadnew_pluginverifier;prvWritenew_pluginverifier;prvDeletenew_pluginverifier;prvAppendnew_pluginverifier;prvAppendTonew_pluginverifier;prvAssignnew_pluginverifier;prvSharenew_pluginverifier"
    SetPrivilege $utilityExe $configProp "schedule manager" $privileges "Local"

    $privileges = "prvCreatenew_pluginverifier;prvReadnew_pluginverifier;prvWritenew_pluginverifier;prvDeletenew_pluginverifier;prvAppendnew_pluginverifier;prvAppendTonew_pluginverifier;prvAssignnew_pluginverifier;prvSharenew_pluginverifier"
    SetPrivilege $utilityExe $configProp "scheduler" $privileges "Local"

    $privileges = "prvAssignContact;prvReadContact;prvAppendToContact;prvDeleteContact;prvAppendContact;prvWriteContact;prvCreateContact;prvShareContact"
    SetPrivilege $utilityExe $configProp "csr manager" $privileges "Local"

    $privileges = "prvAssignContact;prvReadContact;prvAppendToContact;prvDeleteContact;prvAppendContact;prvWriteContact;prvCreateContact;prvShareContact"
    SetPrivilege $utilityExe $configProp "customer service representative" $privileges "Local"
}

function InitConfigProperties([string] $configFile)
{
  $configXML = [xml](gc $configFile)
  $configProp = New-Object psobject
  
  $configFolder = Split-Path -Parent $configFile
  $configProp | Add-Member -MemberType NoteProperty -Name "configFolder" -Value $configFolder

  $domainName = $configXML.configsettings.crmdomain.value
  $configProp | Add-Member -MemberType NoteProperty -Name "domainName" -Value $domainName

  $adminUserName = $configXML.configsettings.runas.userid
  $configProp | Add-Member -MemberType NoteProperty -Name "userName" -Value $adminUserName

  $adminPassword = $configXML.configsettings.runas.password
  $configProp | Add-Member -MemberType NoteProperty -Name "password" -Value $adminPassword

  $crmUserBase = $configXML.configsettings.crmservers.crmserver.AD.userbase.value
  $configProp | Add-Member -MemberType NoteProperty -Name "crmUserBase" -Value $crmUserBase

  $crmUserStart = $configXML.configsettings.crmservers.crmserver.AD.start.value
  $configProp | Add-Member -MemberType NoteProperty -Name "crmUserStart" -Value $crmUserStart

  $crmUserCount = $configXML.configsettings.crmservers.crmserver.AD.count.value
  $configProp | Add-Member -MemberType NoteProperty -Name "crmUserCount" -Value $crmUserCount

  $crmUserPassword = $configXML.configsettings.crmservers.crmserver.AD.userpassword.value
  $configProp | Add-Member -MemberType NoteProperty -Name "crmUserPassword" -Value $crmUserPassword

  $webserver = $configXML.configsettings.crmservers.crmserver.server.value
  $configProp | Add-Member -MemberType NoteProperty -Name "webserver" -Value $webserver

  $orgName = $configXML.configsettings.crmservers.crmserver.organization.name
  $configProp | Add-Member -MemberType NoteProperty -Name "orgName" -Value $orgName

  $authenticationType = $configXML.configsettings.authentication.type
  $configProp | Add-Member -MemberType NoteProperty -Name "authenticationType" -Value $authenticationType
  
  $organizationServiceUrl = $configXML.configsettings.crmservers.crmserver.organizationserviceurl.value
  $configProp | Add-Member -MemberType NoteProperty -Name "organizationServiceUrl" -Value $organizationServiceUrl
  
  return $configProp
}

$toolkitRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)))

$utilityExe = $toolkitRoot + "\CRM_Perf_Toolkit\Utility\bin\Debug\PerfUtility.exe"
$dbPopExe = $toolkitRoot + "\CRM_Perf_Toolkit\dbPopulator\bin\Debug\NewDbPopulator.exe"
$dataFilePath = $toolkitRoot + "\Binaries\XMLFiles\DBPop"
$solutionPath = $toolkitRoot + "\Binaries\Solutions"

if([string]::IsNullOrEmpty($configFile))
{
    $configFile = $toolkitRoot + "\ConfigFiles\ConfigSettings.xml"
}

$configProperties = New-Object psobject
$configProperties = InitConfigProperties $configFile
$threadCount = 10

PopulateData $dbPopExe "$dataFilePath\OrgDataFiles\ProductSuite.xml" $configProperties

ImportSolution $utilityExe $configProperties $solutionPath\Common\PreDataPopulation\perf_solution_6_0_0_0_managed.zip
ImportSolution $utilityExe $configProperties $solutionPath\Common\PreDataPopulation\FLSCustomizations_Orion_0213_managed.zip
ImportSolution $utilityExe $configProperties $solutionPath\Common\PreDataPopulation\CustomRoleSolution_1_0_0_0_managed.zip
ImportSolution $utilityExe $configProperties $solutionPath\Common\PreDataPopulation\Sla_Perf_Solution.zip

$privileges ="prvReadPostFollow;prvReadmsdyn_PostConfig;prvReadmsdyn_PostRuleConfig;prvReadmsdyn_PostAlbum;prvReadmsdyn_wallsavedquery;prvCreatePost;prvReadPost;prvAppendPost;prvAppendToPost"
SetPrivilege $utilityExe $configProperties "sales associate" $privileges "Global"

$params = "/actiontype:OrgStructureGenerator /domain:$($configProperties.domainName) /webserver:$($configProperties.webserver) /administrator:$($configProperties.userName) /password:$($configProperties.password) /authentication:$($configProperties.authenticationType) /discoveryserver:$($configProperties.webserver) /organization:$($configProperties.orgName) /sectorstart:1 /sectorcount:1"
Start-Process $utilityExe $params
Start-Sleep -s 5
Move-Item .\OrgStructure.xml ..\..\XMLFiles\DBPop\OrgDataFiles -Force

PopulateData $dbPopExe $dataFilePath\OrgDataFiles\OrgStructure.xml $configProperties

$dataFiles = Get-ChildItem -Path $dataFilePath\320UserBaselineData -Filter *.xml
foreach($file in $dataFiles)
{
    PopulateData $dbPopExe $file.FullName $configProperties
}

#create a temp folder to hold all the solutions as per the deployment and release
if(!(Test-Path $solutionPath\workingDir))
{
    New-Item $solutionPath\workingDir -ItemType Directory
}

Get-ChildItem -Path $solutionPath\Common\PostDataPopulation -Include '*.zip' -Recurse | Copy-Item -Destination $solutionPath\workingDir

#Add new solutions introduced in Carina
if(!([string]::Compare($release, "CRM2015SP1", $true)))
{
    Get-ChildItem -Path $solutionPath\CRM2015SP1 -Include '*.zip' -Recurse | Copy-Item -Destination $solutionPath\workingDir
}

#Add new solutions introduced in ARA
if(!([string]::Compare($release, "ARA", $true)))
{
    Get-ChildItem -Path $solutionPath\ARA -Include '*.zip' -Recurse | Copy-Item -Destination $solutionPath\workingDir
}

$solutions = Get-ChildItem -Path $solutionPath\workingDir | % { $_.FullName }
foreach($sol in $solutions)
{
    ImportSolution $utilityExe $configProperties $sol
}

PostDbpop $utilityExe $configProperties

#Clean up
Remove-Item $solutionPath\workingDir -Recurse