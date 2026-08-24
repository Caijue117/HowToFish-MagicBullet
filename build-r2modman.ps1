param(
    [Parameter(Mandatory=$true)]
    [string]$GameDir,

    [Parameter(Mandatory=$true)]
    [string]$ProfileDir
)

$ErrorActionPreference = "Stop"
$Project = Join-Path $PSScriptRoot "HowToFishMagicBullet.csproj"

$managed = Join-Path $GameDir "How to Fish_Data\Managed"
$bepCore = Join-Path $ProfileDir "BepInEx\core"

$required = @(
    (Join-Path $managed "Assembly-CSharp.dll"),
    (Join-Path $managed "FishNet.Runtime.dll"),
    (Join-Path $managed "UnityEngine.dll"),
    (Join-Path $managed "UnityEngine.CoreModule.dll"),
    (Join-Path $managed "UnityEngine.PhysicsModule.dll"),
    (Join-Path $managed "UnityEngine.InputLegacyModule.dll"),
    (Join-Path $managed "UnityEngine.IMGUIModule.dll"),
    (Join-Path $bepCore "BepInEx.dll"),
    (Join-Path $bepCore "0Harmony.dll")
)

foreach ($file in $required) {
    if (!(Test-Path $file)) {
        throw "Missing required DLL: $file"
    }
}

dotnet build $Project -c Release -p:GameDir="$GameDir" -p:ProfileDir="$ProfileDir"
if ($LASTEXITCODE -ne 0) {
    throw "dotnet build failed. See the compiler errors above."
}

$out = Join-Path $PSScriptRoot "bin\Release\netstandard2.1\HowToFishMagicBullet.dll"
if (!(Test-Path $out)) {
    throw "Build completed but output DLL was not found: $out"
}

$pluginDir = Join-Path $ProfileDir "BepInEx\plugins\HowToFishMagicBullet"
New-Item -ItemType Directory -Force -Path $pluginDir | Out-Null
Copy-Item -Force $out (Join-Path $pluginDir "HowToFishMagicBullet.dll")

Write-Host ""
Write-Host "SUCCESS"
Write-Host "Installed to:"
Write-Host (Join-Path $pluginDir "HowToFishMagicBullet.dll")
Write-Host ""
Write-Host "Launch the game from r2modman using Start modded."
