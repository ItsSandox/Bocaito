using System.Collections.ObjectModel;
using Bocaito.Models;
using Bocaito.Services;

namespace Bocaito;

public partial class MenuPage : ContentPage
{
    public ObservableCollection<SupabaseService.CategoriaConProductos> CategoriasConProductos { get; set; }

    public MenuPage()
    {
        InitializeComponent();

        CategoriasConProductos = new ObservableCollection<SupabaseService.CategoriaConProductos>();
        
        // Establecer el BindingContext directamente
        BindingContext = this;

        // Cargar datos
        LoadUserAddress();
        LoadProductosAsync();
    }

    private async void LoadUserAddress()
    {
        try
        {
            var direccion = await App.SupabaseService.ObtenerDireccionUsuarioAsync();
            
            if (direccion != null)
            {
                // Formatear la dirección para mostrar
                lblDireccion.Text = $"{direccion.DireccionTexto}";
            }
            else
            {
                lblDireccion.Text = "Dirección no disponible";
            }
        }
        catch (Exception ex)
        {
            lblDireccion.Text = "Error al cargar dirección";
            await App.Current.MainPage.DisplayAlert("Error",$"Error cargando dirección: {ex.Message}","OK");
        }
    }

    private async void LoadProductosAsync()
    {
        try
        {
            // Mostrar indicador de carga
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            // Obtener categorías con productos
            var categorias = await App.SupabaseService.ObtenerCategoriasConProductosAsync();

            // Limpiar y poblar la colección
            CategoriasConProductos.Clear();
            foreach (var categoria in categorias)
            {
                CategoriasConProductos.Add(categoria);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudieron cargar los productos", "Ok");
            Console.WriteLine($"Error cargando productos: {ex.Message}");
        }
        finally
        {
            // Ocultar indicador de carga
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }
    }

    // Eventos de botones
    
    private async void OnSeleccionarProductoClicked(object sender, EventArgs e)
    {
        try
        {
            // Obtener el Grid que contiene el producto
            if (sender is Grid grid && grid.BindingContext is Producto producto)
            {
                // Agregar el producto al carrito local
                CarritoPage.AgregarProductoAlCarrito(producto);
                
                // Mostrar un mensaje de éxito
                await DisplayAlert("Éxito", $"{producto.Nombre} agregado al carrito", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }
    private void OnFlechaAbajoClicked(object sender, EventArgs e)
    {
        // Implementar lógica para flecha abajo
    }

    private void OnNotificationsClicked(object sender, EventArgs e)
    {
        // Implementar lógica de notificaciones
    }

    private async void OnCarritoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CarritoPage));
    }
}