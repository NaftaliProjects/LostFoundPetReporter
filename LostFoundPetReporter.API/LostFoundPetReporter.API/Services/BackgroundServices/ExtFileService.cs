using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;

namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class ExtFileService : IExtFileService
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IFoundReportRepo _foundRepo;
        private readonly ILostReportRepo _lostRepo;

        public ExtFileService(
            IFileStorageService fileStorage,
            IFoundReportRepo foundRepo,
            ILostReportRepo lostRepo)
        {
            _fileStorage = fileStorage;
            _foundRepo = foundRepo;
            _lostRepo = lostRepo;
        }

        public async Task ProcessFilesAsync(
            int reportId,
            ReportType type,
            List<string> pictureBase64List,
            CancellationToken cancellationToken = default)
        {
            if (type == ReportType.Found)
            {
                await ProcessFilesAsync(
                    reportId,
                    pictureBase64List,
                    _foundRepo,
                    type,
                    cancellationToken);
            }
            else
            {
                await ProcessFilesAsync(
                    reportId,
                    pictureBase64List,
                    _lostRepo,
                    type,
                    cancellationToken);
            }
        }

        private async Task ProcessFilesAsync<TReport>(
            int reportId,
            List<string> pictureBase64List,
            IBaseRepo<TReport> repo,
            ReportType type,
            CancellationToken cancellationToken)
            where TReport : BaseModel, new()
        {
            var entity = repo.Find(reportId);

            if (entity == null)
                return;

            for (int i = 0; i < pictureBase64List.Count; i++)
            {
                string fileName =
                    $"report{DateTime.Now:ddMMyyHHmmss}_{i}.jpg";

                var storedFile = await _fileStorage.SaveBase64Async(
                    pictureBase64List[i],
                    fileName,
                    cancellationToken);

                if (type == ReportType.Found)
                {
                    var foundReport = (FoundReport)(object)entity;

                    foundReport.FoundReportExtFilesNevigation.Add(
                        new FoundReportExtFile
                        {
                            FoundReportId = reportId,
                            FilePath = storedFile.FilePath,
                            FileName = storedFile.FileName,
                            Description = storedFile.FileType
                        });
                }
                else
                {
                    var lostReport = (LostReport)(object)entity;

                    lostReport.LostReportExtFilesNevigation.Add(
                        new LostReportExtFile
                        {
                            LostReportId = reportId,
                            FilePath = storedFile.FilePath,
                            FileName = storedFile.FileName,
                            Description = storedFile.FileType
                        });
                }
            }

            repo.SaveChanges();
        }
    }
}