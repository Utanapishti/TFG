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
        public RPCServer(ValorService.ValorServiceBase valorServiceImpl)
        {
            Server server = new Server()
            {
                Services = { ValorService.BindService(valorServiceImpl) },
                Ports = { new ServerPort("localhost", 5098, ServerCredentials.Insecure) }
            };            
            server.Start();
        }
    }
}
