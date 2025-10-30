using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Horse")]
    public class Horse
    {
        [Key]
        [Column("horseId")]
        [MaxLength(15)]
        public string HorseId { get; set; } = string.Empty;

        [Column("horseName")]
        [Required]
        [MaxLength(100)]
        public string HorseName { get; set; } = string.Empty;

        [Column("age")]
        public int? Age { get; set; }

        [Column("gender")]
        [MaxLength(10)]
        public string? Gender { get; set; }

        [Column("registration")]
        [MaxLength(50)]
        public string? Registration { get; set; }

        [Column("stableId")]
        [MaxLength(15)]
        public string? StableId { get; set; }

        // Navigation properties
        [ForeignKey("StableId")]
        public Stable? Stable { get; set; }

        public ICollection<Owns> Owns { get; set; } = new List<Owns>();
        public ICollection<RaceResults> RaceResults { get; set; } = new List<RaceResults>();
    }
}
