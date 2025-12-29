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
                    GenerateDatabaseEnabled = true;
                }
            }
        }

        FusionSteps.Run((file) =>
        {
            List<AtomicMap> maps = new AtomicLexer(
                File.ReadAllText(file).ToCharArray()).Use();

            return new AtomicParser(maps,
                $"{Path.GetDirectoryName(file.TrimEnd(Path.DirectorySeparatorChar))}/" ?? "")
                .Use(true);
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