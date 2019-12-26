using System;

namespace DataGenerator
{
    public interface IGenerateContext
    {
        Generator Generator { get; set; }

        Type ClassType { get; }

        Type MemberType { get; }

        string MemberName { get; }

        int Depth { get; }

        object Instance { get; }
    }
}