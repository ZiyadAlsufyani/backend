using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("RaceResults")]
    public class RaceResults
    {
        [Column("raceId")]
        [MaxLength(15)]
        public string RaceId { get; set; } = string.Empty;

        [Column("horseId")]
        [MaxLength(15)]
        public string HorseId { get; set; } = string.Empty;

        [Column("results")]
        [MaxLength(50)]
        public string? Results { get; set; }

        [Column("prize", TypeName = "decimal(10,2)")]
        public decimal? Prize { get; set; }

        // Navigation properties
        [ForeignKey("RaceId")]
        public Race Race { get; set; } = null!;

        [ForeignKey("HorseId")]
        public Horse Horse { get; set; } = null!;
    }
}
