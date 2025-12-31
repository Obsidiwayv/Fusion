namespace Fusion.API;


public class FusionConstants
{
    public static string DeveloperXML { get; } = "fusion-dev.xml";
    public static string RootNamespace { get; } = "root://";
}

public class AtomicMap(string key, string value)
{
    public string Key { get; } = key;
    public string Value { get; } = value;
}

