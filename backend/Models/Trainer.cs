using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Trainer")]
    public class Trainer
    {
        [Key]
        [Column("trainerId")]
        [MaxLength(15)]
        public string TrainerId { get; set; } = string.Empty;

        [Column("lname")]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Column("fname")]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Column("stableId")]
        [MaxLength(15)]
        public string? StableId { get; set; }

        // Navigation properties
        [ForeignKey("StableId")]
        public Stable? Stable { get; set; }
    }
}
