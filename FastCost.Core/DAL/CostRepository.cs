using FastCost.Core.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastCost.Core.DAL
{
    public class CostRepository : ICostRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public CostRepository(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<Cost>> GetCostsAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Costs
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Cost>> GetCostsByMonth(DateTime date)
        {
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1);

            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Costs
                .AsNoTracking()
                .Where(c => c.Date >= startDate && c.Date < endDate)
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<List<Cost>> GetCostsByDateRange(DateTime startDate, DateTime endDate)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Costs
                .AsNoTracking()
                .Where(c => c.Date >= startDate && c.Date < endDate)
                .ToListAsync();
        }

        public async Task<Cost?> GetCostAsync(int id)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Costs
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<int> SaveCostAsync(Cost cost)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            if (cost.Id != 0)
            {
                dbContext.Update(cost);
            }
            else
            {
                await dbContext.Costs.AddAsync(cost);
            }

            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteCostAsync(Cost cost)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Costs.Remove(cost);
            return await dbContext.SaveChangesAsync();
        }
    }
}
