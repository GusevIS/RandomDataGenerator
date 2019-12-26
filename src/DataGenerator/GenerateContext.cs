using System;

namespace DataGenerator
{
    public class GenerateContext : IGenerateContext
    {
        public Generator Generator { get; set; }

        public Type ClassType { get; set; }

        public Type MemberType { get; set; }

        public string MemberName { get; set; }

        public int Depth { get; set; }

        public object Instance { get; set; }

    }
}