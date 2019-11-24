SET SolutionDir=%~dp0..\
SET ProjectDir=%~dp0..\

msbuild %ProjectDir%iRacingSDK.Net.Tests\iRacingSDK.Net.Tests.csproj ^
	-p:SolutionDir=%SolutionDir% ^
	-t:build ^
	-v:minimal ^
	-p:Configuration=Debug

.\..\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe -nodots -nologo -labels ^
	.\iRacingSDK.Net.Tests\bin\x64\Debug\iRacingSDK.Net.Tests.exe