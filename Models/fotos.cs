using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
    
    namespace Bocaito.Models{
    [Table("fotos")]
    public class Foto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int? Id { get; set; }

        [Column("enlace")]
        public string Enlace { get; set; }

        // Colección de pedidos con esta foto (opcional)
        public ICollection<Producto> Productos { get; set; }
    }
}