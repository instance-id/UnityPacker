using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace UnityPacker
{
    class PackProgram
    {
        static void Main(string[] args)
        {
            var inpath = "";
            var fileName = "Package";
            var rootDir = "Assets/";
            var destination = "";
            var exts = new string[0];
            var dirs = new string[0];

            if (args.Length == 1 && args[0].Contains(".yml"))
            {
                var contents = File.ReadAllText(args[0]);
                var input = new StringReader(contents);

                var yaml = new YamlStream();
                yaml.Load(input);

                var mapping = (YamlMappingNode) yaml.Documents[0].RootNode;
                var settings = (YamlSequenceNode) mapping.Children[new YamlScalarNode("project")];
                var item = settings.FirstOrDefault() as YamlMappingNode;

                inpath = item.Children[new YamlScalarNode("source")].ToString();
                fileName = item.Children[new YamlScalarNode("packagename")].ToString();
                rootDir = item.Children[new YamlScalarNode("rootdirectory")].ToString();
                destination = item.Children[new YamlScalarNode("destination")].ToString();
                exts = new[] {item.Children[new YamlScalarNode("ignoredextensions")].ToString()};
                dirs = new[] {item.Children[new YamlScalarNode("ignoredfolders")].ToString()};

                exts = exts.ToString().Split(',');
                dirs = dirs.ToString().Split(',');
            }
            else if (args.Length >= 2)
            {
                inpath = args[0];
                fileName = args.Length > 1 ? args[1] : "Package";
                rootDir = args.Length > 2 ? args[2] : "Assets/";
                exts = args.Length > 3 ? args[3].Split(',') : new string[0];
                dirs = args.Length > 4 ? args[4].Split(',') : new string[0];
            }
            else
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("UnityPacker [Path/config.yml]");
                Console.WriteLine("Or: ");
                Console.WriteLine("UnityPacker [Source Path] [Package Name] [Respect Meta] [Omitted Extensions] [Omitted Directories]");
                return;
            }
            
            var extensions = new List<string>(exts)
            {
                "meta" // always skip meta files
            };
            var pack = Package.FromDirectory(inpath, fileName, true, extensions.ToArray(), dirs);
            pack.GeneratePackage(rootDir, destination);
        }
    }
}
