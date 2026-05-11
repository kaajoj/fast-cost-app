using FastCost.Core.DAL;
using FastCost.Core.Mappings;
using FastCost.Core.Services;
using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace FastCost;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .UseLiveCharts()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddDbContextFactory<AppDbContext>();

        MapsterConfig.RegisterMappings();

        builder.Services.AddSingleton<IAllCostsService, AllCostsService>();
		builder.Services.AddSingleton<ICostRepository, CostRepository>();
		builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();

        builder.Services.AddSingleton<Views.MainPage>();
        builder.Services.AddSingleton<Views.AllCostsPage>();
        builder.Services.AddSingleton<Views.AnalysisPage>();

        builder.Services.AddTransient<Views.CostPage>();
        builder.Services.AddTransient<Views.ChartPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
