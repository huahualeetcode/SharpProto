using System;
namespace SharpProto
{
    public interface IComplier
    {
        public bool Compile(string Filename, string OutputDir);
    }
}
