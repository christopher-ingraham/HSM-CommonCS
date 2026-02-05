namespace HSM_CommonCS.Messaging
{
    public interface IMessageBus : IDisposable
    {
        Task StartAsync();
        Task DeclareQueueAsync(string queueName);
        Task StartConsumerAsync(string queueName, Func<string, Task> onMessage);
    }
}
