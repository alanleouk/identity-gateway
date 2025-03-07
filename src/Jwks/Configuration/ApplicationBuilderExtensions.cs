using Jwks;
using Jwks.Constants;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseJwks(this WebApplication app)
        {
            app.MapGet(EndpointConstants.Jwks,
                async (HttpContext context, [FromServices] IMediator mediator, [FromQuery] string? kid = null) =>
                await mediator.Send(new GetJwksFeature.Request { Kid = kid }));
            
            app.MapGet(EndpointConstants.PublicKey,
                async (HttpContext context, [FromServices] IMediator mediator) =>
                    await mediator.Send(new GetPublicKeyFeature.Request()));
        }
    }
}
