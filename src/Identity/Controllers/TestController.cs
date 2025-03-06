using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [ApiController] 
    [Authorize]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpPost("[action]")]
        [Authorize()]
        public async Task<bool> TestAuthorisedPostRequest()
        {
            Console.WriteLine(User.IsInRole("Admin"));
            return true;
        }
    }
}
