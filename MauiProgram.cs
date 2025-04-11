using Microsoft.Extensions.Logging;
using Bocaito.Services;
using CommunityToolkit.Maui;

namespace Bocaito;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("SF-Pro-Display-Regular.otf", "SFProRegular");
                fonts.AddFont("SF-Pro-Display-Bold.otf", "SFProBold");
                fonts.AddFont("SF-Pro-Display-Light.otf", "SFProLight");
                fonts.AddFont("SF-Pro-Display-Medium.otf", "SFProMedium");
                fonts.AddFont("SF-Pro-Display-Semibold.otf", "SFProSemibold");
                fonts.AddFont("SF-Pro-Display-Black.otf", "SFProBlack");
                fonts.AddFont("SF-Pro-Display-Heavy.otf", "SFProHeavy");
				fonts.AddFont("Montserrat-Thin.ttf", "MontserratThin");
				fonts.AddFont("RammettoOne-Regular.ttf", "RammettoOneRegular");
				fonts.AddFont("Montserrat-Light.ttf", "MontserratLight");
			});
		builder.Services.AddSingleton<SupabaseService>();
		builder.Services.AddTransient<LogInPage>();
		builder.Services.AddTransient<SignUpPage>();
		builder.Services.AddTransient<ForgotPassword2Page>();
		builder.Services.AddTransient<ForgotPasswordPage>();
		builder.Services.AddTransient<MenuPage>();
		builder.Services.AddTransient<CarritoPage>();
		builder.Services.AddTransient<PedidosPage>();
		builder.Services.AddTransient<ReservasPage>();
		builder.Services.AddTransient<CrearReservaPage>();
		builder.Services.AddTransient<CuentaPage>();
		builder.Services.AddTransient<DireccionesPage>();
		builder.Services.AddTransient<NuevaDireccion>();
		


#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

}
