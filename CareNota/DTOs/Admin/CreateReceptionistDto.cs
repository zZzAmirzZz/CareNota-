using System.ComponentModel.DataAnnotations;

namespace CareNota.DTOs.Admin
{
    public class CreateReceptionistDto
    {
        [Required] public string FullName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string PhoneNumber { get; set; }
    }
}
