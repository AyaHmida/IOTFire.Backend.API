namespace IoTFire.Backend.Api.Models.DTOs.FamillyMember
{
    public class PendingInvitationDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
