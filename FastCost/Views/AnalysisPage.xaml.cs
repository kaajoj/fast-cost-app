using FastCost.Core.Models;
using FastCost.Core.Services;
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
        await LoadAndDisplayCosts(SelectedDate);
    }

    private async Task DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if(e.NewDate != null)
        {
            await LoadAndDisplayCosts((DateTime)e.NewDate);
        }
    }

    private async Task LoadAndDisplayCosts(DateTime date)
    {
        SelectedDate = date; // Update the bindable property

        GroupCosts.Clear();
        var groupCosts = await _allCostsService.GetCostsByMonthGroupByCategory(date);
        foreach (var costGroup in groupCosts)
        {
            decimal sumGroup = decimal.Zero;
            foreach (var cost in costGroup)
            {
                sumGroup += cost.Value.GetValueOrDefault();
            }
            costGroup.Key.SumValue = (decimal?)sumGroup;

            GroupCosts.Add(costGroup);
        }

        Sum = await _allCostsService.GetSum(date);

        BindingContext = this;
    }
}