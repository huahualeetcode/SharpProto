using System;
using System.Collections.Generic;

namespace SharpProto
{
    [Message]
    public struct Person
    {
        [Field(1)]
        public string name;

        [Field(2)]
        public int id;
    }

    [Message]
    public struct Group
    {
        [Field(1)]
        public string name;

        [Field(2)]
        public IList<Person> members;        
    }
}
