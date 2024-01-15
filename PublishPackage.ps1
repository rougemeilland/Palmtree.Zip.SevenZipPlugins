$baseDirectory = $PSScriptRoot
$localNugetPackageDirectory = $Env:LOCAL_NUGET_PACKAGE_PATH

if (-not (Test-Path $localNugetPackageDirectory -PathType Container)) {

    Write-Error -Message "ローカルパッケージフォルダが存在しません。LOCAL_NUGET_PACKAGE_PATH 環境変数を確認してください。" -Category NotEnabled
    Exit 2
}

foreach ($projectDirectory in Get-ChildItem $baseDirectory -Directory | Where-Object {$_.Name -match "^Palmtree\.Zip\.SevenZipPlugin\.[A-Za-z0-9_]+$"}) {
    $nugetPackageFiles = Get-ChildItem (Join-Path (Join-Path $projectDirectory.FullName "bin") "Release") -File | Where-Object {$_.Name -match "\.nupkg$"}
    foreach ($nugetPackageFile in $nugetPackageFiles) {
        $command = "nuget"
        $args = "add `"" + $nugetPackageFile.FullName + "`" -Source `"" + $localNugetPackageDirectory + "`""

        $process = Start-Process -FilePath $command -ArgumentList $args -NoNewWindow -PassThru
        $handle = $process.Handle # おまじない https://stackoverflow.com/questions/10262231/obtaining-exitcode-using-start-process-and-waitforexit-instead-of-wait
        $process.WaitForExit()
        if ($process.ExitCode -ne 0) {
            Write-Warning ("nuget の実行に失敗しました。: 終了コード: " + $process.ExitCode)
        }
        else {
            # Copy-Item $nugetPackageFile $localNugetPackageDirectory
        }
    }
}
