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

        public Task<List<Cost>> LoadCostsBackUp()
        {
            return _costRepository.GetCostsAsync();
        }

        public Task<List<Cost>> LoadCostsByMonth(DateTime date)
        {
            return _costRepository.GetCostsByMonth(date);
        }

        public async Task<IEnumerable<IGrouping<CategoryModel, CostModel>>> GetCostsByMonthGroupByCategory(DateTime date)
        {
            var results = await _costRepository.GetCostsByMonth(date);

            return results.Adapt<List<CostModel>>()
                .Select(c =>
                {
                    c.Category ??= new CategoryModel { Name = "no category" };
                    return c;
                })
                .GroupBy(cost => cost.Category!);
        }

        public async Task<decimal> GetSum(DateTime date)
        {
            var costs = await _costRepository.GetCostsByMonth(date);
            return costs.Sum(c => c.Value);
        }

        public async Task<List<(string Month, decimal Total)>> GetMonthlyTotals(int months)
        {
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-(months - 1));
            var endDate = new DateTime(now.Year, now.Month, 1).AddMonths(1);

            var costs = await _costRepository.GetCostsByDateRange(startDate, endDate);

            var totalsByMonth = costs
                .GroupBy(c => new { c.Date.Year, c.Date.Month })
                .ToDictionary(g => (g.Key.Year, g.Key.Month), g => g.Sum(c => c.Value));

            var results = new List<(string Month, decimal Total)>(months);
            for (int i = months - 1; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                totalsByMonth.TryGetValue((date.Year, date.Month), out var total);
                results.Add((date.ToString("MMM"), total));
            }

            return results;
        }
    }
}
