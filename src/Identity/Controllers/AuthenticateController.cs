using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
/*
using PasswordFeature = OSZone.Identity.Features.Authenticate.Password;
using TokenFeature = OSZone.Identity.Features.Authenticate.Token;
using OtpFeature = OSZone.Identity.Features.Authenticate.Otp;
using U2fFeature = OSZone.Identity.Features.Authenticate.U2f;

namespace Identity.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticateController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [Authorize(AuthenticationSchemes = "Identity.Application")]
        [HttpGet("test")]
        public async Task<U2fFeature.Response> Test([FromQuery] U2fFeature.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpPost("password")]
        public async Task<PasswordFeature.Response> Password([FromBody] PasswordFeature.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpPost("token")]
        public async Task<TokenFeature.Response> Token([FromBody] TokenFeature.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpPost("otp")]
        public async Task<OtpFeature.Response> Otp([FromBody] OtpFeature.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpPost("u2f")]
        public async Task<U2fFeature.Response> Authenticator([FromBody] U2fFeature.Request request)
        {
            return await _mediator.Send(request);
        }
    }
}
*/