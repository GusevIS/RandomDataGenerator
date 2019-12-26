using System;
using System.Collections.Generic;
using System.Linq;
using DataGenerator.Fluent;
using DataGenerator.Reflection;

namespace DataGenerator
{

    public class Generator
    {
        public Generator()
        {
            Configuration = new Configuration();
        }

        public Configuration Configuration { get; }

        public void Configure(Action<ConfigurationBuilder> builder)
        {
            var configurationBuilder = new ConfigurationBuilder(Configuration);
            builder(configurationBuilder);
        }


        public T Single<T>()
        {
            var type = typeof(T);
            var classMapping = GetMapping(type);

            var instance = GenerateInstance<T>(classMapping);

            return instance;
        }

        public T Single<T>(Action<ClassMappingBuilder<T>> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var type = typeof(T);

            // copy class mapping before changes
            var classMapping = GetMapping(type).Clone();

            // allow overriding class mapping
            var mappingBuilder = new ClassMappingBuilder<T>(classMapping);
            builder(mappingBuilder);

            var instance = GenerateInstance<T>(classMapping);

            return instance;
        }


        public IList<T> List<T>()
        {
            return List<T>(2, 10);
        }

        public IList<T> List<T>(int min, int max)
        {
            var count = RandomGenerator.Current.Next(min, max);
            return List<T>(count);
        }

        public IList<T> List<T>(int count)
        {
            var type = typeof(T);
            var classMapping = GetMapping(type);

            // generate at least one
            count = Math.Max(1, count);
            var list = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                var instance = GenerateInstance<T>(classMapping);
                list.Add(instance);
            }

            return list;
        }

        public IList<T> List<T>(Action<ListGeneratorBuilder<T>> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var type = typeof(T);

            // copy class mapping before changes
            var classMapping = GetMapping(type).Clone();

            // allow overriding class mapping
            var mappingBuilder = new ListGeneratorBuilder<T>(classMapping);
            builder(mappingBuilder);

            // generate at least one
            var count = Math.Max(1, mappingBuilder.GenerateCount);
            var list = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                var instance = GenerateInstance<T>(classMapping);
                list.Add(instance);
            }

            return list;
        }


        private T GenerateInstance<T>(ClassMapping classMapping)
        {
            var typeAccessor = classMapping.TypeAccessor;
            var instance = classMapping.Factory != null
                ? classMapping.Factory(typeAccessor.Type)
                : typeAccessor.Create();

            foreach (var memberMapping in classMapping.Members)
            {
                var dataSource = memberMapping.DataSource;
                if (memberMapping.Ignored || dataSource == null)
                    continue;

                var memberAccessor = memberMapping.MemberAccessor;
                var context = new GenerateContext
                {
                    Generator = this,
                    ClassType = typeAccessor.Type,
                    MemberType = memberAccessor.MemberType,
                    MemberName = memberAccessor.Name,
                    Instance = instance
                };

                var value = dataSource.NextValue(context);
                SetValueWithCoercion(memberAccessor, instance, value);
            }

            return (T)instance;
        }


        private ClassMapping GetMapping(Type type)
        {
            var mapping = Configuration.Mapping
                .GetOrAdd(type, t => new ClassMapping(TypeAccessor.GetAccessor(type)));

            if (mapping.Mapped)
                return mapping;

            bool autoMap = mapping.AutoMap || Configuration.AutoMap;
            if (!autoMap)
                return mapping;

            // thread-safe initialization
            lock (mapping.SyncRoot)
            {
                if (mapping.Mapped)
                    return mapping;

                var typeAccessor = mapping.TypeAccessor;

                // scan for data sources
                var dataSources = Configuration
                    .DataSources()
                    .OrderBy(p => p.Priority)
                    .ToList();

                var properties = typeAccessor.GetProperties()
                    .Where(p => p.HasGetter && p.HasSetter);

                foreach (var property in properties)
                {
                    // get or create member
                    var memberMapping = mapping.Members.FirstOrDefault(m => m.MemberAccessor.Name == property.Name);
                    if (memberMapping == null)
                    {
                        memberMapping = new MemberMapping { MemberAccessor = property };
                        mapping.Members.Add(memberMapping);
                    }


                    // skip already mapped fields
                    if (memberMapping.Ignored || memberMapping.DataSource != null)
                        continue;


                    // search all for first match
                    var context = new MappingContext(mapping, memberMapping);
                    memberMapping.DataSource = dataSources.FirstOrDefault(d => d.TryMap(context));
                }

                mapping.Mapped = true;

                return mapping;
            }
        }


        private void SetValueWithCoercion(IMemberAccessor targetAccessor, object target, object value)
        {
            Type memberType = targetAccessor.MemberType;
            Type valueType = value?.GetType().GetUnderlyingType();

            object v = ReflectionHelper.CoerceValue(memberType, valueType, value);
            targetAccessor.SetValue(target, v);
        }


        private static readonly Lazy<Generator> _current = new Lazy<Generator>(() => new Generator());

        public static Generator Default => _current.Value;

        public static Generator Create(Action<ConfigurationBuilder> builder)
        {
            var generator = new Generator();
            generator.Configure(builder);

            return generator;
        }
    }
}
