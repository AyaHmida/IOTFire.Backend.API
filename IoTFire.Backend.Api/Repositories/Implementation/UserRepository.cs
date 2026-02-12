using IoTFire.Backend.Api.Data;
using IoTFire.Backend.Api.Models.Entities;
using IoTFire.Backend.Api.Models.Entities.Enums;
using IoTFire.Backend.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace IoTFire.Backend.Api.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower().Trim());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim());
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> CreateAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();  // INSERT INTO users (...)

            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users.Where(u => u.IsActive).ToListAsync();
        public async Task<IEnumerable<User>> GetAllForAdminAsync()
            => await _context.Users
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        public async Task<IEnumerable<User>> GetPendingUsersAsync()
            => await _context.Users
                .Where(u => !u.IsActive && !u.IsDeleted && u.Role != EnumRole.Admin)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();
        public async Task<IEnumerable<User>> GetActiveUsersAsync()
            => await _context.Users
                .Where(u => u.IsActive && !u.IsSuspended && !u.IsDeleted)
                .ToListAsync();

        public async Task<IEnumerable<User>> GetSuspendedUsersAsync()
            => await _context.Users
                .Where(u => u.IsSuspended && !u.IsDeleted)
                .ToListAsync();


    }

}

