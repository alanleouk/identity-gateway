namespace Identity.Models.Responses
{
    public interface IErrorResponse
    {
        IList<IError>? Errors { get; set; }
    }
}
