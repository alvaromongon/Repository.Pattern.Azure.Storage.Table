@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

REM Build
REM "%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" YourSolution.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
dotnet build Microsoft.Azure.Storage.Table.Repository.sln --configuration %config%

REM Package
mkdir Build
call nuget pack -OutputDirectory Build Microsoft.Azure.Storage.Table.Repository.nuspec