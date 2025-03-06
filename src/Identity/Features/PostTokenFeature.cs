using Identity.Models.FindModels;
using Identity.Models.Responses;
using Identity.Repository;
using Identity.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection.Features;

// See: https://datatracker.ietf.org/doc/html/rfc8693

public class PostTokenFeature
{
    public class Request : IRequest<Response>
    {
        public string? Key { get; set; }
    }

    public class Response : ISuccessResponse, IErrorResponse
    {
        public bool Success { get; set; }
        public IList<IError>? Errors { get; set; }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IdentityDbContext _db;
        private readonly IUserContextService _userContextService;
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;

        public Handler(
            IdentityDbContext db,
            IUserContextService userContextService,
            IUserService userService,
            ILoginService loginService)
        {
            _db = db;
            _userContextService = userContextService;
            _userService = userService;
            _loginService = loginService;
        }

        public async Task<Response> Handle(Request request, CancellationToken ct)
        {
            var response = new Response();

            var userId = _userContextService.UserId;
            var token = await _db.Tokens
                .Where(e => e.User.Id == userId && e.Key == request.Key)
                .Where(e => e.IsEnabled)
                .Where(e => e.RemainingUses == null || e.RemainingUses > 0)
                .Where(e => e.ExpiryDateUtc == null || e.ExpiryDateUtc < DateTime.UtcNow)
                .FirstOrDefaultAsync(ct);

            var user = await _userService.FindAsync(new ById(userId.Value), ct);
            if (user != null)
            {
                if (token != null)
                {
                    if (token.RemainingUses != null)
                    {
                        token.RemainingUses = token.RemainingUses--;
                        await _db.SaveChangesAsync(ct);
                    }

                    var setAuthClaimResult = await _loginService.SetAuthenticationClaim(user, "token");
                    response.Success = setAuthClaimResult.Success;
                }
            }

            await Task.Delay(Random.Shared.Next(500, 1500));

            return response;
        }
    }
}
