namespace Identity.Constants
{
    public struct CodeAndMessage
    {
        public string Code;
        public string Message;
    }

    public static class MessageConstants
    {
        public const string UserIdNotFound = "User Id not found: {0}";
        public const string ExternalLoginNotFound = "External login not found";
        public const string ExternalLoginExists = "External login exists";


        public static readonly CodeAndMessage PropertyTooShort = new()
        {
            Code = "property-too-short",
            Message = "Property '{0}' is too short"
        };
        
        public static readonly CodeAndMessage InvalidUsername = new()
        {
            Code = "invalid-username",
            Message = "Username '{0}' does not meet the validation criteria"
        };
        
        public static readonly CodeAndMessage DuplicateUsername = new()
        {
            Code = "duplicate-username",
            Message = "Username '{0}' already exists"
        };
        
        public static readonly CodeAndMessage InvalidEmail = new()
        {
            Code = "invalid-email",
            Message = "Email '{0}' does not meet the validation criteria"
        };
        
        public static readonly CodeAndMessage DuplicateEmail = new()
        {
            Code = "duplicate-email",
            Message = "Email '{0}' already exists"
        };
    }
}