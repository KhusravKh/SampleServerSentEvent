using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using SampleSignalR.Hubs;

namespace SampleSignalR.Services;

public class SseService(IHubContext<StreamHub, INotificationClient> hubContext,
    ILogger<SseService> logger) : ISseService
{
    private readonly ConcurrentDictionary<Guid, string> _subscribers = new();

    public void SubScribeAsync(Guid jobLogId, string userId)
    {
        _subscribers.TryAdd(jobLogId, userId);
    }

    public bool IsSubscribedAsync(Guid jobLogId)
    {
        return _subscribers.ContainsKey(jobLogId);
    }

    public async Task SendMessageToSubscriberAsync(Guid jobLogId, object message)
    {
        if (_subscribers.TryGetValue(jobLogId, out var userId))
        {
            await hubContext.Clients.User(userId).SendMessageAsync(message);
            RemoveSubscriberAsync(jobLogId);
        }
        else
        {
            logger.LogWarning("User with JobLogId {JobLogId} not found.", jobLogId);
        }
    }

    public void RemoveSubscriberAsync(Guid jobLogId)
    {
        if (_subscribers.TryRemove(jobLogId, out _))
        {
            logger.LogInformation("Subscriber with JobLogId {JobLogId} removed.", jobLogId);
        }
        else
        {
            logger.LogWarning("Subscriber with JobLogId {JobLogId} not found.", jobLogId);
        }
    }

    public async Task SendMessageToSubscriberAsync<T>(Guid jobLogId, Func<Task<T>> callback)
    {
        if (!IsSubscribedAsync(jobLogId))
            return;
        
        var message = await callback();
        if (message != null)
           await SendMessageToSubscriberAsync(jobLogId, message);
        
        RemoveSubscriberAsync(jobLogId);
    }
}