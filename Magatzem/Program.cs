using GestorCalculs;
using Magatzem;
using Messaging;
using Scripting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddJsonFile("functionsDefinition.json"))
    .ConfigureServices((context,services) =>
    {
        services.Configure<SubscriberOptions>(context.Configuration.GetSection("RabbitMQDadesGenerades"));
        services.Configure<PublisherOptions>(context.Configuration.GetSection("RabbitMQCalculDades"));
        services.Configure<GestorFuncionsOptions>(context.Configuration.GetSection("FunctionsDefinition"));        
        services.AddSingleton<Subscriber>();
        services.AddSingleton<Publisher>();
        services.AddSingleton<GestorFuncions>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
