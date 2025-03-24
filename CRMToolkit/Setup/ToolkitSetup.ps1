param (
[parameter(Mandatory=$false)]
[string] $release,

[parameter(Mandatory=$true)]
[string] $adminName,

[parameter(Mandatory=$true)]
[string] $adminPassword,

[parameter(Mandatory=$false)]
[string] $domainName,

[parameter(Mandatory=$true)]
[string] $crmServerName,

[parameter(Mandatory=$true)]
[string] $CRMdomain,

[parameter(Mandatory=$false)]
[string] $sqlServerName,

[parameter(Mandatory=$true)]
[string] $orgName,

[parameter(Mandatory=$true)]
[string] $configSku,

[parameter(Mandatory=$false)]
[string] $userBase, 

[parameter(Mandatory=$false)]
[string] $userPassword, 

[parameter(Mandatory=$true)]
[string] $emSqlServer, 

[parameter(Mandatory=$true)]
[string] $userStart, 

[parameter(Mandatory=$true)]
[string] $userCount, 

[parameter(Mandatory=$false)]
[string] $crmSdkBinDir,

[parameter(Mandatory=$false)]
[string] $crmSDKReservedDllPath,

[parameter(Mandatory=$false)]
[string] $outlookDllsPath,

[parameter(Mandatory=$false, HelpMessage="The path of the folder that contains wsdl.exe or the full path of wsdl.exe")]
[string] $wsdlPath,

[parameter(Mandatory=$false)]
[bool] $ssl,

[parameter(Mandatory=$false)]
[bool] $adxEnabled,

[parameter(Mandatory=$false)]
[string] $adxCustomerPortalUrl,

[parameter(Mandatory=$false)]
[string] $adxCommunityPortalUrl,

[parameter(Mandatory=$false)]
[string] $adxPartnerPortalUrl,

[parameter(Mandatory=$false)]
[string] $adxUserBase, 

[parameter(Mandatory=$false)]
[string] $adxUserpassword, 


[parameter(Mandatory=$false)]
[bool] $isAzure
)

function GetWSDL([string] $wsdlExe, [string] $serviceListFile, [string] $toolkitRoot, [string] $msbuildPath)
{
    #Build Utility.exe to download WSDL from CRM server to local
    Write-Host "Compile Utility project in $toolkitRoot\CRM_Perf_Toolkit\Utility" -ForegroundColor Green
    RunProgram $msbuildPath "$toolkitRoot\CRM_Perf_Toolkit\Utility\Utility.csproj /p:Configuration=Debug /p:Platform=AnyCPU /noconsolelogger"

    $prefix = "http://"
    if(![string]::Compare($configSku, "online", $true) -or $ssl -eq $true)
    {
        $prefix = "https://"
    }
    $tempWsdlDir = "$toolkitRoot\Setup\WSDL\WSDL"
    if(!(Test-Path $tempWsdlDir))
    {
        New-Item $tempWsdlDir -ItemType Directory
    }
    $content = Get-Content $serviceListFile
    foreach ($serviceName in $content)
    {
        Write-Host "Download WSDL for $serviceName" -ForegroundColor Green
        if($isAzure)
        {
            RunProgram "$toolkitRoot\CRM_Perf_Toolkit\Utility\bin\Debug\PerfUtility.exe" "/actionType:DownloadWSDL $prefix$crmServerName`:81 $adminName $adminPassword $serviceName $tempWsdlDir $configSku $domainName"
        }
        else
        {
            RunProgram "$toolkitRoot\CRM_Perf_Toolkit\Utility\bin\Debug\PerfUtility.exe" "/actionType:DownloadWSDL $prefix$crmServerName $adminName $adminPassword $serviceName $tempWsdlDir $configSku $domainName"
        }
        GenerateWSDLProxy $wsdlExe $serviceName $toolkitRoot
    }
    PostProcess $toolkitRoot

}

function GenerateWSDLProxy ([string] $wsdlExe, [string] $serviceName, [string] $toolkitRoot)
{
    $serviceNameSpace = $serviceName;
    $outputFolder = "$toolkitRoot\CRM_Perf_Toolkit\CRM_Perf_BenchMark\WSDL"
	if(!(test-path($outputFolder)))
    {
        New-Item $outputFolder -ItemType directory
    }
    if( ($serviceName -eq "Annotation") -or ($serviceName -eq "Service") -or ($serviceName -eq "Solution"))
    {
        $serviceNameSpace += "WebService"
    }
    Write-Host "Generage proxy for $serviceName" -ForegroundColor Green
    RunProgram $wsdlExe "/n:$serviceNameSpace $toolkitRoot\Setup\WSDL\WSDL\$serviceName.wsdl /out:$outputFolder"
}

# Change proxy classes' url to have 'CRM' prefix
function PostProcess([string] $toolkitRoot)
{
    $outputFolder = "$toolkitRoot\CRM_Perf_Toolkit\CRM_Perf_BenchMark\WSDL"
    Get-ChildItem $outputFolder |
    ForEach-Object {
       $toBeReplaced = 'this.Url = "'
        $replace = $toBeReplaced + "crm"
        $temp = Get-Content $_.FullName
        $temp -replace [Regex]::Escape($toBeReplaced), $replace > $_.FullName
    }
}

function ConfigSettings ([string] $toolkitRoot, [bool] $ssl)
{

    $prefix = "http:"
    if($ssl -eq $true)
    {
        $prefix = "https:"
    }
    $configFile = "ConfigSettingsTemplate_Onprem.xml"
    if(!([string]::Compare($configSku, "online", $true)))
    {
      $configFile = "ConfigSettingsTemplate_Online.xml"
    }
	if($isAzure)
    {
      $configFile = "ConfigSettingsTemplate_Azure.xml"
    }

    if(!$adminName.Contains("."))
    {
      $adminName = $adminName.Replace("@","")
    }

    Get-Content ("$toolkitRoot\Setup\$configFile") |
    Foreach-Object {$_ -replace "{SERVERNAME}", $crmServerName `
        -replace "{CRMDOMAIN}", $CRMdomain `
        -replace "{SQLSERVERNAME}", $sqlServerName `
        -replace "{EMSQLSERVER}", $emSqlServer  `
        -replace "{DOMAINNAME}", $domainName `
        -replace "{ORGNAME}", $orgName `
        -replace "{DOMAINADMIN}", $adminName `
        -replace "{DOMAINPASSWORD}", $adminPassword `
        -replace "{USERBASE}", $userBase `
        -replace "{USERPASSWORD}", $userPassword `
        -replace "{USERSTART}", $userStart `
        -replace "{USERCOUNT}", $userCount `
        -replace "{PREFIX}", $prefix `
        -replace "{ADXENABLED}", $adxEnabled `
        -replace "{ADXCUSTOMERPORTAL}", $adxCustomerPortalUrl `
        -replace "{ADXCOMMUNITYPORTAL}", $adxCommunityPortalUrl `
        -replace "{ADXUSERBASE}", $adxUserBase `
        -replace "{ADXUSERPASSWORD}", $adxUserpassword `
        -replace "{ADXPARTNERPORTAL}", $adxPartnerPortalUrl `
        -replace "{ToolkitPath}", $toolkitRoot

    } |  Set-Content ("$toolkitRoot\ConfigFiles\ConfigSettings.xml")
}

function EMDBSetup ([string] $toolkitRoot)
{
    $dbtemplateFile = "EMDBTemplate.sql"
   
    Get-Content ("$toolkitRoot\Binaries\Scripts\SQL\$dbtemplateFile") |
    Foreach-Object {$_ -replace "{ORGNAME}", $orgName
    } |  Set-Content ("$toolkitRoot\Binaries\Scripts\SQL\EMDB_Setup.sql")
}


function RandomUserSetup ([string] $toolkitRoot)
{
    $dbtemplateFile = "insertusersTemplate.sql"
   
    Get-Content ("$toolkitRoot\Setup\$dbtemplateFile") |
    Foreach-Object {$_ -replace "{ToolkitPath}", $toolkitRoot -replace "{ORGNAME}", $orgName
    } |  Set-Content ("$toolkitRoot\AddRandomUsers\insertusers.sql")

    $dbtemplateFile = "insertappusersTemplate.sql"
   
    Get-Content ("$toolkitRoot\Setup\$dbtemplateFile") |
    Foreach-Object {$_ -replace "{ToolkitPath}", $toolkitRoot -replace "{ORGNAME}", $orgName
    } |  Set-Content ("$toolkitRoot\AddRandomUsers\\AddApplicationUsers\insertappusers.sql")
}


function ConfigSettingsSetup ([string] $toolkitRoot)
{
    #Replace Path in configsettings.cs 
    $dbtemplateFile = "ConfigSettingsTemplate.cs"
   
    Get-Content ("$toolkitRoot\CRM_Perf_Toolkit\CRM_Perf_BenchMark\Utilities\$dbtemplateFile") |
    Foreach-Object {$_ -replace "{ToolkitPath}", $toolkitRoot.Replace("\","\\")
    } |  Set-Content ("$toolkitRoot\CRM_Perf_Toolkit\CRM_Perf_BenchMark\Utilities\ConfigSettings.cs")
}


function CopyDlls ([string] $toolkitRoot)
{

    if(!([string]::IsNullOrEmpty($crmSdkBinDir)) -and (Test-Path $crmSdkBinDir))
    {
        if(!(Test-Path $toolkitRoot\CRM_Perf_Toolkit\CrmLibs))
        {
            New-Item $toolkitRoot\CRM_Perf_Toolkit\CrmLibs -Type directory
        }
        Copy-Item "$crmSdkBinDir\Microsoft.Crm.Sdk.Proxy.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs"
        Copy-Item "$crmSdkBinDir\Microsoft.Xrm.Sdk.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs"
     }


    if(!([string]::IsNullOrEmpty($crmSDKReservedDllPath)) -and (Test-Path $crmSDKReservedDllPath))
    {
        Copy-Item "$crmSDKReservedDllPath\Microsoft.Crm.Sdk.Reserved.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs"
     }

    if(!([string]::IsNullOrEmpty($outlookDllsPath)) -and (Test-Path $outlookDllsPath))
    {
        if(!(Test-Path $toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook))
        {
            New-Item $toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook -Type directory
        }
        Copy-Item "$outlookDllsPath\Microsoft.Crm.Application.Components.Platform.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook"
        Copy-Item "$outlookDllsPath\Microsoft.Crm.Application.Outlook.Components.Platform.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook"
        Copy-Item "$outlookDllsPath\Microsoft.Crm.Application.Outlook.OfflineSync.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook"
        Copy-Item "$outlookDllsPath\Microsoft.Crm.Constants.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook"
        Copy-Item "$outlookDllsPath\Microsoft.Crm.Core.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook"
        Copy-Item "$outlookDllsPath\Microsoft.Crm.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook"
        Copy-Item "$outlookDllsPath\Microsoft.Crm.Platform.Sdk.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook"
        Copy-Item "$outlookDllsPath\Microsoft.Crm.Sdk.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook"
        Copy-Item "$outlookDllsPath\Microsoft.Crm.Sdk.Proxy.dll" "$toolkitRoot\CRM_Perf_Toolkit\CrmLibs_Outlook"
    }
}

function CheckDlls ([string] $toolkitRoot)
{
    CheckFile $toolkitRoot\CRM_Perf_Toolkit\CrmLibs\Microsoft.Crm.Sdk.Proxy.dll
    CheckFile $toolkitRoot\CRM_Perf_Toolkit\CrmLibs\Microsoft.Xrm.Sdk.dll
    CheckFile $toolkitRoot\CRM_Perf_Toolkit\CrmLibs\Microsoft.Crm.Sdk.Reserved.dll
}

function CheckFile ([string] $filePath)
{
    if(! (Test-Path $filePath))
    {
        throw [System.IO.FileNotFoundException] "$filePath is missing"
    }
}

function GetWsdlExe
{
    # Get wsdl.exe from user input
    if(![string]::IsNullOrEmpty($wsdlPath))
    {
        if(Test-Path $wsdlPath -PathType Container)
        {
            $wsdlExe = Join-Path $wsdlPath "wsdl.exe"
            if(Test-Path($wsdlExe))
            {
                return $wsdlExe
            }
            else
            {
                throw [System.IO.FileNotFoundException] "failed to find wsdl.exe in $wsdlPath"
            }

        }
        else
        {
            if((Test-Path($wsdlPath)) -and ($wsdlPath.EndsWith("wsdl.exe")))
            {
                return $wsdlPath;
            }
            else
            {
                throw [System.IO.FileNotFoundException] "$wsdlPath is not valid"
            }

        }

    }
    # if user didn't specify wsdl.exe path, try to find wsdl.exe in default location - \Program Files(x86)\Microsoft SDKs\Windows
    else
    {
        $wsdlLocation = Join-Path ${env:ProgramFiles(x86)} "Microsoft SDKs\Windows"
        $temp = Get-ChildItem $wsdlLocation wsdl.exe -Recurse -Force
        if($temp.Length -gt 0)
        {
            return $temp[0].FullName;
        }
        else
        {
            throw [System.IO.FileNotFoundException] "WSDL is missing in $wsdlLocation. Please try again by specifing wsdlPath parameter when running ToolkitSetup.ps1"
        }
    }

}

function RunProgram ([string] $programName, [string] $commandLine)
{
    Write-Host "Running command - $programName $commandLine"
    $runCmd = Start-Process $programName -ArgumentList $commandLine -NoNewWindow -Wait -PassThru
    $exitCode = $runCmd.ExitCode
    if($exitCode -ne 0)
    {
        throw [System.Exception] "$programName $commandLine failed"
    }
}



Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force

Write-Host "Setting BypassStrongNameCheck Registry Keys" -ForegroundColor Green
$registryPath = "HKLM:\SOFTWARE\Microsoft\StrongName\Verification\*,*"
$registryPath2 = "HKLM:\SOFTWARE\Wow6432Node\Microsoft\StrongName\Verification\*,*"
$Name = "Verification"
$value = "*.*"
IF(!(Test-Path $registryPath))
  {
    New-Item -Path $registryPath -Force | Out-Null
    #New-ItemProperty -Path $registryPath -Name $name -Value $value  -PropertyType STRING -Force | Out-Null}
 #ELSE {
    #New-ItemProperty -Path $registryPath -Name $name -Value $value  -PropertyType STRING -Force | Out-Null
    }
    IF(!(Test-Path $registryPath2))
  {
    New-Item -Path $registryPath2 -Force | Out-Null
    #New-ItemProperty -Path $registryPath2 -Name $name -Value $value -PropertyType STRING -Force | Out-Null}
 #ELSE {
    #New-ItemProperty -Path $registryPath2 -Name $name -Value $value -PropertyType STRING -Force | Out-Null
    }


$ErrorActionPreference = "stop"
$scriptpath = $MyInvocation.MyCommand.Path
$currentPath = Split-Path $scriptpath
$toolkitRoot = Split-Path $currentPath
Set-Location $currentPath

#Copy dlls needed to build toolkit
CopyDlls $toolkitRoot
CheckDlls $toolkitRoot

#$wsdlExe = GetWsdlExe

#download WSDL and generate WSDL proxy classes for all the web services needed to run macro test cases
#$wsdlDir = Join-path $currentPath WSDL
#pushd $wsdlDir

#if(!(Test-Path "$wsdlDir\ServiceList.txt"))
#{
#    throw [System.IO.FileNotFoundException] "ServiceList.txt is missing"
#}

$runTimeEnv = [System.Runtime.InteropServices.RuntimeEnvironment]
$msbuildPath = Join-Path $runTimeEnv::GetRuntimeDirectory() msbuild.exe

try
{
    #GetWSDL $wsdlExe ServiceList.txt $toolkitRoot $msbuildPath
}
catch
{
    Write-Host $_.Exception.Message
    return -1
}

popd

Write-Host "Creating Config File" -ForegroundColor Green
#set up ConfigSettings.xml
ConfigSettings $toolkitRoot $ssl

Write-Host "Creating Entity Manager Database Script" -ForegroundColor Green
EMDBSetup $toolkitRoot
RandomUserSetup $toolkitRoot
ConfigSettingsSetup $toolkitRoot

#Build the toolkit
if([string]::IsNullOrEmpty($release))
{
    $release = "CRM2015";
}

try
{
    Write-Host "Build Toolkit" -ForegroundColor Green
    #RunProgram $msbuildPath "$toolkitRoot\CRM_Perf_Toolkit\CRM_Perf_Toolkit.sln /P:Configuration=$release /noconsolelogger"
}
catch
{
    Write-Host $_.Exception.Message
    return -1
}
