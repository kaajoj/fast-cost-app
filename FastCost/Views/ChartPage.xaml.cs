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
    private bool _loaded;

    public ChartPage(IAllCostsService allCostsService)
    {
        InitializeComponent();
        _allCostsService = allCostsService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_loaded) return;
        _loaded = true;
        _ = LoadChartAsync();
    }

    private async Task LoadChartAsync()
    {
        var data = await _allCostsService.GetMonthlyTotals(DefaultMonthsToDisplay);

        var values = data.Select(d => (double)d.Total).ToArray();
        var labels = data.Select(d => d.Month).ToArray();

        var series = new ISeries[]
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

        var xAxes = new[]
        {
            new Axis
            {
                Labels = labels,
                LabelsRotation = 0,
                MinStep = 1,
                ForceStepToMin = true
            }
        };

        var yAxes = new[]
        {
            new Axis
            {
                MinLimit = 0
            }
        };

        chart.XAxes = xAxes;
        chart.YAxes = yAxes;
        chart.Series = series;
    }

    private async void OnClose(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
