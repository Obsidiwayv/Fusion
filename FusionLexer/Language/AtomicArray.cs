using System.Text;

namespace FusionLexer.Language;

public class AtomicArrayFactory(LanguageLexer lexer)
{
    /// <summary>
    /// Parse characters into a full string
    /// </summary>
    public List<string> Parse(ref int index)
    {
        lexer.Nodes.Add(NodeType.Array, new()
        {
            Value = "START"
        });

        List<string> array = [];
        for (int sIndex = index + 1;
            sIndex < lexer.Pipeline.FileArray.Length;
            sIndex++)
        {
            char subCharacter = lexer.Pipeline.FileArray[sIndex];

            // Stop the loop when the string has hit another quote
            if (subCharacter == Symbols.BlockEnd)
            {
                break;
            }
            // Parse each character into a string that can fit into a list
            array.Add(lexer.StringFactory.Parse(ref sIndex));
        }
        
        lexer.Nodes.Add(NodeType.Array, new()
        {
            Value = $"END {array.Count}"
        });
        return array;
    }
}