using Microsoft.AspNetCore.Mvc;
using Subscription.Api.Models;
using Subscription.Api.Services;

namespace Subscription.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriberController(IEmailValidationService emailValidationService, SubscriberService subscriberService, ILogger<SubscriberController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<SubscriberResponse>> Register([FromBody] RegisterSubscriberRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.LogInformation("Registering new subscriber with email: {Email}", request.Email);

        // Validate email safety using external service
        var isEmailSafe = await emailValidationService.IsEmailSafeAsync(request.Email);

        if (!isEmailSafe)
        {
            logger.LogWarning("Email validation failed for: {Email}", request.Email);
            return BadRequest("Email is not safe or valid according to validation service");
        }

        var subscriber = await subscriberService.CreateSubscriberAsync(request.Email, request.Name);

        if (subscriber == null)
        {
            logger.LogError("Failed to create subscriber for email: {Email}", request.Email);
            return StatusCode(500, "Failed to create subscriber");
        }

        var response = new SubscriberResponse(subscriber.Id, request.Email, subscriber.CreatedAt);

        logger.LogInformation("Successfully registered subscriber: {Id} with email: {Email}", response.Id, response.Email);

        return CreatedAtAction(nameof(Register), new { id = response.Id }, response);
    }
}
