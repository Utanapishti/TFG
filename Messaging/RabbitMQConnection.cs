using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace SensorReader
{
    public abstract class RabbitMQConnection
    {
        private IConnectionFactory _connectionFactory;
        private ILogger<RabbitMQConnection> _logger;
        private TimeSpan retryTime = TimeSpan.FromSeconds(30);
        private IConnection _connection;
        protected RetryPolicy _policy;       
        private object _lockConnection = new object();

        public RabbitMQConnection(ILogger<RabbitMQConnection> logger,IOptions<ConnectionOptions> options) {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = options.Value.Host,
                Port=options.Value.Port,
            };
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected IModel? Connect()
        {
            if ((_connection == null)
                || (!_connection.IsOpen))
            {
                _logger.LogInformation("Connecting to RabbitMQ queue");

                lock (_lockConnection)
                {
                    _policy = RetryPolicy.Handle<SocketException>()
                        .WaitAndRetryForever((i) => retryTime);

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
                                 durable: false,
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
