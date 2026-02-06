using FastCost.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastCost.DAL
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

        public async Task<Cost?> GetCostAsync(int id)
        {
            return await _dbContext.Costs.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<int> SaveCostAsync(Cost cost)
        {   
            if (cost.Id != 0)
            {
                var existingCost = await _dbContext.Costs.FirstOrDefaultAsync(c => c.Id == cost.Id);
                if (existingCost != null)
                {
                    existingCost.Value = cost.Value;
                    existingCost.Description = cost.Description;
                    existingCost.Date = cost.Date;
                    existingCost.CategoryId = cost.CategoryId;
                    _dbContext.Costs.Update(existingCost);
                }
            }
            else
            {
                await _dbContext.Costs.AddAsync(cost);
            }

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteCostAsync(Cost cost)
        {
            var existingCost = await _dbContext.Costs.FirstOrDefaultAsync(c => c.Id == cost.Id);
            if (existingCost != null)
            {
                _dbContext.Costs.Remove(existingCost);
            }

            return await _dbContext.SaveChangesAsync();
        }
    }
}
