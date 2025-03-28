
Add-Type -AssemblyName System.IO.Compression.FileSystem

function StartProcess([parameter(Mandatory=$true)] [string] $processName, [parameter(Mandatory=$true)] [string] $params)
{
    $processInfo = New-Object System.Diagnostics.ProcessStartInfo
    $processInfo.FileName = $processName
    $processInfo.RedirectStandardError = $true
    $processInfo.RedirectStandardOutput = $false #turn to false to see output when debugging
    $processInfo.UseShellExecute = $false
    $processInfo.Arguments = $params

    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $processInfo
    $process.Start()
    $process.WaitForExit();
}

function InitConfigProperties
{
  Param(
    [parameter(Mandatory=$true)]
    [string]
    $ConfigFile,
    [parameter(Mandatory=$true)]
    $Server
  )


  $configXML = [xml](gc $ConfigFile)
  $configProp = New-Object psobject

  $configFolder = Split-Path -Parent $ConfigFile
  $configProp | Add-Member -MemberType NoteProperty -Name "configFolder" -Value $configFolder

  $domainName = $configXML.configsettings.crmdomain.value
  $configProp | Add-Member -MemberType NoteProperty -Name "domainName" -Value $domainName

  $adminUserName = $configXML.configsettings.runas.userid
  $configProp | Add-Member -MemberType NoteProperty -Name "userName" -Value $adminUserName

  $adminPassword = $configXML.configsettings.runas.password
  $configProp | Add-Member -MemberType NoteProperty -Name "password" -Value $adminPassword

  $webserver = $Server.server.value
  $configProp | Add-Member -MemberType NoteProperty -Name "webserver" -Value $webserver

  $orgName = $Server.organization.name
  $configProp | Add-Member -MemberType NoteProperty -Name "orgName" -Value $orgName

  $authenticationType = $configXML.configsettings.authentication.type
  $configProp | Add-Member -MemberType NoteProperty -Name "authenticationType" -Value $authenticationType

  $organizationServiceUrl = $Server.organizationserviceurl.value
  $configProp | Add-Member -MemberType NoteProperty -Name "organizationServiceUrl" -Value $organizationServiceUrl

  $orgBaseUrl = $Server.organizationbaseurl.value
  $configProp | Add-Member -MemberType NoteProperty -Name "organizationBaseUrl" -Value $orgBaseUrl


  # For online:
  if ($Server.passport)
  {
    $discoveryServiceUrl = $Server.discoveryserver.value
    $configProp | Add-Member -MemberType NoteProperty -Name "discoveryServiceUrl" -Value $discoveryServiceUrl

    $passportUserBase = $Server.passport.userbase.value
    $configProp | Add-Member -MemberType NoteProperty -Name "passportUserBase" -Value $passportUserBase

    $passportUserPassword = $Server.passport.password.value
    $configProp | Add-Member -MemberType NoteProperty -Name "passportPassword" -Value $passportUserPassword

    $passportAdmin = $Server.passport.adminuser.value
    $passportDomain = $Server.passport.passportdomain.value
    $configProp | Add-Member -MemberType NoteProperty -Name "passportDomain" -Value $passportDomain

    $passportAdminFull = $passportAdmin + "@"+ $passportDomain
    $configProp | Add-Member -MemberType NoteProperty -Name "passportFullUsername" -Value $passportAdminFull

  }
  else
  {

    $onpremserverUrl = $Server.serverbaseurl.value
    $configProp | Add-Member -MemberType NoteProperty -Name "serverUrl" -Value $onpremserverUrl

    $crmUserBase = $Server.AD.userbase.value
    $configProp | Add-Member -MemberType NoteProperty -Name "crmUserBase" -Value $crmUserBase

    $crmUserStart = $Server.AD.start.value
    $configProp | Add-Member -MemberType NoteProperty -Name "crmUserStart" -Value $crmUserStart

    $crmUserCount = $Server.AD.count.value
    $configProp | Add-Member -MemberType NoteProperty -Name "crmUserCount" -Value $crmUserCount

    $crmUserPassword = $Server.AD.userpassword.value
    $configProp | Add-Member -MemberType NoteProperty -Name "crmUserPassword" -Value $crmUserPassword

  }
  return $configProp
}

function Expand-PortalsZip
{

  Param(
    [parameter(Mandatory=$true)]
    [string]
    $PortalsZipPath
    )
  $portalszip = Get-ChildItem $PortalsZipPath
  #$portalszip | Get-Member
  $portalszipFolder = $portalszip.Directory

  $repoPath = Join-Path $portalszip.Directory $portalszip.BaseName

  if (-not (Test-Path $repoPath)) {
    # unzip deployer
    Write-Host "Extracting: $portalszip"
    [System.IO.Compression.ZipFile]::ExtractToDirectory($portalszip, $repoPath)

    $packagesPath = Join-Path $repoPath "PackageDeployer\Packages" -Resolve
    Get-ChildItem $packagesPath -File | ? { $_.Name -like "*.zip" } | % {
      [System.IO.Compression.ZipArchive]$zip = $null
      try {
        $zip = [System.IO.Compression.ZipFile]::OpenRead($_.FullName)
        $entry = ($zip.Entries | ? { $_.Name -like "Adxstudio.*.zip" })[0]
        if ($entry) {
          $destinationZip = Join-Path $packagesPath $entry.Name
          [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $destinationZip, $false)
          $packagePath = Join-Path $packagesPath ([System.IO.Path]::GetFileNameWithoutExtension($_.Name))
          if (-not (Test-Path $packagePath)) {
            Write-Host "Extracting: $destinationZip"
            [System.IO.Compression.ZipFile]::ExtractToDirectory($destinationZip, $packagePath)
          }
          Remove-Item $destinationZip -Force
        }
      } finally {
        if ($zip -ne $null) {
          $zip.Dispose()
        }
      }
    }
    return $true
  }
  else {
      Write-Host "$portalszip was already extracted in $repoPath"
      return $false
  }

}

function Register-XrmTooling
{

  Param(
    [parameter(Mandatory=$true)]
    [string]
    $PortalsZipPath
    )
  $portalszip = Get-ChildItem $PortalsZipPath
  $portalszipFolder = $portalszip.Directory

  $repoPath = Join-Path $portalszip.Directory $portalszip.BaseName
  
  Write-Host "Initialize Package Deployer"

	$registerXrmToolingPath = Join-Path $repoPath "PackageDeployer\SdkTools\CrmPackageDeployer\PowerShell\RegisterXRMTooling.ps1"

	if (Test-Path $registerXrmToolingPath) {
		& $registerXrmToolingPath
	}


}

Export-ModuleMember -Function StartProcess
Export-ModuleMember -Function InitConfigProperties
Export-ModuleMember -Function Expand-PortalsZip
Export-ModuleMember -Function Register-XrmTooling