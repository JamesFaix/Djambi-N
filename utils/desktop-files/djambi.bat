@echo off

type title.txt
echo.
echo.

echo Configuring server...
setlocal enableextensions disabledelayedexpansion
set search={app-directory}
set replace=%cd:\=/%
set textFile=%cd%\\server\\appsettings.json
for /f "delims=" %%i in ('type "%textFile%" ^& break ^> "%textFile%" ') do (
    set "line=%%i"
    setlocal enabledelayedexpansion
    >>"%textFile%" echo(!line:%search%=%replace%!
    endlocal
)

echo Starting server...
cd server
start /b ./api.host.exe 1>NUL
timeout 3

echo.
echo Opening client in browser...
explorer "http://localhost:5100"


echo.
echo Close this window to stop the server.
echo All game progress is saved while you play.
pause >NUL
wmic process where "name like '%%api.host.exe%%' and commandline like '%%djambi%%'" delete