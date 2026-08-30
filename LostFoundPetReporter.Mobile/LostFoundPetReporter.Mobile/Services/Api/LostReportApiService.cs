using LostFoundPetReporter.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Api
{
    public class LostReportApiService : ILostReportApiService
    {
        private readonly IApiClient _apiClient;

        public LostReportApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<LostReport>?> GetLostReportsAsync()
        {
            return await _apiClient.GetAsync<IEnumerable<LostReport>>(
                "api/v1/LostReport");
        }

        public async Task<IEnumerable<LostReport>?> GetLostReportByUserIdAsync(int id)
        {
            return await _apiClient.GetAsync<IEnumerable<LostReport>>(
                $"api/v1/LostReport/ByUser/{id}");
        }

        public async Task<LostReport?> GetLostReportAsync(int id)
        {
            return await _apiClient.GetAsync<LostReport>(
                $"api/v1/LostReport/{id}");
        }

        public async Task<LostReport?> CreateLostReportAsync(
            CreateLostReportRequest request)
        {
            return await _apiClient.PostAsync<CreateLostReportRequest, LostReport>(
                "api/v1/LostReport",
                request);
        }

        public async Task UpdateLostReportAsync(
            int id,
            CreateLostReportRequest request)
        {

            request.Id ??= id;

            await _apiClient.PutAsync(
                $"api/v1/LostReport/{id}",
                request);
        }

        public async Task DeleteLostReportAsync(int id)
        {
            await _apiClient.DeleteAsync(
                $"api/v1/LostReport/{id}");
        }

        public async Task<CreateAnimalDescription?> ImageToAnimalDescriptionAsync(List<string> pictureBase64List)
        {
            var request = new ImageToAnimalDescriptionRequest
            {
                PictureBase64List = pictureBase64List
            };

            return await _apiClient.PostAsync<
                ImageToAnimalDescriptionRequest,
                CreateAnimalDescription>(
                "api/v1/LostReport/ImageToAnimalDescription",
                request);
        }

    }
}
