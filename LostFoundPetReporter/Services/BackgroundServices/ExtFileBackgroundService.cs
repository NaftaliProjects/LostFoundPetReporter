namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class ExtFileBackgroundService : BackgroundService
    {
        private readonly IExtFileQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public ExtFileBackgroundService(
            IExtFileQueue queue,
            IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var extFileService = scope.ServiceProvider.GetRequiredService<IExtFileService>();


                    await extFileService.ProcessFilesAsync(
                        task.ReportId,
                        task.Type,
                        task.pictureBase64List,
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    // Log exception
                }
            }
        }
    }
}
