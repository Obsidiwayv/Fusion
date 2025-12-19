using Fusion.DSL;

namespace Fusion.Core;
using Fusion.API;

public class Program
{

    public static void Main(string[] args)
    {
        VerifyDir(AtomicFolders.BinaryOutput);
        VerifyDir(AtomicFolders.BuildOutput);
        VerifyDir(AtomicFolders.ObjOutput);

        File.ReadAllLines("DIRS")
            .ToList()
            // Read each folder path
            .ForEach(folderPath =>
            {
                // Read multiple .atomic files
                List<string>? atomicFiles = SearchDir(folderPath);
                // Load each .atomic file
                atomicFiles?.ForEach(file => {
                    List<AtomicMap> maps = new AtomicLexer(
                        File.ReadAllText(file).ToCharArray()
                    ).Use();

                    ReadonlyAtomicResult res = new AtomicParser(maps, 
                        $"{Path.GetDirectoryName(file.TrimEnd(Path.DirectorySeparatorChar))}/" 
                        ?? "")
                        .Use();
                    if (res.Status == AtomicStatus.ERROR)
                    {
                        throw new Exception($"\x1B[31m{res.Message}\x1B[0m");
                    }
                });
            });
        Directory.Delete(AtomicFolders.ObjOutput, true);
    }

    public static void VerifyDir(string directory)
    {
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
    }

    public static List<string>? SearchDir(string directory)
    {
        IEnumerable<string> atomics = Directory.EnumerateFiles(
            directory, "*.atomic", SearchOption.AllDirectories);

        if (!atomics.Any()) return null;
        return [.. atomics];
    }

    public static string GetProjectName(string file)
    {
        return file.Split(".")[0];
    }
}