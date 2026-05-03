using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SplitSpace.FinanceService.Consuming.Models.Events.SpaceEvents;
using SplitSpace.FinanceService.Dal.Models.Entities;
using SplitSpace.FinanceService.Dal.Repositories;

namespace SplitSpace.FinanceService.Consuming.Consumers;

public sealed class SpaceEventsConsumer : BackgroundService
{
    private readonly ILogger<SpaceEventsConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;


    public SpaceEventsConsumer(
        ILogger<SpaceEventsConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run on a background thread so it doesn't block the host startup
        return Task.Run(() => ConsumeLoop(stoppingToken), stoppingToken);
    }

    private async Task ConsumeLoop(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "kafka:29092",
            GroupId = "space-events-finance-service",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            
            EnableAutoCommit = false,
        };

        using var consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
                _logger.LogError("Kafka consumer error: {Reason} (fatal={IsFatal})", e.Reason, e.IsFatal))
            .SetPartitionsAssignedHandler((_, partitions) =>
                _logger.LogInformation("Partitions assigned: {Partitions}", string.Join(", ", partitions)))
            .SetPartitionsRevokedHandler((_, partitions) =>
                _logger.LogInformation("Partitions revoked: {Partitions}", string.Join(", ", partitions)))
            .Build();

        consumer.Subscribe("space-events");

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<string, string>? result;

            try
            {
                result = consumer.Consume(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown — exit the loop cleanly
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Consume error on topic {Topic}: {Reason}", "space-events", ex.Error.Reason);
                continue;
            }

            if (result.IsPartitionEOF)
            {
                _logger.LogDebug("Reached end of partition {Partition}", result.Partition.Value);
                continue;
            }

            try
            {
                var @event = Deserialize(result.Message.Value);
                if (@event is not null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    await HandleEventAsync(@event, scope.ServiceProvider);
                }
            }
            catch (Exception ex)
            {
                // Log and skip — offset is still committed below so we don't reprocess
                _logger.LogError(ex,
                    "Failed to handle event from topic {Topic} partition {Partition} offset {Offset}. Skipping.",
                    result.Topic, result.Partition.Value, result.Offset.Value);
            }
            finally
            {
                // Always commit so a bad message doesn't block the partition forever
                try { consumer.Commit(result); }
                catch (KafkaException ex)
                {
                    _logger.LogWarning(ex, "Failed to commit offset for partition {Partition}", result.Partition.Value);
                }
            }
        }

        _logger.LogInformation("Kafka consumer stopping, closing gracefully...");
        consumer.Close();
    }

    private SpaceEvent? Deserialize(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<SpaceEvent>(json);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message: {Raw}", json);
            return null;
        }
    }

    private async Task HandleEventAsync(SpaceEvent @event, IServiceProvider services)
    {
        if (@event.Type == nameof(UsersAddedToSpaceEventData))
        {
            UsersAddedToSpaceEventData? data;
            try
            {
                data = @event.Data.Deserialize<UsersAddedToSpaceEventData>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize message: {Raw}", @event.Data);
                return;
            }

            if (data is null)
            {
                _logger.LogError("Message is null");
                return;
            }

            var spaceMemberships = data.UserIds
                .Select(id => new SpaceMembership
                {
                    Id = Guid.CreateVersion7(),
                    SpaceId = data.SpaceId,
                    UserId = id
                })
                .ToArray();
            
            var spaceMembershipRepository = services.GetRequiredService<ISpaceMembershipRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            await spaceMembershipRepository.AddBatchAsync(spaceMemberships);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
