cmake_minimum_required(VERSION 3.16)

project(libnode_api LANGUAGES C)

add_library(node_api SHARED node_api.c)

# Add node-addon-api dependency
execute_process(
    COMMAND node -p "require('node-api-headers').include_dir"
    WORKING_DIRECTORY ${PROJECT_SOURCE_DIR}
    OUTPUT_VARIABLE NODE_ADDON_HEADERS_DIR
)
string(REGEX REPLACE "[\r\n\"]" "" NODE_ADDON_HEADERS_DIR ${NODE_ADDON_HEADERS_DIR})
target_include_directories(node_api PRIVATE ${NODE_ADDON_HEADERS_DIR})