using Microsoft.EntityFrameworkCore;
using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly FinanceServiceDbContext _dbContext;

    public CategoryRepository(FinanceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Category category)
    {
        await _dbContext.AddAsync(category);
    }

    public async Task<Category?> GetAsync(Guid categoryId)
    {
        return await _dbContext.Categories.FindAsync(categoryId);
    }

    public async Task<IReadOnlyCollection<Category>> GetBatchAsync(Guid spaceId)
    {
        return await _dbContext.Categories.Where(c => c.SpaceId == spaceId).ToListAsync();
    }
}
