using System.ComponentModel.DataAnnotations;

namespace IoTFire.Backend.Api.Models.DTOs.FamillyMember
{
    public class AcceptInvitationDto
    {
        [Required(ErrorMessage = "Le token est obligatoire.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string FirstName { get; set; }

        [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [MinLength(8, ErrorMessage = "Minimum 8 caractères.")]
        public string PasswordHash { get; set; }

  
    }

}
