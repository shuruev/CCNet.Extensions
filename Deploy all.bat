@ECHO OFF
SETLOCAL

SET ccnetPath=\\rufc-devbuild.cneu.cnwk\d$\CruiseControl.NET

:XCOPY "CCNet.Extensions.Plugin\bin\Debug\CCNet.Extensions.Plugin.dll" "%ccnetPath%\CCValidator" /D /Y
:XCOPY "CCNet.Extensions.Plugin\bin\Debug\CCNet.Extensions.Plugin.dll" "%ccnetPath%\Server-Library" /D /Y

:XCOPY "CCNet.Build.Common\bin\Debug\Lean.Configuration.dll" "%ccnetPath%\CCNet.Build" /D /Y
:XCOPY "CCNet.Build.Common\bin\Debug\CCNet.Build.Common.dll" "%ccnetPath%\CCNet.Build" /D /Y
:XCOPY "CCNet.Build.Confluence\bin\Debug\CCNet.Build.Confluence.dll" "%ccnetPath%\CCNet.Build" /D /Y
:XCOPY "CCNet.Build.Tfs\bin\Debug\CCNet.Build.Tfs.dll" "%ccnetPath%\CCNet.Build" /D /Y

:XCOPY "CCNet.Build.AzureUpload\bin\Debug\Microsoft.WindowsAzure.Storage.dll" "%ccnetPath%\CCNet.Build" /D /Y
:XCOPY "CCNet.Build.AzureUpload\bin\Debug\CCNet.Build.AzureUpload.exe" "%ccnetPath%\CCNet.Build" /D /Y

XCOPY "CCNet.Build.CheckProject\bin\Debug\CCNet.Build.CheckProject.exe" "%ccnetPath%\CCNet.Build" /D /Y

:XCOPY "CCNet.Build.GenerateNuspec\bin\Debug\CCNet.Build.GenerateNuspec.exe" "%ccnetPath%\CCNet.Build" /D /Y

:XCOPY "CCNet.Build.NotifyProjects\bin\Debug\QuickGraph.dll" "%ccnetPath%\CCNet.Build" /D /Y
:XCOPY "CCNet.Build.NotifyProjects\bin\Debug\CCNet.Build.NotifyProjects.exe" "%ccnetPath%\CCNet.Build" /D /Y

XCOPY "CCNet.Build.Reconfigure\bin\Debug\CCNet.Build.Reconfigure.exe" "%ccnetPath%\CCNet.Build" /D /Y

:XCOPY "CCNet.Build.SetupPackages\bin\Debug\Lean.Database.dll" "%ccnetPath%\CCNet.Build" /D /Y
:XCOPY "CCNet.Build.SetupPackages\bin\Debug\CCNet.Build.SetupPackages.exe" "%ccnetPath%\CCNet.Build" /D /Y

XCOPY "CCNet.Build.SetupProject\bin\Debug\CCNet.Build.SetupProject.exe" "%ccnetPath%\CCNet.Build" /D /Y

PAUSE
