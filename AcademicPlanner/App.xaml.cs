using AcademicPlanner.Services;
using AcademicPlanner.Views;

namespace AcademicPlanner;

public partial class App : Application
{
    private readonly AuthenticationService _authService;

    public App(AuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Page startupPage = new ContentPage();

        var window = new Window(startupPage);

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool hasAccount = await _authService.HasAccountAsync();

            window.Page = hasAccount
                ? new LoginPage(_authService)
                : new SetupAccountPage(_authService);
        });

        return window;
    }
}