using System.Diagnostics;
using Fusion.API;

namespace Fusion.DSL;

public class AtomicAssembler(AtomicProjectFile project, AtomicContext context)
{
    string Headers { get; } = AtomicProjectFile.GetString(project.Includes);
    string Sources { get; } = AtomicProjectFile.GetString(project.Sources);
    string Libs { get; } = AtomicProjectFile.GetString(project.Libs);

    string Flags { get; } = AtomicProjectFile.GetString(project.Flags);

    public void Use()
    {
        Console.WriteLine(Sources);
        if (project.Sources.Count == 0)
        {
            throw new Exception("no input files...");
        }

        List<string> command = [
            $"{project.OutBinary}{(project.IsLibrary
                ? OS.LibraryEXT : OS.ExecutableEXT)}",
            Sources,
            project.Library,
            project.LangVersion ?? "",
            ..GetCommand(hasFlag: true)
        ];

        if (project.IsLibrary)
        {
            CompileStatic();
            return;
        }

        Console.WriteLine(project.ClangBinary);

        StartProc(project.ClangBinary, command);
    }

    public static void StartProc(string binary, List<string> command)
    {
        Console.WriteLine(string.Join(" ", command));
        Process proc = new();
        proc.StartInfo.FileName = binary;
        proc.StartInfo.Arguments = string.Join(" ", command);
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();
        while (!proc.StandardOutput.EndOfStream)
        {
            string? line = proc.StandardOutput.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
                Console.WriteLine(line);
        }
        proc.WaitForExit();
    }

    public List<string> GetCommand(bool hasFlag)
    {
        List<string> command = [];
        if (!string.IsNullOrWhiteSpace(Libs)) command.Add(Libs);
        if (!string.IsNullOrWhiteSpace(Headers)) command.Add(Headers);
        if (!string.IsNullOrWhiteSpace(Flags) && hasFlag) command.Add(Flags);
        return command;
    }

    public void CompileStatic()
    {
        List<string> objects = [];
        foreach (string source in project.Sources)
        {

            string objectFile =
                $"{AtomicFolders.ObjOutput}/{Path.GetFileNameWithoutExtension(source)}{OS.StaticLibObject}";
            objects.Add(objectFile);

            StartProc(project.ClangBinary, [
                "-c",
                source,
                $"-o {objectFile}",
                ..GetCommand(hasFlag: false)
            ]);
            Console.WriteLine($"\x1B[34mCompiled {source} -> {objectFile}\x1B[0m");
        }

        List<string> arguments = [];
        string binary = "";

        if (OS.IsUnix())
        {
            binary = "ar";
            arguments.Add("rcs");
            arguments.Add(
                $"{AtomicFolders.BinaryOutput}/{Path.GetFileNameWithoutExtension(project.OutBinary)}.a");
        }
        else if (OperatingSystem.IsWindows())
        {
            binary = Clang.LibEXE;
            arguments.Add(
                $"/OUT:{AtomicFolders.BinaryOutput}/{Path.GetFileNameWithoutExtension(project.OutBinary)}.lib");
        }

        StartProc(binary, [
            ..arguments,
            ..objects
        ]);
        Console.WriteLine(
            $"Compiled library archive in \x1B[32m{AtomicFolders.BinaryOutput}\x1B[0m");
    }
}