using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataGenerator.Reflection;

namespace DataGenerator.Fluent
{
    public class ConfigurationBuilder
    {
        public ConfigurationBuilder(Configuration configuration)
        {
            Configuration = configuration;
        }


        public Configuration Configuration { get; }


        public ConfigurationBuilder IncludeLoadedAssemblies()
        {
            Configuration.Assemblies.IncludeLoadedAssemblies();
            Configuration.ClearCache();
            return this;
        }

        public ConfigurationBuilder IncludeAssemblyFor<T>()
        {
            return IncludeAssembly(typeof(T).GetTypeInfo().Assembly);
        }

        public ConfigurationBuilder IncludeAssembly(Assembly assembly)
        {
            Configuration.Assemblies.IncludeAssembly(assembly);
            Configuration.ClearCache();
            return this;
        }

        public ConfigurationBuilder IncludeName(string name)
        {
            Configuration.Assemblies.IncludeName(name);
            Configuration.ClearCache();
            return this;
        }


        public ConfigurationBuilder ExcludeAssemblyFor<T>()
        {
            return ExcludeAssembly(typeof(T).GetTypeInfo().Assembly);
        }

        public ConfigurationBuilder ExcludeAssembly(Assembly assembly)
        {
            Configuration.Assemblies.ExcludeAssembly(assembly);
            Configuration.ClearCache();
            return this;
        }

        public ConfigurationBuilder ExcludeName(string name)
        {
            Configuration.Assemblies.ExcludeName(name);
            Configuration.ClearCache();
            return this;
        }


        public ConfigurationBuilder Entity<TEntity>(Action<ClassMappingBuilder<TEntity>> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var type = typeof(TEntity);
            var classMapping = GetClassMap(type);

            var mappingBuilder = new ClassMappingBuilder<TEntity>(classMapping);
            builder(mappingBuilder);

            return this;
        }


        public ConfigurationBuilder Profile<TProfile>() 
            where TProfile : IMappingProfile, new()
        {
            var profile = new TProfile();
            var type = profile.EntityType;
            var classMapping = GetClassMap(type);

            profile.Register(classMapping);

            return this;
        }


        private ClassMapping GetClassMap(Type type)
        {
            var classMapping = Configuration.Mapping.GetOrAdd(type, t =>
            {
                var typeAccessor = TypeAccessor.GetAccessor(t);
                var mapping = new ClassMapping(typeAccessor);
                return mapping;
            });

            return classMapping;
        }

    }
}
