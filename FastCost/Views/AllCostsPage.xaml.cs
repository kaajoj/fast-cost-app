using FastCost.Core.Models;
using FastCost.Core.Services;
using Mapster;

namespace FastCost.Views;

public partial class AllCostsPage : ContentPage
{
    private readonly IAllCostsService _allCostsService;

    public AllCostsPage(IAllCostsService allCostsService)
	{
        InitializeComponent();
        _allCostsService = allCostsService;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs state)
    {
        base.OnNavigatedTo(state);
        
        DateTime currentDate = DateTime.Now;

        if (BindingContext is AllCosts allCosts)
        {
            currentDate = allCosts.SelectedDate;

            allCosts.Costs.Clear();
            var costs = await _allCostsService.LoadCostsByMonth(currentDate);
            foreach (CostModel cost in costs.Adapt<List<CostModel>>().OrderBy(cost => cost.Date))
            {
                allCosts.Costs.Add(cost);
            }

            allCosts.Sum = await _allCostsService.GetSum(currentDate);
        }
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CostPage));
    }

    private async Task CostsCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count != 0)
        {
            // Get the cost model
            var cost = (CostModel)e.CurrentSelection[0];

            await Shell.Current.GoToAsync($"{nameof(CostPage)}?{nameof(CostPage.ItemId)}={cost.Id}");

            // Unselect the UI
            costsCollection.SelectedItem = null;
        }
    }

    private async Task MyDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate != null)
        {
            DateTime selectedDate = (DateTime)e.NewDate;

            if (BindingContext is AllCosts allCosts)
            {
                allCosts.Costs.Clear();
                var costs = await _allCostsService.LoadCostsByMonth(selectedDate);
                foreach (CostModel cost in costs.Adapt<List<CostModel>>().OrderBy(cost => cost.Date))
                {
                    allCosts.Costs.Add(cost);
                }

                allCosts.Sum = (decimal)await _allCostsService.GetSum(selectedDate);
            }
        }
    }
}