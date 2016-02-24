@ECHO OFF
SETLOCAL

SET ccnetPath=\\rufc-devbuild.cneu.cnwk\d$\CruiseControl.NET

:XCOPY "CCNet.Build.Common\bin\Debug\CCNet.Build.Common.dll" "%ccnetPath%\CCNet.Build" /D /Y
XCOPY "CCNet.Build.Reconfigure\bin\Debug\CCNet.Build.Reconfigure.exe" "%ccnetPath%\CCNet.Build" /D /Y

PAUSE
