namespace FusionLexer.Language;

/// <summary>
/// A class extending List with atomic dsl keywords
/// </summary>
public class AtomicKeywordsList : List<string>
{
    public AtomicKeywordsList() : base([
        "binary",
        "includes"
    ]) {}
}