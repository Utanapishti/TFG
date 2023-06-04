using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SensorReader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Messaging
{
    public class Subscriber : RabbitMQConnection
    {
        SubscriberOptions _options;
        ILogger _logger;
        IModel model;
        public event EventHandler<BasicDeliverEventArgs> Received;

        public Subscriber(ILogger<RabbitMQConnection> logger, IOptions<SubscriberOptions> options) : base(logger, options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public void Subscribe()
        {
            model = base.Connect();
            if (model != null)
            {
                var queue = model.QueueDeclare(_options.Channel, false, false, false, null);
                /*foreach(var subscription in _options.Subscriptions)
                {
                    model.QueueBind(queue.QueueName,, subscription);
                }*/

                var consumer = new EventingBasicConsumer(model);
                consumer.Received += Consumer_Received;

                model.BasicConsume(queue.QueueName, true, consumer);
            }
        }

        private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            _logger.LogInformation("Received: " + Encoding.UTF8.GetString(e.Body.ToArray()));
            Received?.Invoke(this, e);
        }
    }
}
