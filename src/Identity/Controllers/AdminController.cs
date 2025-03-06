using Identity.Repository;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdentityDbContext _db;
        private readonly ILoginService _loginService;

        public AdminController(
            IHttpContextAccessor httpContextAccessor, IdentityDbContext db, 
            ILoginService loginService)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
            _loginService = loginService;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public bool Ping()
        {
            return true;
        }

        [HttpGet("[action]")]
        [Authorize]
        public bool None()
        {
            var isInRoleAdmins = User.IsInRole("admins");
            Console.Write(isInRoleAdmins);
            return true;
        }
        
        [HttpGet("[action]")]
        [Authorize]
        public IEnumerable<KeyValuePair<string, string>> Claims()
        {
            return _httpContextAccessor.HttpContext.User.Claims.Select(claim =>
                new KeyValuePair<string, string>(claim.Type, claim.Value));
        }

        [HttpGet("[action]")]
        [Authorize("CookiesScheme")]
        public bool CookiesScheme()
        {
            return true;
        }
        
        [HttpGet("[action]")]
        [AllowAnonymous]
        public string? CheckIp()
        {
            return _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        [AllowAnonymous]
        [HttpGet("migratedatabase")]
        public async Task<IActionResult> MigrateDatabase()
        {
            await _db.Database.MigrateAsync();
            await _db.SaveChangesAsync();
            
            return new ObjectResult(true);
        }
    }
}
