using FusionLexer.Language;

namespace FusionLexer;

public enum NodeType
{
    Array,
    Block,
    StringLiteral,
    Keyword
}

public class AtomicNode()
{
    public List<string> Children { get; set; } = [];

    public string? Value { get; set; }

    public bool HasArray()
    {
        return Children.Count != 0;
    }

    public bool HasValue()
    {
        return !string.IsNullOrEmpty(Value);
    }
}