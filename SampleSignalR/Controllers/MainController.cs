using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SampleSignalR.Hubs;
using SampleSignalR.Services;

namespace SampleSignalR.Controllers;

[Route("api/[controller]")]
public class MainController(IHubContext<StreamHub, INotificationClient> hubContext,
    ISseService sseService) : ControllerBase
{
    [HttpGet("send-to-subscriber")]
    public async Task<IActionResult> Get([FromQuery] Guid jobLogId, [FromQuery] string message)
    {
        await sseService.SendMessageToSubscriberAsync(jobLogId, message);
        return Ok($"Message '{message}' sent to subscriber with JobLogId '{jobLogId}'.");
    }
    
    [HttpGet("send")]
    public async Task<IActionResult> HealthCheck([FromQuery] string message)
    {
        await hubContext.Clients.All.SendMessageAsync(message);
        return Ok($"Message '{message}' would be sent to connected SignalR clients.");
    }

    [HttpGet("send-to-subscriber-with-callback")]
    public async Task<IActionResult> SendToSubscriberWithCallback([FromQuery] Guid jobLogId, 
        [FromQuery] string message, [FromQuery] decimal a, [FromQuery] decimal b)
    {
        await sseService.SendMessageToSubscriberAsync(jobLogId, 
            () => CallbackFunction(message, a, b));
        return Ok($"Callback message sent to subscriber with JobLogId '{jobLogId}'.");
    }

    private static Task<string> CallbackFunction(string msg, decimal x, decimal y)
    {
        // Simulate some asynchronous operation
        return Task.FromResult($"Callback processed message: {msg}, Sum: {x + y}");
    }
}