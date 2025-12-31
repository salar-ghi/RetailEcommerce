namespace Infrastructure.Persistence;

public class DatabaseSeederHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseSeederHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
