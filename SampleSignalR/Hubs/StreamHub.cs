using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SampleSignalR.Services;

namespace SampleSignalR.Hubs;

[Authorize]
public class StreamHub(
    ILogger<StreamHub> logger,
    ISseService sseService) : Hub<INotificationClient>
{
    
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendMessageToServer(Guid jobLogId)
    {
        logger.LogInformation("Received from client ({ConnectionId}): {jobLogId}", Context.ConnectionId, jobLogId);
        //_subscribers.TryAdd(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "undefined", jobLogId);
        sseService.SubScribeAsync(jobLogId, Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "undefined");
        // Optionally broadcast back to all clients
        await Clients.All.SendMessageAsync($"Echo from server: {jobLogId}");
    }
}