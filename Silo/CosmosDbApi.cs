using System.Reflection.Metadata.Ecma335;
using Microsoft.Azure.Cosmos;

namespace Silo;

public class CosmosDbApi
{
    private readonly string _endpoint;
    private readonly string _key;

    public CosmosDbApi(string endpoint, string key)
    {
        _endpoint = endpoint;
        _key = key;
    }

    private record ItemReference(string Id, string PartitionKey);

    public async ValueTask ClearContainer(string dbId, string containerId, bool autoscale, int throughput)
    {
        
        
        using CosmosClient client = new(accountEndpoint: _endpoint, authKeyOrResourceToken: _key);
        var db = client.GetDatabase(dbId);
        var container = db.GetContainer(containerId);

        var options = await container.ReadContainerAsync();
        
        await container.DeleteContainerAsync();
        await db.CreateContainerAsync(
            new ContainerProperties(containerId, options.Resource.PartitionKeyPaths),
            autoscale 
                ? ThroughputProperties.CreateAutoscaleThroughput(throughput)
                : ThroughputProperties.CreateManualThroughput(throughput));




        // Read and delete document one by one is extremely time consuming
        // Log($"Start to clear {containerId} from {dbId}");
        // var query = new QueryDefinition(@"SELECT * FROM c");
        // var feed = container.GetItemQueryIterator<System.Collections.Generic.Dictionary<string, object>>(query);
        // List<ItemReference> toDelete = new();
        // while (feed.HasMoreResults)
        // {
        //     foreach (var item in await feed.ReadNextAsync())
        //     {
        //         if(!item.TryGetValue("id", out object idO))
        //             continue;
        //         if (!item.TryGetValue("PartitionKey", out object partitionKeyO))
        //             continue;
        //         
        //         toDelete.Add(new ItemReference(Id: idO.ToString(), PartitionKey: partitionKeyO.ToString()));
        //     }
        // }
        //
        // Log($"To delete {toDelete.Count.ToString()} from {containerId}");
        // foreach (var itemReference in toDelete)
        // {
        //     await container.DeleteItemAsync<object>(itemReference.Id, new PartitionKey(itemReference.PartitionKey));
        // }
        // Log($@"Cleared {containerId}");
    }

    private void Log(string msg) => Console.WriteLine($"{DateTimeOffset.Now.ToString()} - {msg}");

}