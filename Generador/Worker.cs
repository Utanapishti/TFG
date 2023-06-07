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
        TimeSpan _delay;
        string _name;

        public Worker(ILogger<Worker> logger, IOptions<ConnectionOptions> connectionOptions, IOptions<GeneradorOptions> options)
        {
            _logger = logger;
            _publisher = new RabbitMQConnection(logger, connectionOptions);            
            _dataGenerator = new DataGenerator(options.Value.Values);
            _delay = options.Value.Interval;
            _name = options.Value.Name;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _publisher.Connect(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {                
                var valor = _dataGenerator.GetValue();
                _logger.LogInformation("Value {valor} generated at: {time}", valor, DateTimeOffset.Now);
                _publisher.Publish(ProtocolDadaGenerada.GeneratePayload(new TipusDades.DadaGenerada(_name, valor)));
                await Task.Delay(_delay, stoppingToken);
            }
        }
    }
}