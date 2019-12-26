using System;

namespace DataGenerator.Fluent
{
    public class ListGeneratorBuilder<TEntity> : ClassMappingBuilder<TEntity>
    {
        public ListGeneratorBuilder(ClassMapping classMapping) : base(classMapping)
        {
            Random(2, 10);
        }

        public int GenerateCount { get; set; }

        public ListGeneratorBuilder<TEntity> Count(int count)
        {
            GenerateCount = count;
            return this;
        }

        public ListGeneratorBuilder<TEntity> Random(int min, int max)
        {
            var count = RandomGenerator.Current.Next(min, max);
            GenerateCount = count;
            return this;
        }

        public new ListGeneratorBuilder<TEntity> AutoMap(bool value = true)
        {
            base.AutoMap(value);
            return this;
        }

    }
}