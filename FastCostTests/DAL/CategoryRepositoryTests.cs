using FastCost.Core.DAL;
using FastCost.Core.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FastCostTests.DAL
{
    public class CategoryRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        private static CancellationToken Ct => TestContext.Current.CancellationToken;

        private AppDbContext CreateContext() => new(_options);
        private CategoryRepository CreateRepo() => new(new TestDbContextFactory(_options));

        [Fact]
        public async Task GetCategories_ShouldReturnAllCategories()
        {
            using var context = CreateContext();
            context.Categories.AddRange(
                new Category { Name = "food" },
                new Category { Name = "transport" }
            );
            await context.SaveChangesAsync(Ct);
            var repo = CreateRepo();

            var result = await repo.GetCategories();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnEmpty_WhenNoCategories()
        {
            using var context = CreateContext();
            var repo = CreateRepo();

            var result = await repo.GetCategories();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCategory_ShouldReturnCategory_WhenExists()
        {
            using var context = CreateContext();
            var category = new Category { Name = "shopping" };
            context.Categories.Add(category);
            await context.SaveChangesAsync(Ct);
            var repo = CreateRepo();

            var result = await repo.GetCategory(category.Id);

            Assert.NotNull(result);
            Assert.Equal("shopping", result.Name);
        }

        [Fact]
        public async Task GetCategory_ShouldReturnNull_WhenNotExists()
        {
            using var context = CreateContext();
            var repo = CreateRepo();

            var result = await repo.GetCategory(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task SaveCategory_ShouldInsert_WhenIdIsZero()
        {
            using var context = CreateContext();
            var repo = CreateRepo();
            var category = new Category { Name = "newCategory" };

            await repo.SaveCategory(category);

            using var verify = CreateContext();
            var saved = await verify.Categories.FirstOrDefaultAsync(c => c.Name == "newCategory", Ct);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task SaveCategory_ShouldUpdate_WhenIdIsNotZero()
        {
            using var context = CreateContext();
            var category = new Category { Name = "original" };
            context.Categories.Add(category);
            await context.SaveChangesAsync(Ct);
            var repo = CreateRepo();

            category.Name = "updated";
            await repo.SaveCategory(category);

            using var verify = CreateContext();
            var updated = await verify.Categories.FindAsync([category.Id], Ct);
            Assert.NotNull(updated);
            Assert.Equal("updated", updated.Name);
        }

        [Fact]
        public async Task DeleteCategory_ShouldRemoveCategory()
        {
            using var context = CreateContext();
            var category = new Category { Name = "toDelete" };
            context.Categories.Add(category);
            await context.SaveChangesAsync(Ct);
            var repo = CreateRepo();

            await repo.DeleteCategory(category);

            using var verify = CreateContext();
            var deleted = await verify.Categories.FindAsync([category.Id], Ct);
            Assert.Null(deleted);
        }
    }
}
