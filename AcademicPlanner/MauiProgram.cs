using AcademicPlanner.Data;
using AcademicPlanner.Views;
using Microsoft.Extensions.Logging;

namespace AcademicPlanner;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<AcademicPlannerDatabase>();
        builder.Services.AddTransient<TermsPage>();
        builder.Services.AddTransient<TermEditPage>();
        return builder.Build();
	}
}
