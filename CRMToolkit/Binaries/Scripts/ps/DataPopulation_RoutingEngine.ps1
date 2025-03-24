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
    $process.Start() | Out-Null
    $process.WaitForExit()
    return $process.ExitCode
}

function PopulateData ([string] $dbPopExe, [string] $dataFile, [string] $sideLoadPath)
{
    $params = "$dataFile /discoveryserver:$($configProp.webserver) /webserver:$($configProp.webserver) /orgname:$($configProp.orgName) /domain:$($configProp.domainName) /organizationserviceurl:$($configProp.organizationServiceUrl) /threadcount:$($threadcount) /password:$($configProp.crmUserPassword) /userbase:$($configProp.crmUserBase)"  
    If ($sideLoadPath -ne "") {
        $params = "$params /sideload:$sideLoadPath"
    }
    $time = Get-Date -Format MM-dd_HH:mm    
    Write-Host "$time -- started populating $dataFile"
    $exitcode = StartProcess $dbPopExe $params
    Write-Host "------------------------------------------------------------"
    If ($exitcode -ne 0){
        Write-Host "Aborted: $dataFile failed with exit code $exitcode"
        Write-Host "Please check the Error Log in the working directory"
        Break
    } Else {
        Write-Host "$dataFile successfully populated"
        Write-Host "There may be warnings in the Error Log"
    }
    Write-Host "------------------------------------------------------------"
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

$dbPopExe = $crmtoolkitPath + "\CRM_Perf_Toolkit\dbPopulator\bin\Debug\NewDbPopulator.exe"
$dataFilePath = "$crmtoolkitPath\Binaries\XMLFiles\DBPop\RoutingEngine" + $NumOfUsers
$sideLoadPath = $crmtoolkitPath + "\Binaries\XMLFiles\DBPop\RoutingEngineData\"

if([string]::IsNullOrEmpty($configFile))
{
    $configFile = $crmtoolkitPath + "\ConfigFiles\ConfigSettings.xml"
}

$configProperties = New-Object psobject
$configProperties = InitConfigProperties $configFile

$threadCount = 1

PopulateData $dbPopExe "$dataFilePath\1BaseEntityPop.xml" ""
PopulateData $dbPopExe "$dataFilePath\2_2000ORResourcesPop.xml" $sideLoadPath
PopulateData $dbPopExe "$dataFilePath\2_2000WAResourcesPop.xml" $sideLoadPath
PopulateData $dbPopExe "$dataFilePath\3_8000ORWorkOrderPop.xml" $sideLoadPath
PopulateData $dbPopExe "$dataFilePath\3_8000WAWorkOrderPop.xml" $sideLoadPath