using GestorCalculs;
using Google.Protobuf;
using GRPC;
using Messaging;
using Microsoft.Extensions.Options;
using Protocol;
using TipusDades;


namespace GestorDades
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly RabbitMQConnection _conTractament;
        private readonly RabbitMQConnection _conGenerador;
        private readonly GestorFuncions _gestorFuncions;
        private readonly ValorsRPCServer _rpcServer;

        public Worker(ILogger<Worker> logger, TractamentConnection tractamentConnection, GeneradorConnection generadorConnection,GestorFuncions gestorFuncions)
        {            
            _logger=logger;
            _gestorFuncions = gestorFuncions;
            _gestorFuncions.PeticioCalcul = PeticioCalcul;
            _rpcServer = new ValorsRPCServer(new ValorServiceImpl(logger,gestorFuncions), new StatusServiceImpl(logger,gestorFuncions));
            _rpcServer.Start();            
            HealthService.Status = ServingStatus.NOT_SERVING;
            _conTractament = tractamentConnection;
            _conTractament.Received += _conTractament_Received;
            _conGenerador = generadorConnection;
            _conGenerador.Received += _subscriber_Received;
        }

        private void _conTractament_Received(object? sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            //Dada rebuda
            var dadaCalculada =  ProtocolDadaCalculada.Parse(e.Body.ToArray());
            _gestorFuncions.RebutDadaCalculada(dadaCalculada.NomVariable, dadaCalculada.Valor,dadaCalculada.Timestamp);
            _logger.LogInformation("Received data from {name}: {valor}", dadaCalculada.NomVariable, dadaCalculada.Valor);
            _conTractament.AckMessage(e.DeliveryTag);
        }

        private void PeticioCalcul(string variableCalcular, string variableRebuda, GestorCalculs.Dada dadaRebuda, uint tsAltresValors)
        {
            var calculDada= new TFG.Protobuf.CalculDada()
            {
                VariableCalcular = variableCalcular,
                VariableRebuda = variableRebuda,
                ValorRebut=dadaRebuda.Valor,
                TimestampRebut=dadaRebuda.Timestamp,
                TimestampAltres=tsAltresValors
            };

            _logger.LogInformation($"Peticio de calcul de la variable {variableCalcular} per recepcio de {variableRebuda}. TS Rebut: {dadaRebuda.Timestamp} - TS Altres: {tsAltresValors}");

            _conTractament.Publish(MessageExtensions.ToByteArray(calculDada), "peticioCalcul");
        }

        private void _subscriber_Received(object? sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            //Dada rebuda
            var dadaRebuda = ProtocolDadaGenerada.Parse(e.Body.ToArray());
            _gestorFuncions.RebutDada(dadaRebuda.Name, dadaRebuda.Valor);
            _logger.LogInformation("Received data from {name}: {valor}", dadaRebuda.Name,dadaRebuda.Valor);
            _conGenerador.AckMessage(e.DeliveryTag);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Connectant a cua de tractament...");
            _conTractament.ConnectAndSubscribe(stoppingToken, new string[] { "resultatCalcul" });
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