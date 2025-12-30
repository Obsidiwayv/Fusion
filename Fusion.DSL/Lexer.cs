using System.Text;

namespace Fusion.DSL;

public class AtomicMap(string key, string value)
{
    public string Key { get; } = key;
    public string Value { get; } = value;
}

public class AtomicMaps
{
    public static List<AtomicMap> Tokens { get; } = [
        new AtomicMap("binary", "BIN"),
        new AtomicMap("version", "VER"),
        new AtomicMap("uses", "USE_VER"),
        new AtomicMap("language", "LANG"),
        new AtomicMap("define_lib", "LIBRARY"),
        new AtomicMap("add_includes", "HDRS"),
        new AtomicMap("add_sources", "SRCS"),
        new AtomicMap("add_libs", "LIBS"),
        new AtomicMap("put_assets", "ASSETS"),
        new AtomicMap("put_flags", "FLAGS")
    ];
}

public class AtomicLexer(char[] file, AtomicContext context)
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
                    if (C() != ' ' && !string.IsNullOrWhiteSpace(C().ToString()))
                    {
                        stringLine.Append(C());
                    }
                    if (C() == '\n')
                    {
                        if (!string.IsNullOrWhiteSpace(stringLine.ToString()))
                        {
                            maps.Add(new AtomicMap(stringLine.ToString(), "ARRAY_ELEMENT"));
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