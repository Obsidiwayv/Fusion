using Fusion.API;

namespace Fusion.DSL;

public class XMLOptions
{
    public bool? GenerateCompileDatabase { get; set; }
}

public class AtomicContext
{
    public required XMLOptions BuildOptions { get; init; }

    public List<AtomicMap> UseLexer(string file)
    {
        return new AtomicLexer(
            File.ReadAllText(file).ToCharArray(), this).Use();
    }

    public ReadonlyAtomicResult UseParser(List<AtomicMap> maps, string file)
    {
        return new AtomicParser(maps,
                $"{Path.GetDirectoryName(file.TrimEnd(Path.DirectorySeparatorChar))}/" ?? "", this)
                .Use(true);
    }
}