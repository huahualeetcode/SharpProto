using System;

namespace SharpProto
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new CompilerBase().ValidateProto(typeof(Group)));
        }
    }
}
