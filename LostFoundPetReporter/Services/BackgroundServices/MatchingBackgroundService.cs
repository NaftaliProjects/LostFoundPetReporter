namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class MatchingBackgroundService : BackgroundService
    {
        private readonly IMatchingQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public MatchingBackgroundService(IMatchingQueue queue, IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var matchingService = scope.ServiceProvider.GetRequiredService<IMatchingService>();

                    if (task.Type == ReportType.Found)
                    {
                        await matchingService.TryMatchFoundReportAsync(task.ReportId, stoppingToken);
                    }
                    else
                    {
                        await matchingService.TryMatchLostReportAsync(task.ReportId, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    // Log exception
                }
            }
        }
    }
}
