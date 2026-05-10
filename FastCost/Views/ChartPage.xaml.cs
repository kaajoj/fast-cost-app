using FastCost.Core.Services;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace FastCost.Views;

public partial class ChartPage : ContentPage
{
    private const int DefaultMonthsToDisplay = 4;
    private readonly IAllCostsService _allCostsService;
    private string[] _labels = Array.Empty<string>();

    public ChartPage(IAllCostsService allCostsService)
    {
        InitializeComponent();
        _allCostsService = allCostsService;
        chart.DataPointerDown += OnDataPointerDown;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = LoadChartAsync();
    }

    private async Task LoadChartAsync()
    {
        var data = await _allCostsService.GetMonthlyTotals(DefaultMonthsToDisplay);

        var values = data.Select(d => (double)d.Total).ToArray();
        _labels = data.Select(d => d.Month).ToArray();

        var series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = values,
                Name = "Expenses",
                Fill = new SolidColorPaint(SKColors.CadetBlue)
            }
        };

        var xAxes = new[]
        {
            new Axis
            {
                Labels = _labels,
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

    private void OnDataPointerDown(IChartView sender, IEnumerable<ChartPoint> points)
    {
        var point = points.FirstOrDefault();
        if (point == null) return;

        var idx = (int)point.Coordinate.SecondaryValue;
        var month = idx >= 0 && idx < _labels.Length ? _labels[idx] : string.Empty;
        SelectedValueLabel.Text = $"{month}: {point.Coordinate.PrimaryValue:0.##}";
    }

    private async void OnClose(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
