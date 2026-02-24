using System.Text;

namespace FusionLexer.Language;

public class AtomicStringFactory(LanguageLexer lexer)
{
    /// <summary>
    /// Parse characters into a full string
    /// </summary>
    public string Parse(ref int index)
    {
        lexer.Nodes.Add(NodeType.StringLiteral, new()
        {
            Value = "START"
        });

        StringBuilder str = new();
        for (int sIndex = index + 1;
            sIndex < lexer.Pipeline.FileArray.Length;
            sIndex++)
        {
            char subCharacter = lexer.Pipeline.FileArray[sIndex];

            // Stop the loop when the string has hit another quote
            if (subCharacter == Symbols.StringLiteral)
            {
                break;
            }
            str.Append(subCharacter);
        }
        
        lexer.Nodes.Add(NodeType.StringLiteral, new()
        {
            Value = $"END {str.Length}"
        });
        return str.ToString();
    }
}