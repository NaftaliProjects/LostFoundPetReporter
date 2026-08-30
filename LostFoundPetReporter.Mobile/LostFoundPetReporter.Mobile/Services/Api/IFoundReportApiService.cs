using LostFoundPetReporter.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Api
{
    public interface IFoundReportApiService
    {
        Task<IEnumerable<FoundReport>?> GetFoundReportsAsync();

        Task<FoundReport?> GetFoundReportAsync(int id);
        Task<IEnumerable<FoundReport>?> GetFoundReportByUserIdAsync(int id);

        Task<FoundReport?> CreateFoundReportAsync(CreateFoundReportRequest request);

        Task UpdateFoundReportAsync(int id, CreateFoundReportRequest request);

        Task DeleteFoundReportAsync(int id);

        Task<CreateAnimalDescription?> ImageToAnimalDescriptionAsync(List<string> pictureBase64List);



    }
}
