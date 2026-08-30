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

                       Return a single JSON object describing the animal visible in the image(s).

                       Use the following fields:

                       * name: The animal's name if it can be determined from the image or provided context. Otherwise use an empty string.
                       * type: The general animal type, such as "dog", "cat", "bird", "rabbit", etc.
                       * breed: The animal's breed if it can reasonably be identified from its visible characteristics. Otherwise use an empty string.
                       * colors: The main visible colors of the animal. If there are multiple colors, separate them with commas, for example "white,black,brown".
                       * sex: "Male" or "Female" only if it can reasonably be determined from the image. Otherwise use an empty string.
                       * age: Approximate age in years only if it can reasonably be estimated from the image. Otherwise use null.
                       * size: "Small", "Medium", or "Large" based on the apparent physical size of the animal. If it cannot reasonably be determined, use an empty string.
                       * weightKg: Approximate weight in kilograms only if it can reasonably be estimated. Otherwise use null.
                       * coatLength: "Short", "Medium", or "Long" based on the visible fur length. Otherwise use an empty string.
                       * coatType: Describe the visible coat type, such as "Straight", "Curly", "Wavy", or "Wire". Otherwise use an empty string.
                       * pattern: Describe the visible coat pattern, such as "Solid", "Spotted", "Striped", "Tabby", "Tuxedo", or "Mixed". Otherwise use an empty string.
                       * distinctiveMarkings: Describe clearly visible unique markings that could help identify the animal, such as "white patch on chest", "black spot above left eye", or "white paws". Otherwise use an empty string.
                       * eyeColor: The visible eye color, such as "Brown", "Blue", "Green", or "Amber". Otherwise use an empty string.
                       * earDescription: Describe clearly visible distinctive ear characteristics, such as "left ear folded", "both ears upright", or "one floppy ear". Otherwise use an empty string.
                       * tailDescription: Describe clearly visible distinctive tail characteristics, such as "long tail with white tip", "short tail", or "curled tail". Otherwise use an empty string.
                       * collarPresent: true if a collar is clearly visible, false if no collar is clearly visible, or null if the image does not allow this to be determined.
                       * collarColor: The visible collar color. Otherwise use an empty string.
                       * collarType: The visible collar type, such as "standard collar", "reflective collar", or "leather collar". Otherwise use an empty string.
                       * harnessPresent: true if a harness is clearly visible, false if no harness is clearly visible, or null if it cannot be determined.
                       * harnessColor: The visible harness color. Otherwise use an empty string.

                       Important rules:

                       1. Do not invent or guess information that cannot reasonably be determined from the image.
                       2. Prefer an empty string or null over an uncertain answer.
                       3. Only describe characteristics that are actually visible.
                       4. If multiple images show the same animal, combine the information from all images into the same JSON object.
                       5. Assume all provided images represent one animal.
                       6. Return ONLY one valid JSON object.
                       7. Do not include Markdown, code fences, explanations, or additional text.

                       Return JSON in exactly this structure:

                       {
                       "name": "",
                       "type": "",
                       "breed": "",
                       "colors": "",
                       "sex": "",
                       "age": null,
                       "size": "",
                       "weightKg": null,
                       "coatLength": "",
                       "coatType": "",
                       "pattern": "",
                       "distinctiveMarkings": "",
                       "eyeColor": "",
                       "earDescription": "",
                       "tailDescription": "",
                       "collarPresent": null,
                       "collarColor": "",
                       "collarType": "",
                       "harnessPresent": null,
                       "harnessColor": ""
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
