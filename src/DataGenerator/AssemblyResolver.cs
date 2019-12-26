using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DataGenerator.Reflection;

namespace DataGenerator
{
    public class AssemblyResolver
    {
        private readonly List<Func<IEnumerable<Assembly>>> _sources;
        private readonly List<Func<Assembly, bool>> _includes;
        private readonly List<Func<Assembly, bool>> _excludes;

        public AssemblyResolver()
        {
            _sources = new List<Func<IEnumerable<Assembly>>>();
            _includes = new List<Func<Assembly, bool>>();
            _excludes = new List<Func<Assembly, bool>>();
        }

        public List<Func<IEnumerable<Assembly>>> Sources
        {
            get { return _sources; }
        }

        public List<Func<Assembly, bool>> Includes
        {
            get { return _includes; }
        }

        public List<Func<Assembly, bool>> Excludes
        {
            get { return _excludes; }
        }


        public void IncludeLoadedAssemblies()
        {
#if NETSTANDARD1_3 || NETSTANDARD1_5
            //TDOD figure out how to do this
#else
            _sources.Add(() => AppDomain.CurrentDomain.GetAssemblies());
#endif
        }

        public void IncludeAssemblyFor<T>()
        {
            IncludeAssembly(typeof(T).GetTypeInfo().Assembly);
        }

   
        public void IncludeAssembly(Assembly assembly)
        {
            _sources.Add(() => new[] { assembly });
        }

        public void IncludeName(string name)
        {
            _includes.Add(a => a.FullName.Contains(name));
        }


        public void ExcludeAssemblyFor<T>()
        {
            ExcludeAssembly(typeof(T).GetTypeInfo().Assembly);
        }

        public void ExcludeAssembly(Assembly assembly)
        {
            _excludes.Add(a => a == assembly);
        }

        public void ExcludeName(string name)
        {
            _excludes.Add(a => a.FullName.StartsWith(name));
        }


        public IEnumerable<Assembly> Resolve()
        {
            // default to loaded assemblies
            if (_sources.Count == 0)
                IncludeLoadedAssemblies();

            var assemblies = _sources
                .SelectMany(source => source())
                .Where(assembly => _includes.Count == 0 || _includes.Any(include => include(assembly)))
                .Where(assembly => _excludes.Count == 0 || !_excludes.Any(exclude => exclude(assembly)))
                .Distinct()
                .ToList();

            return assemblies;
        }
    }
}