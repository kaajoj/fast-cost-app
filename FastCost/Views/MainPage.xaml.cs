using FastCost.DAL;
using System.Globalization;

namespace FastCost.Views;

public partial class MainPage : ContentPage
{
    private readonly ICostRepository _costRepository;

    public MainPage(ICostRepository costRepository)
	{
		InitializeComponent();
        _costRepository = costRepository;
    }
    
    protected override async void OnAppearing()
    {
        var currentDate = DateTime.UtcNow.Date;
        var costs = await _costRepository.GetCostsByMonth(currentDate);

        var costsInCurrentMonth = costs.Sum(c => c.Value);

        var currentMonthName = DateTime.UtcNow.Date.ToString("MMMM");
        SummaryText.Text = $"Expenses in {currentMonthName}: {costsInCurrentMonth}";
    }

    private void OnCostChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                var indexOfDot = e.NewTextValue.IndexOf('.');
                var indexOfComma = e.NewTextValue.IndexOf(',');
                var numberFormat = new NumberFormatInfo
                {
                    NumberDecimalSeparator = indexOfComma > indexOfDot ? "," : ".",
                    NumberGroupSeparator = indexOfComma > indexOfDot ? "." : ","
                };

                decimal.TryParse(e.NewTextValue, NumberStyles.Number, numberFormat, out var enteredCost);
                
                if (enteredCost != 0)
                {
                    CostBtn.IsEnabled = true;
                    CostBtn.BackgroundColor = Color.Parse("Lime");
                    CostBtn.Handler.UpdateValue("Background");
                }
            }
            else
            {
                CostText.Text = string.Empty;
                CostBtn.IsEnabled = false;
                CostBtn.BackgroundColor = Color.Parse("LightGray");
                CostBtn.Handler.UpdateValue("Background");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async void OnCostEntered(object sender, EventArgs e)
	{
        SemanticScreenReader.Announce(CostText.Text);

        {
            try
            {
                var indexOfDot = CostText.Text.IndexOf('.');
                var indexOfComma = CostText.Text.IndexOf(',');
                var numberFormat = new NumberFormatInfo
                {
                    NumberDecimalSeparator = indexOfComma > indexOfDot ? "," : ".",
                    NumberGroupSeparator = indexOfComma > indexOfDot ? "." : ","
                };

                decimal.TryParse(CostText.Text, NumberStyles.Number, numberFormat, out var enteredCost);
                CostText.Text = string.Empty;
                await Shell.Current.GoToAsync($"{nameof(CostPage)}?{nameof(CostPage.CostValue)}={enteredCost}", true);
            }
            catch (ArgumentNullException)
            {
                await DisplayAlert("Unable to add cost", "Cost value was not valid.", "OK");
            }
            catch (Exception)
            {
                await DisplayAlert("Unable to add cost", "Cost adding failed.", "OK");
            }
        }
    }
}

