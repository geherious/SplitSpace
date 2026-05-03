using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories;

public interface ITagRepository
{
    Task AddAsync(Tag tag);
    
    Task<Tag?> GetAsync(Guid id);
    
    Task<IReadOnlyCollection<Tag>> GetBatchAsync(Guid spaceId);
}
