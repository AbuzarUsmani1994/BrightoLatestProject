param(
    [Parameter(Mandatory = $true)][string]$PublishDir,
    [Parameter(Mandatory = $true)][string]$SiteRoot,
    [Parameter(Mandatory = $true)][string]$BackupRoot,
    [Parameter(Mandatory = $true)][string]$CommitSha
)

$ErrorActionPreference = 'Stop'

$PublishDir = (Resolve-Path $PublishDir).Path.TrimEnd('\')
$SiteRoot   = (Resolve-Path $SiteRoot).Path.TrimEnd('\')

$stamp     = Get-Date -Format 'yyyyMMdd_HHmmss'
$shortSha  = $CommitSha.Substring(0, 7)
$backupDir = Join-Path $BackupRoot "$stamp`_$shortSha"

New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
Write-Host "Backing up files about to be overwritten to $backupDir"

# Back up only the live files this deploy will actually change (by content
# hash, not just presence), and remember which relative paths are brand new
# (no prior live file) so a rollback can delete them instead of leaving them
# behind. Files that are byte-identical between live and publish output are
# left alone entirely - nothing to back up, nothing to roll back.
$newFiles = Get-ChildItem -Path $PublishDir -Recurse -File
$relativePaths = @()
$newlyAddedPaths = New-Object 'System.Collections.Generic.HashSet[string]'
$changedPaths = New-Object 'System.Collections.Generic.HashSet[string]'
$publishedHashes = @{}
$backedUpCount = 0
foreach ($file in $newFiles) {
    $relativePath = $file.FullName.Substring($PublishDir.Length).TrimStart('\')
    $relativePaths += $relativePath
    $newHash = (Get-FileHash -Path $file.FullName -Algorithm SHA256).Hash
    $publishedHashes[$relativePath] = $newHash
    $liveFile = Join-Path $SiteRoot $relativePath
    if (Test-Path $liveFile) {
        $liveHash = (Get-FileHash -Path $liveFile -Algorithm SHA256).Hash
        if ($liveHash -ne $newHash) {
            $backupTarget = Join-Path $backupDir $relativePath
            New-Item -ItemType Directory -Path (Split-Path $backupTarget) -Force | Out-Null
            Copy-Item -Path $liveFile -Destination $backupTarget -Force
            $backedUpCount++
            [void]$changedPaths.Add($relativePath)
        }
    }
    else {
        [void]$newlyAddedPaths.Add($relativePath)
    }
}
Write-Host "Backed up $backedUpCount changed file(s) out of $($newFiles.Count) published file(s)."

# Gracefully unload the app so locked bin\*.dll files can be replaced. Poll for
# up to 30s instead of a flat sleep, since IIS can take longer than 5s to
# release file handles under load.
$appOffline = Join-Path $SiteRoot 'App_Offline.htm'
Set-Content -Path $appOffline -Value '<html><body>Deploying an update, back in a moment...</body></html>'
$sampleDll = Get-ChildItem -Path (Join-Path $PublishDir 'bin') -Filter '*.dll' -File -ErrorAction SilentlyContinue | Select-Object -First 1
$waited = 0
while ($sampleDll -and $waited -lt 30) {
    $liveDll = Join-Path $SiteRoot "bin\$($sampleDll.Name)"
    if (-not (Test-Path $liveDll)) { break }
    try {
        $stream = [System.IO.File]::Open($liveDll, 'Open', 'ReadWrite', 'None')
        $stream.Close()
        break
    }
    catch {
        Start-Sleep -Seconds 2
        $waited += 2
    }
}

try {
    # /R:3 /W:5 fails fast on locked files instead of robocopy's default
    # 1,000,000 retries at 30s each, which would hang the job for hours.
    robocopy $PublishDir $SiteRoot /E /R:3 /W:5 /NFL /NDL /NJH /NJS /NC /NS
    if ($LASTEXITCODE -ge 8) {
        throw "robocopy failed with exit code $LASTEXITCODE"
    }

    # robocopy's exit code alone isn't always trustworthy (e.g. an access-denied
    # error against the destination root itself can exhaust its retries without
    # the exit code reflecting a hard failure), so confirm on disk that every
    # file robocopy was supposed to write actually landed with the right content.
    $verifyFailures = @()
    foreach ($relativePath in ($changedPaths + $newlyAddedPaths)) {
        $liveTarget = Join-Path $SiteRoot $relativePath
        if (-not (Test-Path $liveTarget)) {
            $verifyFailures += $relativePath
            continue
        }
        $liveHashAfter = (Get-FileHash -Path $liveTarget -Algorithm SHA256).Hash
        if ($liveHashAfter -ne $publishedHashes[$relativePath]) {
            $verifyFailures += $relativePath
        }
    }
    if ($verifyFailures.Count -gt 0) {
        throw "Deploy verification failed: $($verifyFailures.Count) file(s) were not correctly copied to $SiteRoot (robocopy exit code was $LASTEXITCODE). First mismatch: $($verifyFailures[0])"
    }
}
catch {
    Write-Warning "Deploy failed, rolling back from $backupDir : $_"
    foreach ($relativePath in $relativePaths) {
        $liveTarget = Join-Path $SiteRoot $relativePath
        if ($newlyAddedPaths.Contains($relativePath)) {
            Remove-Item -Path $liveTarget -Force -ErrorAction SilentlyContinue
        }
        elseif ($changedPaths.Contains($relativePath)) {
            $backupSource = Join-Path $backupDir $relativePath
            Copy-Item -Path $backupSource -Destination $liveTarget -Force
        }
        # else: file was byte-identical pre-deploy, nothing to roll back
    }
    Remove-Item $appOffline -Force -ErrorAction SilentlyContinue
    throw
}

Remove-Item $appOffline -Force -ErrorAction SilentlyContinue
Write-Host "Deployment complete. Commit $CommitSha is live at $SiteRoot."
