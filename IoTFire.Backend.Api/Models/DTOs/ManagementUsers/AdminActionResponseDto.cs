namespace IoTFire.Backend.Api.Models.DTOs.ManagementUsers
{
    public class AdminActionResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserAdminDto? User { get; set; } 

    }
}
