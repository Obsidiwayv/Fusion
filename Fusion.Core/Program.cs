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
            XmlNodeList devOptionNodes = xml.GetElementsByTagName("BuildOption");

            foreach (XmlNode node in devOptionNodes)
            {
                string optionName = node.InnerText;
                if (optionName == "CompileDatabase")
                {
                    xmlOptions.GenerateCompileDatabase = true;
                }
            }
        }

        var context = new AtomicContext
        {
            BuildOptions = xmlOptions
        };

        FusionSteps.Run((file) =>
        {
            List<AtomicMap> maps = context.UseLexer(file);
            return context.UseParser(maps, file);
        });
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