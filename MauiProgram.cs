using Microsoft.Extensions.Logging;

namespace Bocaito;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("SF-Pro-Display-Regular.otf", "SFProRegular");
                fonts.AddFont("SF-Pro-Display-Bold.otf", "SFProBold");
                fonts.AddFont("SF-Pro-Display-Light.otf", "SFProLight");
                fonts.AddFont("SF-Pro-Display-Medium.otf", "SFProMedium");
                fonts.AddFont("SF-Pro-Display-Semibold.otf", "SFProSemibold");
                fonts.AddFont("SF-Pro-Display-Black.otf", "SFProBlack");
                fonts.AddFont("SF-Pro-Display-Heavy.otf", "SFProHeavy");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

}
