using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        // Cosmos DB and Service Bus configuration should come from settings
        services.AddSingleton<ICosmosDbService, CosmosDbService>(sp =>
            new CosmosDbService(
                hostContext.Configuration["CosmosDb:ConnectionString"],
                hostContext.Configuration["CosmosDb:DatabaseId"],
                hostContext.Configuration["CosmosDb:ContainerId"]));

        services.AddSingleton<IServiceBusPublisher, ServiceBusPublisher>(sp =>
            new ServiceBusPublisher(
                hostContext.Configuration["ServiceBus:ConnectionString"],
                hostContext.Configuration["ServiceBus:TopicName"]));

        services.AddHostedService<EventPollingService>();
    })
    .Build();

await host.RunAsync();