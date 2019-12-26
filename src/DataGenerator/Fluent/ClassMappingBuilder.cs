using System;
using System.Linq.Expressions;

namespace DataGenerator.Fluent
{
    public class ClassMappingBuilder<TEntity>
    {
        protected ClassMappingBuilder()
        {

        }

        public ClassMappingBuilder(ClassMapping classMapping)
        {
            ClassMapping = classMapping;
        }

        public ClassMapping ClassMapping { get; protected set; }


        public ClassMappingBuilder<TEntity> AutoMap(bool value = true)
        {
            ClassMapping.AutoMap = value;
            return this;
        }

        public ClassMappingBuilder<TEntity> Factory(Func<Type, object> factory)
        {
            ClassMapping.Factory = factory;
            return this;
        }


        public MemberConfigurationBuilder<TEntity, TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var propertyAccessor = ClassMapping.TypeAccessor.FindProperty(property);

            var memberMapping = ClassMapping.Members.Find(m => m.MemberAccessor.MemberInfo == propertyAccessor.MemberInfo);
            if (memberMapping == null)
            {
                memberMapping = new MemberMapping();
                memberMapping.MemberAccessor = propertyAccessor;

                ClassMapping.Members.Add(memberMapping);
            }

            var builder = new MemberConfigurationBuilder<TEntity, TProperty>(memberMapping);
            return builder;
        }
    }
}