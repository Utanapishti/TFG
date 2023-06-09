using GestorCalculs;
using GRPC;
using Magatzem;
using Messaging;
using Scripting;



IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddJsonFile("functionsDefinition.json"))
    .ConfigureServices((context, services) =>
    {
        services.Configure<GeneradorConnectionOptions>(context.Configuration.GetSection("RabbitMQDadesGenerades"));
        services.Configure<TractamentConnectionOptions>(context.Configuration.GetSection("RabbitMQCalculDades"));
        services.Configure<GestorFuncionsOptions>(context.Configuration.GetSection("FunctionsDefinition"));        
        services.AddSingleton<GestorFuncions>();
        services.AddHostedService<Worker>();        
    })
    .Build();

host.Run();
