using LostFoundPetReporter.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Api
{
    public class FoundReportApiService : IFoundReportApiService
    {
        private readonly IApiClient _apiClient;

        public FoundReportApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<FoundReport>?> GetFoundReportsAsync()
        {
            return await _apiClient.GetAsync<IEnumerable<FoundReport>>(
                "api/v1/FoundReport");
        }

        public async Task<IEnumerable<FoundReport>?> GetFoundReportByUserIdAsync(int id)
        {
            return await _apiClient.GetAsync<IEnumerable<FoundReport>>(
                $"api/v1/FoundReport/ByUser/{id}");
        }

        public async Task<FoundReport?> GetFoundReportAsync(int id)
        {
            return await _apiClient.GetAsync<FoundReport>(
                $"api/v1/FoundReport/{id}");
        }

        public async Task<FoundReport?> CreateFoundReportAsync(
            CreateFoundReportRequest request)
        {
            return await _apiClient.PostAsync<CreateFoundReportRequest, FoundReport>(
                "api/v1/FoundReport",
                request);
        }

        public async Task UpdateFoundReportAsync(
            int id,
            CreateFoundReportRequest request)
        {

            request.Id ??= id;

            await _apiClient.PutAsync(
                $"api/v1/FoundReport/{id}",
                request);
        }

        public async Task DeleteFoundReportAsync(int id)
        {
            await _apiClient.DeleteAsync(
                $"api/v1/FoundReport/{id}");
        }

        public async Task<AnimalDescription?> ImageToAnimalDescriptionAsync(List<string> pictureBase64List)
        {
            var request = new ImageToAnimalDescriptionRequest
            {
                PictureBase64List = pictureBase64List
            };

            return await _apiClient.PostAsync<
                ImageToAnimalDescriptionRequest,
                AnimalDescription>(
                "api/v1/FoundReport/ImageToAnimalDescription",
                request);
        }


    }
}
