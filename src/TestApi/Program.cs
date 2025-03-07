using Authority.Services;
using DebugHelper;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Authority Service
var authorityService = new AuthorityService(configuration.GetSection("Authority"));
services.AddSingleton(authorityService);

#if DEBUG
builder.DebugOnKestrel(authorityService.PrimaryAuthority);
#endif

// Jwks
services.AddJwksServices()
    .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseHttpsRedirection();

// Features
app.UseJwks();

app.Run();
