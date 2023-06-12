using GestorCalculs;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFG.Protobuf.GRPC;

namespace GestorDades
{
    public class ValorServiceImpl : ValorService.ValorServiceBase
    {
        public readonly ILogger _logger;
        private readonly GestorFuncions gestorFuncions;
        int i = 0;

        public ValorServiceImpl(ILogger logger, GestorFuncions gestorFuncions)
        {
            _logger = logger;
            this.gestorFuncions = gestorFuncions;
        }

        public override Task<RespostaPeticioValor> Valor(PeticioValor request, ServerCallContext context)
        {
            _logger.Log(LogLevel.Information, $"Received petition for value {request.NomVariable}");

            if (String.IsNullOrEmpty(request.NomVariable))
            {
                return Task.FromResult(new RespostaPeticioValor()
                {
                    Correcte = false
                }); ;
            }

            var valorResposta = gestorFuncions.DemanaUltimaDada(request.NomVariable);

            if (valorResposta != null) {
                if (valorResposta.Timestamp <= request.TimestampValor)
                {
                    return Task.FromResult(new RespostaPeticioValor()
                    {
                        Valor = valorResposta.Valor,                        
                        Correcte=true,
                    });
                }
                else
                {
                    string errorMessage = $"Timestamp requested for {request.NomVariable}: {request.TimestampValor}. Only {valorResposta.Timestamp} available";
                    _logger.LogError(errorMessage);
                    return Task.FromResult<RespostaPeticioValor>(new RespostaPeticioValor()
                    {
                        Correcte=false
                    });
                }
                
            }
            else
            {
                string errorMessage = $"Variable {request.NomVariable} not found";
                _logger.LogError(errorMessage);
                return Task.FromResult<RespostaPeticioValor>(new RespostaPeticioValor()
                { Correcte = false });
            }
        }
    }
}
