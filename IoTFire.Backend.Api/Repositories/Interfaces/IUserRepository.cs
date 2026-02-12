using IoTFire.Backend.Api.Models.Entities;

namespace IoTFire.Backend.Api.Repositories.Interfaces
{
    public interface IUserRepository
    { 
        Task<bool> EmailExistsAsync(string email);     
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);  
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();

        Task<IEnumerable<User>> GetAllForAdminAsync();   
        Task<IEnumerable<User>> GetPendingUsersAsync();
        Task<IEnumerable<User>> GetActiveUsersAsync();   
        Task<IEnumerable<User>> GetSuspendedUsersAsync(); 

    }
}
