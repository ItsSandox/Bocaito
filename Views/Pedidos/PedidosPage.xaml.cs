using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using Bocaito.Models;
using Bocaito.Services;

namespace Bocaito;

public partial class PedidosPage : ContentPage, INotifyPropertyChanged
{
    public ObservableCollection<PedidoItemModel> Pedidos { get; set; }
    public bool TienePedidos { get; set; }

    public PedidosPage()
    {
        InitializeComponent();
        Pedidos = new ObservableCollection<PedidoItemModel>();
        BindingContext = this;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarPedidosAsync();
    }
    
    private async void CargarPedidosAsync()
    {
        try
        {
            loadingIndicator.IsVisible = loadingIndicator.IsRunning = true;
            Pedidos.Clear();
            
            var pedidosData = await App.SupabaseService.ObtenerPedidosUsuarioAsync();
            
            foreach (var pedido in pedidosData)
            {
                Pedidos.Add(new PedidoItemModel
                {
                    Id = pedido.Id,
                    FechaCreacion = pedido.FechaCreacion,
                    Estado = pedido.Estado,
                    Total = pedido.Total
                });
            }
            
            TienePedidos = Pedidos.Count > 0;
            OnPropertyChanged(nameof(TienePedidos));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los pedidos: {ex.Message}", "OK");
        }
        finally
        {
            loadingIndicator.IsVisible = loadingIndicator.IsRunning = false;
        }
    }
    
    private void OnRegresarClicked(object sender, EventArgs e) => Shell.Current.GoToAsync("..");
    
    private async void OnIrAlMenuClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//menu");
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

// Modelo simplificado para los pedidos
public class PedidoItemModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    public int? Id { get; set; }
    
    private DateTime _fechaCreacion;
    public DateTime FechaCreacion
    {
        get => _fechaCreacion;
        set
        {
            if (_fechaCreacion != value)
            {
                _fechaCreacion = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FechaFormateada));
            }
        }
    }
    
    public string FechaFormateada
    {
        get
        {
            var cultureInfo = new CultureInfo("es-ES");
            return $"{_fechaCreacion.Day} de {cultureInfo.DateTimeFormat.GetMonthName(_fechaCreacion.Month).ToLower() }";
        }
    }
    
    private string _estado;
    public string Estado
    {
        get => string.IsNullOrEmpty(_estado) ? string.Empty : char.ToUpper(_estado[0]) + _estado.Substring(1);
        set
        {
            if (_estado != value)
            {
                _estado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstadoColor));
            }
        }
    }
    
    public string EstadoColor => _estado?.ToLower() switch
    {
        "pendiente" => "#FFA500",
        "en preparación" => "#3498DB",
        "en camino" => "#9B59B6",
        "entregado" => "#2ECC71",
        "cancelado" => "#E74C3C",
        _ => "#808080"
    };
    
    public decimal Total { get; set; }
    
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}