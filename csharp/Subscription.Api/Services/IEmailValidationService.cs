namespace Subscription.Api.Services;

public interface IEmailValidationService
{
    Task<bool> IsEmailSafeAsync(string email);
}
