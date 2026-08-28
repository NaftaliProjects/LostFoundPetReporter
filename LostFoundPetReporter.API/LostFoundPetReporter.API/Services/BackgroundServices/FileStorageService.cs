using static LostFoundPetReporter.API.Services.BackgroundServices.IFileStorageService;

namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class FileStorageService : IFileStorageService
    {

        public async Task<StoredFileInfo> SaveBase64Async(string base64, CancellationToken cancellationToken = default)
        {
            // Parse Base64
            // Determine file type
            // Generate filename
            // Convert Base64 → byte[]
            // Write bytes to disk

            byte[] bytes = Convert.FromBase64String(base64);

            string fileName = $"{Guid.NewGuid()}.jpg";
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
