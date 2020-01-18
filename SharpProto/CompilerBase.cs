using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpProto
{
    public class CompilerBase
    {
        private List<Type> primitiveTypes = new List<Type>()
        {
            typeof(Int32),
            typeof(Int64),
            typeof(String),
            typeof(Single),
            typeof(Double)
        };        
        
        public CompilerBase()
        {
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
