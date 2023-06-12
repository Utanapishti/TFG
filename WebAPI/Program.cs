using Grpc.Core;
using Grpc.Core.Utils;
using System.Text.Json.Serialization;
using TFG.Protobuf.GRPC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

string? rpcHost = builder.Configuration.GetSection("StatusRPC").GetValue<string>("Host");
int rpcPort = builder.Configuration.GetSection("StatusRPC").GetValue<int>("Port");

var rpcChannel = new Channel(rpcHost + ":" + rpcPort.ToString(), ChannelCredentials.Insecure);
var rpcClient = new StatusService.StatusServiceClient(rpcChannel);

app.MapGet(
    "/status", async ()=>
{    
        var resposta = rpcClient.GetStatus(new PeticioStatus());

        var resultat = await resposta.ResponseStream.ToListAsync();

        return resultat.Select(status => new Status(status.NomVariable,status.Valor,status.Timestamp)).ToArray(); 
});

app.Run();


internal record Status(string nomVariable, double valor, uint timestamp);


