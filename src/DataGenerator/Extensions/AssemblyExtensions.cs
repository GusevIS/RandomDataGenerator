using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataGenerator.Reflection;

namespace DataGenerator.Extensions
{
    public static class AssemblyExtensions
    {
        
        public static IEnumerable<Type> GetTypesAssignableFrom<T>(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();

            return assembly
                .GetLoadableTypes()
                .Where(t =>
                {
                    var i = t.GetTypeInfo();
                    return i.IsPublic && !i.IsAbstract && typeInfo.IsAssignableFrom(i);
                });
        }

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            if (assembly.IsDynamic)
                return Enumerable.Empty<Type>();

            Type[] types;

            try
            {
                types = assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null).ToArray();
            }
            catch (NotSupportedException)
            {
                return Enumerable.Empty<Type>();
            }

            return types;
        }

#if NET40 || PORTABLE
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif
    }
}