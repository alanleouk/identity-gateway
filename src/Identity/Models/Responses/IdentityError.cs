using System.Globalization;
using Identity.Constants;

namespace Identity.Models.Responses
{
    public class IdentityError : IError
    {
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }

        public static IdentityError CodeAndMessage(CodeAndMessage config, params object?[] args)
        {
            return new IdentityError
            {
                ErrorCode = config.Code,
                ErrorMessage = string.Format(CultureInfo.CurrentCulture, config.Message, args)
            };
        }
    }
}