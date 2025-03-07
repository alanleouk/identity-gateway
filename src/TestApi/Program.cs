using Authority.Services;
using DebugHelper;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Authority Service
var authorityConfig = new AuthorityConfig(configuration.GetSection("Authority"));
services.AddSingleton(authorityConfig);

#if DEBUG
builder.DebugOnKestrel(authorityConfig.PrimaryAuthority);
#endif

// Jwks
services.AddJwksServices()
    .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseHttpsRedirection();

// Features
app.UseJwks();

app.Run();
