using FastCost.Core.DAL.Entities;
using FastCost.Core.Mappings;
using FastCost.Core.Models;
using Mapster;
using Xunit;

namespace FastCostTests.Mappings
{
    public class MapsterConfigTests
    {
        static MapsterConfigTests()
        {
            MapsterConfig.RegisterMappings();
        }

        [Fact]
        public void CostToCostModel_ShouldMapScalarFields()
        {
            var cost = new Cost
            {
                Id = 1,
                Value = 99.5m,
                Description = "Test",
                Date = new DateTime(2024, 3, 15),
                CategoryId = 2
            };

            var model = cost.Adapt<CostModel>();

            Assert.Equal(1, model.Id);
            Assert.Equal(99.5m, model.Value);
            Assert.Equal("Test", model.Description);
            Assert.Equal(new DateTime(2024, 3, 15), model.Date);
            Assert.Equal(2, model.CategoryId);
        }

        [Fact]
        public void CostToCostModel_ShouldMapCategory()
        {
            var cost = new Cost
            {
                Id = 1,
                Value = 10m,
                Date = DateTime.Now,
                Category = new Category { Id = 3, Name = "food" }
            };

            var model = cost.Adapt<CostModel>();

            Assert.NotNull(model.Category);
            Assert.Equal(3, model.Category.Id);
            Assert.Equal("food", model.Category.Name);
        }

        [Fact]
        public void CategoryToCategoryModel_ShouldMapAllFields()
        {
            var category = new Category { Id = 5, Name = "transport" };

            var model = category.Adapt<CategoryModel>();

            Assert.Equal(5, model.Id);
            Assert.Equal("transport", model.Name);
        }

        [Fact]
        public void CostModelToCost_ShouldMapScalarFields()
        {
            var model = new CostModel
            {
                Id = 7,
                Value = 55m,
                Description = "groceries",
                Date = new DateTime(2024, 5, 20),
                CategoryId = 4
            };

            var cost = model.Adapt<Cost>();

            Assert.Equal(7, cost.Id);
            Assert.Equal(55m, cost.Value);
            Assert.Equal("groceries", cost.Description);
            Assert.Equal(new DateTime(2024, 5, 20), cost.Date);
            Assert.Equal(4, cost.CategoryId);
        }

        [Fact]
        public void CostModelToCost_ShouldIgnoreCategoryNavigation()
        {
            var model = new CostModel
            {
                Id = 1,
                Value = 10m,
                Date = DateTime.Now,
                Category = new CategoryModel { Id = 3, Name = "food" }
            };

            var cost = model.Adapt<Cost>();

            // Category navigation property should not be mapped (it's ignored)
            Assert.Null(cost.Category);
        }

        [Fact]
        public void CostToCostModel_ShouldMapListOfCosts()
        {
            var costs = new List<Cost>
            {
                new Cost { Id = 1, Value = 10m, Date = DateTime.Now },
                new Cost { Id = 2, Value = 20m, Date = DateTime.Now },
            };

            var models = costs.Adapt<List<CostModel>>();

            Assert.Equal(2, models.Count);
            Assert.Equal(10m, models[0].Value);
            Assert.Equal(20m, models[1].Value);
        }
    }
}
