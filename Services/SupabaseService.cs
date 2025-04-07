using System;
using System.Text.Json;
using System.Threading.Tasks;
using Bocaito.Models;
using Microsoft.Maui.Controls;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;


namespace Bocaito.Services
{
    public class SupabaseService
    {
        private Supabase.Client? _client;
        public Supabase.Client? Client => _client;

        public async Task<bool> InitializeAsync()
        {
            try
            {
                var url = "https://roqjqdllqagecccpsaix.supabase.co";
                var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InJvcWpxZGxscWFnZWNjY3BzYWl4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDI1MzEyMDQsImV4cCI6MjA1ODEwNzIwNH0.Mrq2Zvn9FFLXzGkwl1f0HJtMbJjwPd4Wyc_5NYPS5hw";

                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = true
                };

                _client = new Supabase.Client(url, key, options);
                await _client.InitializeAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar Supabase: {ex.Message}");
                return false;
            }
        }
        //Métodos de consulta de datos
    public async Task<Direccion> ObtenerDireccionUsuarioAsync()
    {
        if (_client == null)
            throw new InvalidOperationException("Supabase client is not initialized");
        
        try 
        {
            // Obtener el usuario autenticado actual
            var currentUser = _client.Auth.CurrentUser;
            if (currentUser == null)
                return null;

            // Buscar la dirección del usuario en la tabla de direcciones
            var direccionQuery = await _client
                .From<Direccion>()
                .Where(d => d.UserId == currentUser.Id)
                .Get();

            // Devolver la primera dirección encontrada (asumiendo que el usuario tiene una dirección principal)
            return direccionQuery.Models.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la dirección del usuario: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert(
                "Error", 
                "No se pudo obtener la dirección", 
                "Ok"
            );
            return null;
        }
    }



    public async Task<List<CategoriaConProductos>> ObtenerCategoriasConProductosAsync()
    {
        if (_client == null)
            throw new InvalidOperationException("Supabase client is not initialized");

        try
        {
            // Obtener todas las categorías
            var categorias = await _client
                .From<Categoria>()
                .Get();

            var resultado = new List<CategoriaConProductos>();

            // Para cada categoría, obtener sus productos
            foreach (var categoria in categorias.Models)
            {
                // Obtener productos de la categoría
                var productos = await _client
                    .From<Producto>()
                    .Where(p => p.CategoriaId == categoria.Id)
                    .Get();

                var productosConFoto = new List<Producto>();

                // Para cada producto, obtener su foto
                foreach (var producto in productos.Models)
                {
                    // Obtener la foto del producto
                    var fotoQuery = await _client
                        .From<Foto>()
                        .Where(f => f.Id == producto.FotoId)
                        .Get();

                    var productoConFoto = new Producto
                    {
                        Id = producto.Id,
                        Nombre = producto.Nombre,
                        Descripcion = producto.Descripcion,
                        Precio = producto.Precio,
                        Foto = fotoQuery.Models.FirstOrDefault() != null 
                            ? new Foto { Enlace = fotoQuery.Models.First().Enlace } 
                            : null
                    };

                    productosConFoto.Add(productoConFoto);
                }

                resultado.Add(new CategoriaConProductos
                {
                    CategoriaNombre = categoria.Nombre,
                    Productos = productosConFoto
                });
            }

            return resultado;
        }
        catch (Exception ex)
        {
            // Manejar y registrar el error
            Console.WriteLine($"Error al obtener categorías con productos: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert(
                "Error", 
                "No se pudieron cargar los productos", 
                "Ok"
            );
            return new List<CategoriaConProductos>();
        }
    }

    // Clase para representar categorías con sus productos
    public class CategoriaConProductos
    {
        public string CategoriaNombre { get; set; }
        public List<Producto> Productos { get; set; }
    }
    public Usuario GetCurrentUsuario()
    {
        if (_client == null)
            throw new InvalidOperationException("Supabase client is not initialized");
        
        try 
        {
            // Obtener el usuario autenticado actual
            var currentUser = _client.Auth.CurrentUser;
            if (currentUser == null || currentUser.UserMetadata == null)
                return null; // No hay usuario autenticado o sin metadatos
            
            // Crear y retornar un objeto Usuario con los metadatos
            return new Usuario
            {
                user_id = currentUser.Id,
                Nombre = currentUser.UserMetadata.TryGetValue("nombre", out object nombre) && nombre != null ? nombre.ToString() : string.Empty,
                Apellido = currentUser.UserMetadata.TryGetValue("apellido", out object apellido) && apellido != null ? apellido.ToString() : string.Empty,
                Telefono = currentUser.UserMetadata.TryGetValue("telefono", out object telefono) && telefono != null ? telefono.ToString() : string.Empty
            };
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("Error",$"Error al obtener el usuario actual: {ex.Message}","Ok");
            return null;
        }
    }
        //Métodos de insertar datos

    public async Task<Usuario?> CreateUsuario(string nombre, string apellido, string telefono ,string uid)
        {
            if (_client == null)
                throw new InvalidOperationException("Supabase client is not initialized");
            
            var usuario = new Usuario
            {
                Nombre = nombre,
                Apellido = apellido,
                Telefono = telefono,
                user_id = uid
            };
            
            // Insertar en la tabla usuarios
            var response = await _client.From
                <Usuario>()
                .Insert(usuario);

            // Verificar si la operación fue exitosa
            return response.Models.FirstOrDefault();
        }

        // Métodos de autenticación
    public async Task<Session?> RestoreSession()
    {
        if (_client == null)
            throw new InvalidOperationException("Supabase client is not initialized");
        
        try
        {
            // Obtener el access token almacenado
            string? accessToken = await SecureStorage.GetAsync("access_token");
            string? refresh_token = await SecureStorage.GetAsync("refresh_token");
            
            if (string.IsNullOrEmpty(accessToken))
                return null; // No hay token guardado
            
            // Verificar si el token es válido
            try 
            {
                var user = await _client.Auth.GetUser(accessToken);
                await _client.Auth.SetSession(accessToken, refresh_token, false);
                
                // Si llegamos aquí, el token es válido
                return new Session { AccessToken = accessToken };
            }
            catch
            {
                // El token ha expirado o es inválido
                SignOut(); // Cerrar sesión
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al restaurar sesión: {ex.Message}");
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            await Application.Current.MainPage.DisplayAlert("Error",$"Error al restaurar sesión: {ex.Message}","Ok");
            return null;
        }
    }
        public async Task<bool> RequestPasswordRecovery(string email)
        {
            try
            {
                // Enviar correo de recuperación de contraseña
                await _client.Auth.ResetPasswordForEmail(email);
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", 
                $"No se pudo recuperar la contraseña: {ex}", 
                "OK");
                return false;
            }
        }
        public async Task<bool> UpdatePassword(string email, string otp, string newPassword){
            try{
                var result = await _client.Auth.VerifyOTP(email,otp, Constants.EmailOtpType.Email);
                if(result != null){
                    await _client.Auth.Update(new UserAttributes
                {
                    Password = newPassword
                });
                    return true;

                }else{
                    await Application.Current.MainPage.DisplayAlert("Error", 
                    $"No se pudo cambiar la contraseña: {result}", 
                    "OK");
                return false;
                }

            }catch(Exception ex){
                await Application.Current.MainPage.DisplayAlert("Error", 
                $"estas en el ex de update password No se pudo cambiar la contraseña: {ex}", 
                "OK");
                return false;
            }
        }
        public async Task<Session?> SignIn(string email, string password)
        {
            if (_client == null)
                throw new InvalidOperationException("Supabase client is not initialized");
            try
            {
                var response = await _client.Auth.SignIn(email, password);
            if (response != null)
                {
                    await StoreSessionToken(response);
                }

                return response;
            }
            catch (Exception ex)
            {   
                await Application.Current.MainPage.DisplayAlert("Error", 
                $"No se pudo iniciar sesión: {ex}", 
                "OK");
                throw;
            }
        }

    public async Task<Session?> SignUp(string email, string password, string nombre, string apellido, string telefono)
    {
        if (_client == null)
            throw new InvalidOperationException("Supabase client is not initialized");
        
        try
        {
            // Crear objeto de metadatos
            var metadata = new Dictionary<string, object>
            {
                { "nombre", nombre },
                { "apellido", apellido },
                { "telefono", telefono }
            };
            
            // Registrar usuario con metadatos
            var options = new SignUpOptions { Data = metadata };
            var authResponse = await _client.Auth.SignUp(email, password, options);
            
            if (authResponse?.User != null)
            {
                return authResponse;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al registrar usuario: {ex}");
            throw;
        }
    }
        
        
        public async Task SignOut()
        {
            if (_client == null)
                throw new InvalidOperationException("Supabase client is not initialized");
            try
            {
                await _client.Auth.SignOut();
                SecureStorage.Remove("access_token");
                SecureStorage.Remove("refresh_token");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", 
                $"No se pudo cerrar sesión: {ex}", 
                "OK");
                
                throw;
            }
            
        }
        
        public Session? CurrentSession
        {
            get
            {
                if (_client == null)
                    throw new InvalidOperationException("Supabase client is not initialized");
                
                return _client.Auth.CurrentSession;
            }
        }
        
        public User? CurrentUser
        {
            get
            {
                if (_client == null)
                    throw new InvalidOperationException("Supabase client is not initialized");
                
                return _client.Auth.CurrentUser;
            }
        }

        public async Task<bool> ResetPasswordForEmail(string email)
        {
            if (_client == null)
                throw new InvalidOperationException("Supabase client is not initialized");
            
            try
            {
                await _client.Auth.ResetPasswordForEmail(email);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo de recuperación: {ex.Message}");
                return false;
            }
        }

        public async Task<User?> UpdateUser(UserAttributes attributes)
        {
            if (_client == null)
                throw new InvalidOperationException("Supabase client is not initialized");
            
            try
            {
                return await _client.Auth.Update(attributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar usuario: {ex.Message}");
                return null;
            }
        }

        //aux
        public async Task StoreSessionToken(Session session)
        {
            await SecureStorage.SetAsync("access_token", session.AccessToken);
            await SecureStorage.SetAsync("refresh_token", session.RefreshToken);
            // Opcionalmente guardar el email si lo necesitas
            // await SecureStorage.SetAsync("email", email);
        }
    }
}

