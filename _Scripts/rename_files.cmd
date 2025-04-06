@echo off
setlocal

if errorlevel 1 goto BAD_START
set varScriptDir=%~dp0
set varOriginalDir=%CD%

:DO_REBUILDING
REM Regarding the next line see https://stackoverflow.com/questions/18309941/what-does-it-mean-by-command-cd-d-dp0-in-windows
cd /d %~dp0
call powershell.exe ".\ps_scripts\rename_files.ps1"
if %errorlevel% NEQ 0 goto BAD_END
cd/d %varOriginalDir%

:GOOD_END
echo  command rename_files.cmd succeeded...
cd/d %varOriginalDir%
exit/B 0

:BAD_END
echo command rename_files.cmd has failed, ERRORLEVEL=%errorlevel%
cd/d %varOriginalDir%
set %errorlevel%=1
exit /B %errorlevel%

:BAD_START
echo Skipping the rename_files.cmd contents, since already ERRORLEVEL=%ERRORLEVEL%
exit /B %errorlevel%