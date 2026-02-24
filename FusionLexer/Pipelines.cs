using FusionAPI;
using FusionLexer.Language;

namespace FusionLexer;

public class AtomicNodesDict : Dictionary<NodeType, AtomicNode> {}

public class LexerPrelude
{
    public static char[] LoadAtomicFile(string path)
    {
        return File.ReadAllText(path).ToCharArray();
    }

    public static void ParseAll(List<string> files, Action<AtomicNodesDict> func)
    {
        files.ForEach(file =>
        {
            var pipeline = new LexerPipeline(LoadAtomicFile(file))
                .ParseFile();
            // Execute a function everytime a file is parsed
            func(pipeline);
        });
    }

    /// <summary>
    /// Searches the directory for .atomic files 
    /// </summary>
    /// <returns>Result</returns>
    public static Result LookupAtomicFiles(string directory, out List<string> fileMap)
    {
        try
        {
            fileMap = Directory.EnumerateFileSystemEntries(directory)
                .Where(item => item.EndsWith(Extensions.AtomicFile))
                .ToList();

            return new(FusionStatus.Done, "Finished file lookup");
        }
        catch (Exception e)
        {
            // Return an empty fileMap because it wont matter anyway
            fileMap = [];
            return new(FusionStatus.Error, e.Message);
        }
    }
}

/// <summary>
/// Pipeline that lexes an atomic file
/// </summary>
public class LexerPipeline(char[] fileCharArray)
{
    public char[] FileArray { get; } = fileCharArray;

    public AtomicNodesDict Tokens = [];

    /// <summary>
    /// Will parse an .atomic file into a usable dictionary
    /// </summary>
    public AtomicNodesDict ParseFile()
    {
        var language = new LanguageLexer(this);
        language.RunSetup();

        for (int index = 0; index < FileArray.Length; index++)
        {
            language.RunLexer(ref index, FileArray[index]);
        }

        return language.Nodes;
    }
}