using System.ComponentModel.DataAnnotations;

namespace IoTFire.Backend.Api.Models.DTOs.ManagementUsers
{
    public class SuspendUserDto
    {
        [Required(ErrorMessage = "La raison est obligatoire.")]
        [MinLength(10, ErrorMessage = "Raison trop courte (min 10 caracteres).")]
        public string Reason { get; set; } = string.Empty;

    }
}
