using Generador;
using Messaging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {        
        services.Configure<ConnectionOptions>(context.Configuration.GetSection("RabbitMQDadesGenerades"));
        services.AddSingleton<RabbitMQConnection>();
        services.Configure<GeneradorOptions>(context.Configuration.GetSection("Generador"));        
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
