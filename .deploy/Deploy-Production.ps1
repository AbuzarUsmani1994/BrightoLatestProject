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

# Back up the current live version of every file this deploy is about to touch,
# and remember which relative paths are brand new (no prior live file) so a
# rollback can delete them again instead of leaving them behind.
$newFiles = Get-ChildItem -Path $PublishDir -Recurse -File
$relativePaths = @()
$newlyAddedPaths = @()
$backedUpCount = 0
foreach ($file in $newFiles) {
    $relativePath = $file.FullName.Substring($PublishDir.Length).TrimStart('\')
    $relativePaths += $relativePath
    $liveFile = Join-Path $SiteRoot $relativePath
    if (Test-Path $liveFile) {
        $backupTarget = Join-Path $backupDir $relativePath
        New-Item -ItemType Directory -Path (Split-Path $backupTarget) -Force | Out-Null
        Copy-Item -Path $liveFile -Destination $backupTarget -Force
        $backedUpCount++
    }
    else {
        $newlyAddedPaths += $relativePath
    }
}
Write-Host "Backed up $backedUpCount existing file(s)."

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
}
catch {
    Write-Warning "Deploy failed, rolling back from $backupDir : $_"
    foreach ($relativePath in $relativePaths) {
        $liveTarget = Join-Path $SiteRoot $relativePath
        if ($newlyAddedPaths -contains $relativePath) {
            Remove-Item -Path $liveTarget -Force -ErrorAction SilentlyContinue
        }
        else {
            $backupSource = Join-Path $backupDir $relativePath
            Copy-Item -Path $backupSource -Destination $liveTarget -Force
        }
    }
    Remove-Item $appOffline -Force -ErrorAction SilentlyContinue
    throw
}

Remove-Item $appOffline -Force -ErrorAction SilentlyContinue
Write-Host "Deployment complete. Commit $CommitSha is live at $SiteRoot."
