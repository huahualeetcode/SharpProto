using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;

namespace SharpProto
{
    public class GeneratorBase
    {        
        private List<Type> primitiveTypes = new List<Type>()
        {
            typeof(Int32),
            typeof(Int64),
            typeof(String),
            typeof(Single),
            typeof(Double)
        };
        
        public GeneratorBase()
        {
        }

        protected void CompileProto(string Filename) {

            var code = File.ReadAllText(Filename);

            Console.WriteLine("Compling");
            Console.WriteLine(code);

            var refs = AppDomain.CurrentDomain.GetAssemblies();
            var refFiles = refs.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();
            CSharpCodeProvider provider = new CSharpCodeProvider();
            var compileParams = new System.CodeDom.Compiler.CompilerParameters()
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };

            var compilerResult = provider.CompileAssemblyFromSource(compileParams, code);
            var asm = compilerResult.CompiledAssembly;
        }

        public bool ValidateProto(Type protoType)
        {
            Console.WriteLine("Validating " + protoType.FullName);

            var hashSet = new HashSet<int>();
            var fieldInfos = protoType.GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                var fieldAttr = fieldInfo.GetCustomAttribute<FieldAttribute>();
                if (fieldAttr == null)
                {
                    throw new ArgumentException("field " + fieldInfo.Name + " does not have a tag");
                }
                if (hashSet.Contains(fieldAttr.ID))
                {
                    throw new ArgumentException("tag " + fieldAttr.ID + " already used");
                }
                hashSet.Add(fieldAttr.ID);

                Console.WriteLine(fieldInfo.Name + ":" + fieldAttr.ID);

                var fieldType = fieldInfo.FieldType;
                
                if (fieldType.IsGenericType)
                {
                    if (fieldType.GetGenericTypeDefinition() == typeof(IList<>))
                    {                        
                        var argType = fieldType.GetGenericArguments()[0];
                        Console.WriteLine(fieldInfo.Name + " is a IList<" + argType.Name + ">");
                        if (primitiveTypes.Contains(argType)) continue;
                        var messageAttr = argType.GetCustomAttribute<MessageAttribute>();
                        if (messageAttr == null)
                        {
                            throw new ArgumentException(argType.Name + " must be a message.");
                        }

                        if (!ValidateProto(argType)) return false;
                    }                    
                }
            }

            if (hashSet.Count == 0)
            {
                throw new ArgumentException("no tags found, not a valid message");
            }

            return true;
        }
    }
}
