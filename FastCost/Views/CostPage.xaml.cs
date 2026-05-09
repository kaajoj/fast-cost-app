using FastCost.Core;
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
    private readonly ICategoryRepository _categoryRepository;
    private Task? _loadCostTask;
    private Task? _preloadTask;

    private static List<CategoryItem>? _cachedCategories;

    private static readonly Dictionary<string, string> _emojiMap = new()
    {
        { "food",      "🍕" },
        { "apartment", "🏠" },
        { "shopping",  "🛒" },
        { "transport", "🚗" },
        { "trip",      "🏖" },
        { "bank",      "🏦" },
        { "company",   "💼" },
        { "other",     "💸" }
    };

    public CostPage(ICostRepository costRepository, ICategoryRepository categoryRepository)
    {
        InitializeComponent();
        BindingContext = new CostModel();
        _costRepository = costRepository;
        _categoryRepository = categoryRepository;

        // Immediately use cache if available to avoid "pop-in" effect
        if (_cachedCategories != null)
        {
            categoriesCollection.ItemsSource = _cachedCategories;
        }
        
        _preloadTask = PreloadCategoriesAsync();
    }

    private async Task PreloadCategoriesAsync()
    {
        if (_cachedCategories != null) return;
        var categories = await _categoryRepository.GetCategories();
        _cachedCategories = categories
            .Select(c => new CategoryItem(c.Id, c.Name, _emojiMap.GetValueOrDefault(c.Name.ToLower(), "❓")))
            .ToList();
        
        // If it was loaded for the first time, set it now
        if (categoriesCollection.ItemsSource == null)
        {
            categoriesCollection.ItemsSource = _cachedCategories;
        }
    }

    public string ItemId
    {
        set { _loadCostTask = LoadCost(value); }
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
                    costModel.Value = decimal.Parse(costValue, CultureInfo.InvariantCulture);
                    costModel.Date = DateTime.Now;
                }
            }
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs state)
    {
        base.OnNavigatedTo(state);
        _ = InitializePageAsync();
    }

    private async Task InitializePageAsync()
    {
        await Task.WhenAll(
            _loadCostTask ?? Task.CompletedTask,
            _preloadTask ?? Task.CompletedTask);

        // Ensure ItemsSource is set (in case it wasn't set in constructor or preload)
        if (categoriesCollection.ItemsSource == null)
        {
            categoriesCollection.ItemsSource = _cachedCategories;
        }

        if (BindingContext is CostModel costModel)
        {
            if (costModel.CategoryId != null)
            {
                categoriesCollection.SelectedItem = _cachedCategories?.FirstOrDefault(c => c.Id == costModel.CategoryId);
            }

            // Focus value editor only when adding a new cost (value is null or zero)
            if (costModel.Value == null || costModel.Value == 0)
            {
                CostValueEditor.Focus();
            }
        }
    }

    private void OnCostValueCompleted(object sender, EventArgs e)
    {
        DescriptionEditor.Focus();
    }

    private void OnDescriptionCompleted(object sender, EventArgs e)
    {
    }

    private void OnCategorySelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is CategoryItem selected)
        {
            ((CostModel)BindingContext).CategoryId = selected.Id;
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var enteredCost = CostParser.Parse(CostValueEditor.Text);

            if (BindingContext is CostModel costModel)
            {
                costModel.Value = enteredCost;
                var cost = costModel.Adapt<Cost>();
                await _costRepository.SaveCostAsync(cost);
            }

            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync("//allCosts");
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

    private record CategoryItem(int Id, string Name, string Emoji);
}
