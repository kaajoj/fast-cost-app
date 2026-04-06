using FastCost.Core.DAL.Entities;

namespace FastCost.Core.DAL
{
    public interface ICostRepository
    {
        Task<List<Cost>> GetCostsAsync();
        Task<Cost?> GetCostAsync(int id);
        Task<List<Cost>> GetCostsByMonth(DateTime date);
        Task<List<Cost>> GetCostsByDateRange(DateTime startDate, DateTime endDate);
        Task<int> SaveCostAsync(Cost cost);
        Task<int> DeleteCostAsync(Cost cost);
    }
}
