@ECHO OFF
SETLOCAL

SET ServerPath=\\rufrt-vxbuild\d$\Program Files\CruiseControl.NET-1.5.6804.1\Server

XCOPY "CCNet.Extensions.Plugin\bin\Debug\CCNet.Extensions.Plugin.dll" "%ServerPath%\" /Y
XCOPY "CCNet.ProjectAdapter\bin\Debug\CCNet.ProjectAdapter.exe" "%ServerPath%\CCNet.ProjectAdapter\" /Y
XCOPY "CCNet.ProjectAdapter\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.ProjectAdapter\" /Y
XCOPY "CCNet.ProjectChecker\bin\Debug\CCNet.ProjectChecker.exe" "%ServerPath%\CCNet.ProjectChecker\" /Y
XCOPY "CCNet.ProjectChecker\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.ProjectChecker\" /Y
XCOPY "CCNet.ProjectNotifier\bin\Debug\CCNet.ProjectNotifier.exe" "%ServerPath%\CCNet.ProjectNotifier\" /Y
XCOPY "CCNet.ProjectNotifier\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.ProjectNotifier\" /Y
XCOPY "CCNet.ServiceChecker\bin\Debug\CCNet.ServiceChecker.exe" "%ServerPath%\CCNet.ServiceChecker\" /Y
XCOPY "CCNet.ServiceChecker\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.ServiceChecker\" /Y

PAUSE
