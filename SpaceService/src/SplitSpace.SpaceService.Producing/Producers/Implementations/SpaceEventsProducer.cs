using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using SplitSpace.SpaceService.Producing.Models.SpaceEvents;

namespace SplitSpace.SpaceService.Producing.Producers.Implementations;

public sealed class SpaceEventsProducer : ISpaceEventsProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<SpaceEventsProducer> _logger;
 
    public SpaceEventsProducer(
        ILogger<SpaceEventsProducer> logger)
    {
        _logger = logger;
 
        var config = new ProducerConfig
        {
            BootstrapServers = "kafka:29092",
            MessageTimeoutMs = 5_000,
 
            // Strongest durability guarantee: wait for all in-sync replicas
            Acks = Acks.All,
            
            // Compress in transit
            CompressionType = CompressionType.Snappy,
        };
 
        _producer = new ProducerBuilder<string, string>(config).Build();
    }
 
    public async Task ProduceAsync<TData>(SpaceEvent<TData> @event)
    {
        var key = @event.SpaceId.ToString();
        var value = JsonSerializer.Serialize(@event);
 
        var message = new Message<string, string>
        {
            Key = key,
            Value = value,
        };
 
        try
        {
            await _producer.ProduceAsync("space-events", message);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(
                ex,
                "Failed to produce event to topic {Topic}: {Reason}", "space-events", ex.Error.Reason);
 
            throw;
        }
    }
 
    public void Dispose()
    {
        // Flush ensures any buffered messages are delivered before shutdown
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}