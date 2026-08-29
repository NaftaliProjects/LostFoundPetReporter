using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.API.Services.API;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LostFoundPetReporter.Services.API
{
    public class AnimalDescriptionService : IAnimalDescriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AnimalDescriptionService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<AnimalDescriptionDto> ImageToAnimalDescriptionAsync(
            List<string> pictureBase64List,
            CancellationToken cancellationToken = default)
        {
            // Gemini implementation here
        }
    }
}