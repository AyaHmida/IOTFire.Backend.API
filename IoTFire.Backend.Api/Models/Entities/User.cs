using IoTFire.Backend.Api.Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTFire.Backend.Api.Models.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("last_name")]          // nom → last_name
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("first_name")]         // prenom → first_name
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("phone_number")]       // telephone → phone_number
        public string? PhoneNumber { get; set; }

        [Column("role")]
        public EnumRole Role { get; set; } = EnumRole.Occupant;

        [Column("parent_user_id")]
        public int? ParentUserId { get; set; }

        [ForeignKey("ParentUserId")]
        public User? ParentUser { get; set; }

        [MaxLength(512)]
        [Column("reset_token")]
        public string? ResetToken { get; set; }

        [Column("token_expiration")]
        public DateTime? TokenExpiration { get; set; }

        [Column("is_active")]          // estActive → is_active
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
