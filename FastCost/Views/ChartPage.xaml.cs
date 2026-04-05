using FastCost.Core.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace FastCost.Views;

public partial class ChartPage : ContentPage
{
    private const int DefaultMonthsToDisplay = 4;
    private readonly IAllCostsService _allCostsService;

    public ChartPage(IAllCostsService allCostsService)
    {
        InitializeComponent();
        _allCostsService = allCostsService;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs state)
    {
        base.OnNavigatedTo(state);
        _ = LoadChartAsync();
    }

    private async Task LoadChartAsync()
    {
        var data = await _allCostsService.GetMonthlyTotals(DefaultMonthsToDisplay);

        var values = data.Select(d => (double)d.Total).ToArray();
        var labels = data.Select(d => d.Month).ToArray();

        chart.Series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = values,
                Name = "Expenses",
                Fill = new SolidColorPaint(SKColors.CadetBlue)
            },
            new LineSeries<double>
            {
                Values = values,
                Name = "Trend",
                Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 3 },
                Fill = null,
                GeometrySize = 8,
                GeometryStroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 2 }
            }
        };

        chart.XAxes = new[]
        {
            new Axis
            {
                Labels = labels,
                LabelsRotation = 0,
                MinStep = 1,
                ForceStepToMin = true
            }
        };

        chart.YAxes = new[]
        {
            new Axis
            {
                MinLimit = 0
            }
        };
    }

    private async void OnClose(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
