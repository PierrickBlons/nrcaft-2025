using Microsoft.EntityFrameworkCore;
using Subscription.Api.Data;

namespace Subscription.Api.Services;

public class SubscriberService(HttpClient httpClient, SubscriberDbContext context)
{
    public async Task<Subscriber> CreateSubscriberAsync(string email, string name)
    {
        var existingSubscriber = await context.Subscribers
            .FirstOrDefaultAsync(s => s.Email == email);

        if (existingSubscriber != null)
        {
            return existingSubscriber;
        }

        string ExtractDomain(string e)
        {
            if (string.IsNullOrWhiteSpace(e))
                throw new ArgumentException("Email is required.", nameof(e));

            int at = e.LastIndexOf('@');
            if (at <= 0 || at == e.Length - 1)
                throw new ArgumentException("Invalid email format.", nameof(e));

            var domain = e[(at + 1)..].ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("Email domain is missing.", nameof(e));

            return domain;
        }

        var domain = ExtractDomain(email);
        var isSubscriptionEnabled = await httpClient.GetAsync($"https://api.subscriptions.internal/subscribers/?domain={domain}");

        if (!isSubscriptionEnabled.IsSuccessStatusCode)
        {
            return null;
        }

        var subscriptionDetails = await isSubscriptionEnabled.Content.ReadAsStringAsync();
        if (!subscriptionDetails.Contains("\"status\":\"active\"", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var subscriber = new Subscriber
        {
            Email = email,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        context.Subscribers.Add(subscriber);
        await context.SaveChangesAsync();

        return subscriber;
    }
}

public class Subscriber
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

