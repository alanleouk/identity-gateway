using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Constants;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Identity.Services
{
	public interface ITokenService
	{
		string? CreateIdToken(TokenServiceRequest request, string issuerUri, 
			int tokenExpirySeconds = 3600,
			string? accessToken = null);
		
		string? CreateAccessToken(TokenServiceRequest request, string issuerUri, 
			int tokenExpirySeconds = 3600);
		JwtSecurityToken? ValidateToken(string token, string issuerUri);
	}

	// AccessTokenHash
	// auth_time
	// idp
	// role
	// scope
	// amr

	public class TokenService : ITokenService
	{
		public readonly ICredentialsService _credentialsService;

		public TokenService(ICredentialsService credentialsService)
		{
			_credentialsService = credentialsService;
		}

		public virtual string? CreateIdToken(TokenServiceRequest request, string issuerUri, int tokenExpirySeconds, string? accessToken)
		{
			var securityKey = _credentialsService.JsonWebKeys().FirstOrDefault();
			if (securityKey == null)
			{
				return null;
			}

			// TODO: Auth Time

			var tokenHandler = new JsonWebTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				TokenType = "JWT",
				Issuer = issuerUri,
				Expires = DateTime.UtcNow.AddSeconds(tokenExpirySeconds),
				SigningCredentials = new SigningCredentials(securityKey, securityKey.Alg),
				Claims = new Dictionary<string, object>()
			};

			AddSubject(request, tokenDescriptor);
			AddEmail(request, tokenDescriptor);
			AddNonce(request, tokenDescriptor);

			if (request.ClientId != null)
			{
				tokenDescriptor.Claims.Add(JwtClaimTypes.Audience, request.ClientId);
			}

			if (accessToken != null)
			{
				AddTokenHash(tokenDescriptor, JwtClaimTypes.AccessTokenHash, accessToken);
			}

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return token;
		}

		public virtual string? CreateAccessToken(TokenServiceRequest request, string issuerUri, int tokenExpirySeconds)
		{
			var securityKey = _credentialsService.JsonWebKeys().FirstOrDefault();
			if (securityKey == null)
			{
				return null;
			}

			// TODO: Auth Time
			
			var tokenHandler = new JsonWebTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				TokenType = "at+jwt",
				Issuer = issuerUri,
				Expires = DateTime.UtcNow.AddSeconds(tokenExpirySeconds),
				SigningCredentials = new SigningCredentials(securityKey, securityKey.Alg),
				Claims = new Dictionary<string, object>()
			};

			AddClientId(request, tokenDescriptor);
			AddSubject(request, tokenDescriptor);
			AddEmail(request, tokenDescriptor);
			
			if (request.Audience != null)
			{
				tokenDescriptor.Claims.Add(JwtClaimTypes.Audience, request.Audience);
			}

			if (request.Roles != null)
			{
				tokenDescriptor.Claims.Add(JwtClaimTypes.Role, request.Roles);
			}

			if (request.Scopes != null)
			{
				tokenDescriptor.Claims.Add(JwtClaimTypes.Scope, request.Scopes);
			}

			tokenDescriptor.Claims.Add(JwtClaimTypes.AuthenticationMethod, new[] { "external" });

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return token;
		}

		public JwtSecurityToken? ValidateToken(string token, string issuerUri)
		{
			var securityToken = ReadToken(token);
			if (securityToken == null)
			{
				return null;
			}

			var keyId = securityToken.Header.FirstOrDefault(item => item.Key == "kid").Value?.ToString();
			if (keyId == null)
			{
				return null;
			}

			var securityKey = _credentialsService.Find(keyId);
			if (securityKey == null)
			{
				return null;
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = false, // TODO: Is this required?
					ValidIssuer = issuerUri,
					ValidAudience = null,
					IssuerSigningKey = securityKey
				}, out SecurityToken validatedToken);

				return (JwtSecurityToken)validatedToken;
			}
			catch
			{
				return null;
			}
		}

		public JwtSecurityToken? ReadToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			return tokenHandler.ReadToken(token) as JwtSecurityToken;
		}

		private void AddTokenHash(SecurityTokenDescriptor tokenDescriptor, string type, string value)
		{
			var hashAlgorithm = tokenDescriptor.SigningCredentials.Algorithm switch
			{
				"ES256" => HashAlgorithmName.SHA256,
				"RS256" => HashAlgorithmName.SHA256,
				"ES384" => HashAlgorithmName.SHA384,
				"RS384" => HashAlgorithmName.SHA384,
				"ES521" => HashAlgorithmName.SHA512,
				"RS521" => HashAlgorithmName.SHA512,
				_ => throw new NotSupportedException()
			};
			
			using (var alg = HashAlgorithm.Create(hashAlgorithm.Name))
			{
				var hash = alg.ComputeHash(Encoding.ASCII.GetBytes(value));
				var size = (alg.HashSize / 8) / 2;

				var leftPart = new byte[size];
				Array.Copy(hash, leftPart, size);

				tokenDescriptor.Claims.Add(type, Base64UrlEncoder.Encode(leftPart));
			}
		}

		private void AddSubject(TokenServiceRequest request, SecurityTokenDescriptor tokenDescriptor)
		{
			if (request.Subject != null)
			{
				tokenDescriptor.Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim("sub", request.Subject.ToString().ToLower()),
				});
			}
		}

		private void AddClientId(TokenServiceRequest request, SecurityTokenDescriptor tokenDescriptor)
		{
			if (request.ClientId != null)
			{
				tokenDescriptor.Claims.Add(JwtClaimTypes.ClientId, request.ClientId);
			}
		}

		private void AddEmail(TokenServiceRequest request, SecurityTokenDescriptor tokenDescriptor)
		{
			if (request.Email != null)
			{
				tokenDescriptor.Claims.Add(JwtClaimTypes.Email, request.Email);
			}
		}

		private void AddNonce(TokenServiceRequest request, SecurityTokenDescriptor tokenDescriptor)
		{
			if (request.Nonce != null)
			{
				tokenDescriptor.Claims.Add(JwtClaimTypes.Nonce, request.Nonce);
			}
		}
	}
}
