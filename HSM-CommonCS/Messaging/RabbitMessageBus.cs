using HSM_CommonCS.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace HSM_CommonCS.Messaging
{
    internal class RabbitMessageBus : IMessageBus
    {
        private ILog _log;
        private readonly ConnectionFactory _factory;

        private IChannel _channel;
        private IConnection _connection;

        public RabbitMessageBus(ILog log, RabbitConfig cfg)
        {
            _log = log;

            _factory = new ConnectionFactory
            {
                HostName = cfg.Host,
                Port = cfg.Port,
                UserName = cfg.Username,
                Password = cfg.Password
            };
        }

        public async Task StartAsync()
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            _log.Trace($"RabbitMQ connected to {_factory.HostName}");
        }

        public async Task DeclareQueueAsync(string queueName)
        {
            EnsureChannel();

            await _channel!.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public async Task StartConsumerAsync(string queueName, Func<string, Task> onMessage)
        {
            EnsureChannel();

            var consumer = new AsyncEventingBasicConsumer(_channel!);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                await onMessage(message);
            };

            await _channel!.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer);

            _log.Trace($"Consuming queue '{queueName}'");
        }

        private void EnsureChannel()
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMessageBus not started");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
