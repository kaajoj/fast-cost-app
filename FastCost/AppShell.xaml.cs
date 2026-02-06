namespace FastCost;

public partial class AppShell : Shell
{
    public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(Views.CostPage), typeof(Views.CostPage));

        BindingContext = this;
    }
}
