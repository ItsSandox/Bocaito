using Newtonsoft.Json;
namespace Bocaito.Models

{
    public class Usuario
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("Nombre")]
        public string Nombre { get; set; }

        [JsonProperty("Apellido")]
        public string Apellido { get; set; }
    }
}
