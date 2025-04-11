using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bocaito.Models;

namespace Bocaito;

public partial class CrearReservaPage : ContentPage, INotifyPropertyChanged
{
    private DateTime _fechaSeleccionada;
    public DateTime FechaSeleccionada
    {
        get => _fechaSeleccionada;
        set
        {
            if (_fechaSeleccionada != value)
            {
                _fechaSeleccionada = value;
                OnPropertyChanged();
            }
        }
    }

    private TimeSpan _horaSeleccionada;
    public TimeSpan HoraSeleccionada
    {
        get => _horaSeleccionada;
        set
        {
            if (_horaSeleccionada != value)
            {
                _horaSeleccionada = value;
                OnPropertyChanged();
            }
        }
    }

    private string _descripcion;
    public string Descripcion
    {
        get => _descripcion;
        set
        {
            if (_descripcion != value)
            {
                _descripcion = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime MinimumDate { get; }
    public DateTime MaximumDate { get; }

    public CrearReservaPage()
    {
        InitializeComponent();

        // Establecer fechas límite para la reserva
        MinimumDate = DateTime.Today;
        MaximumDate = DateTime.Today.AddMonths(3);

        // Establecer valores predeterminados
        FechaSeleccionada = DateTime.Today;
        HoraSeleccionada = DateTime.Now.TimeOfDay;

        BindingContext = this;
    }

    private async void OnCrearReservaClicked(object sender, EventArgs e)
    {
        try
        {
            // Combinar fecha y hora seleccionadas
            var fechaReserva = FechaSeleccionada.Date + HoraSeleccionada;

            // Validar que la fecha de reserva sea futura
            if (fechaReserva <= DateTime.Now)
            {
                await DisplayAlert("Error", "La reserva debe ser para una fecha y hora futuras.", "OK");
                return;
            }

            // Crear la reserva usando el servicio de Supabase
            var reservaCreada = await App.SupabaseService.CrearReservaAsync(fechaReserva, Descripcion ?? "");

            if (reservaCreada != null)
            {
                // Mostrar mensaje de éxito
                await DisplayAlert("Éxito", "Reserva creada correctamente", "OK");

                // Navegar de vuelta a la página de reservas
                await Shell.Current.GoToAsync("//reservas");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo crear la reserva", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }

    private async void OnRegresarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}