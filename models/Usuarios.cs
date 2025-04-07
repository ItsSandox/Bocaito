using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models; 

namespace Bocaito.Models
{
    [Table("usuarios")]
    public class Usuario : BaseModel
    {
        [PrimaryKey("id",false)]
        public int? id { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Apellido")]
        public string Apellido { get; set; }

        [Column("Telefono")]
        public string Telefono { get; set; }

        [Column("user_id")]
        public string user_id { get; set; }

    }
}