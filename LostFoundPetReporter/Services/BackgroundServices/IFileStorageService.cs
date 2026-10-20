namespace LostFoundPetReporter.API.Services.BackgroundServices
{



    public interface IFileStorageService
    {
        public record StoredFileInfo(string FilePath, string FileName, string FileType);


        Task<StoredFileInfo> SaveBase64Async(
            string base64,
            string fileName,
            CancellationToken cancellationToken = default);

        string ConvertToBase64(string filePath);
    }
}
