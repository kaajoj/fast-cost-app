using FastCost.Core.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastCost.Core.DAL
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public CategoryRepository(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<Category>> GetCategories()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetCategory(int id)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> SaveCategory(Category category)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            if (category.Id != 0)
            {
                dbContext.Update(category);
            }
            else
            {
                await dbContext.Categories.AddAsync(category);
            }

            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteCategory(Category category)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Remove(category);
            return await dbContext.SaveChangesAsync();
        }
    }
}
