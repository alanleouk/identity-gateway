namespace Identity.Models.Responses
{
    public interface IError
    {
        string? ErrorCode { get; set; }
        string? ErrorMessage { get; set; }
    }
}
