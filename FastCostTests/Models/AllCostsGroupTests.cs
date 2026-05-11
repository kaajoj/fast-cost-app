using FastCost.Core.Models;
using Xunit;

namespace FastCostTests.Models
{
    public class AllCostsGroupTests
    {
        [Fact]
        public void SelectedDate_ShouldRaisePropertyChanged_WhenValueChanges()
        {
            var model = new AllCostsGroup { SelectedDate = new DateTime(2024, 1, 1) };
            string? changedProperty = null;
            model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

            model.SelectedDate = new DateTime(2024, 6, 1);

            Assert.Equal(nameof(AllCostsGroup.SelectedDate), changedProperty);
        }

        [Fact]
        public void SelectedDate_ShouldNotRaisePropertyChanged_WhenSameValue()
        {
            var date = new DateTime(2024, 1, 1);
            var model = new AllCostsGroup { SelectedDate = date };
            bool raised = false;
            model.PropertyChanged += (_, _) => raised = true;

            model.SelectedDate = date;

            Assert.False(raised);
        }

        [Fact]
        public void Sum_ShouldRaisePropertyChanged_WhenValueChanges()
        {
            var model = new AllCostsGroup { Sum = 0m };
            string? changedProperty = null;
            model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

            model.Sum = 200m;

            Assert.Equal(nameof(AllCostsGroup.Sum), changedProperty);
        }

        [Fact]
        public void Sum_ShouldNotRaisePropertyChanged_WhenSameValue()
        {
            var model = new AllCostsGroup { Sum = 75m };
            bool raised = false;
            model.PropertyChanged += (_, _) => raised = true;

            model.Sum = 75m;

            Assert.False(raised);
        }

        [Fact]
        public void GroupCosts_ShouldBeEmptyByDefault()
        {
            var model = new AllCostsGroup();

            Assert.NotNull(model.GroupCosts);
            Assert.Empty(model.GroupCosts);
        }

        [Fact]
        public void Sum_ShouldBeZeroByDefault()
        {
            var model = new AllCostsGroup();

            Assert.Equal(0m, model.Sum);
        }
    }
}
