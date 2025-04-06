set ThisCommandDir=%~dp0
set ProjectsRoot=%ThisCommandDir%..
call powershell.exe ".\ps_scripts\AddNewFiles.ps1" %ProjectsRoot% "*.sln" "*.csproj" "Readme.txt" "*.cs" "*.svcinfo" "*.wsdl"
