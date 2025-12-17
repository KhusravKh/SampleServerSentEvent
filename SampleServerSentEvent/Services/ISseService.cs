namespace SampleServerSentEvent.Services;

public interface ISseService
{
    Task SubScribeAsync(long jobLogId, HttpContext httpContext);
    
    bool IsSubscribedAsync(long jobLogId);
    
    Task SendMessageToSubscriberAsync(long jobLogId, object message);
    
    Task RemoveSubscriberAsync(long jobLogId);
    
    Task SendMessageToSubscriberThroughCallbackAsync<T>(long jobLogId, Func<Task<T>> callback);
}