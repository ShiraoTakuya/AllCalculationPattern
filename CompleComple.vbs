Set WshShell = WScript.CreateObject("WScript.Shell")
WshShell.Run("C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe CompiledSource.cs"),1,true
WshShell.Run("CompiledSource.exe")