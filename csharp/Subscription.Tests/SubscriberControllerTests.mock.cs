using System.Net;
using Moq;
using Subscription.Api.Models;
using Subscription.Api.Services;
using Subscription.Tests.Setup;
using Subscription.Tests.Builders;
using Subscription.Tests.Doubles;

namespace Subscription.Tests;

public class SubscriberControllerMockTests : IClassFixture<TestMockWebApplicationFactory>
{
    private readonly TestMockWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SubscriberControllerMockTests(TestMockWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _factory.MockSubscribersSet.Invocations.Clear();
        _factory.MockDbContext.Invocations.Clear();
    }

    [Fact]
    public async Task Give_CreatedSubscriber_when_email_is_safe()
    {
        // Arrange
        var email = "test@example.com";

        _factory.StubHttpHandler.StubValidateResponse(email, $"{{\"safe\":true}}");
        _factory.StubHttpHandler.StubSubscriptionStatusResponse("example.com", $"{{\"domain\":\"example.com\",\"status\":\"active\"}}");

        var request = new RegisterSubscriberRequest { Email = email, Name = "John Doe", WebSiteUrl = new Uri("https://example.com") };

        // Act
        var response = await _client.PostAsync("/api/subscriber/register", request.ToJsonStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var subscriberResponse = await response.DeserializeResponse<SubscriberResponse>();
        
        Assert.Equivalent(
            new {
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

        _factory.StubHttpHandler.StubValidateResponse(email, $"{{\"safe\":true}}");
        _factory.StubHttpHandler.StubSubscriptionStatusResponse("example.com", $"{{\"domain\":\"example.com\",\"status\":\"active\"}}");

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

        _factory.StubHttpHandler.StubValidateResponse(email, $"{{\"safe\":false}}");

        var request = new RegisterSubscriberRequest { Email = email };

        // Act
        var response = await _client.PostAsync("/api/subscriber/register", request.ToJsonStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private void SetupMockSubscribersSetAsQueryable<T>(IQueryable<T> data)
    {
        _factory.MockSubscribersSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));
        _factory.MockSubscribersSet.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(data.Expression);
        _factory.MockSubscribersSet.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(data.ElementType);
        _factory.MockSubscribersSet.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(data.GetEnumerator());
        _factory.MockSubscribersSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));
    }
}


