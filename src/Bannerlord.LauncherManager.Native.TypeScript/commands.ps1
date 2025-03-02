param([string]$type, [string]$Configuration = "Release")

$ErrorActionPreferenceOld = $ErrorActionPreference;
$ErrorActionPreference = "Stop";

function Copy-Item2([System.String] $Path, [System.String] $Destination) {
    $directory = [System.IO.Path]::GetDirectoryName([System.IO.Path]::GetFullPath($Destination));
    New-Item -Type Directory -Path $directory -Force;
    Copy-Item -Path $Path -Destination $Destination -Recurse -Force;
}

try {
    # Clean
    if ($type -eq "build" -or $type -eq "test" -or $type -eq "clear") {
        Remove-Item *.tgz, *.h, *.dll, *.lib, build, dist, coverage, .nyc_output -Recurse -Force -ErrorAction Ignore;
    }
    # Build C#
    if ($type -eq "build" -or $type -eq "test" -or $type -eq "build-native") {
        Write-Host "Building Bannerlord.LauncherManager.Native ($Configuration)";

        Invoke-Command -ScriptBlock {
            dotnet publish -r win-x64 --self-contained -c $Configuration ../Bannerlord.LauncherManager.Native;
        }
        
        Copy-Item2 -Path "../Bannerlord.LauncherManager.Native/bin/$Configuration/net9.0/win-x64/native/Bannerlord.LauncherManager.Native.dll" -Destination $PWD | Out-Null;
        Copy-Item2 -Path "../Bannerlord.LauncherManager.Native/bin/$Configuration/net9.0/win-x64/native/Bannerlord.LauncherManager.Native.lib" -Destination $PWD | Out-Null;
        Copy-Item2 -Path "../Bannerlord.LauncherManager.Native/bin/$Configuration/net9.0/win-x64/Bannerlord.LauncherManager.Native.h" -Destination $PWD | Out-Null;
    }
    # Build NAPI
    if ($type -eq "build" -or $type -eq "test" -or $type -eq "build-napi") {
        Write-Host "Building NAPI ($Configuration)";

        $tag = "";
        $tag = If ($Configuration -eq "Release") { "--release" } Else { $tag }
        $tag = If ($Configuration -eq "Debug") { "--debug" } Else { $tag }
        Invoke-Command -ScriptBlock {
            npx node-gyp rebuild --arch=x64 $tag;
            #npx cmake-js compile --arch=x64 $tag;
        }
    }
    # Build JS
    if ($type -eq "build" -or $type -eq "test" -or $type -eq "test-build" -or $type -eq "build-ts") {
        Write-Host "Building @butr/vortexextensionnative";

        Invoke-Command -ScriptBlock {
            npx tsc -p tsconfig.json;
            npx tsc -p tsconfig.module.json;
        }
    }
    if ($type -eq "build" -or $type -eq "test" -or $type -eq "test-build" -or $type -eq "build-content") {
        Write-Host "Copying content";

        Copy-Item2 -Path "Bannerlord.LauncherManager.Native.dll" -Destination "dist" | Out-Null;
        Copy-Item2 -Path "build/$Configuration/launchermanager.node" -Destination "dist" | Out-Null;
        
        Copy-Item2 -Path "src/test/Version.xml" -Destination "dist/main/test/bin/Win64_Shipping_Client/" | Out-Null;
        Copy-Item2 -Path "src/test/Harmony.xml" -Destination "dist/main/test/Modules/Bannerlord.Harmony/SubModule.xml" | Out-Null;
        Copy-Item2 -Path "src/test/UIExtenderEx.xml" -Destination "dist/main/test/Modules/Bannerlord.UIExtenderEx/SubModule.xml" | Out-Null;
        
        Copy-Item2 -Path "src/test/Version.xml" -Destination "dist/module/test/bin/Win64_Shipping_Client/" | Out-Null;
        Copy-Item2 -Path "src/test/Harmony.xml" -Destination "dist/module/test/Modules/Bannerlord.Harmony/SubModule.xml" | Out-Null;
        Copy-Item2 -Path "src/test/UIExtenderEx.xml" -Destination "dist/module/test/Modules/Bannerlord.UIExtenderEx/SubModule.xml" | Out-Null;
    }
    if ($type -eq "test" -or $type -eq "test-build" -or $type -eq "test-no-build") {
        Write-Host "Testing with Configuration $Configuration";

        Invoke-Command -ScriptBlock {
            npx cspell '{README.md,.github/*.md,src/**/*.ts,src/**/*.js}';
            npx nyc ava -- $Configuration;
        }
    }
}
finally {
    $ErrorActionPreference = $ErrorActionPreferenceOld;
}

