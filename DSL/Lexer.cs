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
    ];
}

public class AtomicLexer
{
    public static List<AtomicMap> Use(char[] file)
    {
        List<AtomicMap> maps = [];
        StringBuilder word = new();
        for (int i = 0; i < file.Length; i++)
        {
            if (file[i] != '"' && file[i] != '(' && file[i] != ')')
            {
                word.Append(file[i]);
            }
            if (file[i] == '"') maps.Add(new AtomicMap(file[i].ToString(), "QUOTE"));
            if (file[i] == '(') maps.Add(new AtomicMap(file[i].ToString(), "ARRAY_START"));
            if (file[i] == ')') maps.Add(new AtomicMap(file[i].ToString(), "ARRAY_END"));

            foreach (AtomicMap map in AtomicMaps.Tokens)
            {
                if (map.Key == word.ToString())
                {
                    maps.Add(new AtomicMap(word.ToString(), map.Value));
                    word.Clear();
                    break;
                }
            }
            if (file[i] == '"')
            {
                i++;
                StringBuilder stringLine = new();

                for (int w = 0 + i; w < file.Length; w++)
                {
                    if (file[w] == '"')
                    {
                        maps.Add(new AtomicMap(stringLine.ToString(), "STRING"));
                        maps.Add(new AtomicMap(file[i].ToString(), "QUOTE"));
                        break;
                    }
                    i++;
                    stringLine.Append(file[w]);
                }
            }
            if (file[i] == '(')
            {
                i++;
                StringBuilder stringLine = new();

                for (int w = 0 + i; w < file.Length; w++)
                {
                    if (file[w] != ' ' && !string.IsNullOrWhiteSpace(file[w].ToString()))
                    {
                        stringLine.Append(file[w]);
                    }
                    if (file[w] == '\n')
                    {
                        if (!string.IsNullOrWhiteSpace(stringLine.ToString()))
                        {
                            maps.Add(new AtomicMap(stringLine.ToString(), "ARRAY_ELEMENT"));
                        }
                        stringLine.Clear();
                        continue;
                    } else if (file[w] == ')')
                    {
                        break;
                    }
                    i++;
                }
            }
            if (char.IsWhiteSpace(file[i])) word.Clear(); 
        }
        return maps;
    }
}