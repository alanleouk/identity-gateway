using Identity.Models.IdentityModels;

namespace Identity.Validators
{
    public interface IIdentityValidator<in T>
    {
        Task<MyIdentityResult> ValidateAsync(T item, CancellationToken ct = default);
    }
}