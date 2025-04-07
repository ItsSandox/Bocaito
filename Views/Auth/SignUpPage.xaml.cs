using System.Threading.Tasks;
using Supabase;
namespace Bocaito
{
    public partial class SignUpPage : ContentPage
    {

        public SignUpPage()
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
        private void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ForgotPasswordPage());
        }
    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        if (loadingIndicator.IsRunning)
            return;
        
        // Validar campos primero (opcional pero recomendado)
        if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(apellido.Text) ||
            string.IsNullOrWhiteSpace(correo.Text) || string.IsNullOrWhiteSpace(contraseña.Text))
        {
            await Application.Current.MainPage.DisplayAlert("Error", 
                "Todos los campos son obligatorios", "Aceptar");
            return;
        }
        
        loadingIndicator.IsVisible = true;
        loadingIndicator.IsRunning = true;
        signup.IsEnabled = false;
        
        try
        {
            // Registrar usuario en Auth y guardar en tabla usuarios
            var result = await App.SupabaseService.SignUp(
                correo.Text, contraseña.Text, nombre.Text, apellido.Text, telefono.Text);
            
            if (result?.User != null)
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", 
                    "Usuario registrado correctamente", "Aceptar");
                await Shell.Current.GoToAsync("//login");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", 
                    "No se pudo completar el registro del usuario", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", 
                $"No se pudo registrar el usuario: {ex.Message}", "Aceptar");
        }
        finally
        {
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
            signup.IsEnabled = true;
        }
    }
        async void OnLogInClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//login");
        }
    /*private async void OnSignUpClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await App.SupabaseService.CreateUsuario();
            if (result == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el usuario.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                await Shell.Current.GoToAsync("//login");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo registrar el usuario.\nError: {ex.Message}", "Aceptar");
        }
        finally
        {
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
            signup.IsEnabled = true;
        }
    }*/
    }
}