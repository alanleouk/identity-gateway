using System.ComponentModel.DataAnnotations;

namespace Identity.Models.AccountModels
{
    public class ExternalLoginConfirmationModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
