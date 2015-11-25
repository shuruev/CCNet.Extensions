@ECHO OFF
SETLOCAL

SET ccnetPath=\\rufc-devbuild.cneu.cnwk\d$\CruiseControl.NET

XCOPY "CCNet.Extensions.Plugin\bin\Release\CCNet.Extensions.Plugin.dll" "%ccnetPath%\Server-Library" /Y

PAUSE
