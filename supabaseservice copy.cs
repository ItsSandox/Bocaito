using Supabase;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase.Postgrest.Models;

namespace Bocaito.Services
{
    public class SupabaseService
    {
        private static readonly string SupabaseUrl = "https://roqjqdllqagecccpsaix.supabase.co";
        private static readonly string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InJvcWpxZGxscWFnZWNjY3BzYWl4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDI1MzEyMDQsImV4cCI6MjA1ODEwNzIwNH0.Mrq2Zvn9FFLXzGkwl1f0HJtMbJjwPd4Wyc_5NYPS5hw";
        private Supabase.Client supabase;

        public SupabaseService()
        {
            supabase = new Supabase.Client(SupabaseUrl, SupabaseKey);
        }

        public async Task Initialize()
        {
            await supabase.InitializeAsync();
        }

        public async Task<Session> SignUp(string email, string password, string nombre, string apellido)
        {
            var session = await supabase.Auth.SignUp(email, password);
            try{
            if (session == null || session.User == null)
                throw new Exception("Error en el registro de usuario 1");
            Console.WriteLine("Id: " + session.User.Id);
            var newUser = new Usuario
            {
                id = session.User.Id,
                Nombre = nombre,
                Apellido = apellido 
            };
            Console.WriteLine("Id: " + session.User.Id);

            await supabase.From<Usuario>().Insert(new List<Usuario> { newUser });

            return session;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el registro de usuario 2" + session.User.Id, ex);
            }
        }
    }
    public class Usuario : BaseModel
    {
        public string id { get; set; }
        public string Nombre { get; set; }
        public string Apellido{ get; set; }
    }
}