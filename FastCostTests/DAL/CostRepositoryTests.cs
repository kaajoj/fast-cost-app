using FastCost.Core.DAL;
using FastCost.Core.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FastCostTests.DAL
{
    public class CostRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        private AppDbContext CreateContext() => new(_options);
        private CostRepository CreateRepo() => new(new TestDbContextFactory(_options));

        [Fact]
        public async Task GetCostsAsync_ShouldReturnAllCosts()
        {
            using var context = CreateContext();
            context.Costs.AddRange(
                new Cost { Value = 10, Date = DateTime.Now },
                new Cost { Value = 20, Date = DateTime.Now }
            );
            await context.SaveChangesAsync();
            var repo = CreateRepo();

            var result = await repo.GetCostsAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCostsAsync_ShouldReturnEmpty_WhenNoCosts()
        {
            using var context = CreateContext();
            var repo = CreateRepo();

            var result = await repo.GetCostsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCostsByMonth_ShouldReturnOnlyCostsForGivenMonth()
        {
            using var context = CreateContext();
            var targetDate = new DateTime(2024, 3, 15);
            context.Costs.AddRange(
                new Cost { Value = 10, Date = new DateTime(2024, 3, 5) },
                new Cost { Value = 20, Date = new DateTime(2024, 3, 28) },
                new Cost { Value = 30, Date = new DateTime(2024, 2, 15) },
                new Cost { Value = 40, Date = new DateTime(2024, 4, 1) }
            );
            await context.SaveChangesAsync();
            var repo = CreateRepo();

            var result = await repo.GetCostsByMonth(targetDate);

            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.Equal(3, c.Date.Month));
        }

        [Fact]
        public async Task GetCostsByMonth_ShouldIncludeCostOnFirstDayOfMonth()
        {
            using var context = CreateContext();
            context.Costs.Add(new Cost { Value = 10, Date = new DateTime(2024, 3, 1) });
            await context.SaveChangesAsync();
            var repo = CreateRepo();

            var result = await repo.GetCostsByMonth(new DateTime(2024, 3, 10));

            Assert.Single(result);
        }

        [Fact]
        public async Task GetCostsByMonth_ShouldExcludeCostOnFirstDayOfNextMonth()
        {
            using var context = CreateContext();
            context.Costs.Add(new Cost { Value = 10, Date = new DateTime(2024, 4, 1) });
            await context.SaveChangesAsync();
            var repo = CreateRepo();

            var result = await repo.GetCostsByMonth(new DateTime(2024, 3, 10));

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCostsByMonth_ShouldIncludeCategory()
        {
            using var context = CreateContext();
            var category = new Category { Name = "food" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            context.Costs.Add(new Cost { Value = 10, Date = new DateTime(2024, 3, 5), CategoryId = category.Id });
            await context.SaveChangesAsync();
            var repo = CreateRepo();

            var result = await repo.GetCostsByMonth(new DateTime(2024, 3, 1));

            Assert.Single(result);
            Assert.NotNull(result[0].Category);
            Assert.Equal("food", result[0].Category.Name);
        }

        [Fact]
        public async Task GetCostAsync_ShouldReturnCost_WhenExists()
        {
            using var context = CreateContext();
            var cost = new Cost { Value = 50, Date = DateTime.Now };
            context.Costs.Add(cost);
            await context.SaveChangesAsync();
            var repo = CreateRepo();

            var result = await repo.GetCostAsync(cost.Id);

            Assert.NotNull(result);
            Assert.Equal(50, result.Value);
        }

        [Fact]
        public async Task GetCostAsync_ShouldReturnNull_WhenNotExists()
        {
            using var context = CreateContext();
            var repo = CreateRepo();

            var result = await repo.GetCostAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task SaveCostAsync_ShouldInsert_WhenIdIsZero()
        {
            using var context = CreateContext();
            var repo = CreateRepo();
            var cost = new Cost { Value = 99, Date = DateTime.Now };

            await repo.SaveCostAsync(cost);

            var saved = await context.Costs.FirstOrDefaultAsync(c => c.Value == 99);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task SaveCostAsync_ShouldUpdate_WhenIdIsNotZero()
        {
            using var context = CreateContext();
            var cost = new Cost { Value = 10, Description = "original", Date = DateTime.Now };
            context.Costs.Add(cost);
            await context.SaveChangesAsync();
            var repo = CreateRepo();

            cost.Value = 99;
            cost.Description = "updated";
            await repo.SaveCostAsync(cost);

            using var verify = CreateContext();
            var updated = await verify.Costs.FindAsync(cost.Id);
            Assert.NotNull(updated);
            Assert.Equal(99, updated.Value);
            Assert.Equal("updated", updated.Description);
        }

        [Fact]
        public async Task DeleteCostAsync_ShouldRemoveCost()
        {
            using var context = CreateContext();
            var cost = new Cost { Value = 50, Date = DateTime.Now };
            context.Costs.Add(cost);
            await context.SaveChangesAsync();
            var repo = CreateRepo();

            await repo.DeleteCostAsync(cost);

            using var verify = CreateContext();
            Assert.Empty(verify.Costs);
        }
    }
}
