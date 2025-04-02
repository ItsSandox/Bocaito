using System.Threading.Tasks;
using Supabase;
namespace Bocaito
{
    public partial class LogInPage : ContentPage
    {

        public LogInPage()
        {
            InitializeComponent();
        }
        private void OnButtonEyeClicked(object sender, EventArgs e)
        {
            if (contraseña.IsPassword == false){
                contraseña.IsPassword = true;
                btnEye.Source = "eye.svg";
            }
            else
            {
                contraseña.IsPassword = false;
                btnEye.Source = "eye_closed.svg";
            }

        }
        async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgotPasswordPage());
        }
        async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//signup");
        }
        private async void OnLogInClicked(object sender, EventArgs e)
        {
            // Si ya está procesando, ignorar
            if (loadingIndicator.IsRunning)
                return;
                
            bool camposValidos = true;

            if (string.IsNullOrWhiteSpace(correo.Text))
            {
                correoFrame.BorderColor = Colors.Red;
                camposValidos = false;
            }
            else
            {
                correoFrame.BorderColor = Colors.Transparent;
            }
            if (string.IsNullOrWhiteSpace(contraseña.Text))
            {
                contraseñaFrame.BorderColor = Colors.Red;
                camposValidos = false;
            }
            else
            {
                contraseñaFrame.BorderColor = Colors.Transparent;
            }
            if (!camposValidos){
                return;
            }
            
            // Activar indicador de carga
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;
            signin.IsEnabled = false; // Desactivar botón mientras se procesa
            
            try
            {
                var result = await App.SupabaseService.SignIn(correo.Text, contraseña.Text);
                if (result?.User != null)
                {
                    Console.WriteLine($"Bienvenido: {result.User.Email}");
                    await Application.Current.MainPage.DisplayAlert(
                        "Lo hiciste wey",
                        "Waos ni tu te lo cree.",
                        "Aceptar"
                    );
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error de inicio de sesión",
                        "No se pudo iniciar sesión. Por favor, verifica tu correo y contraseña.",
                        "Aceptar"
                    );
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error de inicio de sesión",
                    $"Error: {ex.Message}",
                    "Aceptar"
                );
            }
            finally
            {
                // Desactivar indicador de carga
                loadingIndicator.IsVisible = false;
                loadingIndicator.IsRunning = false;
                signin.IsEnabled = true; // Reactivar botón
            }
        }
    }
}