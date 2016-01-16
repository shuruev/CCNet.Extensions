@ECHO OFF
SETLOCAL

SET ccnetPath=\\rufc-devbuild.cneu.cnwk\d$\CruiseControl.NET

XCOPY "CCNet.Build.Reconfigure\bin\Debug\CCNet.Build.Reconfigure.exe" "%ccnetPath%\CCNet.Build" /D /Y

PAUSE
