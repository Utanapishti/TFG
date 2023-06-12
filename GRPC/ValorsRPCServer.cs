using Grpc.Core;
using Grpc.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFG.Protobuf.GRPC;

namespace GRPC
{
    public class ValorsRPCServer
    {
        private Server _server;

        public ValorsRPCServer(ValorService.ValorServiceBase valorServiceImpl, StatusService.StatusServiceBase statusServiceImpl)
        {
            _server = new Server()
            {
                Services = { 
                    ValorService.BindService(valorServiceImpl), 
                    StatusService.BindService(statusServiceImpl),
                    Grpc.Health.V1.Health.BindService(new HealthService()) 
                },
                Ports = { new ServerPort("0.0.0.0", 5098, ServerCredentials.Insecure) }
            };                        
        }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            _server.KillAsync();
        }
    }
}
