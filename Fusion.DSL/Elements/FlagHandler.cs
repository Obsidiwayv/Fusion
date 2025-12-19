using Fusion.API;

namespace Fusion.DSL.Elements;

public class AtomicFlags
{
    public HostOS Host { get; set; } = HostOS.Unknown;

    public void IsCurrent(string key)
    {
        if (key == "win32 {")
        {
            Host = HostOS.Win64;
        }
        else if (key == "darwin {")
        {
            Host = HostOS.Darwin;
        }
        else if (key == "linux {")
        {
            Host = HostOS.Linux;
        }
        if (key == "}")
        {
            Host = HostOS.Unknown;
        }
    }

    public bool IsWindows()
    {
        return Host == HostOS.Win64 && OperatingSystem.IsWindows();
    }

    public bool IsDarwin()
    {
        return Host == HostOS.Darwin && OperatingSystem.IsMacOS();
    }

    public bool IsLinux()
    {
        return Host == HostOS.Linux && OperatingSystem.IsLinux();
    }
}