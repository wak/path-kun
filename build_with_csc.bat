@echo off

set CSC=
set OPT=

REM 32bit

if EXIST C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe (
    set CSC=C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe
    set OPT=
)

if EXIST C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe (
    set CSC=C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe
    set OPT=
)

if EXIST C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe (
    set CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe
    set OPT=
)

REM 64bit

if EXIST C:\Windows\Microsoft.NET\Framework64\v2.0.50727\csc.exe (
    set CSC=C:\Windows\Microsoft.NET\Framework64\v2.0.50727\csc.exe
    set OPT=
)

if EXIST C:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe (
    set CSC=C:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe
    set OPT=
)

if EXIST C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe (
    set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
    set OPT=
)

echo ============================================================
echo = CSC: %CSC%
echo = OPT: %OPT%
echo ============================================================

%CSC% %OPT% /nologo /target:winexe /out:PathKunBasic.exe PathKunBasic\PathKunBasic.cs PathKunLib\PathKunLib.cs
%CSC% %OPT% /nologo /target:winexe /out:PathKunPathOnly.exe PathKunPathOnly\PathKunPathOnly.cs PathKunLib\PathKunLib.cs
%CSC% %OPT% /nologo /target:winexe /out:PathKunExplorer.exe PathKunExplorer\PathKunExplorer.cs PathKunLib\PathKunLib.cs

pause
