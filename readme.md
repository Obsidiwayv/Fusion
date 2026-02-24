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
```
binary "binary_name" {
    type = executable
    includes {
        "Public"
        "Private"
    }
}
```

### Atomic file DNA
```
binary
"binary_name" - Name of the binary
{} - start end bin/lib block
type = executable/shared_library/static_library
includes {} - I know this looks confusing but this bracket is also an array
```


### 3rd/1st party libraries
Directories with os names contain their own link and runtime files

LibraryDir 
    | Macos
    | Linux
    | Win64
    | Include
        | LibraryName