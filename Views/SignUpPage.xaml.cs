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
            try{
                var result = await App.SupabaseService.SignUp(correo.Text, contraseña.Text);
                if(result?.User != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Exito", 
                    "Usuario registrado correctamente", 
                    "OK");
                    await Shell.Current.GoToAsync("//login");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el usuario", "OK");
                }
            }
            catch (Exception ex){
                await Application.Current.MainPage.DisplayAlert("Error", 
                "No se pudo registrar el usuario",
                $"Error: {ex.Message}",
                 "Aceptar");
            }
            finally{
                // Desactivar indicador de carga
                loadingIndicator.IsVisible = false;
                loadingIndicator.IsRunning = false;
                signup.IsEnabled = true;
            }
        }
        async void OnLogInClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}