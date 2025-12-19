using Fusion.API;

namespace Fusion.DSL.Elements;

public class AtomicFlags
{
    public HostOS Host { get; set; } = HostOS.Unknown;

    public bool Continue { get; set; } = true;

    public void IsCurrent(string key)
    {
        if (key == "win32")
        {
            Host = HostOS.Win64;
        }
        else if (key == "darwin")
        {
            Host = HostOS.Darwin;
        }
        else if (key == "linux")
        {
            Host = HostOS.Linux;
        }
        if (key == "}")
        {
            Host = HostOS.Unknown;
        }
        if (key == "{")
        {
            Continue = false;
        }
        else
        {
            // If the Continue prop is false then assign true so we dont accidentlly 
            // parse the beginning bracket
            if (!Continue) Continue = true;
        }
    }

    public bool IsWindows()
    {
        return Host == HostOS.Win64
            && OperatingSystem.IsWindows()
            && Continue;
    }

    public bool IsDarwin()
    {
        return Host == HostOS.Darwin
            && OperatingSystem.IsMacOS()
            && Continue;
    }

    public bool IsLinux()
    {
        return Host == HostOS.Linux
            && OperatingSystem.IsLinux()
            && Continue;
    }
}