using GestorCalculs;
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
        private readonly GestorFuncions _gestorFuncions;
        private readonly ILogger _logger;
        int i = 0;

        public ValorServiceImpl(ILogger logger, GestorFuncions gestorFuncions)
        {
            _gestorFuncions = gestorFuncions;
            _logger = logger;
        }

        public override Task<RespostaPeticioValor> Valor(PeticioValor request, ServerCallContext context)
        {
            var dada=_gestorFuncions.DemanaUltimaDada(request.NomVariable);
            if (dada != null)
            {
                return Task.FromResult(new RespostaPeticioValor()
                {
                    Valor = dada.Valor,
                    TimestampRebut = dada.Timestamp
                });
            }
            else
            {
                _logger.LogError($"Petition received for {request.NomVariable}. Data not found");
                return (Task<RespostaPeticioValor>)Task.FromException(new KeyNotFoundException(request.NomVariable));
            }
        }
    }
}
