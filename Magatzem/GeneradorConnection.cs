using Messaging;
using Microsoft.Extensions.Options;



public class GeneradorConnection : RabbitMQConnection
{
    public GeneradorConnection(ILogger<RabbitMQConnection> logger, IOptions<GeneradorConnectionOptions> options) : base(logger, options)
    {
    }
}

public class GeneradorConnectionOptions : ConnectionOptions { };