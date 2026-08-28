using static LostFoundPetReporter.API.Services.BackgroundServices.IFileStorageService;

namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class FileStorageService : IFileStorageService
    {

        public async Task<StoredFileInfo> SaveBase64Async(
    string base64,
    string fileName,
    CancellationToken cancellationToken = default)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            string filePath = Path.Combine("uploads", fileName);

            Directory.CreateDirectory("uploads");

            await File.WriteAllBytesAsync(
                filePath,
                bytes,
                cancellationToken);

            return new StoredFileInfo(
                filePath,
                fileName,
                "image/jpeg");
        }

        public string ConvertToBase64(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            byte[] bytes = File.ReadAllBytes(filePath);

            return Convert.ToBase64String(bytes);

            
        }
    }
}
