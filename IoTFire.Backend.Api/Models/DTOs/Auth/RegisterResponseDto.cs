using IoTFire.Backend.Api.Models.Entities.Enums;

namespace IoTFire.Backend.Api.Models.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public EnumRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
