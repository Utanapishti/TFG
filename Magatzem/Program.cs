using GestorCalculs;
using GRPC;
using Magatzem;
using Messaging;
using Scripting;



IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddJsonFile("functionsDefinition.json"))
    .ConfigureServices((context, services) =>
    {
        services.Configure<ConsumerOptions>(context.Configuration.GetSection("RabbitMQDadesGenerades"));
        services.Configure<PublisherOptions>(context.Configuration.GetSection("RabbitMQCalculDades"));
        services.Configure<GestorFuncionsOptions>(context.Configuration.GetSection("FunctionsDefinition"));
        services.AddSingleton<Consumer>();
        services.AddSingleton<Publisher>();
        services.AddSingleton<GestorFuncions>();
        services.AddHostedService<Worker>();        
    })
    .Build();

host.Run();
