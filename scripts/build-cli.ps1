# =============================================================================
# build-cli.ps1 — Build Confuser.CLI and package ConfuserEx-CLI.zip
#
# Usage:
#   .\scripts\build-cli.ps1
#   .\scripts\build-cli.ps1 -Configuration Debug
#   .\scripts\build-cli.ps1 -OutDir .\dist
#
# Requires: .NET 10 SDK
# Output:   <OutDir>\ConfuserEx-CLI.zip  (+ unpacked folder for direct use)
# =============================================================================

[CmdletBinding()]
param(
    [ValidateSet('Release', 'Debug')]
    [string] $Configuration = 'Release',

    [string] $OutDir = 'artifacts'
)

$ErrorActionPreference = 'Stop'

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $RepoRoot

$Tfm = 'net10.0'
$CliProject = 'Confuser.CLI\Confuser.CLI.csproj'
$RuntimeProject = 'Confuser.Runtime\Confuser.Runtime.csproj'
$BuildOut = Join-Path $RepoRoot "Confuser.CLI\bin\$Configuration\$Tfm"
$OutDirFull = Join-Path $RepoRoot $OutDir
$ZipPath = Join-Path $OutDirFull 'ConfuserEx-CLI.zip'
$UnpackedDir = Join-Path $OutDirFull 'cli'

Write-Host ''
Write-Host "============================================"
Write-Host " Building Confuser.CLI ($Configuration / $Tfm)"
Write-Host "============================================"
Write-Host ''

# Runtime is copied into CLI output but is not a ProjectReference — build it first.
dotnet build $RuntimeProject -c $Configuration --verbosity minimal
if ($LASTEXITCODE -ne 0) { throw "Confuser.Runtime build failed (exit $LASTEXITCODE)" }

dotnet build $CliProject -c $Configuration --verbosity minimal
if ($LASTEXITCODE -ne 0) { throw "Confuser.CLI build failed (exit $LASTEXITCODE)" }

$ExePath = Join-Path $BuildOut 'Confuser.CLI.exe'
if (-not (Test-Path $ExePath)) {
    throw "Build succeeded but Confuser.CLI.exe not found at: $BuildOut"
}

Write-Host ''
Write-Host " Packaging -> $OutDir"
New-Item -ItemType Directory -Path $OutDirFull -Force | Out-Null

if (Test-Path $UnpackedDir) { Remove-Item $UnpackedDir -Recurse -Force }
New-Item -ItemType Directory -Path $UnpackedDir -Force | Out-Null

Get-ChildItem $BuildOut -Exclude '*.pdb', '*.xml' |
    Copy-Item -Destination $UnpackedDir -Recurse -Force

if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
Compress-Archive -Path (Join-Path $UnpackedDir '*') -DestinationPath $ZipPath

$SizeMb = [math]::Round((Get-Item $ZipPath).Length / 1MB, 1)

Write-Host ''
Write-Host "============================================"
Write-Host " Done"
Write-Host "============================================"
Write-Host "  Zip:      $ZipPath  ($SizeMb MB)"
Write-Host "  Unpacked: $UnpackedDir"
Write-Host "  Run:      $UnpackedDir\Confuser.CLI.exe <project.crproj>"
Write-Host "  Note:     requires .NET 10 Runtime/SDK"
Write-Host ''
