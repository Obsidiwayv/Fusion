namespace Fusion.API;

public enum HostOS
{
    Win64,
    Darwin,
    Linux,
    Unknown
}

public class OS
{
    public static string ExecutableEXT { get; } = OperatingSystem.IsWindows()
        ? ".exe"
        : "";

    public static string LibraryEXT { get; } = OperatingSystem.IsWindows()
        ? ".dll"
        : OperatingSystem.IsMacOS()
        ? ".dylib"
        : ".so";

    public static string StaticLibraryEXT { get; } = OperatingSystem.IsWindows()
        ? ".lib" : ".a";

    public static string StaticLibObject { get; } = OperatingSystem.IsWindows()
        ? ".obj" : ".o";

    public static bool IsUnix()
    {
        return OperatingSystem.IsLinux() || OperatingSystem.IsMacOS();
    }

    public static HostOS GetHost()
    {
        if (OperatingSystem.IsWindows()) return HostOS.Win64;
        if (OperatingSystem.IsMacOS()) return HostOS.Darwin;
        if (OperatingSystem.IsLinux()) return HostOS.Linux;
        return HostOS.Unknown;
    }

    public static string GetHostName(HostOS host) => host switch
    {
        HostOS.Win64 => "Windows",
        HostOS.Darwin => "MacOS",
        HostOS.Linux => "Linux",
        _ => "Unknown"
    };
}

public class AtomicFolders
{
    public static string BinaryOutput { get; } = "Bin";
    public static string BuildOutput { get; } = $"{BinaryOutput}/Build";
    public static string ObjOutput { get; } = $"{BinaryOutput}/obj";
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public class OperatingSystemOnly : Attribute
{
    public OperatingSystemOnly(HostOS os)
    {
        if (OS.GetHost() != os)
        {
            Console.WriteLine($"FUSION WARNING: This member does not support {OS.GetHostName(os)}, it may not work");
        }
    }
}