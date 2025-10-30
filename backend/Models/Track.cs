using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Track")]
    public class Track
    {
        [Key]
        [Column("trackName")]
        [MaxLength(100)]
        public string TrackName { get; set; } = string.Empty;

        [Column("location")]
        [MaxLength(100)]
        public string? Location { get; set; }

        [Column("length", TypeName = "decimal(5,2)")]
        public decimal? Length { get; set; }

        // Navigation properties
        public ICollection<Race> Races { get; set; } = new List<Race>();
    }
}
