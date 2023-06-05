using GestorCalculs;
using Google.Protobuf;
using GRPC;
using Messaging;
using Microsoft.Extensions.Options;
using Protocol;

namespace Magatzem
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly RabbitMQConnection _conTractament;
        private readonly RabbitMQConnection _conGenerador;
        private GestorFuncions _gestorFuncions;
        private RPCServer _rpcServer;

        public Worker(ILogger<Worker> logger, IOptions<TractamentConnectionOptions> tractamentOptions, IOptions<GeneradorConnectionOptions> generadorOptions ,GestorFuncions gestorFuncions)
        {
            _rpcServer = new RPCServer(new ValorServiceImpl(logger));            
            _gestorFuncions = gestorFuncions;
            _gestorFuncions.PeticioCalcul = PeticioCalcul;
            _logger = logger;
            _conTractament = new RabbitMQConnection(_logger, tractamentOptions);
            _conGenerador = new RabbitMQConnection(_logger, generadorOptions);
            /*_publisher.CreatePublisher();
            _subscriber.Subscribe();
            _subscriber.Received += _subscriber_Received;            */
        }

        private void PeticioCalcul(string variableCalcular, string variableRebuda, Dada dadaRebuda, uint tsActual)
        {
            var calculDada= new TFG.Protobuf.CalculDada()
            {
                VariableCalcular = variableCalcular,
                VariableRebuda = variableRebuda,
                ValorRebut=dadaRebuda.Valor,
                TimeStampRebut=dadaRebuda.Timestamp,
                TimeStampActual=tsActual
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
            while (!stoppingToken.IsCancellationRequested)
            {                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}