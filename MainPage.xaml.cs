using Bocaito.Services;
using Supabase.Gotrue;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Bocaito
{
    public partial class MainPage : ContentPage
    {
        private readonly SupabaseService _supabaseService = new();

        public MainPage()
        {
            InitializeComponent();
            IniciarSupabase();
        }

        private async void IniciarSupabase()
        {
            await _supabaseService.Initialize();
        }

        private async void signup_clicked(object sender, EventArgs e)
        {

            try
            {
                var session = await _supabaseService.SignUp(correo.Text, contraseña.Text, nombre.Text, apellido.Text);
                if (session != null)
                {
                    await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
