using Microsoft.AspNetCore.Mvc;
using SampleServerSentEvent.Services;

namespace SampleServerSentEvent.Controllers;

public class MainController(ISseService sseService,
    ICategoryService categoryService) : ControllerBase
{
    [HttpGet("stream/{jobLogId:long}")]
    public async Task Index(long jobLogId, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";
        
        await sseService.SubScribeAsync(jobLogId, HttpContext);
    }
    
    [HttpGet("send")]
    public async Task<IActionResult> Send([FromQuery] long jobLogId, [FromQuery] string message, CancellationToken cancellationToken)
    {
        if (!sseService.IsSubscribedAsync(jobLogId)) 
            return BadRequest("JobLogId is not valid.");
        
        await sseService.SendMessageToSubscriberAsync(jobLogId, message);
        return Ok("Message sent to subscriber.");
    }
    
    [HttpGet("send-to-subscriber-with-callback")]
    public async Task<IActionResult> SendToSubscriberWithCallback([FromQuery] long jobLogId, 
        [FromQuery] string message, [FromQuery] decimal a, [FromQuery] decimal b)
    {
        await sseService.SendMessageToSubscriberThroughCallbackAsync(jobLogId, 
            () => CallbackFunction(message, a, b));
        return Ok($"Callback message sent to subscriber with JobLogId '{jobLogId}'.");
    }

    private static Task<string> CallbackFunction(string msg, decimal x, decimal y)
    {
        // Simulate some asynchronous operation
        return Task.FromResult($"Callback processed message: {msg}, Sum: {x + y}");
    }
    
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        await sseService.SendMessageToSubscriberThroughCallbackAsync(1, () 
            => categoryService.GetCategories([1, 2, 3]));
        
        return Ok("Categories request sent.");
    }
}