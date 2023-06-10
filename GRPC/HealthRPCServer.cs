using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRPC
{
    public class HealthRPCServer
    {
        private Server _server;

        public HealthRPCServer()
        {
            _server = new Server()
            {
                Services = { Grpc.Health.V1.Health.BindService(new HealthService()) },
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
