using GRPC;
using Messaging;
using Microsoft.Extensions.Options;
using MockedSensor;
using Protocol;

namespace Generador
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        RabbitMQConnection _publisher;
        DataGenerator _dataGenerator;
        int _delay;
        string _name;
        HealthRPCServer _rpcServer;

        public Worker(ILogger<Worker> logger, IOptions<ConnectionOptions> connectionOptions, IOptions<GeneradorOptions> options)
        {
            _logger = logger;
            _publisher = new RabbitMQConnection(logger, connectionOptions);            
            _dataGenerator = new DataGenerator(options.Value.Values);
            _delay = options.Value.Interval;
            _name = options.Value.Name;
            _rpcServer = new HealthRPCServer();
            _rpcServer.Start();
            HealthService.Status = ServingStatus.NOT_SERVING;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _publisher.Connect(stoppingToken);
            HealthService.Status = ServingStatus.SERVING;
            while (!stoppingToken.IsCancellationRequested)
            {                
                var valor = _dataGenerator.GetValue();
                _logger.LogInformation("Value {valor} generated at: {time}", valor, DateTimeOffset.Now);
                _publisher.Publish(ProtocolDadaGenerada.GeneratePayload(new TipusDades.DadaGenerada(_name, valor)));
                await Task.Delay(_delay, stoppingToken);
            }
            HealthService.Status = ServingStatus.NOT_SERVING;
        }
    }
}