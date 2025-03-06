using System.Net.Mail;
using System.Text.RegularExpressions;
using Identity.Constants;
using Identity.Models.IdentityModels;
using Identity.Models.FindModels;
using Identity.Models.Responses;
using Identity.Services;

namespace Identity.Validators
{
    public class UserValidator : IIdentityValidator<User>
    {
        private readonly IUserService _userService;
        
        public bool AllowOnlyAlphanumericUserNames { get; set; }

        public UserValidator(IUserService userService)
        {
            _userService = userService;
            AllowOnlyAlphanumericUserNames = true;
        }

        public async Task<MyIdentityResult> ValidateAsync(User item, CancellationToken ct = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var errors = new List<IError>();
            
            await ValidateUsernameAsync(item, errors, ct);
            await ValidateEmailAsync(item, errors, ct);

            if (errors.Count > 0)
            {
                return await Task.FromResult(MyIdentityResult.FailedResult(errors));
            }

            return MyIdentityResult.SuccessResult;
        }
        
        private async Task ValidateUsernameAsync(User user, IList<IError> errors, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                errors.Add(IdentityError.CodeAndMessage(MessageConstants.PropertyTooShort, nameof(user.Username)));
            }
            else if (AllowOnlyAlphanumericUserNames && !Regex.IsMatch(user.Username, @"^[A-Za-z0-9@_\.]+$"))
            {
                // If any characters are not letters or digits, its an illegal user name
                errors.Add(IdentityError.CodeAndMessage(MessageConstants.InvalidUsername, nameof(user.Username)));
            }
            else
            {
                var owner = await _userService.FindAsync(new ByUsername(user.Username), ct);
                if (owner != null && owner.Id != user.Id)
                {
                    errors.Add(IdentityError.CodeAndMessage(MessageConstants.DuplicateUsername, nameof(user.Username)));
                }
            }
        }
        
        private async Task ValidateEmailAsync(User user, IList<IError> errors, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                errors.Add(IdentityError.CodeAndMessage(MessageConstants.PropertyTooShort, nameof(user.Email)));
                return;
            }
            
            try
            {
                var m = new MailAddress(user.Email);
            }
            catch (FormatException)
            {
                errors.Add(IdentityError.CodeAndMessage(MessageConstants.InvalidEmail, nameof(user.Email)));
                return;
            }
            
            var owner = await _userService.FindAsync(new ByEmail(user.Email), ct);
            if (owner != null && owner.Id != user.Id)
            {
                errors.Add(IdentityError.CodeAndMessage(MessageConstants.DuplicateEmail, nameof(user.Email)));
            }
        }
    }
}