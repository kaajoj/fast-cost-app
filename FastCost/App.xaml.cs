using FastCost.Core.DAL;
using FastCost.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace FastCost;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider, IAllCostsService allCostsService)
	{
        InitializeComponent();
        InitializeDatabase(serviceProvider);
    }

    private void InitializeDatabase(IServiceProvider serviceProvider)
    {
        try
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating database: {ex.Message}");
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

#if WINDOWS
        window.Width = 900;
        window.Height = 700;
#endif

        return window;
    }
}
