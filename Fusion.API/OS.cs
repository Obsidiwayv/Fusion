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

    public static string GetExecutableExtension()
    {
        if (GetHost() == HostOS.Win64)
        {
            return ".exe";
        } else
        {
            // Linux or darwin
            return "";
        }
    }

    public static string SharedLibraryExtension(HostOS os) => os switch
    {
        HostOS.Win64 => ".dll",
        HostOS.Darwin => ".dylib",
        HostOS.Linux => ".so",
        _ => FusionConstants.UnknownKey
    };

    /// <returns>
    /// [linker-file-ext, compiled-object-file-ext]
    /// both could be {FusionConstants.UnknownKey}, if the os is unknown
    /// </returns>
    public static string[] PlatformStaticLinkObjects(HostOS os) => os switch
    {
        HostOS.Win64 => [".lib", ".obj"],
        HostOS.Darwin => [".a", ".o"],
        HostOS.Linux => [".a", ".o"],
        _ => [FusionConstants.UnknownKey, FusionConstants.UnknownKey]
    };

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