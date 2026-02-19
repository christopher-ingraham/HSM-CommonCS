using HSM_CommonCS.Messages;
using System.Text.Json;

namespace HSM_CommonCS.Messaging;

public class MessageDispatcher
{
    private readonly Dictionary<int, Func<string, Task>> _handlers = new();

    public void Register<T>(int messageId, Func<T, Task> handler)
        where T : IL2L2Message
    {
        _handlers[messageId] = json =>
        {
            var msg = JsonSerializer.Deserialize<T>(json)
            ?? throw new InvalidOperationException(
                 $"Failed to deserialize message id {messageId} as {typeof(T).Name}");
            return handler(msg);
        };
    }

    public Task Dispatch(string json)
    {
        // Every message envelope carries the id so we know what type it is
        using var doc = JsonDocument.Parse(json);
        var id = doc.RootElement.GetProperty("id").GetInt32();

        if (_handlers.TryGetValue(id, out var handler))
            return handler(json);

        return Task.CompletedTask;  // unknown message - ignore
    }
}
