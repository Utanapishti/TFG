using GestorCalculs;
using Google.Protobuf;
using GRPC;
using Messaging;
using Microsoft.Extensions.Options;
using Protocol;
using TipusDades;


namespace Magatzem
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly RabbitMQConnection _conTractament;
        private readonly RabbitMQConnection _conGenerador;
        private readonly GestorFuncions _gestorFuncions;
        private readonly ValorsRPCServer _rpcServer;

        public Worker(ILogger<Worker> logger, IOptions<TractamentConnectionOptions> tractamentOptions, IOptions<GeneradorConnectionOptions> generadorOptions ,GestorFuncions gestorFuncions)
        {            
            _gestorFuncions = gestorFuncions;
            _gestorFuncions.PeticioCalcul = PeticioCalcul;
            _rpcServer = new ValorsRPCServer(new ValorServiceImpl(logger,gestorFuncions));
            _rpcServer.Start();            
            HealthService.Status = ServingStatus.NOT_SERVING;
            _logger = logger;
            _logger.LogInformation(tractamentOptions.Value.User);
            _conTractament = new RabbitMQConnection(_logger, tractamentOptions);
            _logger.LogInformation(generadorOptions.Value.User);
            _conTractament.Received += _conTractament_Received;
            _conGenerador = new RabbitMQConnection(_logger, generadorOptions);
            _conGenerador.Received += _subscriber_Received;
        }

        private void _conTractament_Received(object? sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            //Dada rebuda
            var dadaCalculada =  ProtocolDadaCalculada.Parse(e.Body.ToArray());
            _gestorFuncions.RebutDadaCalculada(dadaCalculada.NomVariable, dadaCalculada.Valor,dadaCalculada.Timestamp);
            _logger.LogInformation("Received data from {name}: {valor}", dadaCalculada.NomVariable, dadaCalculada.Valor);
        }

        private void PeticioCalcul(string variableCalcular, string variableRebuda, GestorCalculs.Dada dadaRebuda, uint tsAltresValors)
        {
            var calculDada= new TFG.Protobuf.CalculDada()
            {
                VariableCalcular = variableCalcular,
                VariableRebuda = variableRebuda,
                ValorRebut=dadaRebuda.Valor,
                TimestampRebut=dadaRebuda.Timestamp,
                TimestampActual=tsAltresValors
            };

            _conTractament.Publish(MessageExtensions.ToByteArray(calculDada));
        }

        private void _subscriber_Received(object? sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            //Dada rebuda
            var dadaRebuda = ProtocolDadaGenerada.Parse(e.Body.ToArray());
            _gestorFuncions.RebutDada(dadaRebuda.Name, dadaRebuda.Valor);
            _logger.LogInformation("Received data from {name}: {valor}", dadaRebuda.Name,dadaRebuda.Valor);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Connectant a cua de tractament...");
            _conTractament.Connect(stoppingToken);
            _logger.LogInformation("Connectant a cua de dades generades...");
            _conGenerador.ConnectAndSubscribe(stoppingToken);
            HealthService.Status = ServingStatus.SERVING;
            while (!stoppingToken.IsCancellationRequested)
            {                
                await Task.Delay(1000, stoppingToken);
            }
            HealthService.Status = ServingStatus.NOT_SERVING;
        }
    }
}