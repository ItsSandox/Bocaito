using System.Threading.Tasks;
using Bocaito.Models;
using Bocaito.Services;

namespace Bocaito;

public partial class NuevaDireccion : ContentPage
{
    public NuevaDireccion()
    {
        InitializeComponent();
    }

    private async void OnRegresarClicked(object sender, EventArgs e)
    {
        // Volver a la página anterior
        await Shell.Current.GoToAsync("..");
    }

    private async void OnGuardarDireccionClicked(object sender, EventArgs e)
    {
        try
        {
            // Validar campos
            if (!ValidarCampos())
            {
                return;
            }

            // Mostrar indicador de carga
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            // Obtener el usuario actual
            var currentUser = App.SupabaseService.CurrentUser;
            if (currentUser == null)
            {
                await DisplayAlert("Error", "Debes iniciar sesión para agregar una dirección", "OK");
                return;
            }

            // Crear nueva dirección
            var nuevaDireccion = new Direccion
            {
                UserId = currentUser.Id,
                DireccionTexto = entryDireccion.Text.Trim(),
                Numero = entryNumero.Text?.Trim() ?? string.Empty,
                Indicaciones = entryIndicaciones.Text?.Trim() ?? string.Empty,
                Telefono = entryTelefono.Text?.Trim() ?? string.Empty
            };

            // Insertar dirección en Supabase
            var result = await App.SupabaseService.Client
                .From<Direccion>()
                .Insert(nuevaDireccion);

            // Verificar si la inserción fue exitosa
            if (result.Models.Count > 0)
            {
                await DisplayAlert("Éxito", "Dirección agregada correctamente", "OK");
                
                // Regresar a la página anterior
                await Shell.Current.GoToAsync("//menu");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar la dirección", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
        finally
        {
            // Ocultar indicador de carga
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }
    }

    private bool ValidarCampos()
    {
        // Validar dirección
        if (string.IsNullOrWhiteSpace(entryDireccion.Text))
        {
            DisplayAlert("Validación", "Por favor, ingresa la dirección completa", "OK");
            return false;
        }

        // Validar número de teléfono (opcional, pero si se ingresa debe ser válido)
        if (!string.IsNullOrWhiteSpace(entryTelefono.Text) && 
            !EsTelefonoValido(entryTelefono.Text))
        {
            DisplayAlert("Validación", "Por favor, ingresa un número de teléfono válido", "OK");
            return false;
        }

        return true;
    }

    private bool EsTelefonoValido(string telefono)
    {
        // Eliminar espacios y guiones
        telefono = telefono.Replace(" ", "").Replace("-", "");

        // Verificar que solo contenga números y tenga una longitud razonable
        return System.Text.RegularExpressions.Regex.IsMatch(telefono, @"^\d{8,15}$");
    }
}