using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FastCost.DAL.Entities;

namespace FastCost.Models
{
    public class AllCostsGroup : INotifyPropertyChanged
    {
        public ObservableCollection<IGrouping<Category, CostModel>> GroupCosts { get; set; } = new();

        private string _selectedDate;
        public string SelectedDate
        {
            get
            {
                return _selectedDate ?? DateTime.Now.ToString();
            }
            set
            {
                _selectedDate = value;
            }
        }

        public decimal sum = 0;
        public decimal Sum
        {
            get { return sum; }
            set
            {
                if (sum != value)
                {
                    sum = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
