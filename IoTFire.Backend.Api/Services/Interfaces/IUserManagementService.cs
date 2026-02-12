using IoTFire.Backend.Api.Models.DTOs.ManagementUsers;

namespace IoTFire.Backend.Api.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserAdminDto>> GetAllUsersAsync();
        Task<IEnumerable<UserAdminDto>> GetPendingUsersAsync();
        Task<IEnumerable<UserAdminDto>> GetSuspendedUsersAsync();

        Task<AdminActionResponseDto> ValidateUserAsync(int userId);
        Task<AdminActionResponseDto> SuspendUserAsync(int userId, string reason);
        Task<AdminActionResponseDto> ReactivateUserAsync(int userId);
        Task<AdminActionResponseDto> DeleteUserAsync(int userId);

    }
}
