using FastCost.Core.Models;
using System.Collections.ObjectModel;
using Xunit;

namespace FastCostTests.Models
{
    public class AllCostsTests
    {
        [Fact]
        public void Costs_ShouldRaisePropertyChanged()
        {
            var model = new AllCosts();
            string? changedProperty = null;
            model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

            model.Costs = new ObservableCollection<CostModel>();

            Assert.Equal(nameof(AllCosts.Costs), changedProperty);
        }

        [Fact]
        public void SelectedDate_ShouldRaisePropertyChanged_WhenValueChanges()
        {
            var model = new AllCosts { SelectedDate = new DateTime(2024, 1, 1) };
            string? changedProperty = null;
            model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

            model.SelectedDate = new DateTime(2024, 6, 1);

            Assert.Equal(nameof(AllCosts.SelectedDate), changedProperty);
        }

        [Fact]
        public void SelectedDate_ShouldNotRaisePropertyChanged_WhenSameValue()
        {
            var date = new DateTime(2024, 1, 1);
            var model = new AllCosts { SelectedDate = date };
            bool raised = false;
            model.PropertyChanged += (_, _) => raised = true;

            model.SelectedDate = date;

            Assert.False(raised);
        }

        [Fact]
        public void Sum_ShouldRaisePropertyChanged_WhenValueChanges()
        {
            var model = new AllCosts { Sum = 0m };
            string? changedProperty = null;
            model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

            model.Sum = 100m;

            Assert.Equal(nameof(AllCosts.Sum), changedProperty);
        }

        [Fact]
        public void Sum_ShouldNotRaisePropertyChanged_WhenSameValue()
        {
            var model = new AllCosts { Sum = 50m };
            bool raised = false;
            model.PropertyChanged += (_, _) => raised = true;

            model.Sum = 50m;

            Assert.False(raised);
        }

        [Fact]
        public void Costs_ShouldBeEmptyByDefault()
        {
            var model = new AllCosts();

            Assert.NotNull(model.Costs);
            Assert.Empty(model.Costs);
        }

        [Fact]
        public void Sum_ShouldBeZeroByDefault()
        {
            var model = new AllCosts();

            Assert.Equal(0m, model.Sum);
        }
    }
}
