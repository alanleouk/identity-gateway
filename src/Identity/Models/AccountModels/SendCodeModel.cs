using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.Models.AccountModels
{
    public class SendCodeModel
    {
        public string? SelectedProvider { get; set; }

        public ICollection<SelectListItem>? Providers { get; set; }

        public string? ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}
