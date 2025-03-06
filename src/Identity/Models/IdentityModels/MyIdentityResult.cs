using Identity.Models.Responses;

namespace Identity.Models.IdentityModels
{
    public class MyIdentityResult : IErrorResponse, ISuccessResponse
    {
        private static readonly MyIdentityResult _success = new MyIdentityResult { Success = true };
        private static readonly MyIdentityResult _lockedOut = new MyIdentityResult { IsLockedOut = true };
        private static readonly MyIdentityResult _notAllowed = new MyIdentityResult { IsNotAllowed = true };

        public MyIdentityResult(params IError[] errors) : this(new List<IError>(errors))
        {
        }

        public MyIdentityResult(IList<IError> errors)
        {
            /*
            if (errors == null)
            {
                errors = new[] {Resources.DefaultError};
            }
            */
            Success = false;
            Errors = errors;
        }

        public IList<IError>? Errors { get; set; }
        public bool Success { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsNotAllowed { get; set; }

        public static MyIdentityResult SuccessResult => _success;
        public static MyIdentityResult LockedOutResult => _lockedOut;
        public static MyIdentityResult NotAllowedResult => _notAllowed;

        public static MyIdentityResult FailedResult(IList<IError> errors)
        {
            return new MyIdentityResult(errors);
        }

        public static MyIdentityResult FailedResult(params IError[] errors)
        {
            return new MyIdentityResult(errors);
        }
    }
}
