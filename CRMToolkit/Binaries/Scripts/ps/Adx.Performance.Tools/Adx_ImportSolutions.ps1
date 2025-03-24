param(
  [Parameter(Mandatory=$True)]
  $PortalsZipPath,
  [Parameter(Mandatory=$False)]
  $configFile,
  [Parameter(Mandatory=$False)]
  $toolkitRoot,
  [Parameter(Mandatory=$False)]
  $id
)

$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module "$ScriptDir\..\Adx.Performance.Tools" -Force

function InstallAdxPortals
{
 Param(
  [parameter(Mandatory=$true)]
    [string]
    $PortalType,
  [parameter(Mandatory=$true)]
    $ConfigProps,
  [parameter(Mandatory=$true)]
    $SolutionBase
  )

  $buildFolder = Join-Path $SolutionBase "\Build"
  $pvsFolder = Join-Path $SolutionBase "\PackageDeployer\SdkTools\CrmPackageDeployer\PowerShell"

  if ($ConfigProps.discoveryServiceUrl)
  {
    Write-Host "Online Detected"
    Write-Host "Creating credentials with:"
    Write-Host "password: $($ConfigProps.passportPassword)"
    Write-Host "username: $($ConfigProps.passportFullUsername)"
    $securedPassportPassword = ConvertTo-SecureString -String $ConfigProps.passportPassword -AsPlainText -Force
    $passportCred = New-Object System.Management.Automation.PSCredential ($ConfigProps.passportFullUsername, $securedPassportPassword)

    $params =  @{
      organizationName = $ConfigProps.orgName;
      credential = $passportCred;
    }

    $params >> $logfile ;
    $time = Get-Date -Format MM-dd_HH:mm
    "Job $id : $time -- started installing Adx portal(s) for $($ConfigProps.organizationBaseUrl)." >> $logfile

    cd $buildFolder;

    ("customer", "community", "partner") | %{

      $time = Get-Date -Format MM-dd_HH:mm
      "Job $id : $time -- started installing Adx $_ portal for $($ConfigProps.organizationBaseUrl)." >> $logfile

      Write-Host "Calling DeployPerfEnv.ps1 with parameters: -portalType $_ and:" -ForegroundColor Cyan;
      $params
      .\DeployPerfEnv.ps1 -portalType $_ @params ;

      $time = Get-Date -Format MM-dd_HH:mm
      "Job $id : $time -- done installing Adx $_ portal for $($ConfigProps.organizationBaseUrl)." >> $logfile
    }

    $time = Get-Date -Format MM-dd_HH:mm
    "Job $id : $time -- done installing Adx portal(s) for $($ConfigProps.organizationBaseUrl)." >> $logfile

  }
  else
  {
    Write-Host "OnPrem detected"
    Write-Host "Creating credentials with:"
    Write-Host "password: $($ConfigProps.password)"
    Write-Host "username: $($ConfigProps.userName)"

    $securedPassportPassword = ConvertTo-SecureString -String $ConfigProps.password -AsPlainText -Force
    $passportCred = New-Object System.Management.Automation.PSCredential ($ConfigProps.userName, $securedPassportPassword)

    $params =  @{
      organization = $ConfigProps.orgName;
      credential = $passportCred;
      lcid = 1033;
      serverUrl = $ConfigProps.serverUrl;

    }

    $params >> $logfile ;
    $time = Get-Date -Format MM-dd_HH:mm
    "Job $id : $time -- started installing Adx portal(s) for $($ConfigProps.organizationBaseUrl)." >> $logfile

    cd $pvsFolder;
    $packagesRoot = [System.IO.Path]::GetFullPath((Join-Path $SolutionBase "\PackageDeployer\Packages\"))


    ("customer", "community", "partner") | %{

      switch ($_)
      {
          "customer" {
            $buildPath = Join-Path $packagesRoot "CustomerPortal\"
            $packageName = "Adxstudio.CustomerPortal.dll"
          }
          "community" {
            $buildPath = Join-Path $packagesRoot "CommunityPortal\"
            $packageName = "Adxstudio.CommunityPortal.dll"
          }
          "partner" {
            $buildPath = Join-Path $packagesRoot "PartnerPortal\"
            $packageName = "Adxstudio.PartnerPortal.dll"
          }

          default { throw "Unknown portal type: choose between customer, community, and partner."}
      }


      $time = Get-Date -Format MM-dd_HH:mm
      "Job $id : $time -- started installing Adx $_ portal for $($ConfigProps.organizationBaseUrl)." >> $logfile

      Write-Host "Calling DeployPerfEnv.ps1 with parameters: -portalType $_ and:" -ForegroundColor Cyan;
      $params

      .\DeployPackage.ps1 -buildPath $buildPath -packageName $packageName @params

      $time = Get-Date -Format MM-dd_HH:mm
      "Job $id : $time -- done installing Adx $_ portal for $($ConfigProps.organizationBaseUrl)." >> $logfile
    }

    $time = Get-Date -Format MM-dd_HH:mm
    "Job $id : $time -- done installing Adx portal(s) for $($ConfigProps.organizationBaseUrl)." >> $logfile



  }


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

$expandresult = Expand-PortalsZip -PortalsZipPath $PortalsZipPath

# if expansion succeeded (i.e. it is the first time we unzip portals)
if ($expandresult -eq $true)
{
  Register-XrmTooling  -PortalsZipPath $PortalsZipPath
}

Write-Host "Waiting 3 seconds before starting installation... Press Ctrl+C to abort" -ForegroundColor Cyan
Start-Sleep -s 3

$portalszip = Get-ChildItem $PortalsZipPath
$solutionBase = Join-Path $portalszip.Directory $portalszip.BaseName



if([string]::IsNullOrEmpty($configFile))
{
    $configFile = $toolkitRoot + "\ConfigFiles\ConfigSettings.xml"
}

$configXML = [xml](gc $ConfigFile)
$server = $configXML.configsettings.crmservers.ChildNodes[$id]


if ($server.adxenabled.value -eq "True")
{

  $configProperties = New-Object psobject
  $configProperties = InitConfigProperties -ConfigFile $configFile -Server $server

  if ($server.adx.customerportalbaseurl `
      -And $server.adx.communityportalbaseurl `
      -And $server.adx.partnerportalbaseurl)
  {
    $portalType="allthreeportals"
  }
  else
  {
    $portalType="customer"
  }

  InstallAdxPortals -PortalType $portalType -ConfigProps $configProperties -SolutionBase $solutionBase
}

cd (Join-Path $ScriptDir "/..") ;
