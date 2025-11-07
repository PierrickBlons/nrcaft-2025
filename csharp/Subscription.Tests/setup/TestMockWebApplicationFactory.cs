using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Subscription.Api.Data;
using Subscription.Api.Services;
using Subscription.Tests.Doubles;

namespace Subscription.Tests.Setup;

public class TestMockWebApplicationFactory : WebApplicationFactory<Program>
{
    public StubHttpMessageHandler StubHttpHandler { get; } = new();
    public Mock<SubscriberDbContext> MockDbContext { get; }
    public Mock<DbSet<Subscriber>> MockSubscribersSet { get; }

    public TestMockWebApplicationFactory()
    {
        MockDbContext = new Mock<SubscriberDbContext>(new DbContextOptionsBuilder<SubscriberDbContext>().Options);
        MockSubscribersSet = new Mock<DbSet<Subscriber>>();
        
        // Setup IQueryable and IAsyncEnumerable interfaces on MockSubscribersSet
        // This must be done before accessing MockSubscribersSet.Object
        var data = new List<Subscriber>().AsQueryable();
        
        MockSubscribersSet.As<IAsyncEnumerable<Subscriber>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestDbAsyncEnumerator<Subscriber>(data.GetEnumerator()));

        MockSubscribersSet.As<IQueryable<Subscriber>>()
            .Setup(m => m.Provider)
            .Returns(new TestDbAsyncQueryProvider<Subscriber>(data.Provider));

        MockSubscribersSet.As<IQueryable<Subscriber>>().Setup(m => m.Expression).Returns(data.Expression);
        MockSubscribersSet.As<IQueryable<Subscriber>>().Setup(m => m.ElementType).Returns(data.ElementType);
        MockSubscribersSet.As<IQueryable<Subscriber>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
        
        // Setup the mock DbContext to return the mock DbSet
        MockDbContext.Setup(m => m.Subscribers).Returns(MockSubscribersSet.Object);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the default DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SubscriberDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test database
            // services.AddDbContext<SubscriberDbContext>(options =>
            //     options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            // Add mock DbContext
            services.AddSingleton(MockDbContext.Object);

            // Clear and reconfigure services to use stub handler
            services.AddHttpClient<IEmailValidationService, EmailValidationService>(client =>
            {
                client.BaseAddress = new Uri("https://api.emailvalidation.com");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() => StubHttpHandler);

            services.AddHttpClient<SubscriberService>(client =>
            {
                client.BaseAddress = new Uri("https://api.subscriptions.internal");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() => StubHttpHandler);

            // Add logging for HTTP client
            services.AddLogging(builder => builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Debug));
        });

        builder.UseEnvironment("Testing");
    }
}
