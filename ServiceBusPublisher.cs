using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Threading.Tasks;

public class ServiceBusPublisher : IServiceBusPublisher
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public ServiceBusPublisher(string connectionString, string topicName)
    {
        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(topicName);
    }

    public async Task PublishAsync(EventDocument eventDocument)
    {
        var messageBody = JsonSerializer.Serialize(eventDocument);
        var message = new ServiceBusMessage(messageBody)
        {
            CorrelationId = eventDocument.CorrelationId ?? System.Guid.NewGuid().ToString()
        };
        await _sender.SendMessageAsync(message);
    }

    // Make sure to properly dispose of resources
    public async ValueTask DisposeAsync()
    {
        await _sender.CloseAsync();
        await _client.DisposeAsync();
    }
}