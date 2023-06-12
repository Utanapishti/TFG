using Messaging;
using Microsoft.Extensions.Options;

public class TractamentConnection : RabbitMQConnection
{
    public TractamentConnection(ILogger<RabbitMQConnection> logger, IOptions<TractamentConnectionOptions> options) : base(logger, options)
    {
        logger.LogInformation(options.Value.Host);
    }
}

public class TractamentConnectionOptions : ConnectionOptions { };