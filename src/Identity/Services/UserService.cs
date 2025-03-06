using System.Globalization;
using System.Security.Claims;
using Identity;
using Identity.Constants;
using Identity.Models.IdentityModels;
using Identity.Models.FindModels;
using Identity.Repository;
using Identity.Validators;
using Microsoft.EntityFrameworkCore;

// See: https://github.com/aspnet/Identity/blob/master/src/Core/UserManager.cs
// See: https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/UserManager.cs
// See: https://github.com/dotnet/aspnetcore/blob/a899ce80c36138df814b3258093a21de3ca95c5c/src/Identity/EntityFrameworkCore/src/UserStore.cs#L609
// See: https://github.com/dotnet/aspnetcore/blob/a899ce80c36138df814b3258093a21de3ca95c5c/src/Identity/EntityFrameworkCore/src/UserOnlyStore.cs#L477

namespace Identity.Services
{
    public class UserService : IUserService
    {
        public int MaxFailedAccessAttemptsBeforeLockout { get; set; } = 3;
        public TimeSpan DefaultAccountLockoutTimeSpan { get; set; } = TimeSpan.Zero;

        private readonly IdentityDbContext _db;
        private readonly IIdentityValidator<User> _userValidator;

        // private readonly IUserTokenProvider<TUser> _userTokenProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IdentityDbContext db,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userValidator = new UserValidator(this);
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual async Task<bool> IsLockedOutAsync(User user)
        {
            if (user == null)
            {
                throw new InvalidOperationException("User cannot be null");
            }

            if (!user.LockoutEnabled)
            {
                return await Task.FromResult(false);
            }

            var lockoutTime = user.LockoutEndUtc;
            var result = lockoutTime >= DateTimeOffset.UtcNow;

            return await Task.FromResult(result);
        }

        public virtual async Task<MyIdentityResult> AccessFailedAsync(User user)
        {
            if (user == null)
            {
                throw new InvalidOperationException("User cannot be null");
            }

            var count = user.AccessFailedCount++;
            if (count >= MaxFailedAccessAttemptsBeforeLockout)
            {
                user.LockoutEndUtc = DateTime.UtcNow.Add(DefaultAccountLockoutTimeSpan);
                user.AccessFailedCount = 0;
            }

            await _db.SaveChangesAsync();
            return MyIdentityResult.SuccessResult;
        }

        public virtual Guid? GetUserId(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }

            return null;
        }

        public virtual async Task<User?> FindAsync(ById by, CancellationToken ct = default)
        {
            return await _db.Users.FirstOrDefaultAsync(item => item.Id == by.Id, ct);
        }

        public async Task<User?> FindAsync(ByExternalLogin by, CancellationToken ct = default)
        {
            var userLogin =
                await _db.ExternalLogins
                    .Include(item => item.User).FirstOrDefaultAsync(
                        item => item.Provider == by.Provider && item.Key == by.Key, ct);

            if (userLogin != null)
            {
                return await _db.Users.FirstOrDefaultAsync(item => item.Id == userLogin.User.Id, ct);
            }

            return null;
        }

        public virtual async Task<User?> FindAsync(ByUsername by, CancellationToken ct = default)
        {
            if (by.Username == null)
            {
                throw new ArgumentNullException(nameof(by.Username));
            }

            return await _db.Users.FirstOrDefaultAsync(item => item.Username == by.Username, ct);
        }

        public virtual async Task<User?> FindAsync(ByEmail by, CancellationToken ct = default)
        {
            if (by.Email == null)
            {
                throw new ArgumentNullException(nameof(by.Email));
            }

            return await _db.Users.FirstOrDefaultAsync(item => item.Email == by.Email, ct);
        }

        public virtual async Task<User?> FindAsync(ByClaimsPrincipal by, CancellationToken ct = default)
        {
            if (by.Principal == null)
            {
                throw new ArgumentNullException(nameof(by.Principal));
            }

            var id = GetUserId(by.Principal);
            if (id == null)
            {
                return await Task.FromResult<User?>(null);
            }

            return await FindAsync(new ById(id.Value), ct);
        }

        public virtual async Task<MyIdentityResult> CreateAsync(User user, CancellationToken ct = default)
        {
            user.SecurityStamp = NewSecurityStamp();

            var result = await _userValidator.ValidateAsync(user, ct);
            if (!result.Success)
            {
                return result;
            }
            
            await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
            return MyIdentityResult.SuccessResult;
        }

        private static string NewSecurityStamp()
        {
            return Guid.NewGuid().ToString();
        }

        public virtual async Task<IList<UserExternalLogin>> GeTUserExternalLoginsAsync(Guid userId,
            CancellationToken ct = default)
        {
            return await _db.ExternalLogins
                .Where(item => item.User.Id == userId)
                .ToListAsync(ct);
        }

        public virtual async Task<IList<UserExternalLogin>> GeTUserExternalLoginsAsync(User user,
            CancellationToken ct = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.ExternalLogins == null)
            {
                await _db.Entry(user).Reference(u => u.ExternalLogins).LoadAsync(ct);
            }

            if (user.ExternalLogins == null)
            {
                throw new InvalidOperationException("Failed to load external logins");
            }

            return user.ExternalLogins;
        }

        public virtual async Task<MyIdentityResult> RemoveExternalLoginAsync(Guid userId, MyUserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var user = await FindAsync(new ById { Id = userId });
            if (user == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    MessageConstants.UserIdNotFound,
                    userId));
            }

            var externalLogin =
                await _db.ExternalLogins.FirstOrDefaultAsync(
                    item => item.User == user && item.Provider == login.LoginProvider && item.Key == login.ProviderKey);

            if (externalLogin == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    MessageConstants.ExternalLoginNotFound));
            }

            _db.ExternalLogins.Remove(externalLogin);
            user.SecurityStamp = NewSecurityStamp();
            await _db.SaveChangesAsync();
            return MyIdentityResult.SuccessResult;
        }

        public virtual async Task<MyIdentityResult> AddExternalLoginAsync(Guid userId, MyUserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var user = await FindAsync(new ById { Id = userId });
            if (user == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    MessageConstants.UserIdNotFound,
                    userId));
            }

            var externalLogin =
                await _db.ExternalLogins.FirstOrDefaultAsync(
                    item => item.User == user && item.Provider == login.LoginProvider && item.Key == login.ProviderKey);

            if (externalLogin != null)
            {
                return MyIdentityResult.FailedResult();
            }

            externalLogin = new UserExternalLogin
            {
                Key = login.ProviderKey,
                Name = login.DisplayName,
                Provider = login.LoginProvider,
                User = user
            };
            await _db.ExternalLogins.AddAsync(externalLogin);
            user.SecurityStamp = NewSecurityStamp();
            await _db.SaveChangesAsync();
            return MyIdentityResult.SuccessResult;
        }

        public virtual async Task<IList<string>> GetRolesAsync(Guid userId)
        {
            return await _db.Users
                .Include(u => u.Claims)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Claims)
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(r => r.Value)
                .ToListAsync();
        }
    }
}
