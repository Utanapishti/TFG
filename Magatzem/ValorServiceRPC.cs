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
        public readonly ILogger _logger;
        private readonly GestorFuncions gestorFuncions;
        int i = 0;

        public ValorServiceImpl(ILogger logger, GestorCalculs.GestorFuncions gestorFuncions)
        {
            _logger = logger;
            this.gestorFuncions = gestorFuncions;
        }

        public override Task<RespostaPeticioValor> Valor(PeticioValor request, ServerCallContext context)
        {
            _logger.Log(LogLevel.Information, $"Received petition for value {request.NomVariable}");

            var valorResposta = gestorFuncions.DemanaUltimaDada(request.NomVariable);

            if (valorResposta != null) {
                if (valorResposta.Timestamp <= request.TimestampValor)
                {
                    return Task.FromResult(new RespostaPeticioValor()
                    {
                        Valor = valorResposta.Valor,
                    });
                }
                else return (Task<RespostaPeticioValor>)Task<RespostaPeticioValor>.FromException(new Exception($"Timestamp requested for {request.NomVariable}: {request.TimestampValor}. Only {valorResposta.Timestamp} available"));
            }
            else
            {
                return (Task<RespostaPeticioValor>)Task<RespostaPeticioValor>.FromException(new KeyNotFoundException($"Variable {request.NomVariable} not found"));
            }
        }
    }
}
