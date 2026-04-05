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

        builder.Services.AddDbContext<AppDbContext>();

        MapsterConfig.RegisterMappings();

        builder.Services.AddScoped<IAllCostsService, AllCostsService>();
		builder.Services.AddScoped<ICostRepository, CostRepository>();
		builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
		builder.Services.AddTransient<Views.ChartPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
