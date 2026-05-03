using Microsoft.EntityFrameworkCore;
using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories.Implementations;

public class TagRepository : ITagRepository
{
    private readonly FinanceServiceDbContext _db;

    public TagRepository(FinanceServiceDbContext db) => _db = db;

    public async Task AddAsync(Tag tag)
    {
        await _db.Tags.AddAsync(tag);
    }

    public async Task<Tag?> GetAsync(Guid id) => await _db.Tags.FindAsync([id]);

    public async Task<IReadOnlyCollection<Tag>> GetBatchAsync(Guid spaceId) =>
        await _db.Tags.Where(t => t.SpaceId == spaceId).ToListAsync();
}
