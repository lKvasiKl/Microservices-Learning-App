using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuretion;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _chennel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuretion = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuretion["RabbitMQHost"],
                Port = int.Parse(_configuretion["RabbitMQPort"])
            };

            _connection = factory.CreateConnection();
            _chennel = _connection.CreateModel();
            _chennel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _chennel.QueueDeclare().QueueName;
            _chennel.QueueBind(queue: _queueName,
                exchange: "trigger",
                routingKey: "");

            Console.WriteLine("--> Listening on the Message Bus...");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_chennel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event Received!");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _chennel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection Shutdown");
        }

        public override void Dispose()
        {
            if (_chennel.IsOpen)
            {
                _chennel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}