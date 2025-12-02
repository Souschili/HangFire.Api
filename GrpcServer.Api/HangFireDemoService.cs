using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SharedHangFire;

namespace GrpcServer.Api
{
    public class HangFireDemoService: HangFireServices.HangFireServicesBase
    {
        public override Task<ConnectionStringResponse> GetConnectionString(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new ConnectionStringResponse
            {
                ConnectionString = "Server=.;Database=HangFire;User Id=sa;Password=pass;"
            });
        }
    }
}
