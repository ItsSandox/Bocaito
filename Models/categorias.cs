using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Bocaito.Models
{
    [Table("categorias")]
        public class Categoria : BaseModel
        {
            [PrimaryKey("id", false)]
            public int? Id { get; set; }

            [Column("nombre")]
            public string Nombre { get; set; }

            // Colección de pedidos en esta categoría (opcional)
            public ICollection<Producto> Productos { get; set; }
        }
}