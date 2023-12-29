xcopy ..\bin\Release\net6.0\FewerPasscodeNotes.dll FewerPasscodeNotes /y
xcopy ..\README.md FewerPasscodeNotes /y

del FewerPasscodeNotes.zip
powershell Compress-Archive FewerPasscodeNotes FewerPasscodeNotes.zip
