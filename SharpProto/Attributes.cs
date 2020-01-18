using System;
namespace SharpProto
{
    [AttributeUsage(AttributeTargets.Field |
                       AttributeTargets.Struct)]
    public class FieldAttribute : System.Attribute
    {
        public int ID { get; set; }

        public FieldAttribute(int id)
        {
            this.ID = id;
        }
    }

    [AttributeUsage(AttributeTargets.Struct)]
    public class MessageAttribute : System.Attribute
    {
    }
}
