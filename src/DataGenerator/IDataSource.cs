namespace DataGenerator
{
    public interface IDataSource
    {
        object NextValue(IGenerateContext generateContext);
    }
}
