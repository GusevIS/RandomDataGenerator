using System;

namespace DataGenerator.Sources
{
    public class FactoryDataSource<TEntity, TProperty> : IDataSource
    {

        public FactoryDataSource(Func<TEntity, TProperty> factory)
        {
            Factory = factory;
        }

        public Func<TEntity, TProperty> Factory { get; }

        public object NextValue(IGenerateContext generateContext)
        {
            var instance = generateContext.Instance is TEntity
                ? (TEntity)generateContext.Instance
                : default(TEntity);

            return Factory(instance);
        }
    }
}