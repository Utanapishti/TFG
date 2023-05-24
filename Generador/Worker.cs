using Messaging;
using Microsoft.Extensions.Options;
using MockedSensor;
using Protocol;

namespace Generador
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        Publisher _publisher;
        DataGenerator _dataGenerator;
        TimeSpan _delay;

        public Worker(ILogger<Worker> logger, Publisher publisher, IOptions<GeneradorOptions> options)
        {
            _logger = logger;
            _publisher = publisher;
            _dataGenerator = new DataGenerator(options.Value.Values);
            _delay = options.Value.Interval;
            _publisher.CreatePublisher();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var valor = _dataGenerator.GetValue();
                _publisher.Publish(Protocol.Protocol.GeneratePayload(new TipusDades.DadaGenerada(valor, DateTime.Now)));
                await Task.Delay(_delay, stoppingToken);
            }
        }
    }
}