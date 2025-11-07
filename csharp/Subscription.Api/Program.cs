using Microsoft.EntityFrameworkCore;
using Subscription.Api.Data;
using Subscription.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add Entity Framework
builder.Services.AddDbContext<SubscriberDbContext>(options =>
    options.UseInMemoryDatabase("SubscriberDB"));

builder.Services.AddHttpClient<IEmailValidationService, EmailValidationService>(client =>
{
    client.BaseAddress = new Uri("https://api.emailvalidation.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<IEmailValidationService, EmailValidationService>();
builder.Services.AddScoped<SubscriberService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
