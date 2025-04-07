using Microsoft.IdentityModel.Tokens;

namespace Bocaito
{
    public partial class CuentaPage : ContentPage
    {
        public CuentaPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            var user = App.SupabaseService.GetCurrentUsuario();
            
            if (user != null)
            {
                usuarioLabel.Text = $"¡Hola {user.Nombre}!";
            }
        }
        private async void OnCerrarSesionClicked(object sender, EventArgs e)
        {
            try
            {
                await App.SupabaseService.SignOut();
                await Shell.Current.GoToAsync("//login", true);
            }
            catch (System.Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo cerrar sesión. Inténtalo de nuevo. {ex}", "OK");
            }
        }
    }
}