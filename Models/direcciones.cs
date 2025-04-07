using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Bocaito.Models
{
    [Table("direcciones")]
    public class Direccion : BaseModel
    {
        [PrimaryKey("id", false)]
        public int? Id { get; set; }

        [Column("direccion")]
        public string DireccionTexto { get; set; }

        [Column("numero")]
        public string Numero { get; set; }

        [Column("indicaciones")]
        public string Indicaciones { get; set; }

        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }
        
    }
    
}