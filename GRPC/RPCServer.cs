using Grpc.Core;
using Grpc.Core.Logging;
using GRPCServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFG.Protobuf.GRPC;

namespace GRPC
{
    public class RPCServer
    {
        private Server _server;

        public RPCServer(ValorService.ValorServiceBase valorServiceImpl)
        {
            _server = new Server()
            {
                Services = { ValorService.BindService(valorServiceImpl) },
                Ports = { new ServerPort("localhost", 5098, ServerCredentials.Insecure) }
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
