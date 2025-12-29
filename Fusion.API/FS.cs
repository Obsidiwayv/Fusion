namespace Fusion.API;

public class FusionFS
{
    public static List<string>? SearchDir(string directory)
    {
        IEnumerable<string> atomics = Directory.EnumerateFiles(
            directory, "*.atomic", SearchOption.AllDirectories);

        if (!atomics.Any()) return null;
        return [.. atomics];
    }
}