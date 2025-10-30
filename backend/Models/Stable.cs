using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Stable")]
    public class Stable
    {
        [Key]
        [Column("stableId")]
        [MaxLength(15)]
        public string StableId { get; set; } = string.Empty;

        [Column("stableName")]
        [Required]
        [MaxLength(100)]
        public string StableName { get; set; } = string.Empty;

        [Column("location")]
        [MaxLength(100)]
        public string? Location { get; set; }

        [Column("colors")]
        [MaxLength(50)]
        public string? Colors { get; set; }

        // Navigation properties
        public ICollection<Horse> Horses { get; set; } = new List<Horse>();
        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
    }
}
