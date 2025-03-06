using System.ComponentModel.DataAnnotations;

namespace Identity.Models.AccountModels
{
    public class VerifyCodeModel
    {
        [Required]
        public string? Provider { get; set; }

        [Required]
        public string? Code { get; set; }

        public string? ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
