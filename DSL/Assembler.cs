using System.Diagnostics;
using System.Net;

namespace Fusion.DSL;

public class AtomicAssembler(AtomicProjectFile project)
{
    string Headers { get; } = project.GetString(project.Includes);
    string Sources { get; } = project.GetString(project.Sources);
    string Libs { get; } = project.GetString(project.Libs);

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
            ..GetCommand()
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

    public List<string> GetCommand()
    {
        List<string> command = [];
        if (!string.IsNullOrWhiteSpace(Libs)) command.Add(Libs);
        if (!string.IsNullOrWhiteSpace(Headers)) command.Add(Headers);
        return command;
    }

    public void CompileStatic()
    {
        List<string> objects = [];
        foreach (string source in project.Sources)
        {

            string objectFile =
                $"{Program.ObjOutput}/{Path.GetFileNameWithoutExtension(source)}{OS.StaticLibObject}";
            objects.Add(objectFile);

            StartProc(project.ClangBinary, [
                "-c",
                source,
                $"-o {objectFile}",
                ..GetCommand()
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
                $"{Program.BinaryOutput}/{Path.GetFileNameWithoutExtension(project.OutBinary)}.a");
        }
        else if (OperatingSystem.IsWindows())
        {
            binary = Clang.LibEXE;
            arguments.Add(
                $"/OUT:{Program.BinaryOutput}/{Path.GetFileNameWithoutExtension(project.OutBinary)}.lib");
        }

        StartProc(binary, [
            ..arguments,
            ..objects
        ]);
        Console.WriteLine(
            $"Compiled library archive in \x1B[32m{Program.BinaryOutput}\x1B[0m");
    }
}