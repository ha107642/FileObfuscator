%systemroot%\Microsoft.NET\Framework\v4.0.30319\msbuild "FileObfuscator.csproj" /p:Configuration=Release;DebugSymbols=false;DebugType=None
xcopy "bin\Release\FileObfuscator.exe" "%ProgramFiles%\FileObfuscator\" /Y

set TARGET='%ProgramFiles%\FileObfuscator\FileObfuscator.exe'
set SHORTCUT='%USERPROFILE%\AppData\Roaming\Microsoft\Windows\SendTo\File Obfuscator.lnk'
set PWS=powershell.exe -ExecutionPolicy Bypass -NoLogo -NonInteractive -NoProfile

%PWS% -Command "$ws = New-Object -ComObject WScript.Shell; $S = $ws.CreateShortcut(%SHORTCUT%); $S.TargetPath = %TARGET%; $S.Arguments = '-o'; $S.Save()"