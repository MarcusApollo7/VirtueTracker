using Microsoft.Extensions.Logging;
using VirtueTracker.Services;
using VirtueTracker.Interfaces;

namespace VirtueTracker;

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
			});
		builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
		builder.Services.AddSingleton<IQuoteRepository, QuoteRepository>();
		builder.Services.AddSingleton<IVirtueRepository, VirtueRepository>();
		builder.Services.AddSingleton<IMeaningRepository, MeaningRepository>();
		builder.Services.AddSingleton<ILanguageService, LanguageService>();
		builder.Services.AddSingleton<IShuffleQuoteService, ShuffleQuoteService>();
		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
