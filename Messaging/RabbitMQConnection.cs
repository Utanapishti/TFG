using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging
{
    public class RabbitMQConnection
    {
        private IConnectionFactory _connectionFactory;
        private ILogger _logger;
        private TimeSpan retryTime = TimeSpan.FromSeconds(30);
        private IConnection _connection;
        protected RetryPolicy _policy;       
        private object _lockConnection = new object();
        IModel? _model;
        string _queueName;
        public event EventHandler<BasicDeliverEventArgs> Received;
        private PublicationAddress _publicationAddress;
        IBasicProperties _properties;

        public RabbitMQConnection(ILogger logger,IOptions<ConnectionOptions> options) {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = options.Value.Host,
                Port=options.Value.Port,
            };
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueName = options.Value.Channel;
            _publicationAddress = new PublicationAddress(ExchangeType.Direct, string.Empty, _queueName);
        }

        public bool Connect()
        {
            _model = ConnectModel();
            if (_model != null)
            {
                _model.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
                _properties = _model.CreateBasicProperties();
                _properties.DeliveryMode = 2;
            }

            return _model != null;
        }

        public void Subscribe()
        {
            
            if (_model != null)
            {
                var queue = _model.QueueDeclare(_queueName, true, false, false, null);
                var consumer = new EventingBasicConsumer(_model);
                consumer.Received += Consumer_Received;

                _model.BasicConsume(queue.QueueName, true, consumer);
            }
        }
        public void Publish(byte[] data)
        {
            _policy.Execute(() =>
            {                
                _model.BasicPublish(_publicationAddress, _properties, data);
            });
        }

        private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            _logger.LogInformation("Received: " + Encoding.UTF8.GetString(e.Body.ToArray()));
            Received?.Invoke(this, e);
        }

        protected IModel? ConnectModel()
        {
            if ((_connection == null)
                || (!_connection.IsOpen))
            {
                _logger.LogInformation("Connecting to RabbitMQ queue");

                lock (_lockConnection)
                {
                    _policy = RetryPolicy.Handle<Exception>()
                        .WaitAndRetryForever((i) => retryTime, (Exception, time) => { _model = ConnectModel(); });

                    _policy.Execute(() =>
                    {
                        _connection = _connectionFactory.CreateConnection();                        
                    });

                    if ((_connection != null) && (_connection.IsOpen))
                    {
                        _connection.ConnectionShutdown += _connection_ConnectionShutdown;                        
                    }
                }
            }
            return _connection?.CreateModel();
        }       
        

        private void _connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"Connection shutdown {e.ReplyText}");
            Connect();
        }

        public void TestSend() {
            var factory = new ConnectionFactory { HostName = "rabbitmq",Port=5672};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            const string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                routingKey: "hello",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine($" [x] Sent {message}");            
        }
    }
}
