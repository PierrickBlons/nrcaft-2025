using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Subscription.Api.Data;
using Subscription.Api.Models;
using Subscription.Api.Services;
using Subscription.Tests.Builders;
using Subscription.Tests.Setup;

namespace Subscription.Tests;

public class SubscriberControllerFakeTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SubscriberControllerFakeTests(TestWebApplicationFactory factory)
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

        var subscriberResponse = await response.DeserializeResponse<SubscriberResponse>();

        Assert.Equivalent(
            new
            {
                Email = email,
                Id = subscriberResponse!.Id
            },
        subscriberResponse);
    }

    [Fact]
    public async Task Give_ExistingSubscriber_when_subscriber_already_exists()
    {
        // Arrange
        var email = "existing@example.com";
       
        var request = new RegisterSubscriberRequest { Email = email, Name = "John Doe", WebSiteUrl = new Uri("https://example.com") };

        // Act
        var response = await _client.PostAsync("/api/subscriber/register", request.ToJsonStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var subscriberResponse = await response.DeserializeResponse<SubscriberResponse>();
    }

    [Fact]
    public async Task RegisterSubscriber_WithUnsafeEmail_ReturnsBadRequest()
    {
        // Arrange
        var email = "unsafe@spam.com";
        var request = new RegisterSubscriberRequest { Email = email };

        // Act
        var response = await _client.PostAsync("/api/subscriber/register", request.ToJsonStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}


