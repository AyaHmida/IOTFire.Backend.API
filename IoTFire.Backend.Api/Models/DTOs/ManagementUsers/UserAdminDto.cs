namespace IoTFire.Backend.Api.Models.DTOs.ManagementUsers
{
    public class UserAdminDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public string Statut { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsSuspended { get; set; }
        public string? SuspensionReason { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
