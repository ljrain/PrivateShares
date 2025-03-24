[CmdletBinding(DefaultParametersetName='None')]
param(
[parameter(Mandatory=$false, HelpMessage="Full path of the ConfigSettings.xml file.")]
[string]
$configFile,
[parameter(Mandatory=$false, HelpMessage="Flag to specifiy whether or not we want to install the data.")]
[switch]
$InstallData,
[parameter(ParameterSetName='AdxSolution', Mandatory=$false, HelpMessage="Flag to specifiy whether or not we want to install the solutions.")]
[switch]
$InstallSolutions,
[parameter(ParameterSetName='AdxSolution', Mandatory=$true, HelpMessage="Provide the full path of the Portals zip file.")]
[string]
$PortalsZipPath
)



if($InstallSolutions)
{
  if($InstallData){
    Write-Host "Installing Adx Solutions AND Data" -ForegroundColor Cyan;
  }
  else{
    Write-Host "Installing Adx Solutions only" -ForegroundColor Cyan;
  }

}
elseif($InstallData)
{
  Write-Host "Installing Adx Data only" -ForegroundColor Cyan;
}
else {
  Write-Host "Nothing to be done, pass -InstallSolutions and/or -InstallData flags" -ForegroundColor Yellow;
  Write-Host "Before installing data, read Binaries\XMLFiles\DBPop\PortalsREADME.txt " -ForegroundColor Yellow;
  Exit
}

$toolkitRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)))


$utilityExe = $toolkitRoot + "\CRM_Perf_Toolkit\Utility\bin\Release\PerfUtility.exe"
$dbPopExe = $toolkitRoot + "\CRM_Perf_Toolkit\dbPopulator\bin\Release\NewDbPopulator.exe"
$dataFilePath = $toolkitRoot + "\Binaries\XMLFiles\DBPop"
$solutionPath = $toolkitRoot + "\Binaries\Solutions"

if([string]::IsNullOrEmpty($configFile))
{
    $configFile = $toolkitRoot + "\ConfigFiles\ConfigSettings.xml"
}

$adxServerCount = 0;
$configXML = [xml](gc $ConfigFile)
$i = 0;
$logfile = (Join-Path $toolkitRoot "AdxInstallation.log");
$time = Get-Date -Format MM-dd_HH:mm
"$time -- starting adx deployement" > $logfile

foreach ($server in $configXML.configsettings.crmservers.ChildNodes)
{

  if ($server.adxenabled.value -eq "True")
  {
    $adxServerCount++;


    Write-Host "`Populating server $i." -ForegroundColor Cyan;
    if($InstallSolutions)
    {
      .\Adx.Performance.Tools\Adx_ImportSolutions.ps1 -PortalsZipPath $PortalsZipPath -configFile $configFile -toolkitRoot $toolkitRoot -id $i
    }
    if($InstallData)
    {
      .\Adx.Performance.Tools\Adx_ImportData.ps1 -configFile $configFile -toolkitRoot $toolkitRoot -id $i
    }

  }
  $i++;
}

Write-Host "`nPopulated $adxServerCount Adx server(s)." -ForegroundColor Cyan;

$time = Get-Date -Format MM-dd_HH:mm
"$time -- finished adx deployement" >> $logfile


