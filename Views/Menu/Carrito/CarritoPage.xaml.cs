using System.Collections.ObjectModel;
using Bocaito.Models;
using System.ComponentModel; // Agregar este using
using System.Runtime.CompilerServices; // Agregar este using

namespace Bocaito;

public partial class CarritoPage : ContentPage
{
    public ObservableCollection<CarritoItemViewModel> ItemsCarrito { get; set; }
    public decimal Total { get; set; }
    public bool TieneItems { get; set; }

    // Carrito local estático para mantenerlo disponible entre sesiones
    private static ObservableCollection<CarritoItemViewModel> _carritoLocal;

    public CarritoPage()
    {
        InitializeComponent();
        
        // Inicializar el carrito local si es la primera vez
        if (_carritoLocal == null)
        {
            _carritoLocal = new ObservableCollection<CarritoItemViewModel>();
        }
        
        ItemsCarrito = _carritoLocal;
        BindingContext = this;
        
        // Actualizar la vista
        ActualizarVista();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ActualizarVista();
    }
    
    private void ActualizarVista()
    {
        // Calcular totales
        CalcularTotales();
        
        // Actualizar visibilidad del resumen
        TieneItems = ItemsCarrito.Count > 0;
        OnPropertyChanged(nameof(TieneItems));
        OnPropertyChanged(nameof(ItemsCarrito));
    }
    
    private void CalcularTotales()
    {
        // Calcular total
        Total = ItemsCarrito.Sum(item => item.Producto.Precio * item.Cantidad);
        
        // Notificar cambios
        OnPropertyChanged(nameof(Total));
    }
    
    // Método estático para agregar productos al carrito local
    public static void AgregarProductoAlCarrito(Producto producto, int cantidad = 1)
    {
        // Inicializar si es necesario
        if (_carritoLocal == null)
        {
            _carritoLocal = new ObservableCollection<CarritoItemViewModel>();
        }
        
        // Buscar si el producto ya está en el carrito
        var itemExistente = _carritoLocal.FirstOrDefault(i => i.Producto.Id == producto.Id);
        
        if (itemExistente != null)
        {
            // Actualizar cantidad
            itemExistente.Cantidad += cantidad;
        }
        else
        {
            // Agregar nuevo item
            _carritoLocal.Add(new CarritoItemViewModel
            {
                Id = DateTime.Now.Ticks, // ID temporal
                Producto = producto,
                Cantidad = cantidad
            });
        }
    }
    
    // Eventos de botones
    
    private void OnRegresarClicked(object sender, EventArgs e)
    {
        // Volver a la página anterior
        Shell.Current.GoToAsync("..");
    }
    
    private async void OnVaciarCarritoClicked(object sender, EventArgs e)
    {
        // Confirmar acción
        bool confirmar = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas vaciar el carrito?", "Sí", "No");
        
        if (confirmar)
        {
            // Vaciar carrito local
            ItemsCarrito.Clear();
            
            // Actualizar vista
            ActualizarVista();
        }
    }
    
    private void OnAumentarCantidadClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int productoId)
        {
            // Buscar el producto en el carrito
            var item = ItemsCarrito.FirstOrDefault(i => i.Producto.Id == productoId);
            
            if (item != null)
            {
                // Aumentar cantidad
                item.Cantidad++;
                
                // Actualizar vista
                ActualizarVista();
            }
        }
    }
    
    private async void OnDisminuirCantidadClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int productoId)
        {
            // Buscar el producto en el carrito
            var item = ItemsCarrito.FirstOrDefault(i => i.Producto.Id == productoId);
            
            if (item != null)
            {
                if (item.Cantidad > 1)
                {
                    // Disminuir cantidad
                    item.Cantidad--;
                }
                else
                {
                    // Eliminar del carrito
                    bool confirmar = await DisplayAlert("Confirmar", "¿Eliminar este producto del carrito?", "Sí", "No");
                    
                    if (confirmar)
                    {
                        ItemsCarrito.Remove(item);
                    }
                }
                
                // Actualizar vista
                ActualizarVista();
            }
        }
    }
    
    private void OnIrAlMenuClicked(object sender, EventArgs e)
    {
        // Navegar a la página de menú
        Shell.Current.GoToAsync("//menu");
    }
    
    private async void OnRealizarOrdenClicked(object sender, EventArgs e)
    {
        if (ItemsCarrito.Count == 0)
        {
            await DisplayAlert("Error", "El carrito está vacío", "OK");
            return;
        }
        
        bool confirmar = await DisplayAlert("Confirmar Orden", "¿Estás seguro de que deseas realizar esta orden?", "Sí", "No");
        
        if (confirmar)
        {
            try
            {
                // Mostrar indicador de carga
                loadingIndicator.IsVisible = true;
                loadingIndicator.IsRunning = true;
                
                // Guardar el carrito en la base de datos
                bool exito = await App.SupabaseService.GuardarCarritoEnBaseDeDatosAsync(ItemsCarrito.ToList());
                
                if (exito)
                {
                    // Limpiar carrito local
                    ItemsCarrito.Clear();
                    
                    // Actualizar vista
                    ActualizarVista();
                    
                    // Mostrar mensaje de éxito
                    await DisplayAlert("Éxito", "Tu orden ha sido realizada con éxito", "OK");
                    
                    // Opcional: Navegar a una página de confirmación o a la página de pedidos
                    Shell.Current.GoToAsync("//pedidos");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo completar la orden", "OK");
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
    }
}

// Clase para el ViewModel de CarritoItem
public class CarritoItemViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public long Id { get; set; } // Cambiado a long para usar DateTime.Ticks
    
    public Producto Producto { get; set; }
    
    private int _cantidad;
    public int Cantidad
    {
        get => _cantidad;
        set
        {
            if (_cantidad != value)
            {
                _cantidad = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Subtotal)); // También notificar que Subtotal cambió
            }
        }
    }
    
    public decimal Subtotal => Producto.Precio * Cantidad;

    // Método para notificar cambios de propiedad
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}