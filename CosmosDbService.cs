using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CosmosDbService : ICosmosDbService
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;

    public CosmosDbService(string connectionString, string databaseId, string containerId)
    {
        _cosmosClient = new CosmosClient(connectionString);
        _container = _cosmosClient.GetContainer(databaseId, containerId);
    }

    public async Task<IEnumerable<EventDocument>> GetEventsDueTodayAsync()
    {
        var today = DateTime.UtcNow.Date;
        var query = $"SELECT * FROM c WHERE c.NextEventDate = '{today:yyyy-MM-dd}'";
        var iterator = _container.GetItemQueryIterator<EventDocument>(query);
        var results = new List<EventDocument>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task UpdateEventDocumentAsync(EventDocument eventDocument)
    {
        // Assume eventDocument.Id is the unique key
        await _container.UpsertItemAsync(eventDocument, new PartitionKey(eventDocument.Id));
    }
}