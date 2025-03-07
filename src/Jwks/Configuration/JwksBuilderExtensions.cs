using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;
using Jwks.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JwksBuilderExtensions
    {
        public static IJwksBuilder AddDeveloperSigningCredential(this IJwksBuilder builder, 
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

            builder.Services.AddSingleton<ICredentialsService>(credentialService);
            builder.Services.AddSingleton<IPublicCredentialsService, PublicCredentialService>();

            return builder;
        }
    }
}
