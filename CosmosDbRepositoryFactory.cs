
public class CosmosDbRepositoryFactory
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly Dictionary<string, object> _repositories = new();

    public CosmosDbRepositoryFactory(CosmosClient cosmosClient, string databaseName)
    {
        _cosmosClient = cosmosClient;
        _databaseName = databaseName;
    }

    public ICosmosDbRepository<T> GetRepository<T>(string containerName) where T : class, IPartitionedEntity
    {
        if (_repositories.ContainsKey(containerName))
        {
            return (ICosmosDbRepository<T>)_repositories[containerName];
        }

        var repository = new CosmosDbRepository<T>(_cosmosClient, _databaseName, containerName);
        _repositories.Add(containerName, repository);
        return repository;
    }
}
    