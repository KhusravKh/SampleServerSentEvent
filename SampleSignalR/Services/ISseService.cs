namespace SampleSignalR.Services;

public interface ISseService
{
    void SubScribeAsync(Guid jobLogId, string userId);
    
    bool IsSubscribedAsync(Guid jobLogId);
    
    Task SendMessageToSubscriberAsync(Guid jobLogId, object message);
    
    void RemoveSubscriberAsync(Guid jobLogId);
    
    Task SendMessageToSubscriberAsync<T>(Guid jobLogId, Func<Task<T>> callback);
}