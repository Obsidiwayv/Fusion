using System.Text;
using Fusion.API;

namespace Fusion.DSL;

public enum AtomicTokenEnum
{
    Binary,
    Version,
    UseVersion,
    Language,
    Library,
    Headers,
    Sources,
    Libs,
    Assets,
    Flags,
    ArrayElement,
    Quote,
}

        // new(AtomicTokenListEnum.Binary, "binary"),
        // new("version"),
        // new("uses"),
        // new("language"),
        // new("define_lib"),
        // new("add_includes"),
        // new("add_sources"),
        // new("add_libs"),
        // new("put_assets"),
        // new("put_flags")

public class AtomicTokens
{
    public static AtomicTokenDict Map { get; } = new()
    {
        {AtomicTokenEnum.Binary, "binary"},
        {AtomicTokenEnum.Version, "version"},
        {AtomicTokenEnum.Language, "language"},
        {AtomicTokenEnum.Language, "define_lib"},
        {AtomicTokenEnum.Headers, "add_includes"},
        {AtomicTokenEnum.Sources, "add_sources"},
        {AtomicTokenEnum.Libs, "add_libs"},
        {AtomicTokenEnum.Assets, "put_assets"},
        {AtomicTokenEnum.Flags, "put_flags"}
    };
}

public class AtomicLexer(char[] file)
{
    private static int Index { get; set; } = 0;

    public AtomicMapList Use()
    {
        AtomicMapList maps = [];
        StringBuilder word = new();
        for (; Index < file.Length; Index++)
        {
            if (Current() != '"' && Current() != '(' && Current() != ')')
            {
                word.Append(Current());
            }
            if (Current() == '"') maps.Add(new AtomicMap(CurrentAsString(), "QUOTE"));
            if (Current() == '(') maps.Add(new AtomicMap(CurrentAsString(), "ARRAY_START"));
            if (Current() == ')') maps.Add(new AtomicMap(CurrentAsString(), "ARRAY_END"));

            foreach (AtomicMap map in AtomicMaps.Tokens)
            {
                if (map.Key == word.ToString())
                {
                    maps.Add(new AtomicMap(word.ToString(), map.Value));
                    word.Clear();
                    break;
                }
            }
            if (Current() == '"')
            {
                Next();
                StringBuilder stringLine = new();

                for (int w = 0 + Index; w < file.Length; w++)
                {
                    if (file[w] == '"')
                    {
                        maps.Add(new AtomicMap(stringLine.ToString(), "STRING"));
                        maps.Add(new AtomicMap(CurrentAsString(), "QUOTE"));
                        break;
                    }
                    Next();
                    stringLine.Append(file[w]);
                }
            }
            if (Current() == '(')
            {
                Next();
                StringBuilder stringLine = new();

                for (int NextIndex = 0 + Index; NextIndex < file.Length; NextIndex++)
                {
                    char C()
                    {
                        return file[NextIndex];
                    }
                    stringLine.Append(C());

                    if (C() == '\n')
                    {
                        if (!string.IsNullOrWhiteSpace(stringLine.ToString()))
                        {
                            var line = stringLine.ToString();
                            var trimmed = line.Trim();
                            maps.Add(new AtomicMap(trimmed, "ARRAY_ELEMENT"));
                        }
                        stringLine.Clear();
                        continue;
                    } else if (C() == ')')
                    {
                        break;
                    }
                    Next();
                }
            }
            if (char.IsWhiteSpace(Current())) word.Clear(); 
        }
        Index = 0;
        return maps;
    }

    private static void Next()
    {
        Index++;
    }

    private char Current()
    {
        return file[Index];
    }

    public string CurrentAsString()
    {
        return file[Index].ToString();
    }
}