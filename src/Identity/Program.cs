using System.Net;
using Identity;
using Identity.Configuration;
using Identity.Factories;
using Identity.Repository;
using Identity.Services;
using Identity.Validators;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection.Features;
// ReSharper disable InconsistentNaming

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// TODO: Move to config and use when signing, etc
var endpoints = new List<string>
{
    "https://localhost",
    "https://local-5.dev.alanleouk.net"
};

// TODO: Move to config and use in CounterSignedAccessKeyLogin services
var trustedKeys = new List<string>
{
    "https://local-5.dev.alanleouk.net/.well-known/openid-configuration/jwks?kid=07Zb%2FMNJU4UtG3GLJbm7"
};

#if DEBUG
// IdentityModelEventSource.ShowPII = true;
#endif

#if DEBUG
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    var kestrelSection = context.Configuration.GetSection("Kestrel");

    serverOptions.Configure(kestrelSection)
        .Endpoint("Https", listenOptions =>
        {
            if (listenOptions.ListenOptions.IPEndPoint != null)
            {
                listenOptions.ListenOptions.IPEndPoint.Address = IPAddress.Parse("127.0.0.5");
            }
        });
});
#endif

services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Data Protection (DEBUG)
services.AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo($@"{configuration["dataProtection:securePath"]}Keys.IdentityGateway/"));

// Database
services.AddDbContext<IdentityDbContext>(o =>
    o.UseSqlServer(configuration.GetConnectionString("PrimaryConnection"),
        x => x.MigrationsAssembly("Identity")));

// Cors
var origins = endpoints
    .Select(item => item)
    .ToList();
origins.Add("https://jwt.io");
services.AddCors(o =>
{
    o.AddPolicy("AllowLimited",
        corsBuilder =>
        {
            corsBuilder
                .WithOrigins(origins.ToArray())
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Authentication
var authenticationBuilder = services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

authenticationBuilder.AddCookie(o =>
{
    o.LoginPath = new PathString("/Account/Login");
    o.Cookie.HttpOnly = true;
    o.AccessDeniedPath = "/Account/AccessDenied";
    o.Cookie.Name = IdentityConstants.ApplicationScheme;
    o.ExpireTimeSpan = TimeSpan.FromDays(21);
    o.LoginPath = "/Account/Login";
    o.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    o.SlidingExpiration = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    o.Cookie.SameSite = SameSiteMode.Strict;
});

var googleClientId = configuration.GetValue<string>("Auth:Oauth:Google:ClientId");
var googleClientSecret = configuration.GetValue<string>("Auth:Oauth:Google:ClientSecret");
if (googleClientId != null && googleClientSecret != null)
{
    authenticationBuilder.AddGoogle(o =>
    {
        o.ClientId = googleClientId;
        o.ClientSecret = googleClientSecret;
        o.Scope.Clear();
        o.Scope.Add("openid");
        o.Scope.Add("email");
    });
}

// Authorisation Policies
services.AddAuthorization(o =>
{
    o.AddPolicy("CookiesScheme",
        new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme)
            .RequireAuthenticatedUser()
            .Build());
});

// MediatR
services.MyAddMediatR("Identity");

// Controllers
var mvcBuilder = services.AddMvc(o => o.EnableEndpointRouting = false);
#if DEBUG
mvcBuilder.AddRazorRuntimeCompilation();
#endif

// Other Services
services.AddSingleton<ITokenService, TokenService>();
services.AddScoped<IUserContextService, UserContextService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<ILoginService, LoginService>();
services.AddScoped<IIdentityValidator<User>, UserValidator>();
services.AddSingleton<IUserClaimsPrincipalFactory, UserClaimsPrincipalFactory>();

// TODO: Add credentials from KeyVault
services.AddDeveloperSigningCredential();

// App
    
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowLimited");
var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};
app.UseCookiePolicy(cookiePolicyOptions);
app.UseAuthentication();
app.UseAuthorization();
// app.UseOSZoneApis();
#if DEBUG
app.MigrateDatabase<IdentityDbContext>();
app.AddDevelopmentData();
#endif

// Features
app.MapGet(EndpointConstants.Configuration,
    async (HttpContext context, [FromServices] IMediator mediator) =>
    await mediator.Send(new GetConfigurationFeature.Request()));

app.MapGet(EndpointConstants.Jwks,
    async (HttpContext context, [FromServices] IMediator mediator, [FromQuery] string? kid = null) =>
    await mediator.Send(new GetJwksFeature.Request { Kid = kid }));

app.MapPost(EndpointConstants.Token,
    async (HttpContext context, [FromServices] IMediator mediator,
        [FromForm] PostTokenFeature.Request request) => await mediator.Send(request));

app.MapGet(EndpointConstants.Authorization, async (HttpContext context, [FromServices] IMediator mediator,
    string? response_type, string? response_mode, string? scope, string? client_id, string? state, string? redirect_uri,
    string? nonce, string? request_uri) =>
{
    var request = new GetAuthorizeFeature.Request
    {
        ResponseType = response_type,
        ResponseMode = response_mode,
        Scope = scope,
        ClientId = client_id,
        State = state,
        RedirectUri = redirect_uri,
        Nonce = nonce,
        RequestUri = request_uri
    };

    await mediator.Send(request);
});

app.MapGet(EndpointConstants.Userinfo,
    [Authorize, AllowAnonymous] async (HttpContext context, [FromServices] IMediator mediator) =>
    await mediator.Send(new GetUserInfoFeature.Request()));

app.MapGet(EndpointConstants.EndSession, async (HttpContext context, [FromServices] IMediator mediator,
    string? id_token_hint, string? post_logout_redirect_uri, string? state) =>
{
    var request = new GetEndSessionFeature.Request
    {
        IdTokenHint = id_token_hint,
        PostLogoutRedirectUri = post_logout_redirect_uri,
        State = state
    };

    await mediator.Send(request);
});

app.MapPost(EndpointConstants.Introspection,
    async (HttpContext context, [FromServices] IMediator mediator,
        [FromForm] PostIntrospectionFeature.Request request) => await mediator.Send(request));

app.MapPost(EndpointConstants.Revocation,
    async (HttpContext context, [FromServices] IMediator mediator,
        [FromForm] PostRevocationFeature.Request request) => await mediator.Send(request));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
