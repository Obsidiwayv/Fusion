namespace Fusion.API;

public delegate ReadonlyAtomicResult FusionFunctionWithResult(string file);

public class FusionSteps
{
    public static void Run(FusionFunctionWithResult functionWithResult)
    {
            File.ReadAllLines("DIRS")
            .ToList()
            // Read each folder path
            .ForEach(folderPath =>
            {
                // Read multiple .atomic files
                List<string>? atomicFiles = FusionFS.SearchDir(folderPath);
                // Load each .atomic file
                atomicFiles?.ForEach(file => {
                    ReadonlyAtomicResult usedFunction = functionWithResult.Invoke(file);
                    
                    if (usedFunction.Status == AtomicStatus.ERROR)
                    {
                        throw new Exception($"\x1B[31m{usedFunction.Message}\x1B[0m");
                    }
                });
            });
    }
}