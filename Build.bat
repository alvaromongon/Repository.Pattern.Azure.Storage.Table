@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

REM Build
"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" Microsoft.Azure.Storage.Table.Repository.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM https://docs.myget.org/docs/reference/custom-build-scripts
