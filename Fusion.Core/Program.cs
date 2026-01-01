using Fusion.DSL;
using Fusion.API;
using System.Xml;

namespace Fusion.Core;

public class Program
{

    public static void Main(string[] args)
    {
        VerifyDir(AtomicFolders.BinaryOutput);
        VerifyDir(AtomicFolders.BuildOutput);
        VerifyDir(AtomicFolders.ObjOutput);

        var xmlOptions = new XMLOptions();

        if (File.Exists(FusionConstants.DeveloperXML))
        {
            XmlDocument xml = new();
            xml.Load(FusionConstants.DeveloperXML);
            XmlNodeList? nodes = xml.SelectNodes("/BuildOptions/Option");

            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    bool nEnabled = true;

                    if (node.Attributes != null)
                    {
                        XmlAttribute? enabled = node.Attributes["enabled"];
                        if (enabled != null && enabled.Value == "0")
                        {
                            nEnabled = false;
                        }
                    }
                    
                    
                    if (node.InnerText == "GenerateCommands" && nEnabled)
                    {
                        xmlOptions.GenerateCompileDatabase = true;
                    }
                }
            }
        }

        var context = new AtomicContext
        {
            BuildOptions = xmlOptions
        };

        FusionSteps.Run((file) =>
        {
            AtomicMapList maps = AtomicContext.UseLexer(file);
            return context.UseParser(maps, file);
        });

        // This will only write the json file if the xml option is enabled
        context.database.Write();
        Directory.Delete(AtomicFolders.ObjOutput, true);
    }

    public static void VerifyDir(string directory)
    {
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
    }

    public static string GetProjectName(string file)
    {
        return file.Split(".")[0];
    }
}