using Fusion.DSL;

namespace Fusion;

public class Program
{
    public static string BinaryOutput { get; } = "Bin";
    public static string BuildOutput { get; } = $"{BinaryOutput}/Build";
    public static string ObjOutput { get; } = $"{BinaryOutput}/obj";

    public static void Main(string[] args)
    {
        VerifyDir(BinaryOutput);
        VerifyDir(BuildOutput);
        VerifyDir(ObjOutput);

        File.ReadAllLines("DIRS")
            .ToList()
            // Read each folder path
            .ForEach(folderPath =>
            {
                // Read multiple .atomic files
                List<string>? atomicFiles = SearchDir(folderPath);
                // Load each .atomic file
                atomicFiles?.ForEach(file => {
                    List<AtomicMap> maps = AtomicLexer.Use(
                        File.ReadAllText(file).ToCharArray());

                    ReadonlyAtomicResult res = new AtomicParser(maps, 
                        $"{Path.GetDirectoryName(file.TrimEnd(Path.DirectorySeparatorChar))}/" 
                        ?? "")
                        .Use();
                    if (res.Status == AtomicStatus.ERROR)
                    {
                        throw new Exception(res.Message);
                    }
                });
            });
        Directory.Delete(ObjOutput, true);
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