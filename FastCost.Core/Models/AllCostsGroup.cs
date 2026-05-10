using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FastCost.Core.Models
{
    public class AllCostsGroup : INotifyPropertyChanged
    {
        private ObservableCollection<IGrouping<CategoryModel, CostModel>> _groupCosts = new();
        public ObservableCollection<IGrouping<CategoryModel, CostModel>> GroupCosts 
        { 
            get => _groupCosts;
            set
            {
                if (_groupCosts != value)
                {
                    _groupCosts = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _sum = 0;
        public decimal Sum
        {
            get => _sum;
            set
            {
                if (_sum != value)
                {
                    _sum = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
