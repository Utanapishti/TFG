using Grpc.Core;
using Grpc.Health.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRPC
{    
    public class HealthService:Health.HealthBase
    {
        //The health status is set to static
        //as it applies to the whole service
        public static ServingStatus Status { get; set; } = ServingStatus.UNKNOWN;

        public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HealthCheckResponse()
            {
                Status = (HealthCheckResponse.Types.ServingStatus)Status
            });
        }
    }

    public enum ServingStatus
    {
        UNKNOWN = 0,
        SERVING = 1,
        NOT_SERVING = 2,
        SERVICE_UNKNOWN = 3
    }

}
