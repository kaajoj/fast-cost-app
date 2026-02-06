using FastCost.DAL;
using FastCost.Mappings;
using FastCost.Services;
using Microsoft.Extensions.Logging;

namespace FastCost;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
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

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
