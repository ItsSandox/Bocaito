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
        
        // Inicializar la conexión con Supabase
        InitializeSupabaseAsync();

        MainPage = new AppShell();
    }

    private async void InitializeSupabaseAsync()
    {
        try
        {
            bool initialized = await SupabaseService.InitializeAsync();
            
            if (!initialized)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error de conexión",
                    "No se pudo conectar a la base de datos. Por favor, verifica tu conexión a internet.",
                    "Aceptar"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar Supabase: {ex.Message}");

            await Application.Current.MainPage.DisplayAlert(
                "Error de conexión",
                "No se pudo conectar a la base de datos. Por favor, verifica tu conexión a internet.",
                "Aceptar"
            );
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(MainPage);
    }
}