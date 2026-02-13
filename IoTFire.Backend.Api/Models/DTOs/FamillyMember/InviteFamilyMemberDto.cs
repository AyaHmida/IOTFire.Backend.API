using System.ComponentModel.DataAnnotations;

namespace IoTFire.Backend.Api.Models.DTOs.FamillyMember
{
    public class InviteFamilyMemberDto
    {
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; }
    }
}
