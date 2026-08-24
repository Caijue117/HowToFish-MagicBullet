param(
    [string]$GameDir = ""
)

$ErrorActionPreference = "Stop"
$Project = Join-Path $PSScriptRoot "HowToFishMagicBullet.csproj"

if ([string]::IsNullOrWhiteSpace($GameDir)) {
    $candidates = @(
        "C:\Program Files (x86)\Steam\steamapps\common\How to Fish",
        "C:\Program Files\Steam\steamapps\common\How to Fish"
    )

    foreach ($candidate in $candidates) {
        if (Test-Path (Join-Path $candidate "How to Fish_Data\Managed\Assembly-CSharp.dll")) {
            $GameDir = $candidate
            break
        }
    }
}

if ([string]::IsNullOrWhiteSpace($GameDir)) {
    throw 'Could not find the game automatically. Run: .\build.ps1 -GameDir "D:\SteamLibrary\steamapps\common\How to Fish"'
}

$managed = Join-Path $GameDir "How to Fish_Data\Managed"
$bepCore = Join-Path $GameDir "BepInEx\core"

$required = @(
    (Join-Path $managed "Assembly-CSharp.dll"),
    (Join-Path $managed "FishNet.Runtime.dll"),
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

dotnet build $Project -c Release -p:GameDir="$GameDir"

$out = Join-Path $PSScriptRoot "bin\Release\netstandard2.1\HowToFishMagicBullet.dll"
if (!(Test-Path $out)) {
    throw "Build completed but output DLL was not found: $out"
}

$pluginDir = Join-Path $GameDir "BepInEx\plugins\HowToFishMagicBullet"
New-Item -ItemType Directory -Force -Path $pluginDir | Out-Null
Copy-Item -Force $out (Join-Path $pluginDir "HowToFishMagicBullet.dll")

Write-Host ""
Write-Host "Installed:"
Write-Host (Join-Path $pluginDir "HowToFishMagicBullet.dll")
Write-Host ""
Write-Host "F8  = toggle"
Write-Host "[   = FOV -"
Write-Host "]   = FOV +"
