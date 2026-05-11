using System.Collections.ObjectModel;
using FastCost.Core.Models;
using FastCost.Core.Services;
using Mapster;

namespace FastCost.Views;

public partial class AllCostsPage : ContentPage
{
    private readonly IAllCostsService _allCostsService;
    private bool _isNavigating = false;
    private int _localDataVersion = 0;
    private DateTime? _lastLoadedDate = null;
    private DateTime? _loadingForDate = null;

    public AllCostsPage(IAllCostsService allCostsService)
	{
        InitializeComponent();
        _allCostsService = allCostsService;
        this.BindingContext = new AllCosts();
    }

    public Task PreloadDataAsync()
    {
        return BindingContext is AllCosts allCosts ? LoadData(allCosts) : Task.CompletedTask;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs state)
    {
        base.OnNavigatedTo(state);

        if (BindingContext is AllCosts allCosts && NeedsReload(allCosts.SelectedDate))
        {
            await LoadData(allCosts);
        }
    }

    private bool NeedsReload(DateTime date)
    {
        return _localDataVersion != AppState.DataVersion
            || _lastLoadedDate?.Month != date.Month
            || _lastLoadedDate?.Year != date.Year;
    }

    private async Task LoadData(AllCosts allCosts)
    {
        var date = allCosts.SelectedDate;
        if (_loadingForDate == date) return;

        _loadingForDate = date;
        try
        {
            var costs = await _allCostsService.LoadCostsByMonth(date);
            if (allCosts.SelectedDate != date) return;

            var (mapped, sum) = await Task.Run(() =>
            {
                var list = costs.Adapt<List<CostModel>>();
                list.Sort((a, b) => a.Date.CompareTo(b.Date));
                var s = list.Sum(c => c.Value ?? 0);
                return (list, s);
            });
            if (allCosts.SelectedDate != date) return;

            allCosts.Costs = new ObservableCollection<CostModel>(mapped);
            allCosts.Sum = sum;

            _localDataVersion = AppState.DataVersion;
            _lastLoadedDate = date;
        }
        finally
        {
            if (_loadingForDate == date) _loadingForDate = null;
        }
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

    private void ScrollToBottom_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is AllCosts allCosts && allCosts.Costs.Count > 0)
        {
            costsCollection.ScrollTo(allCosts.Costs.Count - 1, position: ScrollToPosition.End, animate: true);
        }
    }

    private async void OnCostTapped(object sender, EventArgs e)
    {
        if (_isNavigating) return;

        var grid = (Grid)sender;
        if (grid.BindingContext is not CostModel cost) return;

        _isNavigating = true;
        try
        {
            VisualStateManager.GoToState(grid, "Selected");
            await Task.Delay(200);
            await Shell.Current.GoToAsync($"{nameof(CostPage)}?{nameof(CostPage.ItemId)}={cost.Id}");
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
        if (e.NewDate != null && BindingContext is AllCosts allCosts)
        {
            await LoadData(allCosts);
        }
    }
}
