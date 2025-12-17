using Microsoft.EntityFrameworkCore;
using SampleServerSentEvent.Context;
using SampleServerSentEvent.Models;

namespace SampleServerSentEvent.Services;

public class CategoryService(ApplicationDbContext dbContext) : ICategoryService
{
    public async Task<List<Category>> GetCategories(List<long> ids)
    {
        return await dbContext.Categories
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
    }
}