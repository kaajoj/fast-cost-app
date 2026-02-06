using FastCost.Models;
using FastCost.Services;
using System.Collections.ObjectModel;

namespace FastCost.Views;

public partial class AnalysisPage : ContentPage
{
    private readonly IAllCostsService _allCostsService;

    public AnalysisPage(IAllCostsService allCostsService)
    {
        InitializeComponent();
        _allCostsService = allCostsService;
    }

    public ObservableCollection<IGrouping<CategoryModel, CostModel>> GroupCosts { get; set; } = new();

    public DateTime selectedDate = DateTime.Now;
    public DateTime SelectedDate
    {
        get { return selectedDate; }
        set
        {
            if (selectedDate != value)
            {
                selectedDate = value;
                OnPropertyChanged();
            }
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

    protected override async void OnNavigatedTo(NavigatedToEventArgs state)
    {
        base.OnNavigatedTo(state);

        var currentDate = SelectedDate;

        GroupCosts.Clear();
        var groupCosts = await _allCostsService?.GetCostsByMonthGroupByCategory(currentDate);
        foreach (var costGroup in groupCosts)
        {
            decimal? sumGroup = decimal.Zero;
            foreach (var cost in costGroup)
            {
                sumGroup += cost.Value;
            }
            costGroup.Key.SumValue = sumGroup;

            GroupCosts.Add(costGroup);
        }

        Sum = await _allCostsService.GetSum(currentDate);

        BindingContext = this;
    }

    private async void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        DateTime selectedDate = e.NewDate;

        GroupCosts.Clear();
        var groupCosts = await _allCostsService?.GetCostsByMonthGroupByCategory(selectedDate);
        foreach (var costGroup in groupCosts)
        {
            decimal? sumGroup = decimal.Zero;
            foreach (var cost in costGroup)
            {
                sumGroup += cost.Value;
            }
            costGroup.Key.SumValue = sumGroup;

            GroupCosts.Add(costGroup);
        }

        Sum = await _allCostsService.GetSum(selectedDate);

        BindingContext = this;
    }
}