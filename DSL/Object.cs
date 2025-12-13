namespace Fusion.DSL;

public class AtomicProjectFile
{
    public string OutBinary { get; set; } = "";
    public string? Version { get; set; }
    public string Library { get; set; } = "";
    public string? LangVersion { get; set; }
    public string ClangBinary { get; set; } = "";
    public bool IsLibrary { get; set; } = false;
    public List<string> Includes { get; } = [];
    public List<string> Sources { get; } = [];
    public List<string> Libs { get; } = [];

    public string GetString(List<string> strings)
    {
        return string.Join(" ", strings);
    }
}