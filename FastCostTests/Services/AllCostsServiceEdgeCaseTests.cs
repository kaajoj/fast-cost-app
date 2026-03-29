using FastCost.Core.DAL;
using FastCost.Core.DAL.Entities;
using FastCost.Core.Services;
using Moq;
using Xunit;

namespace FastCostTests.Services
{
    public class AllCostsServiceEdgeCaseTests
    {
        private readonly Mock<ICostRepository> _costRepositoryMock;
        private readonly AllCostsService _allCostsService;

        public AllCostsServiceEdgeCaseTests()
        {
            _costRepositoryMock = new Mock<ICostRepository>();
            _allCostsService = new AllCostsService(_costRepositoryMock.Object);
        }

        [Fact]
        public async Task LoadCostsBackUp_ShouldReturnEmpty_WhenNoCosts()
        {
            _costRepositoryMock.Setup(r => r.GetCostsAsync()).ReturnsAsync(new List<Cost>());

            var result = await _allCostsService.LoadCostsBackUp();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSum_ShouldReturnZero_WhenNoCostsForMonth()
        {
            var date = new DateTime(2024, 3, 1);
            _costRepositoryMock.Setup(r => r.GetCostsByMonth(date)).ReturnsAsync(new List<Cost>());

            var result = await _allCostsService.GetSum(date);

            Assert.Equal(0m, result);
        }

        [Fact]
        public async Task GetSum_ShouldSumAllCostValues()
        {
            var date = new DateTime(2024, 3, 1);
            var costs = new List<Cost>
            {
                new Cost { Value = 10.5m, Date = date },
                new Cost { Value = 20.25m, Date = date },
                new Cost { Value = 5m, Date = date }
            };
            _costRepositoryMock.Setup(r => r.GetCostsByMonth(date)).ReturnsAsync(costs);

            var result = await _allCostsService.GetSum(date);

            Assert.Equal(35.75m, result);
        }

        [Fact]
        public async Task LoadCostsByMonth_ShouldReturnEmpty_WhenNoCostsForMonth()
        {
            var date = new DateTime(2024, 3, 1);
            _costRepositoryMock.Setup(r => r.GetCostsByMonth(date)).ReturnsAsync(new List<Cost>());

            var result = await _allCostsService.LoadCostsByMonth(date);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCostsByMonthGroupByCategory_ShouldAssignNoCategoryLabel_WhenCategoryIsNull()
        {
            var date = new DateTime(2024, 3, 1);
            var costs = new List<Cost>
            {
                new Cost { Id = 1, Value = 10m, Date = date, CategoryId = null }
            };
            _costRepositoryMock.Setup(r => r.GetCostsByMonth(date)).ReturnsAsync(costs);

            var result = (await _allCostsService.GetCostsByMonthGroupByCategory(date)).ToList();

            Assert.Single(result);
            Assert.Equal("no category", result[0].Key.Name);
        }

        [Fact]
        public async Task GetCostsByMonthGroupByCategory_ShouldGroupMultipleCostsUnderSameCategory()
        {
            var date = new DateTime(2024, 3, 1);
            var category = new Category { Id = 1, Name = "food" };
            var costs = new List<Cost>
            {
                new Cost { Id = 1, Value = 10m, Date = date, Category = category, CategoryId = 1 },
                new Cost { Id = 2, Value = 20m, Date = date, Category = category, CategoryId = 1 }
            };
            _costRepositoryMock.Setup(r => r.GetCostsByMonth(date)).ReturnsAsync(costs);

            var result = (await _allCostsService.GetCostsByMonthGroupByCategory(date)).ToList();

            Assert.Single(result);
            Assert.Equal(2, result[0].Count());
        }

        [Fact]
        public async Task GetCostsByMonthGroupByCategory_ShouldCreateSeparateGroups_ForDifferentCategories()
        {
            var date = new DateTime(2024, 3, 1);
            var costs = new List<Cost>
            {
                new Cost { Id = 1, Value = 10m, Date = date, Category = new Category { Id = 1, Name = "food" }, CategoryId = 1 },
                new Cost { Id = 2, Value = 20m, Date = date, Category = new Category { Id = 2, Name = "transport" }, CategoryId = 2 }
            };
            _costRepositoryMock.Setup(r => r.GetCostsByMonth(date)).ReturnsAsync(costs);

            var result = (await _allCostsService.GetCostsByMonthGroupByCategory(date)).ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCostsByMonthGroupByCategory_ShouldReturnEmpty_WhenNoCosts()
        {
            var date = new DateTime(2024, 3, 1);
            _costRepositoryMock.Setup(r => r.GetCostsByMonth(date)).ReturnsAsync(new List<Cost>());

            var result = (await _allCostsService.GetCostsByMonthGroupByCategory(date)).ToList();

            Assert.Empty(result);
        }
    }
}
