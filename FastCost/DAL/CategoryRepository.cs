using FastCost.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastCost.DAL
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public CategoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Category>> GetCategories()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategory(int id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> SaveCategory(Category category)
        {
            if (category.Id != 0)
            {
                _dbContext.Categories.Update(category);
            }
            else
            {
                await _dbContext.Categories.AddAsync(category);
            }

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteCategory(Category category)
        {
            _dbContext.Categories.Remove(category);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
