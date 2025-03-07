using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Identity;
using Identity.Repository;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class DevelopmentDataExtensions
{
    public static void AddDevelopmentData(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            AddUserAlan(db);
        }
    }

    private static void AddUserAlan(IdentityDbContext db)
    {
        var id = new Guid("175DA634-DBBD-4CFA-a925-5E6832754708");
        var user = db.Users.FirstOrDefault(item => item.Id == id);
        if (user == null)
        {
            db.Users.Add(new User
            {
                Id = id,
                Username = "alanleouk",
                Email = "alanleouk@gmail.com",
                EmailConfirmed = true,
                LockoutEnabled = true,
                Claims = new List<UserClaim>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Type = ClaimsIdentity.DefaultRoleClaimType,
                        Value = "Super"
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Type = ClaimsIdentity.DefaultRoleClaimType,
                        Value = "Admin"
                    }
                },
                ExternalLogins = new List<UserExternalLogin>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Key = "117718877492411497510",
                        Provider = "Google",
                        Name = "Google (Alan)",
                    }
                }
            });

            db.SaveChanges();
        }
    }
}
