using FastCost.Core.DAL;
using FastCost.Core.DAL.Entities;
using FastCost.Core.Models;
using Mapster;

namespace FastCost.Core.Services
{
    public class AllCostsService : IAllCostsService
    {
        private readonly ICostRepository _costRepository;

        public AllCostsService(ICostRepository costRepository)
        {
            _costRepository = costRepository;
        }

        public async Task<List<Cost>> LoadCostsBackUp()
        {
            var results = await _costRepository.GetCostsAsync();
            return results;
        }

        public async Task<List<Cost>> LoadCostsByMonth(DateTime date)
        {
            return await _costRepository.GetCostsByMonth(date);
        }

        public async Task<decimal> GetSum(DateTime date)
        {
            var costs = await _costRepository.GetCostsByMonth(date);
            return costs.Sum(c => c.Value);
        }

        public async Task<List<(string Month, decimal Total)>> GetMonthlyTotals(int months)
        {
            var results = new List<(string Month, decimal Total)>();
            var now = DateTime.Now;

            for (int i = months - 1; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                var costs = await _costRepository.GetCostsByMonth(date);
                var total = costs.Sum(c => c.Value);
                results.Add((date.ToString("MMM"), total));
            }

            return results;
        }

        public async Task<IEnumerable<IGrouping<CategoryModel, CostModel>>> GetCostsByMonthGroupByCategory(DateTime date)
        {
            var results = await _costRepository.GetCostsByMonth(date);
            var costs = results.Adapt<List<CostModel>>();

            foreach (var cost in costs)
            {
                if (cost.Category is null)
                {
                    cost.Category = new CategoryModel { Name = "no category" };
                }
            }

            return costs.GroupBy(cost => cost.Category);
        }
    }
}
