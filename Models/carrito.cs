using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Bocaito.Models
{
    [Table("carrito")]
    public class Carrito : BaseModel
    {
        [PrimaryKey("id", false)]
        public int? Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }
    }
}