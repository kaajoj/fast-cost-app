using System.Collections.ObjectModel;
using FastCost.Core.Models;
using FastCost.Core.Services;
using Mapster;

namespace FastCost.Views;

public partial class AllCostsPage : ContentPage
{
    private readonly IAllCostsService _allCostsService;
    private bool _isNavigating = false;

    public AllCostsPage(IAllCostsService allCostsService)
	{
        InitializeComponent();
        _allCostsService = allCostsService;
        this.BindingContext = new AllCosts();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs state)
    {
        base.OnNavigatedTo(state);
        _isNavigating = true;

        if (BindingContext is AllCosts allCosts)
        {
            var costs = await _allCostsService.LoadCostsByMonth(allCosts.SelectedDate);
            allCosts.Costs = new ObservableCollection<CostModel>(costs.Adapt<List<CostModel>>().OrderBy(c => c.Date));
            allCosts.Sum = allCosts.Costs.Sum(c => c.Value ?? 0);
        }

        _isNavigating = false;
    }

    private async void OnSwipedLeft(object sender, SwipedEventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI) return;
        await Shell.Current.GoToAsync("//analysis");
    }

    private async void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI) return;
        await Shell.Current.GoToAsync("//mainPage");
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CostPage));
    }

    private async void OnCostTapped(object sender, EventArgs e)
    {
        if (_isNavigating) return;

        var grid = (Grid)sender;
        if (grid.BindingContext is not CostModel cost) return;

        _isNavigating = true;
        try
        {
            // Manual highlight using VisualStateManager
            VisualStateManager.GoToState(grid, "Selected");
            
            // Allow time for the highlight to be seen
            await Task.Delay(200);

            await Shell.Current.GoToAsync($"{nameof(CostPage)}?{nameof(CostPage.ItemId)}={cost.Id}");
            
            // Reset state so it's normal when coming back
            VisualStateManager.GoToState(grid, "Normal");
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private void OnPreviousMonth(object sender, EventArgs e)
    {
        if (allCostsDatePicker.Date.HasValue)
        {
            allCostsDatePicker.Date = allCostsDatePicker.Date.Value.AddMonths(-1);
        }
    }

    private void OnNextMonth(object sender, EventArgs e)
    {
        if (allCostsDatePicker.Date.HasValue)
        {
            allCostsDatePicker.Date = allCostsDatePicker.Date.Value.AddMonths(1);
        }
    }

    private async void MyDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate != null)
        {
            DateTime selectedDate = (DateTime)e.NewDate;

            if (BindingContext is AllCosts allCosts)
            {
                var costs = await _allCostsService.LoadCostsByMonth(selectedDate);
                allCosts.Costs = new ObservableCollection<CostModel>(costs.Adapt<List<CostModel>>().OrderBy(c => c.Date));
                allCosts.Sum = allCosts.Costs.Sum(c => c.Value ?? 0);
            }
        }
    }
}