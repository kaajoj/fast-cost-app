using FastCost.Core.Models;
using Xunit;

namespace FastCostTests.Models
{
    public class CategoryModelTests
    {
        [Fact]
        public void Equals_ShouldReturnTrue_WhenSameName()
        {
            var a = new CategoryModel { Id = 1, Name = "food" };
            var b = new CategoryModel { Id = 2, Name = "food" };

            Assert.Equal(a, b);
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenDifferentName()
        {
            var a = new CategoryModel { Name = "food" };
            var b = new CategoryModel { Name = "transport" };

            Assert.NotEqual(a, b);
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenNull()
        {
            var a = new CategoryModel { Name = "food" };

            Assert.False(a.Equals(null));
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenDifferentType()
        {
            var a = new CategoryModel { Name = "food" };

            Assert.False(a.Equals("food"));
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenSameInstance()
        {
            var a = new CategoryModel { Name = "food" };

            Assert.True(a.Equals(a));
        }

        [Fact]
        public void GetHashCode_ShouldBeEqual_ForSameName()
        {
            var a = new CategoryModel { Id = 1, Name = "food" };
            var b = new CategoryModel { Id = 2, Name = "food" };

            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_ShouldDiffer_ForDifferentNames()
        {
            var a = new CategoryModel { Name = "food" };
            var b = new CategoryModel { Name = "transport" };

            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void Equals_ShouldAllowUseAsDictionaryKey_GroupingBySameName()
        {
            var costs = new[]
            {
                new { Category = new CategoryModel { Name = "food" }, Value = 10m },
                new { Category = new CategoryModel { Name = "food" }, Value = 20m },
                new { Category = new CategoryModel { Name = "transport" }, Value = 5m }
            };

            var groups = costs.GroupBy(c => c.Category).ToList();

            Assert.Equal(2, groups.Count);
            Assert.Equal(30m, groups.First(g => g.Key.Name == "food").Sum(c => c.Value));
        }
    }
}
