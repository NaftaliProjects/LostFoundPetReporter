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
            
            if (pictureBase64List == null || pictureBase64List.Count == 0)
            {
                throw new ArgumentException("At least one image is required.", nameof(pictureBase64List));
            }

            var apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Gemini API key is not configured.");
            }

            var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent";


            var parts = new List<object>();

            // Prompt
            parts.Add(new
            {
                text = """
                       Analyze the provided image(s) of an animal.

                       Return information about the animal using the following fields:

                       - name: The animal's name if it can be determined. Otherwise use an empty string.
                       - colors: Describe the animal's main colors, can be concated with ',' for example : red,blue,green.
                       - type: The general animal type, such as dog, cat, bird, rabbit, etc.
                       - breed: The animal's breed if it can reasonably be determined. Otherwise use an empty string.

                       Do not invent information that cannot reasonably be determined from the images.
                       Return ONLY valid JSON in this exact format:
                       And Return ONLY ONE JSON Assuming each image is about one animal

                       {
                         "name": "",
                         "colors": "",
                         "type": "",
                         "breed": ""
                       }
                       """
            });

           


            // Images
            foreach (var base64Image in pictureBase64List)
            {
                var imageData = RemoveBase64Prefix(base64Image);

                parts.Add(new
                {
                    inline_data = new
                    {
                        mime_type = "image/jpeg",
                        data = imageData
                    }
                });
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                url);

            request.Headers.Add("X-goog-api-key", apiKey);

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(
                request,
                cancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Gemini API returned {(int)response.StatusCode} " +
                    $"{response.StatusCode}: {responseBody}");
            }

            // Parse Gemini response
            using var document = JsonDocument.Parse(responseBody);

            var generatedText =
                document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

            if (string.IsNullOrWhiteSpace(generatedText))
            {
                throw new InvalidOperationException(
                    "Gemini returned an empty response.");
            }

            var cleanJson = CleanJsonResponse(generatedText);


            Console.WriteLine("===== GEMINI GENERATED TEXT =====");
            Console.WriteLine(generatedText);
            Console.WriteLine("");
            Console.WriteLine(cleanJson);
            Console.WriteLine("=================================");


            // Convert Gemini JSON into our DTO
            var animalDescription =
                JsonSerializer.Deserialize<AnimalDescriptionDto>(
                    cleanJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (animalDescription == null)
            {
                throw new InvalidOperationException(
                    "Failed to deserialize Gemini response.");
            }

            return animalDescription;
        }

        private static string RemoveBase64Prefix(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                return base64;

            var commaIndex = base64.IndexOf(',');

            if (commaIndex >= 0)
            {
                return base64[(commaIndex + 1)..];
            }

            return base64;
        }


        private static string CleanJsonResponse(string response)
        {
            response = response.Trim();

            if (response.StartsWith("```json"))
            {
                response = response["```json".Length..];
            }
            else if (response.StartsWith("```"))
            {
                response = response["```".Length..];
            }

            if (response.EndsWith("```"))
            {
                response = response[..^3];
            }

            return response.Trim();
        }



    }
}
