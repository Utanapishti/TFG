using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SensorReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Messaging
{
    public class Publisher : RabbitMQConnection
    {
        PublisherOptions _options;
        private PublicationAddress _publicationAddress;
        IBasicProperties _properties;
        //IModel _model;
        

        public Publisher(ILogger<RabbitMQConnection> logger, IOptions<PublisherOptions> options) : base(logger, options)
        {
            _options = options.Value;
            _publicationAddress = new PublicationAddress(ExchangeType.Topic, _options.Exchange, _options.Channel);
        }

        public void CreatePublisher()
        {
            using var model = base.Connect();
            if (model != null)
            {
                //model.QueueDeclare(_options.Channel);
                _properties = model.CreateBasicProperties();
                _properties.DeliveryMode = 2;
                model.ExchangeDeclare(exchange: _options.Exchange, type: ExchangeType.Direct);
            }
        }

        public void Publish(byte[] data)
        {
            _policy.Execute(() =>
            {
                using var model = Connect();
                model.BasicPublish(_publicationAddress, _properties, data);
            });
        }
    }
}
