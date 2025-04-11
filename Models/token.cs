
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Bocaito.Models
{
    [Table("tokens")]
    public class Token : BaseModel
    {
        [PrimaryKey("id", false)]
        public int? Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("firebase_token")]
        public string FirebaseToken { get; set; }

        [Column("device_type")]
        public string DeviceType { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}