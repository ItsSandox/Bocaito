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
                
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;
            signup.IsEnabled = false;
            try{
                var result = await App.SupabaseService.SignUp(correo.Text, contraseña.Text);
                if(result?.User != null)
                {
                    try{
                        var id = result.User.Id;
                        var result2 = await App.SupabaseService.CreateUsuario(nombre.Text, apellido.Text, telefono.Text,id);
                        if (result2 == null)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", 
                            $"No se pudo registrar el usuario tabla", 
                            "OK");
                        }
                        else{
                            await Application.Current.MainPage.DisplayAlert("Exito", 
                            "Usuario tabla registrado correctamente", 
                            "OK");

                        }
                        await Application.Current.MainPage.DisplayAlert("Exito", 
                        "Usuario registrado correctamente", 
                        "OK");
                        await Shell.Current.GoToAsync("//login");
                    }
                    catch(Exception ex){
                        await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo registrar el usuario tabla.\nError: {ex.Message}", "Aceptar");
                    }
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