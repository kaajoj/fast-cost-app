using FastCost.Core.Models;
using FastCost.Core.Services;

namespace FastCost.Views;

public partial class AnalysisPage : ContentPage
{
    private readonly IAllCostsService _allCostsService;

    public AnalysisPage(IAllCostsService allCostsService)
    {
        InitializeComponent();
        _allCostsService = allCostsService;
        this.BindingContext = new AllCostsGroup();
    }

    private async void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI) return;
        await Shell.Current.GoToAsync("//allCosts");
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs state)
    {
        base.OnNavigatedTo(state);
        if (BindingContext is AllCostsGroup allCostsGroup)
        {
            await LoadAndDisplayCosts(allCostsGroup.SelectedDate);
        }
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
        if (BindingContext is AllCostsGroup allCostsGroup)
        {
            allCostsGroup.SelectedDate = date;

            allCostsGroup.GroupCosts.Clear();
            var groupCosts = await _allCostsService.GetCostsByMonthGroupByCategory(date);
            decimal totalSum = decimal.Zero;
            foreach (var costGroup in groupCosts)
            {
                decimal sumGroup = decimal.Zero;
                foreach (var cost in costGroup)
                {
                    sumGroup += cost.Value.GetValueOrDefault();
                }
                costGroup.Key.SumValue = sumGroup;
                totalSum += sumGroup;
                allCostsGroup.GroupCosts.Add(costGroup);
            }

            allCostsGroup.Sum = totalSum;
        }
    }
}