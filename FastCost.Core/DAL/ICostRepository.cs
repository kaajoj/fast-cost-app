using FastCost.DAL.Entities;

namespace FastCost.DAL
{
    public interface ICostRepository
    {
        Task<List<Cost>> GetCostsAsync();
        Task<Cost?> GetCostAsync(int id);
        Task<List<Cost>> GetCostsByMonth(DateTime date);
        Task<int> SaveCostAsync(Cost cost);
        Task<int> DeleteCostAsync(Cost cost);
    }
}
