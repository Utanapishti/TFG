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
        private TimeSpan retryTime = TimeSpan.FromSeconds(5);
        private IConnection _connection;
        protected RetryPolicy _policy;       
        private object _lockConnection = new object();
        private CancellationToken _cancelToken;
        IModel? _model;
        string _exchange;
        public event EventHandler<BasicDeliverEventArgs> Received;
        IBasicProperties _properties;

        public RabbitMQConnection(ILogger<RabbitMQConnection> logger,IOptions<ConnectionOptions> options) {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = options.Value.Host,
                Port=options.Value.Port,
                UserName=options.Value.User,
                Password=options.Value.Password
            };
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exchange = options.Value.Exchange;            
        }

        public void ConnectAndSubscribe(CancellationToken cancelToken, string[] topics=null)
        {
            var policy = RetryPolicy.HandleResult<bool>(false).WaitAndRetryForever<bool>(i => retryTime);
            policy.Execute(() => Connect(cancelToken));
            Subscribe(topics);
        }

        public bool Connect(CancellationToken cancelToken)
        {
            _logger.LogInformation("Connecting to RabbitMQ queue");
            _cancelToken = cancelToken;

            _model = ConnectModel();
            if (_model != null)
            {                
                _properties = _model.CreateBasicProperties();                
                _properties.Persistent = true;
                _model.ExchangeDeclare(_exchange, ExchangeType.Direct, true, false);
            }
            else _logger.LogInformation("Connection failed");

            return _model != null;
        }

        public void Subscribe(string[] topics)
        {
            _logger.LogInformation("Connecting to RabbitMQ queue");

            if (_model != null)
            {
                var queueName = _model.QueueDeclare().QueueName;
                if (topics == null || topics.Length == 0)
                {
                    _model.QueueBind(queueName, _exchange, String.Empty);
                }
                else
                {
                    foreach (var topic in topics)
                    {
                        _model.QueueBind(queueName, _exchange, topic);
                    }
                }
                var consumer = new EventingBasicConsumer(_model);
                consumer.Received += Consumer_Received;

                _model.BasicConsume(queueName, false, consumer);
            }
            else _logger.LogInformation("Connection failed");
        }
        public void Publish(byte[] data,string topic="")
        {
            var publicationAddress = new PublicationAddress(ExchangeType.Direct, _exchange, topic);

            _policy.Execute(() =>
            {                
                _model.BasicPublish(publicationAddress, _properties, data);
            });
        }
        public void AckMessage(ulong tag)
        {            
            _model?.BasicAck(tag, false);
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
                lock (_lockConnection)
                {
                    _policy = RetryPolicy.Handle<Exception>()
                        .WaitAndRetryForever((i) => retryTime);

                    _policy.Execute((CancellationToken cancelToken) =>
                    {
                        _connection = _connectionFactory.CreateConnection();                                
                    }, _cancelToken);

                    if ((_connection != null) && (_connection.IsOpen))
                    {
                        _logger.LogInformation("Connected to RabbitMQ queue");
                        _connection.ConnectionShutdown += _connection_ConnectionShutdown;                        
                    }
                }
            }
            return _connection?.CreateModel();
        }       
        

        private void _connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"Connection shutdown {e.ReplyText}");
            Connect(_cancelToken);
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
