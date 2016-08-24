@ECHO OFF
SETLOCAL

SET ccnetBuild=\\rufc-devbuild.cneu.cnwk\d$\CruiseControl.NET\CCNet.Build

XCOPY "CCNet.Build.PrepareProject\bin\Debug\CCNet.Build.PrepareProject.Run.exe" "%ccnetBuild%" /D /Y
XCOPY "CCNet.Build.CustomReport\bin\Debug\CCNet.Build.CustomReport.Run.exe" "%ccnetBuild%" /D /Y

XCOPY "NetBuild.CheckProject\bin\Debug\NetBuild.CheckProject.Run.exe" "%ccnetBuild%" /D /Y

PAUSE
