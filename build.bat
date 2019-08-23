set CSC=
set OPT=

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

%CSC% %OPT% /target:winexe /out:path-kun.exe path-kun.cs path-lib.cs
%CSC% %OPT% /target:winexe /out:path-kun-explorer.exe path-kun-explorer.cs path-lib.cs
%CSC% %OPT% /target:winexe /out:path-kun-pathonly.exe path-kun-pathonly.cs path-lib.cs

pause
