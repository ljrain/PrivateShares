param(
[parameter(Mandatory=$true, HelpMessage="CRM2013SP1, CRM2015, CRM2015SP1  , ARA etc...")]
[string]
$release,
[parameter(Mandatory=$false, HelpMessage="Full path of the ConfigSettings.xml file")]
[string]
$configFile
)


function StartProcess([string] $processName, [string] $params)
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

  $webServer = $configXML.configsettings.crmservers.crmserver.server.value
  $configProp | Add-Member -MemberType NoteProperty -Name "webserver" -Value $webServer

  $orgName = $configXML.configsettings.crmservers.crmserver.organization.name
  $configProp | Add-Member -MemberType NoteProperty -Name "orgName" -Value $orgName

  $authenticationType = $configXML.configsettings.authentication.type
  $configProp | Add-Member -MemberType NoteProperty -Name "authenticationType" -Value $authenticationType

  return $configProp
}

$toolkitRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)))

$dbPopExe = "..\..\..\CRM_Perf_Toolkit\dbPopulator\bin\Debug\dbPopulator.exe"
$dataFilePath = "..\..\XMLFiles\DBPop\5UserBaselineData"
$solutionPath = $toolkitRoot + "\Binaries\Solutions"

if([string]::IsNullOrEmpty($configFile))
{
    $configFile = "..\..\..\ConfigFiles\ConfigSettings.xml"
}

$configProperties = New-Object psobject
$configProperties = InitConfigProperties $configFile

$params = "/domain:$($configProperties.domainName) /webserver:$($configProperties.webserver) /userbase:$($configProperties.adminUserName) /password:$($configProperties.adminPassword) /threadcount:1 /crmuserbase:$($configProperties.crmUserBase) /crmuserstart:$($configProperties.crmUserStart) /crmusercount:$($configProperties.crmUserCount) /orgname:$($configProperties.orgName)"
StartProcess "..\..\Exe\UserConfigGen.exe" $params

StartProcess $dbPopExe ".\UserPopulate.xml"
StartProcess $dbPopExe "$dataFilePath\OrgDataFiles\ProductSuite.xml"

$params = "$dataFilePath\NestedSFAEntitiesWithState_Custom_Administrator.xml /domain:$($configProp.domainName) /discoveryserver:$($configProp.webserver) /webserver:$($configProp.webserver) /userbase:$($configProp.userName) /password:$($configProp.password) /crmuserbase:$($configProp.crmUserBase) /crmuserstart:$($configProp.crmUserStart) /crmusercount:$($configProp.crmUserCount) /orgname:$($configProp.orgName)"   
StartProcess $dbPopExe $params

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

