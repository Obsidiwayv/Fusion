using Fusion.API;

namespace Fusion.DSL;

public class XMLOptions
{
    public bool? GenerateCompileDatabase { get; set; }
}

/// <summary>
/// Class used for defining multiple atomic map lists in the project
/// </summary>
public class AtomicTokenDict : Dictionary<AtomicTokenEnum, string> {}

public class AtomicContext
{
    public required XMLOptions BuildOptions { get; init; }

    public AtomicCompileDatabase database;

    public AtomicContext()
    {
        database = new(this);
    }

    public static AtomicMapList UseLexer(string file)
    {
        return new AtomicLexer(
            File.ReadAllText(file).ToCharArray()).Use();
    }

    public ReadonlyAtomicResult UseParser(AtomicMapList maps, string file)
    {
        return new AtomicParser(maps,
                $"{Path.GetDirectoryName(file.TrimEnd(Path.DirectorySeparatorChar))}/" ?? "", this)
                .Use(true);
    }
}