using SplitSpace.SpaceService.Producing.Models.SpaceEvents;

namespace SplitSpace.SpaceService.Producing.Producers;

public interface ISpaceEventsProducer
{
    Task ProduceAsync<TData>(SpaceEvent<TData> @event);
}