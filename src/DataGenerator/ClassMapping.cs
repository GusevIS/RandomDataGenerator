using System;
using System.Collections.Generic;
using DataGenerator.Reflection;

namespace DataGenerator
{
    public class ClassMapping
#if !NETSTANDARD1_3 && !NETSTANDARD1_5
        : ICloneable
#endif
    {
        public ClassMapping()
        {
            AutoMap = true;
            Members = new List<MemberMapping>();
            SyncRoot = new object();
        }

        public ClassMapping(TypeAccessor typeAccessor) : this()
        {
            TypeAccessor = typeAccessor;
        }

        public bool AutoMap { get; set; }

        public bool Ignored { get; set; }

        public bool Mapped { get; set; }

        public Func<Type, object> Factory { get; set; }

        public TypeAccessor TypeAccessor { get; set; }

        public List<MemberMapping> Members { get; }

        public object SyncRoot { get; }

#if !NETSTANDARD1_3 && !NETSTANDARD1_5
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

        public ClassMapping Clone()
        {
            var classMapping = new ClassMapping
            {
                AutoMap = AutoMap,
                Ignored = Ignored,
                Mapped = Mapped,
                TypeAccessor = TypeAccessor,

            };


            foreach (var m in Members)
            {
                var memberMapping = new MemberMapping
                {
                    Ignored = m.Ignored,
                    MemberAccessor = m.MemberAccessor,
                    DataSource = m.DataSource
                };

                classMapping.Members.Add(memberMapping);
            }

            return classMapping;
        }
    }
}
