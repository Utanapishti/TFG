using Generador;
using Messaging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<PublisherOptions>(context.Configuration.GetSection("RabbitMQ"));
        services.AddSingleton<Publisher>();
        services.Configure<GeneradorOptions>(context.Configuration.GetSection("Generador"));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
