using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Microsoft.Maui.Controls;

namespace Bocaito;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Cambiar el color de la barra de estado
            Window.SetStatusBarColor(Android.Graphics.Color.White);

            // Forzar los íconos de la barra de estado a ser oscuros
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                // Establece los íconos de la barra de estado como oscuros (dark icons)
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            }

            // Si deseas mantener otros comportamientos, puedes agregar código adicional aquí
        }
}
