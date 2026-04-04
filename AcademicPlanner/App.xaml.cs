using AcademicPlanner.Services;

namespace AcademicPlanner;

public partial class App : Application
{
    private readonly SeedDataService _seedDataService;

    public App(SeedDataService seedDataService)
    {
        InitializeComponent();
        _seedDataService = seedDataService;

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _seedDataService.SeedAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}