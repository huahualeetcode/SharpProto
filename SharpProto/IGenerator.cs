using System;
namespace SharpProto
{
    public interface IGenerator
    {
        public bool Generate(string Filename, string OutputDir);
    }
}
