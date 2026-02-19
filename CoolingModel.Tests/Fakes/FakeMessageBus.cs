using HSM_CommonCS.Messaging;

namespace CoolingModel.Tests.Fakes;

/// <summary>
/// In-memory message bus for testing. Captures published messages and lets tests
/// simulate received messages.
/// </summary>
public class FakeMessageBus : IMessageBus
{
    private readonly Dictionary<string, List<string>> _published = new();
    private readonly Dictionary<string, Func<string, Task>> _consumers = new();
    private readonly HashSet<string> _declaredQueues = new();
    private bool _started;
    private bool _disposed;

    /// <summary>True after StartAsync() has been called.</summary>
    public bool IsStarted => _started;

    /// <summary>All declared queue names.</summary>
    public IReadOnlySet<string> DeclaredQueues => _declaredQueues;

    /// <summary>Get all messages published to a given queue.</summary>
    public IReadOnlyList<string> GetPublished(string queueName) =>
        _published.TryGetValue(queueName, out var msgs) ? msgs : Array.Empty<string>();

    public Task StartAsync()
    {
        _started = true;
        return Task.CompletedTask;
    }

    public Task DeclareQueueAsync(string queueName)
    {
        _declaredQueues.Add(queueName);
        return Task.CompletedTask;
    }

    public Task StartConsumerAsync(string queueName, Func<string, Task> onMessage)
    {
        _consumers[queueName] = onMessage;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Simulate publishing a message (for future use when PublishAsync is added to IMessageBus).
    /// </summary>
    public void SimulatePublish(string queueName, string message)
    {
        if (!_published.ContainsKey(queueName))
            _published[queueName] = new List<string>();
        _published[queueName].Add(message);
    }

    /// <summary>
    /// Simulate receiving a message on a queue (triggers the consumer callback).
    /// </summary>
    public async Task SimulateReceive(string queueName, string message)
    {
        if (_consumers.TryGetValue(queueName, out var handler))
            await handler(message);
    }

    public void Dispose()
    {
        _disposed = true;
    }
}
