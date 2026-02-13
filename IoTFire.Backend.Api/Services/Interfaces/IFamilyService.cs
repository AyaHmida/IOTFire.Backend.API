using IoTFire.Backend.Api.Models.DTOs.FamillyMember;

namespace IoTFire.Backend.Api.Services.Interfaces
{
    public interface IFamilyService
    {
      
        Task<(bool Success, string Message)> InviteMemberAsync(int occupantId, InviteFamilyMemberDto dto);

       
        Task<FamilyListResponseDto> GetFamilyMembersAsync(int occupantId);

        Task<(bool Success, string Message)> RevokeMemberAsync(int occupantId, int memberId);

       
        Task<(bool Valid, string Email)> ValidateInvitationTokenAsync(string token);

 
        Task<(bool Success, string Message)> AcceptInvitationAsync(AcceptInvitationDto dto);
    }
}
