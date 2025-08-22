using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_cinema_challenge.Models
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        [Column("CustomerId")]
        public int Id { get; set; }

        [Required]
        [Column("CustomerName")]
        public required string Name { get; set; }

        [Required]
        [Column("CustomerEmail")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [Column("CustomerPhone")]
        [Phone]
        public required string Phone { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
