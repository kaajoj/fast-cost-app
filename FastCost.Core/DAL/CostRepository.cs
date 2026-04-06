using FastCost.Core.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastCost.Core.DAL
{
    public class CostRepository : ICostRepository
    {
        private readonly AppDbContext _dbContext;

        public CostRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Cost>> GetCostsAsync()
        {
            return await _dbContext.Costs.ToListAsync();
        }

        public async Task<List<Cost>> GetCostsByMonth(DateTime date)
        {
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1);

            return await _dbContext.Costs
                .Where(c => c.Date >= startDate && c.Date < endDate)
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<List<Cost>> GetCostsByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Costs
                .Where(c => c.Date >= startDate && c.Date < endDate)
                .ToListAsync();
        }

        public async Task<Cost?> GetCostAsync(int id)
        {
            return await _dbContext.Costs.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<int> SaveCostAsync(Cost cost)
        {
            if (cost.Id != 0)
            {
                var existing = await _dbContext.Costs.FindAsync(cost.Id);
                if (existing != null)
                    _dbContext.Entry(existing).CurrentValues.SetValues(cost);
            }
            else
            {
                await _dbContext.Costs.AddAsync(cost);
            }

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteCostAsync(Cost cost)
        {
            var existing = await _dbContext.Costs.FindAsync(cost.Id);
            if (existing != null)
                _dbContext.Costs.Remove(existing);

            return await _dbContext.SaveChangesAsync();
        }
    }
}
