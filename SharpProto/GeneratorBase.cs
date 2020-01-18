using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;


namespace SharpProto
{
    public class GeneratorBase
    {        
        protected readonly List<Type> primitiveTypes = new List<Type>()
        {
            typeof(Int32),
            typeof(Int64),
            typeof(String),
            typeof(Single),
            typeof(Double)
        };

        private Dictionary<Type, IList<Type>> graph = new Dictionary<Type, IList<Type>>();
        
        public GeneratorBase()
        {
        }

        protected Assembly CompileProto(string Filename)
        {
            var code = File.ReadAllText(Filename);

            Console.WriteLine("Compling");
            Console.WriteLine(code);

            var tree = SyntaxFactory.ParseSyntaxTree(code);

            string fileName = "proto.dll";

            var refs = AppDomain.CurrentDomain.GetAssemblies();
            var references = refs.Where(a => !a.IsDynamic).Select(a => MetadataReference.CreateFromFile(a.Location)).ToArray();
                    
            var compilation = CSharpCompilation.Create(fileName)
              .WithOptions(
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
              .AddReferences(references)
              .AddSyntaxTrees(tree);
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            EmitResult compilationResult = compilation.Emit(path);
            if (compilationResult.Success)
            {
                Assembly assembly =
                  AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                Console.WriteLine("proto compiled!");
                return assembly;
            }
            else
            {
                foreach (Diagnostic codeIssue in compilationResult.Diagnostics)
                {
                    string issue = $"ID: {codeIssue.Id}, Message: {codeIssue.GetMessage()},Location: { codeIssue.Location.GetLineSpan()},Severity: { codeIssue.Severity}";
                    Console.WriteLine(issue);
                }
            }
            return null;
        }

        protected Type[] ValidateProtos(Assembly assembly)
        {
            graph.Clear();

            var types = assembly.GetTypes();

            if (types.Select(t => ValidateProto(t)).Contains(false))
            {
                return null;
            }

            var seen = new HashSet<Type>();

            var ordered = new List<Type>();

            foreach (Type t in types)
            {
                if (seen.Contains(t)) continue;
                TopologicalSorting(t, seen, ordered);
            }

            return ordered.ToArray();
        }

        private void TopologicalSorting(Type cur, HashSet<Type> seen, IList<Type> ordered)
        {
            seen.Add(cur);

            foreach (Type nxt in graph[cur])
            {
                if (seen.Contains(nxt)) continue;
                TopologicalSorting(nxt, seen, ordered);
            }

            ordered.Add(cur);
        }

        // Validate the protos and build a dependecy graph.
        private bool ValidateProto(Type protoType)
        {
            if (!graph.ContainsKey(protoType))
            {
                graph.Add(protoType, new List<Type>());
            }

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

                        graph[protoType].Add(argType);
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
