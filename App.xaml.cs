using System.IO.Pipelines;
using Bocaito.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bocaito;

public partial class App : Application
{
    // Propiedad para acceder al servicio de Supabase desde cualquier parte de la app
    public static SupabaseService SupabaseService { get; private set; }

    public App(SupabaseService supabaseService)
    {
        InitializeComponent();

        App.Current.UserAppTheme = AppTheme.Light;
        
        // Guardar el servicio como propiedad estática
        SupabaseService = supabaseService;
        
    }

    protected override Window CreateWindow(IActivationState? activationState)
	{
		var appShell = new AppShell();
        return new Window(appShell);
	}
    

    protected override async void OnStart()
    {
        base.OnStart();
        
        // Primero inicializar Supabase
        bool initialized = false;
        try
        {
            initialized = await SupabaseService.InitializeAsync();
            
            if (!initialized)
            {
                
                await Application.Current.MainPage.DisplayAlert(
                    "Error de conexión",
                    "No se pudo conectar a la base de datos. Por favor, verifica tu conexión a internet.",
                    "Aceptar"
                );
                return;
            }
#if ANDROID
            // Solicitar permisos de notificaciones en Android 13+ (API 33+)
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                // Usando la solicitud de permisos directamente con Android API
                var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                if (activity != null)
                {
                    var context = activity.ApplicationContext;
                    var notificationManager = context.GetSystemService(Android.Content.Context.NotificationService) as Android.App.NotificationManager;
                    
                    if (notificationManager != null && !notificationManager.AreNotificationsEnabled())
                    {
                        var intent = new Android.Content.Intent(Android.Provider.Settings.ActionAppNotificationSettings);
                        intent.PutExtra(Android.Provider.Settings.ExtraAppPackage, context.PackageName);
                        activity.StartActivity(intent);
                    }
                }
            }
#endif
            }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar Supabase: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert(
                "Error de conexión",
                "No se pudo conectar a la base de datos. Por favor, verifica tu conexión a internet.",
                "Aceptar"
            );
            return;
        }
        
        // Ahora intentar restaurar la sesión y navegar
        if (initialized){
            try{
                var result = await SupabaseService.RestoreSession();
            
                if (result != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                    "Exito",
                    $"Resultado: {result}",
                    "Aceptar"
                    );
                    await Shell.Current.GoToAsync("//menu", true);
                }
                else
                {
                    await Shell.Current.GoToAsync("//login", true);
                }
            }catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Hubo un error: {ex}",
                    "Aceptar"
                );
                return;
            }
        }
    }
    
}