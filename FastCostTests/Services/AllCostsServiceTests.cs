using FastCost.DAL;
using FastCost.DAL.Entities;
using FastCost.Models;
using FastCost.Services;
using Mapster;
using Moq;
using Xunit;

namespace FastCostTests.Services
{
    public class AllCostsServiceTests
    {
        private readonly Mock<ICostRepository> _costRepositoryMock;
        private readonly AllCostsService _allCostsService;

        public AllCostsServiceTests()
        {
            _costRepositoryMock = new Mock<ICostRepository>();
            _allCostsService = new AllCostsService(_costRepositoryMock.Object);
        }

        [Fact]
        public async Task LoadCostsBackUp_ShouldReturnCosts()
        {
            // Arrange
            var costs = new List<Cost> { new Cost { Id = 1, Value = 100, Description = "Test", Date = DateTime.Now, CategoryId = 1 } };
            _costRepositoryMock.Setup(repo => repo.GetCostsAsync()).ReturnsAsync(costs);

            // Act
            var result = await _allCostsService.LoadCostsBackUp();

            // Assert
            Assert.Equal(costs, result);
        }

        [Fact]
        public async Task LoadCostsByMonth_ShouldReturnCostsForGivenMonth()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);
            var costs = new List<Cost> { new Cost { Id = 1, Value = 100, Description = "Test", Date = date, CategoryId = 1 } };
            _costRepositoryMock.Setup(repo => repo.GetCostsByMonth(date)).ReturnsAsync(costs);

            // Act
            var result = await _allCostsService.LoadCostsByMonth(date);

            // Assert
            Assert.Equal(costs, result);
        }

        [Fact]
        public async Task GetSum_ShouldReturnSumOfCostsForGivenMonth()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);
            var costs = new List<Cost> { new Cost { Id = 1, Value = 100, Description = "Test", Date = date, CategoryId = 1 } };
            _costRepositoryMock.Setup(repo => repo.GetCostsByMonth(date)).ReturnsAsync(costs);

            // Act
            var result = await _allCostsService.GetSum(date);

            // Assert
            Assert.Equal(100, result);
        }

        [Fact]
        public async Task GetCostsByMonthGroupByCategory_ShouldReturnGroupedCosts()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);
            var category = new Category { Id = 1, Name = "TestCategory" };
            var costs = new List<Cost> { new Cost { Id = 1, Value = 100, Description = "Test", Date = date, CategoryId = 1, Category = category } };
            var costModels = costs.Adapt<List<CostModel>>();
            _costRepositoryMock.Setup(repo => repo.GetCostsByMonth(date)).ReturnsAsync(costs);

            // Act
            var result = await _allCostsService.GetCostsByMonthGroupByCategory(date);

            // Assert
            var expected = costModels.GroupBy(c => c.Category);
            Assert.Equal(expected.Count(), result.Count());

            foreach (var expectedGroup in expected)
            {
                var actualGroup = result.First(g => g.Key.Id == expectedGroup.Key.Id);
                Assert.Equal(expectedGroup.Key.Id, actualGroup.Key.Id);
                Assert.Equal(expectedGroup.Key.Name, actualGroup.Key.Name);
                Assert.Equal(expectedGroup.Count(), actualGroup.Count());

                foreach (var expectedCost in expectedGroup)
                {
                    var actualCost = actualGroup.First(c => c.Id == expectedCost.Id);
                    Assert.Equal(expectedCost.Id, actualCost.Id);
                    Assert.Equal(expectedCost.Value, actualCost.Value);
                    Assert.Equal(expectedCost.Description, actualCost.Description);
                    Assert.Equal(expectedCost.Date, actualCost.Date);
                    Assert.Equal(expectedCost.CategoryId, actualCost.CategoryId);
                }
            }
        }
    }
}
