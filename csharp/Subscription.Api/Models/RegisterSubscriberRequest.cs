using System.ComponentModel.DataAnnotations;

namespace Subscription.Api.Models;

public class RegisterSubscriberRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Uri WebSiteUrl { get; set; }
}
