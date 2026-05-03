using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    
    Task<Category?> GetAsync(Guid categoryId);
    Task<IReadOnlyCollection<Category>> GetBatchAsync(Guid spaceId);
}
