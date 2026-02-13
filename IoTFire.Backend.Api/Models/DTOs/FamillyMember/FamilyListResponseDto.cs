namespace IoTFire.Backend.Api.Models.DTOs.FamillyMember
{
    public class FamilyListResponseDto
    {
        public List<FamilyMemberDto> ActiveMembers { get; set; } = new();
        public List<PendingInvitationDto> PendingInvitations { get; set; } = new();
    }

}
