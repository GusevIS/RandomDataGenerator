using System;

namespace DataGenerator
{
    public interface IDataSourceDiscover : IDataSource
    {
        int Priority { get; }

        bool TryMap(IMappingContext mappingContext);
    }
}