@ECHO OFF
SETLOCAL

SET ccnetPath=\\rufc-devbuild.cneu.cnwk\d$\CruiseControl.NET

XCOPY "CCNet.Extensions.Plugin\bin\Debug\CCNet.Extensions.Plugin.dll" "%ccnetPath%\CCValidator" /D /Y
XCOPY "CCNet.Extensions.Plugin\bin\Debug\CCNet.Extensions.Plugin.dll" "%ccnetPath%\Server-Library" /D /Y

XCOPY "CCNet.Build.Common\bin\Debug\Lean.Configuration.dll" "%ccnetPath%\CCNet.Build" /D /Y
XCOPY "CCNet.Build.Common\bin\Debug\CCNet.Build.Common.dll" "%ccnetPath%\CCNet.Build" /D /Y
XCOPY "CCNet.Build.GenerateNuspec\bin\Debug\CCNet.Build.GenerateNuspec.exe" "%ccnetPath%\CCNet.Build" /D /Y

PAUSE
