namespace Fusion.API;


public class FusionConstants
{
    public static string DeveloperXML { get; } = "fusion-dev.xml";
    public static string RootNamespace { get; } = "root://";
    public static string UnknownKey { get; } = "NONE";
}

public class AtomicMap<K>(K key, string value)
{
    public K Key { get; } = key;
    public string Value { get; } = value;
}

