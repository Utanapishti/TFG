using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFG.Protobuf.GRPC;

namespace Magatzem
{
    public class ValorServiceImpl : ValorService.ValorServiceBase
    {
        public readonly ILogger _logger;
        int i = 0;

        public ValorServiceImpl(ILogger logger)
        {
            _logger = logger;
        }

        public override Task<RespostaPeticioValor> Valor(PeticioValor request, ServerCallContext context)
        {
            i++;
            return Task.FromResult(new RespostaPeticioValor()
            {
                Valor = i,
                TimestampRebut = (uint)i
            });            
        }
    }
}
