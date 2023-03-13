using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.HostedServices;

public class IndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;

    public IndexCreationService(RedisConnectionProvider provider)
        => _provider = provider;

    public Task StartAsync(CancellationToken cancellationToken)
        => _provider.Connection.CreateIndexAsync(typeof(Person));

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
