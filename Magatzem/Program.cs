using GestorCalculs;
using GRPCServer.Services;
using Magatzem;
using Messaging;
using Microsoft.AspNetCore.Builder;
using Scripting;

var host = WebApplication.CreateBuilder(args);
host.Configuration.AddJsonFile("functionsDefinition.json");
var services = host.Services;
services.AddGrpc();
services.Configure<ConsumerOptions>(host.Configuration.GetSection("RabbitMQDadesGenerades"));
services.Configure<PublisherOptions>(host.Configuration.GetSection("RabbitMQCalculDades"));
services.Configure<GestorFuncionsOptions>(host.Configuration.GetSection("FunctionsDefinition"));
services.AddSingleton<Consumer>();
services.AddSingleton<Publisher>();
services.AddSingleton<GestorFuncions>();
//services.AddHostedService<Worker>();
var app = host.Build();

app.MapGrpcService<ValorService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
