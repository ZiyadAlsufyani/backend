using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Race")]
    public class Race
    {
        [Key]
        [Column("raceId")]
        [MaxLength(15)]
        public string RaceId { get; set; } = string.Empty;

        [Column("raceName")]
        [Required]
        [MaxLength(100)]
        public string RaceName { get; set; } = string.Empty;

        [Column("trackName")]
        [MaxLength(100)]
        public string? TrackName { get; set; }

        [Column("raceDate")]
        public DateTime? RaceDate { get; set; }

        [Column("raceTime")]
        public TimeSpan? RaceTime { get; set; }

        // Navigation properties
        [ForeignKey("TrackName")]
        public Track? Track { get; set; }

        public ICollection<RaceResults> RaceResults { get; set; } = new List<RaceResults>();
    }
}
