using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Owns")]
    public class Owns
    {
        [Column("ownerId")]
        [MaxLength(15)]
        public string OwnerId { get; set; } = string.Empty;

        [Column("horseId")]
        [MaxLength(15)]
        public string HorseId { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("OwnerId")]
        public Owner Owner { get; set; } = null!;

        [ForeignKey("HorseId")]
        public Horse Horse { get; set; } = null!;
    }
}
