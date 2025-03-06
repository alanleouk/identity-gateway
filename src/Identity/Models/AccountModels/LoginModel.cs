using System.ComponentModel.DataAnnotations;

namespace Identity.Models.AccountModels
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Otp { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        
        public string? CounterSignedAccessKey { get; set; }
        
        public string? PublicKey { get; set; }
    }
}
