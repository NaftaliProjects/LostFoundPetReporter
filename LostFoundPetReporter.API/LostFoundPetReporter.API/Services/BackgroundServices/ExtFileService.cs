using LostFoundPetReporter.CoreDb.ReposInterfaces;

namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class ExtFileService : IExtFileService
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IFoundReportRepo _repo;

        public ExtFileService(
            IFileStorageService fileStorage,
            IFoundReportRepo repo)
        {
            _fileStorage = fileStorage;
            _repo = repo;
        }

        public async Task ProcessFilesAsync(int reportId, ReportType type, List<string> pictureBase64List, CancellationToken cancellationToken = default)
        {
            var entity = _repo.Find(reportId);

            if (entity == null)
                return;

            for (int i = 0; i < pictureBase64List.Count; i++)
            {
                string fileName = $"foundReport{DateTime.Now:ddMMyyHHmmss}_{i}.jpg";


                var base64 = pictureBase64List[i];

                var storedFile =
                    await _fileStorage.SaveBase64Async(
                        base64,
                        fileName,
                        cancellationToken);

                var extFile = new FoundReportExtFile
                {
                    FoundReportId = reportId,
                    FilePath = storedFile.FilePath,
                    FileName = storedFile.FileName,
                    Description = storedFile.FileType
                };

                entity.FoundReportExtFilesNevigation.Add(extFile);
            }

            // Save the report + newly added files
            _repo.SaveChanges();
        }
    }
}
