using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Bocaito.Models
{
    [Table("reservas")]
    public class Reserva : BaseModel
    {
        [PrimaryKey("id", false)]
        public int? Id { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }
    }
}