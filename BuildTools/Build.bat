@echo off
set filename=%date:~0,4%%date:~5,2%%date:~8,2%%time:~0,2%%time:~3,2%%time:~6,2%

set "filename=%filename: =0%"

set targetProject=D:\Github\XiaoCaoTools

set UnityExePath="C:\Program Files\Unity\Hub\Editor\2021.3.34f1c1\Editor\Unity.exe"

echo Start Build %targetProject%

REM pause

%UnityExePath% -projectPath %targetProject% -quit -batchmode -executeMethod CIBuild.Build -logFile %filename%_buildApk.log


REM %1 -projectPath %2 -quit -batchmode -executeMethod APKBuild.Build -logFile build.log

if not %errorlevel%==0 ( goto fail ) else ( goto success )
 
:success
echo Build APK OK
REM Copr Dir
goto end

:fail
echo Build APK Fail

goto end

 
:end
pause
REM pause