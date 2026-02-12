using IoTFire.Backend.Api.Models.DTOs.ManagementUsers;
using IoTFire.Backend.Api.Repositories.Interfaces;
using IoTFire.Backend.Api.Services.Interfaces;

namespace IoTFire.Backend.Api.Services.Implementation
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _repo;
        public UserManagementService(IUserRepository repo) => _repo = repo;
        public async Task<IEnumerable<UserAdminDto>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllForAdminAsync();
            return users.Select(MapToAdminDto);
        }
        public async Task<IEnumerable<UserAdminDto>> GetPendingUsersAsync()
        {
            var users = await _repo.GetPendingUsersAsync();
            return users.Select(MapToAdminDto);
        }

        public async Task<IEnumerable<UserAdminDto>> GetSuspendedUsersAsync()
        {
            var users = await _repo.GetSuspendedUsersAsync();
            return users.Select(MapToAdminDto);
        }

        public async Task<AdminActionResponseDto> ValidateUserAsync(int userId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                return Fail("Utilisateur introuvable.");

            if (user.IsActive)
                return Fail("Ce compte est deja valide.");

            user.IsActive = true;         
            user.IsSuspended = false;      
            await _repo.UpdateAsync(user);

            return Ok("Compte valide avec succes.", user);
        }

        public async Task<AdminActionResponseDto> SuspendUserAsync(
            int userId, string reason)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                return Fail("Utilisateur introuvable.");

            if (!user.IsActive)
                return Fail("Impossible de suspendre un compte non valide.");

            if (user.IsSuspended)
                return Fail("Ce compte est deja suspendu.");

            user.IsSuspended = true;   
            user.SuspensionReason = reason;  
            await _repo.UpdateAsync(user);

            return Ok("Compte suspendu avec succes.", user);
        }
        public async Task<AdminActionResponseDto> ReactivateUserAsync(int userId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                return Fail("Utilisateur introuvable.");

            if (!user.IsSuspended)
                return Fail("Ce compte n'est pas suspendu.");

            user.IsSuspended = false; 
            user.SuspensionReason = null;   
            await _repo.UpdateAsync(user);

            return Ok("Compte reactivie avec succes.", user);
        }

        public async Task<AdminActionResponseDto> DeleteUserAsync(int userId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                return Fail("Utilisateur introuvable.");

            user.IsDeleted = true;  
            user.IsActive = false;  
            user.IsSuspended = false;
            await _repo.UpdateAsync(user);

            return Ok("Compte supprime (soft delete) avec succes.", user);
        }
        private static AdminActionResponseDto Ok(
                    string msg, Models.Entities.User? user = null) => new()
                    {
                        Success = true,
                        Message = msg,
                        User = user != null ? MapToAdminDto(user) : null
                    };

        private static AdminActionResponseDto Fail(string msg) => new()
        { Success = false, Message = msg };
        private static UserAdminDto MapToAdminDto(Models.Entities.User u) => new()
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role.ToString(),
            IsActive = u.IsActive,
            IsSuspended = u.IsSuspended,
            SuspensionReason = u.SuspensionReason,
            CreatedAt = u.CreatedAt,
            Statut = u.IsSuspended ? "Suspendu"
                           : u.IsActive ? "Actif"
                           : "En attente"
        };

    }
}
