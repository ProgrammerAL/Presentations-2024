
$presentationPath = "$PSScriptRoot/../"
$demoCodeRootPath = "$PSScriptRoot/../demo-code-feedback-system"

$vsExePath = "C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/devenv.exe"

& $vsExePath "$demoCodeRootPath/src/FeedbackApp.sln"
& $vsExePath "$demoCodeRootPath/build/build/Build.sln"
& $vsExePath "$demoCodeRootPath/infra/PulumiInfra.sln"
& $vsExePath "$demoCodeRootPath/deploy/deploy/Deploy.sln"
& $vsExePath "$presentationPath/../"

& "c:/ZoomIt/ZoomIt.exe"

Start-Process -FilePath MSEdge -ArgumentList "https://programmeral.com/posts/20241114_UpdateConf2024", "https://github.com/ProgrammerAL/Presentations-2024/actions", "https://app.pulumi.com/ProgrammerAl/update-conf-2024/dev", "https://portal.azure.com/#browse/resourcegroups"

