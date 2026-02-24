#pragma warning disable IDE0056
#pragma warning disable IDE0028

using System.Text;

namespace FusionLexer.Language;

public class Symbols
{
    public static char StringLiteral { get; } = '"';
    public static char BlockStart { get; } = '{';
    public static char BlockEnd { get; } = '}';

    public static bool IsSymbol(char c)
    {
        return c == StringLiteral ||
            c == BlockStart ||
            c == BlockEnd;
    }

    public static bool IsBracket(char c)
    {
        return c == BlockStart ||
            c == BlockEnd;
    }
}


public class LanguageLexer
{
    public AtomicStringFactory StringFactory;
    public AtomicArrayFactory ArrayFactory;

    public AtomicNodesDict Nodes = [];

    public List<string> AssemblyMap = [];

    public AtomicKeywordsList Keywords = new();

    public StringBuilder Word { get; set; } = new();

    public bool BinaryBlockActive = false;

    public LexerPipeline Pipeline;

    public LanguageLexer(LexerPipeline pipeline)
    {
        StringFactory = new(this);
        Pipeline = pipeline;
    }

    public void RunSetup()
    {
        // For setup, nothing will be here for now
    }

    public void RunLexer(ref int index, char character)
    {
        if (!Symbols.IsSymbol(character) 
            || !char.IsWhiteSpace(character))
        {
            Word.Append(character);
        }

        if (Keywords.Contains(Word.ToString()))
        {
            Nodes.Add(NodeType.Keyword, new()
            {
                Value = Word.ToString()
            });
            AssemblyMap.Add(Word.ToString());

            Word.Clear();
            return;
        }
        if (character == Symbols.StringLiteral)
        {
            index++;
            string str = StringFactory.Parse(ref index);
            Nodes.Add(NodeType.StringLiteral, new()
            {
                Value = str
            });
            AssemblyMap.Add(str);
            return;
        }
        HandleBrackets(ref index, character);

        foreach (var (key, node) in Nodes)
        {
            Console.WriteLine($"key: {key}, val: {node.Value}");
        }
    }

    /// <summary>
    /// Will turn the provided code within the brackets into an array
    /// Depending on if "binary" is defined 2 characters behind
    /// </summary>
    private void HandleBrackets(ref int index, char character)
    {
        if (character == Symbols.BlockStart &&
            // Get the 2nd to current keyword
            // and compare to make sure this is a binary block
            AssemblyMap[AssemblyMap.Count - 2] != Keywords[1])
        {
            // This is for the check at the end
            BinaryBlockActive = true;
        } else if (character == Symbols.BlockStart)
        {
            Nodes.Add()
        }
    }
}