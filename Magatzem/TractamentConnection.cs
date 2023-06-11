using Messaging;
using Microsoft.Extensions.Options;

public class TractamentConnection : RabbitMQConnection
{
    public TractamentConnection(ILogger<RabbitMQConnection> logger, IOptions<TractamentConnection> options) : base(logger, options)
    {
    }
}

public class TractamentConnection : ConnectionOptions { };