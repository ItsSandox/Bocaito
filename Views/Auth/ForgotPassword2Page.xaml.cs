using System.Threading.Tasks;

namespace Bocaito
{
    public partial class ForgotPassword2Page : ContentPage
    {
        private string _email;

        public ForgotPassword2Page(string email)
        {
            InitializeComponent();
            _email = email;
        }

        async void OnVerifyCodeClicked(object sender, EventArgs e)
        {
            if (loadingIndicator.IsRunning)
                return;
                
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;
            resetpassword.IsEnabled = false;

            try{
                var result = await App.SupabaseService.UpdatePassword(_email,otp.Text,contraseña.Text);
                if (result){

                    await Application.Current.MainPage.DisplayAlert("Éxito", 
                    $"Has cambiado la contraseña", 
                    "OK");
                    await Shell.Current.GoToAsync("//login");

                }else{
                    //ya tengo un error del otro lado jaja
                }
            }catch(Exception ex){
                    await Application.Current.MainPage.DisplayAlert("Error", 
                    $"Hubo un error{ex}", 
                    "OK");
            }finally{
                // Desactivar indicador de carga
                loadingIndicator.IsVisible = false;
                loadingIndicator.IsRunning = false;
                resetpassword.IsEnabled = true;
            }
        }

        async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..", true);
        }
    }
}