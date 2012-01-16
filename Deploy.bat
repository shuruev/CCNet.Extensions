@ECHO OFF
SETLOCAL

SET ServerPath=\\rufrt-vxbuild\d$\CruiseControl.NET\Server

XCOPY "CCNet.Extensions.Plugin\bin\Debug\CCNet.Extensions.Plugin.dll" "%ServerPath%\" /Y
XCOPY "CCNet.ProjectAdapter\bin\Debug\CCNet.ProjectAdapter.exe" "%ServerPath%\CCNet.ProjectAdapter\" /Y
XCOPY "CCNet.ProjectAdapter\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.ProjectAdapter\" /Y
XCOPY "CCNet.ProjectChecker\bin\Debug\CCNet.ProjectChecker.exe" "%ServerPath%\CCNet.ProjectChecker\" /Y
XCOPY "CCNet.ProjectChecker\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.ProjectChecker\" /Y
XCOPY "CCNet.ProjectNotifier\bin\Debug\CCNet.ProjectNotifier.exe" "%ServerPath%\CCNet.ProjectNotifier\" /Y
XCOPY "CCNet.ProjectNotifier\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.ProjectNotifier\" /Y
XCOPY "CCNet.ServiceChecker\bin\Debug\CCNet.ServiceChecker.exe" "%ServerPath%\CCNet.ServiceChecker\" /Y
XCOPY "CCNet.ServiceChecker\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.ServiceChecker\" /Y
XCOPY "CCNet.ObsoleteCleaner\bin\Debug\CCNet.ObsoleteCleaner.exe" "%ServerPath%\CCNet.ObsoleteCleaner\" /Y
XCOPY "CCNet.ObsoleteCleaner\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.ObsoleteCleaner\" /Y
XCOPY "CCNet.ObsoleteCleaner\bin\Debug\CCNet.Releaser.Client.dll" "%ServerPath%\CCNet.ObsoleteCleaner\" /Y
XCOPY "CCNet.ObsoleteCleaner\bin\Debug\ReleaserInterfaces.dll" "%ServerPath%\CCNet.ObsoleteCleaner\" /Y
XCOPY "CCNet.ObsoleteCleaner\bin\Debug\CCNet.ObsoleteCleaner.exe.config" "%ServerPath%\CCNet.ObsoleteCleaner\" /Y
XCOPY "CCNet.ProjectNotifier\bin\Debug\CCNet.SourceNotifier.exe" "%ServerPath%\CCNet.SourceNotifier\" /Y
XCOPY "CCNet.ProjectNotifier\bin\Debug\CCNet.Common.dll" "%ServerPath%\CCNet.SourceNotifier\" /Y

PAUSE
