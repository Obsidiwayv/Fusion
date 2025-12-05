using System.Diagnostics;
using System.Net;

namespace Fusion.DSL;

public class AtomicAssembler
{
    public static void Use(AtomicParser parser)
    {
        string headers = string.Join(" ", parser.Includes);
        string sources = string.Join(" ", parser.Sources);
        string libs = string.Join(" ", parser.Libs);

        if (sources.Length == 0)
        {
            throw new Exception("no input files...");
        }

        List<string> command = [
            $"{parser.OutBinary}{(parser.IsLibrary
                ? OS.LibraryEXT : OS.ExecutableEXT)}",
            sources,
            parser.Library,
            parser.LangVersion ?? ""
        ];

        if (!string.IsNullOrWhiteSpace(libs)) command.Add(libs);
        if (!string.IsNullOrWhiteSpace(headers)) command.Add(headers);

        if (parser.IsLibrary)
        {
            CompileStatic(parser);
            return;
        }

        Console.WriteLine(parser.ClangBinary);

        StartProc(parser.ClangBinary, command);
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

    public static void CompileStatic(AtomicParser parser)
    {
        List<string> objects = [];
        foreach (string source in parser.Sources)
        {
            string headers = string.Join(" ", parser.Includes);
            string libs = string.Join(" ", parser.Libs);
            List<string> command = [];
            if (!string.IsNullOrWhiteSpace(libs)) command.Add(libs);
            if (!string.IsNullOrWhiteSpace(headers)) command.Add(headers);

            string objectFile = 
                $"{Program.ObjOutput}/{Path.GetFileNameWithoutExtension(source)}{OS.StaticLibObject}";
            objects.Add(objectFile);

            StartProc(parser.ClangBinary, [
                "-c",
                source,
                $"-o {objectFile}",
                ..command
            ]);
            Console.WriteLine($"Compiled {source} -> {objectFile}");
        }

        List<string> arguments = [];
        string binary = "";

        if (OS.IsUnix())
        {
            binary = "llvm-ar";
            arguments.Add("rcs");
            arguments.Add($"/OUT:{Program.BinaryOutput}/{Path.GetFileNameWithoutExtension(parser.OutBinary)}.a");
        } else if (OperatingSystem.IsWindows())
        {
            binary = Clang.LibEXE;
            arguments.Add($"/OUT:{Program.BinaryOutput}/{Path.GetFileNameWithoutExtension(parser.OutBinary)}.lib");
        }

        StartProc(binary, [
            ..arguments,
            ..objects
        ]);
        Console.WriteLine($"Compiled library archive in {Program.BinaryOutput}");
    }
}