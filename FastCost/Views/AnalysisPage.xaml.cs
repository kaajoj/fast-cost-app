using FastCost.Core.Models;
using FastCost.Core.Services;
using System.Collections.ObjectModel;

namespace FastCost.Views;

public partial class AnalysisPage : ContentPage
{
    private readonly IAllCostsService _allCostsService;
    private readonly IServiceProvider _serviceProvider;
    private int _localDataVersion = 0;
    private DateTime? _lastLoadedDate = null;
    private DateTime? _loadingForDate = null;

    public AnalysisPage(IAllCostsService allCostsService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _allCostsService = allCostsService;
        _serviceProvider = serviceProvider;
        this.BindingContext = new AllCostsGroup();
    }

    public Task PreloadDataAsync()
    {
        return BindingContext is AllCostsGroup allCostsGroup ? LoadData(allCostsGroup) : Task.CompletedTask;
    }

    private async void OnChartClicked(object sender, EventArgs e)
    {
        var chartPage = _serviceProvider.GetService<ChartPage>();
        if (chartPage != null)
            await Navigation.PushModalAsync(new NavigationPage(chartPage));
    }

    private async void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI) return;
        await Shell.Current.GoToAsync("//allCosts");
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs state)
    {
        base.OnNavigatedTo(state);

        if (BindingContext is AllCostsGroup allCostsGroup && NeedsReload(allCostsGroup.SelectedDate))
        {
            await LoadData(allCostsGroup);
        }
    }

    private bool NeedsReload(DateTime date)
    {
        return _localDataVersion != AppState.DataVersion
            || _lastLoadedDate?.Month != date.Month
            || _lastLoadedDate?.Year != date.Year;
    }

    private async Task LoadData(AllCostsGroup allCostsGroup)
    {
        var date = allCostsGroup.SelectedDate;
        if (_loadingForDate == date) return;

        _loadingForDate = date;
        try
        {
            var (groupCosts, total) = await Task.Run(async () =>
            {
                var groups = (await _allCostsService.GetCostsByMonthGroupByCategory(date)).ToList();
                decimal sum = 0;
                foreach (var costGroup in groups)
                {
                    decimal sumGroup = 0;
                    foreach (var cost in costGroup)
                    {
                        sumGroup += cost.Value.GetValueOrDefault();
                    }
                    costGroup.Key.SumValue = sumGroup;
                    sum += sumGroup;
                }
                return (groups, sum);
            });
            if (allCostsGroup.SelectedDate != date) return;

            allCostsGroup.GroupCosts = new ObservableCollection<IGrouping<CategoryModel, CostModel>>(groupCosts);
            allCostsGroup.Sum = total;

            _localDataVersion = AppState.DataVersion;
            _lastLoadedDate = date;
        }
        finally
        {
            if (_loadingForDate == date) _loadingForDate = null;
        }
    }

    private void ScrollToBottom_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is not AllCostsGroup allCostsGroup) return;
        if (allCostsGroup.GroupCosts.Count == 0) return;

        var lastGroupIdx = allCostsGroup.GroupCosts.Count - 1;
        var lastGroup = allCostsGroup.GroupCosts[lastGroupIdx];
        var lastItemIdx = lastGroup.Count() - 1;
        if (lastItemIdx < 0) return;

        CollectionViewGroup.ScrollTo(lastItemIdx, lastGroupIdx, ScrollToPosition.End, animate: true);
    }

    private void OnPreviousMonth(object sender, EventArgs e)
    {
        if (AnalysisPageDatePicker.Date.HasValue)
        {
            AnalysisPageDatePicker.Date = AnalysisPageDatePicker.Date.Value.AddMonths(-1);
        }
    }

    private void OnNextMonth(object sender, EventArgs e)
    {
        if (AnalysisPageDatePicker.Date.HasValue)
        {
            AnalysisPageDatePicker.Date = AnalysisPageDatePicker.Date.Value.AddMonths(1);
        }
    }

    private async void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate != null && BindingContext is AllCostsGroup allCostsGroup)
        {
            allCostsGroup.SelectedDate = (DateTime)e.NewDate;
            await LoadData(allCostsGroup);
        }
    }
}
