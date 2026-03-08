using FastCost.Core.DAL;
using FastCost.Core.DAL.Entities;
using FastCost.Core.Models;
using Mapster;
using System.Globalization;

namespace FastCost.Views;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
[QueryProperty(nameof(CostValue), nameof(CostValue))]
public partial class CostPage : ContentPage
{
    private readonly ICostRepository _costRepository;
    private string? _selectedCategory = null;
    private Label? _previousSelectedLabel = null;
    private readonly Dictionary<int, string> _categoryDict;

    public CostPage(ICostRepository costRepository) 
    {
        InitializeComponent();
        BindingContext = new CostModel();
        // this.categoriesGrid.SelectionChanged += OnCategorySelected;

        _costRepository = costRepository;

        _categoryDict = new Dictionary<int, string>();
        int col = 0;
        int row = 0;
        for (int i = 1; i <= 8; i++)
        {
            _categoryDict.Add(i, $"LblCatC{col}R{row}");
            if (i == 4)
            {
                col = 0;
                row = 2;
            }
            else
            {
                col++;
            }            
        }
    }

    public string ItemId
    {
        set { _ = LoadCost(value); }
    }

    private async Task LoadCost(string id)
    {
        int.TryParse(id, out int result);
        if (result != 0)
        {
            var cost = await _costRepository.GetCostAsync(result);
            var costModel = cost.Adapt<CostModel>();
            BindingContext = costModel;
        }
    }

    private string costValue = string.Empty;
    public string CostValue
    {
        get => costValue;
        set
        {
            costValue = value;
            if (!string.IsNullOrEmpty(costValue))
            {
                if (BindingContext is CostModel costModel)
                {
                    costModel.Value = decimal.Parse(costValue);
                    costModel.Date = DateTime.UtcNow;
                }
            }
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs state)
    {
        base.OnNavigatedTo(state);

        if (((CostModel)BindingContext).Value != null && ((CostModel)BindingContext).Value != 0)
        {
            CostValueEditor.Text = ((CostModel)BindingContext).Value.ToString();


            int? categoryId = ((CostModel)BindingContext).CategoryId;
            if (categoryId != null)
            {
                _categoryDict.TryGetValue((int)categoryId, out var labelName);

                if (!string.IsNullOrEmpty(labelName))
                {
                    Label categoryLabel = (Label)this.FindByName(labelName);
                    categoryLabel.BackgroundColor = Color.Parse("CadetBlue");
                    categoryLabel?.Handler?.UpdateValue("Background");

                    _previousSelectedLabel = categoryLabel;
                }
            }
        }

    }



    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var indexOfDot = CostValueEditor.Text.IndexOf('.');
            var indexOfComma = CostValueEditor.Text.IndexOf(',');
            var numberFormat = new NumberFormatInfo
            {
                NumberDecimalSeparator = indexOfComma > indexOfDot ? "," : ".",
                NumberGroupSeparator = indexOfComma > indexOfDot ? "." : ","
            };

            decimal.TryParse(CostValueEditor.Text?.Trim(), NumberStyles.Number, numberFormat, out var enteredCost);

            if (BindingContext is CostModel costModel)
            {
                costModel.Value = enteredCost;

                var cost = costModel.Adapt<Cost>();
                await _costRepository.SaveCostAsync(cost);
            }

            await Shell.Current.GoToAsync($"//allCosts", true);
        }
        catch (ArgumentNullException)
        {
            await DisplayAlertAsync("Unable to add cost", "Cost value was not valid.", "OK");
        }
        catch (Exception)
        {
            await DisplayAlertAsync("Unable to add cost", "Cost adding failed.", "OK");
        }
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is CostModel costModel)
        {
            var cost = costModel.Adapt<Cost>();

            if (await DisplayAlertAsync("Delete cost", "Do you want to remove the cost with the value: " + cost.Value + "?", "Yes", "No"))
            {
                await _costRepository.DeleteCostAsync(cost);
                await Shell.Current.GoToAsync("..");
            }
        }
    }

    private void OnCategorySelected(object sender, TappedEventArgs args)
    {
        if (_previousSelectedLabel != null)
        {
            _previousSelectedLabel.BackgroundColor = Application.Current?.PlatformAppTheme == AppTheme.Dark ? Colors.Black : Colors.White;
            _previousSelectedLabel?.Handler?.UpdateValue("Background");
        }

        Label selectedLabel = (Label)sender;
        _previousSelectedLabel = selectedLabel;

        selectedLabel.BackgroundColor = Color.Parse("CadetBlue");
        selectedLabel?.Handler?.UpdateValue("Background");

        _selectedCategory = args.Parameter?.ToString();

        if (_selectedCategory != null)
        {
            ((CostModel)BindingContext).CategoryId = int.Parse(_selectedCategory);
        }

        // var id = selectedLabel.Id;
        // var text = selectedLabel.Text;
    }
}