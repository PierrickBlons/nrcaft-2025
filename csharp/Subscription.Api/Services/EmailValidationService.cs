namespace Subscription.Api.Services;

public class EmailValidationService(HttpClient httpClient, ILogger<EmailValidationService> logger) : IEmailValidationService
{
    public async Task<bool> IsEmailSafeAsync(string email)
    {
        try
        {
            logger.LogInformation("Validating email safety for: {Email}", email);
            var response = await httpClient.GetAsync($"/validate?email={Uri.EscapeDataString(email)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Email validation response: {Response}", content);

                return content.Contains("\"safe\":true", StringComparison.OrdinalIgnoreCase);
            }

            logger.LogWarning("Email validation service returned status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating email safety for: {Email}", email);
            return false;
        }
    }
}
