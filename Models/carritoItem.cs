using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Bocaito.Models
{
    [Table("carrito_items")]
    public class CarritoItem : BaseModel
    {
        [PrimaryKey("id", false)]
        public int? Id { get; set; }

        [Column("carrito_id")]
        public int? CarritoId { get; set; }

        [Column("producto_id")]
        public int? ProductoId { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }
    }
}