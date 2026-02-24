using FusionLexer;

namespace FusionC;

public class Program
{
    private static string DirectoryFile = "DIRS";

    public static void Main(string[] args)
    {
        if (!File.Exists(DirectoryFile))
            throw new Exception($"{DirectoryFile} does not exist in the root");

        foreach (var line in File.ReadAllLines(DirectoryFile))
        {
            var lookup = LexerPrelude
                .LookupAtomicFiles(line, out List<string> fMap);
            if (lookup.Status == FusionAPI.FusionStatus.Error)
            {
                throw new FileLoadException(lookup.Message);
            }

            LexerPrelude.ParseAll(fMap, (dict) => {});
        }
    }
}