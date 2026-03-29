using FastCost.Core.Models;
using System.ComponentModel;
using Xunit;

namespace FastCostTests.Models
{
    public class CostModelTests
    {
        [Fact]
        public void FormattedDate_ShouldFormatAsDdMm()
        {
            var model = new CostModel { Date = new DateTime(2024, 3, 5) };

            Assert.Equal("05.03", model.FormattedDate);
        }

        [Fact]
        public void FormattedDate_ShouldPadDayAndMonth()
        {
            var model = new CostModel { Date = new DateTime(2024, 1, 7) };

            Assert.Equal("07.01", model.FormattedDate);
        }

        [Fact]
        public void Value_ShouldRaisePropertyChanged_WhenValueChanges()
        {
            var model = new CostModel { Value = 10m };
            string? changedProperty = null;
            model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

            model.Value = 20m;

            Assert.Equal(nameof(CostModel.Value), changedProperty);
        }

        [Fact]
        public void Value_ShouldNotRaisePropertyChanged_WhenSameValue()
        {
            var model = new CostModel { Value = 10m };
            bool raised = false;
            model.PropertyChanged += (_, _) => raised = true;

            model.Value = 10m;

            Assert.False(raised);
        }

        [Fact]
        public void Date_ShouldRaisePropertyChanged_WhenValueChanges()
        {
            var model = new CostModel { Date = new DateTime(2024, 1, 1) };
            string? changedProperty = null;
            model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

            model.Date = new DateTime(2024, 6, 15);

            Assert.Equal(nameof(CostModel.Date), changedProperty);
        }

        [Fact]
        public void Date_ShouldNotRaisePropertyChanged_WhenSameValue()
        {
            var date = new DateTime(2024, 1, 1);
            var model = new CostModel { Date = date };
            bool raised = false;
            model.PropertyChanged += (_, _) => raised = true;

            model.Date = date;

            Assert.False(raised);
        }

        [Fact]
        public void Value_ShouldRaisePropertyChanged_WhenChangedFromNull()
        {
            var model = new CostModel { Value = null };
            string? changedProperty = null;
            model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

            model.Value = 5m;

            Assert.Equal(nameof(CostModel.Value), changedProperty);
        }
    }
}
