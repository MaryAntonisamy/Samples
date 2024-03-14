
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CosmosDbRepository<T> : ICosmosDbRepository<T> where T : class, IPartitionedEntity
{
    private readonly Container _container;

    public CosmosDbRepository(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        _container = cosmosClient.GetContainer(databaseName, containerName);
    }

    public async Task AddAsync(T item)
    {
        await _container.CreateItemAsync(item, new PartitionKey(item.NextEventDate.ToString("o")));
    }

    public async Task DeleteAsync(string id, string partitionKeyValue)
    {
        await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKeyValue));
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<T>();
        var results = new List<T>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task<T> GetByIdAsync(string id, string partitionKeyValue)
    {
        try
        {
            ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKeyValue));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UpdateAsync(string id, T item, string partitionKeyValue)
    {
        await _container.UpsertItemAsync(item, new PartitionKey(partitionKeyValue));
    }
}
    