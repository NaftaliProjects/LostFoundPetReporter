using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;

namespace LostFoundPetReporter.Mobile.Services.Api
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TModel?> GetAsync<TModel>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TModel>();
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                endpoint,
                request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task PutAsync<TRequest>(
            string endpoint,
            TRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(
                endpoint,
                request);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);

            response.EnsureSuccessStatusCode();
        }
    }
}
