param(
  [Parameter(Mandatory=$False)]
  $configFile,
  [Parameter(Mandatory=$False)]
  $toolkitRoot,
  [Parameter(Mandatory=$False)]
  $id
)

$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module "$ScriptDir\..\Adx.Performance.Tools"

function PopulateData([string] $dbPopExe, [string] $dataFile, [psobject] $configProp, $threadcount)
{

  if ($configProp.discoveryServiceUrl)
  {
    $params = "$dataFile /discoveryserver:$($configProp.discoveryServiceUrl) /orgname:$($configProp.orgName) /domain:$($configProp.passportDomain) /organizationserviceurl:$($configProp.organizationServiceUrl) /password:$($configProp.passportPassword) /userbase:$($configProp.passportUserBase) /threadperusercount:$threadcount -nosla -q "
  }
  else
  {
    $params = "$dataFile /discoveryserver:$($configProp.webserver) /webserver:$($configProp.webserver) /orgname:$($configProp.orgName) /domain:$($configProp.domainName) /organizationserviceurl:$($configProp.organizationServiceUrl) /password:$($configProp.crmUserPassword) /userbase:$($configProp.crmUserBase) /threadperusercount:$threadcount "
  }

  $time = Get-Date -Format MM-dd_HH:mm
  "Job $id : $time -- started populating $dataFile" >> $logfile

  StartProcess $dbPopExe $params

  $time = Get-Date -Format MM-dd_HH:mm
  "Job $id : $time -- finished populating $dataFile" >> $logfile

}

if($id -eq $null)
{
  $id = 0;
}

if([string]::IsNullOrEmpty($toolkitRoot))
{
    $toolkitRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition))))
}

$logfile = (Join-Path $toolkitRoot "AdxInstallation.log");


$utilityExe = $toolkitRoot + "\CRM_Perf_Toolkit\Utility\bin\Debug\PerfUtility.exe"
$dbPopExe = $toolkitRoot + "\CRM_Perf_Toolkit\dbPopulator\bin\Debug\NewDbPopulator.exe"
$dataFilePath = $toolkitRoot + "\Binaries\XMLFiles\DBPop"
$solutionPath = $toolkitRoot + "\Binaries\Solutions\"

$logfile = (Join-Path $toolkitRoot "AdxInstallation.log");

if([string]::IsNullOrEmpty($configFile))
{
    $configFile = $toolkitRoot + "\ConfigFiles\ConfigSettings.xml"
}

$adxServerCount = 0;
$configXML = [xml](gc $ConfigFile)
$server = $configXML.configsettings.crmservers.ChildNodes[$id]


if ($server.adxenabled.value -eq "True")
{

  $configProperties = New-Object psobject
  $configProperties = InitConfigProperties -ConfigFile $configFile -Server $server

  if ($server.adx.orgsize.value -eq "small")
  {
    $orgSizePath = "Adx_30BaselineData"
  }
  elseif ($server.adx.orgsize.value -eq "medium")
  {
    $orgSizePath = "Adx_320BaselineData"
  }

  if ($server.passport)
  {
    $adxEntitiesFolder = "online"
  }
  else {
    $adxEntitiesFolder = "onprem"

  }

  $dbPopXMLfolder =Join-Path (Join-Path $dataFilePath $orgSizePath) $adxEntitiesFolder
  $xmlfiles =  Get-ChildItem $dbPopXMLfolder -Filter "*.xml"| Select-Object Name

  $time = Get-Date -Format MM-dd_HH:mm
  "Job $id : $time -- started populating $($server.adx.orgsize.value) data" >> $logfile

  foreach ($file in $xmlfiles)
  {
    $fullpathfile = Join-Path $dbPopXMLfolder $file.Name
    $entityxmlfile = [xml](gc $fullpathfile)
    $numthreads = $entityxmlfile.DbPopConfig.exec.threadCount
    PopulateData $dbPopExe $fullpathfile $configProperties $numthreads
  }

  $time = Get-Date -Format MM-dd_HH:mm
  "Job $id : $time -- end populating $($server.adx.orgsize.value) data" >> $logfile
}
