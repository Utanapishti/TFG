using GestorCalculs;
using Grpc.Core;
using Grpc.Core.Utils;
using TFG.Protobuf.GRPC;

namespace GestorDades
{
    internal class StatusServiceImpl : StatusService.StatusServiceBase
    {
        private ILogger<Worker> logger;
        private GestorFuncions gestorFuncions;

        public StatusServiceImpl(ILogger<Worker> logger, GestorFuncions gestorFuncions)
        {
            this.logger = logger;
            this.gestorFuncions = gestorFuncions;
        }

        public override Task GetStatus(PeticioStatus request, IServerStreamWriter<RespostaStatus> responseStream, ServerCallContext context)
        {
            return Task.Run(() =>
               responseStream.WriteAllAsync<RespostaStatus>(
                gestorFuncions.GetNomsVariables().Select(nomVariable =>
                {
                    Dada? dada = gestorFuncions.DemanaUltimaDada(nomVariable);
                    return new RespostaStatus()
                    {
                        NomVariable = nomVariable,
                        Valor = dada?.Valor ?? 0,
                        Timestamp = dada?.Timestamp ?? 0
                    };
                })));
        }
    }
}