using System.Net;

namespace Subscription.Tests.Doubles;

public class StubHttpMessageHandler : HttpMessageHandler
{
    private const string BASE_URI = "https://api.emailvalidation.com";
    private readonly Dictionary<string, HttpResponseMessage> responses = new();

    public void SetupResponse(string requestUrl, HttpStatusCode statusCode, string content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content)
        };
        responses[requestUrl] = response;
    }

    public void StubValidateResponse(string email, string content)
    {
        var requestUrl = $"{BASE_URI}/validate?email={Uri.EscapeDataString(email)}";
        SetupResponse(requestUrl, HttpStatusCode.OK, content);
    }

    public void StubSubscriptionStatusResponse(string domain, string content)
    {
        var requestUrl = $"https://api.subscriptions.internal/subscribers/?domain={Uri.EscapeDataString(domain)}";
        SetupResponse(requestUrl, HttpStatusCode.OK, content);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri?.ToString() ?? string.Empty;

        if (responses.TryGetValue(url, out var response))
        {
            return Task.FromResult(response);
        }

        // Default response for unmocked requests
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Not found")
        });
    }
}
