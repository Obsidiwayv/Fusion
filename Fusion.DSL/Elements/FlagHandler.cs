using Fusion.API;

namespace Fusion.DSL.Elements;

public class AtomicFlags
{
    public HostOS Host { get; set; } = HostOS.Unknown;

    public bool Continue { get; set; } = true;

    public void IsCurrent(string key)
    {
        Console.WriteLine(key);
        if (key == "}")
        {
            Continue = false;
        }
        else
        {
            // If the Continue prop is false then assign true so we dont accidentlly 
            // parse the beginning bracket
            if (!Continue) Continue = true;
        }
        
        if (key == "win32 {")
        {
            Continue = false;
            Host = HostOS.Win64;
        }
        else if (key == "darwin {")
        {
            Continue = false;
            Host = HostOS.Darwin;
        }
        else if (key == "linux {")
        {
            Continue = false;
            Host = HostOS.Linux;
        }
        Console.WriteLine(Continue);
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