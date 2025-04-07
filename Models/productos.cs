using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Bocaito.Models
{
    [Table("productos")]
    public class Producto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int? Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("foto_id")]
        public int? FotoId { get; set; }

        [Column("categoria_id")]
        public int? CategoriaId { get; set; }

        // Propiedades de navegación (opcional, para facilitar las relaciones)
        public Categoria Categoria { get; set; }
        public Foto Foto { get; set; }
    }
}