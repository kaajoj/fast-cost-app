using FastCost.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace FastCost;

public partial class App : Application
{
    private static readonly TaskCompletionSource _dbInitTcs = new();
    public static Task DbInitTask => _dbInitTcs.Task;

    public App(IServiceProvider serviceProvider)
	{
        InitializeComponent();
        _ = Task.Run(() => InitializeDatabase(serviceProvider));
    }

    private static void InitializeDatabase(IServiceProvider serviceProvider)
    {
        try
        {
            var factory = serviceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using var dbContext = factory.CreateDbContext();
            dbContext.Database.Migrate();
            _dbInitTcs.SetResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating database: {ex.Message}");
            _dbInitTcs.SetException(ex);
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
