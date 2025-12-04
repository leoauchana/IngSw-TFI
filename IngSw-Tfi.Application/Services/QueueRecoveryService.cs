using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IngSw_Tfi.Application.Services;

public class QueueRecoveryService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public QueueRecoveryService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<IIncomeRepository>();
        var queueService = scope.ServiceProvider.GetRequiredService<IPriorityQueueService>();

        var incomesEarrings = await repo.GetAllEarrings();
        if (incomesEarrings == null || !incomesEarrings.Any())
        {
            return;
        }

        foreach (var income in incomesEarrings)
        {
            queueService.Enqueue(income);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
