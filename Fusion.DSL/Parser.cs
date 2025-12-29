using Fusion.DSL.Elements;
using Fusion.API;

namespace Fusion.DSL;

public class AtomicParser(List<AtomicMap> atomicMaps, string project)
{
    public AtomicProjectFile ProjectFile { get; } = new();

    public int CurrentIndex { get; set; } = 0;

    private readonly AtomicResult result = new(AtomicStatus.WAITING, null);

    private string Root { get; } = "root://";

    public ReadonlyAtomicResult Use(bool UseAssembler)
    {
        try
        {
            for (; CurrentIndex < atomicMaps.Count; CurrentIndex++)
            {

                AtomicMap map = atomicMaps[CurrentIndex];
                if (map.Value == "HDRS")
                {
                    ParseArrayElements((key) =>
                    {
                        if (key.StartsWith(Root))
                        {
                            ProjectFile.Includes.Add($"-I{key.Replace(Root, "")}");
                        }
                        else
                        {
                            ProjectFile.Includes.Add($"-I{project}{key}");
                        }
                    });
                }
                if (map.Value == "SRCS")
                {
                    ParseArrayElements((key) =>
                    {
                        ProjectFile.Sources.Add($"{project}{key}");
                    });
                }
                if (map.Value == "LIBS")
                {
                    ParseArrayElements((key) =>
                    {
                        if (key.Contains(':'))
                        {
                            string[] keys = key.Split(":");
                            if (keys[0] == "darwin"
                                && OperatingSystem.IsMacOS())
                            {
                                ProjectFile.Libs.Add(keys[1]);
                            }
                            if (keys[0] == "win64"
                                && OperatingSystem.IsWindows())
                            {
                                ProjectFile.Libs.Add($"-l{keys[1]}");
                            }
                            if (keys[0] == "linux"
                                && OperatingSystem.IsLinux())
                            {
                                ProjectFile.Libs.Add($"-l{keys[1]}");
                            }
                        }
                        else
                        {
                            ProjectFile.Libs.Add($"-l{key}");
                        }
                    });
                }
                if (map.Value == "ASSETS")
                {
                    ParseArrayElements((key) =>
                    {
                        if (!File.Exists(key))
                        {
                            result.Invalidate($"{key} doesnt exist in the project");
                            return;
                        }
                        File.Copy(key,
                            $"{AtomicFolders.BuildOutput}/{Path.GetFileName(key)}",
                            true);
                    });
                }
                if (map.Value == "BIN")
                {
                    ParseString((key) =>
                    {
                        ProjectFile.OutBinary = $"-o {AtomicFolders.BuildOutput}/{key}";
                    });
                }
                if (map.Value == "VER")
                {
                    ParseString((key) => ProjectFile.Version = key);
                }
                if (map.Value == "USE_VER")
                {
                    ParseString((key) =>
                    {
                        Console.WriteLine($"File is using ATOMIC v{key}");
                    });
                }
                if (map.Value == "LIBRARY")
                {
                    ParseString((key) =>
                    {
                        if (key == "shared")
                        {
                            if (OperatingSystem.IsMacOS())
                            {
                                ProjectFile.Library = "-dynamiclib";
                            }
                            else
                            {
                                ProjectFile.Library = "-shared";
                            }
                            ;
                        }
                        else if (key != "static")
                        {
                            result.Invalidate(
                                $"define_lib needs to be either static or shared, got {key}");
                            return;
                        }
                        ProjectFile.IsLibrary = true;
                    });
                }
                if (map.Value == "LANG")
                {
                    ParseString((key) =>
                    {
                        if (key.Contains(':'))
                        {
                            string[] keys = key.Split(":");
                            string str = $"{keys[0]}{keys[1]}";
                            if (keys[0] == "c")
                            {
                                if (!Clang.CVersion.Contains(str))
                                {
                                    result.Invalidate("Invalid c version");
                                    return;
                                }
                                ProjectFile.ClangBinary = Clang.Binary;
                            }
                            else if (keys[0] == "c++")
                            {
                                if (!Clang.CPPVersion.Contains(str))
                                {
                                    result.Invalidate("Invalid c++ version");
                                    return;
                                }
                                ProjectFile.ClangBinary = Clang.CPlusPlusBinary;
                            }
                            else
                            {
                                result.Invalidate("Invalid language");
                                return;
                            }
                            ProjectFile.LangVersion = $"--std={str}";
                        }
                        else
                        {
                            if (key == "c")
                            {
                                ProjectFile.ClangBinary = Clang.Binary;
                            }
                            else if (key == "c++")
                            {
                                ProjectFile.ClangBinary = Clang.CPlusPlusBinary;
                            }
                            else
                            {
                                result.Invalidate("Invalid language");
                                return;
                            }
                        }
                    });
                }
                if (map.Value == "FLAGS")
                {
                    AtomicFlags flags = new();
                    ParseArrayElements((key) =>
                    {
                        flags.IsCurrent(key);

                        if (flags.IsWindows())
                        {
                            ProjectFile.Flags.Add(key);
                        }
                        if (flags.IsDarwin())
                        {
                            ProjectFile.Flags.Add(key);
                        }
                        if (flags.IsLinux())
                        {
                            ProjectFile.Flags.Add(key);
                        }
                    });
                }
            }
        }
        catch (Exception e)
        {
            result.Invalidate(e.Message);
        }
        if (UseAssembler) new AtomicAssembler(ProjectFile).Use();

        return result.Lock();
    }

    public AtomicStatus ParseArrayElements(Action<string> func)
    {
        if (Current(1).Value != "ARRAY_START")
        {
            result.Invalidate("Expected an array, got string");
            return AtomicStatus.ERROR;
        }

        Next();
        for (int arr = CurrentIndex; arr < atomicMaps.Count; arr++)
        {
            AtomicMap arrMap = atomicMaps[arr];
            if (arrMap.Value == "ARRAY_END")
            {
                CurrentIndex = arr;
                break;
            }
            if (arrMap.Value != "ARRAY_START")
            {
                func.Invoke(arrMap.Key);
            }
            Next();
        }
        return AtomicStatus.DONE;
    }

    public AtomicMap Current(int move = 0)
    {
        return atomicMaps[CurrentIndex + move];
    }

    public void Next()
    {
        CurrentIndex++;
    }

    public AtomicStatus ParseString(Action<string> func)
    {
        Next();
        if (Current().Value != "QUOTE")
        {
            result.Invalidate("Expected String got array");
            return AtomicStatus.ERROR;
        }
        Next();
        func.Invoke(Current().Key);
        return AtomicStatus.DONE;
    }
}