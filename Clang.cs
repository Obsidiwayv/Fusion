namespace Fusion;

public class Clang
{
    public static string WindowsClangBin { get; } = "C:/Program Files/LLVM/bin";

    public static string CPlusPlusBinary { get; } = OperatingSystem.IsWindows() 
        ? $"{WindowsClangBin}/clang++.exe" : "clang++";
    public static string Binary { get; } = OperatingSystem.IsWindows() 
        ? $"{WindowsClangBin}/clang.exe" : "clang";

    [OperatingSystemOnly(HostOS.Win64)]
    public static string LibEXE { get; } = $"{WindowsClangBin}/llvm-lib.exe";
    
    [OperatingSystemOnly(HostOS.Win64)]
    public static string ResourceCompiler { get; } = $"{WindowsClangBin}/llvm-rc.exe";

    public static List<string> CVersion { get; } = [
        "c11", 
        "c17", 
        "c23"
    ];
    public static List<string> CPPVersion { get; } = [
        "c++14", 
        "c++17", 
        "c++20", 
        "c++23"
    ];
}