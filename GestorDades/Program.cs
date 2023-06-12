using GestorCalculs;
using GRPC;
using GestorDades;
using Messaging;
using Scripting;



IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddJsonFile("functionsDefinition.json"))
    .ConfigureServices((context, services) =>
    {
        services.Configure<GeneradorConnectionOptions>(context.Configuration.GetSection("RabbitMQDadesGenerades"));
        services.Configure<TractamentConnectionOptions>(context.Configuration.GetSection("RabbitMQDadesCalculades"));
        services.Configure<GestorFuncionsOptions>(context.Configuration.GetSection("FunctionsDefinition"));
        services.AddSingleton<TractamentConnection>();
        services.AddSingleton<GestorFuncions>();
        services.AddSingleton<GeneradorConnection>();
        services.AddSingleton<TractamentConnection>();
        services.AddHostedService<Worker>();        
    })
    .Build();

host.Run();
