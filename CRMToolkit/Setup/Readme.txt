To setup toolkit for release
- Download and install CRM SDK online. The version of CRM SDK to install should match the version of the deployed CRM that you will run the load test against
- Download and install Windows Identity Foundation SDK V3.5 from http://www.microsoft.com/en-us/download/details.aspx?id=4451
- Download and install .NET Framework 4.5.2 Development Pack
- Download and install Microsoft Exchange Web Services Managed API from http://www.microsoft.com/en-us/download/details.aspx?id=35371
- Unzip CRMToolkit.zp under C drive
- Open Powershell and go to C:\CRMToolkit\Setup
- Run ToolkitSetup.ps1 - ToolkitSetup.ps1 -adminName:<CRM Administraor Name> -adminPassword:<CRM Administrator <password>> -domainName:<CRM Domain> -crmServerName:<CRM Server Name> -sqlServerName <SQL Server Name> -orgName:<test org name> -configSku:<Online or Onprem> -release <CRM2013SP1 or CRM2015 or CRM2015SP1> -crmSdkBinDir <CRM SDK bin folder path> -crmSDKReservedDllPath <the location where is located> -outlookDllsPath <the location where dlls required to run outlook test cases, needed only if you need to run outlook test cases>
- Sample commands:
	- For Online: .\ToolkitSetup.ps1 -adminName testusr1@vega5usrtemplate.msolctp-int.com -adminPassword <password> -crmServerName testsg3ag00org1.crmpf2.crmliveperf.com -sqlServerName PERF5sg3OSQL1 -orgName testsg3ag00org1 -configSku online -release CRM2015 -crmSdkBinDir "C:\CRMToolkit\Setup\CRM2015SDK\SDK\Bin" -crmSDKReservedDllPath "C:\Program Files\Microsoft Dynamics CRM\Client\res\web\bin" -outlookDllsPath "C:\Program Files\Microsoft Dynamics CRM\Client\res\web\bin"
	- For Onprem: .\ToolkitSetup.ps1 -adminName administrator -adminPassword <password> -domainName apollo6dom -crmServerName apollo6web -sqlServerName apollo6sql -orgName testOrg -configSku onprem -release CRM2015 -crmSdkBinDir "C:\CRMToolkit\Setup\CRM2015SDK\SDK\Bin" -crmSDKReservedDllPath "C:\Program Files\Microsoft Dynamics CRM\Client\res\web\bin" -outlookDllsPath "C:\Program Files\Microsoft Dynamics CRM\Client\res\web\bin" -ssl $false

To setup toolkit for internal usage:
- From Souce Depot:
	- CD to src\QA\Performance\Toolkit\Setup
	- Run this command - .\ToolkitSetup.ps1 -adminName administrator -adminPassword <password> -domainName apollo6dom -crmServerName apollo6web -orgName testOrg -configSku onprem -release CRM2015 -crmSdkBinDir "..\..\..\..\..\target\debug\amd64" -crmSDKReservedDllPath "..\..\..\..\..\target\debug\amd64" -outlookDllsPath "..\..\..\..\..\target\debug\amd64" -ssl $false
	- Open toolkit in Visual Studio and compile the solution
- From client machines:
	- Copy CRMToolkitInternal.zip to the client machine
	- Unzip CRMToolkitInternal.zip under C drive and rename the folder to CRMToolkit
	- Open Powershell and go to C:\CRMToolkit\Setup
	- Run ToolkitSetup.ps1 - ToolkitSetup.ps1 -adminName:<CRM Administraor Name> -adminPassword:<CRM Administrator <password>> -domainName:<CRM Domain> -crmServerName:<CRM Server Name> -sqlServerName <SQL Server Name> -orgName:<test org name> -configSku:<Online or Onprem> -release <CRM2013SP1 or CRM2015 or CRM2015SP1>
	- Sample commands:
		- For Online: .\ToolkitSetup.ps1 -adminName testusr1@vega5usrtemplate.msolctp-int.com -adminPassword <password> -crmServerName testsg3ag00org1.crmpf2.crmliveperf.com -sqlServerName PERF5sg3OSQL1 -orgName testsg3ag00org1 -configSku online -release CRM2015
		- For Onprem: .\ToolkitSetup.ps1 -adminName administrator -adminPassword <password> -domainName apollo6dom -crmServerName apollo6web -sqlServerName apollo6sql -orgName testOrg -configSku onprem -release CRM2015 -ssl $false


What it does:
- Install CRM SDK
- Copy dlls to toolkit from CRM SDK
- Generate WSDL proxy classes
- Setup ConfigSettings.xml (partially done)
- Build toolkit

Config Settings:
- ConfigSetttings.xml in CRMToolkit\ConfigFiles is required to run Macro
- After running ToolkitSetup.ps1, the ConfigSettings.xml will be set up partially. We need to do the followings before the toolkit can be used to run Macro.
- For Online
	- Run CRMServersGenerationForConfig.sql in CRMToolkit\Binaries\Scripts\SQL against CRM Conifg DB to get the list of orgs to be test against
	- Copy and paste the results from running CRMServersGenerationForConfig.sql to the section of <crmservers>
- For Onprem
	- Modify <AD> section

Setup for load test run:
- Install EMDB on the client machine - run <CRMToolkit Install Dir>\Binaries\Scripts\SQL\EMDB_Setup.sql
- Create load test result database on the client machine - run <CRMToolkit Install Dir>\Binaries\Scripts\SQL\loadtestresultsrepository.sql
- Configure load test store on VS2013 Ultimate - Load Test(from toolbar) -> Manage Test Controller -> Load test results store -> provide server and database (LoadTest2010) -> Test Connection -> OK
- Populate test data
	-- CD to <CRMToolkit Install Dir>\Binaries\Scripts\ps
	-- Run DataPopulation_320UserBaseline.ps1 with no parameters. If you need to test against CRM2015 SP1, run the script with release paramenter - Run DataPopulation_320UserBaseline.ps1 -release CRM2015SP1.
	-- You can find sample data xml files used for data population here - <CRMToolkit Install Dir>\Binaries\XMLFiles\DBPop\320UserBaselineData
Run a load test
	-- Open the load test in Visual Studio - Navigate to the load test file location -> double click the load test file
	-- Run load test - Scroll down to the bottom of the load test file -> find Run Settings section -> right click on it -> Run Load Test