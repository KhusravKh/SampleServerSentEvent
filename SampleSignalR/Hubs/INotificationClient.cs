namespace SampleSignalR.Hubs;

public interface INotificationClient
{
    Task SendMessageAsync(object message);
    
    Task SendMessageAsync(string userId, string message);
}