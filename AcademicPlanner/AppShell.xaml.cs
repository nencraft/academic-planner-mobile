using AcademicPlanner.Views;

namespace AcademicPlanner;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(TermEditPage), typeof(TermEditPage));
        Routing.RegisterRoute(nameof(TermOverviewPage), typeof(TermOverviewPage));
    }
}
