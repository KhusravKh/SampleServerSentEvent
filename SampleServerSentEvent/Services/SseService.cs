using System.Collections.Concurrent;

namespace SampleServerSentEvent.Services;

public class SseService(ILogger<SseService> logger) : ISseService
{
    private readonly ConcurrentDictionary<long, HttpContext> _subscribers = new();


    public async Task SubScribeAsync(long jobLogId, HttpContext httpContext)
    {
        _subscribers.TryAdd(jobLogId, httpContext);
        try
        {
            while (!httpContext.RequestAborted.IsCancellationRequested)
            {
                await httpContext.Response.WriteAsync(": heartbeat\n\n", httpContext.RequestAborted);
                await httpContext.Response.Body.FlushAsync(httpContext.RequestAborted);
                logger.LogInformation("Subscribed JobLogId: {JobLogId} is waiting for messages...", jobLogId);
                await Task.Delay(5000, httpContext.RequestAborted);
            }
        }
        catch (TaskCanceledException)
        {
            await RemoveSubscriberAsync(jobLogId);
        }
    }

    public bool IsSubscribedAsync(long jobLogId)
    {
        return _subscribers.ContainsKey(jobLogId);
    }

    public async Task SendMessageToSubscriberAsync(long jobLogId, object message)
    {
        if (_subscribers.TryGetValue(jobLogId, out var subscriber))
        {
            var data = $"data: {System.Text.Json.JsonSerializer.Serialize(message)}\n\n";
            await subscriber.Response.WriteAsync(data, subscriber.RequestAborted);
            await subscriber.Response.Body.FlushAsync(subscriber.RequestAborted);
            await RemoveSubscriberAsync(jobLogId);
        }
        else
        {
            logger.LogInformation("Subscriber with JobLogId {JobLogId} not found.", jobLogId);
        }
    }

    public async Task RemoveSubscriberAsync(long jobLogId)
    {
        if(_subscribers.TryGetValue(jobLogId, out var subscriber))
        {
            _subscribers.TryRemove(jobLogId, out _);
            logger.LogInformation("Subscriber with JobLogId {JobLogId} removed.", jobLogId);
            // Give some time for the client to receive the last message before aborting the connection
            await Task.Delay(1000);
            subscriber.Abort();
        }
        else
        {
            logger.LogInformation("Subscriber with JobLogId {JobLogId} not found.", jobLogId);
        }
    }

    public async Task SendMessageToSubscriberThroughCallbackAsync<T>(long jobLogId, Func<Task<T>> callback)
    {
        if (!IsSubscribedAsync(jobLogId))
            return;
        
        var message = await callback();
        if (message != null)
            await SendMessageToSubscriberAsync(jobLogId, message);
        
        await RemoveSubscriberAsync(jobLogId);
    }
}