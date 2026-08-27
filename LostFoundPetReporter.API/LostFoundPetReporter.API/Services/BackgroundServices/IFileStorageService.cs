namespace LostFoundPetReporter.API.Services.BackgroundServices
{



    public interface IFileStorageService
    {
        public record StoredFileInfo(string FilePath, string FileName, string FileType);


        Task<StoredFileInfo> SaveBase64Async(
            string base64,
            CancellationToken cancellationToken = default);
    }
}
