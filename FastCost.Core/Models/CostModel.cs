using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FastCost.Core.Models
{
    public partial class CostModel : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private decimal? _value = null;
        public decimal? Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Description { get; set; }

        public int? CategoryId { get; set; }
        public CategoryModel? Category { get; set; }

        public string FormattedDate => Date.ToString("dd.MM");

        private DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
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
