using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Supabase.Gotrue;

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

        // Métodos de autenticación
        public async Task<Session?> SignIn(string email, string password)
        {
            if (_client == null)
                throw new InvalidOperationException("Supabase client is not initialized");
                
            return await _client.Auth.SignIn(email, password);
        }

        public async Task<Session?> SignUp(string email, string password)
        {
            if (_client == null)
                throw new InvalidOperationException("Supabase client is not initialized");
                
            return await _client.Auth.SignUp(email, password);
        }
        
        public async Task SignOut()
        {
            if (_client == null)
                throw new InvalidOperationException("Supabase client is not initialized");
            
            await _client.Auth.SignOut();
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
    }
}