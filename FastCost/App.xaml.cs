using FastCost.DAL;
using FastCost.Services;
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

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new AppShell());
    }
}
