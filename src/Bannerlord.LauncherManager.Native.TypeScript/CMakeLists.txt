cmake_minimum_required(VERSION 3.0)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

project(launchermanager)

add_library(${PROJECT_NAME} SHARED
  src/main.cpp
)

# Set include directories
target_include_directories(${PROJECT_NAME} PRIVATE
  ${CMAKE_JS_INC}
  ${CMAKE_CURRENT_SOURCE_DIR}
)

# Set linker libraries
target_link_libraries(${PROJECT_NAME} PRIVATE
  ${CMAKE_JS_LIB}
  ${CMAKE_CURRENT_SOURCE_DIR}/Bannerlord.LauncherManager.Native.lib
)

# Set compiler options
target_compile_definitions(${PROJECT_NAME} PRIVATE
    NAPI_CPP_EXCEPTIONS
    _SILENCE_CXX17_CODECVT_HEADER_DEPRECATION_WARNING
)
if(MSVC)
  target_compile_options(${PROJECT_NAME} PRIVATE /EHsc)
  target_compile_options(${PROJECT_NAME} PRIVATE /W4)
endif()

# Add node-addon-api dependency
execute_process(
    COMMAND node -p "require('node-addon-api').include"
    WORKING_DIRECTORY ${PROJECT_SOURCE_DIR}
    OUTPUT_VARIABLE NODE_ADDON_API_DIR
)
string(REGEX REPLACE "[\r\n\"]" "" NODE_ADDON_API_DIR ${NODE_ADDON_API_DIR})
target_include_directories(${PROJECT_NAME} PRIVATE ${NODE_ADDON_API_DIR})

add_definitions(-DNAPI_VERSION=6)