using System.Diagnostics;

namespace Fusion.DSL;

public class AtomicParser(List<AtomicMap> atomicMaps, string project)
{
    public string OutBinary { get; set; } = "";
    public string? Version { get; set; }
    public string Library { get; set; } = "";
    public string? LangVersion { get; set; }
    public string ClangBinary { get; set; } = "";

    public bool IsLibrary { get; set; } = false;
    public List<string> Includes { get; } = [];
    public List<string> Sources { get; } = [];
    public List<string> Libs { get; } = [];

    private readonly AtomicResult result = new(AtomicStatus.WAITING, null);

    public ReadonlyAtomicResult Use()
    {
        try
        {
            for (int index = 0; index < atomicMaps.Count; index++)
            {
                AtomicMap map = atomicMaps[index];
                if (map.Value == "HDRS")
                {
                    ParseArrayElements(ref index, (key) =>
                        Includes.Add($"-I{project}{key}"));
                }
                if (map.Value == "SRCS")
                {
                    ParseArrayElements(ref index, (key) =>
                    {
                        Sources.Add($"{project}{key}");
                    });
                }
                if (map.Value == "LIBS")
                {
                    ParseArrayElements(ref index, (key) =>
                    {
                        if (key.Contains(':'))
                        {
                            string[] keys = key.Split(":");
                            if (keys[0] == "darwin"
                                && OperatingSystem.IsMacOS())
                            {
                                Libs.Add($"-l{keys[1]}");
                            }
                            if (keys[0] == "win64"
                                && OperatingSystem.IsWindows())
                            {
                                Libs.Add($"-l{keys[1]}");
                            }
                            if (keys[0] == "linux"
                                && OperatingSystem.IsLinux())
                            {
                                Libs.Add($"-l{keys[1]}");
                            }
                        }
                        else
                        {
                            Libs.Add($"-l{key}");
                        }
                    });
                }
                if (map.Value == "ASSETS")
                {
                    ParseArrayElements(ref index, (key) =>
                    {
                        if (!File.Exists(key))
                        {
                            result.Invalidate($"{key} doesnt exist in the project");
                            return;
                        }
                        File.Copy(key,
                            $"{Program.BuildOutput}/{Path.GetFileName(key)}",
                            true);
                    });
                }
                if (map.Value == "BIN")
                {
                    ParseString(ref index, (key) =>
                    {
                        OutBinary = $"-o {Program.BuildOutput}/{key}";
                    });
                }
                if (map.Value == "VER")
                {
                    ParseString(ref index, (key) => Version = key);
                }
                if (map.Value == "USE_VER")
                {
                    ParseString(ref index, (key) =>
                    {
                        Console.WriteLine($"File is using ATOMIC v{key}");
                    });
                }
                if (map.Value == "LIBRARY")
                {
                    ParseString(ref index, (key) =>
                    {
                        if (key == "shared")
                        {
                            if (OperatingSystem.IsMacOS())
                            {
                                Library = "-dynamiclib";
                            } else
                            {
                                Library = "-shared";
                            };
                        }
                        else if (key != "static")
                        {
                            result.Invalidate(
                                $"define_lib needs to be either static or shared, got {key}");
                            return;
                        }
                        IsLibrary = true;
                    });
                }
                if (map.Value == "LANG")
                {
                    ParseString(ref index, (key) =>
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
                                ClangBinary = Clang.Binary;
                            }
                            else if (keys[0] == "c++")
                            {
                                if (!Clang.CPPVersion.Contains(str))
                                {
                                    result.Invalidate("Invalid c++ version");
                                    return;
                                }
                                ClangBinary = Clang.CPlusPlusBinary;
                            } else
                            {
                                result.Invalidate("Invalid language");
                                return;
                            }
                            LangVersion = $"--std={str}";
                        } else
                        {
                            if (key == "c")
                            {
                                ClangBinary = Clang.Binary;
                            } else if (key == "c++")
                            {
                                ClangBinary = Clang.CPlusPlusBinary;
                            } else
                            {
                                result.Invalidate("Invalid language");
                                return;
                            }
                        }
                    });
                }
            }
        }
        catch (Exception e)
        {
            result.Invalidate(e.Message);
        }
        AtomicAssembler.Use(this);
        return result.Lock();
    }

    public AtomicStatus ParseArrayElements(
        ref int index,
        Action<string> func)
    {
        if (atomicMaps[index + 1].Value != "ARRAY_START")
        {
            result.Invalidate("Expected an array, got string");
            return AtomicStatus.ERROR;
        }

        index++;
        for (int arr = index; arr < atomicMaps.Count; arr++)
        {
            AtomicMap arrMap = atomicMaps[arr];
            if (arrMap.Value == "ARRAY_END")
            {
                index = arr;
                break;
            }
            if (arrMap.Value != "ARRAY_START")
            {
                func.Invoke(arrMap.Key);
            }
            index++;
        }
        return AtomicStatus.DONE;
    }

    public AtomicStatus ParseString(ref int index, Action<string> func)
    {
        index++;
        if (atomicMaps[index].Value != "QUOTE")
        {
            result.Invalidate("Expected String got array");
            return AtomicStatus.ERROR;
        }
        index++;
        func.Invoke(atomicMaps[index].Key);
        return AtomicStatus.DONE;
    }
}