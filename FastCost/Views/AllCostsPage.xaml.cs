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
        
        DateTime currentDate = DateTime.Now;

        if (BindingContext is AllCosts allCosts)
        {
            currentDate = allCosts.SelectedDate;

            var costs = await _allCostsService.LoadCostsByMonth(currentDate);
            allCosts.Costs = new ObservableCollection<CostModel>(costs.Adapt<List<CostModel>>().OrderBy(c => c.Date));
            allCosts.Sum = await _allCostsService.GetSum(currentDate);
            costsCollection.SelectionMode = SelectionMode.Single;
            _isNavigating = false;
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

    private async void CostsCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isNavigating || e.CurrentSelection.Count == 0) return;

        _isNavigating = true;
        var cost = (CostModel)e.CurrentSelection[0];
        costsCollection.SelectionMode = SelectionMode.None;
        await Shell.Current.GoToAsync($"{nameof(CostPage)}?{nameof(CostPage.ItemId)}={cost.Id}");
    }

    private async Task MyDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate != null)
        {
            DateTime selectedDate = (DateTime)e.NewDate;

            if (BindingContext is AllCosts allCosts)
            {
                var costs = await _allCostsService.LoadCostsByMonth(selectedDate);
                allCosts.Costs = new ObservableCollection<CostModel>(costs.Adapt<List<CostModel>>().OrderBy(c => c.Date));

                allCosts.Sum = (decimal)await _allCostsService.GetSum(selectedDate);
            }
        }
    }
}