using IoTFire.Backend.Api.Helpers;
using IoTFire.Backend.Api.Models.DTOs.Auth;
using IoTFire.Backend.Api.Models.Entities;
using IoTFire.Backend.Api.Repositories.Interfaces;
using IoTFire.Backend.Api.Services.Interfaces;

namespace IoTFire.Backend.Api.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {            
            bool emailExists = await _userRepository.EmailExistsAsync(request.Email);
            if (emailExists)
            {
                _logger.LogWarning(
                    "[Register] Email already in use: {Email}", request.Email);
                throw new InvalidOperationException(
                    $"Email '{request.Email}' is already registered.");
            }
            if (request.ParentUserId.HasValue)
            {
                var parentExists = await _userRepository
                    .GetByIdAsync(request.ParentUserId.Value);

                if (parentExists == null)
                {
                    throw new InvalidOperationException(
                        $"Parent user with ID {request.ParentUserId} not found.");
                }
            }
            string hashedPassword = PasswordHelper.HashPassword(request.Password);

            
            var newUser = new User
            {
                LastName = request.LastName.Trim(),
                FirstName = request.FirstName.Trim(),
                Email = request.Email.Trim().ToLower(),
                PasswordHash = hashedPassword,
                PhoneNumber = request.PhoneNumber?.Trim(),
                Role = request.Role,
                ParentUserId = request.ParentUserId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

           
            User createdUser = await _userRepository.CreateAsync(newUser);

            _logger.LogInformation(
                "[Register] New user registered → ID: {Id} | Email: {Email} | Role: {Role}",
                createdUser.Id, createdUser.Email, createdUser.Role);

           
            return new RegisterResponseDto
            {
                Id = createdUser.Id,
                LastName = createdUser.LastName,
                FirstName = createdUser.FirstName,
                Email = createdUser.Email,
                PhoneNumber = createdUser.PhoneNumber,
                Role = createdUser.Role,
                IsActive = createdUser.IsActive,
                CreatedAt = createdUser.CreatedAt
            };
        }
    }
}
