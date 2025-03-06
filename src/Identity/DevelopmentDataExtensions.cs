using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Identity;
using Identity.Repository;
using Identity.Services;
using Microsoft.IdentityModel.Tokens;
using Services;

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
    
    public static IServiceCollection AddDeveloperSigningCredential(this IServiceCollection services, 
        bool persistKey = true, string? filename = null)
    {
        var credentialService = new CredentialsService();
            
        filename ??= Path.Combine(Directory.GetCurrentDirectory(), "tempkey.jwk");
            
        if (File.Exists(filename))
        {
            var jsonWebKey = new JsonWebKey(File.ReadAllText(filename));
            credentialService.Add(jsonWebKey);
        }
        else
        {
            var kid = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16))[..20];
                
            var securityKey = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP384))
            {
                KeyId = kid
            };

            var jsonWebKey = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(securityKey);
            jsonWebKey.Alg = SecurityAlgorithms.EcdsaSha384;
            jsonWebKey.Use = "sig";
                
            if (persistKey)
            {
                File.WriteAllText(filename, JsonSerializer.Serialize(jsonWebKey));
            }
                
            credentialService.Add(jsonWebKey);
        }

        services.AddSingleton<ICredentialsService>(credentialService);
        services.AddSingleton<IPublicCredentialsService, PublicCredentialService>();

        return services;
    }
}
