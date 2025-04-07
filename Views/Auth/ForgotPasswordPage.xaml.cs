using System.Threading.Tasks;
using Supabase;

namespace Bocaito
{
    public partial class ForgotPasswordPage : ContentPage
    {

        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        async void OnSendCodeClicked(object sender, EventArgs e)
        {
            if (loadingIndicator.IsRunning)
                return;
                
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;
            enviarcodigo.IsEnabled = false;
            try{
                var result = await App.SupabaseService.RequestPasswordRecovery(correo.Text);
                if (result){
                    await Application.Current.MainPage.DisplayAlert("Éxito", 
                    $"Código enviado a: {correo.Text}", 
                    "OK");
                    await Navigation.PushAsync(new ForgotPassword2Page(correo.Text));
                }else{
                    await Application.Current.MainPage.DisplayAlert("Error", 
                    $"Hubo un error", 
                    "OK");
                }
            }catch(Exception ex){
                    await Application.Current.MainPage.DisplayAlert("Error", 
                    $"Parece que el correo {correo.Text} no está registrado {ex}", 
                    "OK");
            }finally{
                // Desactivar indicador de carga
                loadingIndicator.IsVisible = false;
                loadingIndicator.IsRunning = false;
                enviarcodigo.IsEnabled = true;
            }
        } 

        async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..", true);
        }
    }
}