using System;
using System.IO;

namespace SharpProto
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine(string.Format("Usage: SharpProto proto.cs [cpp|python] output_dir"));
                return;
            }

            var protoFilename = args[0];
            if (!File.Exists(protoFilename))
            {
                throw new FileNotFoundException(protoFilename + " was not found.");
            }

            var lang = args[1];
            if (lang != "cpp" && lang != "python")
            {
                throw new NotSupportedException(lang + " language is not supported.");
            }

            var outputDir = args[2];
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            _ = new CppGenerator().Generate(protoFilename, outputDir);
        }
    }
}
