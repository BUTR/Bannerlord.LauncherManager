{
    "targets": [
        {
            "target_name": "launchermanager",
            "sources": [
                "src-native/main.cpp"
            ],
            "include_dirs": [
                "<!@(node -p \"require('node-addon-api').include\")",
                "<(module_root_dir)"
            ],
            "conditions": [
                ["OS=='win'", {
                    "include_dirs": [
                        "<(module_root_dir)/vcpkg_installed/x64-windows-static/include"
                    ],
                    "libraries": [
                        "<(module_root_dir)/Bannerlord.LauncherManager.Native.lib"
                    ],
                    "configurations": {
                        "Debug": {
                            "msvs_settings": {
                                "VCLinkerTool": {
                                    "AdditionalDependencies": [
                                        "<(module_root_dir)/vcpkg_installed/x64-windows-static/debug/lib/isa-l_static.lib"
                                    ]
                                }
                            }
                        },
                        "Release": {
                            "msvs_settings": {
                                "VCLinkerTool": {
                                    "AdditionalDependencies": [
                                        "<(module_root_dir)/vcpkg_installed/x64-windows-static/lib/isa-l_static.lib"
                                    ]
                                }
                            }
                        }
                    }
                }],
                ["OS=='linux'", {
                    "include_dirs": [
                        "<(module_root_dir)/vcpkg_installed/x64-linux/include"
                    ],
                    "libraries": [
                        "-L<(module_root_dir)",
                        "-l:Bannerlord.LauncherManager.Native.so",
                        "<(module_root_dir)/vcpkg_installed/x64-linux/lib/libisal.a"
                    ],
                    "ldflags": [
                        "-Wl,-rpath,<(module_root_dir)"
                    ]
                }]
            ],
            "defines": [
                "NAPI_CPP_EXCEPTIONS",
                "NAPI_VERSION=10",
                "_SILENCE_CXX17_CODECVT_HEADER_DEPRECATION_WARNING"
            ],
            "cflags!": [ "-fno-exceptions" ],
            "cflags_cc!": [ "-fno-exceptions" ],
            "cflags_cc": [ "-std=c++20", "-fexceptions" ],
            "msvs_settings": {
                "VCCLCompilerTool": {
                    "AdditionalOptions": [
                        "/EHsc",
                        "/std:c++20"
                    ],
                    "ExceptionHandling": 1,
                    "EnablePREfast": "true"
                }
            }
        }
    ]
}
