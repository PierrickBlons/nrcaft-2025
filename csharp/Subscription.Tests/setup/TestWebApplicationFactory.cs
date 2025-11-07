using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Subscription.Api.Data;
using Subscription.Tests.Doubles;

namespace Subscription.Tests.Setup;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public StubHttpMessageHandler StubHttpHandler { get; } = new();
    private readonly string databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SubscriberDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<SubscriberDbContext>(options =>
                 options.UseInMemoryDatabase(databaseName: databaseName));
        });

        builder.UseEnvironment("Testing");
    }
}
