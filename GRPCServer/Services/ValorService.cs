using Grpc.Core;
using TFG.Protobuf.GRPC;

namespace GRPCServer.Services
{
    public class ValorService: TFG.Protobuf.GRPC.ValorService.ValorServiceBase
    {
        public readonly ILogger _logger;
        int i = 0;

        public ValorService(ILogger<ValorService> logger)
        {
            _logger = logger;
        }

        public override Task<RespostaPeticioValor> Valor(PeticioValor request, ServerCallContext context)
        {
            return Task.FromResult(new RespostaPeticioValor()
            {
                Valor = i,
                TimestampRebut = (uint)i
            });
            i++;
        }
    }
}
