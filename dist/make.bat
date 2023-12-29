xcopy ..\bin\Release\net6.0\FewerPasscodeNotes.dll FewerPasscodeNotes /y
xcopy ..\README.md FewerPasscodeNotes /y

powershell -Command "$data = Get-Content 'FewerPasscodeNotes/manifest.json' -raw | ConvertFrom-Json;$major,$minor,$build = $data.version_number.Split('.');$build = 1 + $build;$data.version_number = $major,$minor,$build -join '.';$data | ConvertTo-Json -depth 32| set-content 'FewerPasscodeNotes/manifest.json'"

del FewerPasscodeNotes.zip
powershell Compress-Archive FewerPasscodeNotes FewerPasscodeNotes.zip
