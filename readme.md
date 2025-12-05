# Fusion Build Tool

Fusion is a build tool that uses the ATOMIC DSL.

You need `dotnet 9` and `LLVM` installed to use this!

### Usage
Define an atomic file in any directory with a `DIRS` file in the root

### DIRS file
```
someprogram
```
this is recursive so keep a mind for that


### .atomic file
you can name the file this: `example.build.atomic`, `example.atomic` or anything with 
`.atomic` at the end

### .atomic specs
`define_lib` can be omitted for a regular executable
```
binary "program"
version "0.1"
uses "0.1"
language "c++:19"
define_lib "static"
add_includes (
    Include
    Another_Include
)
add_sources (
    main.cc
    example.cc
)
add_libs (
    win32:Deps/GLFW3.lib
    darwin:Deps/GLFW3.a
)
put_assets (
    img/1.png
    img/2.png
)
```