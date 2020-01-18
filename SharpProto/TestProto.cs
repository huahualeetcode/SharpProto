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
        public Person member1;
        [Field(3)]
        public Person member2;
    }
}
