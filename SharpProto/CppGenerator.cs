using System;
namespace SharpProto
{
    public class CppGenerator : GeneratorBase, IGenerator
    {
        public CppGenerator()
        {
        }

        public bool Generate(string Filename, string OutputDir)
        {
            base.CompileProto(Filename);

            return false;
        }
    }
}
