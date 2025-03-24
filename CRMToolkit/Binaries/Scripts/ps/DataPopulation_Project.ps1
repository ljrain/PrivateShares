param(
[parameter(Mandatory=$true, HelpMessage="OrganizationName")]
[string]
$organizationName,
[parameter(Mandatory=$true, HelpMessage="Number of Users (30, 320, 2000, 5000)")]
[string]
$NumOfUsers,
[parameter(Mandatory=$false, HelpMessage="Full path of the ConfigSettings.xml file")]
[string]
$configFile,
[parameter(Mandatory=$false, HelpMessage="Path to toolkit")]
[string]
$crmtoolkitPath
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

function PopulateData ([string] $dbPopExe, [string] $dataFile)
{
	$params = "$dataFile /discoveryserver:$($configProp.webserver) /webserver:$($configProp.webserver) /orgname:$($configProp.orgName) /domain:$($configProp.domainName) /organizationserviceurl:$($configProp.organizationServiceUrl) /threadcount:$($threadcount) /password:$($configProp.crmUserPassword) /userbase:$($configProp.crmUserBase)"  
    $time = Get-Date -Format MM-dd_HH:mm    
    Write-Host "$time -- started populating $dataFile"
    StartProcess $dbPopExe $params
        
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

if([string]::IsNullOrEmpty($crmtoolkitPath))
{
    $crmtoolkitPath = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)))
}

$dbPopExe = "$crmtoolkitPath\CRM_Perf_Toolkit\dbPopulator\bin\Debug\NewDbPopulator.exe"
$dataFilePath = "$crmtoolkitPath\Binaries\XMLFiles\DBPop\Project" + $NumOfUsers

if([string]::IsNullOrEmpty($configFile))
{
    $configFile = $crmtoolkitPath + "\ConfigFiles\ConfigSettings.xml"
}

$configProperties = New-Object psobject
$configProperties = InitConfigProperties $configFile

$threadCount = 40

PopulateData $dbPopExe "$dataFilePath\0ProjectParameter.xml"
PopulateData $dbPopExe "$dataFilePath\1CharacteristicRecords.xml"
PopulateData $dbPopExe "$dataFilePath\1ResourceRoleRecords.xml"
PopulateData $dbPopExe "$dataFilePath\1WorkHourTemplateRecords.xml"
PopulateData $dbPopExe "$dataFilePath\1ExpenseCategories.xml"
PopulateData $dbPopExe "$dataFilePath\2BookableResourceRecords.xml"
PopulateData $dbPopExe "$dataFilePath\3ProjectsTemplateRecord.xml"
PopulateData $dbPopExe "$dataFilePath\3ProjectsRecords.xml"
PopulateData $dbPopExe "$dataFilePath\3ProjectsRecordsFromTemplate.xml"
PopulateData $dbPopExe "$dataFilePath\4ExpenseRecords.xml"
PopulateData $dbPopExe "$dataFilePath\4TimeEntryRecords.xml"
PopulateData $dbPopExe "$dataFilePath\4OpportunityRecords.xml"
