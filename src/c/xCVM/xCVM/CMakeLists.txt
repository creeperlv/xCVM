# CMakeList.txt : CMake project for xCVM, include source and define
# project specific logic here.
#

# Add source to this project's executable.
add_executable (xCVM "xCVM.c" "xCVM.h")

target_link_libraries(xCVM "libxCVM")

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET xCVM PROPERTY CXX_STANDARD 20)
endif()

# TODO: Add tests and install targets if needed.
