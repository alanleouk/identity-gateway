using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Identity.Models;
using Identity.Repository;
using Identity.Services;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly IdentityDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IdentityDbContext db, ITokenService tokenService, IUserService userService, ILogger<HomeController> logger)
        {
            _db = db;
            _tokenService = tokenService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                return View();
            }

            var user = await _db.Users
                .Include(item => item.Claims)
                .FirstOrDefaultAsync(item => item.Id == userId);
            if (user == null)
            {
                return View();
            }       

            var tokenServiceRequest = new TokenServiceRequest
            {
                Audience = new List<string>() { "identity_gateway" },
                ClientId = "identity",
                Nonce = "Test Nonce",
                Email = user.Email,
                Subject = userId.Value.ToString().ToLower(),
                Roles = user.Claims.Where(item => item.Type == ClaimTypes.Role).Select(item => item.Value).ToList(),
                Scopes = user.Claims.Where(item => item.Type == "scope").Select(item => item.Value).ToList(),
                PublicKey = User.Claims.FirstOrDefault(item => item.Type == "public_key")?.Value
            };

            var token = _tokenService.CreateAccessToken(tokenServiceRequest, "https://local-5.dev.alanleouk.net");

            ViewData["Jwt"] = token;


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}