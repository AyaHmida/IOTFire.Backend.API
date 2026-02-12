using IoTFire.Backend.Api.Models.DTOs.ManagementUsers;
using IoTFire.Backend.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IoTFire.Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserManagementService _service;
        public AdminController(IUserManagementService service)
            => _service = service;

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllUsersAsync();
            return Ok(users);
        }
        // Retourne les comptes en attente de validation
        [HttpGet("users/pending")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _service.GetPendingUsersAsync();
            return Ok(users);
        }
        // Retourne les comptes suspendus
        [HttpGet("users/suspended")]
        public async Task<IActionResult> GetSuspendedUsers()
        {
            var users = await _service.GetSuspendedUsersAsync();
            return Ok(users);
        }
        //valider compte utilisateur
        [HttpPatch("users/{id}/validate")]
        public async Task<IActionResult> ValidateUser(int id)
        {
            var result = await _service.ValidateUserAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        //suspended un utilisateur
        [HttpPatch("users/{id}/suspend")]
        public async Task<IActionResult> SuspendUser(
                    int id, [FromBody] SuspendUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SuspendUserAsync(id, dto.Reason);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        //reactive un utilisateur
        [HttpPatch("users/{id}/reactivate")]
        public async Task<IActionResult> ReactivateUser(int id)
        {
            var result = await _service.ReactivateUserAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _service.DeleteUserAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }



    }
}
