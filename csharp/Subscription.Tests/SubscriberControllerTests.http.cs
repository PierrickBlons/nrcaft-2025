using System.Net;
using Subscription.Api.Models;
using Subscription.Api.Services;
using Subscription.Tests.Setup;
using Subscription.Tests.Builders;

namespace Subscription.Tests;

public class SubscriberControllerHttpTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SubscriberControllerHttpTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Give_CreatedSubscriber_when_email_is_safe()
    {
        // Arrange
        var email = "test@example.com";

        var request = new RegisterSubscriberRequest { Email = email, Name = "John Doe", WebSiteUrl = new Uri("https://example.com") };

        // Act
        var response = await _client.PostAsync("/api/subscriber/register", request.ToJsonStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
