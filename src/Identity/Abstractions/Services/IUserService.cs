
using System.Security.Claims;
using Identity.Models.IdentityModels;
using Identity.Models.FindModels;

namespace Identity.Services
{
    public interface IUserService
    {
        Guid? GetUserId(ClaimsPrincipal principal);
        
        Task<User?> FindAsync(ById by, CancellationToken ct = default);
        Task<User?> FindAsync(ByExternalLogin by, CancellationToken ct = default);
        Task<User?> FindAsync(ByUsername by, CancellationToken ct = default);
        Task<User?> FindAsync(ByEmail by, CancellationToken ct = default);
        Task<User?> FindAsync(ByClaimsPrincipal by, CancellationToken ct = default);
        
        Task<bool> IsLockedOutAsync(User user);
        Task<MyIdentityResult> AccessFailedAsync(User user);

        Task<MyIdentityResult> CreateAsync(User user, CancellationToken ct = default);
        
        Task<IList<UserExternalLogin>> GeTUserExternalLoginsAsync(Guid userId, CancellationToken ct = default);
        Task<IList<UserExternalLogin>> GeTUserExternalLoginsAsync(User user, CancellationToken ct = default);
        Task<MyIdentityResult> RemoveExternalLoginAsync(Guid userId, MyUserLoginInfo login);
        Task<MyIdentityResult> AddExternalLoginAsync(Guid userId, MyUserLoginInfo login);

        Task<IList<string>> GetRolesAsync(Guid userId);
    }
}
