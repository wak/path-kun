@echo off

set CSC=
set OPT=

if EXIST C:\Windows\Microsoft.NET\Framework64\v2.0.50727\msbuild.exe (
    set CSC=C:\Windows\Microsoft.NET\Framework64\v2.0.50727\msbuild.exe
    set OPT=
)

if EXIST C:\Windows\Microsoft.NET\Framework64\v3.5\msbuild.exe (
    set CSC=C:\Windows\Microsoft.NET\Framework64\v3.5\msbuild.exe
    set OPT=
)

if EXIST C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe (
    set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe
    set OPT=
)

%CSC% %OPT% /p:Configuration=Release path-kun.sln

echo =================================
echo ビルドが完了しました。
echo 以下のファイルをご利用ください。
echo.
echo   bin\Release\
echo     PathKunBasic.exe
echo     PathKunExplorer.exe
echo     PathKunPathOnly.exe
echo.

pause
