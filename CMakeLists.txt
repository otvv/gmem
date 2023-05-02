# cmake settings
cmake_minimum_required(VERSION 3.0)
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall -Wextra")

# project settings
set(CMAKE_CXX_STANDARD 17)
project(gmem-test)
include_directories(${CMAKE_SOURCE_DIR}/src)

# executable
add_executable(gmem-test
    src/main.cpp
)
