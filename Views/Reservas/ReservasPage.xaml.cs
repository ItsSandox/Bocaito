using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bocaito.Models;
using Bocaito.Services;

namespace Bocaito;

public partial class ReservasPage : ContentPage, INotifyPropertyChanged
{
    public ObservableCollection<Reserva> Reservas { get; set; }
    public bool TieneReservas { get; set; }

    public ReservasPage()
    {
        InitializeComponent();
        Reservas = new ObservableCollection<Reserva>();
        BindingContext = this;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarReservasAsync();
    }
    
    private async void CargarReservasAsync()
    {
        try
        {
            loadingIndicator.IsVisible = loadingIndicator.IsRunning = true;
            Reservas.Clear();
            
            var reservasData = await App.SupabaseService.ObtenerReservasUsuarioAsync();
            
            foreach (var reserva in reservasData)
            {
                Reservas.Add(reserva);
            }
            
            TieneReservas = Reservas.Count > 0;
            OnPropertyChanged(nameof(TieneReservas));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las reservas: {ex.Message}", "OK");
        }
        finally
        {
            loadingIndicator.IsVisible = loadingIndicator.IsRunning = false;
        }
    }
    
    private async void OnCrearReservaClicked(object sender, EventArgs e)
    {
        try
        {
            // Navegar a la página de crear reserva
            await Shell.Current.GoToAsync("//crear-reserva");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}