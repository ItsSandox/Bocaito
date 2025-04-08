namespace Bocaito;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	// Registro de rutas para páginas de autenticación
    Routing.RegisterRoute("login", typeof(LogInPage));
    Routing.RegisterRoute("signup", typeof(SignUpPage));
    Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));
    Routing.RegisterRoute("forgotpassword2", typeof(ForgotPassword2Page));
    
    // Registro de rutas para páginas de navegación principal
    Routing.RegisterRoute("menu", typeof(MenuPage));
    Routing.RegisterRoute("pedidos", typeof(PedidosPage));
    Routing.RegisterRoute("reservas", typeof(ReservasPage));
    Routing.RegisterRoute("cuenta", typeof(CuentaPage));
    Routing.RegisterRoute(nameof(CarritoPage), typeof(CarritoPage));
	}
}
