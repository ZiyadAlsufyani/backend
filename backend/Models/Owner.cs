using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Owner")]
    public class Owner
    {
        [Key]
        [Column("ownerId")]
        [MaxLength(15)]
        public string OwnerId { get; set; } = string.Empty;

        [Column("lname")]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Column("fname")]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Owns> Owns { get; set; } = new List<Owns>();
    }
}
