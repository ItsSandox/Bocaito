using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bocaito.Models;

namespace Bocaito;

public partial class DireccionesPage : ContentPage, INotifyPropertyChanged
{
    public ObservableCollection<DireccionViewModel> Direcciones { get; set; }

    public DireccionesPage()
    {
        InitializeComponent();
        Direcciones = new ObservableCollection<DireccionViewModel>();
        BindingContext = this;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarDireccionesAsync();
    }
    
    private async void CargarDireccionesAsync()
    {
        try
        {
            // Mostrar indicador de carga
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;
            
            // Limpiar lista actual
            Direcciones.Clear();
            
            // Obtenemos el usuario actual
            var currentUser = App.SupabaseService.CurrentUser;
            if (currentUser == null)
            {
                await DisplayAlert("Error", "Debes iniciar sesión para ver tus direcciones", "OK");
                return;
            }
            
            // Consultamos direcciones usando el client de Supabase directamente
            var direccionesQuery = await App.SupabaseService.Client
                .From<Direccion>()
                .Where(d => d.UserId == currentUser.Id)
                .Get();
            
            if (direccionesQuery.Models.Count == 0)
            {
                // No hay direcciones
                return;
            }
            
            // Agregamos las direcciones al ObservableCollection
            foreach (var direccion in direccionesQuery.Models)
            {
                Direcciones.Add(new DireccionViewModel
                {
                    Id = direccion.Id,
                    DireccionNombre = "Casa", // Por defecto todas son "Casa", podría ser customizado
                    DireccionTexto = direccion.DireccionTexto,
                    Numero = direccion.Numero,
                    Indicaciones = direccion.Indicaciones,
                    Telefono = direccion.Telefono
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las direcciones: {ex.Message}", "OK");
        }
        finally
        {
            // Ocultar indicador de carga
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }
    }
    
    // Eventos de botones
    
    private async void OnRegresarClicked(object sender, EventArgs e)
    {
        // Volver a la página anterior
        await Shell.Current.GoToAsync("//cuenta", true);
    }
    
    private void OnOpcionesClicked(object sender, EventArgs e)
    {
        // Obtener el ID de la dirección desde el CommandParameter
        if (sender is ImageButton button && button.CommandParameter is int direccionId)
        {
            // Mostrar opciones para esta dirección (editar, eliminar, etc.)
            MostrarOpcionesDireccion(direccionId);
        }
    }
    
    private async void MostrarOpcionesDireccion(int direccionId)
    {
        // Mostrar opciones como un ActionSheet
        string action = await DisplayActionSheet("Opciones de dirección", 
            "Cancelar", null, "Editar", "Eliminar", "Establecer como predeterminada");
        
        switch (action)
        {
            case "Editar":
                // Navegación a página de edición con el ID
                await Shell.Current.GoToAsync($"editardireccion?id={direccionId}");
                break;
                
            case "Eliminar":
                // Confirmar eliminación
                bool confirmar = await DisplayAlert("Confirmar", 
                    "¿Estás seguro de que deseas eliminar esta dirección?", "Sí", "No");
                
                if (confirmar)
                {
                    await EliminarDireccion(direccionId);
                }
                break;
                
            case "Establecer como predeterminada":
                // Establecer como predeterminada
                await EstablecerComoPredeterminada(direccionId);
                break;
        }
    }
    
    private async Task EliminarDireccion(int direccionId)
    {
        try
        {
            // Mostrar indicador de carga
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;
            
            // Eliminar dirección de la base de datos
            await App.SupabaseService.Client
                .From<Direccion>()
                .Where(d => d.Id == direccionId)
                .Delete();
            
            // Eliminar de la colección local
            var direccionAEliminar = Direcciones.FirstOrDefault(d => d.Id == direccionId);
            if (direccionAEliminar != null)
            {
                Direcciones.Remove(direccionAEliminar);
            }
            
            await DisplayAlert("Éxito", "Dirección eliminada correctamente", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo eliminar la dirección: {ex.Message}", "OK");
        }
        finally
        {
            // Ocultar indicador de carga
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }
    }
    
    private async Task EstablecerComoPredeterminada(int direccionId)
    {
        // Esta funcionalidad requeriría una columna adicional en la tabla de direcciones
        // para marcar una como predeterminada.
        // Por ahora, solo mostraremos un mensaje.
        await DisplayAlert("Información", "Esta función estará disponible próximamente", "OK");
    }
    
    private async void OnAgregarDireccionClicked(object sender, EventArgs e)
    {
        // Navegar a la página para agregar una nueva dirección
        await Shell.Current.GoToAsync("//nuevadireccion");
    }
    
    // Implementación de INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Modelo para visualización de direcciones
public class DireccionViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    public int? Id { get; set; }
    public string DireccionNombre { get; set; }
    public string DireccionTexto { get; set; }
    public string Numero { get; set; }
    public string Indicaciones { get; set; }
    public string Telefono { get; set; }
    
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}