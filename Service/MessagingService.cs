
using MLQuestionSimilarity.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MLQuestionSimilarity.Services
{
    public class MessagingService
    {
        private readonly string _queueName = "Similarity-Worker-Queue";
        private IConnection _connection;
        private IChannel _channel;

        public async Task StartConsuming(Func<InputData, Task> onMessageReceived, CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(_queueName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var input = JsonSerializer.Deserialize<InputData>(message);
                if (input != null)
                    await onMessageReceived(input);
            };

            await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer);
        }

        public async Task StopAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}
