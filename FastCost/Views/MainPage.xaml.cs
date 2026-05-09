using FastCost.Core;
using FastCost.Core.Services;
using System.Globalization;

namespace FastCost.Views;

public partial class MainPage : ContentPage
{
    private readonly IAllCostsService _allCostsService;
    private readonly IServiceProvider _serviceProvider;
    private bool _isNavigating = false;
    private bool _isPreloaded = false;

    public MainPage(IAllCostsService allCostsService, IServiceProvider serviceProvider)
	{
		InitializeComponent();
        _allCostsService = allCostsService;
        _serviceProvider = serviceProvider;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await App.DbInitTask;

        var currentDate = DateTime.UtcNow.Date;
        var totalSum = await _allCostsService.GetSum(currentDate);

        var currentMonthName = DateTime.UtcNow.Date.ToString("MMMM");
        SummaryText.Text = $"Expenses in {currentMonthName}: {totalSum}";

        if (!_isPreloaded)
        {
            _isPreloaded = true;
            _ = PreloadOtherPagesAsync();
        }
    }

    private async Task PreloadOtherPagesAsync()
    {
        try
        {
            var allCostsPage = _serviceProvider.GetService<AllCostsPage>();
            if (allCostsPage != null) await allCostsPage.PreloadDataAsync();

            var analysisPage = _serviceProvider.GetService<AnalysisPage>();
            if (analysisPage != null) await analysisPage.PreloadDataAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Preload error: {ex.Message}");
        }
    }

    private void OnCostChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                var enteredCost = CostParser.Parse(e.NewTextValue);

                if (enteredCost != 0)
                {
                    CostBtn.IsEnabled = true;
                    CostBtn.BackgroundColor = Color.Parse("Lime");
                    CostBtn.Handler?.UpdateValue("Background");
                }
            }
            else
            {
                CostText.Text = string.Empty;
                CostBtn.IsEnabled = false;
                CostBtn.BackgroundColor = Color.Parse("LightGray");
                CostBtn.Handler?.UpdateValue("Background");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async void OnSwipedLeft(object sender, SwipedEventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI) return;
        await Shell.Current.GoToAsync("//allCosts");
    }

    private async void OnCostEntered(object sender, EventArgs e)
	{
        if (_isNavigating) return;
        _isNavigating = true;

        try
        {
            SemanticScreenReader.Announce(CostText.Text);

            var enteredCost = CostParser.Parse(CostText.Text);
            CostText.Text = string.Empty;
            await CostText.HideSoftInputAsync(CancellationToken.None);
            
            await Shell.Current.GoToAsync($"{nameof(CostPage)}?{nameof(CostPage.CostValue)}={enteredCost.ToString(CultureInfo.InvariantCulture)}", true);
        }
        catch (ArgumentNullException)
        {
            await DisplayAlertAsync("Unable to add cost", "Cost value was not valid.", "OK");
        }
        catch (Exception)
        {
            await DisplayAlertAsync("Unable to add cost", "Cost adding failed.", "OK");
        }
        finally
        {
            _isNavigating = false;
        }
    }
}
