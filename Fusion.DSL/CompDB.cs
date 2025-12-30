using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fusion.API;

namespace Fusion.DSL;

public class AtomicCompileDatabaseJSON
{
    [JsonPropertyName("command")]
    public required string Command { get; set; }

    [JsonPropertyName("file")]
    public required string File { get; set; }

    [JsonPropertyName("directory")]
    public string Directory { get; set; } = 
        System.IO.Directory.GetCurrentDirectory();
}

public class AtomicCompileDatabase(AtomicContext context)
{

    static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public List<AtomicCompileDatabaseJSON> JSONData { get; } = [];

    public void PushJSON(AtomicCompileDatabaseJSON json)
    {
        // only push the json if the setting is on
        if (context.BuildOptions.GenerateCompileDatabase == true)
        {
            JSONData.Add(json);
        }
    }

    public AtomicStatus Write()
    {
        try
        {
            if (context.BuildOptions.GenerateCompileDatabase == true)
            {
                Console.WriteLine("Generate 'compile_commands.json' is enabled");


                File.WriteAllText(
                    "compile_commands.json",
                    JsonSerializer.Serialize(
                        JSONData,
                        options
                    ),
                    new UTF8Encoding(false)
                );

                Console.WriteLine("Wrote compile_commands.json");
            }
            return AtomicStatus.DONE;
        }
        catch
        {
            return AtomicStatus.ERROR;
        }
    }
}
