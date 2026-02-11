using IoTFire.Backend.Api.Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace IoTFire.Backend.Api.Models.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name must not exceed 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name must not exceed 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

       
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character.")]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

       
        public EnumRole Role { get; set; } = EnumRole.Occupant;

        public int? ParentUserId { get; set; }
    }
}
