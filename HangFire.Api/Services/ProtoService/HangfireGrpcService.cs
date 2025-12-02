namespace HangFire.Api.Services.ProtoService;

using SharedHangFire;
using Google.Protobuf.WellKnownTypes;

public interface IHangfireGrpcService
{
    Task<string> GetConnectionStringAsync();
}

public class HangfireGrpcService : IHangfireGrpcService
{
    private readonly HangFireServices.HangFireServicesClient _client;

    public HangfireGrpcService(HangFireServices.HangFireServicesClient client)
    {
        _client = client;
    }

    public async Task<string> GetConnectionStringAsync()
    {
        var reply = await _client.GetConnectionStringAsync(new Empty());
        return reply.ConnectionString;
    }
}
