using FastCost.DAL.Entities;
using FastCost.Models;

namespace FastCost.Services
{
    public interface IAllCostsService
    {
        public Task<List<Cost>> LoadCostsBackUp();
        public Task<List<Cost>> LoadCostsByMonth(DateTime date);
        public Task<decimal> GetSum(DateTime date);
        public Task<IEnumerable<IGrouping<CategoryModel?, CostModel>>> GetCostsByMonthGroupByCategory(DateTime date);
    }
}
